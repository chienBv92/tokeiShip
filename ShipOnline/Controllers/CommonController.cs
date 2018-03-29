using ShipOnline.Models;
using ShipOnline.Models.Define;
using ShipOnline.Resources;
using ShipOnline.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ShipOnline.Controllers
{
    public class CommonController : BaseController
    {
        // GET: Common

        [HttpGet]
        public ActionResult CheckTimeOut()
        {
            if ((base.GetCache<CmnEntityModel>("CmnEntityModel") == null) || (base.GetCache<CmnEntityModel>("CmnEntityModel").USER_ID == 0))
            {
                this.Response.StatusCode = Constant.TIME_OUT;
                //logger.Fatal(Messages.E044);
                return new EmptyResult();
            }

            JsonResult result = Json(
                "Success",
                JsonRequestBehavior.AllowGet
            );
            return result;
        }

        /// <summary>
        /// Authent Timeout
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult AuthentTimeout(ActionExecutingContext filterContext)
        {
            Session.Clear();
            
            if (Request.IsAjaxRequest())
            {
                this.Response.StatusCode = Constant.TIME_OUT;
                return new EmptyResult();
            }
            else 
            {
                return this.RedirectToAction("Login", "UserAccount", new { timeout = true });
            }
        }

        public ActionResult RedirectTimeout()
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            if (currentUser == null || currentUser.USER_ID == 0)
            {
                return RedirectToAction("Login", "UserAccount");
            }
            return new EmptyResult();
        }

        public ActionResult GetDistrictByCityCd(int cityCd)
        {
            using (CommonService service = new CommonService())
            {
                var districtList = service.GetDistrictByCityCd(cityCd);
                var result = (from s in districtList
                              select new
                              {
                                  key = s.DISTRICT_CD,
                                  value = s.DISTRICT_NAME
                              }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get town by district, city
        /// </summary>
        /// <param name="cityCd"></param>
        /// <param name="districtCd"></param>
        /// <returns></returns>
        public ActionResult GetTownByDistrictCd(int cityCd, int districtCd)
        {
            using (CommonService service = new CommonService())
            {
                var townList = service.GetTownByDistrictCd(cityCd, districtCd);
                var result = (from s in townList
                              select new
                              {
                                  key = s.TOWN_CD,
                                  value = s.TOWN_NAME
                              }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        #region Get Date Time
        public ActionResult GetDateTime(int city = 0, int district = 0, int product_weight = 0, string product_type = "", int product_lenght = 0, int product_width = 0, int product_height = 0, string other = "", string callback = "")
        {
            CommonService service = new CommonService();
            SelectTimeModel model = new SelectTimeModel();
            model.CallBack = callback;
            //CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            //var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            //if (currentUser == null)
            //{
            //    return RedirectToAction("Login", "UserAccount");
            //}

            //var model = new OrderShipModel
            //{
            //    CallBack = callback,
            //    RECEIVED_CITY = city,
            //    RECEIVED_DISTRICT = district,
            //};

            // Tinh gia tri M = M1 +M2
            decimal money_phu_thu = 0;
            if (other.Equals(Other_Requirement.CanTry))
            {
                money_phu_thu = (money_phu_thu + 10) * 1000;
            }
            decimal money_weight = 0;

            int weight_real = product_weight;
            if (product_type.Equals(Product_Type.Cumbrous))
            {
                int weight_change = (int)((product_lenght * product_height * product_width) / 6500);
                weight_real = weight_change > weight_real ? weight_change : weight_real; // Khoi luong chuyen doi lon hon khoi luong thuc thi lay khoi luong chuyen doi
            }
            if (weight_real > 3)
            {
                money_weight = ((weight_real - 3) * 5) * 1000;
            }

            money_weight = money_weight + money_phu_thu;

            //Tinh gia cuoc phu thuoc noi thanh, ngoai thanh

            var InfoDistrict = service.getInfoDistrict(city, district);
            if (InfoDistrict != null)
            {
                var currentDate = UtilityService.Utility.GetCurrentDateTime();
                DateTime date = currentDate.Date;
                int hour = currentDate.Hour;
                int minute = currentDate.Minute;
                int currentTime = hour * 60 + minute + Constant.MinuteExtend;
                IList<TimeModel> ListTimeFastest = new List<TimeModel>();
                TimeModel TimeFast = new TimeModel();

                if (8 * 60 < currentTime && currentTime < 10 * 60) // Khung gio 8-10
                {
                    model.TAKE_DATE = date;
                    model.TAKE_HOUR_FROM = 10; // gio lấy hàng bat dau
                    model.TAKE_HOUR_TO = model.TAKE_HOUR_FROM + 2; // gio lấy hàng ket thuc

                    if (InsideDistrict.INSIDE.Equals(InfoDistrict.INSIDE))
                    {
                        TimeFast.TAKE_HOUR_FROM = 12;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fastest;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastestText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fastest + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        // sang ngay hom sau
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 10;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 12;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                    else
                    {
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 11;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                }
                else if (10 * 60 < currentTime && currentTime < 12 * 60)  // Khung gio 10-12
                {
                    model.TAKE_DATE = date;
                    model.TAKE_HOUR_FROM = 12; // gio lấy hàng bat dau
                    model.TAKE_HOUR_TO = model.TAKE_HOUR_FROM + 2; // gio lấy hàng ket thuc

                    if (InsideDistrict.INSIDE.Equals(InfoDistrict.INSIDE))
                    {
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fastest;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastestText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fastest + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        // sang ngay hom sau
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 10;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 12;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                    else
                    {
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 11;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                }
                else if (12 * 60 < currentTime && currentTime < 14 * 60)  // Khung gio 12-14
                {
                    model.TAKE_DATE = date;
                    model.TAKE_HOUR_FROM = 14; // gio lấy hàng bat dau
                    model.TAKE_HOUR_TO = model.TAKE_HOUR_FROM + 2; // gio lấy hàng ket thuc

                    if (InsideDistrict.INSIDE.Equals(InfoDistrict.INSIDE))
                    {
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fastest;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastestText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fastest + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        // sang ngay hom sau
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 10;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 12;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                    else
                    {
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 11;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                }
                else if (14 * 60 < currentTime && currentTime < 16 * 60)  // Khung gio 14-16
                {
                    model.TAKE_DATE = date;
                    model.TAKE_HOUR_FROM = 16; // gio lấy hàng bat dau
                    model.TAKE_HOUR_TO = model.TAKE_HOUR_FROM + 2; // gio lấy hàng ket thuc

                    if (InsideDistrict.INSIDE.Equals(InfoDistrict.INSIDE))
                    {
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fastest;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastestText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fastest + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        // sang ngay hom sau
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 10;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 12;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                    else
                    {
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 11;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                }
                else
                {
                    if (16 * 60 < currentTime)
                    {
                        date = date.AddDays(1);
                    }

                    model.TAKE_DATE = date;
                    model.TAKE_HOUR_FROM = 8; // gio lấy hàng bat dau
                    model.TAKE_HOUR_TO = model.TAKE_HOUR_FROM + 2; // gio lấy hàng ket thuc

                    if (InsideDistrict.INSIDE.Equals(InfoDistrict.INSIDE))
                    {
                        // sang ngay hom sau
                        TimeFast.TAKE_HOUR_FROM = 10;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fastest;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastestText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fastest + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 12;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        // sang ngay hom sau nua
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 10;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 12;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 16;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 4;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 20;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 2;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_inside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                    else
                    {
                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 11;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_TYPE_STRING = Ship_Type.FastText;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date;
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 8;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 11;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);

                        TimeFast = new TimeModel();
                        TimeFast.TAKE_HOUR_FROM = 14;
                        TimeFast.TAKE_HOUR_TO = TimeFast.TAKE_HOUR_FROM + 3;
                        TimeFast.TAKE_DATE = date.AddDays(1);
                        TimeFast.SHIP_TYPE = Ship_Type.Fast;
                        TimeFast.SHIP_MONEY = Constant.money_outside_fast + money_weight;
                        ListTimeFastest.Add(TimeFast);
                    }
                }

                if (InsideDistrict.INSIDE.Equals(InfoDistrict.INSIDE))
                {
                    model.TIME_SHIP_LIST = new SelectList(this.GetListTimeInCity(), "Value", "Text").ToList();
                    model.SHIP_MONEY_NORMAL = Constant.money_inside_normal + money_weight;
                }
                else
                {
                    model.TIME_SHIP_LIST = new SelectList(this.GetListTimeOutCity(), "Value", "Text").ToList();
                    model.SHIP_MONEY_NORMAL = Constant.money_outside_normal + money_weight;

                }

                //Tinh ngay chuyen thuong
                List<DateModel> ListDateNormal = new List<DateModel>();
                date = ListTimeFastest.Last().TAKE_DATE;
                for (int i = 0; i < Constant.dateNormalNumber; i++)  // chon duoc 7 ngay sau ngay chuyen nhanh
                {
                    DateModel DateNormal = new DateModel();
                    DateNormal.DATE_VALUE = date.AddDays(1 + i);
                    DateNormal.DATE_KEY = DateNormal.DATE_VALUE;
                    ListDateNormal.Add(DateNormal);
                }

                model.DATE_NORMAL_LIST = ListDateNormal.ToList().Select(
                f => new SelectListItem
                {
                    Value = f.DATE_VALUE.ToString("dd/MM/yyyy"),
                    Text = f.DATE_KEY.ToString("dd/MM/yyyy")
                }).ToList();

                model.LIST_TIME_FASTEST = ListTimeFastest;
                foreach (var TimeList in model.LIST_TIME_FASTEST)
                {
                    TimeList.SHIP_TYPE_TEXT = Ship_Type.Items[TimeList.SHIP_TYPE].ToString();
                }
            }

            return this.PartialView("PopupGetDateTime", model);
        }

        #endregion
        public IList<SelectListItem> GetListTimeInCity()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Dictionary<string, string> createStatus = new Dictionary<string, string>();

            createStatus.Add("8h - 10h", "1");
            createStatus.Add("10h - 12h", "2");
            createStatus.Add("12h - 14h", "3");
            createStatus.Add("14h - 16h", "4");
            createStatus.Add("16h - 20h", "5");
            createStatus.Add("20h - 22h", "6");

            foreach (KeyValuePair<string, string> pair in createStatus)
            {
                list.Add(new SelectListItem() { Text = pair.Key, Value = pair.Value, Selected = false });
            }

            return list;
        }

        public IList<SelectListItem> GetListTimeOutCity()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Dictionary<string, string> createStatus = new Dictionary<string, string>();

            createStatus.Add("8h - 11h", "1");
            createStatus.Add("11h - 14h", "2");
            createStatus.Add("14h - 17h", "3");

            foreach (KeyValuePair<string, string> pair in createStatus)
            {
                list.Add(new SelectListItem() { Text = pair.Key, Value = pair.Value, Selected = false });
            }

            return list;
        }

        public ActionResult GetListGroupArea(int forUser)
        {
            using (CommonService service = new CommonService())
            {
                var groupList = service.GetListGroupArea(forUser);
                var result = (from s in groupList
                              select new
                              {
                                  key = s.GROUP_CD,
                                  value = s.GROUP_NAME
                              }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}