using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Models
{
    internal static class Image
    {
        public static unsafe async Task<BenriImage> Format(BenriImage image)
        {
            var bmp = image.Bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width, image.Height)),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var ptr = (byte*) (void*) bmp.Scan0;

            for (var y = 0; y < bmp.Height; ++y)
            {
                for (var x = 0; x < bmp.Width; ++x)
                {
                    var r = *(ptr + 2)%10.0;
                    byte rVal;
                    switch ((int) r)
                    {
                        case 1:
                            rVal = (byte) (*(ptr + 2) - 1); // X0
                            break;

                        case 2:
                            rVal = (byte) (*(ptr + 2) + 2); // X4
                            break;

                        case 3:
                            rVal = (byte) (*(ptr + 2) + 1); // X7
                            break;

                        default:
                            rVal = *(ptr + 2);
                            break;
                    }
                    *(ptr + 2) = rVal;

                    var g = *(ptr + 1)%10.0;
                    byte gVal;
                    switch ((int) g)
                    {
                        case 1:
                            gVal = (byte) (*(ptr + 1) - 1);
                            break;

                        case 2:
                            gVal = (byte) (*(ptr + 1) + 2);
                            break;

                        case 3:
                            gVal = (byte) (*(ptr + 1) + 1);
                            break;

                        default:
                            gVal = *(ptr + 1);
                            break;
                    }
                    *(ptr + 1) = gVal;

                    var b = *ptr%10.0;
                    byte bVal;
                    switch ((int) b)
                    {
                        case 1:
                            bVal = (byte) (*ptr - 1);
                            break;

                        case 2:
                            bVal = (byte) (*ptr + 2);
                            break;

                        case 3:
                            bVal = (byte) (*ptr + 1);
                            break;

                        default:
                            bVal = *ptr;
                            break;
                    }
                    *(ptr) = bVal;
                    ptr += 4;
                }
            }
            image.Bitmap.UnlockBits(bmp);

            return image;
        }

        public static unsafe async Task<bool> IsFormated(BenriImage image)
        {
            var bmp = image.Bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width, image.Height)),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var ptr = (byte*) (void*) bmp.Scan0;

            for (var y = 0; y < bmp.Height; ++y)
            {
                for (var x = 0; x < bmp.Width; ++x)
                {
                    var r = *(ptr + 2)%10.0;
                    var g = *(ptr + 1)%10.0;
                    var b = *ptr%10.0;

                    if ((int) r == 1 || (int) g == 1 || (int) b == 1)
                    {
                        image.Bitmap.UnlockBits(bmp);
                        return false;
                    }

                    ptr += 4;
                }
            }

            image.Bitmap.UnlockBits(bmp);
            return true;
        }

        public static unsafe async Task<BenriImage> Write(BenriImage image, string text)
        {
            var bmp = image.Bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width, image.Height)),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var ptr = (byte*) (void*) bmp.Scan0;

            var pos = 0;

            for (var y = 0; y < bmp.Height; ++y)
            {
                for (var x = 0; x < bmp.Width; ++x)
                {
                    var r = Math.Floor(*(ptr + 2)/10.0);
                    byte rVal;
                    var g = Math.Floor(*(ptr + 1)/10.0);
                    byte gVal;
                    var b = Math.Floor(*ptr/10.0);
                    byte bVal;

                    var binaryText = text.Substring(pos, 1);
                    switch (binaryText)
                    {
                        case "0":
                            rVal = Convert.ToByte(r + "2");
                            gVal = Convert.ToByte(g + "2");
                            bVal = Convert.ToByte(b + "2");
                            break;

                        case "1":
                            rVal = Convert.ToByte(r + "3");
                            gVal = Convert.ToByte(g + "3");
                            bVal = Convert.ToByte(b + "3");
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    *(ptr + 2) = rVal;
                    *(ptr + 1) = gVal;
                    *(ptr) = bVal;

                    ptr += 4;

                    if (pos < text.Length - 1)
                    {
                        pos++;
                    }
                    else
                    {
                        image.Bitmap.UnlockBits(bmp);
                        return image;
                    }
                }
            }
            image.Bitmap.UnlockBits(bmp);

            return image;
        }

        public static unsafe async Task<string> Read(BenriImage image)
        {
            var result = new StringBuilder(8 * 1024);

            var bmp = image.Bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width, image.Height)),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var ptr = (byte*) (void*) bmp.Scan0;

            for (var y = 0; y < bmp.Height; ++y)
            {
                for (var x = 0; x < bmp.Width; ++x)
                {
                    var r = *(ptr + 2)%10.0;
                    var g = *(ptr + 1)%10.0;
                    var b = *ptr%10.0;

                    if ((int) r == 2 || (int) g == 2 || (int) b == 2)
                    {
                        result.Append("0");
                    }
                    else if ((int) r == 3 || (int) g == 3 || (int) b == 3)
                    {
                        result.Append("1");
                    }
                    else
                    {
                        image.Bitmap.UnlockBits(bmp);
                        return result.ToString();
                    }

                    ptr += 4;
                }
            }

            image.Bitmap.UnlockBits(bmp);
            return result.ToString();
        }
    }
}