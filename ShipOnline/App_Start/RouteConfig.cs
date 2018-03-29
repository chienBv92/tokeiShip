using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShipOnline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
              new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

            // Tao Url than thien cho SEO
            routes.MapRoute(
                name: "HelpMe",
                url: "ho-tro-khach-hang/{id}",
                defaults: new { controller = "Home", action = "SupportCenter", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Service",
               url: "dich-vu/{id}",
               defaults: new { controller = "PDFManage", action = "ViewPdf", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Intro",
               url: "gioi-thieu/{id}",
               defaults: new { controller = "Home", action = "Intro", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Tai khoan",
                url: "tai-khoan/{metatitle}/{id}",
                defaults: new { controller = "UserAccount", action = "Register", id = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "Home",
              url: "trang-chu/{id}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
          );
        }
    }
}
