using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileItDataLayer.Helpers
{
    public class ExceptionHelper
    {
        public static bool DebugMode = false;
        public const string BaseFileName = "FileItInnerSVCErrors_";
        public static string LogDirectory = System.Configuration.ConfigurationManager.AppSettings["ErrorLog"].ToString();


        public static void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        public static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            LogError(message);
        }

        public static void LogError(Exception ex)
        {
            LogError(ex.ToString());
        }

        public static void LogDebug(string s)
        {
            if (DebugMode)
            {
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
            if (LogDirectory == null || LogDirectory.Length == 0) {
                LogDirectory = @"C:\Logs";
            }
            var filename = Path.Combine(LogDirectory, BaseFileName + DateTime.Now.ToString("MMddyyyy") + ".log");
            return filename;
        }
    }
}
