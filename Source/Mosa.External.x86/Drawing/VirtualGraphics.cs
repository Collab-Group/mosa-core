using Mosa.Runtime.x86;
using System.Drawing;

namespace Mosa.External.x86.Drawing
{
    public class VirtualGraphics : Graphics
    {
        Image bitmap;

        public VirtualGraphics(int Width, int Height)
        {
            bitmap = new Image(Width, Height);

            Bpp = 4;

            this.Width = Width;
            this.Height = Height;

            CurrentDriver = "Virtual Graphics";

            VideoMemoryCacheAddr = (uint)bitmap.RawData.Address;

            ResetLimit();
        }

        public override void Disable() { }

        public override void Enable() { }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            if (IsInBounds(X, Y))
                bitmap.RawData.Write32((uint)((Width * Y + X) * Bpp), Color);
        }

        public override uint GetPoint(int X, int Y)
        {
            if (IsInBounds(X, Y))
                return bitmap.RawData.Read32((uint)((Width * Y + X) * Bpp));

            return 0;
        }

        public override void Update()
        {
        }
    }
}
