﻿using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Drawing
{
    public unsafe class VBEGraphics : Graphics
    {
        private readonly VBEDriver vBEDriver;
        private readonly MemoryBlock SecondBuffer;
        private readonly MemoryBlock ThirdBuffer;
        private readonly Pointer VideoMemory;

        public VBEGraphics()
        {
            vBEDriver = new VBEDriver();

            Bpp = 4;

            VideoMemory = vBEDriver.VideoMemory.Address;

            Width = (int)vBEDriver.ScreenWidth;
            Height = (int)vBEDriver.ScreenHeight;

            CurrentDriver = "VBE";

            SecondBuffer = new MemoryBlock((uint)FrameSize);
            ThirdBuffer = new MemoryBlock((uint)FrameSize);

            VideoMemoryCacheAddr = (uint)SecondBuffer.Address;

            //Clean
            Clear(0x0);
            Update();
        }

        public override void Disable() { }

        public override void Enable() { }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            if (X < Width)
            {
                SecondBuffer.Write32((uint)((Width * Y + X) * Bpp), Color);
            }
        }

        public override uint GetPoint(int X, int Y)
        {
            SecondBuffer.Read32((uint)((Width * Y + X) * Bpp));

            return 0;
        }

        public override void Update()
        {
            if (VBE.VBEModeInfo->BitsPerPixel == 32)
            {
                for (int i = 0; i < FrameSize; i += 4)
                {
                    if (SecondBuffer.Address.Load32(i) != ThirdBuffer.Address.Load32(i))
                    {
                        VideoMemory.Store32(i, SecondBuffer.Address.Load32(i));
                    }
                }
                ASM.MEMCPY((uint)ThirdBuffer.Address, (uint)SecondBuffer.Address, (uint)FrameSize);
            }
            else if (VBE.VBEModeInfo->BitsPerPixel == 24)
            {
            }
            else if (VBE.VBEModeInfo->BitsPerPixel == 16)
            {
            }
            else if (VBE.VBEModeInfo->BitsPerPixel == 8)
            {
            }
        }
    }
}
