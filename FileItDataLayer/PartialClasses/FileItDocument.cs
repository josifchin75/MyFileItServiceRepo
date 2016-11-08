using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using FileItDataLayer.Helpers;

namespace FileItDataLayer.Models
{
    public class FileItDocument
    {
        public string FILENAME { get; set; }
        public string EXTENSION { get; set; }
        public string ARCHIVED { get; set; }
        public DateTime INDEXEDON { get; set; }
        public int PAGENUMBER { get; set; }
        public int FILESIZE { get; set; }
        public string PIXELTYPE { get; set; }
        public string PIXELDEPTH { get; set; }
        public string RESOLUTION { get; set; }
        public string PIXELSIZEX { get; set; }
        public string PIXELSIZEY { get; set; }
        public string ORIENTATION { get; set; }
        public string STORAGEID { get; set; }
        public string SIDE { get; set; }
        public bool PUBLIC { get; set; } //**
        public int VERSION_NO { get; set; }
        public bool CHECKEDOUT { get; set; } //**
        public string VERSIONID { get; set; }
        public string ALTSTORAGE { get; set; }
        public bool DELETED { get; set; } //**
        public DateTime LASTVIEWED { get; set; }
        public DateTime LASTMODIFIED { get; set; }
        public string USERID { get; set; }
        public string STATIONID { get; set; }
        public string BATCHNO { get; set; }

        public Dictionary<int, string> Indexes { get; set; }

        public string ImageFilePath { get; set; }
        public string ImageBase64 { get; set; }
        public string WebImageBase64 { get; set; }
        public string WebImageBase64Src { get; set; }
        public string WebImageThumbBase64 { get; set; }
        public string WebImageThumbBase64Src { get; set; }

        public bool IncludeThumbnail { get; set; }

        public static string GetNextFileName()
        {
            var file = Path.Combine(FileItBasePath, "counter.cnt");
            var currentInt = 0;

            if (!File.Exists(file))
            {
                currentInt = 100;
            }
            else
            {
                currentInt = FileHelper.HexToInt(FileHelper.ReadFile(file));
            }
            currentInt++;

            FileHelper.WriteFile(file, FileHelper.IntToHex(currentInt), false);

            return FileHelper.IntToHex(currentInt);
        }

        public static string FileItBasePath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["FileItBasePath"].ToString();
            }
        }

        public int ThumbWidth
        {
            get
            {
                return 200;
            }
        }

        public string PngFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(ImageFilePath), Path.GetFileNameWithoutExtension(ImageFilePath) + ".png");
            }
        }

        public string PngThumbFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(PngFilePath), Path.GetFileNameWithoutExtension(PngFilePath) + "_thumb.png");
            }
        }

        public FileItDocument(DataRow dr, string oStorePath, bool includeThumbnail)
        {
            Indexes = new Dictionary<int, string>();
            IncludeThumbnail = includeThumbnail;
            //load up the object from the datarow
            foreach (DataColumn col in dr.Table.Columns)
            {
                if (col.ColumnName.Contains("INDEX") && col.ColumnName != "INDEXEDON")
                {
                    Indexes.Add(int.Parse(col.ColumnName.Replace("INDEX", "")), dr[col.ColumnName].ToString());
                }
                else
                {
                    SetPropValue(col.ColumnName, dr[col.ColumnName].ToString());
                }
            }

            SetImageFilePath(oStorePath);
        }

        public FileItDocument()
        {

        }

        public string AddImageFileFolderPath(string oStorePath)
        {
            var result = "";
            result = Path.Combine(oStorePath, GetDateDirectoryName(INDEXEDON));
            return result;
        }

        private void SetImageFilePath(string oStorePath)
        {
            //check the index date folder path
            var lookupfile = FILENAME + "." + EXTENSION;
            var lookuppath = Path.Combine(oStorePath, lookupfile);
            if (!File.Exists(lookuppath))
            {
                lookuppath = Path.Combine(oStorePath, GetDateDirectoryName(INDEXEDON), lookupfile);
                if (!File.Exists(lookuppath))
                {
                    lookuppath = null;
                }
            }
            ImageFilePath = lookuppath;
            GetWebImageBase64();
        }

        private void GetWebImageBase64()
        {
            //get the base 64 for display
            if (ImageFilePath != null)
            {
                //byte[] imageArray = System.IO.File.ReadAllBytes(ImageFilePath);
                //ImageBase64 = Convert.ToBase64String(imageArray);
                ImageBase64 = FileHelper.FileToBase64(ImageFilePath);
                WebImageBase64 = EXTENSION.Equals("tif", StringComparison.CurrentCultureIgnoreCase) ? ConvertImageToPng() : ImageBase64;
                WebImageBase64Src = "data:" + SrcImageType + ";base64," + WebImageBase64;
                if (IncludeThumbnail)
                {
                    WebImageThumbBase64 = ResizeImage(ThumbWidth);
                    WebImageThumbBase64Src = "data:image/png;base64," + WebImageThumbBase64;
                }
            }
        }

        private string SrcImageType
        {
            get
            {
                var result = "image/png";
                switch (this.EXTENSION.ToLower())
                {
                    case "jpg":
                        result = "image/jpg";
                        break;
                    case "txt":
                        //result = "text/plain";
                        break;
                    case "pdf":
                        //result = "application/pdf";
                        break;
                }
                return result;
            }
        }

        private string GetDateDirectoryName(DateTime d)
        {
            var result = d.Year + "-" + d.Month.ToString().PadLeft(2, '0') + "-" + d.Day.ToString().PadLeft(2, '0');
            return result;
        }

        private void SetPropValue(string prop, object val)
        {
            Type type = this.GetType();
            PropertyInfo propInfo = type.GetProperty(prop);

            //bool check
            if (propInfo.PropertyType == false.GetType() && (val.ToString().Equals("N", StringComparison.CurrentCultureIgnoreCase) || val.ToString().Equals("Y", StringComparison.CurrentCultureIgnoreCase)))
            {
                val = val.ToString().Equals("Y", StringComparison.CurrentCultureIgnoreCase) ? "true" : "false";
            }

            DateTime d = new DateTime();
            if (propInfo.PropertyType == d.GetType() && val.ToString().Length == 0)
            {
                val = null;
            }
            if (val != null)
            {
                propInfo.SetValue(this, Convert.ChangeType(val, propInfo.PropertyType), null);
            }
        }

        public string ConvertImageToPng()
        {
            var result = "";
            var args = ImageMagickHelper.CreateConvertArguments(this.ImageFilePath, PngFilePath);
            var exePath = CommandLine.ImageMagickPath;
            CommandLine.RunCommand(exePath, args);
            if (File.Exists(PngFilePath))
            {
                result = FileHelper.FileToBase64(PngFilePath);
            }
            return result;
        }

        public string ResizeImage(int width)
        {
            var result = "";

            var args = ImageMagickHelper.CreateResizeArguments((EXTENSION == "TIFF" || EXTENSION == "TIF" ? PngFilePath : ImageFilePath), PngThumbFilePath, ThumbWidth);
            var exePath = CommandLine.ImageMagickPath;
            CommandLine.RunCommand(exePath, args);
            if (File.Exists(PngThumbFilePath))
            {
                result = FileHelper.FileToBase64(PngThumbFilePath);
            }
            return result;
        }


    }
}
