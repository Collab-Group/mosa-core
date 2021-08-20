using Mosa.Runtime;

namespace Mosa.External.x86
{
    public static class ArrayImpl
    {
        public static void Reverse<T>(T[] array)
        {
            int n = array.Length;
            T[] aux = array;

            for (int i = 0; i < n; i++)
                array[n - 1 - i] = aux[i];

            GC.DisposeObject(aux);
        }

        public static void Reverse<T>(T[] array, int index, int length)
        {
            T[] aux = array;

            for (int i = index; i < length; i++)
                array[length - 1 - i] = aux[i];

            GC.DisposeObject(aux);
        }
    }
}
