using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime;

namespace Mosa.External.x86.Driver
{
    public enum VBERegister
    {
        VBE_DISPI_DISABLED = 0x00,
        VBE_DISPI_ENABLED = 0x01,

        VBE_DISPI_INDEX_ID = 0,
        VBE_DISPI_INDEX_XRES,
        VBE_DISPI_INDEX_YRES,
        VBE_DISPI_INDEX_BPP,
        VBE_DISPI_INDEX_ENABLE,
        VBE_DISPI_INDEX_BANK,
        VBE_DISPI_INDEX_VIRT_WIDTH,
        VBE_DISPI_INDEX_VIRT_HEIGHT,
        VBE_DISPI_INDEX_X_OFFSET,
        VBE_DISPI_INDEX_Y_OFFSET,

        VBE_DISPI_IOPORT_INDEX = 0x01CE,
        VBE_DISPI_IOPORT_DATA = 0x01CF,

        VBE_DISPI_ID5 = 0xB0C5,

        VBE_DISPI_BPP_4 = 0x04,
        VBE_DISPI_BPP_8 = 0x08,
        VBE_DISPI_BPP_15 = 0x0F,
        VBE_DISPI_BPP_16 = 0x10,
        VBE_DISPI_BPP_24 = 0x18,
        VBE_DISPI_BPP_32 = 0x20,

        VBE_DISPI_LFB_ENABLED = 0x40,

        VBE_DISPI_NOCLEARMEM = 0x80,

        VBE_DISPI_BANK_ADDRESS = 0xA0000
    }

    public class BGADriver
    {
        public MemoryBlock LinearFrameBuffer;

        public uint Width, Height, Bpp;

        public ushort BGADepth;

        public BGADriver(PCIDevice device, uint width, uint height, uint depth = 32)
        {
            if (device == null)
                Panic.Error("BGADriver PCIDevice is null.");

            Width = width;
            Height = height;
            Bpp = depth / 8;

            if (depth == 32)
                BGADepth = (ushort)VBERegister.VBE_DISPI_BPP_32;
            else if (depth == 24)
                BGADepth = (ushort)VBERegister.VBE_DISPI_BPP_24;
            else if (depth == 16)
                BGADepth = (ushort)VBERegister.VBE_DISPI_BPP_16;
            else if (depth == 15)
                BGADepth = (ushort)VBERegister.VBE_DISPI_BPP_15;
            else if (depth == 8)
                BGADepth = (ushort)VBERegister.VBE_DISPI_BPP_8;
            else if (depth == 4)
                BGADepth = (ushort)VBERegister.VBE_DISPI_BPP_4;

            // Check version of BGA first
            if (ReadRegister((ushort)VBERegister.VBE_DISPI_INDEX_ID) != (ushort)VBERegister.VBE_DISPI_ID5)
            {
                Console.WriteLine("Old version of BGA detected :/");
                return;
            }

            // Set display mode
            SetMode();

            // TODO : Fix
            // Set linear frame buffer address
            LinearFrameBuffer = Memory.GetPhysicalMemory(new Pointer(0xE0000000), Width * Height * Bpp);
        }

        public void SetStatus(bool enable)
        {
            WriteRegister((ushort)VBERegister.VBE_DISPI_INDEX_ENABLE, (ushort)(enable ? ((ushort)VBERegister.VBE_DISPI_ENABLED | (ushort)VBERegister.VBE_DISPI_LFB_ENABLED) : (ushort)VBERegister.VBE_DISPI_DISABLED));
        }

        public void SetMode()
        {
            SetStatus(false);
            WriteRegister((ushort)VBERegister.VBE_DISPI_INDEX_XRES, (ushort)Width);
            WriteRegister((ushort)VBERegister.VBE_DISPI_INDEX_YRES, (ushort)Height);
            WriteRegister((ushort)VBERegister.VBE_DISPI_INDEX_BPP, BGADepth);
            SetStatus(true);
        }

        public static void WriteRegister(ushort index, ushort data)
        {
            IOPort.Out16((ushort)VBERegister.VBE_DISPI_IOPORT_INDEX, index);
            IOPort.Out16((ushort)VBERegister.VBE_DISPI_IOPORT_DATA, data);
        }

        public static ushort ReadRegister(ushort index)
        {
            IOPort.Out16((ushort)VBERegister.VBE_DISPI_IOPORT_INDEX, index);
            return IOPort.In16((ushort)VBERegister.VBE_DISPI_IOPORT_DATA);
        }
    }
}
