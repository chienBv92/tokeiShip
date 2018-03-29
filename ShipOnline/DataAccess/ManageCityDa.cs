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

namespace ShipOnline.DataAccess
{
    public class ManageCityDa:BaseDa
    {
        #region REGIST/ UPDATE
        public CityModel getInfoCity(int CityCd)
        {
            string sql = @"
                SELECT  *
                FROM    [MstCity]
                WHERE   CITY_CD = @CITY_CD";

            return base.SingleOrDefault<CityModel>(sql.ToString(), new
            {
                CITY_CD = CityCd
            });
        }

        public long RegisterCity(MstCity city)
        {
            long result = 0;

            //Check create new customer
                StringBuilder sqlinsert = new StringBuilder();
                city.DEL_FLG = DeleteFlag.NON_DELETE;

                sqlinsert.Append(@" 
                    INSERT INTO [MstCity] 
                        ([CITY_CD]
                        ,[CITY_ZIP_CD]
                        ,[CITY_NAME]
                        ,[DSP_ORDER]
                        ,[DEL_FLG]
                        ,[STATUS])
                    VALUES
                        (@CITY_CD,
                        @CITY_ZIP_CD,
                        @CITY_NAME,
                        @DSP_ORDER,
                        @DEL_FLG,
                        @STATUS)");

                result = base.DbAdd(sqlinsert.ToString(), city);                
            return result;

        }

        public long UpdateCity(MstCity city)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();
            city.DEL_FLG = DeleteFlag.NON_DELETE;

            sqlinsert.Append(@" 
                    UPDATE [dbo].[MstCity]
                    SET [CITY_ZIP_CD] = @CITY_ZIP_CD
                    ,[CITY_NAME] = @CITY_NAME
                    ,[DSP_ORDER] = @DSP_ORDER
                    ,[DEL_FLG] = @DEL_FLG
                    ,[STATUS] = @STATUS
                    WHERE [CITY_CD] = @CITY_CD");

            result = base.DbUpdate(sqlinsert.ToString(), city, new { CITY_CD  =  city.CITY_CD});
            return result;

        }

        public bool FindCity(int city_Cd, int cityCdOld)
        {
            string sql = @"
                SELECT  COUNT(*)
                FROM    [MstCity]
                WHERE   CITY_CD = @CITY_CD";

            int count = base.SingleOrDefault<int>(sql, new {
                CITY_CD = city_Cd,
            });
            if(count > 0 && cityCdOld == 0){
                return true;
            }else{
                return false;
            }
        }
        #endregion

        #region LIST
        public IEnumerable<MstCity> SearchCityList(DataTableModel dt, CityModel model, out int total_row)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
                SELECT *
                FROM MstCity
                WHERE
                    DEL_FLG = @DEL_FLG ");

            if (!string.IsNullOrEmpty(model.CITY_NAME))
            {
                sql.Append(" AND    (CITY_NAME LIKE @CITY_NAME)");
            }

            int lower = dt.iDisplayStart + 1;
            int upper = dt.iDisplayStart + dt.iDisplayLength;

            PagingHelper.SQLParts parts;
            PagingHelper.SplitSQL(sql.ToString(), out parts);

            string sqlpage = PagingHelper.BuildPageQuery(lower, dt.iDisplayLength, parts);
            string sqlcount = parts.sqlCount;

            var dataList = base.Query<MstCity>(sqlpage,
                new
                {
                    DEL_FLG = model.DEL_FLG,
                    CITY_NAME = '%' + model.CITY_NAME + '%',
                    pageindex = lower,
                    pagesize = upper
                }).ToList();

            total_row = base.Query<int>(sqlcount,
              new
              {
                  DEL_FLG = model.DEL_FLG,
                  CITY_NAME = '%' + model.CITY_NAME + '%',
                  pageindex = lower,
                  pagesize = upper
              }).FirstOrDefault();

            return dataList;

        }
        #endregion

        #region DELETE
        public int DeleteCity(long CITY_CD)
        {
            int result = 0;

            var currentuser = base.CmnEntityModel;
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
            UPDATE MstCity
                SET DEL_FLG = @DEL_FLG
            WHERE 
                CITY_CD = @CITY_CD ");

            result = base.Execute(sql.ToString(), new
            {
                DEL_FLG = DeleteFlag.DELETE,
                CITY_CD = CITY_CD
            });

            return result;
        }

        #endregion

    }
}