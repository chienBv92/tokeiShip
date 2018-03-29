using ShipOnline.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Entity
{
    public class MstCity:BaseEntity
    {
        public int CITY_CD { get; set; }
        public string CITY_ZIP_CD { get; set; }
        public string CITY_NAME { get; set; }
        public int DSP_ORDER { get; set; }
        public string STATUS { get; set; }
    }
}