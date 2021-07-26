using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;

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

            FrameCacheAddr = (uint)memoryBlock.Address;

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

		public override void Update()
        {
            ASM.MEMCPY(vbeDriverAddr, memoryBlockAddr, (uint)FrameSize);
        }
    }
}
