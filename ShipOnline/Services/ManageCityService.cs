using ShipOnline.BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using ShipOnline.Models.Define;
using ShipOnline.DataAccess;
using System.Transactions;


namespace ShipOnline.Services
{
    public class ManageCityService:BaseServices
    {
        #region REGIST/ UPDATE
        public long InsertCity(MstCity city)
        {
            var ManageCityDa = new ManageCityDa();

            long res = 0;
            // Declare new DataAccess object
            ManageCityDa dataAccess = new ManageCityDa();
            using (var transaction = new TransactionScope())
            {
                res = dataAccess.RegisterCity(city);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
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

        #region Search
        public IEnumerable<MstCity> SearchCityList(DataTableModel dt, CityModel model, out int total_row)
        {
            ManageCityDa dataAccess = new ManageCityDa();
            IEnumerable<MstCity> results = dataAccess.SearchCityList(dt, model, out total_row);
            return results;
        }
        #endregion

        #region DELETE
        public bool DeleteCity(long CITY_CD)
        {
            ManageCityDa dataAccess = new ManageCityDa();

            int result = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    result = dataAccess.DeleteCity(CITY_CD);

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