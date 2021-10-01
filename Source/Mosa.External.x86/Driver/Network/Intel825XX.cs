//Reference: https://www.intel.com/content/dam/doc/manual/pci-pci-x-family-gbe-controllers-software-dev-manual.pdf

using Mosa.External.x86;
using Mosa.External.x86.Networking;
using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using System.Runtime.InteropServices;
using static Mosa.External.x86.MMIO;

namespace Mosa.External.x86.Driver
{
    public unsafe class Intel825XX : EthernetController
    {
        #region Statics
        public static uint BAR0;
        public static uint RXDescs;
        public static uint TXDescs;

        public static bool FullDuplex
        {
            get
            {
                return (ReadRegister(8) & (1 << 0)) != 0;
            }
        }
        public static int Speed
        {
            get
            {
                if ((ReadRegister(8) & (3 << 6)) == 0)
                {
                    return 10;
                }
                if ((ReadRegister(8) & (2 << 6)) != 0)
                {
                    return 1000;
                }
                if ((ReadRegister(8) & (1 << 6)) != 0)
                {
                    return 100;
                }
                return 0;
            }
        }

        public static void Init(PCIDevice device)
        {
            Console.WriteLine("Intel 825XX Series Ethernet Controller Exist");
            device.EnableDevice();

            BAR0 = (uint)(device.BAR0 & (~3));
            //Map(BAR0, 0x10000);
            Console.WriteLine($"BAR0:{BAR0.ToString("x2")}");

            WriteRegister(0x14, 0x1);
            bool HasEEPROM = false;
            for (int i = 0; i < 1024; i++)
            {
                if ((ReadRegister(0x14) & 0x10) != 0)
                {
                    HasEEPROM = true;
                    break;
                }
            }

            //Must be set
            if (!HasEEPROM)
            {
                Ethernet.MACAddress[0] = In8(BAR0 + 0x5400);
                Ethernet.MACAddress[1] = In8(BAR0 + 0x5401);
                Ethernet.MACAddress[2] = In8(BAR0 + 0x5402);
                Ethernet.MACAddress[3] = In8(BAR0 + 0x5403);
                Ethernet.MACAddress[4] = In8(BAR0 + 0x5404);
                Ethernet.MACAddress[5] = In8(BAR0 + 0x5405);
                Console.WriteLine("This controller has no EEPROM");
            }
            else
            {
                Ethernet.MACAddress[0] = (byte)(ReadROM(0) & 0xFF);
                Ethernet.MACAddress[1] = (byte)(ReadROM(0) >> 8);
                Ethernet.MACAddress[2] = (byte)(ReadROM(1) & 0xFF);
                Ethernet.MACAddress[3] = (byte)(ReadROM(1) >> 8);
                Ethernet.MACAddress[4] = (byte)(ReadROM(2) & 0xFF);
                Ethernet.MACAddress[5] = (byte)(ReadROM(2) >> 8);
                Console.WriteLine("EEPROM on this controller");
            }

            Console.Write("MAC Address: ");
            for (int i = 0; i < 6; i++) Console.Write($"{Ethernet.MACAddress[i].ToString("x2").PadLeft(2, '0')}{((i == 5) ? "" : ":")}");
            Console.WriteLine();

            Linkup();
            for (int i = 0; i < 0x80; i++)
                WriteRegister((ushort)(0x5200 + i * 4), 0);

            Console.WriteLine($"IRQ:{device.InterruptLine}");

            RXInit();
            TXInit();

            PIC.ClearMask((byte)(0x20 + device.InterruptLine));
            IDT.INTs.Add(new IDT.INT(0x20u + device.InterruptLine, OnInterrupt));
            /*
            Scheduler.CreateThread(new System.Threading.ThreadStart(() =>
            {
                for (; ; ) OnInterrupt();
            }), PageFrameAllocator.PageSize);
            */

            WriteRegister(0x00D0, 0x1F6DC);
            WriteRegister(0x00D0, 0xFF & ~4);
            ReadRegister(0xC0);

            Console.WriteLine($"Speed:{Speed}M/s FullDuplex:{FullDuplex}"); ;
            Console.WriteLine("Configuration Done");
        }

