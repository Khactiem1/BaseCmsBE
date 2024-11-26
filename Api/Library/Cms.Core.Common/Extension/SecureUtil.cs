using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cms.Core.Common.Extension
{
    public static class SecureUtil
    {
        public const string mscSQLInjectionMessage = "SQL Injection detected. Please re-check parameter clause";
        private static readonly Regex regSystemThreats = new Regex("\\s?;\\s?|\\s?drop \\s|\\s?grant \\s|^'|\\s?--|\\s?union \\s|\\s?delete \\s|\\s?update \\s|\\s?truncate \\s|\\s?sysobjects\\s?|\\s?xp_.*?|\\s?syslogins\\s?|\\s?sysremote\\s?|\\s?sysusers\\s?|\\s?sysxlogins\\s?|\\s?sysdatabases\\s?|\\s?aspnet_.*?|\\s?exec \\s?|\\s?execute \\s?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool DetectSqlInjection(string inputSql)
        {
            return !inputSql.IsNullOrEmptyExt() && regSystemThreats.IsMatch(inputSql);
        }

        public static void HandleSqlInjection()
        {
        }

        public static string SafeSqlLiteral(string inputSql)
        {
            if (inputSql.IsNullOrEmptyExt()) { return inputSql; }
            return inputSql.Replace("'", "''");
        }

        public static string SafeSortLiteral(string inputSort)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!inputSort.IsNullOrEmptyExt())
            {
                string[] array = inputSort.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (array != null && array.Length != 0)
                {
                    string[] array2 = array;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        string text = array2.ValueExt(i);
                        string text2 = "";
                        int num = text.IndexOf(" asc", StringComparison.CurrentCultureIgnoreCase);
                        if (num > 0)
                        {
                            text = text.Remove(num, 4);
                        }
                        int num2 = text.IndexOf(" desc", StringComparison.CurrentCultureIgnoreCase);
                        if (num2 > 0)
                        {
                            text = text.Remove(num2, 5);
                            text2 = " DESC";
                        }
                        text = text.Trim();
                        if (text.StartsWith("[") && text.EndsWith("]"))
                        {
                            text += text2;
                        }
                        else if (text.Length > 0)
                        {
                            text = "[" + text + "]" + text2;
                        }
                        if (!text.IsNullOrEmptyExt() && stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(",");
                        }
                        stringBuilder.Append(text);
                    }
                }
            }
            return stringBuilder.ToString();
        }

        public static string SafeColumnLiteral(string inputColumns)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!inputColumns.IsNullOrEmptyExt())
            {
                string[] array = inputColumns.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (array != null && array.Length != 0)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        string text = array.ValueExt(i).Trim();
                        if (text.CompareTo("*") != 0 && (!text.StartsWith("[") || !text.EndsWith("]")))
                        {
                            text = "[" + text + "]";
                        }
                        if (!text.IsNullOrEmptyExt() && stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(",");
                        }
                        stringBuilder.Append(text);
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
