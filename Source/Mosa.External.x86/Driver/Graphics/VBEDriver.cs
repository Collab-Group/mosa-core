using Mosa.Kernel.x86;

namespace Mosa.External.x86.Driver
{
    public unsafe class VBEDriver
    {
		public MemoryBlock VideoMemory;

		public uint ScreenWidth
		{
			get
			{
				return VBE.VBEModeInfo->ScreenWidth;
			}
		}
		public uint ScreenHeight
		{
			get
			{
				return VBE.VBEModeInfo->ScreenHeight;
			}
		}

		public VBEDriver()
        {
			VideoMemory = Memory.GetPhysicalMemory((Runtime.Pointer)VBE.VBEModeInfo->PhysBase, (uint)(VBE.VBEModeInfo->ScreenWidth * VBE.VBEModeInfo->ScreenHeight * (VBE.VBEModeInfo->BitsPerPixel / 8)));
        }
	}
}
