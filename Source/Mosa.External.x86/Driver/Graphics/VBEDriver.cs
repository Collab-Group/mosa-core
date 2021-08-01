﻿using Mosa.External.x86;
using Mosa.Kernel.x86;
using Mosa.Runtime;

namespace Mosa.External.x86.Driver
{
    public class VBEDriver
    {
		public MemoryBlock Video_Memory;
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
			Video_Memory = Memory.GetPhysicalMemory(VBE.MemoryPhysicalLocation, (uint)(VBE.ScreenWidth * VBE.ScreenHeight * (VBE.BitsPerPixel / 8)));
        }
	}
}
