using HtmlAgilityPack;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DormitoryCounter.Model
{
    public class PointHistory
    {
        [ExcelColumnName("姓名")]
        public string Name { get; set; }
        [ExcelColumnName("宿舍")]
        public string Dormitory { get; set; }
        [ExcelColumnName("扣分类型")]
        public string HistoryType { get; set; }
        [ExcelColumnName("分数")]
        public double Point { get; set; }
        [ExcelColumnName("日期")]
        public DateOnly Date { get; set; }
        [ExcelColumnName("原因")]
        public string Content { get; set; }
        public PointHistory(HtmlNode node)
        {
            Name = node.SelectSingleNode("//body/form/div[3]/div/table/tr[3]/td[2]/span").InnerText;
            var regex = RegexCollections.GetDormitoryString();
            Dormitory = regex.Match(node.SelectSingleNode("//body/form/div[3]/div/table/tr[4]/td[2]/span").InnerText).Value;
            HistoryType = node.SelectSingleNode("//body/form/div[3]/div/table/tr[5]/td[2]/span").InnerText;
            Point = Convert.ToDouble(node.SelectSingleNode("//body/form/div[3]/div/table/tr[6]/td[2]/span").InnerText);
            Date = DateOnly.FromDateTime(Convert.ToDateTime(node.SelectSingleNode("//body/form/div[3]/div/table/tr[7]/td[2]/span").InnerText));
            Content = node.SelectSingleNode("//body/form/div[3]/div/table/tr[9]/td[2]/span").InnerText;
        }
        public static List<DormitoryHistory> GetDormitoryPoints(IEnumerable<PointHistory> histories)
        {
            var dict = new Dictionary<string, double>();
            foreach (var history in histories)
            {
                if (!dict.ContainsKey(history.Dormitory)) dict[history.Dormitory] = 0d;
                dict[history.Dormitory] += history.Point;
            }
            var result = new List<DormitoryHistory>();
            foreach (var item in dict)
            {
                result.Add(new DormitoryHistory { Dormitory = item.Key, Point = item.Value });
            }
            result = result.OrderBy(t => t.Point).ToList();
            return result;
        }
        public class DormitoryHistory
        {
            [ExcelColumnName("宿舍名")]
            public required string Dormitory { get; set; }
            [ExcelColumnName("分数")]
            public required double Point { get; set; }
        }
    }
}
