using ShipOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipOnline.Controllers
{
    public class AdminSystemController : BaseController
    {
        
        // GET: /AdminSystem/
        [HttpGet]
        public ActionResult Index()
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            return View();
        }
	}
}