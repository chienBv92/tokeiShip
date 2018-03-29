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
using ShipOnline.UtilityService;
using ShipOnline.Resources;
using ShipOnline.UtilityServices.SafePassword;

namespace ShipOnline.Services
{
    public class ManageUserService:BaseServices
    {
        #region SEARCH
        public IEnumerable<TblUserAccountEx> SearchUserList(DataTableModel dt, UserAccountModel model, out int total_row)
        {
            ManageUserDa dataAccess = new ManageUserDa();
            IEnumerable<TblUserAccountEx> results = dataAccess.SearchUserList(dt, model, out total_row);
            return results;
        }
        #endregion


        #region REGIST/ UPDATE
        public long InsertUser(UserAccountModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageUserDa dataAccess = new ManageUserDa();
            using (var transaction = new TransactionScope())
            {
                TblUserAccount entity = new TblUserAccount();
                entity.USER_EMAIL = model.USER_EMAIL;
                entity.EMAIL_CONFIRMED = EmailConfirmed.None;
                entity.USER_NAME = model.USER_NAME;
                entity.SHOP_NAME = model.SHOP_NAME;
                entity.USER_PASSWORD = SafePassword.GetSaltedPassword(model.USER_PASSWORD);
                entity.PASSWORD_LAST_UPDATE_DATE = Utility.GetCurrentDateTime();
                entity.USER_AUTHORITY = model.USER_AUTHORITY;
                entity.AREA = model.AREA;
                entity.USER_CITY = model.USER_CITY;
                entity.USER_DISTRICT = model.USER_DISTRICT;
                entity.USER_TOWN = model.USER_TOWN;
                entity.USER_ADDRESS = model.USER_ADDRESS;
                entity.USER_PHONE = model.USER_PHONE;
                entity.USER_FAMILY = 0;
                entity.LOGIN_LOCK_FLG = LockFlag.NON_LOCK;
                entity.GENDER = true;
                entity.DEL_FLG = DeleteFlag.NON_DELETE;
                entity.STATUS = model.STATUS;
                entity.INS_DATE = Utility.GetCurrentDateTime();
                entity.UPD_DATE = Utility.GetCurrentDateTime();

                res = dataAccess.InsertUser(entity);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }

        public long UpdateUser(UserAccountModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            ManageUserDa dataAccess = new ManageUserDa();
            using (var transaction = new TransactionScope())
            {
                TblUserAccount entity = new TblUserAccount();
                entity.USER_ID = model.USER_ID = model.USER_ID_HIDDEN;
                entity.USER_NAME = model.USER_NAME;
                entity.SHOP_NAME = model.SHOP_NAME;
                entity.AREA = model.AREA;
                entity.USER_AUTHORITY = model.USER_AUTHORITY;
                entity.USER_CITY = model.USER_CITY;
                entity.USER_DISTRICT = model.USER_DISTRICT;
                entity.USER_TOWN = model.USER_TOWN;
                entity.USER_ADDRESS = model.USER_ADDRESS;
                entity.USER_PHONE = model.USER_PHONE;
                entity.LOGIN_LOCK_FLG = model.LOGIN_LOCK_FLG;
                entity.DEL_FLG = DeleteFlag.NON_DELETE;
                entity.STATUS = model.STATUS;
                entity.INS_DATE = Utility.GetCurrentDateTime();
                entity.PASSWORD_LAST_UPDATE_DATE = Utility.GetCurrentDateTime();
                entity.UPD_DATE= Utility.GetCurrentDateTime();

                res = dataAccess.UpdateUser(entity);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }
        #endregion

        #region DELETE
        public bool DeleteUser(long USER_ID = 0)
        {
            ManageUserDa dataAccess = new ManageUserDa();

            int result = 0;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    result = dataAccess.DeleteUser(USER_ID);

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