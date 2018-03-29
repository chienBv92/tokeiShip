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
using System.Globalization;

namespace ShipOnline.DataAccess
{
    public class OrderShipDa:BaseDa
    {
        #region REGIST/ UPDATE

        public long InsertOrder(TblOrder order)
        {
            long result = 0;

            //Check create new customer
            StringBuilder sqlinsert = new StringBuilder();
            order.DEL_FLG = DeleteFlag.NON_DELETE;

            sqlinsert.Append(@" 
                    INSERT INTO [TblOrder] 
                        ([USER_ID]
                        ,[SHIP_CODE]
                        ,[PRODUCT_NAME]
                        ,[PRODUCT_TYPE]
                        ,[CREATE_DATE]
                        ,[PRODUCT_WEIGHT]
                        ,[PRODUCT_HEIGHT]
                        ,[PRODUCT_LENGTH]
                        ,[PRODUCT_WIDTH]
                        ,[PRODUCT_POSITION]
                        ,[ACCEPTANCE_ID]
                        ,[ACCEPTANCE_DATE]
                        ,[RECEIVED_NAME]
                        ,[RECEIVED_PHONE]
                        ,[RECEIVED_CITY]
                        ,[RECEIVED_DISTRICT]
                        ,[RECEIVED_TOWN]
                        ,[RECEIVED_ADDRESS]
                        ,[RECEIVE_HOUR_FROM]
                        ,[RECEIVE_HOUR_TO]
                        ,[RECEIVE_TIME_DATE]
                        ,[SHIP_TYPE]
                        ,[PRICE_PRODUCT]
                        ,[PRICE_SHIP]
                        ,[DISCOUNT]
                        ,[USER_PAYMENT]
                        ,[ORDER_STATUS]
                        ,[DELIVERY_DATE]
                        ,[RECEIVED_DATE]
                        ,[DEL_FLG]
                        ,[OTHER_REQUIREMENT]
                        ,[UPD_DATE])
                    VALUES
                        (@USER_ID,
                        @SHIP_CODE,
                        @PRODUCT_NAME,
                        @PRODUCT_TYPE,
                        @CREATE_DATE,
                        @PRODUCT_WEIGHT,
                        @PRODUCT_HEIGHT,
                        @PRODUCT_LENGTH,
                        @PRODUCT_WIDTH,
                        @PRODUCT_POSITION,
                        @ACCEPTANCE_ID,
                        @ACCEPTANCE_DATE,
                        @RECEIVED_NAME,
                        @RECEIVED_PHONE,
                        @RECEIVED_CITY,
                        @RECEIVED_DISTRICT,
                        @RECEIVED_TOWN,
                        @RECEIVED_ADDRESS,
                        @RECEIVE_HOUR_FROM,
                        @RECEIVE_HOUR_TO,
                        @RECEIVE_TIME_DATE,
                        @SHIP_TYPE,
                        @PRICE_PRODUCT,
                        @PRICE_SHIP,
                        @DISCOUNT,
                        @USER_PAYMENT,
                        @ORDER_STATUS,
                        @DELIVERY_DATE,
                        @RECEIVED_DATE,
                        @DEL_FLG,
                        @OTHER_REQUIREMENT,
                        @UPD_DATE)");

            result = base.DbAdd(sqlinsert.ToString(), order);
            return result;

        }

//        public long UpdateCity(MstCity city)
//        {
//            long result = 0;

//            //Check create new customer
//            StringBuilder sqlinsert = new StringBuilder();
//            city.DEL_FLG = DeleteFlag.NON_DELETE;

//            sqlinsert.Append(@" 
//                    UPDATE [dbo].[MstCity]
//                    SET [CITY_ZIP_CD] = @CITY_ZIP_CD
//                    ,[CITY_NAME] = @CITY_NAME
//                    ,[DSP_ORDER] = @DSP_ORDER
//                    ,[DEL_FLG] = @DEL_FLG
//                    ,[STATUS] = @STATUS
//                    WHERE [CITY_CD] = @CITY_CD");

//            result = base.DbUpdate(sqlinsert.ToString(), city, new { CITY_CD = city.CITY_CD });
//            return result;

//        }

//        public bool FindCity(int city_Cd, int cityCdOld)
//        {
//            string sql = @"
//                SELECT  COUNT(*)
//                FROM    [MstCity]
//                WHERE   CITY_CD = @CITY_CD";

//            int count = base.SingleOrDefault<int>(sql, new
//            {
//                CITY_CD = city_Cd,
//            });
//            if (count > 0 && cityCdOld == 0)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
        #endregion

        #region VIEW ORDER SHIP
        public OrderShipModel getInforOrder(long OrderId)
        {
            string sql = @"
                SELECT  A.*, B.CITY_NAME as RECEIVE_CITY_NAME, C.DISTRICT_NAME as RECEIVE_DISTRICT_NAME, D.TOWN_NAME as RECEIVE_TOWN_NAME,
                E.USER_NAME as ACCEPTANCE_NAME
                FROM TblOrder A
                LEFT JOIN MstCity B
                ON A.RECEIVED_CITY = B.CITY_CD
                LEFT JOIN MstDistrict C
                ON A.RECEIVED_CITY = C.CITY_CD AND A.RECEIVED_DISTRICT = C.DISTRICT_CD
                LEFT JOIN MstTown D
                ON A.RECEIVED_CITY = D.CITY_CD AND A.RECEIVED_DISTRICT = D.DISTRICT_CD AND A.RECEIVED_TOWN = D.TOWN_CD
                LEFT JOIN TblUserAccount E
                ON A.ACCEPTANCE_ID = E.USER_ID

                WHERE   A.ORDER_ID = @ORDER_ID
                AND A.DEL_FLG = @DEL_FLG";

            return base.SingleOrDefault<OrderShipModel>(sql.ToString(), new
            {
                ORDER_ID =  OrderId,
                DEL_FLG = DeleteFlag.NON_DELETE
            });
        }
        #endregion

        #region LIST
        /// <summary>
        /// Select list no have status
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<OrderShipModel> SearchOrderListTotalNoneStatus(DataTableModel dt, OrderShipCondition model)
        {
            StringBuilder sql = new StringBuilder();
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            sql.Append(@"
                SELECT A. *
                FROM TblOrder A
                WHERE
                    A.USER_ID = @USER_ID
                  AND A.DEL_FLG = @DEL_FLG");
            
            if (!string.IsNullOrEmpty(model.TEXT_SEARCH))
            {
                sql.Append(" AND (A.SHIP_CODE LIKE @TEXT_SEARCH ESCAPE '|' ) OR ( A.RECEIVED_NAME LIKE @TEXT_SEARCH ESCAPE '|') OR ( A.RECEIVED_PHONE LIKE @TEXT_SEARCH ESCAPE '|') ");
            }
            if (!string.IsNullOrEmpty(model.FROM_DATE))
            {
                fromDate = DateTime.ParseExact(model.FROM_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                sql.Append(" AND CAST(A.CREATE_DATE AS DATE) >= @FROM_DATE ");
            }
            if (!string.IsNullOrEmpty(model.TO_DATE))
            {
                toDate = DateTime.ParseExact(model.TO_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                sql.Append(" AND CAST(A.CREATE_DATE AS DATE) <= @TO_DATE ");
            }
            sql.Append(" ORDER BY CREATE_DATE DESC, SHIP_CODE DESC, RECEIVED_NAME ASC");

            var dataList = base.Query<OrderShipModel>(sql.ToString(),
                new
                {
                    USER_ID = CmnEntityModel.USER_ID,
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    TEXT_SEARCH = '%' + UtilityServices.UtilityServices.replaceWildcardCharacters(model.TEXT_SEARCH) + '%',
                    TO_DATE = toDate,
                    FROM_DATE = fromDate
                }).ToList();

            return dataList;

        }
        /// <summary>
        /// Select list have status all
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<OrderShipModel> SearchOrderShipListTotal(DataTableModel dt, OrderShipCondition model)
        {
            StringBuilder sql = new StringBuilder();
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            sql.Append(@"
                SELECT A. *
                FROM TblOrder A
                WHERE
                    A.USER_ID = @USER_ID
                  AND A.DEL_FLG = @DEL_FLG");
            if (!string.IsNullOrEmpty(model.ORDER_STATUS_LIST))
            {
                sql.Append(" AND A.ORDER_STATUS IN ('" + model.ORDER_STATUS_LIST + "')");

            }
            if (!string.IsNullOrEmpty(model.TEXT_SEARCH))
            {
                sql.Append(" AND (A.SHIP_CODE LIKE @TEXT_SEARCH ESCAPE '|' ) OR ( A.RECEIVED_NAME LIKE @TEXT_SEARCH ESCAPE '|') OR ( A.RECEIVED_PHONE LIKE @TEXT_SEARCH ESCAPE '|') ");
            }
            if (!string.IsNullOrEmpty(model.FROM_DATE))
            {
                fromDate = DateTime.ParseExact(model.FROM_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                sql.Append(" AND CAST(A.CREATE_DATE AS DATE) >= @FROM_DATE ");
            }
            if (!string.IsNullOrEmpty(model.TO_DATE))
            {
                toDate = DateTime.ParseExact(model.TO_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                sql.Append(" AND CAST(A.CREATE_DATE AS DATE) <= @TO_DATE ");
            }
            sql.Append(" ORDER BY CREATE_DATE DESC, SHIP_CODE DESC, RECEIVED_NAME ASC");

            var dataList = base.Query<OrderShipModel>(sql.ToString(),
                new
                {
                    USER_ID = CmnEntityModel.USER_ID,
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    TEXT_SEARCH = '%' + UtilityServices.UtilityServices.replaceWildcardCharacters(model.TEXT_SEARCH) + '%',
                    TO_DATE = toDate,
                    FROM_DATE = fromDate
                }).ToList();

            return dataList;

        }
        /// <summary>
        /// List display paging
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="model"></param>
        /// <param name="total_row"></param>
        /// <returns></returns>
        public IEnumerable<OrderShipModel> SearchOrderShipList(DataTableModel dt, OrderShipCondition model, out int total_row)
        {
            StringBuilder sql = new StringBuilder();
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            sql.Append(@"
                SELECT A. *
                FROM TblOrder A
                WHERE
                    A.USER_ID = @USER_ID
                  AND A.DEL_FLG = @DEL_FLG");
            if (!string.IsNullOrEmpty(model.ORDER_STATUS_LIST))
            {
                sql.Append(" AND A.ORDER_STATUS IN ('" + model.ORDER_STATUS_LIST + "')");

            }
            if (!string.IsNullOrEmpty(model.TEXT_SEARCH))
            {
                sql.Append(" AND (A.SHIP_CODE LIKE @TEXT_SEARCH ESCAPE '|' ) OR ( A.RECEIVED_NAME LIKE @TEXT_SEARCH ESCAPE '|') OR ( A.RECEIVED_PHONE LIKE @TEXT_SEARCH ESCAPE '|') ");
            }
            if (!string.IsNullOrEmpty(model.FROM_DATE))
            {
                fromDate = DateTime.ParseExact(model.FROM_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                sql.Append(" AND CAST(A.CREATE_DATE AS DATE) >= @FROM_DATE ");
            }
            if (!string.IsNullOrEmpty(model.TO_DATE))
            {
                toDate = DateTime.ParseExact(model.TO_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                sql.Append(" AND CAST(A.CREATE_DATE AS DATE) <= @TO_DATE ");
            }
          
            sql.Append(" ORDER BY CREATE_DATE DESC, SHIP_CODE DESC, RECEIVED_NAME ASC");

            int lower = dt.iDisplayStart + 1;
            int upper = dt.iDisplayStart + dt.iDisplayLength;

            PagingHelper.SQLParts parts;
            PagingHelper.SplitSQL(sql.ToString(), out parts);

            string sqlpage = PagingHelper.BuildPageQuery(lower, dt.iDisplayLength, parts);
            string sqlcount = parts.sqlCount;

            var dataList = base.Query<OrderShipModel>(sqlpage,
                new
                {
                    USER_ID = CmnEntityModel.USER_ID,
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    TEXT_SEARCH = '%' + UtilityServices.UtilityServices.replaceWildcardCharacters(model.TEXT_SEARCH) + '%',
                    TO_DATE = toDate,
                    FROM_DATE = fromDate,
                    pageindex = lower,
                    pagesize = upper
                }).ToList();

            total_row = base.Query<int>(sqlcount,
              new
              {
                  USER_ID = CmnEntityModel.USER_ID,
                  DEL_FLG = DeleteFlag.NON_DELETE,
                  TEXT_SEARCH = '%' + UtilityServices.UtilityServices.replaceWildcardCharacters(model.TEXT_SEARCH) + '%',
                  TO_DATE = toDate,
                  FROM_DATE = fromDate,
                  pageindex = lower,
                  pagesize = upper
              }).FirstOrDefault();

            return dataList;

        }
        /// <summary>
        /// Search follow state 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<OrderShipModel> SearchOrderShipByStatus(int Status)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"
                SELECT A. *
                FROM TblOrder A
                WHERE
                    A.USER_ID = @USER_ID 
                   AND ORDER_STATUS = @STATUS
                   AND A.DEL_FLG = @DEL_FLG");
            
            sql.Append(" ORDER BY CREATE_DATE DESC, SHIP_CODE DESC, RECEIVED_NAME ASC");

            var dataList = base.Query<OrderShipModel>(sql.ToString(),
                new
                {
                    USER_ID = CmnEntityModel.USER_ID,
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    STATUS = Status
                }).ToList();

            return dataList;

        }

        /// <summary>
        /// Select List this month
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IEnumerable<OrderShipModel> SearchOrderByMonth(string thisMonth)
        {
            StringBuilder sql = new StringBuilder();
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();
            string thisMonthFrom = "01/" + thisMonth ;
            string thisMonthTo = "31/" + thisMonth;

            sql.Append(@"
                SELECT A. *
                FROM TblOrder A
                WHERE
                    A.USER_ID = @USER_ID
                  AND A.DEL_FLG = @DEL_FLG");

            if (!string.IsNullOrEmpty(thisMonthFrom))
            {
                fromDate = DateTime.ParseExact(thisMonthFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                sql.Append(" AND CAST(A.CREATE_DATE AS DATE) >= @FROM_DATE ");
            }
            if (!string.IsNullOrEmpty(thisMonthTo))
            {
                toDate = DateTime.ParseExact(thisMonthTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                sql.Append(" AND CAST(A.CREATE_DATE AS DATE) <= @TO_DATE ");
            }
            sql.Append(" ORDER BY CREATE_DATE DESC, SHIP_CODE DESC, RECEIVED_NAME ASC");

            var dataList = base.Query<OrderShipModel>(sql.ToString(),
                new
                {
                    USER_ID = CmnEntityModel.USER_ID,
                    DEL_FLG = DeleteFlag.NON_DELETE,
                    TO_DATE = toDate,
                    FROM_DATE = fromDate
                }).ToList();

            return dataList;

        }
        #endregion

        #region EXPORT CSV
        public IList<OrderShipModel> getDataForCSV(OrderShipCondition condition)
        {
            StringBuilder sql = new StringBuilder();
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            sql.Append(@"
                SELECT  A.*, B.CITY_NAME as RECEIVE_CITY_NAME, C.DISTRICT_NAME as RECEIVE_DISTRICT_NAME, D.TOWN_NAME as RECEIVE_TOWN_NAME,
                E.USER_NAME as ACCEPTANCE_NAME
                FROM TblOrder A
                LEFT JOIN MstCity B
                ON A.RECEIVED_CITY = B.CITY_CD
                LEFT JOIN MstDistrict C
                ON A.RECEIVED_CITY = C.CITY_CD AND A.RECEIVED_DISTRICT = C.DISTRICT_CD
                LEFT JOIN MstTown D
                ON A.RECEIVED_CITY = D.CITY_CD AND A.RECEIVED_DISTRICT = D.DISTRICT_CD AND A.RECEIVED_TOWN = D.TOWN_CD
                LEFT JOIN TblUserAccount E
                ON A.ACCEPTANCE_ID = E.USER_ID

                WHERE A.USER_ID = @USER_ID
                AND A.DEL_FLG = @DEL_FLG");

            if (!string.IsNullOrEmpty(condition.ORDER_STATUS_LIST))
            {
                sql.Append(" AND A.ORDER_STATUS IN ('" + condition.ORDER_STATUS_LIST + "')");

            }
            if (!string.IsNullOrEmpty(condition.TEXT_SEARCH))
            {
                sql.Append(" AND (A.SHIP_CODE LIKE @TEXT_SEARCH ESCAPE '|' ) OR ( A.RECEIVED_NAME LIKE @TEXT_SEARCH ESCAPE '|') OR ( A.RECEIVED_PHONE LIKE @TEXT_SEARCH ESCAPE '|') ");
            }
            if (!string.IsNullOrEmpty(condition.FROM_DATE))
            {
                fromDate = DateTime.ParseExact(condition.FROM_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                sql.Append(" AND A.CREATE_DATE >= @FROM_DATE ");
            }
            if (!string.IsNullOrEmpty(condition.TO_DATE))
            {
                toDate = DateTime.ParseExact(condition.TO_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                sql.Append(" AND A.CREATE_DATE <= @TO_DATE ");
            }

            sql.Append(" ORDER BY CREATE_DATE DESC, SHIP_CODE DESC, RECEIVED_NAME ASC");

            return base.Query<OrderShipModel>(sql.ToString(), new
            {
                USER_ID = CmnEntityModel.USER_ID,
                DEL_FLG = DeleteFlag.NON_DELETE,
                TEXT_SEARCH = '%' + UtilityServices.UtilityServices.replaceWildcardCharacters(condition.TEXT_SEARCH) + '%',
                TO_DATE = toDate,
                FROM_DATE = fromDate
            }).ToList();
        }
        #endregion
    }
}