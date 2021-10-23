using Mosa.External.x86.Driver;
using Mosa.Kernel;
using Mosa.Kernel.x86;

namespace Mosa.External.x86.Networking
{
    public unsafe abstract class EthernetController
    {
        public static EthernetController Controller;

        public static void Initialize()
        {
            foreach (var dev in PCI.Devices)
            {
                if (Controller != null) return;

                if (dev.VendorID == (ushort)VendorID.Intel)
                {
                    if (
                        dev.DeviceID == (ushort)DeviceID.Intel82542 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82543GC ||
                        dev.DeviceID == (ushort)DeviceID.Intel82543GC_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82544EI ||
                        dev.DeviceID == (ushort)DeviceID.Intel82544EI_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82543EI ||
                        dev.DeviceID == (ushort)DeviceID.Intel82544GC ||
                        dev.DeviceID == (ushort)DeviceID.Intel82540EM ||
                        dev.DeviceID == (ushort)DeviceID.Intel82545EM ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546EB ||
                        dev.DeviceID == (ushort)DeviceID.Intel82545EM_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546EB_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82541EI ||
                        dev.DeviceID == (ushort)DeviceID.Intel82541ER ||
                        dev.DeviceID == (ushort)DeviceID.Intel82540EM_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82540EP ||
                        dev.DeviceID == (ushort)DeviceID.Intel82540EP_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82541EI_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82547EI ||
                        dev.DeviceID == (ushort)DeviceID.Intel82547EI_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546EB_2 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82540EP_2 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82545GM ||
                        dev.DeviceID == (ushort)DeviceID.Intel82545GM_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82545GM_2 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82566MM_ICH8 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82566DM_ICH8 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82566DC_ICH8 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82562V_ICH8 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82566MC_ICH8 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82571EB ||
                        dev.DeviceID == (ushort)DeviceID.Intel82571EB_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82571EB_2 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82547EI_2 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82541GI ||
                        dev.DeviceID == (ushort)DeviceID.Intel82547EI_3 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82541ER_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546EB_3 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546EB_4 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546EB_5 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82541PI ||
                        dev.DeviceID == (ushort)DeviceID.Intel82572EI ||
                        dev.DeviceID == (ushort)DeviceID.Intel82572EI_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82572EI_2 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546GB ||
                        dev.DeviceID == (ushort)DeviceID.Intel82573E ||
                        dev.DeviceID == (ushort)DeviceID.Intel82573E_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel80003ES2LAN ||
                        dev.DeviceID == (ushort)DeviceID.Intel80003ES2LAN_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546GB_1 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82573L ||
                        dev.DeviceID == (ushort)DeviceID.Intel82571EB_3 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82575 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82575_serdes ||
                        dev.DeviceID == (ushort)DeviceID.Intel82546GB_2 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82572EI_3 ||
                        dev.DeviceID == (ushort)DeviceID.Intel80003ES2LAN_2 ||
                        dev.DeviceID == (ushort)DeviceID.Intel80003ES2LAN_3 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82571EB_4 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82566DM_ICH9 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82562GT_ICH8 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82562G_ICH8 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82576 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82574L ||
                        dev.DeviceID == (ushort)DeviceID.Intel82575_quadcopper ||
                        dev.DeviceID == (ushort)DeviceID.Intel82567V_ICH9 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82567LM_4_ICH9 ||
                        dev.DeviceID == (ushort)DeviceID.Intel82577LM ||
                        dev.DeviceID == (ushort)DeviceID.Intel82577LC ||
                        dev.DeviceID == (ushort)DeviceID.Intel82578DM ||
                        dev.DeviceID == (ushort)DeviceID.Intel82578DC ||
                        dev.DeviceID == (ushort)DeviceID.Intel82567LM_ICH9_egDellE6400Notebook ||
                        dev.DeviceID == (ushort)DeviceID.Intel82579LM ||
                        dev.DeviceID == (ushort)DeviceID.Intel82579V ||
                        dev.DeviceID == (ushort)DeviceID.Intel82576NS ||
                        dev.DeviceID == (ushort)DeviceID.Intel82580 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI350 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI210 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI210_1 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI217LM ||
                        dev.DeviceID == (ushort)DeviceID.IntelI217VA ||
                        dev.DeviceID == (ushort)DeviceID.IntelI218V ||
                        dev.DeviceID == (ushort)DeviceID.IntelI218LM ||
                        dev.DeviceID == (ushort)DeviceID.IntelI218LM2 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI218V_1 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI218LM3 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI218V3 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI219LM ||
                        dev.DeviceID == (ushort)DeviceID.IntelI219V ||
                        dev.DeviceID == (ushort)DeviceID.IntelI219LM2 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI219V2 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI219LM3 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI219LM_1 ||
                        dev.DeviceID == (ushort)DeviceID.IntelI219LM_2
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
        //Check Out Intel825XX.cs Line 67
        //Check Out Intel825XX.cs Line 235
        //If You Want To Implement A Network Driver You Must Set Ethernet.MACAddress
        public abstract void Send(byte* Buffer, ushort Length);
    }
}
