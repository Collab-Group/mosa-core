using Mosa.Runtime;

namespace Mosa.External.x86.IO
{
    public class ReadableBytes
    {
        private readonly byte[] UnderlyingArray;

        private int Position = 0;

        public ReadableBytes(byte[] source)
        {
            UnderlyingArray = source;
        }

        public byte[] Read(int offset, int count)
        {
            byte[] buffer = new byte[count];

            for (int i = 0; i < count; i++)
                buffer[Position + offset + i] = UnderlyingArray[i];

            Position += count;
            return buffer;
        }

        public int ReadByte(int offset = 0)
        {
            int pos = Position + offset;

            if (pos > UnderlyingArray.Length)
                return -1;

            byte b = UnderlyingArray[pos];
            Position++;

            return b;
        }

        public void Free()
        {
            GC.DisposeObject(UnderlyingArray);
        }
    }
}
