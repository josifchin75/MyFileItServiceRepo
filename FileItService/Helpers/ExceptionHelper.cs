using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FileItService.Helpers
{
    public class ExceptionHelper
    {
        public const string BaseFileName = "FileItMainSVCErrors_";
        public static string LogDirectory = System.Configuration.ConfigurationManager.AppSettings["ErrorLog"].ToString();

        public static void LogError(Exception ex)
        {
            LogError(ex.ToString());
        }

        public static void LogDebug(string s) {
            if (AppGlobal.DebugMode) {
                LogError(s);
            }
        }

        public static void LogError(string s)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(GetLogFileName(), true);
                s = GetDateString() + s + System.Environment.NewLine + System.Environment.NewLine;
                foreach (var c in s)
                {
                    sw.Write(c);
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        public static string GetDateString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("**************************************************" + System.Environment.NewLine);
            sb.Append(DateTime.Now.ToString() + System.Environment.NewLine);
            sb.Append("--------------------------------------------------" + System.Environment.NewLine);
            return sb.ToString();
        }

        public static string GetLogFileName()
        {
            var filename = Path.Combine(LogDirectory, BaseFileName + DateTime.Now.ToString("MMddyyyy") + ".log");
            return filename;
        }
    }
}