using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Define;
using ShipOnline.Models.Entity;
using ShipOnline.Models.Extend;
using ShipOnline.Resources;
using SystemSetup.UtilityServices.PagingHelper;
using System.Text;
using ShipOnline.UtilityServices.SafePassword;
using ShipOnline.UtilityService;
namespace ShipOnline.DataAccess
{
    public class ManageUserDa:BaseDa
    {
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
                    ,[USER_AUTHORITY] = @USER_AUTHORITY
                    ,[USER_CITY] = @USER_CITY
                    ,[USER_DISTRICT] = @USER_DISTRICT
                    ,[USER_TOWN] = @USER_TOWN
                    ,[USER_ADDRESS] = @USER_ADDRESS
                    ,[USER_PHONE] = @USER_PHONE
                    ,[SHOP_NAME] = @SHOP_NAME
                    ,[AREA] = @AREA
                    ,[LOGIN_LOCK_FLG] = @LOGIN_LOCK_FLG
                    ,[DEL_FLG] = @DEL_FLG
                    ,[STATUS] = @STATUS
                    ,[UPD_DATE] = @UPD_DATE
                    WHERE [USER_ID] = @USER_ID");

            result = base.DbUpdate(sqlinsert.ToString(), entity, new { USER_ID = entity.USER_ID });
            return result;

        }
        #endregion


        #region LIST
        public IEnumerable<TblUserAccountEx> SearchUserList(DataTableModel dt, UserAccountModel model, out int total_row)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
                SELECT A. *, B.CITY_NAME, C.DISTRICT_NAME, T.TOWN_NAME
                FROM TblUserAccount A
                LEFT JOIN MstCity B
                ON A.USER_CITY = B.CITY_CD
                LEFT JOIN MstDistrict C
                ON A.USER_CITY = C.CITY_CD AND A.USER_DISTRICT = C.DISTRICT_CD
                LEFT JOIN MstTown T
                ON A.USER_CITY = T.CITY_CD AND A.USER_DISTRICT = T.DISTRICT_CD AND A.USER_TOWN = T.TOWN_CD
                WHERE
                    A.DEL_FLG = @DEL_FLG 
                    AND A.LOGIN_LOCK_FLG = @LOGIN_LOCK_FLG");
            if (model.CITY_CD_SEARCH > 0)
            {
                sql.Append(" AND    (A.USER_CITY LIKE @CITY_CD_SEARCH)");
            }

            if (model.DISTRICT_CD_SEARCH > 0)
            {
                sql.Append(" AND    (A.USER_DISTRICT LIKE @DISTRICT_CD_SEARCH)");
            }
            if (model.TOWN_CD_SEARCH > 0)
            {
                sql.Append(" AND    (A.USER_TOWN LIKE @TOWN_CD_SEARCH)");
            }
            if (!string.IsNullOrEmpty(model.USER_ADDRESS))
            {
                sql.Append(" AND    (A.USER_ADDRESS LIKE @USER_ADDRESS)");
            }

            if (!string.IsNullOrEmpty(model.USER_EMAIL))
            {
                sql.Append(" AND    (A.USER_EMAIL LIKE @USER_EMAIL)");
            }
            if (!string.IsNullOrEmpty(model.USER_NAME))
            {
                sql.Append(" AND    (A.USER_NAME LIKE @USER_NAME)");
            }
            if (!string.IsNullOrEmpty(model.SHOP_NAME))
            {
                sql.Append(" AND    (A.SHOP_NAME LIKE @SHOP_NAME)");
            }
            if (!string.IsNullOrEmpty(model.USER_PHONE))
            {
                sql.Append(" AND    (A.USER_PHONE LIKE @USER_PHONE)");
            }

            if (model.AREA > 0)
            {
                sql.Append(" AND (A.AREA = @AREA)");
            }

            sql.Append(" AND    (A.USER_AUTHORITY = @USER_AUTHORITY)");

            sql.Append(" ORDER BY A.USER_EMAIL asc, A.USER_NAME asc, A.UPD_DATE desc");

            int lower = dt.iDisplayStart + 1;
            int upper = dt.iDisplayStart + dt.iDisplayLength;

            PagingHelper.SQLParts parts;
            PagingHelper.SplitSQL(sql.ToString(), out parts);

            string sqlpage = PagingHelper.BuildPageQuery(lower, dt.iDisplayLength, parts);
            string sqlcount = parts.sqlCount;

