using Mosa.Runtime;
using System;
using System.Collections.Generic;

namespace Mosa.External.x86
{
    public static class StringImpl
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


        public static string Format(string format, params object?[] args)
        {
            string result = "";
            int count = 0;
            for (int i = 0; i < format.Length; i++)
            {
                if (count < 10 && format[i] == '{' && format[i + 1] == (count.ToString())[0] && format[i + 2] == '}')
                {
                    result += args[count].ToString();
                    i += 3;
                    count++;
                    continue;
                }
                result += format[i];
            }
            return result;
        }
    }
}
