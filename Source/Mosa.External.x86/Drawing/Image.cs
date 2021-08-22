using Mosa.External.x86;
using Mosa.Runtime;

namespace System.Drawing
{
    public class Image
    {
        public MemoryBlock RawData;
        public int Bpp;
        public int Width;
        public int Height;

        public Image(int width, int height)
        {
            Width = width;
            Height = height;
            Bpp = 4;
            RawData = new MemoryBlock((uint)(width * height * Bpp));
        }

        public Image ScaleImage(int NewWidth, int NewHeight)
        {
            int w1 = Width, h1 = Height;
            MemoryBlock temp = new MemoryBlock((uint)(NewWidth * NewHeight * 4));

            int x_ratio = ((w1 << 16) / NewWidth) + 1, y_ratio = ((h1 << 16) / NewHeight) + 1;
            int x2, y2;

            for (int i = 0; i < NewHeight; i++)
            {
                for (int j = 0; j < NewWidth; j++)
                {
                    x2 = ((j * x_ratio) >> 16);
                    y2 = ((i * y_ratio) >> 16);
                    temp[(uint)((i * NewWidth) + j)] = RawData[(uint)((y2 * w1) + x2)];
                }
            }

            Image image = new Image()
            {
                Width = NewWidth,
                Height = NewHeight,
                Bpp = Bpp,
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
