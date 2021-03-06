﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFileItService.Helpers
{
    public static class Extensions
    {
        public static string ConvertToFirebirdBoolean(this string s)
        {
            var result = "0";
            var trueStrings = new List<string>() { 
                "true",
                "1"
            };
            if (s != null)
            {
                trueStrings.ForEach(t =>
                {
                    if (s.Equals(t, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result = "1";
                    }
                });
            }
            return result;
        }

        public static string ConvertToFirebirdString(this bool b)
        {
            var result = b ? "1" : "0";
            return result;
        }

        public static string FBDateFormat = "MM/dd/yyyy";

        public static string ConvertToFirebirdDate(this DateTime? d) {
            var result = "12/31/1977";
            if (d != null) {
                DateTime date;
                if (DateTime.TryParse(d.ToString(), out date)) {
                    result = date.ToString(FBDateFormat);
                }
            }
            return result;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName).SetValue(obj, value, null);
        }


    }
}