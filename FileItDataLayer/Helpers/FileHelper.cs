using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileItDataLayer.Helpers
{
    public class FileHelper
    {
        public static string ReadFile(string filepath)
        {
            var result = "";
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filepath);
                result = sr.ReadToEnd();
            }
            catch (Exception ex) {
                ExceptionHelper.Log(ex);
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

        public static string WriteFile(string filepath, string text, bool append)
        {
            var result = "";
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filepath, append);
                
                foreach (var c in text) {
                    sw.Write(c);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
            }
            return result;
        }

        public static string FileToBase64(string filePath)
        {
            byte[] byteArray = System.IO.File.ReadAllBytes(filePath);
            return Convert.ToBase64String(byteArray);
        }

        public static bool Base64ToFile(string filePath, string base64)
        {
            var result = false;
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(base64));
                result = true;
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex);
            }
            return result;
        }

        public static int HexToInt(string hex){
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        public static string IntToHex(int i){
            return i.ToString("X8");
        }

        /*  public static int GetBase64FileSize(string base64) {
           
              File.WriteAllBytes(filePath, Convert.FromBase64String(base64));
          }*/
    }
}
