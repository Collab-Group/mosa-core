using Mosa.External.x86.Driver;
using Mosa.Kernel;
using Mosa.Kernel.x86;
using System.Collections.Generic;

namespace Mosa.External.x86.Networking
{
    public class NetworkManager
    {
        public static List<Buffer> AwaitingBuffers;

        public const int BufferSize = 1520; // Make this depend on the driver used, but we have to make it const as it seems structs can only access the variable if it's const

        public static Network Initialize()
        {
            AwaitingBuffers = new List<Buffer>();

            // RTL8139 first (because it's generally available on real hardware more often than other devices like PCNETII)
            if (PCI.Exists(VendorID.Realtek, DeviceID.RTL8139))
            {
                RTL8139 device = new RTL8139();
                //BufferSize = device.MaxPacketSize;
                return device;
            }

            if (PCI.Exists(VendorID.AMD, DeviceID.PCNETII))
            {
                PCNETII device = new PCNETII();
                //BufferSize = device.MaxPacketSize;
                return device;
            }

            Panic.Error("No network device is available for the current system.");
            return null;
        }
    }
}
