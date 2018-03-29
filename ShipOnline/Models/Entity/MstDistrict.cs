using ShipOnline.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Entity
{
    public class MstDistrict : BaseEntity
    {
        public int DISTRICT_CD { get; set; }
        public int CITY_CD { get; set; }
        public string DISTRICT_NAME { get; set; }
        public int DSP_ORDER { get; set; }
        public string STATUS { get; set; }
        public string INSIDE { get; set; }
    }
}