using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using ShipOnline.Models.Define;
using ShipOnline.Resources;
using System.Text;
using ShipOnline.UtilityService;
using SystemSetup.UtilityServices.PagingHelper;
using ShipOnline.UtilityServices.SafePassword;

namespace ShipOnline.DataAccess
{
    public class UserAccountDa:BaseDa
    {
        #region LOGIN
        public UserAccountModel getInfoUser(UserAccountModel model)
        {
            string sql = @"
                SELECT  A.*, B.CITY_NAME, C.DISTRICT_NAME, D.TOWN_NAME
                FROM TblUserAccount A
                LEFT JOIN MstCity B
                ON A.USER_CITY = B.CITY_CD
                LEFT JOIN MstDistrict C
                ON A.USER_CITY = C.CITY_CD AND A.USER_DISTRICT = C.DISTRICT_CD
                LEFT JOIN MstTown D
                ON A.USER_CITY = D.CITY_CD AND A.USER_DISTRICT = D.DISTRICT_CD AND A.USER_TOWN = D.TOWN_CD

                WHERE   A.USER_EMAIL = @USER_EMAIL AND A.USER_PASSWORD = @USER_PASSWORD
                AND A.DEL_FLG = @DEL_FLG";

            return base.SingleOrDefault<UserAccountModel>(sql.ToString(), new
            {
                USER_EMAIL = model.USER_EMAIL,
                USER_PASSWORD = SafePassword.GetSaltedPassword(model.USER_PASSWORD),
                DEL_FLG = DeleteFlag.NON_DELETE
            });
        }
        #endregion

        #region REGIST/ UPDATE
        public bool CheckExistUserAccount(string userEmail)
        {
            string sql = @"
                SELECT  COUNT(*)
                FROM    [TblUserAccount]
                WHERE   USER_EMAIL = @USER_EMAIL AND DEL_FLG = @DEL_FLG";

            int count = base.SingleOrDefault<int>(sql, new
            {
                USER_EMAIL = userEmail,
                DEL_FLG = DeleteFlag.NON_DELETE
            });
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public long InsertUser(TblUserAccount entity)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();

            sqlinsert.Append(@" 
                    INSERT INTO [TblUserAccount] 
                        ([USER_EMAIL]
                        ,[EMAIL_CONFIRMED]
                        ,[USER_NAME]
                        ,[SHOP_NAME]
                        ,[AREA]
                        ,[USER_PASSWORD]
                        ,[PASSWORD_LAST_UPDATE_DATE]
                        ,[USER_AUTHORITY]
                        ,[USER_CITY]
                        ,[USER_DISTRICT]
                        ,[USER_TOWN]
                        ,[USER_ADDRESS]
                        ,[USER_PHONE]
                        ,[USER_FAMILY]
                        ,[LOGIN_LOCK_FLG]
                        ,[GENDER]
                        ,[DEL_FLG]
                        ,[STATUS]
                        ,[INS_DATE]
                        ,[UPD_DATE])
                    VALUES
                        (@USER_EMAIL,
                        @EMAIL_CONFIRMED,
                        @USER_NAME,
                        @SHOP_NAME,
                        @AREA,
                        @USER_PASSWORD,
                        @PASSWORD_LAST_UPDATE_DATE,
                        @USER_AUTHORITY,
                        @USER_CITY,
                        @USER_DISTRICT,
                        @USER_TOWN,
                        @USER_ADDRESS,
                        @USER_PHONE,
                        @USER_FAMILY,
                        @LOGIN_LOCK_FLG,
                        @GENDER,
                        @DEL_FLG,
                        @STATUS,
                        @INS_DATE,
                        @UPD_DATE)");

            result = base.DbAdd(sqlinsert.ToString(), entity);
            return result;
        }

        public long UpdateUser(TblUserAccount entity)
        {
            long result = 0;
            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();

            sqlinsert.Append(@" 
                    UPDATE [dbo].[TblUserAccount]
                    SET [USER_NAME] = @USER_NAME
                    ,[USER_CITY] = @USER_CITY
                    ,[USER_DISTRICT] = @USER_DISTRICT
                    ,[USER_TOWN] = @USER_TOWN
                    ,[USER_ADDRESS] = @USER_ADDRESS
                    ,[USER_PHONE] = @USER_PHONE
                    ,[SHOP_NAME] = @SHOP_NAME
                    ,[AREA] = @AREA
                    ,[UPD_DATE] = @UPD_DATE
                    WHERE [USER_ID] = @USER_ID");

            result = base.DbUpdate(sqlinsert.ToString(), entity, new { USER_ID = entity.USER_ID });
            return result;

        }
        #endregion

    }
}