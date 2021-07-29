using Mosa.Runtime;

//namespace MOSA2
namespace Mosa.External.x86
{
    //Memory Mapped IO

    public static class MMIO
    {
        public static byte In8(uint address)
        {
            return Intrinsic.Load8((Pointer)address);
        }

        public static void Out8(uint address, byte value)
        {
            Intrinsic.Store8((Pointer)address, value);
        }

        public static ushort In16(uint address)
        {
            return Intrinsic.Load16((Pointer)address);
        }

        public static void Out16(uint address, ushort value)
        {
            Intrinsic.Store16((Pointer)address, value);
        }

        public static uint In32(uint address)
        {
            return Intrinsic.Load32((Pointer)address);
        }

        public static void Out32(uint address, uint value)
        {
            Intrinsic.Store32((Pointer)address, value);
        }

        public static ulong In64(uint address)
        {
            return Intrinsic.Load64((Pointer)address);
        }

        public static void Out64(uint address, ulong value)
        {
            Intrinsic.Store64((Pointer)address, value);
        }
    }
}
