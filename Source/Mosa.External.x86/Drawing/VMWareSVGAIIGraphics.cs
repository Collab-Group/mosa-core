using Mosa.External.x86.Driver;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Drawing
{
    public class VMWareSVGAIIGraphics : Graphics
    {
        private readonly VMWareSVGAII vMWareSVGAII;
        private readonly uint svgaAddress;

        public VMWareSVGAIIGraphics(int width, int height)
        {
            vMWareSVGAII = new VMWareSVGAII();
            vMWareSVGAII.SetMode((uint)width, (uint)height);

            Width = width;
            Height = height;

            CurrentDriver = "VMWare SVGA II";

            svgaAddress = (uint)vMWareSVGAII.Video_Memory.Address;
            VideoMemoryCacheAddr = (uint)(svgaAddress + FrameSize);

            ResetLimit();
        }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            if (IsInBounds(X, LimitX, Y, LimitY, LimitWidth, LimitHeight))
                vMWareSVGAII.Video_Memory.Write32((uint)(FrameSize + ((Width * Y + X) * Bpp)), Color);
        }

        public override uint GetPoint(int X, int Y)
        {
            if (IsInBounds(X, LimitX, Y, LimitY, LimitWidth, LimitHeight))
                return vMWareSVGAII.Video_Memory.Read32((uint)(FrameSize + ((Width * Y + X) * Bpp)));

            return 0;
        }

        public override void Update()
        {
            ASM.MEMCPY(svgaAddress, VideoMemoryCacheAddr, (uint)FrameSize);
            vMWareSVGAII.Update();
        }

        public override void Disable()
        {
            vMWareSVGAII.Disable();
        }

        public override void Enable()
        {
            vMWareSVGAII.Enable();
        }
    }
}
