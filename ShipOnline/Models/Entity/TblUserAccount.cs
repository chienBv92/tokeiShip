using ShipOnline.Models.System;
using ShipOnline.Resources;
using ShipOnline.UtilityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Entity
{
    public class TblUserAccount:BaseEntity
    {
        public long USER_ID { get; set; }
        public string USER_EMAIL { get; set; }
        public string EMAIL_CONFIRMED { get; set; }
        public string USER_NAME { get; set; }
        public string SHOP_NAME { get; set; }
        public string USER_PASSWORD { get; set; }
        public DateTime PASSWORD_LAST_UPDATE_DATE { get; set; }
        public int AREA { get; set; }
        public string AREA_STRING { get; set; }
        public int USER_AUTHORITY { get; set; }
        public int USER_CITY { get; set; }
        public int USER_DISTRICT { get; set; }
        public int USER_TOWN { get; set; }
        public string USER_ADDRESS { get; set; }
        public string USER_PHONE { get; set; }
        public Nullable<bool> GENDER { get; set; }
        public string LOGIN_LOCK_FLG { get; set; }
        public string STATUS { get; set; }
        public Nullable<int> USER_FAMILY { get; set; }

        public TblUserAccount(): base()
        {
            INS_DATE = Utility.GetCurrentDateTime();
            PASSWORD_LAST_UPDATE_DATE = Utility.GetCurrentDateTime();
            UPD_DATE = Utility.GetCurrentDateTime();
            AREA = 0;
            USER_AUTHORITY = 0;
            LOGIN_LOCK_FLG = LockFlag.NON_LOCK;
            STATUS = StatusFlag.DISPLAY;
        }
    }
}