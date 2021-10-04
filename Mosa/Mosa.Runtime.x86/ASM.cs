using System;
using System.Runtime.InteropServices;

namespace Mosa.Runtime.x86
{
    public static class ASM
    {
        [DllImport("MEMCPY.o", EntryPoint = "MEMCPY")]
        public static extern void MEMCPY(uint DEST, uint SOURCE, uint LENGTH);

        [DllImport("MEMFILL.o", EntryPoint = "MEMFILL")]
        public static extern void MEMFILL(uint DEST, uint LENGTH, uint VALUE);

        [DllImport("INSD.o", EntryPoint = "INSD")]
        public static extern void INSD(ushort PORT, uint DATA, uint COUNT);

        [DllImport("OUTSD.o", EntryPoint = "OUTSD")]
        public static extern void OUTSD(ushort PORT, uint DATA, uint COUNT);
    }
}