            var dataList = base.Query<TblUserAccountEx>(sqlpage,
                new
                {
                    DEL_FLG = model.DEL_FLG,
                    LOGIN_LOCK_FLG = model.LOGIN_LOCK_FLG,
                    CITY_CD_SEARCH = model.CITY_CD_SEARCH,
                    DISTRICT_CD_SEARCH = model.DISTRICT_CD_SEARCH,
                    TOWN_CD_SEARCH = model.TOWN_CD_SEARCH,
                    USER_ADDRESS = '%' + model.USER_ADDRESS + '%',
                    USER_NAME = '%' + model.USER_NAME + '%',
                    SHOP_NAME = '%' + model.SHOP_NAME + '%',
                    AREA = model.AREA,
                    USER_PHONE = model.USER_PHONE + '%',
                    USER_EMAIL = '%' + model.USER_EMAIL + '%',
                    USER_AUTHORITY = model.USER_AUTHORITY,
                    pageindex = lower,
                    pagesize = upper
                }).ToList();

            total_row = base.Query<int>(sqlcount,
              new
              {
                  DEL_FLG = model.DEL_FLG,
                  LOGIN_LOCK_FLG = model.LOGIN_LOCK_FLG,
                  CITY_CD_SEARCH = model.CITY_CD_SEARCH,
                  DISTRICT_CD_SEARCH = model.DISTRICT_CD_SEARCH,
                  TOWN_CD_SEARCH = model.TOWN_CD_SEARCH,
                  USER_ADDRESS = '%' + model.USER_ADDRESS + '%',
                  USER_NAME = '%' + model.USER_NAME + '%',
                  SHOP_NAME = '%' + model.SHOP_NAME + '%',
                  AREA = model.AREA,
                  USER_PHONE = model.USER_PHONE + '%',
                  USER_EMAIL = '%' + model.USER_EMAIL + '%',
                  USER_AUTHORITY = model.USER_AUTHORITY,
                  pageindex = lower,
                  pagesize = upper
              }).FirstOrDefault();

            return dataList;

        }

        public UserAccountModel getInfoUser(long UserId)
        {
            string sql = @"
                SELECT  A.*, 
                B.CITY_NAME, C.DISTRICT_NAME, T.TOWN_NAME
                FROM TblUserAccount A
                LEFT JOIN MstCity B
                ON A.USER_CITY = B.CITY_CD
                LEFT JOIN MstDistrict C
                ON A.USER_CITY = C.CITY_CD AND A.USER_DISTRICT = C.DISTRICT_CD
                LEFT JOIN MstTown T
                ON A.USER_CITY = T.CITY_CD AND A.USER_DISTRICT = T.DISTRICT_CD AND A.USER_TOWN = T.TOWN_CD

                WHERE   A.USER_ID = @USER_ID";

            return base.SingleOrDefault<UserAccountModel>(sql.ToString(), new
            {
                USER_ID = UserId
            });
        }
        #endregion

        #region DELETE
        public int DeleteUser(long USER_ID = 0)
        {
            int result = 0;

            var currentuser = base.CmnEntityModel;
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
            UPDATE TblUserAccount
                SET DEL_FLG = @DEL_FLG
            WHERE 
                USER_ID = @USER_ID");

            result = base.Execute(sql.ToString(), new
            {
                DEL_FLG = DeleteFlag.DELETE,
                USER_ID = USER_ID
            });

            return result;
        }

        #endregion


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

        // Xác nhận tài khoản
        public bool ConfirmEmail(long userId)
        {
            long result = 0;
            StringBuilder sqlinsert = new StringBuilder();
            TblUserAccount entity = new TblUserAccount();

            sqlinsert.Append(@" 
                    UPDATE [TblUserAccount]
                    SET [EMAIL_CONFIRMED] = @EMAIL_CONFIRMED
                        ,[UPD_DATE] = @UPD_DATE
                    WHERE [USER_ID] = @USER_ID");

            result =  base.Execute(sqlinsert.ToString(), new
            {
                USER_ID = userId,
                EMAIL_CONFIRMED = EmailConfirmed.Yes,
                UPD_DATE =  Utility.GetCurrentDateTime()
                
            });

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public long ReSetPassword(long userId)
        {
            StringBuilder sqlinsert = new StringBuilder();
            TblUserAccount entity = new TblUserAccount();
            string password = "123456";
            password = SafePassword.GetSaltedPassword(password);
            sqlinsert.Append(@" 
                    UPDATE [TblUserAccount]
                    SET [EMAIL_CONFIRMED] = @EMAIL_CONFIRMED
                        ,[USER_PASSWORD] = @USER_PASSWORD
                        ,[UPD_DATE] = @UPD_DATE
                    WHERE [USER_ID] = @USER_ID");

            return base.Execute(sqlinsert.ToString(), new
            {
                USER_ID = userId,
                EMAIL_CONFIRMED = EmailConfirmed.RePassword,
                USER_PASSWORD = password,
                UPD_DATE = Utility.GetCurrentDateTime()

            });
        }

        public long selectUserId(string userEmail)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@" 
                    SELECT * FROM [TblUserAccount]
                    WHERE [USER_EMAIL] = @USER_EMAIL");

            return base.SingleOrDefault<TblUserAccount>(sql.ToString(), new
            {
                USER_EMAIL = userEmail
            }).USER_ID;
           
        }

        #endregion

    }
}