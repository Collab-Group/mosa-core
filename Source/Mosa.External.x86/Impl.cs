using Mosa.Runtime;
using System;
using System.Collections.Generic;

namespace Mosa.External.x86
{
    public static class Impl
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

        //Not available until GC is setup
        public static string ToString(this ulong u) 
        {
            string s = "";
            ulong temp = u;

            do
            {
                s += (char)((temp % 10) + 0x30);
                temp /= 10;
            } while (temp != 0);

            string r = "";

            for (int i = 0; i < s.Length; i++)
            {
                r += s[s.Length - 1 - i];
            }

            return r;
        }
    }
}
