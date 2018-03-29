//------------------------------------------------------------------------
// Version		: 001
// Designer		: ChienBV
// Date			: 2018/01/23
// Comment		: Create new
//------------------------------------------------------------------------

using ShipOnline.UtilityService;
using System;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace ShipOnline.UtilityServices
{
    public static partial class DownloadUtil
    {
        public static void AddHeaderContentDisposition(HttpRequestBase request, HttpResponseBase response,
            string outputFileName)
        {
            AddHeaderContentDisposition(response, outputFileName, request.Browser.Browser, request.Browser.MajorVersion, request.UserAgent);
        }

        public static void AddHeaderContentDisposition(HttpRequest request, HttpResponseBase response,
            string outputFileName)
        {
            AddHeaderContentDisposition(response, outputFileName, request.Browser.Browser, request.Browser.MajorVersion, request.UserAgent);
        }

        private static void AddHeaderContentDisposition(HttpResponseBase response, string outputFileName, string browser, int majorVersion, string userAgent)
        {
            string encodedFileName = HttpUtility.UrlPathEncode(outputFileName);
            encodedFileName = encodedFileName.Replace("+", "%20");

            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetExpires(Utility.GetCurrentDateTime());
            response.Cache.SetMaxAge(new TimeSpan(0, 0, 0, 0));

            if (browser.ToUpper().IndexOf("IE") >= 0 || userAgent.Contains("Trident"))
            {
                // IE8 以下（RFC 6266 未サポート）
                if (majorVersion < 9)
                {
                    response.AppendHeader("Content-Disposition",
                        "attachment;filename=\"" +
                        encodedFileName.Replace("%20", " ") + "\"");
                }
                // IE9 以上（RFC 6266 サポート）
                else
                {
                    response.AppendHeader("Content-Disposition",
                        "attachment;filename*=utf-8''" + encodedFileName);
                }
            }
            // IE 以外
            else
            {
                // Safari 5.1.7 がまだ RFC 6266 未対応らしいので 
                // filename="xxxxx" の併記が必要。 
                response.AppendHeader("Content-Disposition",
                    "attachment;filename=\"" + outputFileName +
                    "\";filename*=utf-8''" + encodedFileName);
            }
        }


    }
}
