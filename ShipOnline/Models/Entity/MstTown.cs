using ShipOnline.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Entity
{
    public class MstTown : BaseEntity
    {
        public int TOWN_CD { get; set; }
        public int DISTRICT_CD { get; set; }
        public int CITY_CD { get; set; }
        public string TOWN_NAME { get; set; }
        public string STATUS { get; set; }
        public long GROUP_CD_SENDER { get; set; }
        public int DSP_ORDER_SENDER { get; set; }
        public long GROUP_CD_RECEIVE { get; set; }
        public int DSP_ORDER_RECEIVE { get; set; }
        public long GROUP_CD { get; set; }
        public int DSP_ORDER { get; set; }
    }
}