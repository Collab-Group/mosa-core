
using Mosa.External.x86.Drawing.Fonts;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;
using System.Drawing;
using System;

namespace Mosa.External.x86.Driver
{
    public class VGA
    {
        private static ushort miscPort = 0x3c2;
        private static ushort crtcIndexPort = 0x3d4;
        private static ushort crtcDataPort = 0x3d5;
        private static ushort sequencerIndexPort = 0x3c4;
        private static ushort sequencerDataPort = 0x3c5;
        private static ushort graphicsControllerIndexPort = 0x3ce;
        private static ushort graphicsControllerDataPort = 0x3cf;
        private static ushort attributeControllerIndexPort = 0x3c0;
        private static ushort attributeControllerReadPort = 0x3c1;
        private static ushort attributeControllerWritePort = 0x3c0;
        private static ushort attributeControllerResetPort = 0x3da;

        private static MemoryBlock memoryBlock;
        public static uint VideoMemoryCacheAddr;

        public static int Width = 320;
        public static int Height = 200;

        private static unsafe void WriteRegisters(byte* registers)
        {

            // Memory
            memoryBlock = new MemoryBlock((uint)320 * 201);
            VideoMemoryCacheAddr = (uint)memoryBlock.Address;

            // Misc
            IOPort.Out8(miscPort, *registers++);

            // Sequencer
            for (byte i = 0; i < 5; i++)
            {
                IOPort.Out8(sequencerIndexPort, i);
                IOPort.Out8(sequencerDataPort, *registers++);
            }

            // Cathode Ray Tube Controller
            IOPort.Out8(crtcIndexPort, 0x03);
            IOPort.Out8(crtcDataPort, (byte)(IOPort.In8(crtcDataPort) | 0x80));
            IOPort.Out8(crtcIndexPort, 0x11);
            IOPort.Out8(crtcDataPort, (byte)(IOPort.In8(crtcDataPort) & ~0x80));

            registers[0x03] = (byte)(registers[0x03] | 0x80);
            registers[0x11] = (byte)(registers[0x11] & ~0x80);

            for (byte i = 0; i < 25; i++)
            {
                IOPort.Out8(crtcIndexPort, i);
                IOPort.Out8(crtcDataPort, *registers++);
            }

            // Graphics Controller
            for (byte i = 0; i < 9; i++)
            {
                IOPort.Out8(graphicsControllerIndexPort, i);
                IOPort.Out8(graphicsControllerDataPort, *registers++);
            }

            // Attribute Controller
            for (byte i = 0; i < 21; i++)
            {
                IOPort.In8(attributeControllerResetPort);
                IOPort.Out8(attributeControllerIndexPort, i);
                IOPort.Out8(attributeControllerWritePort, *registers++);
            }

            IOPort.In8(attributeControllerResetPort);
            IOPort.Out8(attributeControllerIndexPort, 0x20);
        }

        private static bool SupportsMode(uint width, uint height, uint colordepth)
        {
            return width == 320 && height == 200 && colordepth == 8;
        }

        private unsafe bool SetMode(uint width, uint height, uint colordepth)
        {
            if (!SupportsMode(width, height, colordepth))
                return false;

            byte[] g_320x200x256 =
            {
                /* MISC */
                    0x63,
                /* SEQ */
                    0x03, 0x01, 0x0F, 0x00, 0x0E,
                /* CRTC */
                    0x5F, 0x4F, 0x50, 0x82, 0x54, 0x80, 0xBF, 0x1F,
                    0x00, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x9C, 0x0E, 0x8F, 0x28, 0x40, 0x96, 0xB9, 0xA3,
                    0xFF,
                /* GC */
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x05, 0x0F,
                    0xFF,
                /* AC */
                    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                    0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                    0x41, 0x00, 0x0F, 0x00, 0x00
            };

            fixed (byte* p = g_320x200x256)
            {
                WriteRegisters(p);
                return true;
            }
        }

        public void Initialize()
        {
            SetMode(320, 200, 8);
        }

        public unsafe byte* GetFrameBufferSegment()
        {
            IOPort.Out8(graphicsControllerIndexPort, 0x06);
            byte segmentNumber = (byte)(IOPort.In8(graphicsControllerDataPort) & (3 << 2));
            switch (segmentNumber)
            {
                default:
                case 0 << 2: return (byte*)0x00000;
                case 1 << 2: return (byte*)0xA0000;
                case 2 << 2: return (byte*)0xB0000;
                case 3 << 2: return (byte*)0xB8000;
            }
        }

        byte GetColorIndex(byte r, byte g, byte b)
        {
            if (r == 0x00 && g == 0x00 && b == 0x00) return 0x00; // black
            if (r == 0x00 && g == 0x00 && b == 0xA8) return 0x01; // blue
            if (r == 0x00 && g == 0xA8 && b == 0x00) return 0x02; // green
            if (r == 0xA8 && g == 0x00 && b == 0x00) return 0x04; // red
            if (r == 0xFF && g == 0xFF && b == 0xFF) return 0x3F; // white
            return 0x00;
        }

        public void DrawPoint(uint x, uint y, byte r, byte g, byte b)
        {
            DrawPoint(x, y, GetColorIndex(r, g, b));
        }

        public unsafe void DrawPoint(uint x, uint y, byte colorIndex)
        {
            if (x < 0 || 320 <= x || y < 0 || 200 <= y)
                return;
            memoryBlock.Write8(320 * y + x, colorIndex);
        }

        public void DrawFilledRectangle(uint x, uint y, uint w, uint h, byte r, byte g, byte b)
        {
            for (uint Y = y; Y < y + h; Y++)
                for (uint X = x; X < x + w; X++)
                    DrawPoint(X, Y, r, g, b);
        }

        public void DrawFilledRectangle(uint x, uint y, uint w, uint h, byte colorIndex)
        {
            for (uint Y = y; Y < y + h; Y++)
                for (uint X = x; X < x + w; X++)
                    DrawPoint(X, Y, colorIndex);
        }

        public unsafe void Update()
        {
            ASM.MEMCPY((uint)(GetFrameBufferSegment() + 320), VideoMemoryCacheAddr, 64000);
        }

        public void Clear(byte r, byte g, byte b)
        {
            DrawFilledRectangle(0, 0, 320, 200, r, g, b);
        }

        public void Clear(byte colorIndex)
        {
            DrawFilledRectangle(0, 0, 320, 200, colorIndex);
        }
    }
}
