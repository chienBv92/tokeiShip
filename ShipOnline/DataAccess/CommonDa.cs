using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using System.Text;
using ShipOnline.Resources;
using ShipOnline.Models.Define;

namespace ShipOnline.DataAccess
{
    public class CommonDa:BaseDa
    {
        public IEnumerable<MstCity> GetCityList()
        {
            // Declare database connection      
            StringBuilder sql = new StringBuilder();
            // SQL発行  
            sql.Append(@" SELECT * FROM MstCity
                        WHERE DEL_FLG = @DEL_FLG
                        ORDER BY CITY_CD");

            return base.Query<MstCity>(sql.ToString(),
                new
                {
                    DEL_FLG = DeleteFlag.NON_DELETE
                }).ToList();
        }

        public IEnumerable<MstDistrict> GetDistrictList()
        {
            // Declare database connection      
            StringBuilder sql = new StringBuilder();
            // SQL発行  
            sql.Append(@" SELECT * FROM MstDistrict
                        WHERE DEL_FLG = @DEL_FLG
                        ORDER BY DISTRICT_CD");

            return base.Query<MstDistrict>(sql.ToString(),
                new
                {
                    DEL_FLG = DeleteFlag.NON_DELETE
                }).ToList();
        }

        public IEnumerable<MstTown> GetTownList()
        {
            // Declare database connection      
            StringBuilder sql = new StringBuilder();
            // SQL発行  
            sql.Append(@" SELECT * FROM MstTown
                        WHERE DEL_FLG = @DEL_FLG
                        ORDER BY CITY_CD, DISTRICT_CD, TOWN_CD");

            return base.Query<MstTown>(sql.ToString(),
                new
                {
                    DEL_FLG = DeleteFlag.NON_DELETE
                }).ToList();
        }

        // Get District by CityCd
        public IEnumerable<MstDistrict> GetDistrictByCityCd(int cityCd)
        {
            // Declare database connection      
            StringBuilder sql = new StringBuilder();
            // SQL発行  
            sql.Append(" SELECT * FROM MstDistrict");

            sql.Append(" WHERE DEL_FLG = @DEL_FLG");
            sql.Append(" AND CITY_CD = @CITY_CD");
            sql.Append(" ORDER BY DISTRICT_CD");

            var pics = base.Query<MstDistrict>(sql.ToString(),
                new
                {
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    CITY_CD = cityCd,
                }).ToList();

            // Create variable ouput   
            return pics;
        }

        // Get Town by DistrictCd
        public IEnumerable<MstTown> GetTownByDistrictCd(int cityCd, int districtCd)
        {
            // Declare database connection      
            StringBuilder sql = new StringBuilder();
            // SQL発行  
            sql.Append(" SELECT * FROM MstTown");

            sql.Append(" WHERE DEL_FLG = @DEL_FLG");
            sql.Append(" AND CITY_CD = @CITY_CD AND DISTRICT_CD = @DISTRICT_CD");
            sql.Append(" ORDER BY TOWN_CD");

            var pics = base.Query<MstTown>(sql.ToString(),
                new
                {
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    CITY_CD = cityCd,
                    DISTRICT_CD = districtCd
                }).ToList();

            // Create variable ouput   
            return pics;
        }

        public DistrictModel getInfoDistrict(int CityCd, int DistrictCd)
        {
            string sql = @"
                SELECT  A.*
                FROM    [MstDistrict] A
                WHERE   A.CITY_CD = @CITY_CD AND A.DISTRICT_CD = @DISTRICT_CD";

            return base.SingleOrDefault<DistrictModel>(sql.ToString(), new
            {
                CITY_CD = CityCd,
                DISTRICT_CD = DistrictCd
            });
        }
        /// <summary>
        /// Select List group Area by forUser
        /// </summary>
        /// <param name="forUser"></param>
        /// <returns></returns>
        public IEnumerable<MstGroupArea> GetListGroupArea(int forUser)
        {
            // Declare database connection      
            StringBuilder sql = new StringBuilder();
            // SQL発行  
            sql.Append(" SELECT * FROM [MstGroupArea]");

            sql.Append(" WHERE DEL_FLG = @DEL_FLG AND FOR_USER = @FOR_USER");

            sql.Append(" ORDER BY GROUP_CD");

            var pics = base.Query<MstGroupArea>(sql.ToString(),
                new
                {
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    FOR_USER = forUser
                }).ToList();

            // Create variable ouput   
            return pics;
        }

        public int getMaxOrder(long USER_ID)
        {
            var sqlexitsUser = new StringBuilder();
            sqlexitsUser.Append(@" 
                SELECT count(*)
                    FROM TblOrder 
                    WHERE USER_ID = @USER_ID
            ");
            var result = base.SingleOrDefault<int>(sqlexitsUser.ToString(), new { USER_ID = USER_ID });
            return result;
        }

        public long getShipID(string SHIP_CODE)
        {
            var sql = new StringBuilder();
            long result = 0;
            sql.Append(@" 
                SELECT ORDER_ID
                    FROM TblOrder 
                    WHERE SHIP_CODE = @SHIP_CODE
            ");
            var entity = base.Query<TblOrder>(sql.ToString(), new { SHIP_CODE = SHIP_CODE }).SingleOrDefault();

            if (entity != null)
            {
                result =  entity.ORDER_ID;
            }
            return result;
        }
        /// <summary>
        /// Get infor User from User ID
        /// </summary>
        /// <param name="SHIP_CODE"></param>
        /// <returns></returns>
        public UserAccountModel getInforUser(long UserID)
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

                WHERE   A.USER_ID = @USER_ID";

            return base.SingleOrDefault<UserAccountModel>(sql.ToString(), new
            {
                USER_ID = UserID
            });
        }
    }
}