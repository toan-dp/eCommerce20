using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SqlClientExtention.Extentions
{
    public static class ObjectExtension
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        public static void SetPropertyValue(this object src , string propertyName , object value)
        {
            var ModelType = src.GetType();
            var _propertyInfo = ModelType.GetProperty(propertyName);
            var _targetType = _propertyInfo.PropertyType.IsNullableType() ?
                        Nullable.GetUnderlyingType(_propertyInfo.PropertyType) : _propertyInfo.PropertyType;
            
            var _value = Convert.ChangeType(value, _targetType);
            _propertyInfo.SetValue(src, _value, null);
        }

        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var ModelInstance = new T();

            foreach (var item in source)
            {
                ModelInstance.SetPropertyValue(item.Key, item.Value);
            }
            return ModelInstance;
        }

        public static IDictionary<string, object> ToDictionary(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public static string GetElapsedTime(this DateTime? datetime)
        {

            if (datetime == null)
            {
                return "time was not specified";
            }

            TimeSpan ts = DateTime.Now.Subtract((DateTime)datetime);

            int years = ts.Days / 365;
            int months = ts.Days / 30;
            int weeks = ts.Days / 52;

            if (years == 1) // one year ago
            {
                return "A year ago";
            }

            if (years > 1) // greater than one year
            {
                if (ts.Days % 365 == 0) // even year
                {
                    return (int)(ts.TotalDays / 365) + " years ago";
                }
                else // not really entire years
                {
                    return "About " + (int)(ts.TotalDays / 365) + " years ago";
                }
            }

            if (months == 1) // one month
            {
                return "About a month ago";
            }

            if (months > 1) // more than one month
            {
                return "About " + months + " months ago";
            }

            if (weeks == 1) // a week ago
            {
                return "About a week ago";
            }

            if (weeks > 1) // more than a week ago, but less than a month ago
            {
                return "About " + weeks + " weeks ago";
            }

            if (ts.Days == 1) // one day ago
            {
                return "Yesterday";
            }

            if (ts.Days > 1) //  more than one day ago, but less than one week ago
            {
                return ts.Days + " days ago";
            }

            if (ts.Hours == 1) // An hour ago
            {
                return "About an hour ago";
            }

            if (ts.Hours > 1 && ts.Hours <= 24) // More than an hour ago, but less than a day ago
            {
                return "About " + ts.Hours + " hours ago";
            }

            if (ts.Minutes == 1)
            {
                return "About a minute ago";
            }

            if (ts.Minutes == 0)
            {
                return ts.Seconds + " seconds ago";
            }

            return ts.Minutes + " minutes ago";
        }

        public static int IndexOfAny(this string obj , string[] values , int startIndex = 0)
        {
            int index_of_firstmatch = -1;
            foreach (var val in values)
            {
                index_of_firstmatch = obj.IndexOf(val , startIndex);
                if (index_of_firstmatch != -1)
                    break;
            }

            return index_of_firstmatch;
        }

        public static int IndexOfAny(this string obj, List<string> values, int startIndex = 0)
        {
            int index_of_firstmatch = -1;
            string objLowerCase = obj.ToLower();
            foreach (var val in values)
            {
                index_of_firstmatch = objLowerCase.IndexOf(val.ToLower(), startIndex);
                if (index_of_firstmatch != -1)
                    break;
            }

            return index_of_firstmatch;
        }
    }
}
