using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileItDataLayer.Helpers
{
    public class ImageMagickHelper
    {
        public static string CreateConvertArguments(string originalFilePath, string newFilePath)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(originalFilePath + " " + newFilePath);
            return sb.ToString();
        }

        public static string CreateResizeArguments(string originalFilePath, string newFilePath, int width) {
            var sb = new System.Text.StringBuilder();
            sb.Append(originalFilePath + " -resize " + width.ToString() + " " + newFilePath);
            return sb.ToString();
        }
    }
}
