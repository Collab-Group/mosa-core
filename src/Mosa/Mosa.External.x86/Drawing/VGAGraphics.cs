
using Mosa.External.x86.Driver;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Drawing
{
    public class VGAGraphics : Graphics
    {
        private VGA vga;
        public VGAGraphics()
        {
            vga = new VGA();
            vga.Initialize();
            Width = 320;
            Height = 200;

            CurrentDriver = "VGA";

            VideoMemoryCacheAddr = VGA.VideoMemoryCacheAddr;
            Clear(0x00);
        }

        public override void Disable() { }

        public override void Enable() { }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            vga.DrawPoint((uint)X, (uint)Y, (byte)Color);
        }

        public override uint GetPoint(int X, int Y)
        {
            throw new System.NotImplementedException();
        }

        public override unsafe void Update()
        {
            ASM.MEMCPY((uint)(vga.GetFrameBufferSegment() + 320), VideoMemoryCacheAddr, 64000);
        }
    }
}
