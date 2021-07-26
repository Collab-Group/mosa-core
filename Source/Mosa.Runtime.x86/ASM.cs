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
    }
}
