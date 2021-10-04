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

        public static int DrawBitFontChar(Graphics graphics, byte[] Raw, int Size, int Size8, uint Color, int Index, int X, int Y, bool Calculate = false)
        {
            if (Index < 0)
                return Size / 2;

            int MaxX = 0;
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
                        }
            return MaxX;
        }

        public static int Calculate(string FontName, string s)
        {
            foreach (var v in RegisteredBitFont)
            {
                int Size8 = v.Size / 8;

                if (v.Name == FontName)
                {
                    int r = 0;
                    foreach (var j in s)
                        r += DrawBitFontChar(null, v.Raw, v.Size, Size8, 0, v.Charset.IndexOf(j), 0, 0, true);

                    return r;
                }
            }
            return 0;
        }
    }
}
