namespace Mosa.Kernel.x86
{
    public class TableConvert
    {
        public static sbyte[] unhexTable =
          { -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
           ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
           ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
           , 0, 1, 2, 3, 4, 5, 6, 7, 8, 9,-1,-1,-1,-1,-1,-1
           ,-1,10,11,12,13,14,15,-1,-1,-1,-1,-1,-1,-1,-1,-1
           ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
           ,-1,10,11,12,13,14,15,-1,-1,-1,-1,-1,-1,-1,-1,-1
           ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
          };

        public static int Convert(string hexNumber)
        {
            int decValue = unhexTable[(byte)hexNumber[0]];
            for (int i = 1; i < hexNumber.Length; i++)
            {
                decValue *= 16;
                decValue += unhexTable[(byte)hexNumber[i]];
            }
            return decValue;
        }
    }
}
