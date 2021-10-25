using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Drawing
{
    public unsafe class VBEGraphics : Graphics
    {
        private readonly VBEDriver vBEDriver;
        private readonly MemoryBlock memoryBlock;
        private readonly MemoryBlock memoryBlock_Cache;
        private readonly Pointer vbeDriverAddr;

        public VBEGraphics()
        {
            vBEDriver = new VBEDriver();

            Bpp = 4;

            vbeDriverAddr = vBEDriver.VideoMemory.Address;

            Width = (int)vBEDriver.ScreenWidth;
            Height = (int)vBEDriver.ScreenHeight;

            CurrentDriver = "VBE";

            memoryBlock = new MemoryBlock((uint)FrameSize);
            memoryBlock_Cache = new MemoryBlock((uint)FrameSize);

            VideoMemoryCacheAddr = (uint)memoryBlock.Address;

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
                memoryBlock.Write32((uint)((Width * Y + X) * Bpp), Color);
            }
        }

        public override uint GetPoint(int X, int Y)
        {
            memoryBlock.Read32((uint)((Width * Y + X) * Bpp));

            return 0;
        }

        public override void Update()
        {
            if (VBE.VBEModeInfo->BitsPerPixel == 32)
            {
                for (int i = 0; i < FrameSize; i += 4)
                {
                    if (memoryBlock.Address.Load32(i) != memoryBlock_Cache.Address.Load32(i))
                    {
                        vbeDriverAddr.Store32(i, memoryBlock.Address.Load32(i));
                    }
                }
                ASM.MEMCPY((uint)memoryBlock_Cache.Address, (uint)memoryBlock.Address, (uint)FrameSize);
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
