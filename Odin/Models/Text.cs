using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Odin.Models
{
    internal static class Text
    {
        public static string ConvertToBinary(string text)
        {
            var result = "";
            var byteText = Encoding.UTF8.GetBytes(text);
            foreach (var b in byteText)
            {
                result += Convert.ToString(b, 2).PadLeft(8, '0');
            }

            return result;
        }

        public static string ConvertToString(string text)
        {
            var list = new List<byte>();
            for (var i = 0; i < text.Length; i += 8)
            {
                var binaryText = text.Substring(i, 8);
                var byteText = Convert.ToByte(binaryText, 2);
                list.Add(byteText);
            }

            var result = Encoding.UTF8.GetString(list.ToArray());

            return result;
        }
    }
}