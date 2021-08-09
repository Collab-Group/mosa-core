using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System.Runtime.InteropServices;

namespace Mosa.External.x86.Driver
{
    //https://wiki.osdev.org/RTL8139

    public unsafe class RTL8139 : Network
    {
        public static uint IOBase;
        public static byte* RX;
        public static byte* TX;
        private const int RXSize = 8192 + 16 + 1500;
        private const int TXSize = 1536;
        public static byte[] MACAddress;

        public const ushort MAC1 = 0x00;
        public const ushort MAC2 = 0x04;

        private static byte[] StartRegisters;
        private static byte[] CommandRegisters;

        public RTL8139()
        {

            StartRegisters = new byte[] { 0x20, 0x24, 0x28, 0x2C };
            CommandRegisters = new byte[] { 0x10, 0x14, 0x18, 0x1C };

            PCIDevice device = PCI.GetDevice(VendorID.Realtek, DeviceID.RTL8139);

            if (device == null)
            {
                Console.WriteLine("RTL8139 PCI device not found.");
                return;
            }

            //Register It
            Console.WriteLine($"IRQ:{device.InterruptLine}");

            device.EnableDevice();

            IOBase = device.BAR0 & 0xFFFFFFFC;

            // Turn on the device
            IOPort.Out8((ushort)(IOBase + 0x52), 0x0);

            // Software reset
            IOPort.Out8((ushort)(IOBase + 0x37), 0x10);

            // Wait until the reset is complete
            while ((IOPort.In8((ushort)(IOBase + 0x37)) & 0x10) != 0) ;

            // Initialize the receive buffer
            RX = (byte*)GC.AllocateObject(RXSize);
            TX = (byte*)GC.AllocateObject(TXSize);

            // Write the *physical* memory location of receive buffer to RBSTART
            IOPort.Out32((ushort)(IOBase + 0x30), (uint)RX);

            // Set the TOK and ROK bits high
            IOPort.Out16((ushort)(IOBase + 0x3C), 0x0005);

            // Configure the receive buffer
            IOPort.Out32((ushort)(IOBase + 0x44), 0xf | (1 << 7)); // (1 << 7) is the WRAP bit, 0xf is AB+AM+APM+AAP

            // Enable the receiver and transmitter
            IOPort.Out8((ushort)(IOBase + 0x37), 0x0C);

            MACAddress = new byte[6];
            uint MACP1 = IOPort.In32((ushort)(IOBase + MAC1));
            ushort MACP2 = IOPort.In16((ushort)(IOBase + MAC2));
            MACAddress[0] = (byte)((MACP1 >> 0) & 0xFF);
            MACAddress[1] = (byte)((MACP1 >> 8) & 0xFF);
            MACAddress[2] = (byte)((MACP1 >> 16) & 0xFF);
            MACAddress[3] = (byte)((MACP1 >> 24) & 0xFF);
            MACAddress[4] = (byte)((MACP2 >> 0) & 0xFF);
            MACAddress[5] = (byte)((MACP2 >> 8) & 0xFF);

            Console.Write("MACAddress:");
            for (int i = 0; i < MACAddress.Length; i++)
            {
                Console.Write($"{MACAddress[i].ToString("x2")}{((i == MACAddress.Length - 1) ? "" : ":")}");
            }

            Console.WriteLine();

            Console.WriteLine("Successfully initialized the RTL8139 PCI device!");
        }

        public const ushort ISR = 0x3E;
        public const ushort TransmitOK = (1 << 2);
        public const ushort ReceiveOK = (1 << 0);


        //Read Inerrupt By device.InterruptLine
        public override void OnInterrupt()
        {
            ushort STS = IOPort.In16((ushort)(IOBase + ISR));
            IOPort.Out16((ushort)(IOBase + ISR), STS);

            if ((STS & TransmitOK) != 0)
            {
                Console.WriteLine("Transmit OK");
            }
            if ((STS & ReceiveOK) != 0)
            {
                Console.WriteLine("Receive OK");
                Receive();
            }
        }

        public static int RXIndex = 0;

        public const ushort CMD = 0x37;
        public const ushort RXBufPtr = 0x38;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public ushort Status;
            public ushort Size;
        }

        public void Receive()
        {
            byte* Buffer = RX;
            int Index = RXIndex;

            while ((IOPort.In8((ushort)(IOBase + CMD)) & 0x01) == 0)
            {
                int Offset = Index % RXSize;
                Header* header = ((Header*)(RX + Offset));

                byte* Data = (byte*)GC.AllocateObject(header->Size);
                Mosa.Runtime.x86.ASM.MEMCPY((uint)Data, (uint)(RX + Offset + sizeof(Header)), header->Size);
                //Do something

                Index = (int)((Index + header->Size + sizeof(Header) + 3) & 0xFFFFFFFC);

                IOPort.Out16((ushort)(IOBase + RXBufPtr), (ushort)(Index - 16));
            }

            RXIndex = Index;
        }

        private static int TXPair = 0;
        public override bool SendPacket(byte* buffer,uint length)
        {
            ASM.MEMCPY((uint)TX, (uint)buffer,length);

            IOPort.Out32(StartRegisters[TXPair], (uint)TX);
            IOPort.Out32(CommandRegisters[TXPair], length);

            TXPair++;
            if (TXPair > 3) TXPair = 0;

            return true;
        }
    }
}
