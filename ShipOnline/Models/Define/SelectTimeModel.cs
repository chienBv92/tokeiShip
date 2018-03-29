using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipOnline.Models.Define
{
    public class SelectTimeModel
    {
        public int TAKE_HOUR_TO { get; set; }

        public int TAKE_HOUR_FROM { get; set; }
        public DateTime TAKE_DATE { get; set; }

        public decimal SHIP_MONEY_NORMAL { get; set; }

        public DateTime RECEIVED_DATE { get; set; }

        public IList<TimeModel> LIST_TIME_FASTEST = new List<TimeModel>();

        public List<SelectListItem> TIME_SHIP_LIST { get; set; }

        public List<SelectListItem> DATE_NORMAL_LIST { get; set; }
        public int TIME_SHIP_SELECT { get; set; }

        public string CallBack { get; set; }

    }
}