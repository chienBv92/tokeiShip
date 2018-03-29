using ShipOnline.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Resources;
using System.Web.Mvc;
namespace ShipOnline.Models.Extend
{
    public class TblUserAccountEx : TblUserAccount
    {
        public string CITY_NAME { get; set; }

        public string DISTRICT_NAME { get; set; }

        public string TOWN_NAME { get; set; }
    }
}