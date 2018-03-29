using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Define
{
    public class TimeModel
    {
        public int TAKE_HOUR_TO { get; set; }
        public int TAKE_HOUR_FROM { get; set; }

        public string SHIP_TYPE_STRING { get; set; }

        public int SHIP_TYPE { get; set; }
        public string SHIP_TYPE_TEXT { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TAKE_DATE { get; set; }

        public decimal SHIP_MONEY { get; set; }

        //TimeModel(int takeFrom, int takeTo, DateTime takeDate)
        //{
        //    this.TAKE_HOUR_FROM = takeFrom;
        //    this.TAKE_HOUR_TO = takeTo;
        //    this.TAKE_DATE = takeDate;

        //}
    }
}