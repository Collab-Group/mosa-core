using Mosa.Kernel.x86;

namespace Mosa.External.x86.Driver
{
    public class VBEDriver
    {
		public MemoryBlock VideoMemory;

		public uint ScreenWidth
		{
			get
			{
				return VBE.ScreenWidth;
			}
		}
		public uint ScreenHeight
		{
			get
			{
				return VBE.ScreenHeight;
			}
		}

		public VBEDriver()
        {
			VideoMemory = Memory.GetPhysicalMemory(VBE.MemoryPhysicalLocation, (uint)(VBE.ScreenWidth * VBE.ScreenHeight * (VBE.BitsPerPixel / 8)));
        }
	}
}
