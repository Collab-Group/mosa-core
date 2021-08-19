using Mosa.Runtime;

namespace Mosa.External.x86
{
    public static class ArrayImpl
    {
        public static void Reverse(this byte[] array)
        {
            int n = array.Length;
            byte[] current = array;

            for (int i = 0; i < n; i++)
                array[n - 1 - i] = current[i];

            GC.DisposeObject(current);
        }
    }
}
