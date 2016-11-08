using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileItService.Helpers
{
    public static class Extensions
    {
        public static object GetPropertyValue(this object obj, string propertyName){
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName).SetValue(obj, value, null);
        }


    }
}