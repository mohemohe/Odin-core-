namespace Odin.Models
{
    internal static class Core
    {
        private static BenriImage image;

        public static string Read(string filePath)
        {
            var result = "";

            image = IO.ReadImage(filePath);
            if (Image.IsFormated(ref image))
            {
                var binaryText = Image.Read(ref image);
                result = Text.ConvertToString(binaryText);
            }
            else
            {
                Image.Format(ref image);
            }

            return result;
        }

        public static void Write(string text)
        {
            var writeText = Text.ConvertToBinary(text);
            if (!string.IsNullOrEmpty(text))
            {
                Image.Write(ref image, writeText);
            }
            IO.SaveImagePNG(ref image);
        }
    }
}