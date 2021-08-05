using Mosa.External.x86.Driver;
using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Drawing
{
    public class BGAGraphics : Graphics
    {
        public BGADriver BGADriver;

        public MemoryBlock VideoMemory;

        public uint BGADriverAddr;

        public BGAGraphics(PCIDevice device, int width, int height)
        {
            BGADriver = new BGADriver(device, (uint)width, (uint)height);
            BGADriverAddr = (uint)BGADriver.LinearFrameBuffer.Address;

            Width = width;
            Height = height;
            Bpp = (int)BGADriver.Bpp;

            CurrentDriver = "BGA";

            VideoMemory = new MemoryBlock(KernelMemory.AllocateVirtualMemory((uint)FrameSize), (uint)FrameSize);
            VideoMemoryCacheAddr = (uint)VideoMemory.Address;

            ResetLimit();
        }

        public override void Disable()
        {
            BGADriver.SetStatus(false);
        }

        public override void Enable()
        {
            BGADriver.SetStatus(true);
        }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            if (IsInBounds(X, Y))
                switch (Bpp)
                {
                    case 2:
                        VideoMemory.Write16((uint)((Width * Y + X) * Bpp), System.Drawing.Color.Convert8888RGBto565RGB(Color));
                        break;
                    case 3:
                        VideoMemory.Write24((uint)((Width * Y + X) * Bpp), Color & 0x00FFFFFF);
                        break;
                    case 4:
                        VideoMemory.Write32((uint)((Width * Y + X) * Bpp), Color);
                        break;
                }
        }

        public override uint GetPoint(int X, int Y)
        {
            if (IsInBounds(X,Y))
                switch (Bpp)
                {
                    case 4:
                        return VideoMemory.Read32((uint)((Width * Y + X) * Bpp));
                }

            return 0;
        }

        public override void Update()
        {
            ASM.MEMCPY(BGADriverAddr, VideoMemoryCacheAddr, (uint)FrameSize);
        }
    }
}
