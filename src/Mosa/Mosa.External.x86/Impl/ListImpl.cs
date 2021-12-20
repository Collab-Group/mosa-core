using System.Collections.Generic;

namespace Mosa.External.x86
{
    public static class ListImpl
    {
        public static List<byte> Skip(this List<byte> list, int num)
        {
            List<byte> newList = new List<byte>();

            for (int i = num; i < list.Count; i++)
                newList[i - num] = list[i];

            return newList;
        }

        public static List<byte> Take(this List<byte> list, int num)
        {
            List<byte> newList = new List<byte>();

            for (int i = 0; i < num; i++)
                newList[i] = list[i];

            return newList;
        }
    }
}
