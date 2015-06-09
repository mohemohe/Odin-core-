using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Odin.Models
{
    internal static class IO
    {
        public static BenriImage ReadImage(string filePath)
        {
            BenriImage image;
            using (var bitmap = new Bitmap(filePath))
            {
                image = new BenriImage((Bitmap) bitmap.Clone(), filePath);
            }

            return image;
        }

        public static void SaveImage(ref BenriImage image, string filePath = null)
        {
            if (filePath == null)
            {
                var ext = Path.GetExtension(image.FilePath);
                filePath = image.FilePath.Remove(image.FilePath.Length - ext.Length) + @"." + DateTime.Now.Ticks + ext;
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            image.Bitmap.Save(filePath, image.Format);
        }

        public static void SaveImagePNG(ref BenriImage image, string filePath = null)
        {
            if (filePath == null)
            {
                var ext = Path.GetExtension(image.FilePath);
                filePath = image.FilePath.Remove(image.FilePath.Length - ext.Length) + @"." + DateTime.Now.Ticks + @".png";
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            image.Bitmap.Save(filePath, ImageFormat.Png);
        }
    }
}