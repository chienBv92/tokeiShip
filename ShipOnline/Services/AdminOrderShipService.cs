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
    public class AdminOrderShipService : BaseServices
    {
        #region ORDER SHIP LIST
        public IEnumerable<OrderShipModel> SearchOrderListTotalNoneStatus(DataTableModel dt, AdminOrderList condition)
        {
            AdminOrderShipDa dataAccess = new AdminOrderShipDa();

            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderListTotalNoneStatus(dt, condition);
            return results;
        }
        public IEnumerable<OrderShipModel> SearchOrderShipListTotal(DataTableModel dt, AdminOrderList condition)
        {
            AdminOrderShipDa dataAccess = new AdminOrderShipDa();

            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderShipListTotal(dt, condition);
            return results;
        }

        public IEnumerable<OrderShipModel> SearchOrderShipByStatus(int status)
        {
            AdminOrderShipDa dataAccess = new AdminOrderShipDa();
            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderShipByStatus(status);
            return results;
        }

        public IEnumerable<OrderShipModel> SearchOrderShipList(DataTableModel dt, AdminOrderList condition, out int total_row)
        {
            AdminOrderShipDa dataAccess = new AdminOrderShipDa();

            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderShipList(dt, condition, out total_row);
            return results;
        }

        public IEnumerable<OrderShipModel> SearchOrderByMonth(string thisMonth)
        {
            AdminOrderShipDa dataAccess = new AdminOrderShipDa();

            IEnumerable<OrderShipModel> results = dataAccess.SearchOrderByMonth(thisMonth);
            return results;
        }
        #endregion

    }
}