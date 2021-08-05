using Mosa.External.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;

namespace System.Drawing
{
    public class Image
    {
        public MemoryBlock RawData;
        public int Length;
        public int Bpp;
        public int Width;
        public int Height;

        public Image ScaledImage(int NewWidth, int NewHeight)
        {
            int w1 = this.Width, h1 = this.Height;
            MemoryBlock temp = new MemoryBlock((uint)(NewWidth * NewHeight * 4));

            int x_ratio = ((w1 << 16) / NewWidth) + 1, y_ratio = ((h1 << 16) / NewHeight) + 1;
            int x2, y2;

            for (int i = 0; i < NewHeight; i++)
            {
                for (int j = 0; j < NewWidth; j++)
                {
                    x2 = ((j * x_ratio) >> 16);
                    y2 = ((i * y_ratio) >> 16);
                    temp[(uint)((i * NewWidth) + j)] = this.RawData[(uint)((y2 * w1) + x2)];
                }
            }

            Image image = new Image()
            {
                Width = NewWidth,
                Height = NewHeight,
                Bpp = 4,
                Length = Width * Height * 4,
                RawData = temp
            };

            return image;
        }

        public void Dispose() 
        {
            RawData.Free();
            GC.DisposeObject(this);
        }
    }
}
