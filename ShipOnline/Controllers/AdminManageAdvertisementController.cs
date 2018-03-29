using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipOnline.Controllers
{
    public class AdminManageAdvertisementController : BaseController
    {
        //
        // GET: /AdminManageAdvertisement/
        public ActionResult AdvertisementList()
        {
            return View();
        }

        public ActionResult AdvertisementEdit()
        {
            return View();
        }
	}
}