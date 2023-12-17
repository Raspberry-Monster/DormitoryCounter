using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DormitoryCounter.Model
{
    public static partial class RegexCollections
    {
        [GeneratedRegex(@"[0-9]+")]
        public static partial Regex GetHistoryPageRegex();
    }
}
