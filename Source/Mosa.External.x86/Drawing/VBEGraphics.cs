﻿using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Drawing
{
    public class VBEGraphics : Graphics
    {
        private readonly VBEDriver vBEDriver;
        private readonly MemoryBlock memoryBlock;
        private readonly uint vbeDriverAddr;

        public VBEGraphics()
        {
            vBEDriver = new VBEDriver();

            Bpp = VBE.BitsPerPixel / 8;

            vbeDriverAddr = (uint)vBEDriver.VideoMemory.Address;

            Width = (int)vBEDriver.ScreenWidth;
            Height = (int)vBEDriver.ScreenHeight;

            CurrentDriver = "VBE";

			memoryBlock = new MemoryBlock(KernelMemory.AllocateVirtualMemory((uint)FrameSize), (uint)FrameSize);
            VideoMemoryCacheAddr = (uint)memoryBlock.Address;

            ResetLimit();
        }

        public override void Disable() { }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            if (X >= LimitX && X < LimitX + LimitWidth && Y >= LimitY && Y < LimitY + LimitHeight)
                switch (Bpp)
                {
                    case 2:
                        memoryBlock.Write16((uint)((Width * Y + X) * Bpp), System.Drawing.Color.Convert8888RGBto565RGB(Color));
                        break;
                    case 3:
                        memoryBlock.Write24((uint)((Width * Y + X) * Bpp), Color & 0x00FFFFFF);
                        break;
                    case 4:
                        memoryBlock.Write32((uint)((Width * Y + X) * Bpp), Color);
                        break;
                }
        }

		public override uint GetPoint(int X, int Y)
        {
            if (X >= LimitX && X < LimitX + LimitWidth && Y >= LimitY && Y < LimitY + LimitHeight) 
                switch (Bpp) 
                {
                    case 4:
                        return memoryBlock.Read32((uint)((Width * Y + X) * Bpp));
                }

			return 0;
		}

        public override void Update()
        {
            ASM.MEMCPY(vbeDriverAddr, VideoMemoryCacheAddr, (uint)FrameSize);
        }
    }
}
