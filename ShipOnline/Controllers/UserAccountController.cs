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
using System.Configuration;
using BotDetect.Web.Mvc;

namespace ShipOnline.Controllers
{
    public class UserAccountController : BaseController
    {
        #region LOGIN
        // GET: UserAccount
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(bool timeout = false)
        {
            UserAccountModel model = new UserAccountModel();
            ViewBag.ErrorLogin = "";

            if (base.GetCache<CmnEntityModel>("CmnEntityModel") != null &&
                System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                TempData.Clear();
                FormsAuthentication.SignOut();
                base.RemoveCache("CmnEntityModel");
                Session.Clear();

                return this.RedirectToAction("Login", "UserAccount");
            }
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
                        if (userInfo.LOGIN_LOCK_FLG != LockFlag.NON_LOCK)
                        {
                            ModelState.AddModelError("", "Tài khoản đang bị khóa!");
                            return View(userInfo);
                        }
                        if (userInfo.EMAIL_CONFIRMED != EmailConfirmed.Yes)
                        {
                            ModelState.AddModelError("", "Tài khoản chưa được xác nhận, vui lòng kiểm tra email!");
                            return View(userInfo);
                        }
                        if (userInfo.STATUS != StatusFlag.DISPLAY)
                        {
                            ModelState.AddModelError("", "Tài khoản chưa được duyệt! ");
                            ModelState.AddModelError("", "Chúng tôi sẽ liên hệ trong thời gian sớm nhất để có dịch vụ tốt nhất! ");
                            return View(userInfo);
                        }
                        if (userInfo.LOGIN_LOCK_FLG == LockFlag.NON_LOCK && userInfo.STATUS == StatusFlag.DISPLAY)
                        {
                            if (userInfo.PASSWORD_LAST_UPDATE_DATE.Year < 2017 || Utility.GetCurrentDateTime() >
                             userInfo.PASSWORD_LAST_UPDATE_DATE.AddMonths(6))
                            {
                                ViewBag.PASSWORD_EXPIRED = string.Format(Message.PasswordOnExpired);
                                ModelState.AddModelError("", @Message.PasswordOnExpired);
                                return this.RedirectToAction("ChangePassword", "ChangePassword");
                            }
                            userInfo.IpAddress = IpAddress;
                            userInfo.UserAgent = UserAgent;
                            userInfo.BrowserType = BrowserType;
                            userInfo.BrowserVersion = BrowserVersion;
                            // Update session
                            UpdateCmnEntityModel(userInfo);
                            //Session["USER_INFO"] = userInfo;
                            FormsAuthentication.SetAuthCookie(userInfo.USER_EMAIL, false);

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
            CmnEntityModel.USER_EMAIL = loginInfoInput.USER_EMAIL;
            CmnEntityModel.USER_NAME = loginInfoInput.USER_NAME;
            CmnEntityModel.SHOP_NAME = loginInfoInput.SHOP_NAME;
            CmnEntityModel.AREA = loginInfoInput.AREA;
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

        #region LOGOUT
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            TempData.Clear();
            FormsAuthentication.SignOut();
            base.RemoveCache("CmnEntityModel");
            Session.Clear();

            return this.RedirectToAction("Index", "Home");
        }

        #endregion

        #region REGISTER/ UPDATE
        public ActionResult Register(long UserId = 0)
        {
            UserAccountModel model = new UserAccountModel();

            CommonService comService = new CommonService();
            ManageUserDa dataAccess = new ManageUserDa();
            if (UserId > 0)
            {
                UserAccountModel infor = new UserAccountModel();
                infor = dataAccess.getInfoUser(UserId);
                model = infor != null ? infor : model;
                model.DISTRICT_CD_KEY = model.USER_CITY.ToString() + "_" + model.USER_DISTRICT.ToString();
                model.TOWN_CD_KEY = model.USER_CITY.ToString() + "_" + model.USER_DISTRICT.ToString() + "_" + model.USER_TOWN.ToString();
            }

            model.CITY_LIST = comService.GetCityList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString(),
                Text = f.CITY_NAME
            }).ToList();
            model.CITY_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "Tỉnh/thành phố" });

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

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation("CaptchaCode", "exampleCaptcha", "Mã capcha không đúng!")]
        public ActionResult Edit(UserAccountModel model)
        {
            try
            {
                using (UserAccountService service = new UserAccountService())
                {
                    if (ModelState.IsValid)
                    {
                        bool isNew = false;
                        SendMailService sendMailservice = new SendMailService();
                        if (model.USER_ID_HIDDEN == 0)
                        {
                            isNew = true;

                            service.InsertUser(model);
                            // send mail confirm account
                            sendMailRegisterAccount(model);

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
                        foreach (var mes in ErrorMessages)
                        {
                            ModelState.AddModelError(mes.Key, mes.Errors.ToString());
                        }
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
        public ActionResult CheckExistUserAccount(string userAccount)
        {
                // Declare new DataAccess object
                ManageUserDa dataAccess = new ManageUserDa();

                var exist = dataAccess.CheckExistUserAccount(userAccount);
                
            return new EmptyResult();
        }

        #endregion

        #region VIEW ACCOUNT
        public ActionResult ViewAccount(long UserId = 0)
        {
            UserAccountModel model = new UserAccountModel();

            CommonService comService = new CommonService();
            ManageUserDa dataAccess = new ManageUserDa();
            if (UserId > 0)
            {
                UserAccountModel infor = new UserAccountModel();
                infor = dataAccess.getInfoUser(UserId);
                model = infor != null ? infor : model;
            }


            return View(model);
        }
        #endregion

        #region CHANGE PASSWORD
        public ActionResult ChangePassword(long UserId = 0)
        {
            UserAccountModel model = new UserAccountModel();

            CommonService comService = new CommonService();
            ManageUserDa dataAccess = new ManageUserDa();
            if (UserId > 0)
            {
                UserAccountModel infor = new UserAccountModel();
                infor = dataAccess.getInfoUser(UserId);
                model = infor != null ? infor : model;
            }


            return View(model);
        }
        #endregion

        #region SEND MAIL
        // send email confirm account
        public void sendMailRegisterAccount(UserAccountModel model)
        {
            SendMailService sendMailservice = new SendMailService();
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Views/Common/Sendmail.cshtml"));

            content = content.Replace("{{UserEmail}}", model.USER_EMAIL);
            content = content.Replace("{{UserName}}", model.USER_NAME);
            content = content.Replace("{{Phone}}", model.USER_PHONE);
            string subject = "Xác nhận tài khoản mới";
            var callbackUrl = Url.Action("ConfirmEmail", "UserAccount", new { UserEmail = model.USER_EMAIL }, protocol: Request.Url.Scheme);
            content = content.Replace("{{callback}}", callbackUrl);
            //var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            //sendMailservice.SendMail(toEmail, subject, content);
            sendMailservice.SendMail(model.USER_EMAIL, subject, content); // Gửi email đến tài khoản đăng kí
        }

        // send mail reset password
        public void sendMailResetPassword(string UserEmail)
        {
            SendMailService sendMailservice = new SendMailService();
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Views/Common/ResetPassword.cshtml"));

            content = content.Replace("{{UserEmail}}", UserEmail);
            string subject = "Xác nhận thiết lập mật khẩu";
            var callbackUrl = Url.Action("ConfirmResetPassword", "UserAccount", new { UserEmail = UserEmail }, protocol: Request.Url.Scheme);
            content = content.Replace("{{callback}}", callbackUrl);

            sendMailservice.SendMail(UserEmail, subject, content); // Gửi email đến tài khoản đăng kí
        }

        // check exist user
        public ActionResult ConfirmEmail(string UserEmail)
        {
                // Declare new DataAccess object
                ManageUserDa dataAccess = new ManageUserDa();

                var UserId = dataAccess.selectUserId(UserEmail);
                if (UserId > 0)
                {
                    var suscess = dataAccess.ConfirmEmail(UserId);
                    if (suscess)
                    {
                        return this.RedirectToAction("Login", "UserAccount");
                    }
                }

            return new EmptyResult();
        }

        // check exist user
        public ActionResult ConfirmResetPassword(string UserEmail)
        {
            // Declare new DataAccess object
            ManageUserDa dataAccess = new ManageUserDa();

            var UserId = dataAccess.selectUserId(UserEmail);
            if (UserId > 0)
            {
                var suscess = dataAccess.ConfirmEmail(UserId);
                if (suscess)
                {
                    return this.RedirectToAction("Login", "UserAccount");
                }
            }

            return new EmptyResult();
        }
        #endregion

        #region PASSWORD
        [HttpGet]
        public ActionResult ResetPassword()
        {
            UserAccountModel model = new UserAccountModel();

            return View(model);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetPassword(UserAccountModel model)
        {
            if (ModelState.IsValid)
            {
                using (UserAccountService service = new UserAccountService())
                {
                    ManageUserDa dataAccess = new ManageUserDa();

                    var UserId = dataAccess.selectUserId(model.USER_EMAIL);
                    if (UserId > 0)
                    {
                        var suscess = dataAccess.ReSetPassword(UserId);
                        sendMailResetPassword(model.USER_EMAIL);
                        if (suscess > 0)
                        {
                            ViewBag.sendMailSuccess = "Yêu cầu reset mật khẩu của bạn đã được gửi tới email:" + model.USER_EMAIL;
                        }
                        return this.View();
                    }
                }
            }
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
            }

            return View();
        }
        #endregion
    }
}