// Version		: 001
// Designer		: ChienBV
// Date			: 2018/01/23
// Comment		: Create new

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SystemSetup.UtilityServices.PagingHelper
{
    public class PagingHelper
    {
        public static Regex rxColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        public static Regex rxOrderBy = new Regex(@"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*", RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        public static Regex rxDistinct = new Regex(@"\ADISTINCT\s", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public struct SQLParts
        {
            public string sql;
            public string sqlCount;
            public string sqlSelectRemoved;
            public string sqlOrderBy;
        }

        public static bool SplitSQL(string sql, out SQLParts parts)
        {
            parts.sql = sql;
            parts.sqlSelectRemoved = null;
            parts.sqlCount = null;
            parts.sqlOrderBy = null;

            // Extract the columns from "SELECT <whatever> FROM"
            var m = rxColumns.Match(sql);
            if (!m.Success)
                return false;

            // Save column list and replace with COUNT(*)
            Group g = m.Groups[1];
            parts.sqlSelectRemoved = sql.Substring(g.Index);

            if (rxDistinct.IsMatch(parts.sqlSelectRemoved))
                parts.sqlCount = sql.Substring(0, g.Index) + "COUNT(" + m.Groups[1].ToString().Trim() + ") " + sql.Substring(g.Index + g.Length);
            else
                parts.sqlCount = sql.Substring(0, g.Index) + "COUNT(*) " + sql.Substring(g.Index + g.Length);


            // Look for the last "ORDER BY <whatever>" clause not part of a ROW_NUMBER expression
            m = rxOrderBy.Match(parts.sqlCount);
            if (!m.Success)
            {
                parts.sqlOrderBy = null;
            }
            else
            {
                g = m.Groups[0];
                parts.sqlOrderBy = g.ToString();
                parts.sqlCount = parts.sqlCount.Substring(0, g.Index) + parts.sqlCount.Substring(g.Index + g.Length);
            }

            return true;
        }

        public static string BuildPageQuery(long skip, long take, PagingHelper.SQLParts parts)
        {
            parts.sqlSelectRemoved = rxOrderBy.Replace(parts.sqlSelectRemoved, "", 1);
            if (rxDistinct.IsMatch(parts.sqlSelectRemoved))
            {
                parts.sqlSelectRemoved = "peta_inner.* FROM (SELECT " + parts.sqlSelectRemoved + ") peta_inner";
            }
            var sqlPage = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) rownum, {1}) dapper_paged WHERE dapper_paged.rownum between @pageindex and @pagesize",
                                    parts.sqlOrderBy == null ? "ORDER BY (SELECT NULL)" : parts.sqlOrderBy, parts.sqlSelectRemoved);
           
            return sqlPage;
        }

        public static string GetPagingSelectStatement(string sql, string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "1";
            }
            return string.Format("SELECT * FROM ({0})dapper_paged ORDER BY {1} OFFSET @pageindex - 1 ROWS FETCH NEXT @pagesize ROWS ONLY", sql, sortOrder);
        }

        public static string GetTotalCountSelectStatement(string sql)
        {
            return string.Format("SELECT COUNT(0) FROM ({0})dapper_paged", sql);
        }
    }
}
