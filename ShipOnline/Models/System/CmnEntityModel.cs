using System;
using System.Collections.Generic;
using System;
using System.Data;
using System.Web.Mvc;

namespace ShipOnline.Models
{
    
	/// <summary>
	/// Common entity Model
	/// </summary>
    [Serializable] 
	public class CmnEntityModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CmnEntityModel"/> class.
		/// </summary>
		public CmnEntityModel()
		{
			this.Clear();
			this.CurrentScreenID = string.Empty;
			this.ParentScreenID = string.Empty;
			this.ScreenRoute = string.Empty;
		}

        public long USER_ID { get; set; }

        public string USER_EMAIL { get; set; }

        public string EMAIL_CONFIRMED { get; set; }

        public string USER_NAME { get; set; }
        public string SHOP_NAME { get; set; }
        public int USER_AUTHORITY { get; set; }
        public int AREA { get; set; }

        public int USER_CITY { get; set; }
        public int USER_DISTRICT { get; set; }
        public int USER_TOWN { get; set; }

        public string CITY_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string TOWN_NAME { get; set; }

        public string USER_ADDRESS { get; set; }
        public string USER_PHONE { get; set; }
        public string LOGIN_LOCK_FLG { get; set; }
        public string STATUS { get; set; }
        public int? USER_FAMILY { get; set; }

		public string CurrentScreenID { get; set; }

		public string ParentScreenID { get; set; }

        public string ipAddress { get; set; }

        public string userAgent { get; set; }

        public string browserType { get; set; }

        public string browserVersion { get; set; }

        public string ScreenRoute { get; set; }

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
            this.USER_ID = 0;
            this.USER_EMAIL = string.Empty;
            this.USER_NAME = string.Empty;
		}
	}
}