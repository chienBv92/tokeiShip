using ShipOnline.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.System
{
    public class BaseEntity
    {
        public string DEL_FLG { get; set; }

        public DateTime? INS_DATE { get; set; }

        public DateTime? UPD_DATE { get; set; }

        public long INS_USER_ID { get; set; }

        public string INS_USER_NAME { get; set; }

        public long UPD_USER_ID { get; set; }

        public string UPD_USER_NAME { get; set; }

        public BaseEntity()
        {
            DEL_FLG = DeleteFlag.NON_DELETE; ;
        }
    }
}