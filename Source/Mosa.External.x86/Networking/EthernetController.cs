using Mosa.External.x86.Driver;
using Mosa.Kernel;
using Mosa.Kernel.x86;

namespace Mosa.External.x86.Networking
{
    public unsafe abstract class EthernetController
    {
        public static EthernetController Controller;

        public static void Init()
        {
            foreach (var dev in PCI.Devices)
            {
                if (Controller != null) return;

                if (dev.VendorID == (ushort)VendorID.Intel)
                {
                    if (
                        dev.DeviceID == 0x100E || //Intel® 82540EM Gigabit Ethernet Controller
                        dev.DeviceID == 0x100F || //Intel® 82545EM Gigabit Ethernet Controller
                        dev.DeviceID == 0x1004 || //Intel® 82543GC Gigabit Ethernet Controller
                        dev.DeviceID == 0x107C || //Intel® 82541PI Gigabit Ethernet Controller
                        dev.DeviceID == 0x105E || //Intel® 82571EB Gigabit Ethernet Controller
                        dev.DeviceID == 0x10D3 || //Intel® 82574L/82583L Gigabit Ethernet Controller
                        dev.DeviceID == 0x10DE || //Intel® ICH10 Intergrated Gigabit Ethernet Controller
                        dev.DeviceID == 0x10CD    //Intel® ICH10R Intergrated Gigabit Ethernet Controller
                        )
                    {
                        //Must Be Static. MOSA Compiler Got Some Problems
                        Controller = new Intel825XX(dev);
                    }
                }
            }

            if (Controller == null) Console.WriteLine("No network device is available for the current system.");
        }

        //Interface
        //Check Out Intel825XX.cs Line 66
        public abstract void Send(byte* Buffer, ushort Length);
    }
}
