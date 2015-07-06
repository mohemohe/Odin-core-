using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Odin.Models
{
    internal static class IO
    {
        public static async Task<BenriImage> ReadImage(string filePath)
        {
            BenriImage image;
            using (var bitmap = new Bitmap(filePath))
            {
                image = new BenriImage((Bitmap) bitmap.Clone(), filePath);
            }

            return image;
        }

        public static void SaveImage(BenriImage image, string filePath = null)
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

        public static async Task<string> SaveImagePNG(BenriImage image, string filePath = null)
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

            return Path.GetFileName(filePath);
        }
    }
}