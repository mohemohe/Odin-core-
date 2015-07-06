using System.Threading.Tasks;

namespace Odin.Models
{
    internal static class Core
    {
        public static BenriImage _image;

        public static async Task<string> Read(string filePath)
        {
            var result = "";

            _image = await IO.ReadImage(filePath);
            if (await Image.IsFormated(_image))
            {
                var binaryText = await Image.Read(_image);
                result = await Text.ConvertToString(binaryText);
            }

            return result;
        }

        public static async Task<string> Write(string text)
        {
            if (_image != null)
            {
                var image = _image;
                image = await Image.Format(image);

                var writeText = await Text.ConvertToBinary(text);
                if (!string.IsNullOrEmpty(text))
                {
                    image = await Image.Write(image, writeText);
                }
                return await IO.SaveImagePNG(image);
            }
            return null;
        }
    }
}