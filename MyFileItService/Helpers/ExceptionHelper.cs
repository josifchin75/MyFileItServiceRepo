using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MyFileItService.Helpers
{
    public class ExceptionHelper
    {
        public const string BaseFileName = "MyFileItSVCErrors_";
        public static string LogDirectory = System.Configuration.ConfigurationManager.AppSettings["ErrorLog"].ToString();

        public static void LogError(Exception ex)
        {
            LogError(ex.ToString());
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

        public static void WriteFile(string s, string filePath, bool append)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filePath, append);
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

        public static string ReadFile(string filePath)
        {
            string result = "";
            StreamReader sr = null;

            try
            {
                sr = new StreamReader(filePath);
                result = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogError(ex);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
            return result;
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