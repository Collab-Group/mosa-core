using System.Drawing;

namespace Mosa.External.x86.Drawing
{
    public class VirtualGraphics : Graphics
    {
        public Image bitmap;

        public VirtualGraphics(int Width, int Height)
        {
            bitmap = new Image(Width, Height);

            Bpp = 4;

            this.Width = Width;
            this.Height = Height;

            CurrentDriver = "Virtual Graphics";

            VideoMemoryCacheAddr = (uint)bitmap.RawData.Address;

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
                bitmap.RawData.Write32((uint)((Width * Y + X) * Bpp), Color);
            }
        }

        public override uint GetPoint(int X, int Y)
        {
            return bitmap.RawData.Read32((uint)((Width * Y + X) * Bpp));
        }

        public override void Update()
        {
        }
    }
}
