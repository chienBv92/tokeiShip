using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.BusinessServices;
using ShipOnline.DataAccess;
using ShipOnline.Models.Define;
using ShipOnline.Models.Entity;
using ShipOnline.Models.Extend;
using System.Transactions;

namespace ShipOnline.Services
{
    public class ManageTownService : BaseServices
    {
        #region REGIST/ UPDATE
        public long InsertTown(TownModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageTownDa dataAccess = new ManageTownDa();
            using (var transaction = new TransactionScope())
            {
                res = dataAccess.InsertTown(model);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }

        public long UpdateTown(TownModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageTownDa dataAccess = new ManageTownDa();
            using (var transaction = new TransactionScope())
            {
                res = dataAccess.UpdateTown(model);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }
        #endregion

        #region SEARCH
        public IEnumerable<MstTownEx> SearchTownList(DataTableModel dt, TownModel model, out int total_row)
        {
            ManageTownDa dataAccess = new ManageTownDa();
            IEnumerable<MstTownEx> results = dataAccess.SearchTownList(dt, model, out total_row);
            return results;
        }
        #endregion

        #region DELETE
        public bool DeleteTown(long CITY_CD, long DISTRICT_CD, long TOWN_CD)
        {
            ManageTownDa dataAccess = new ManageTownDa();

            int result = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    result = dataAccess.DeleteTown(CITY_CD, DISTRICT_CD, TOWN_CD);

                    if (result > 0)
                        transaction.Complete();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    result = -1;
                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return (result > 0);
        }

        #endregion

    }
}