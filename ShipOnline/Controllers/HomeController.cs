using ShipOnline.Models.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShipOnline.DataAccess;
using ShipOnline.Services;
using ShipOnline.Service.Common;
using ShipOnline.Resources;
using ShipOnline.Models.Entity;
using ShipOnline.UtilityService;
using ShipOnline.Models;
using System.Web.Security;

namespace ShipOnline.Controllers
{
    public class HomeController:BaseController
    {
        [HttpGet]
        public ActionResult Index(OrderShipModel model)
        {
            CommonService comService = new CommonService();
            var tmpCondition = Session["OrderShip"] as OrderShipModel;
            if (tmpCondition != null)
            {
                model = tmpCondition;
            }

            model.CITY_LIST = comService.GetCityList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString(),
                Text = f.CITY_NAME
            }).ToList();

            model.DISTRICT_LIST = comService.GetDistrictList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString(),
                Text = f.DISTRICT_NAME
            }).ToList();
            model.DISTRICT_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "Quận/huyện" });

            model.TOWN_LIST = comService.GetTownList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString() + "_" + f.TOWN_CD.ToString(),
                Text = f.TOWN_NAME
            }).ToList();
            model.TOWN_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "Xã/phường" });

            Session["OrderShip"] = null;

            return View(model);
        }

        public ActionResult SaveSessionOrder(OrderShipModel session)
        {
            if (Request.IsAjaxRequest())
            {
                // Save data Session

                Session["OrderShip"] = session;

            }
            return new EmptyResult();
        }

        public ActionResult Intro()
        {
            return View();
        }

        public ActionResult SupportCenter()
        {

            return View();
        }
    }
}