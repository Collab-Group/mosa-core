﻿using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Drawing
{
    public class VBEGraphics : Graphics
    {
        private readonly VBEDriver vBEDriver;
        private readonly MemoryBlock memoryBlock;

        public VBEGraphics()
        {
            vBEDriver = new VBEDriver();

			Width = (int)vBEDriver.ScreenWidth;
            Height = (int)vBEDriver.ScreenHeight;

            CurrentDriver = "VBE";

			memoryBlock = new MemoryBlock(KernelMemory.AllocateVirtualMemory((uint)FrameSize), (uint)FrameSize);

			ResetLimit();
        }

        public override void Clear(uint Color)
        {
            //memoryBlock.Fill32(0, Color, (uint)FrameSize, Bpp);

            uint Addr = (uint)memoryBlock.Address;
            ASM.MEMFILL(Addr, (uint)FrameSize, Color);
        }

        // TODO: Fix
        /*public override void DrawFilledRectangle(uint Color, int X, int Y, int Width, int Height)
        {
            for (int h = 0; h < Height; h++)
                if (X >= 0)
                {
                    int w = Width;
                    if (X + Width > this.Width)
                        w = this.Width - X;

                    memoryBlock.Fill32((uint)(this.Width * (h + Y) + X), Color, (uint)w, Bpp);
                }
                else
                {
                    if (Width + X >= 0)
                        memoryBlock.Fill32((uint)(this.Width * (h + Y)), Color, (uint)(Width + X), Bpp);
                }
        }*/

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

		public override void Update()
        {
            uint addr = vBEDriver.Video_Memory.Address.ToUInt32();
            uint bufferaddr = memoryBlock.Address.ToUInt32();

            /*for (int i = 0; i < FrameSize; i++)
            {
                byte bufferi = Native.Get8((uint)(bufferaddr + i));

                if (Native.Get8((uint)(addr + i)) != bufferi)
                    Native.Set8((uint)(addr + i), bufferi);
            }*/

            ASM.MEMCPY(addr, bufferaddr, (uint)FrameSize);
        }
    }
}
