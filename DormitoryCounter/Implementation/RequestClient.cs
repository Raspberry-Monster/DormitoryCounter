using DormitoryCounter.Model;
using HtmlAgilityPack;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace DormitoryCounter.Implementation
{
    public static class RequestClient
    {
        public readonly static JsonSerializerOptions DefaultJsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public static string GetDateString(this DateOnly date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        public static FormUrlEncodedContent CreateQueryHttpContent(Dictionary<string, string> webFormData, DateOnly startDate, DateOnly endDate)
        {
            var contentDictionary = new Dictionary<string, string>
            {
                { "__EVENTTARGET", string.Empty },
                { "__EVENTARGUMENT", string.Empty },
                { "__LASTFOCUS", string.Empty },
                { "__VIEWSTATE", webFormData["__VIEWSTATE"] },
                { "__VIEWSTATEGENERATOR", webFormData["__VIEWSTATEGENERATOR"] },
                { "__EVENTVALIDATION", webFormData["__EVENTVALIDATION"] },
                { "ddlStGrade", "0" },
                { "ddlStClass", string.Empty },
                { "txbProjectDate1", startDate.GetDateString() },
                { "txbProjectDate2", endDate.GetDateString() },
                { "Button1", "查询" }
            };
            return new FormUrlEncodedContent(contentDictionary);
        }
        public static StringContent CreateLoginHttpContent(string userName, string passWord)
        {
            var user = new User()
            {
                UserName = userName,
                Password = passWord
            };
            var serializeResult = JsonSerializer.Serialize(user, DefaultJsonSerializerOptions);
            return new StringContent(serializeResult, Encoding.UTF8);
        }
        public async static Task<bool> Query(string userName, string passWord, DateOnly startDate, DateOnly endDate, string targetOutputFile, bool orderByDescending)
        {
            try
            {
                var cookieManager = new CookieContainer();
                using var loginContent = CreateLoginHttpContent(userName, passWord);
                using var httpClientHandler = new HttpClientHandler()
                {
                    CookieContainer = cookieManager,
                    UseCookies = true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                using var httpClient = new HttpClient(httpClientHandler)
                {
                    BaseAddress = RequestTarget.APITarget,
                };
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");

                using var loginRequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = RequestTarget.LoginPage,
                    Content = loginContent
                };
                loginRequest.Headers.Add("X-AjaxPro-Method", "userLogin");
                using var loginResult = await httpClient.SendAsync(loginRequest);
                var content = await loginResult.Content.ReadAsStringAsync();
                var loginResultString = RegexCollections.GetLoginInfo().Match(content).Value;
                if (loginResultString != "LoginOK")
                {
                    MessageBox.Show(loginResultString, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                var wapRecordPageString = await httpClient.GetStringAsync(RequestTarget.WapRecordTodayPage);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(wapRecordPageString);
                var states = htmlDocument.DocumentNode.SelectNodes("//body/form/div/input");
                if (states is null) return false;
                var dict = new Dictionary<string, string>();
                foreach (var node in states)
                {
                    dict.Add(node.Id, node.GetAttributeValue("value", string.Empty));
                }
                using var queryContent = CreateQueryHttpContent(dict, startDate, endDate);
                using var queryRequest = new HttpRequestMessage()
                {
                    Content = queryContent,
                    Method = HttpMethod.Post,
                    RequestUri = RequestTarget.WapRecordTodayPage
                };
                using var queryResult = await httpClient.SendAsync(queryRequest);
                var queryResultContent = await queryResult.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(queryResultContent);
                var histories = htmlDocument.DocumentNode.SelectNodes("//body/form/div/div/div/table/tr/td/table/tr/td/a");
                if (histories is null)
                {
                    MessageBox.Show("恭喜! 目前无扣分记录", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
                var detailedIdUris = new List<Uri>();
                foreach (var history in histories)
                {
                    var phrase = history.GetAttributeValue("onclick", string.Empty);
                    var regex = RegexCollections.GetHistoryPageRegex();
                    var match = regex.Match(phrase);
                    if (!match.Success) continue;
                    detailedIdUris.Add(RequestTarget.WapRecordShow(match.Value));
                }
                var pointHistories = new List<PointHistory>();
                foreach (var detailedIdUri in detailedIdUris)
                {
                    var detailedResultContent = await httpClient.GetStringAsync(detailedIdUri);
                    htmlDocument.LoadHtml(detailedResultContent);
                    pointHistories.Add(new PointHistory(htmlDocument.DocumentNode));
                }
                var dormitoryPoints = PointHistory.GetDormitoryPoints(pointHistories, orderByDescending);
                var personPoints = PointHistory.GetPersonPoints(pointHistories, orderByDescending);
                var result = new Dictionary<string, object>
                {
                    { "扣分记录", pointHistories },
                    { "宿舍扣分", dormitoryPoints },
                    {"单人扣分", personPoints }
                };
                MiniExcel.SaveAs(targetOutputFile, result, overwriteFile: true);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
