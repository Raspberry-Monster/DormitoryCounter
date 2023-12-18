using System.Text.RegularExpressions;

namespace DormitoryCounter.Model
{
    public static partial class RegexCollections
    {
        [GeneratedRegex(@"[0-9]+")]
        public static partial Regex GetHistoryPageRegex();
        [GeneratedRegex(@"(\S*)(?=-\S*)")]
        public static partial Regex GetDormitoryString();
        [GeneratedRegex(@"(?!"")+(\S*)+(?="";/\*)")]
        public static partial Regex GetLoginInfo();

    }
}
