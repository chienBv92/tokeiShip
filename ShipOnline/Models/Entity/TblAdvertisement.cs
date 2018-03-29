using ShipOnline.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Entity
{
    public class TblAdvertisement : BaseEntity
    {
        public int ADVERTISEMENT_ID { get; set; }
        public int ADVERTISEMENT_TYPE { get; set; }
        public string COMPANY_NAME { get; set; }
        public string ADVERTISEMENT_NAME { get; set; }
        public string ADVERTISEMENT_LINK { get; set; }
        public string ADVERTISEMENT_IMAGE { get; set; }
        public string DEL_FLG { get; set; }
        public string STATUS { get; set; }
        public DateTime? INS_DATE { get; set; }
        public int INS_USER_ID { get; set; }
        public DateTime? UPD_DATE { get; set; }
        public int UPD_USER_ID { get; set; }
    }
}