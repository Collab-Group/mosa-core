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

            bitFontDescriptor.Dispose();
        }

        private const int FontAlpha = 96;
        private static bool AtEdge = false;

        public static int DrawBitFontChar(Graphics graphics, byte[] Raw, int Size, int Size8, uint Color, int Index, int X, int Y, bool Calculate = false, bool AntiAliasing = true)
        {
            if (Index < 0)
                return Size / 2;

            int MaxX = 0;
            int SizePerFont = Size * Size8 * Index;
            AtEdge = false;

            for (int h = 0; h < Size; h++)
                for (int aw = 0; aw < Size8; aw++)
                    for (int ww = 0; ww < 8; ww++)
                        if ((Raw[SizePerFont + (h * Size8) + aw] & (0x80 >> ww)) != 0)
                        {
                            int max = (aw * 8) + ww;

                            int x = X + max;
                            int y = Y + h;

                            if (!Calculate)
                            {
                                graphics.DrawPoint(Color, x, y);

                                if (AntiAliasing && AtEdge)
                                {
                                    int tx = X + (aw * 8) + ww - 1;
                                    int ty = Y + h;
                                    Color ac = System.Drawing.Color.FromArgb((int)graphics.GetPoint(tx, ty));
                                    ac.R = (byte)((((((byte)((Color >> 16) & 0xFF)) * FontAlpha) + ((255 - FontAlpha) * ac.R)) >> 8) & 0xFF);
                                    ac.G = (byte)((((((byte)((Color >> 8) & 0xFF)) * FontAlpha) + ((255 - FontAlpha) * ac.G)) >> 8) & 0xFF);
                                    ac.B = (byte)((((((byte)((Color) & 0xFF)) * FontAlpha) + ((255 - FontAlpha) * ac.B)) >> 8) & 0xFF);
                                    graphics.DrawPoint((uint)ac.ToArgb(), tx, ty);
                                    ac.Dispose();
                                }

                                AtEdge = false;
                            }

                            if (max > MaxX)
                                MaxX = max;
                        }
                        else
                        {
                            AtEdge = true;
                        }
            return MaxX;
        }

        public static int Calculate(string FontName, string s)
        {
            for (int i = 0; i < RegisteredBitFont.Count; i++)
            {
                BitFontDescriptor v = RegisteredBitFont[i];
                int Size8 = v.Size / 8;

                if (v.Name == FontName)
                {
                    int r = 0;
                    for (int i1 = 0; i1 < s.Length; i1++)
                    {
                        char j = s[i1];
                        r += DrawBitFontChar(null, v.Raw, v.Size, Size8, 0, v.Charset.IndexOf(j), 0, 0, true);
                    }

                    return r;
                }

                v.Dispose();
            }
            return 0;
        }

        //TotalX will be the last line of it used.
        public static int DrawBitFontString(this Graphics graphics, string FontName, uint color, string Text, int X, int Y, bool AntiAlising = true, int Divide = 0)
        {
            BitFontDescriptor bitFontDescriptor = new BitFontDescriptor();

            for (int i1 = 0; i1 < BitFont.RegisteredBitFont.Count; i1++)
            {
                if (BitFont.RegisteredBitFont[i1].Name == FontName)
                    bitFontDescriptor = BitFont.RegisteredBitFont[i1];
            }

            int Size = bitFontDescriptor.Size;
            int Size8 = Size / 8;

            int TotalX = 0;
            string[] Lines = Text.Split('\n');

            for (int l = 0; l < Lines.Length; l++)
            {
                int UsedX = 0;
                for (int i = 0; i < Lines[l].Length; i++)
                {
                    char c = Lines[l][i];
                    UsedX += BitFont.DrawBitFontChar(graphics, bitFontDescriptor.Raw, Size, Size8, color, bitFontDescriptor.Charset.IndexOf(c), UsedX + X, Y + bitFontDescriptor.Size * l, false, AntiAlising) + 2 + Divide;
                }
                TotalX += UsedX;
            }

            bitFontDescriptor.Dispose();

            return TotalX;
        }
    }
}
