using ShipOnline.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Resources;
using System.Web.Mvc;

namespace ShipOnline.Models.Define
{
    public class UserAccountModel: TblUserAccount
    {
        public string CITY_NAME { get; set; }

        public string DISTRICT_NAME { get; set; }

        public string TOWN_NAME { get; set; }

        public int? CITY_CD_SEARCH { get; set; }
        public int? DISTRICT_CD_SEARCH { get; set; }
        public int? TOWN_CD_SEARCH { get; set; }

        public int USER_ID_HIDDEN { get; set; }

        public string DISTRICT_CD_KEY { get; set; }

        public string TOWN_CD_KEY { get; set; }

        public List<SelectListItem> CITY_LIST { get; set; }

        public List<SelectListItem> DISTRICT_LIST { get; set; }

        public List<SelectListItem> TOWN_LIST { get; set; }

        //public List<MstTownEx> ListTown = new List<MstTownEx>();


        public string IpAddress { get; set; }

        /// <summary>
        /// UserAgent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Request.Browser.Type
        /// </summary>
        public string BrowserType { get; set; }

        /// <summary>
        /// Request.Browser.Version
        /// </summary>
        public string BrowserVersion { get; set; }
    }
}