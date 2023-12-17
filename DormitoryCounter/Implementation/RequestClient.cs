using DormitoryCounter.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Windows;

namespace DormitoryCounter.Implementation
{
    public static class RequestClient
    {
        public static string GetDateString(this DateOnly date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        public static FormUrlEncodedContent CreateQueryHttpContent(Dictionary<string, string> webFormData, DateOnly startDate, DateOnly endDate)
        {
            var contentDictionary = new Dictionary<string, string>();
            contentDictionary.Add("__EVENTTARGET", string.Empty);
            contentDictionary.Add("__EVENTARGUMENT", string.Empty);
            contentDictionary.Add("__LASTFOCUS", string.Empty);
            contentDictionary.Add("__VIEWSTATE", webFormData["__VIEWSTATE"]);
            contentDictionary.Add("__VIEWSTATEGENERATOR", webFormData["__VIEWSTATEGENERATOR"]);
            contentDictionary.Add("__EVENTVALIDATION", webFormData["__EVENTVALIDATION"]);
            contentDictionary.Add("ddlStGrade", "0");
            contentDictionary.Add("ddlStClass", string.Empty);
            contentDictionary.Add("txbProjectDate1", startDate.GetDateString());
            contentDictionary.Add("txbProjectDate2", endDate.GetDateString());
            contentDictionary.Add("Button1", "查询");
            return new FormUrlEncodedContent(contentDictionary);
        }
        public static StringContent CreateLoginHttpContent(string userName, string passWord)
        {
            var user = new User()
            {
                UserName = userName,
                Password = passWord
            };
            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var serializeResult = JsonSerializer.Serialize(user, options);
            return new StringContent(serializeResult, Encoding.UTF8);
        }
        public async static Task<bool> Query(string userName, string passWord, DateOnly startDate, DateOnly endDate, string targetOutputFile)
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
                if (content != "\"LoginOK\";/*")
                {
                    MessageBox.Show("登录失败！请检查账号密码是否正确", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var histories = htmlDocument.DocumentNode.SelectNodes("//body/form[1]/div[3]/div[2]/div[2]/table[1]");
                if (histories is null) return true;
                var detailedIds = new List<Uri>();
                foreach(var history in histories)
                {
                    var phrase = history.GetAttributeValue("onclick",string.Empty);
                    var regex = RegexCollections.GetHistoryPageRegex();
                    var match = regex.Match(phrase);
                    if (!match.Success) continue;
                    detailedIds.Add(RequestTarget.WapRecordShow(match.Value));
                }
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
