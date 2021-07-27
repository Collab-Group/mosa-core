using Mosa.External.x86.Driver;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Drawing
{
    public class VMWareSVGAIIGraphics : Graphics
    {
        private readonly VMWareSVGAII vMWareSVGAII;

        public VMWareSVGAIIGraphics(int width, int height)
        {
            vMWareSVGAII = new VMWareSVGAII();
            vMWareSVGAII.SetMode((uint)width, (uint)height);

            Width = width;
            Height = height;

            CurrentDriver = "VMWare SVGA II";

            VideoMemoryCacheAddr = (uint)((uint)vMWareSVGAII.Video_Memory.Address + FrameSize);

            ResetLimit();
        }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            if (X >= LimitX && X < LimitX + LimitWidth && Y >= LimitY && Y < LimitY + LimitHeight)
                vMWareSVGAII.Video_Memory.Write32((uint)(FrameSize + ((Width * Y + X) * Bpp)), Color);
        }

        public override uint GetPoint(int X, int Y)
        {
            if (X >= LimitX && X < LimitX + LimitWidth && Y >= LimitY && Y < LimitY + LimitHeight)
                return vMWareSVGAII.Video_Memory.Read32((uint)(FrameSize + ((Width * Y + X) * Bpp)));

            return 0;
        }

        public unsafe override void Update()
        {
            uint addr = vMWareSVGAII.Video_Memory.Address.ToUInt32();

            /*for (int i = 0; i < FrameSize; i++)
                Native.Set8((uint)(addr + i), Native.Get8((uint)(addr + FrameSize + i)));*/

            // Fast memory copy using assembly
            ASM.MEMCPY(addr, (uint)(addr + FrameSize), (uint)FrameSize);

            vMWareSVGAII.Update();
        }

        public override void Disable()
        {
            vMWareSVGAII.Disable();
        }
    }
}
