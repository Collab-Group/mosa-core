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
            GC.DisposeObject(ls);
            return result;
        }
    }
}
