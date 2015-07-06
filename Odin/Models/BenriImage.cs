using System.Drawing;
using System.Drawing.Imaging;

namespace Odin.Models
{
    internal class BenriImage
    {
        public Bitmap Bitmap;

        public BenriImage(Bitmap bitmap)
        {
            Bitmap = bitmap;
            Format = bitmap.RawFormat;
            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        public BenriImage(Bitmap bitmap, string filePath)
        {
            Bitmap = bitmap;
            Format = bitmap.RawFormat;
            Width = bitmap.Width;
            Height = bitmap.Height;
            FilePath = filePath;
        }

        public BenriImage(BenriImage benriImage)
        {
            Bitmap = benriImage.Bitmap;
            Format = benriImage.Format;
            Width = benriImage.Width;
            Height = benriImage.Height;
            FilePath = benriImage.FilePath;
        }

        public ImageFormat Format { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public string FilePath { get; private set; }
    }
}