using Cms.Core.Common.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    public class WhereParameter
    {
        private const string PrefixWhereParameter = " WHERE 1=1 ";
        private readonly Dictionary<string, object> _whereValues = new Dictionary<string, object>();
        private string _whereClause;
        private bool _isAppendWhere = true;

        public string WhereClause
        {
            get
            {
                return _whereClause;
            }
        }

        public Dictionary<string, object> WhereValues
        {
            get
            {
                return _whereValues;
            }
        }

        public bool IsAppendWhere
        {
            get
            {
                return _isAppendWhere;
            }
            set
            {
                _isAppendWhere = value;
            }
        }

        public string CompiledWhereClause
        {
            get
            {
                return FormatDynamicWhere(this);
            }
        }

        public void AddWhere(string sWhereClause, Dictionary<string, object> dictWhereValues)
        {
            StringBuilder stringBuilder = new StringBuilder(sWhereClause);
            _whereClause += stringBuilder.ToString();
            _whereClause = $"({_whereClause})";

            if(dictWhereValues != null && dictWhereValues.Count > 0)
            {
                foreach (KeyValuePair<string, object> current in dictWhereValues)
                {
                    _whereValues.AddOrUpdate(current.Key, current.Value);
                }
            }
        }

        public void AddWhere(WhereParameter oWhereParameter)
        {
            if (oWhereParameter != null)
            {
                AddWhere(oWhereParameter.WhereClause, oWhereParameter.WhereValues);
            }
        }

        public static string FormatDynamicWhere(WhereParameter whereParams)
        {
            if (whereParams != null && !string.IsNullOrWhiteSpace(whereParams.WhereClause))
            {
                StringBuilder stringBuilder = new StringBuilder(whereParams.WhereClause);
                foreach (KeyValuePair<string, object> current in whereParams.WhereValues)
                {
                    if (current.Value != null)
                    {
                        KeyValuePair<string, object> keyValuePair = current;
                        if (current.Value != null)
                        {
                            if(current.Value.GetType() == typeof(string))
                            {
                                keyValuePair = new KeyValuePair<string, object>(current.Key, SecureUtil.SafeSqlLiteral(current.Value.ToString() + ""));
                            }
                            stringBuilder.Replace("{" + keyValuePair.Key + "}", string.Concat(keyValuePair.Value) ?? "");
                        }
                    }
                }

                string result;
                if (whereParams.IsAppendWhere)
                {
                    result = AppendWherePrefix(stringBuilder.ToString());
                }
                else
                {
                    result = stringBuilder.ToString();
                }
                return result;
            }
            return string.Empty;
        }

        private static string AppendWherePrefix(string whereClause)
        {
            if (!whereClause.Trim().StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
            {
                if (!whereClause.Trim().StartsWith("AND ", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause = "AND (" + whereClause + ")";
                }

                string prefixWhereParameter = PrefixWhereParameter;
                if (!string.IsNullOrEmpty(prefixWhereParameter))
                {
                    whereClause = prefixWhereParameter + whereClause;
                }
            }
            return whereClause;
        }

        public string GetWhereClause()
        {
            return _whereClause;
        }

        public void AddWhere(string clause, object value)
        {
            if (!string.IsNullOrEmpty(_whereClause))
            {
                _whereClause += " AND ";
            }
            _whereClause += clause;
            string paramName = "p" + (WhereClause.Count() + 1).ToString();
            WhereValues.AddOrUpdate(paramName, value);
        }
        public void AddWhere(string clause, string paramName, object paramValue)
        {
            if (!string.IsNullOrEmpty(_whereClause))
            {
                _whereClause += " AND ";
            }
            _whereClause += clause;
            WhereValues.AddOrUpdate(paramName, paramValue);
        }
    }
}
