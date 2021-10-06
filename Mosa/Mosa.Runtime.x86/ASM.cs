using Mosa.Runtime.Plug;
using System.Runtime.InteropServices;

namespace Mosa.Runtime.x86
{
    public static class ASM
    {
        [DllImport("MEMCLR.o")]
        public static extern void MEMCLR(uint DEST, uint LENGTH);

        [DllImport("MEMCPY.o")]
        public static extern void MEMCPY(uint DEST, uint SOURCE, uint LENGTH);

        [DllImport("MEMFILL.o")]
        public static extern void MEMFILL(uint DEST, uint LENGTH, uint VALUE);

        [DllImport("INSD.o")]
        public static extern void INSD(ushort PORT, uint DATA, uint COUNT);

        [DllImport("OUTSD.o")]
        public static extern void OUTSD(ushort PORT, uint DATA, uint COUNT);
    }
}
