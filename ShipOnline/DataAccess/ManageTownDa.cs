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
using ShipOnline.UtilityService;

namespace ShipOnline.DataAccess
{
    public class ManageTownDa:BaseDa
    {
        #region REGIST/ UPDATE
        public bool CheckExistTownCd(int city_Cd, int districtCd, int townCD, int townCdOld)
        {
            string sql = @"
                SELECT  COUNT(*)
                FROM    [MstTown]
                WHERE   CITY_CD = @CITY_CD AND DISTRICT_CD = @DISTRICT_CD AND TOWN_CD = @TOWN_CD";

            int count = base.SingleOrDefault<int>(sql, new
            {
                CITY_CD = city_Cd,
                DISTRICT_CD = districtCd,
                TOWN_CD = townCD
            });
            if (count > 0 && townCdOld == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public long InsertTown(TownModel model)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();
            model.DEL_FLG = DeleteFlag.NON_DELETE;
            model.INS_DATE = Utility.GetCurrentDateTime();

            sqlinsert.Append(@" 
                    INSERT INTO [MstTown] 
                        ([CITY_CD]
                        ,[DISTRICT_CD]
                        ,[TOWN_CD]
                        ,[TOWN_NAME]
                        ,[DEL_FLG]
                        ,[INS_DATE]
                        ,[STATUS])
                    VALUES
                        (@CITY_CD,
                        @DISTRICT_CD,
                        @TOWN_CD,
                        @TOWN_NAME,
                        @DEL_FLG,
                        @INS_DATE,
                        @STATUS)");

            result = base.DbAdd(sqlinsert.ToString(), model);
            return result;
        }

        public long UpdateTown(TownModel model)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();
            model.DEL_FLG = DeleteFlag.NON_DELETE;
            model.UPD_DATE = Utility.GetCurrentDateTime();

            sqlinsert.Append(@" 
                    UPDATE [dbo].[MstTown]
                    SET [TOWN_NAME] = @TOWN_NAME
                    ,[DEL_FLG] = @DEL_FLG
                    ,[UPD_DATE] = @UPD_DATE
                    ,[STATUS] = @STATUS
                    WHERE [CITY_CD] = @CITY_CD_HIDDEN AND DISTRICT_CD = @DISTRICT_CD_HIDDEN AND TOWN_CD = @TOWN_CD_HIDDEN");

            result = base.DbUpdate(sqlinsert.ToString(), model, new { CITY_CD_HIDDEN = model.CITY_CD_HIDDEN, DISTRICT_CD_HIDDEN = model.DISTRICT_CD_HIDDEN, TOWN_CD_HIDDEN = model.TOWN_CD_HIDDEN });
            return result;

        }
        #endregion

        #region LIST
        public IEnumerable<MstTownEx> SearchTownList(DataTableModel dt, TownModel model, out int total_row)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
                SELECT A. *, B.CITY_NAME, C.DISTRICT_NAME
                FROM MstTown A
                LEFT JOIN MstCity B
                ON A.CITY_CD = B.CITY_CD
                LEFT JOIN MstDistrict C
                ON A.CITY_CD = C.CITY_CD AND A.DISTRICT_CD = C.DISTRICT_CD
                WHERE
                    A.DEL_FLG = @DEL_FLG ");
            if (model.CITY_CD_SEARCH > 0)
            {
                sql.Append(" AND    (A.CITY_CD LIKE @CITY_CD_SEARCH)");
            }

            if (model.DISTRICT_CD_SEARCH > 0)
            {
                sql.Append(" AND    (A.DISTRICT_CD LIKE @DISTRICT_CD_SEARCH)");
            }

            if (!string.IsNullOrEmpty(model.TOWN_NAME))
            {
                sql.Append(" AND    (A.TOWN_NAME LIKE @TOWN_NAME)");
            }

            sql.Append(" ORDER BY CITY_NAME asc, DISTRICT_NAME asc, TOWN_NAME asc, UPD_DATE desc");

            int lower = dt.iDisplayStart + 1;
            int upper = dt.iDisplayStart + dt.iDisplayLength;

            PagingHelper.SQLParts parts;
            PagingHelper.SplitSQL(sql.ToString(), out parts);

            string sqlpage = PagingHelper.BuildPageQuery(lower, dt.iDisplayLength, parts);
            string sqlcount = parts.sqlCount;

            var dataList = base.Query<MstTownEx>(sqlpage,
                new
                {
                    DEL_FLG = model.DEL_FLG,
                    CITY_CD_SEARCH = model.CITY_CD_SEARCH,
                    DISTRICT_CD_SEARCH = model.DISTRICT_CD_SEARCH,
                    TOWN_NAME = '%' + model.TOWN_NAME + '%',
                    pageindex = lower,
                    pagesize = upper
                }).ToList();

            total_row = base.Query<int>(sqlcount,
              new
              {
                  DEL_FLG = model.DEL_FLG,
                  CITY_CD_SEARCH = model.CITY_CD_SEARCH,
                  DISTRICT_CD_SEARCH = model.DISTRICT_CD_SEARCH,
                  TOWN_NAME = '%' + model.TOWN_NAME + '%',
                  pageindex = lower,
                  pagesize = upper
              }).FirstOrDefault();

            return dataList;

        }

        public TownModel getInfoTown(int CityCd, int DistrictCd, int TownCd)
        {
            string sql = @"
                SELECT  A.*, B.CITY_NAME, C.DISTRICT_NAME
                FROM MstTown A
                LEFT JOIN MstCity B
                ON A.CITY_CD = B.CITY_CD
                LEFT JOIN MstDistrict C
                ON A.CITY_CD = C.CITY_CD AND A.DISTRICT_CD = C.DISTRICT_CD

                WHERE   A.CITY_CD = @CITY_CD AND A.DISTRICT_CD = @DISTRICT_CD AND  A.TOWN_CD = @TOWN_CD";

            return base.SingleOrDefault<TownModel>(sql.ToString(), new
            {
                CITY_CD = CityCd,
                DISTRICT_CD = DistrictCd,
                TOWN_CD = TownCd
            });
        }
        #endregion

        #region DELETE
        public int DeleteTown(long CITY_CD, long DISTRICT_CD, long TOWN_CD)
        {
            int result = 0;

            var currentuser = base.CmnEntityModel;
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
            UPDATE MstTown
                SET DEL_FLG = @DEL_FLG
            WHERE 
                CITY_CD = @CITY_CD AND DISTRICT_CD = @DISTRICT_CD AND TOWN_CD = @TOWN_CD");

            result = base.Execute(sql.ToString(), new
            {
                DEL_FLG = DeleteFlag.DELETE,
                CITY_CD = CITY_CD,
                DISTRICT_CD = DISTRICT_CD,
                TOWN_CD = TOWN_CD
            });

            return result;
        }

        #endregion

        #region CHECK EXIST TOWN exist group
        public bool CheckExistGroupTown(int city_Cd, int districtCd, int townCD, int forUser)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
                SELECT  COUNT(*)
                FROM    MstTown A ");

            if (forUser == GroupForUser.Receive)
            {
                sql.Append(@" LEFT JOIN MstGroupArea B 
                                ON A.GROUP_CD_RECEIVE = B.GROUP_CD 
                                WHERE B.FOR_USER = @FOR_USER 
                                AND A.GROUP_CD_RECEIVE > @GROUP_CD ");
            }else           
            {
                sql.Append(@" LEFT JOIN MstGroupArea B 
                                ON A.GROUP_CD_SENDER = B.GROUP_CD 
                                WHERE B.FOR_USER = @FOR_USER 
                                AND A.GROUP_CD_SENDER > @GROUP_CD ");
            }

            sql.Append(@" AND  A.CITY_CD = @CITY_CD AND A.DISTRICT_CD = @DISTRICT_CD AND A.TOWN_CD = @TOWN_CD 
                AND A.DEL_FLG = @DEL_FLG ");

            int count = base.SingleOrDefault<int>(sql.ToString(), new
            {
                CITY_CD = city_Cd,
                DISTRICT_CD = districtCd,
                TOWN_CD = townCD,
                FOR_USER = forUser,
                GROUP_CD = GroupCdArea.NON_SET,
                DEL_FLG = DeleteFlag.NON_DELETE,
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

        #endregion

    }
}