using ShipOnline.Models.Define;
using ShipOnline.Models.Entity;
using ShipOnline.Models.Extend;
using ShipOnline.Resources;
using ShipOnline.UtilityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SystemSetup.UtilityServices.PagingHelper;

namespace ShipOnline.DataAccess
{
    public class ManageDistrictDa:BaseDa
    {

        #region REGIST/ UPDATE
        public bool CheckExistDistrictCd(int city_Cd, int districtCd, int districtCdOld)
        {
            string sql = @"
                SELECT  COUNT(*)
                FROM    [MstDistrict]
                WHERE   CITY_CD = @CITY_CD AND DISTRICT_CD = @DISTRICT_CD";

            int count = base.SingleOrDefault<int>(sql, new
            {
                CITY_CD = city_Cd,
                DISTRICT_CD = districtCd
            });
            if (count > 0 && districtCdOld == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public long InsertDistrict(DistrictModel model)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();
            model.DEL_FLG = DeleteFlag.NON_DELETE;

            sqlinsert.Append(@" 
                    INSERT INTO [MstDistrict] 
                        ([CITY_CD]
                        ,[DISTRICT_CD]
                        ,[DISTRICT_NAME]
                        ,[DSP_ORDER]
                        ,[DEL_FLG]
                        ,[STATUS]
                        ,[INSIDE])
                    VALUES
                        (@CITY_CD,
                        @DISTRICT_CD,
                        @DISTRICT_NAME,
                        @DSP_ORDER,
                        @DEL_FLG,
                        @STATUS,
                        @INSIDE)");

            result = base.DbAdd(sqlinsert.ToString(), model);
            return result;
        }

        public long UpdateDistrict(DistrictModel model)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();
            model.DEL_FLG = DeleteFlag.NON_DELETE;

            sqlinsert.Append(@" 
                    UPDATE [dbo].[MstDistrict]
                    SET [DISTRICT_NAME] = @DISTRICT_NAME
                    ,[DSP_ORDER] = @DSP_ORDER
                    ,[DEL_FLG] = @DEL_FLG
                    ,[STATUS] = @STATUS
                    ,[INSIDE] = @INSIDE
                    WHERE [CITY_CD] = @CITY_CD AND DISTRICT_CD = @DISTRICT_CD");

            result = base.DbUpdate(sqlinsert.ToString(), model, new { CITY_CD = model.CITY_CD, DISTRICT_CD = model.DISTRICT_CD });
            return result;

        }
        #endregion

        #region LIST
        public IEnumerable<MstDistrictEx> SearchDistrictList(DataTableModel dt, DistrictModel model, out int total_row)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
                SELECT A. *, B.CITY_NAME
                FROM MstDistrict A
                LEFT JOIN MstCity B
                ON A.CITY_CD = B.CITY_CD
                WHERE
                    A.DEL_FLG = @DEL_FLG AND A.INSIDE = @INSIDE");
            if (model.CITY_CD > 0)
            {
                sql.Append(" AND    (A.CITY_CD LIKE @CITY_CD)");
            }

            if (!string.IsNullOrEmpty(model.DISTRICT_NAME))
            {
                sql.Append(" AND    (A.DISTRICT_NAME LIKE @DISTRICT_NAME)");
            }

            int lower = dt.iDisplayStart + 1;
            int upper = dt.iDisplayStart + dt.iDisplayLength;

            PagingHelper.SQLParts parts;
            PagingHelper.SplitSQL(sql.ToString(), out parts);

            string sqlpage = PagingHelper.BuildPageQuery(lower, dt.iDisplayLength, parts);
            string sqlcount = parts.sqlCount;

            var dataList = base.Query<MstDistrictEx>(sqlpage,
                new
                {
                    DEL_FLG = model.DEL_FLG,
                    INSIDE = model.INSIDE,
                    CITY_CD = model.CITY_CD,
                    DISTRICT_NAME = '%' + model.DISTRICT_NAME + '%',
                    pageindex = lower,
                    pagesize = upper
                }).ToList();

            total_row = base.Query<int>(sqlcount,
              new
              {
                  DEL_FLG = model.DEL_FLG,
                  INSIDE = model.INSIDE,
                  CITY_CD = model.CITY_CD,
                  DISTRICT_NAME = '%' + model.DISTRICT_NAME + '%',
                  pageindex = lower,
                  pagesize = upper
              }).FirstOrDefault();

            return dataList;

        }

        public DistrictModel getInfoDistrict(int CityCd, int DistrictCd)
        {
            string sql = @"
                SELECT  A.*, B.CITY_NAME
                FROM    [MstDistrict] A
                LEFT JOIN MstCity B
                ON A.CITY_CD = B.CITY_CD
                WHERE   A.CITY_CD = @CITY_CD AND A.DISTRICT_CD = @DISTRICT_CD";

            return base.SingleOrDefault<DistrictModel>(sql.ToString(), new
            {
                CITY_CD = CityCd,
                DISTRICT_CD = DistrictCd
            });
        }
        #endregion

        #region DELETE
        public int DeleteDistrict(long CITY_CD, long DISTRICT_CD)
        {
            int result = 0;

            var currentuser = base.CmnEntityModel;
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
            UPDATE MstDistrict
                SET DEL_FLG = @DEL_FLG
            WHERE 
                CITY_CD = @CITY_CD AND DISTRICT_CD = @DISTRICT_CD ");

            result = base.Execute(sql.ToString(), new
            {
                DEL_FLG = DeleteFlag.DELETE,
                CITY_CD = CITY_CD,
                DISTRICT_CD = DISTRICT_CD
            });

            return result;
        }

        public int DeleteTownInArea(long cityCd = 0, long districtCd = 0, long townCD = 0, int forUser = 0)
        {
            int result = 0;

            var currentuser = base.CmnEntityModel;
            StringBuilder sql = new StringBuilder();

            sql.Append(@" 
                    UPDATE [dbo].[MstTown] ");

            if (forUser == GroupForUser.Receive)
            {
                sql.Append("SET GROUP_CD_RECEIVE = @GROUP_CD, DSP_ORDER_RECEIVE = @DSP_ORDER, UPD_DATE = @UPD_DATE ");
            }
            else
            {
                sql.Append("SET GROUP_CD_SENDER = @GROUP_CD, DSP_ORDER_SENDER = @DSP_ORDER, UPD_DATE = @UPD_DATE ");
            }
            sql.Append("WHERE [CITY_CD] = @CITY_CD AND DISTRICT_CD = @DISTRICT_CD AND TOWN_CD = @TOWN_CD");

            result = base.Execute(sql.ToString(), new
            {
                CITY_CD = cityCd,
                DISTRICT_CD = districtCd,
                TOWN_CD = townCD,
                GROUP_CD = GroupCdArea.NON_SET,
                DSP_ORDER = OrderDsp.NON_SET,
                UPD_DATE = Utility.GetCurrentDateTime()
            });

            return result;
        }
        /// <summary>
        /// Delete Group Area
        /// </summary>
        /// <param name="groupCd"></param>
        /// <param name="forUser"></param>
        /// <returns></returns>
        public int DeleteGroupArea(long groupCd = 0, int forUser = 0)
        {
            int result = 0;

            var currentuser = base.CmnEntityModel;
            StringBuilder sql = new StringBuilder();

            sql.Append(@" DELETE
                    FROM [dbo].[MstGroupArea] ");

            sql.Append("WHERE [GROUP_CD] = @GROUP_CD ");

            result = base.Execute(sql.ToString(), new
            {
                DEL_FLG = DeleteFlag.DELETE,
                GROUP_CD = groupCd,
                UPD_DATE = Utility.GetCurrentDateTime()
            });

            return result;
        }

        /// <summary>
        /// Delete All Town In Group Area
        /// </summary>
        /// <param name="groupCd"></param>
        /// <param name="forUser"></param>
        /// <returns></returns>
        public int DeleteAllTownInArea(long groupCd = 0, int forUser = 0)
        {
            int result = 0;

            var currentuser = base.CmnEntityModel;
            StringBuilder sql = new StringBuilder();

            sql.Append(@" 
                    UPDATE [dbo].[MstTown] ");

            if (forUser == GroupForUser.Receive)
            {
                sql.Append("SET GROUP_CD_RECEIVE = @GROUP_CD_SET, DSP_ORDER_RECEIVE = @DSP_ORDER, UPD_DATE = @UPD_DATE ");
                sql.Append("WHERE [GROUP_CD_RECEIVE] = @GROUP_CD ");
            }
            else
            {
                sql.Append("SET GROUP_CD_SENDER = @GROUP_CD_SET, DSP_ORDER_SENDER = @DSP_ORDER, UPD_DATE = @UPD_DATE ");
                sql.Append("WHERE [GROUP_CD_SENDER] = @GROUP_CD ");
            }
            

            result = base.Execute(sql.ToString(), new
            {
                GROUP_CD_SET = GroupCdArea.NON_SET,
                GROUP_CD = groupCd,
                DSP_ORDER = OrderDsp.NON_SET,
                UPD_DATE = Utility.GetCurrentDateTime()
            });

            return result;
        }

        #endregion

        #region REGIST/ UPDATE GROUP AREA

        public long InsertGroupArea(MstGroupArea entity)
        {
            //Check create new customer
            StringBuilder sql = new StringBuilder();

            sql.Append(@" 
                    INSERT INTO [MstGroupArea] 
                        ([GROUP_NAME]
                        ,[FOR_USER]
                        ,[DEL_FLG]
                        ,[INS_DATE]
                        ,[UPD_DATE])
                    VALUES
                        (@GROUP_NAME,
                        @FOR_USER,
                        @DEL_FLG,
                        @INS_DATE,
                        @UPD_DATE)");

            var groupCd = base.DbAddGetIdentity(sql.ToString(), entity);

            return groupCd;
        }
        /// <summary>
        /// Update Town in Area
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long UpdateTownInArea(GroupTown model, int forUser)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sql = new StringBuilder();
            model.DEL_FLG = DeleteFlag.NON_DELETE;

            sql.Append(@" 
                    UPDATE [dbo].[MstTown] ");
                  
            if (forUser == GroupForUser.Receive)
            {
                sql.Append("SET GROUP_CD_RECEIVE = @GROUP_CD, DSP_ORDER_RECEIVE = @DSP_ORDER ");
            }
            else
            {
                sql.Append("SET GROUP_CD_SENDER = @GROUP_CD, DSP_ORDER_SENDER = @DSP_ORDER ");
            }
            sql.Append("WHERE [CITY_CD] = @CITY_CD AND DISTRICT_CD = @DISTRICT_CD AND TOWN_CD = @TOWN_CD");

            result = base.DbUpdate(sql.ToString(), model, new { CITY_CD = model.CITY_CD, DISTRICT_CD = model.DISTRICT_CD, TOWN_CD = model.TOWN_CD });
            return result;

        }

        public long UpdateGroupArea(MstGroupArea entity)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();

            sqlinsert.Append(@" 
                    UPDATE [dbo].[MstGroupArea]
                    SET [GROUP_NAME] = @GROUP_NAME
                    ,[UPD_DATE] = @UPD_DATE
                   
                    WHERE [GROUP_CD] = @GROUP_CD ");

            result = base.DbUpdate(sqlinsert.ToString(), entity, new { GROUP_CD = entity.GROUP_CD });
            return result;

        }
        #endregion

        #region LIST GROUP AREA
        public IEnumerable<MstGroupArea> SearchGroupAreatList(DataTableModel dt, GroupAreaModel model, out int total_row)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
                SELECT A. *
                FROM MstGroupArea A
               
                WHERE
                    A.DEL_FLG = @DEL_FLG AND A.FOR_USER = @FOR_USER");

            if (!string.IsNullOrEmpty(model.GROUP_NAME))
            {
                sql.Append(" AND    (A.GROUP_NAME LIKE @GROUP_NAME)");
            }
            if (!string.IsNullOrEmpty(model.GROUP_CD_LIST))
            {
                sql.Append(" AND A.GROUP_CD IN ('" + model.GROUP_CD_LIST + "')");
            }
            else if (model.CITY_CD > 0 || model.DISTRICT_CD > 0 || model.TOWN_CD > 0)
            {
                sql.Append(" AND A.GROUP_CD IN ('')");
            }
            sql.Append(" ORDER BY A.GROUP_NAME ASC, A.UPD_DATE DESC ");

            int lower = dt.iDisplayStart + 1;
            int upper = dt.iDisplayStart + dt.iDisplayLength;

            PagingHelper.SQLParts parts;
            PagingHelper.SplitSQL(sql.ToString(), out parts);

            string sqlpage = PagingHelper.BuildPageQuery(lower, dt.iDisplayLength, parts);
            string sqlcount = parts.sqlCount;

            var dataList = base.Query<MstGroupArea>(sqlpage,
                new
                {
                    DEL_FLG = model.DEL_FLG,
                    GROUP_NAME = '%' + model.GROUP_NAME + '%',
                    FOR_USER = model.FOR_USER,
                    pageindex = lower,
                    pagesize = upper
                }).ToList();

            total_row = base.Query<int>(sqlcount,
              new
              {
                  DEL_FLG = model.DEL_FLG,
                  GROUP_NAME = '%' + model.GROUP_NAME + '%',
                  FOR_USER = model.FOR_USER,
                  pageindex = lower,
                  pagesize = upper
              }).FirstOrDefault();

            return dataList;

        }

        public GroupAreaModel getInfoGroupArea(long GroupCd)
        {
            string sql = @"
                SELECT *
                FROM    [MstGroupArea] 
                WHERE  GROUP_CD = @GROUP_CD";

            return base.SingleOrDefault<GroupAreaModel>(sql.ToString(), new
            {
                GROUP_CD = GroupCd,
            });
        }

        public IEnumerable<GroupTown> GetListTownInArea(GroupAreaModel condition)
        {
            // Declare database connection      
            StringBuilder sql = new StringBuilder();
            // SQL発行  
            sql.Append(@" SELECT A. *, B.CITY_NAME, C.DISTRICT_NAME
                        FROM MstTown A
                        LEFT JOIN MstCity B
                        ON A.CITY_CD = B.CITY_CD
                        LEFT JOIN MstDistrict C
                        ON A.CITY_CD = C.CITY_CD AND A.DISTRICT_CD = C.DISTRICT_CD
                        WHERE A.DEL_FLG = @DEL_FLG ");

            if (condition.FOR_USER == GroupForUser.Receive)
            {
                sql.Append("AND GROUP_CD_RECEIVE = @GROUP_CD ");
            }
            else
            {
                sql.Append("AND GROUP_CD_SENDER = @GROUP_CD ");
            }

            return base.Query<GroupTown>(sql.ToString(),
                new
                {
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    GROUP_CD = condition.GROUP_CD
                }).ToList();
        }

        public IEnumerable<long> getListGroupForSearch(GroupAreaModel condition)
        {
            // Declare database connection      
            StringBuilder sql = new StringBuilder();
            // SQL発行  

            if (condition.FOR_USER == GroupForUser.Receive)
            {
                sql.Append(@" SELECT DISTINCT GROUP_CD_RECEIVE AS GROUP_CD
                        FROM MstTown 
                        WHERE GROUP_CD_RECEIVE > 0 AND [DEL_FLG] = @DEL_FLG ");
                if(condition.CITY_CD > 0){
                    sql.Append("AND [CITY_CD] = @CITY_CD ");
                }
                if (condition.CITY_CD > 0 && condition.DISTRICT_CD > 0)
                {
                    sql.Append("AND [DISTRICT_CD] = @DISTRICT_CD ");
                }
                if (condition.CITY_CD > 0 && condition.DISTRICT_CD > 0 && condition.TOWN_CD > 0)
                {
                    sql.Append("AND [TOWN_CD] = @TOWN_CD ");
                }
            }
            else
            {
                sql.Append(@" SELECT DISTINCT [GROUP_CD_SENDER] AS GROUP_CD
                        FROM MstTown 
                        WHERE [GROUP_CD_SENDER] > 0 AND [DEL_FLG] = @DEL_FLG ");
                if (condition.CITY_CD > 0)
                {
                    sql.Append("AND [CITY_CD] = @CITY_CD ");
                }
                if (condition.CITY_CD > 0 && condition.DISTRICT_CD > 0)
                {
                    sql.Append("AND [DISTRICT_CD] = @DISTRICT_CD ");
                }
                if (condition.CITY_CD > 0 && condition.DISTRICT_CD > 0 && condition.TOWN_CD > 0)
                {
                    sql.Append("AND [TOWN_CD] = @TOWN_CD ");
                }
            }

            return base.Query<long>(sql.ToString(),
                new
                {
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    CITY_CD = condition.CITY_CD,
                    DISTRICT_CD = condition.DISTRICT_CD,
                    TOWN_CD = condition.TOWN_CD
                }).ToList();
        }
        #endregion
    }
}