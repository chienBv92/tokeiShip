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
    public class DistrictModel:MstDistrict
    {

        public int CITY_CD_HIDDEN { get; set; }
        public int DISTRICT_CD_HIDDEN { get; set; }

        public string CITY_NAME { get; set; }

        public List<SelectListItem> CITY_LIST { get; set; }

        public List<MstDistrictEx> ListDistrict = new List<MstDistrictEx>();

    }
}