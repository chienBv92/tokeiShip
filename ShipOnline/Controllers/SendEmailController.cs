using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace ShipOnline.Controllers
{
    public class SendEmailController : BaseController
    {
        // GET: SendEmail
        public ActionResult Index()
        {
            return View();
        }

    }
}