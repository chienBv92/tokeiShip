using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using ShipOnline.Resources;
using System.Web.Mvc;
using ShipOnline.Models.Extend;

namespace ShipOnline.Models.Define
{
    public class GroupTown : MstTown
    {
        public string CITY_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string DISTRICT_CD_KEY { get; set; }
        public string TOWN_CD_KEY { get; set; }
    }
}