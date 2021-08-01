using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime;

namespace Mosa.External.x86.Driver
{
    //https://wiki.osdev.org/RTL8139
    public unsafe class RTL8139 : Network
    {
        public uint IOBase;
        public byte* ReceiveBuffer;

        public RTL8139()
        {
            PCIDevice device = PCI.GetDevice(VendorID.Realtek, DeviceID.RTL8139);

            if (device == null)
            {
                Console.WriteLine("RTL8139 PCI device not found.");
                return;
            }

            device.EnableDevice();

            IOBase = device.BAR0;

            MaxPacketSize = 1792;

            // Maybe enable PCI Bus Mastering?

            // Turn on the device
            IOPort.Out8((ushort)(IOBase + 0x52), 0x0);

            // Software reset
            IOPort.Out8((ushort)(IOBase + 0x37), 0x10);

            // Wait until the reset is complete
            while ((IOPort.In8((ushort)(IOBase + 0x37)) & 0x10) != 0);

            // Initialize the receive buffer
            ReceiveBuffer = (byte*)GC.AllocateObject(8192 + 16 + 1500);

            // Write the *physical* memory location of receive buffer to RBSTART
            IOPort.Out32((ushort)(IOBase + 0x30), (uint)ReceiveBuffer);

            // Set the TOK and ROK bits high
            IOPort.Out16((ushort)(IOBase + 0x3C), 0x0005);

            // Configure the receive buffer
            IOPort.Out32((ushort)(IOBase + 0x44), 0xf | (1 << 7)); // (1 << 7) is the WRAP bit, 0xf is AB+AM+APM+AAP

            // Enable the receiver and transmitter
            IOPort.Out8((ushort)(IOBase + 0x37), 0x0C);

            Console.WriteLine("Successfully initialized the RTL8139 PCI device!");
        }

        public override void OnInterrupt()
        {
            Panic.Error("RTL8139 OnInterrupt() not implemented yet.");
        }

        public override unsafe bool SendPacket(byte* buffer, uint length)
        {
            Panic.Error("RTL8139 SendPacket() not implemented yet.");
            return false;
        }
    }
}
