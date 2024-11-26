﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Core.Common.Extension
{
    public static class ExtensionUtility
    {
        /// <summary>
        /// Lấy ra table name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableNameOnly(this Type type)
        {
            string tableName = ((ConfigTableAttribute)type.GetCustomAttributes(typeof(ConfigTableAttribute), true)?.FirstOrDefault())?.TableName;
            return tableName;
        }

        /// <summary>
        /// Lấy ra view name, nếu ko có view name thì lấy ra table name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string? GetViewNameOnly(this Type type)
        {
            ConfigTableAttribute table = (ConfigTableAttribute)type.GetCustomAttributes(typeof(ConfigTableAttribute), true)?.FirstOrDefault();
            if (table != null)
            {
                if (!string.IsNullOrEmpty(table.ViewName))
                {
                    return table.ViewName;
                }
                return table.TableName;
            }
            return "";
        }

        /// <summary>
        /// Lấy về cột khoá chính
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetPrimaryKeyFieldName(this Type type)
        {
            return type.GetFieldName(typeof(KeyAttribute));
        }

        /// <summary>
        /// Lấy về tên cột chứa Attribute
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attrType"></param>
        /// <returns></returns>
        public static string GetFieldName(this Type type, Type attrType)
        {
            string primaryKeyName = string.Empty;
            PropertyInfo[] props = type.GetProperties();
            if (props != null)
            {
                var propertyInfoKey = props.SingleOrDefault(p => p.GetCustomAttribute(attrType, true) != null);
                if (propertyInfoKey != null)
                {
                    primaryKeyName = propertyInfoKey.Name;
                }
            }
            return primaryKeyName;
        }

        /// <summary>
        /// Lấy ra các cột của bảng
        /// </summary>
        /// <returns></returns>
        public static List<string> GetColumnTable(this Type type)
        {
            var result = new List<string>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var property in properties)
            {
                if(property.GetCustomAttribute(typeof(NotMappedAttribute), true) == null)
                {
                    result.Add(property.Name);
                }
            }
            return result;
        }

        /// <summary>
        /// Lấy giá trị object theo Key
        /// </summary>
        /// <param name="objEntity">object</param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetValue(this object objEntity, string propertyName)
        {
            if (objEntity != null && !string.IsNullOrEmpty(propertyName))
            {
                if (objEntity.GetType() == typeof(Dictionary<string, object>))
                {
                    return (objEntity as Dictionary<string, object>).Get(propertyName);
                }
                else
                {
                    PropertyInfo info = objEntity.GetType().GetProperty(propertyName);
                    if (info != null)
                    {
                        return info.GetValue(objEntity);
                    }
                    else
                    {
                        return objEntity.ToDictionary().Get(propertyName);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Set giá trị object theo Key
        /// </summary>
        /// <param name="objEntity"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetValue(this object objEntity, string propertyName, object value)
        {
            if (objEntity == null)
                throw new ArgumentNullException(nameof(objEntity), "Object cannot be null.");

            var property = objEntity.GetType().GetProperty(propertyName);

            if (property == null)
                throw new ArgumentException($"Property '{propertyName}' not found on object of type '{objEntity.GetType().Name}'.");

            if (!property.CanWrite)
                throw new InvalidOperationException($"Property '{propertyName}' is read-only and cannot be set.");

            // Convert the value to the property's type, if necessary
            var convertedValue = Convert.ChangeType(value, property.PropertyType);

            // Set the value
            property.SetValue(objEntity, convertedValue);
        }

        /// <summary>
        /// Get giá trị từ Dictionary
        /// </summary>
        /// <returns></returns>
        public static object Get(this Dictionary<string, object> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            return null;
        }

        /// <summary>
        /// Chuyển obj thành Dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null)
            {
                return new Dictionary<string, object>();
            }
            else
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(obj));
            }
        }

        /// <summary>
        /// Thêm 1 trường vs giá trị hoặc có rồi thì update vào Dictionary
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <param name=""></param>
        public static void AddOrUpdate<K, V>(this Dictionary<K, V> keyValuePairs, K key, V value)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs[key] = value;
            }
            else
            {
                keyValuePairs.Add(key, value);
            }
        }

        /// <summary>
        /// Ktra chuỗi có rỗng
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyExt(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Lấy giá trị tại index nào đó
        /// </summary>
        /// <param name="splits"></param>
        /// <param name="index"></param>
        /// <param name="isTrim"></param>
        /// <returns></returns>
        public static string ValueExt(this string[] splits, int index, bool isTrim = true)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (splits == null || splits.Length <= 0) { return default; }

                // Lấy giá trị tại index
                var value = splits[index];
                if (value.IsNullOrEmptyExt()) { return default; }

                // Trả kết quả theo điều kiện
                return isTrim ? value.Trim() : value;
            }
            catch (IndexOutOfRangeException ex)
            {
            }
            catch (Exception ex)
            {
            }
            return default;
        }

    }
}