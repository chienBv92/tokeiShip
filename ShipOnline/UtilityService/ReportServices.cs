//------------------------------------------------------------------------
// Version		: 001
// Designer		: ChienBV
// Date			: 2018/01/23
// Comment		: Create new
//------------------------------------------------------------------------

namespace iEnterAsia.iseiQ.ReportServices
{
    using ShipOnline.UtilityServices;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Linq;

    /// <summary>
    /// Report services
    /// </summary>
    public class ReportServices
    {

        #region Export file Csv
        /// <summary>
        /// Using Data form datatable to export to csv file
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        public static void ExportToCsvData(Controller controller, DataTable dt, string fileName = "data.csv", string[] columns = null)
        {
            string delimiter = ",";

            controller.Response.Clear();
            controller.Response.ContentType = "text/csv;";
            controller.Response.ContentEncoding = System.Text.Encoding.UTF8;
            controller.Response.AppendHeader("Content-type", "application/x-download");
            DownloadUtil.AddHeaderContentDisposition(HttpContext.Current.Request, controller.Response, fileName);

            string value = "";
            var builder = new StringBuilder();

            //write the csv column headers
            if(columns == null)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    value = dt.Columns[i].ColumnName;
                    // Implement special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes
                    if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                        builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                    else
                    {
                        builder.Append(value);
                    }

                    controller.Response.Write(builder.ToString());
                    controller.Response.Write((i < dt.Columns.Count - 1) ? delimiter : Environment.NewLine);
                    builder.Clear();
                }
            }
            else
            {
                for (int i = 0; i < columns.Length; i++)
                {

                    value = columns[i].ToString();
                    // Implement special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes
                    if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                        builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                    else
                    {
                        builder.Append(value);
                    }
                    controller.Response.Write(builder.ToString());
                    controller.Response.Write((i < columns.Length - 1) ? delimiter : Environment.NewLine);
                    builder.Clear();
                }
            }
            
            //write the data
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    controller.Response.Write(row[i].ToString());
                    controller.Response.Write((i < dt.Columns.Count - 1) ? delimiter : Environment.NewLine);
                }
            }

            try
            {
                controller.Response.End();
            }
            catch (HttpException)
            {
                // When canceled.
                // The remote host closed the connection. The error code is 0x800704CD
            }
        }

        public static object FormatDynamicDecimalCsv(object value)
        {
            if (value == null)
            {
                return null;
            }
            decimal tmpOut;
            return Decimal.TryParse(value.ToString(), out tmpOut) ? value : FormatStringCsv(value.ToString());
        }

        public static string FormatDynamicDateTimeCsv(object value)
        {
            if (value == null)
            {
                return null;
            }
            DateTime tmpDate;
            return DateTime.TryParse(value.ToString(), out tmpDate) ? tmpDate.ToString("yyyy/MM/dd") : FormatStringCsv(value.ToString()); 
        }

        public static string FormatStringCsv(string value, bool isReplaceBreakDownLine = false)
        {
            if(value == null)
            {
                return null;
            }
            if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
            {
                return string.Format("\"{0}\"", value.Replace("\"", "\"\""));
            }
            if (isReplaceBreakDownLine)
            {
                value = value.Replace("\r\n", "<改行>");
            }
            return "\"" + value + "\"";
        }

        public static string FormatDateCsv(object dateValue)
        {
            if (dateValue == null || dateValue == string.Empty)
            {
                return null;
            }
            return ((DateTime)dateValue).ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// Convert from IList<T> to DataTable
        /// </summary>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static DataTable ToDataTableT<T>(IList<T> items, string[] columns)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(columns[i]);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    var tmpValue = Props[i].GetValue(item, null);
                    decimal tmpDecimal = 0;
                    if (tmpValue == null)
                    {
                        values[i] = tmpValue;
                    } 
                    else if (Props[i].PropertyType == typeof(Nullable<System.DateTime>)
                        || Props[i].PropertyType == typeof(System.DateTime))
                    {
                        // Format datetime values
                        values[i] = FormatDateCsv(tmpValue);
                    }
                    else if (Props[i].PropertyType == typeof(string)
                        || Props[i].PropertyType == typeof(System.String)
                        || (Props[i].PropertyType == typeof(System.Object) && !Decimal.TryParse(tmpValue.ToString(), out tmpDecimal)))
                    {
                        values[i] = FormatStringCsv(tmpValue.ToString());
                    }
                    else
                    {
                        // orther values
                        values[i] = tmpValue;
                    }
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        #endregion
    }
}