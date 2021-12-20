// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime;
using System.Runtime.InteropServices;

namespace Mosa.Kernel.x86
{
    /// <summary>
    /// Static
    /// </summary>
    public static unsafe class VBE
    {
        #region VBEModeInfoOffset

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VBEInfo
        {
            public ushort Attributes;
            public byte WindowA;
            public byte WindowB;
            public ushort Granularity;
            public ushort WindowSize;
            public ushort SegmentA;
            public ushort SegmentB;
            public uint WinFuncPtr;
            public ushort Pitch;
            public ushort ScreenWidth;
            public ushort ScreenHeight;
            public byte WChar;
            public byte YChar;
            public byte Planes;
            public byte BitsPerPixel;
            public byte Banks;
            public byte MemoryModel;
            public byte BankSize;
            public byte ImagePages;
            public byte Reserved0;
            public byte RedMask;
            public byte RedPosition;
            public byte GreenMask;
            public byte GreenPosition;
            public byte BlueMask;
            public byte BluePosition;
            public byte ReservedMask;
            public byte ReservedPosition;
            public byte DirectColorAttributes;
            public uint PhysBase;
            public uint OffScreenMemoryOff;
            public ushort OffScreenMemorySize;
        }

        #endregion VBEModeInfoOffset

        /// <summary>
        /// Gets a value indicating whether VBE is available.
        /// </summary>
        public static bool IsVBEAvailable => Multiboot.IsMultibootAvailable && VBEModeInfo != null;

        public static VBEInfo* VBEModeInfo => (VBEInfo*)Multiboot.VBEModeInfo;
    }
}
