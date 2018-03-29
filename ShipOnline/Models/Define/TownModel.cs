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
    public class TownModel: MstTown
    {
        public int? CITY_CD_SEARCH { get; set; }
        public int? DISTRICT_CD_SEARCH { get; set; }
        public int CITY_CD_HIDDEN { get; set; }
        public int DISTRICT_CD_HIDDEN { get; set; }
        public int TOWN_CD_HIDDEN { get; set; }

        public string CITY_NAME { get; set; }

        public string DISTRICT_NAME { get; set; }

        public string DISTRICT_CD_KEY { get; set; }

        public List<SelectListItem> CITY_LIST { get; set; }

        public List<SelectListItem> DISTRICT_LIST { get; set; }

        public List<MstTownEx> ListTown = new List<MstTownEx>();

        public TownModel()
        {
            CITY_CD_SEARCH = 0;
            DISTRICT_CD_SEARCH = 0;
        }
    }
}