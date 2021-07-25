using Mosa.External.x86.Drawing.Fonts;

namespace Mosa.External.x86.Drawing
{
    public abstract class Graphics
    {
        public int Width;
        public int Height;
        public const int Bpp = 4;
        public int FrameSize
        {
            get { return Width * Height * Bpp; }
        }

        public int LimitX;
        public int LimitY;
        public int LimitWidth;
        public int LimitHeight;

        public string CurrentDriver;

        public virtual void DrawBitFontString(string FontName, uint color, string Text, int X, int Y, int Devide = 0, bool DisableAntiAliasing = false)
        {
            BitFontDescriptor bitFontDescriptor = new BitFontDescriptor();

            foreach (var v in BitFont.RegisteredBitFont)
                if (v.Name == FontName)
                    bitFontDescriptor = v;

            string[] Lines = Text.Split('\n');
            for (int l = 0; l < Lines.Length; l++)
            {
                int UsedX = 0;
                for (int i = 0; i < Lines[l].Length; i++)
                {
                    char c = Lines[l][i];
                    UsedX += BitFont.DrawBitFontChar(this, bitFontDescriptor.Raw, bitFontDescriptor.Size, Color.FromArgb((int)color), bitFontDescriptor.Charset.IndexOf(c), UsedX + X, Y + bitFontDescriptor.Size * l, !DisableAntiAliasing) + 2 + Devide;
                }
            }
        }

        public virtual void DrawFilledRectangle(uint Color, int X, int Y, int Width, int Height)
        {
            for (int h = 0; h < Height; h++)
                for (int w = 0; w < Width; w++)
                    DrawPoint(Color, X + w, Y + h);
        }

        public virtual void DrawRectangle(uint Color, int X, int Y, int Width, int Height, int Weight)
        {
            DrawFilledRectangle(Color, X, Y, Width, Weight);

            DrawFilledRectangle(Color, X, Y, Weight, Height);
            DrawFilledRectangle(Color, X + (Width - Weight), Y, Weight, Height);

            DrawFilledRectangle(Color, X, Y + (Height - Weight), Width, Weight);
        }

        public abstract void DrawPoint(uint Color, int X, int Y);

        public abstract uint GetPoint(int X, int Y);

        public abstract void Update();

        public virtual void Clear(uint Color)
        {
            DrawFilledRectangle(Color, 0, 0, Width, Height);
        }
        public abstract void Disable();

        public virtual void DrawImage(Image image, int X, int Y, int TransparentColor)
        {
            for (int h = 0; h < image.Height; h++)
                for (int w = 0; w < image.Width; w++)
                    if (image.RawData[image.Width * h + w] != TransparentColor)
                        DrawPoint((uint)image.RawData[image.Width * h + w], X + w, Y + h);
        }

        public virtual void DrawImage(Image image, int X, int Y, bool DrawWithAlpha)
        {
            for (int h = 0; h < image.Height; h++)
                for (int w = 0; w < image.Width; w++)
                    if (DrawWithAlpha)
                    {
                        Color foreground = Color.FromArgb(image.RawData[image.Width * h + w]);
                        Color background = Color.FromArgb((int)GetPoint(X + w, Y + h));

                        int alpha = foreground.GetAlpha();
                        int inv_alpha = 255 - alpha;

                        byte newR = (byte)(((foreground.GetRed() * alpha + inv_alpha * background.GetRed()) >> 8) & 0xFF);
                        byte newG = (byte)(((foreground.GetGreen() * alpha + inv_alpha * background.GetGreen()) >> 8) & 0xFF);
                        byte newB = (byte)(((foreground.GetBlue() * alpha + inv_alpha * background.GetBlue()) >> 8) & 0xFF);

                        DrawPoint((uint)Color.ToArgb(newR, newG, newB), X + w, Y + h);
                    }
                    else DrawPoint((uint)image.RawData[image.Width * h + w], X + w, Y + h);
        }

        public void SetLimit(int X, int Y, int Width, int Height)
        {
            LimitX = X;
            LimitY = Y;
            LimitWidth = Width;
            LimitHeight = Height;
        }
        public void ResetLimit()
        {
            LimitX = 0;
            LimitY = 0;
            LimitWidth = Width;
            LimitHeight = Height;
        }
    }
}
