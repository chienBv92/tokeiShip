using ShipOnline.Models.Define;
using ShipOnline.Models.Entity;
using ShipOnline.UtilityService;
using ShipOnline.Models.System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;


using ShipOnline.Models;
using Dapper;
namespace ShipOnline.DataAccess
{
    
    public class BaseDa
    {
        private CmnEntityModel cmnEntityModel = null;
        //public static readonly string ConnectionString = Environment.GetEnvironmentVariable("EmployeeDB");
        //public static readonly string ConnectionString = "Data Source= .\\SQLEXPRESS;Initial Catalog=otok5434_tokeishipDB;Integrated Security=True;MultipleActiveResultSets=True";
        public static readonly string ConnectionString = "Data Source= 112.78.2.50,1433;Initial Catalog=otok5434_tokeishipDB;user id=otok5434_shipUser;password=j8#Xm2b9;MultipleActiveResultSets=True;Integrated Security=False; Connection Timeout=60";
        //public static readonly string ConnectionString = "Data Source= 112.78.2.50,1433;Initial Catalog=otok5434_shipOnline;user id=otok5434_huycoi;password=Df2hx7&3;MultipleActiveResultSets=True;Integrated Security=False; Connection Timeout=20";

        private IDbConnection conn;
        private IDbCommand cmd;

        // Save session Info
        public CmnEntityModel CmnEntityModel
        {
            get
            {
                if (cmnEntityModel == null)
                {
                    if (HttpContext.Current.Session["CmnEntityModel"] == null)
                    {
                        HttpContext.Current.Session["CmnEntityModel"] = new CmnEntityModel();
                    }
                    cmnEntityModel = (CmnEntityModel)HttpContext.Current.Session["CmnEntityModel"];
                }
                return cmnEntityModel;
            }
        }
        /// <summary>
        /// Init SQL
        /// </summary>
        private void InitConnection()
        {
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            this.conn = factory.CreateConnection();
            this.conn.ConnectionString = ConnectionString;
            this.conn.Open();
        } 

        private IDataParameter CreatParam(string name, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }

        public long DbAddGetIdentity(string sql, BaseEntity entity)
        {
            return (long)(decimal)this.ExecuteNonQuery(true,sql, entity,isGetIdentity:true);
        }

        public int DbAdd(string sql, BaseEntity entity)
        {
            return (int)this.ExecuteNonQuery(true, sql, entity);
        }

        public int DbUpdate(string sql, object entity, object condition)
        {
            return (int)this.ExecuteNonQuery(false, sql, entity, condition);
        }
        /// <summary>
        /// Execute Sql 
        /// </summary>
        /// <param name="isInsert"></param>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <param name="isGetIdentity"></param>
        /// <returns></returns>
        private object ExecuteNonQuery(bool isInsert, string sql, object entity, object condition = null, bool isGetIdentity = false)
        {
            object result;
            DateTime startTime = Utility.GetCurrentDateTime();
            this.InitConnection();
            this.cmd = conn.CreateCommand();
            this.cmd.CommandText = sql;

            // Add sql param from entity
            foreach (var prop in entity.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var key = prop.Name;
                bool blUseParam = true;
                var value = prop.GetValue(entity, null);
                if (key == "INS_DATE" && isInsert)
                    value = startTime;
                if (key == "UPD_DATE")
                    value = startTime;               
                if (key == "INS_DATE" && !isInsert)
                    blUseParam = false;
                if (key == "INS_USER_ID" && !isInsert)
                    blUseParam = false;
                
                if (blUseParam)
                    this.cmd.Parameters.Add(CreatParam("@" + key, value));
            }
            // Add sql condition params
            if (condition != null)
            {
                foreach (var prop in condition.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    this.cmd.Parameters.Add(CreatParam("@ORG_" + prop.Name, prop.GetValue(condition, null)));
                }
            }

            // Execute command
            try
            {
                if (isInsert && isGetIdentity)
                {
                    this.cmd.CommandText += "; SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY] ";
                    result = this.cmd.ExecuteScalar() ?? 0;
                }
                else
                {
                    result = this.cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                this.conn.Dispose();
                this.conn = null;
                this.cmd.Dispose();
                this.cmd = null;
                
            }
            return result;

        }
        /// <summary>
        /// Executes a query, returning the data typed as per T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>( string sql, object param = null){
            dynamic result = null;
            this.InitConnection();

            try{
                DateTime startDate = Utility.GetCurrentDateTime();
                result = conn.Query<T>(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                this.conn.Dispose();
                this.conn = null;
            }
            return result;
        }
        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, object param = null)
        {
            dynamic result = null;
            this.InitConnection();

            try
            {
                DateTime startTime = Utility.GetCurrentDateTime();
                result = this.conn.ExecuteScalar<T>(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                this.conn.Dispose();
                this.conn = null;
            }
            return result;
        }

        public int Execute(string sql, object param = null)
        {
            int result = 0;
            this.InitConnection();

            try
            {
                DateTime startTime = Utility.GetCurrentDateTime();
                IDbTransaction transaction = conn.BeginTransaction();
                result = conn.Execute(sql, param, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                this.conn.Dispose();
                this.conn.Close();
                this.conn = null;
            }
            return result;
        }

        protected T SingleOrDefault<T>(string sql, object param = null)
        {
            dynamic result = null;
            this.InitConnection();
            DateTime startTime = Utility.GetCurrentDateTime();
            DateTime endTime = Utility.GetCurrentDateTime();

            try
            {
                //DateTime startTime = Utility.GetCurrentDateTime();
                //log.StartLog(startTime, companyCd, userAccount, ipAddress, userAgent, browserType, browserVersion);
                startTime = Utility.GetCurrentDateTime();
                int commandTimeout = 30;
                result = conn.Query<T>(sql, param, null, true, commandTimeout).SingleOrDefault();
                endTime = Utility.GetCurrentDateTime();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                this.conn.Dispose();
                this.conn = null;
            }
            return result;
        }


    }
}