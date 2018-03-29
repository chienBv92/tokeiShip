using ShipOnline.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Extend
{
    public class MstTownEx:MstTown
    {
        public string CITY_NAME { get; set; }

        public string DISTRICT_NAME { get; set; }
    }
}