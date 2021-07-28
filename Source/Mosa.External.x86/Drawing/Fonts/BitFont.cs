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

		public static int DrawBitFontChar(Graphics graphics, byte[] Raw, int Size, uint Color, int Index, int X, int Y, bool UseAntiAliasing)
		{
			if (Index == -1)
				return Size / 2;

			int MaxX = 0;
			bool LastPixelIsNotDrawn = false;
			int Size8 = Size / 8;
			int SizePerFont = Size * Size8 * Index;

			byte Red = (byte)((Color >> 16) & 0xFF);

			for (int h = 0; h < Size; h++)
				for (int aw = 0; aw < Size8; aw++)
					for (int ww = 0; ww < 8; ww++)
						if ((Raw[SizePerFont + (h * Size8) + aw] & (0x80 >> ww)) != 0)
						{
							int x = X + (aw * 8) + ww;
							int y = Y + h;

							graphics.DrawPoint(Color, x, y);

							if (x > MaxX)
								MaxX = x;

							if (LastPixelIsNotDrawn)
							{
								if (UseAntiAliasing)
								{
									Color ac = System.Drawing.Color.FromArgb((int)graphics.GetPoint(x - 1, y));
									byte r = (byte)(((Red * 127 + 127 * ac.GetRed()) >> 8) & 0xFF);
									byte g = (byte)(((Red * 127 + 127 * ac.GetGreen()) >> 8) & 0xFF);
									byte b = (byte)(((Red * 127 + 127 * ac.GetBlue()) >> 8) & 0xFF);
									graphics.DrawPoint((uint)System.Drawing.Color.ToArgb(ac.GetAlpha(), r, g, b), x - 1, y);
								}

								LastPixelIsNotDrawn = false;
							}
						} else LastPixelIsNotDrawn = true;

			return MaxX;
		}
	}
}
