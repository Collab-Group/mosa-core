using System.Collections.Generic;
using System.Drawing;

namespace Mosa.External.x86.Drawing.Fonts
{
    public struct BitFontDescriptor
    {
        public string Charset;
        public byte[] Raw;
        public int Size;
        public string Name;

        public BitFontDescriptor(string aName, string aCharset, byte[] aRaw, int aSize)
        {
            Charset = aCharset;
            Raw = aRaw;
            Size = aSize;
            Name = aName;
        }
    }

    public static class BitFont
    {
        public static List<BitFontDescriptor> RegisteredBitFont;

        public static void RegisterBitFont(BitFontDescriptor bitFontDescriptor)
        {
            // Static is not available in MOSA
            if (RegisteredBitFont == null)
                RegisteredBitFont = new List<BitFontDescriptor>();

            RegisteredBitFont.Add(bitFontDescriptor);
        }

        public static int DrawBitFontChar(Graphics graphics, byte[] Raw, int Size, int Size8, uint Color, byte RedOfColor, int Index, int X, int Y, bool UseAntiAliasing, bool Calculate = false)
        {
            if (Index < 0)
                return Size / 2;

            int MaxX = 0;
            bool LastPixelIsNotDrawn = false;
            int SizePerFont = Size * Size8 * Index;

            for (int h = 0; h < Size; h++)
                for (int aw = 0; aw < Size8; aw++)
                    for (int ww = 0; ww < 8; ww++)
                        if ((Raw[SizePerFont + (h * Size8) + aw] & (0x80 >> ww)) != 0)
                        {
                            int max = (aw * 8) + ww;

                            int x = X + max;
                            int y = Y + h;

                            if (!Calculate)
                                graphics.DrawPoint(Color, x, y);

                            if (max > MaxX)
                                MaxX = max;

                            if (LastPixelIsNotDrawn)
                            {
                                if (UseAntiAliasing && !Calculate)
                                {
                                    Color ac = System.Drawing.Color.FromArgb((int)graphics.GetPoint(x - 1, y));

                                    byte r = (byte)(((RedOfColor + (127 * ac.GetRed())) >> 8) & 0xFF);
                                    byte g = (byte)(((RedOfColor + (127 * ac.GetGreen())) >> 8) & 0xFF);
                                    byte b = (byte)(((RedOfColor + (127 * ac.GetBlue())) >> 8) & 0xFF);
                                    graphics.DrawPoint((uint)System.Drawing.Color.ToArgb(ac.GetAlpha(), r, g, b), x - 1, y);
                                }

                                LastPixelIsNotDrawn = false;
                            }
                        }
                        else LastPixelIsNotDrawn = true;

            return MaxX;
        }
    }
}
