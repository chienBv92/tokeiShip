using ShipOnline.BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using ShipOnline.Models.Define;
using ShipOnline.DataAccess;
using System.Transactions;
using ShipOnline.UtilityServices.SafePassword;
using ShipOnline.Resources;
using ShipOnline.UtilityService;

namespace ShipOnline.Services
{
    public class UserAccountService:BaseServices
    {
        #region LOGIN
        public UserAccountModel getInfoUser(UserAccountModel model)
        {
            UserAccountDa dataAccess = new UserAccountDa();
            UserAccountModel user = dataAccess.getInfoUser(model);
            return user;
        }
        #endregion

        #region REGIST/ UPDATE
        public long InsertUser(UserAccountModel model)
        {
            long res = 0;
            // Declare new DataAccess object
            UserAccountDa dataAccess = new UserAccountDa();
            using (var transaction = new TransactionScope())
            {
                TblUserAccount entity = new TblUserAccount();
                entity.USER_EMAIL = model.USER_EMAIL;
                entity.EMAIL_CONFIRMED = EmailConfirmed.None;
                entity.USER_NAME = model.USER_NAME;
                entity.SHOP_NAME = model.SHOP_NAME;
                entity.USER_PASSWORD = SafePassword.GetSaltedPassword(model.USER_PASSWORD);
                entity.PASSWORD_LAST_UPDATE_DATE = Utility.GetCurrentDateTime();
                entity.USER_AUTHORITY = User_Authority.Set_Person;
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
                entity.STATUS = StatusFlag.NON_DISPLAY; // user chua duoc active
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
            UserAccountDa dataAccess = new UserAccountDa();
            using (var transaction = new TransactionScope())
            {
                TblUserAccount entity = new TblUserAccount();

                entity.USER_ID = model.USER_ID = model.USER_ID_HIDDEN;
                entity.USER_NAME = model.USER_NAME;
                entity.SHOP_NAME = model.SHOP_NAME;
                entity.AREA = model.AREA;
                entity.USER_CITY = model.USER_CITY;
                entity.USER_DISTRICT = model.USER_DISTRICT;
                entity.USER_TOWN = model.USER_TOWN;
                entity.USER_ADDRESS = model.USER_ADDRESS;
                entity.USER_PHONE = model.USER_PHONE;
                entity.INS_DATE = Utility.GetCurrentDateTime();
                entity.PASSWORD_LAST_UPDATE_DATE = Utility.GetCurrentDateTime();
                entity.UPD_DATE = Utility.GetCurrentDateTime();

                res = dataAccess.UpdateUser(entity);
                if (res <= 0)
                    transaction.Dispose();
                transaction.Complete();
            }
            return res;
        }
        #endregion

    }
}