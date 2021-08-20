using Mosa.Runtime;
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

        public static int DrawBitFontChar(Graphics graphics, byte[] Raw, int Size, uint color, int Index, int X, int Y, bool UseAntiAliasing, bool Calculate = false)
        {
            Color Color = System.Drawing.Color.FromArgb((int)color);
            if (Index == -1) return Size / 2;

            int MaxX = 0;

            bool LastPixelIsNotDrawn = false;

            int SizePerFont = Size * (Size / 8);
            byte[] Font = new byte[SizePerFont];

            for (uint u = 0; u < SizePerFont; u++)
            {
                Font[u] = Raw[(SizePerFont * Index) + u];
            }

            for (int h = 0; h < Size; h++)
            {
                for (int aw = 0; aw < Size / 8; aw++)
                {

                    for (int ww = 0; ww < 8; ww++)
                    {
                        if ((Font[(h * (Size / 8)) + aw] & (0x80 >> ww)) != 0)
                        {
                            if (!Calculate)
                                graphics.DrawPoint((uint)Color.ToArgb(), X + (aw * 8) + ww, Y + h);

                            if ((aw * 8) + ww > MaxX)
                            {
                                MaxX = (aw * 8) + ww;
                            }

                            if (LastPixelIsNotDrawn)
                            {
                                if (UseAntiAliasing && !Calculate)
                                {
                                    int tx = X + (aw * 8) + ww - 1;
                                    int ty = Y + h;
                                    Color ac = Color.FromArgb((int)graphics.GetPoint(tx, ty));
                                    byte r = (byte)(((Color.GetRed() * 127 + 127 * ac.GetRed()) >> 8) & 0xFF);
                                    byte g = (byte)(((Color.GetRed() * 127 + 127 * ac.GetGreen()) >> 8) & 0xFF);
                                    byte b = (byte)(((Color.GetRed() * 127 + 127 * ac.GetBlue()) >> 8) & 0xFF);
                                    graphics.DrawPoint((uint)Color.ToArgb(ac.GetAlpha(), r, g, b), tx, ty);
                                }

                                LastPixelIsNotDrawn = false;
                            }
                        }
                        else
                        {
                            LastPixelIsNotDrawn = true;
                        }
                    }
                }
            }

            GC.DisposeObject(Font);

            return MaxX;
        }

        public static int Calculate(string FontName, string s)
        {
            foreach (var v in BitFont.RegisteredBitFont)
                if (v.Name == FontName)
                {
                    int r = 0;
                    foreach (var j in s)
                    {
                        r += DrawBitFontChar(null, v.Raw, v.Size, 0, v.Charset.IndexOf(j), 0, 0, false, true);
                    }
                    return r;
                }
            return 0;
        }
    }
}
