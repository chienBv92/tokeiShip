using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShipOnline.Models.System;
using ShipOnline.Services;
using ShipOnline.UtilityService;
using ShipOnline.Models;
using System.Web.Routing;
using ShipOnline.Models.Define;
using ShipOnline.Resources;

namespace ShipOnline.Controllers
{
    //[Authorize]
    public class BaseController : Controller
    {
        private CmnEntityModel cmnEntityModel = null;
        private const string SESSION_SITEMAP = "SESSION_SITEMAP";
        // GET: Base

        public CmnEntityModel CmnEntityModel
        {
            get
            {
                if (cmnEntityModel == null)
                {
                    if (CacheUtil.GetCache<CmnEntityModel>("CmnEntityModel") == null)
                    {
                        CacheUtil.SaveCache("CmnEntityModel", new CmnEntityModel());
                    }
                    cmnEntityModel = (CmnEntityModel)CacheUtil.GetCache<CmnEntityModel>("CmnEntityModel");
                }
                return cmnEntityModel;
            }
        }

        // check session when redirect
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Request.IsAjaxRequest())
            {
                var routeData = filterContext.RouteData;
                var controller = routeData.Values["controller"].ToString();
                var action = routeData.Values["action"].ToString();

                var check = ((controller != "UserAccount" && action != "Login") && (controller != "Home" && action != "Index") && (controller != "Home" && action != "Intro") && (controller != "PDFManage" && action != "ViewPdf") && (controller != "Home" && action != "SupportCenter") && (controller != "Common" && action != "AuthentTimeout"));
                var sessionLogin = Session["CmnEntityModel"] as CmnEntityModel;
                if ((sessionLogin == null || sessionLogin.USER_ID == 0) && check)
                {
                    filterContext.Result = new RedirectToRouteResult(
                           new RouteValueDictionary(new { controller = "UserAccount", action = "Login" }));
                }

                int pos = Sitemap.FindIndex(item => item.ControllerName == controller && item.ActionName == action);

                if (0 <= pos)
                {
                    Sitemap.RemoveRange(pos, 0);
                }
                else
                {
                    var item = new SitemapItem
                    {
                        ControllerName = controller,
                        ActionName = action,
                        RestoreData = null
                    };

                    Sitemap.Insert(0, item);
                }
                base.OnActionExecuting(filterContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected object GetRestoreData()
        {
            object data = null;

            //var area = RouteData.DataTokens["area"].ToString();
            var controller = RouteData.Values["controller"].ToString();
            var action = RouteData.Values["action"].ToString();

            int pos = Sitemap.FindIndex(item=>item.ControllerName == controller && item.ActionName == action);

            if (0 <= pos)
            {
                data = Sitemap[pos].RestoreData;
                //Sitemap[pos].RestoreData = null;
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected void SaveRestoreData(object data)
        {
            if (data != null)
            {
                //var area = RouteData.DataTokens["area"].ToString();
                var controller = RouteData.Values["controller"].ToString();
                var action = RouteData.Values["action"].ToString();

                int pos = Sitemap.FindIndex(item => item.ControllerName == controller && item.ActionName == action);

                if (0 <= pos)
                {
                    Sitemap[pos].RestoreData = data;
                }
            }
        }

        private string GetWindowName()
        {
            var controller = RouteData.Values["controller"].ToString();

            string windowName = WindowName.MAIN;

            HttpCookie cookie = Request.Cookies[WindowName.COOKIE_NAME];
            if (cookie != null)
            {
                if (WindowName.Items.Contains(controller))
                {
                    cookie.Value = WindowName.Items[controller] as string;
                }

                windowName = cookie.Value;
            }

            return windowName;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<SitemapItem> Sitemap
        {
            get
            {
                var windowName = GetWindowName();
                var sitemaps = Session[SESSION_SITEMAP] as IDictionary<string, List<SitemapItem>>;

                if (sitemaps == null)
                {
                    sitemaps = new Dictionary<string, List<SitemapItem>>();
                    foreach (string name in WindowName.Items.Values)
                    {
                        sitemaps.Add(name, new List<SitemapItem>());
                    }

                    Session[SESSION_SITEMAP] = sitemaps;
                }

                var sitemap = sitemaps[windowName];

                return sitemap;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class SitemapItem
        {
            public string AreaName { get; set; }

            public string ControllerName { get; set; }

            public string ActionName { get; set; }

            public object RestoreData { get; set; }
        }

        private void PrintSitemap()
        {
            var windowName = GetWindowName();
            var sitemaps = Session[SESSION_SITEMAP] as IDictionary<string, List<SitemapItem>>;

            var sitemap = sitemaps[windowName];

            var sb = new System.Text.StringBuilder();
            sb.Append(string.Format("\nSitemap stack [{0}]\n", windowName));
            foreach (var item in sitemap.ToArray().Reverse())
            {
                sb.AppendFormat("\t{0}\n", item.ControllerName);
            }
            sb.Append("---------");

            //logger.Debug(sb.ToString());
        }

        /// <summary>
        /// Return the login user object
        /// </summary>
        /// <returns></returns>
        protected int GetScrollTop()
        {
            return (int)Session[Constant.SESSION_SCROLL_TOP];
        }

        /// <summary>
        /// Set the login user object
        /// </summary>
        /// <param name="user"></param>
        protected void SetScrollTop(int value)
        {
            Session[Constant.SESSION_SCROLL_TOP] = value;
        }

        #region Cache

        /// <summary>
        /// Saves the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SaveCache(string key, object value)
        {
            CacheUtil.SaveCache(key, value);
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public T GetCache<T>(string key, object defaultValue = null)
        {
            return CacheUtil.GetCache<T>(key, defaultValue);
        }

        /// <summary>
        /// Removes the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        public void RemoveCache(string key)
        {
            CacheUtil.RemoveCache(key);
        }

        #endregion Cache

    }
}