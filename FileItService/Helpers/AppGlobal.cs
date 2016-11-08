using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace FileItService
{
    public class AppGlobal
    {
        public static string StationID
        {
            get
            {
                return ConfigurationManager.AppSettings["StationID"].ToString();
            }
        }

        public static bool DebugMode { get { return false; } }
    }
}