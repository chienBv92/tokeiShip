using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipOnline.Models.Define
{
    public class OrderShipCondition
    {
        public string ORDER_STATUS_LIST { get; set; }

        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string TEXT_SEARCH { get; set; }

        public string SHIP_CODE { get; set; }

        public string PRODUCT_NAME { get; set; }

        public decimal TotalPriceShip { get; set; }

        public decimal TotalPriceProduct { get; set; }

        public decimal TotalMoney { get; set; }

        public decimal TotalWaitCOD { get; set; }

        public long TotalStatus_0 { get; set; }
        public long TotalStatus_1 { get; set; }
        public long TotalStatus_2 { get; set; }
        public long TotalStatus_3 { get; set; }
        public long TotalStatus_4 { get; set; }
        public long TotalStatus_5 { get; set; }
        public long TotalStatus_6 { get; set; }
        public long TotalStatus_7 { get; set; }
        public long TotalStatus_8 { get; set; }
        public long TotalStatus_9 { get; set; }
        public long TotalStatusAll { get; set; }
        public long TotalOrderForMonth { get; set; }
        public int thisMonth { set; get; }
        public int TotalOrderthisMonth { set; get; }





    }
}