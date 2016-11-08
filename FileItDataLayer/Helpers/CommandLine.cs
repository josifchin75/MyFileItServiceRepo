using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace FileItDataLayer.Helpers
{
    public class CommandLine
    {
        public static string ImageMagickPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ImageMagickPath"].ToString();
            }
        }

        public static bool RunCommand(string exePath, string arguments)
        {
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,//"-resize 50% -draw \"gravity south fill black text 0,0 'Watermark' \" test.jpg result.jpg",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            proc.Start();
            string error = proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            return error.Length == 0;
        }
    }
}