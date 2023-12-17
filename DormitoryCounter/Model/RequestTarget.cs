using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DormitoryCounter.Model
{
    public static class RequestTarget
    {
        public static readonly Uri APITarget = new ("http://neteaching.net/gdzsjnzx/");
        public static readonly Uri LoginPage = new ("ajaxpro/Login,App_Web_login.aspx.cdcab7d2.ashx", UriKind.Relative);
        public static readonly Uri WapRecordTodayPage = new ("wapRecordToday.aspx", UriKind.Relative);
        public static Uri WapRecordShow(string id) { return new($"wapRecordShow.aspx?id={id}", UriKind.Relative); }
    }
}
