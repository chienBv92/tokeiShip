using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using System.Web.Mvc;

namespace ShipOnline.Models.Define
{
    public class OrderShipModel: TblOrder
    {
        
        public string DISTRICT_CD_KEY { get; set; }

        public string TOWN_CD_KEY { get; set; }

        public List<SelectListItem> CITY_LIST { get; set; }

        public List<SelectListItem> DISTRICT_LIST { get; set; }

        public List<SelectListItem> TOWN_LIST { get; set; }

        public string CallBack { get; set; }

        public List<SelectListItem> TIME_SHIP_LIST { get; set; }
        public string RECEIVE_CITY_NAME { get; set; }
        public string RECEIVE_DISTRICT_NAME { get; set; }
        public string RECEIVE_TOWN_NAME { get; set; }

        public string SENDER_NAME { get; set; }
        public string SENDER_PHONE { get; set; }
        public string SENDER_CITY_NAME { get; set; }
        public string SENDER_DISTRICT_NAME { get; set; }
        public string SENDER_TOWN_NAME { get; set; }
        public string SENDER_ADDRESS { get; set; }

        public string ACCEPTANCE_NAME { get; set; }

        public decimal SENDER_TOTAL_MONEY { get; set; }
        public decimal RECEIVE_TOTAL_MONEY { get; set; }
        public string PRODUCT_TYPE_TEXT { get; set; }
        public string SHIP_TYPE_TEXT { get; set; }
        public string ORDER_STATUS_TEXT { get; set; }
        public string OTHER_REQUIREMENT_TEXT { get; set; }
        public string PRODUCT_SIZE_TEXT { get; set; }
        public string PRODUCT_WEIGHT_TEXT { get; set; }
    }
}