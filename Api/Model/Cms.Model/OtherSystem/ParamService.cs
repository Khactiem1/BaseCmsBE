using Cms.Core.Common.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cms.Model
{
    public static class ParamService
    {
        public static string ParseSort(string sort)
        {
            if (sort.IsNullOrEmptyExt()) { return ""; }

            var data = sort.Split(',');
            var sb = new StringBuilder();
            foreach (var id in data)
            {
                if (string.IsNullOrWhiteSpace(id)) { continue; }

                var item = id.Trim();
                if (sb.Length > 0) { sb.Append(","); }

                var ix = item.LastIndexOf(" ");
                string field;
                var dir = "ASC";
                if (ix > 0)
                {
                    field = item.Substring(0, ix);
                    var temp = item.Substring(ix + 1);
                    if ("DESC".Equals(temp, StringComparison.OrdinalIgnoreCase)) { dir = "DESC"; }
                }
                else
                {
                    field = item;
                }

                field = field.Trim();
                if (field.IsNullOrEmptyExt()) { continue; }
                sb.Append($"{field} {dir}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// xử lý tham số column truyền từ client thành câu lệnh sql
        /// </summary>
        /// <param name="column">dữ liệu client</param>
        public static string ParseColumn(string column)
        {
            if (column.IsNullOrEmptyExt() || column == "*") { return "*"; }
            return $"{string.Join(",", column.Split(',').Distinct())}";
        }

        /// <summary>
        /// xử lý tham số column truyền từ client thành câu lệnh sql
        /// </summary>
        /// <param name="column">dữ liệu client</param>
        public static string ParseCustomColumn(string column)
        {
            if (column.IsNullOrEmptyExt() || column == "*") { return "*"; }
            return string.Join(",", column.Split(','));
        }

        private static readonly ConcurrentDictionary<string, List<string>> _entityExtFields = new ConcurrentDictionary<string, List<string>>();
        public static string ParseColumn<T>(string column)
        {
            try
            {
                if (column.IsNullOrEmptyExt() || column == "*") { return "*"; }

                var type = typeof(T);
                var typeKey = type.FullName;
                List<string> cf = null;
                foreach (var item in _entityExtFields)
                {
                    if (typeKey == item.Key)
                    {
                        cf = item.Value;
                        break;
                    }
                }

                if (cf == null)
                {
                    //var attr = (ExtFieldAttribute)type.GetCustomAttributes(typeof(ExtFieldAttribute), false).FirstOrDefault();
                    //List<string> exFields = new List<string>();
                    //if (attr != null && !string.IsNullOrWhiteSpace(attr.Fields))
                    //{
                    //    exFields.AddRange(attr.Fields.Split(','));
                    //}
                    //cf = _entityExtFields[typeKey] = exFields;
                }

                var columns = new List<string>();
                foreach (var item in column.Split(',').Distinct())
                {
                    if (item.StartsWith("CustomField", StringComparison.OrdinalIgnoreCase) || item.StartsWith("Custom_Field", StringComparison.OrdinalIgnoreCase)
                        || (cf != null && cf.Any(n => n.Equals(item, StringComparison.OrdinalIgnoreCase))))
                    {
                        //cột trong json
                        if (item.EndsWith("BooleanType", StringComparison.OrdinalIgnoreCase))
                        {
                            columns.Add($"if(JSON_UNQUOTE(JSON_EXTRACT(ExtData, '$.{item}')) = 'true', true, false) as {item}");
                        }
                        else
                        {
                            columns.Add($"JSON_UNQUOTE(JSON_EXTRACT(ExtData, '$.{item}')) as {item}");
                        }
                    }
                    else
                    {
                        //cột có trong bảng
                        columns.Add(SafeColumn(item));
                    }
                }

                return string.Join(",", columns);
            }
            catch (Exception ex)
            {
            }
            return "*";
        }


        /// <summary>
        /// Chuẩn hóa lại tên column trước khi ghép vào câu lệnh truy vấn
        /// Nếu truyền list ngăn cách dấu , thì tách ra check trước
        /// </summary>
        public static string SafeColumn(string column)
        {
            var sb = new StringBuilder();
            foreach (char c in column)
            {
                if (c == '_'
                    || (c >= 'a' && c <= 'z')
                    || (c >= 'A' && c <= 'Z')
                    || (c >= '0' && c <= '9')
                    )
                {
                    sb.Append(c);
                }
            }
            return $"{sb.ToString()}";
        }

        /// <summary>
        /// Parse chuỗi filter thành whereparameter để lấy dữ liệu paging
        /// </summary>
        /// <param name="input">Chuỗi filter client truyền lên</param>
        /// <returns></returns>
        public static WhereParameter ParseFilter(string input, WhereParameter where = null)
        {
            WhereParameter result = new WhereParameter();
            if (!input.IsNullOrEmptyExt())
            {
                JArray obj = JsonConvert.DeserializeObject<JArray>(input);

                StringBuilder builder = new StringBuilder();
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                if (where != null)
                {
                    parameters = where.WhereValues;
                    if (!string.IsNullOrWhiteSpace(where.GetWhereClause()))
                    {
                        builder.Append(" ( ");
                        builder.Append(where.GetWhereClause());
                        builder.Append(" ) ");
                    }
                    if (obj.Count() > 0 && (builder.Length > 0 || !string.IsNullOrWhiteSpace(builder.ToString())))
                    {
                        builder.Append(" AND ");
                    }
                    builder.Append(" ( ");
                }

                ConvertArray(obj, builder, parameters);
                if (where != null) { builder.Append(")"); }
                result.AddWhere(builder.ToString(), parameters);
            }
            else if (where != null)
            {
                result = where;
            }
            return result;
        }

        /// <summary>
        /// xử lý json string từ client truyền lên thanh Dictionary(string, object)
        /// </summary>
        /// <param name="jsonObject">chuỗi json từ client truyền lên</param>
        public static Dictionary<string, object> ParseClient(string jsonObject)
        {
            if (jsonObject.IsNullOrEmptyExt()) { return null; }

            var temp = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject);
            var result = new Dictionary<string, object>();
            foreach (var item in temp)
            {
                var value = item.Value;
                if (value is string)
                {
                    if (item.Key.Contains("Time") && DateTime.TryParseExact(item.Value.ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tempDate))
                    {
                        value = tempDate;
                    }
                    else if (item.Key.Contains("Date") && DateTime.TryParseExact(item.Value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                    {
                        value = tempDate.Date;
                    }
                }
                result.AddOrUpdate(item.Key, value);
            }

            return result;
        }

        /// <summary>
        /// Xử lý đối tượng JArray thành các thành phần của WhereParameter
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="builder"></param>
        /// <param name="parameters"></param>
        private static void ConvertArray(JArray arr, StringBuilder builder, Dictionary<string, object> parameters)
        {
            if (arr.First != null && arr.First.Type == JTokenType.Array)
            {
                foreach (JToken item in arr)
                {
                    //Nếu là Array thì add thêm dấu cặp mở đóng ngoặc và tiếp tục đệ quy
                    if (item.Type == JTokenType.Array)
                    {
                        builder.Append(" (");
                        ConvertArray((JArray)item, builder, parameters);
                        builder.Append(") ");
                    }
                    else
                    {
                        //Không phải Array thì thực hiện phân tích
                        if (string.Compare(item.Value<string>(), "and", true) == 0)
                        {
                            //Trường hợp Operand and
                            builder.Append(" AND ");
                        }
                        else if (string.Compare(item.Value<string>(), "or", true) == 0)
                        {
                            //Trường hợp Operand or
                            builder.Append(" OR ");
                        }
                    }
                }
            }
            else
            {
                //Nếu có phần tử đầu tiên không phải array thì là bộ filter, thực hiện parse
                builder.Append(ConvertFilterItem(arr, parameters));
            }
        }

        /// <summary>
        /// Covert 1 bộ filteritem sang chuỗi sql
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string ConvertFilterItem(JArray item, Dictionary<string, object> parameters)
        {
            string column = item.First.Value<string>();
            string operatorValue = item.First.Next.Value<string>().ToLower();
            //string paramValueAlias = "";
            string paramName = $"p{parameters.Count() + 1}";
            string paramNameAlias = $"@{paramName}";
            string operatorAlias = operatorValue;
            string pattern = " {0} {1} {2}";
            object dicValue = null;
            string stringValue;
            var valueItem = item.Count() >= 3 ? item[2] : item.Last;
            List<object> values = null;

            //Lấy value
            switch (valueItem.Type)
            {
                case JTokenType.TimeSpan:
                    dicValue = valueItem.Value<TimeSpan>();
                    break;
                case JTokenType.Date:
                    column = $"DATE({column})";
                    dicValue = valueItem.Value<DateTime>().Date;
                    break;
                case JTokenType.Integer:
                    dicValue = valueItem.Value<int>();
                    break;
                case JTokenType.Float:
                    dicValue = valueItem.Value<decimal>();
                    break;
                case JTokenType.Boolean:
                    dicValue = valueItem.Value<bool>();
                    break;
                case JTokenType.Array:
                    values = new List<object>();
                    foreach (JValue arrayItem in (JArray)valueItem)
                    {
                        values.Add(arrayItem.Value);
                    }
                    break;
                default:
                    dicValue = stringValue = valueItem.Value<string>().ToString();
                    //Kiểm tra kiểu dữ liệu khác như Guid, Bool
                    Guid guid;
                    if (Guid.TryParse(stringValue, out guid))
                    {
                        dicValue = guid;
                    }
                    break;
            }

            //Chuyển Operator, sửa lại paramValue theo Operator hoặc xử lý đặc biệt
            switch (operatorValue)
            {
                case "is":
                    operatorAlias = " = ";
                    break;
                case "contains":
                    operatorAlias = " LIKE ";
                    dicValue = $"%{ProcessLikeValue(dicValue.ToString())}%";
                    break;
                case "notcontains":
                    operatorAlias = " NOT LIKE ";
                    dicValue = $"%{ProcessLikeValue(dicValue.ToString())}%";
                    if (dicValue.ToString().Length > 0)
                    {
                        pattern = $"({{0}} IS NULL OR {pattern})";
                    }
                    break;
                case "startswith":
                    operatorAlias = " LIKE ";
                    dicValue = $"{ProcessLikeValue(dicValue.ToString())}%";
                    break;
                // UpdateBy TMChi 04/12/2019
                // thêm case notstartswith
                case "notstartswith":
                    operatorAlias = " NOT LIKE ";
                    dicValue = $"{ProcessLikeValue(dicValue.ToString())}%";
                    if (dicValue.ToString().Length > 0)
                    {
                        pattern = $"({0} IS NULL OR {pattern})";
                    }
                    break;
                case "endswith":
                    operatorAlias = " LIKE ";
                    dicValue = $"%{ProcessLikeValue(dicValue.ToString())}";
                    break;
                case "in":
                case "notin":
                    operatorAlias = operatorValue == "in" ? "IN" : "NOT IN";
                    var listClause = new List<string>();
                    if (values != null && values.Count() > 0)
                    {
                        foreach (var value in values)
                        {
                            paramName = $"p{parameters.Count() + 1}";
                            listClause.Add($"@{paramName}");
                            parameters.AddOrUpdate(paramName, value);
                        }
                    }
                    paramNameAlias = $"({string.Join(",", listClause)})";
                    break;
                case "is null":
                    operatorAlias = " IS NULL ";
                    if (valueItem.Value<string>() == "text")
                    {
                        pattern = "({0} IS NULL OR {0} =@" + paramName + ")";
                        dicValue = "";
                    }
                    else
                    {
                        pattern = "{0} IS NULL";
                    }
                    break;
                case "is not null":
                    operatorAlias = " IS NOT NULL ";
                    paramNameAlias = "";
                    if (valueItem.Value<string>() == "text")
                    {
                        pattern = "{0} IS NOT NULL AND {0} <>@" + paramName;
                        dicValue = "";
                    }
                    else
                    {
                        pattern = "{0} IS NOT NULL";
                    }
                    break;
                case "=":
                case ">=":
                case "<=":
                    //xử lý tình huống muốn lọc cả null = false/0
                    if (item.Count() > 3 && (0.Equals(dicValue) || false.Equals(dicValue)))
                    {
                        var nullToFail = item[3].Value<bool>();
                        if (nullToFail)
                        {
                            pattern = $"({{0}} is null OR {pattern})";
                        }
                    }
                    break;
                case "!=":
                    if (item.Count() > 3 && (0.Equals(dicValue) || false.Equals(dicValue)))
                    {
                        //Nothing
                    }
                    else
                    {
                        pattern = $"({{0}} is null OR {pattern})";
                    }
                    break;
            }

            if (dicValue != null)
            {
                parameters.AddOrUpdate(paramName, dicValue);
            }

            string res = string.Format(pattern, column, operatorAlias, paramNameAlias);
            //if (operatorValue == "contains")
            //{
            //    res = string.Concat(res, " COLLATE utf8mb4_general_ci");
            //}
            return res;
        }

        /// <summary>
        /// Xử lý giá trị của mệnh đề like
        /// </summary>
        /// <param name="value">giá trị lọc</param>
        private static string ProcessLikeValue(string value)
        {
            return value.Replace(@"\", @"\\").Replace("%", "\\%");
        }
    }
}
