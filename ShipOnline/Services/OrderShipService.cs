using ShipOnline.BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using ShipOnline.Models.Define;
using ShipOnline.DataAccess;
using System.Transactions;
using ShipOnline.Resources;

namespace ShipOnline.Services
{
    public class OrderShipService:BaseServices
    {
        #region REGIST/ UPDATE
        public long InsertOrder(OrderShipModel order)
        {
            long res = 0;
            TblOrder entity = new TblOrder();
            CommonDa da = new CommonDa();
            
            entity.DEL_FLG = DeleteFlag.NON_DELETE;
            entity.USER_ID = CmnEntityModel.USER_ID;
            int maxOrder = da.getMaxOrder(entity.USER_ID) + 1;

            entity.SHIP_CODE = "T" + CmnEntityModel.USER_ID.ToString("D4") + maxOrder.ToString("D7");
            entity.PRODUCT_NAME = order.PRODUCT_NAME;
            entity.PRODUCT_TYPE = order.PRODUCT_TYPE;
            entity.CREATE_DATE = UtilityService.Utility.GetCurrentDateTime();
            entity.PRODUCT_WEIGHT = order.PRODUCT_WEIGHT.HasValue? order.PRODUCT_WEIGHT.Value : 0;
            entity.RECEIVED_NAME = order.RECEIVED_NAME;
            entity.RECEIVED_PHONE = order.RECEIVED_PHONE;
            entity.RECEIVED_CITY = order.RECEIVED_CITY;
            entity.RECEIVED_DISTRICT = order.RECEIVED_DISTRICT;
            entity.RECEIVED_TOWN = order.RECEIVED_TOWN;
            entity.RECEIVED_ADDRESS = order.RECEIVED_ADDRESS;
            entity.RECEIVE_HOUR_FROM = order.RECEIVE_HOUR_FROM;
            entity.RECEIVE_HOUR_TO = order.RECEIVE_HOUR_TO;
            entity.RECEIVE_TIME_DATE = order.RECEIVE_TIME_DATE;
            entity.SHIP_TYPE = order.SHIP_TYPE;
            entity.PRICE_PRODUCT = order.PRICE_PRODUCT.HasValue ? order.PRICE_PRODUCT.Value : 0;
            entity.PRICE_SHIP = order.PRICE_SHIP;
            entity.DISCOUNT = order.DISCOUNT;
            entity.USER_PAYMENT = order.USER_PAYMENT;
            entity.ORDER_STATUS = OrderStatus.Create;
            entity.OTHER_REQUIREMENT = order.OTHER_REQUIREMENT;
            entity.PRODUCT_HEIGHT = order.PRODUCT_HEIGHT.HasValue ? order.PRODUCT_HEIGHT.Value : 0;
            entity.PRODUCT_LENGTH = order.PRODUCT_LENGTH.HasValue ? order.PRODUCT_LENGTH.Value : 0;
            entity.PRODUCT_WIDTH = order.PRODUCT_WIDTH.HasValue ? order.PRODUCT_WIDTH.Value : 0;

            // Declare new DataAccess object
            OrderShipDa dataAccess = new OrderShipDa();
            using (var transaction = new TransactionScope())
            {
                res = dataAccess.InsertOrder(entity);
                if (res <= 0)
                    transaction.Dispose();

                if (res > 0)
                {
                    res = da.getShipID(entity.SHIP_CODE);

                    transaction.Complete();
                }
            }
            return res;
        }

        public long UpdateCity(MstCity city)
        {
            var ManageCityDa = new ManageCityDa();

            long res = 0;
            // Declare new DataAccess object
            ManageCityDa dataAccess = new ManageCityDa();
            using (var transaction = new TransactionScope())
            {
                res = dataAccess.UpdateCity(city);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }
        #endregion

        #region VIEW ORDER SHIP
        public OrderShipModel getInforOrder(long OrderId)
        {
            OrderShipDa dataAccess = new OrderShipDa();
            OrderShipModel order = dataAccess.getInforOrder(OrderId);
            return order;
        }
        #endregion

        #region ORDER SHIP LIST
        public IEnumerable<OrderShipModel> SearchOrderListTotalNoneStatus(DataTableModel dt, OrderShipCondition condition)
        {
            OrderShipDa dataAccess = new OrderShipDa();

            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderListTotalNoneStatus(dt, condition);
            return results;
        }
        public IEnumerable<OrderShipModel> SearchOrderShipListTotal(DataTableModel dt, OrderShipCondition condition)
        {
            OrderShipDa dataAccess = new OrderShipDa();
            
            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderShipListTotal(dt, condition);
            return results;
        }

        public IEnumerable<OrderShipModel> SearchOrderShipByStatus(int status)
        {
            OrderShipDa dataAccess = new OrderShipDa();
            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderShipByStatus(status);
            return results;
        }

        public IEnumerable<OrderShipModel> SearchOrderShipList(DataTableModel dt, OrderShipCondition condition, out int total_row)
        {
            OrderShipDa dataAccess = new OrderShipDa();
           
            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderShipList(dt, condition, out total_row);
            return results;
        }

        public IEnumerable<OrderShipModel> SearchOrderByMonth(string thisMonth)
        {
            OrderShipDa dataAccess = new OrderShipDa();

            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderByMonth(thisMonth);
            return results;
        }
        #endregion
    }
}