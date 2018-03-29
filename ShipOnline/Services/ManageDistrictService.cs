using ShipOnline.BusinessServices;
using ShipOnline.DataAccess;
using ShipOnline.Models.Define;
using ShipOnline.Models.Entity;
using ShipOnline.Models.Extend;
using ShipOnline.Resources;
using ShipOnline.UtilityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace ShipOnline.Services
{
    public class ManageDistrictService:BaseServices
    {
        #region REGIST/ UPDATE
        public long InsertDistrict(DistrictModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            using (var transaction = new TransactionScope())
            {
                res = dataAccess.InsertDistrict(model);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }

        public long UpdateDistrict(DistrictModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            using (var transaction = new TransactionScope())
            {
                res = dataAccess.UpdateDistrict(model);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }
        #endregion

        #region SEARCH 
        public IEnumerable<MstDistrictEx> SearchDistrictList(DataTableModel dt, DistrictModel model, out int total_row)
        {
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            IEnumerable<MstDistrictEx> results = dataAccess.SearchDistrictList(dt, model, out total_row);
            return results;
        }
        #endregion

        #region DELETE
        public bool DeleteDistrict(long CITY_CD, long DISTRICT_CD)
        {
            ManageDistrictDa dataAccess = new ManageDistrictDa();

            int result = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    result = dataAccess.DeleteDistrict(CITY_CD, DISTRICT_CD);

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

        public bool DeleteTownInArea(long cityCd = 0, long districtCd = 0, long townCD = 0, int forUser = 0)
        {
            ManageDistrictDa dataAccess = new ManageDistrictDa();

            int result = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    result = dataAccess.DeleteTownInArea(cityCd, districtCd, townCD, forUser);

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
        /// <summary>
        /// Delete Group Area
        /// </summary>
        /// <param name="groupCd"></param>
        /// <param name="forUser"></param>
        /// <returns></returns>
        public bool DeleteGroupArea(long groupCd = 0, int forUser = 0)
        {
            ManageDistrictDa dataAccess = new ManageDistrictDa();

            int result = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    result = dataAccess.DeleteGroupArea(groupCd, forUser);
                    var result2 = dataAccess.DeleteAllTownInArea(groupCd, forUser);
                    
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
         
        #region REGIST/ UPDATE GROUP AREA
        public long InsertGroupArea(GroupAreaModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            using (var transaction = new TransactionScope())
            {
                MstGroupArea entity = new MstGroupArea();
                entity.GROUP_NAME = model.GROUP_NAME;
                entity.FOR_USER = model.FOR_USER;
                entity.DEL_FLG = DeleteFlag.NON_DELETE;
                entity.INS_DATE = Utility.GetCurrentDateTime();
                entity.UPD_DATE = Utility.GetCurrentDateTime();

                res = dataAccess.InsertGroupArea(entity);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }

        public long UpdateTownInArea(GroupTown model, int forUser)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            using (var transaction = new TransactionScope())
            {
                res = dataAccess.UpdateTownInArea(model, forUser);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }

        public long UpdateGroupArea(GroupAreaModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            using (var transaction = new TransactionScope())
            {
                MstGroupArea entity = new MstGroupArea();
                entity.GROUP_CD = model.GROUP_CD;
                entity.GROUP_NAME = model.GROUP_NAME;
                entity.FOR_USER = model.FOR_USER;
                entity.DEL_FLG = DeleteFlag.NON_DELETE;
                entity.INS_DATE = Utility.GetCurrentDateTime();
                entity.UPD_DATE = Utility.GetCurrentDateTime();

                res = dataAccess.UpdateGroupArea(entity);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }
        #endregion

        #region SEARCH GROUP AREA
        public IEnumerable<MstGroupArea> SearchGroupAreatList(DataTableModel dt, GroupAreaModel model, out int total_row)
        {
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            if (model.CITY_CD != null && model.CITY_CD > 0)
            {
                var getListGroup = dataAccess.getListGroupForSearch(model).ToList();
                if (getListGroup.Count > 0)
                {
                    model.GROUP_CD_LIST = string.Join("','", getListGroup.ToArray());
                }
            }
            IEnumerable<MstGroupArea> results = dataAccess.SearchGroupAreatList(dt, model, out total_row);
            return results;
        }
        #endregion

    }
}