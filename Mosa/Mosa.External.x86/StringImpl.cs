using Mosa.Runtime;
using System.Collections.Generic;

namespace Mosa.External.x86
{
    public unsafe static class StringImpl
    {
        public static string[] Split(this string s, char c)
        {
            string str = s;
            List<string> ls = new List<string>();
            int indx;

            while ((indx = str.IndexOf(c)) != -1)
            {
                ls.Add(str.Substring(0, indx));
                str = str.Substring(indx + 1);
            }
            if (str.Length > 0)
            {
                ls.Add(str);
            }
            string[] result = ls.ToArray();
            GC.Dispose(ls);
            return result;
        }

        public static string PadLeft(this string str, int totalWidth, char paddingChar)
        {
            if (totalWidth < str.Length) return str;
            int len = totalWidth - str.Length;
            string result = "";
            for (int i = 0; i < len; i++)
            {
                result += paddingChar;
            }
            result = result + str;

            GC.Dispose(str);

            return result;
        }

        public static string PadRight(this string str, int totalWidth, char paddingChar)
        {
            if (totalWidth < str.Length) return str;
            int len = totalWidth - str.Length;
            string result = "";
            for (int i = 0; i < len; i++)
            {
                result += paddingChar;
            }
            result = str + result;

            GC.Dispose(str);

            return result;
        }
    }
}