        private static void TXInit()
        {
            TXDescs = (uint)GC.AllocateObject(8 * 16);

            for (int i = 0; i < 8; i++)
            {
                TXDesc* desc = (TXDesc*)(TXDescs + (i * 16));
                desc->addr = 0;
                desc->cmd = 0;
            }

            WriteRegister(0x3800, TXDescs);
            WriteRegister(0x3804, 0);
            WriteRegister(0x3808, 8 * 16);
            WriteRegister(0x3810, 0);
            WriteRegister(0x3818, 0);

            WriteRegister(0x0400, (1 << 1) | (1 << 3));
        }

        public static uint RXCurr = 0;
        public static uint TXCurr = 0;

        private static void RXInit()
        {
            RXDescs = (uint)GC.AllocateObject(32 * 16);

            for (uint i = 0; i < 32; i++)
            {
                RXDesc* desc = (RXDesc*)(RXDescs + (i * 16));
                desc->addr = (ulong)(void*)GC.AllocateObject(2048 + 16);
                desc->status = 0;
            }

            WriteRegister(0x2800, RXDescs);
            WriteRegister(0x2804, 0);

            WriteRegister(0x2808, 32 * 16);
            WriteRegister(0x2810, 0);
            WriteRegister(0x2818, 32 - 1);

            WriteRegister(0x0100,
                     (1 << 1) |
                     (1 << 2) |
                     (1 << 3) |
                     (1 << 4) |
                     (0 << 6) |
                     (0 << 8) |
                    (1 << 15) |
                    (1 << 26) |
                    (0 << 16)
                );
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RXDesc
        {
            public ulong addr;
            public ushort length;
            public ushort checksum;
            public byte status;
            public byte errors;
            public ushort special;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TXDesc
        {
            public ulong addr;
            public ushort length;
            public byte cso;
            public byte cmd;
            public byte status;
            public byte css;
            public ushort special;
        }

        public static void WriteRegister(ushort Reg, uint Val)
        {
            Out32(BAR0 + Reg, Val);
        }

        public static uint ReadRegister(ushort Reg)
        {
            return In32(BAR0 + Reg);
        }

        public static ushort ReadROM(uint Addr)
        {
            uint Temp;
            WriteRegister(0x14, 1 | (Addr << 8));
            while (((Temp = ReadRegister(0x14)) & 0x10) == 0) ;
            return ((ushort)((Temp >> 16) & 0xFFFF));
        }

        private static void OnInterrupt()
        {
            uint Status = ReadRegister(0xC0);

            if ((Status & 0x04) != 0)
            {
                //Console.WriteLine("Linking Up");
                Linkup();
            }
            if ((Status & 0x10) != 0)
            {
                //Console.WriteLine("Good Threshold");
            }

            if ((Status & 0x80) != 0)
            {
                //Console.WriteLine("Packet Received");
                uint _RXCurr = RXCurr;
                RXDesc* desc = (RXDesc*)(RXDescs + (RXCurr * 16));
                while ((desc->status & 0x1) != 0)
                {
                    Ethernet.HandlePacket((byte*)desc->addr, desc->length);
                    //desc->addr;
                    desc->status = 0;
                    RXCurr = (RXCurr + 1) % 32;
                    WriteRegister(0x2818, _RXCurr);
                }
            }
        }

        private static void Linkup()
        {
            WriteRegister(0, ReadRegister(0) | 0x40);
        }
        #endregion

        public Intel825XX(PCIDevice device)
        {
            Init(device);
        }

        public override void Send(byte* Buffer, ushort Length)
        {
            TXDesc* desc = (TXDesc*)(TXDescs + (TXCurr * 16));
            desc->addr = (ulong)Buffer;
            desc->length = Length;
            desc->cmd = (1 << 0) | (1 << 1) | (1 << 3);
            desc->status = 0;

            byte _TXCurr = (byte)TXCurr;
            TXCurr = (TXCurr + 1) % 8;
            WriteRegister(0x3818, TXCurr);
            while ((desc->status & 0xff) == 0) ;
        }
    }
}
