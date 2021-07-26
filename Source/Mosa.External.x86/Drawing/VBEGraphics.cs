using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;
using System;
using System.Drawing;

namespace Mosa.External.x86.Drawing
{
    public class VBEGraphics : Graphics
    {
        private readonly VBEDriver vBEDriver;
        private readonly MemoryBlock memoryBlock;
        private readonly uint memoryBlockAddr, vbeDriverAddr;

        public VBEGraphics()
        {
            vBEDriver = new VBEDriver();
            vbeDriverAddr = (uint)vBEDriver.Video_Memory.Address;

			Width = (int)vBEDriver.ScreenWidth;
            Height = (int)vBEDriver.ScreenHeight;

            CurrentDriver = "VBE";

			memoryBlock = new MemoryBlock(KernelMemory.AllocateVirtualMemory((uint)FrameSize), (uint)FrameSize);
            memoryBlockAddr = (uint)memoryBlock.Address;

			ResetLimit();
        }

        public override void Clear(uint Color)
        {
            ASM.MEMFILL(memoryBlockAddr, (uint)FrameSize, Color);
        }

        public override void Disable() { }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            if (X >= LimitX && X < LimitX + LimitWidth && Y >= LimitY && Y < LimitY + LimitHeight)
                memoryBlock.Write32((uint)((Width * Y + X) * Bpp), Color);
        }

		public override uint GetPoint(int X, int Y)
        {
            if (X >= LimitX && X < LimitX + LimitWidth && Y >= LimitY && Y < LimitY + LimitHeight)
				return memoryBlock.Read32((uint)((Width * Y + X) * Bpp));

			return 0;
		}

        public override void DrawImage(Image image, int X, int Y, bool DrawWithAlpha)
        {
            int h = 0;
            while ((h++ <= Height - Y) && h <= image.Height)
                ASM.MEMCPY(
                    (uint)(memoryBlockAddr + ((Width * (Y + h) + X) * Bpp)),
                    (uint)((uint)image.RawData.Address + (image.Width * 4 * h)),
                    (uint)Math.Clamp(image.Width * 4, 0, (Width - X) * 4)
                    );
        }

        public override void DrawFilledRectangle(uint Color, int X, int Y, int aWidth, int aHeight)
        {
            int h = 0;
            while ((h++ <= Height - Y) && h <= aHeight)
                ASM.MEMFILL(
                    (uint)(memoryBlockAddr + ((Width * (Y + h) + X) * Bpp)),
                    (uint)Math.Clamp(aWidth * 4, 0, (Width - X) * 4),
                    Color
                    );
        }

        public override void Update()
        {
            ASM.MEMCPY(vbeDriverAddr, memoryBlockAddr, (uint)FrameSize);
        }
    }
}
