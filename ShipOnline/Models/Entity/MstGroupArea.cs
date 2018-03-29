using ShipOnline.Models.System;
using ShipOnline.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Entity
{
    public class MstGroupArea : BaseEntity
    {
        public long GROUP_CD { get; set; }
        public string GROUP_NAME { get; set; }
        public int FOR_USER { get; set; }
        public string FOR_USER_TEXT { get; set; }

        public MstGroupArea()
        {
            FOR_USER = GroupForUser.Receive;
        }
       
    }
}