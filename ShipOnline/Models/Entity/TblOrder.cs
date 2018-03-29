using ShipOnline.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Entity
{
    public class TblOrder : BaseEntity
    {
        public long ORDER_ID { get; set; }
        public long USER_ID { get; set; }
        public string SHIP_CODE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public int PRODUCT_TYPE { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public int? PRODUCT_WEIGHT { get; set; }
        public string PRODUCT_POSITION { get; set; }
        public long? ACCEPTANCE_ID { get; set; }
        public DateTime? ACCEPTANCE_DATE { get; set; }
        public string RECEIVED_NAME { get; set; }
        public string RECEIVED_PHONE { get; set; }
        public int RECEIVED_CITY { get; set; }
        public int RECEIVED_DISTRICT { get; set; }
        public int RECEIVED_TOWN { get; set; }
        public string RECEIVED_ADDRESS { get; set; }
        public int RECEIVE_HOUR_FROM { get; set; }
        public int RECEIVE_HOUR_TO { get; set; }

        public DateTime RECEIVE_TIME_DATE { get; set; }
        public int SHIP_TYPE { get; set; }
        public decimal? PRICE_PRODUCT { get; set; }
        public decimal PRICE_SHIP { get; set; }
        public decimal? DISCOUNT { get; set; }
        public string USER_PAYMENT { get; set; }
        public int ORDER_STATUS { get; set; }
        public DateTime? DELIVERY_DATE { get; set; }
        public DateTime? RECEIVED_DATE { get; set; }
        public DateTime? UPD_DATE { get; set; }

        public int? PRODUCT_LENGTH { get; set; }

        public int? PRODUCT_WIDTH { get; set; }

        public int? PRODUCT_HEIGHT { get; set; }

        public int OTHER_REQUIREMENT { get; set; }

    }
}