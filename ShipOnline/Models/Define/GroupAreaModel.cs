using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using ShipOnline.Resources;
using System.Web.Mvc;
using ShipOnline.Models.Extend;
using ShipOnline.Models.System;

namespace ShipOnline.Models.Define
{
    public class GroupAreaModel : MstGroupArea
    {
        public int TOWN_CD { get; set; }
        public int DISTRICT_CD { get; set; }
        public int CITY_CD { get; set; }
        public string DISTRICT_CD_KEY { get; set; }
        public string TOWN_CD_KEY { get; set; }
        public string GROUP_CD_LIST { get; set; }
        public List<SelectListItem> CITY_LIST { get; set; }

        public List<SelectListItem> DISTRICT_LIST { get; set; }

        public List<SelectListItem> TOWN_LIST { get; set; }

        public IList<GroupTown> LIST_TOWN { get; set; }
    }
}