using Mosa.Kernel.x86;
using System.Drawing;
using System.Text;

namespace Mosa.External.x86.Drawing
{
    public unsafe class Bitmap : Image
    {
        struct BitmapHeader
        {
            public string Type;
            public uint Size;
            public uint DataSectionOffset;
            public uint Width;
            public uint Height;
            public uint Bpp;
        }

        public Bitmap(byte[] Data)
        {
            MemoryBlock memoryBlock = new MemoryBlock(Data);

            BitmapHeader bitmapHeader = new BitmapHeader();

            bitmapHeader.Type = "" + Encoding.ASCII.GetChar(memoryBlock.Read8(0)) + Encoding.ASCII.GetChar(memoryBlock.Read8(1));
            bitmapHeader.Size = memoryBlock.Read32(2);
            bitmapHeader.DataSectionOffset = memoryBlock.Read32(0xA);
            bitmapHeader.Width = memoryBlock.Read32(0x12);
            bitmapHeader.Height = memoryBlock.Read32(0x16);
            bitmapHeader.Bpp = memoryBlock.Read8(0x1C);

            if (bitmapHeader.Type != "BM")
            {
                Panic.Error("This is not a bitmap");
            }

            if (bitmapHeader.Bpp != 24 && bitmapHeader.Bpp != 32)
            {
                Panic.Error(bitmapHeader.Bpp + " bits bitmap is not supported");
            }

            this.Width = (int)bitmapHeader.Width;
            this.Height = (int)bitmapHeader.Height;
            this.Length = (int)(Width * Height * (bitmapHeader.Bpp / 8));
            this.Bpp = (int)bitmapHeader.Bpp;
            this.RawData = new MemoryBlock((uint)Length);


            int[] temp = new int[Width];
            uint w = 0;
            uint h = (uint)Height - 1;
            for (uint i = 0; i < this.Length * (bitmapHeader.Bpp / 8); i += (bitmapHeader.Bpp / 8))
            {
                if (w == Width)
                {
                    for (uint k = 0; k < temp.Length; k++)
                    {
                        RawData[(uint)Width * h + k] = temp[k];
                    }
                    w = 0;
                    h--;
                }
                switch (bitmapHeader.Bpp)
                {
                    case 24:
                        temp[w] = (int)(0xFF000000 | (int)memoryBlock.Read24(bitmapHeader.DataSectionOffset + i));
                        break;
                    case 32:
                        temp[w] = (int)memoryBlock.Read32(bitmapHeader.DataSectionOffset + i);
                        break;

                }
                //Console.WriteLine(Color.FromArgb(temp[w]).ToString());
                w++;
            }
            return;
        }
    }
}
