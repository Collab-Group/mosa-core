using Mosa.External.x86.Driver;
using Mosa.Kernel;
using Mosa.Kernel.x86;
using System.Collections.Generic;

namespace Mosa.External.x86.Networking
{
    public class NetworkManager
    {
        public static List<byte[]> AwaitingBuffers;

        public static int BufferSize;

        public static Network Initialize()
        {
            AwaitingBuffers = new List<byte[]>();

            // RTL8139 first (because it's generally available on real hardware more often than other devices like PCNETII)
            if (PCI.Exists(VendorID.Realtek, DeviceID.RTL8139))
            {
                RTL8139 device = new RTL8139();
                BufferSize = device.MaxPacketSize;
                return device;
            }

            if (PCI.Exists(VendorID.AMD, DeviceID.PCNETII))
            {
                PCNETII device = new PCNETII();
                BufferSize = device.MaxPacketSize;
                return device;
            }

            Panic.Error("No network device is available for the current system.");
            return null;
        }

        public unsafe static byte[] BytePointer2ByteArray(byte* p, ushort size)
        {
            byte[] b = new byte[size];

            for (uint i = 0; i < size; i++)
                b[i] = p[i];

            return b;
        }
    }
}
