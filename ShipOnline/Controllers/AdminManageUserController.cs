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
using ShipOnline.UtilityServices;
using ShipOnline.Models;
using System.Web.Security;
using ShipOnline.UtilityService;

namespace ShipOnline.Controllers
{
    public class AdminManageUserController : BaseController
    {
        // GET: /AdminManageUser/
        #region LIST
        [HttpGet]
        public ActionResult UserList()
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;
            
            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            UserAccountModel model = new UserAccountModel();

            CommonService comService = new CommonService();
            ManageUserDa dataAccess = new ManageUserDa();

            var tmpCondition = GetRestoreData() as UserAccountModel;
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
            model.CITY_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "" });

            model.DISTRICT_LIST = comService.GetDistrictList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString(),
                Text = f.DISTRICT_NAME
            }).ToList();

            model.TOWN_LIST = comService.GetTownList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString() + "_" + f.TOWN_CD.ToString(),
                Text = f.TOWN_NAME
            }).ToList();

            ViewBag.UserAuthority = new SelectList(UtilityServices.UtilityServices.GetAuthorityUser(), "Value", "Text");

            return View(model);
        }

        [HttpPost]
        public ActionResult UserList(DataTableModel dt, UserAccountModel condition)
        {
            if (ModelState.IsValid)
            {
                using (ManageUserService service = new ManageUserService())
                {
                    int total_row = 0;
                    var dataList = service.SearchUserList(dt, condition, out total_row);

                    int order = 1;
                    int totalRowCount = dataList.Count();
                    int lastItem = dt.iDisplayLength + dt.iDisplayStart;

                    var result = Json(
                        new
                        {
                            sEcho = dt.sEcho,
                            iTotalRecords = total_row,
                            iTotalDisplayRecords = total_row,
                            aaData = (from i in dataList
                                      select new object[]
                                {
                                    i.USER_ID,
                                    order++,
                                    i.USER_EMAIL != null ? HttpUtility.HtmlEncode(i.USER_EMAIL) : String.Empty,
                                    i.USER_NAME != null ? HttpUtility.HtmlEncode(i.USER_NAME) : String.Empty,
                                    i.SHOP_NAME != null ? HttpUtility.HtmlEncode(i.SHOP_NAME) : String.Empty,
                                    i.AREA_STRING =  Area.Items[i.AREA].ToString(),
                                    i.CITY_NAME != null ? HttpUtility.HtmlEncode(i.CITY_NAME) : String.Empty,
                                    i.DISTRICT_NAME != null ? HttpUtility.HtmlEncode(i.DISTRICT_NAME) : String.Empty,
                                    i.TOWN_NAME != null ? HttpUtility.HtmlEncode(i.TOWN_NAME) : String.Empty,
                                    i.USER_ADDRESS != null ? HttpUtility.HtmlEncode(i.USER_ADDRESS) : String.Empty,
                                    i.USER_PHONE != null ? HttpUtility.HtmlEncode(i.USER_PHONE) : String.Empty,
                                    i.INS_DATE != null ? i.INS_DATE.Value.ToString("dd/MM/yyyy") : String.Empty,
                                    i.STATUS =="1"? "Hiển thị" : "Ẩn",
                                    i.DEL_FLG
                                })

                        });
                    SaveRestoreData(condition);

                    result.MaxJsonLength = Int32.MaxValue;
                    return result;
                }
            }
            else
            {
                var ErrorMessages = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
            }
            return new EmptyResult();
        }
        #endregion


        #region REGISTER/ UPDATE
        public ActionResult UserEdit(long UserId = 0)
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;
            
            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            UserAccountModel model = new UserAccountModel();

            CommonService comService = new CommonService();
            ManageUserDa dataAccess = new ManageUserDa();
            if (UserId > 0)
            {
                UserAccountModel infor = new UserAccountModel();
                infor = dataAccess.getInfoUser(UserId);
                model = infor != null ? infor : model;
                model.DISTRICT_CD_KEY = model.USER_CITY.ToString() + "_" + model.USER_DISTRICT.ToString();
                model.TOWN_CD_KEY = model.USER_CITY.ToString() + "_" + model.USER_DISTRICT.ToString() +"_" + model.USER_TOWN.ToString();
            }

            model.CITY_LIST = comService.GetCityList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString(),
                Text = f.CITY_NAME
            }).ToList();
            model.CITY_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "" });

            model.DISTRICT_LIST = comService.GetDistrictList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString(),
                Text = f.DISTRICT_NAME
            }).ToList();

            model.TOWN_LIST = comService.GetTownList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString() + "_" + f.TOWN_CD.ToString(),
                Text = f.TOWN_NAME
            }).ToList();

            ViewBag.UserAuthority = new SelectList(UtilityServices.UtilityServices.GetAuthorityUser(), "Value", "Text");


            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserAccountModel model)
        {
            try
            {
                using (ManageUserService service = new ManageUserService())
                {
                    if (ModelState.IsValid)
                    {
                        bool isNew = false;

                        if (model.USER_ID_HIDDEN == 0)
                        {
                            isNew = true;

                            service.InsertUser(model);
                            JsonResult result = Json(new { isNew = isNew }, JsonRequestBehavior.AllowGet);
                            return result;
                        }
                        else
                        {
                            isNew = false;

                            service.UpdateUser(model);
                            JsonResult result = Json(new { isNew = isNew }, JsonRequestBehavior.AllowGet);
                            return result;
                        }
                    }
                    else
                    {
                        var ErrorMessages = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                    }

                    return new EmptyResult();
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                System.Web.HttpContext.Current.Session["ERROR"] = ex;
                return new EmptyResult();
            }
        }

        // check exist user
        public ActionResult CheckExistUserAccount(string userEmail)
        {
            if (Request.IsAjaxRequest())
            {
                // Declare new DataAccess object
                ManageUserDa dataAccess = new ManageUserDa();

                var exist = dataAccess.CheckExistUserAccount(userEmail);
                JsonResult result = Json(new
                {
                    exist
                }, JsonRequestBehavior.AllowGet);

                return result;

            }
            return new EmptyResult();
        }

        #endregion


        #region DELETE
        public ActionResult DeleteUser(long USER_ID = 0)
        {
            if (Request.IsAjaxRequest())
            {
                if (USER_ID > 0)
                {
                    using (var service = new ManageUserService())
                    {
                        var deleteResult = service.DeleteUser(USER_ID);

                        JsonResult result = Json(new
                        {
                            statusCode = deleteResult ? Constant.SUCCESSFUL : Constant.INTERNAL_SERVER_ERROR
                        }, JsonRequestBehavior.AllowGet);

                        return result;
                    }
                }
                else
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                }
            }

            return new EmptyResult();
        }

        #endregion


        #region LOGIN
        // GET: UserAccount
        [HttpGet]
        public ActionResult Login()
        {
            UserAccountModel model = new UserAccountModel();

            return View(model);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(UserAccountModel model, string btnLogin)
        {
            if (ModelState.IsValid)
            {
                using (UserAccountService service = new UserAccountService())
                {
                    UserAccountModel userInfo = new UserAccountModel();

                    var Info = service.getInfoUser(model);
                    string IpAddress = GetIpAddress();
                    string UserAgent = Request.UserAgent;
                    string BrowserType = Request.Browser.Type;
                    string BrowserVersion = Request.Browser.Version;

                    if (Info != null)
                    {
                        userInfo = Info;
                        if (userInfo.LOGIN_LOCK_FLG != "0")
                        {
                            //UserAccountModel model = new UserAccountModel();
                            ViewBag.ErrorLogin = "Tài khoản đang bị khóa!";
                            ModelState.AddModelError("", "Tài khoản đang bị khóa!");
                            return View(userInfo);
                        }
                        if (userInfo.LOGIN_LOCK_FLG == "0")
                        {
                            if (userInfo.PASSWORD_LAST_UPDATE_DATE.Year < 2017 || Utility.GetCurrentDateTime() >
                             userInfo.PASSWORD_LAST_UPDATE_DATE.AddMonths(6))
                            {
                                ViewBag.PASSWORD_EXPIRED = string.Format(Message.PasswordOnExpired);
                                return this.RedirectToAction("ChangePassword", "ChangePassword");
                            }
                            userInfo.IpAddress = IpAddress;
                            userInfo.UserAgent = UserAgent;
                            userInfo.BrowserType = BrowserType;
                            userInfo.BrowserVersion = BrowserVersion;
                            // Update session chua can thiet
                            //UpdateCmnEntityModel(userInfo); 
                            //Session["USER_INFO"] = userInfo;

                            ViewBag.ErrorLogin = "";
                            return this.RedirectToAction("Index", "Home");

                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "Thông tin đăng nhập không chính xác!");
                        return View(userInfo);
                    }
                }
            }
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
            }

            return View();
        }

        /// <summary>
        /// Update for Session
        /// </summary>
        /// <returns></returns>
        private void UpdateCmnEntityModel(UserAccountModel loginInfoInput)
        {
            CmnEntityModel.USER_ID = loginInfoInput.USER_ID;
            CmnEntityModel.USER_NAME = loginInfoInput.USER_NAME;
            CmnEntityModel.USER_EMAIL = loginInfoInput.USER_EMAIL;
            CmnEntityModel.USER_AUTHORITY = loginInfoInput.USER_AUTHORITY;
            CmnEntityModel.STATUS = loginInfoInput.STATUS;
            CmnEntityModel.LOGIN_LOCK_FLG = loginInfoInput.LOGIN_LOCK_FLG;
            CmnEntityModel.USER_FAMILY = loginInfoInput.USER_FAMILY;
            CmnEntityModel.USER_CITY = loginInfoInput.USER_CITY;
            CmnEntityModel.CITY_NAME = loginInfoInput.CITY_NAME;
            CmnEntityModel.USER_DISTRICT = loginInfoInput.USER_DISTRICT;
            CmnEntityModel.DISTRICT_NAME = loginInfoInput.DISTRICT_NAME;
            CmnEntityModel.USER_TOWN = loginInfoInput.USER_TOWN;
            CmnEntityModel.TOWN_NAME = loginInfoInput.TOWN_NAME;

            CmnEntityModel.ipAddress = loginInfoInput.IpAddress;
            CmnEntityModel.userAgent = loginInfoInput.UserAgent;
            CmnEntityModel.browserType = loginInfoInput.BrowserType;
            CmnEntityModel.browserVersion = loginInfoInput.BrowserVersion;
        }


        private string GetIpAddress()
        {
            string IpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(IpAddress))
            {
                IpAddress = Request.UserHostAddress;
            }
            return IpAddress ?? string.Empty;
        }
        #endregion

	}
}