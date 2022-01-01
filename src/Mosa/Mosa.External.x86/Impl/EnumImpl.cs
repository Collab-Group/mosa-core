using Mosa.Runtime;
using System;

namespace Mosa.External.x86
{
    public static class EnumImpl
    {
        public static unsafe string GetEnumName(this Enum @enum)
        {
            string result = null;

            string c1;
            string c2;
            string c3;

            string s = (string)Intrinsic.GetObjectFromAddress(new Pointer(@enum.TypeDefinition->Name));
            int* p = (int*)Intrinsic.GetObjectAddress(@enum);

            string[] sl = s.Split(',');
            for (int i = 0; i < sl.Length; i++)
            {
                c1 = sl[i].Split(':')[1];
                c2 = p[2].ToString();
                c3 = sl[i].Split(':')[0];

                if (c1 == c2)
                {
                    result = c3;
                }

                c1.Dispose();
                c2.Dispose();
                //c3.Dispose();
            }

            sl.Dispose();

            return result;
        }
    }
}
