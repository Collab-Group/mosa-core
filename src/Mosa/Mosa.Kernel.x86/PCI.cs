//From Cosmos
/*BSD 3-Clause License

Copyright (c) 2021, CosmosOS, COSMOS Project
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/

using System.Collections.Generic;

namespace Mosa.Kernel
{
    public enum VendorID
    {
        Intel = 0x8086,
        AMD = 0x1022,
        VMWare = 0x15AD,
        Bochs = 0x1234,
        VirtualBox = 0x80EE,
        Realtek = 0x10EC
    }

    public enum DeviceID
    {
        SVGAIIAdapter = 0x0405,
        PCNETII = 0x2000,
        PCNETIII = 0x2001,
        BGA = 0x1111,
        VBVGA = 0xBEEF,
        VBoxGuest = 0xCAFE,
        AC97 = 0x2415,
        RTL8139 = 0x8139,

        #region Intel 825XX Series
        Intel82542 = 0x1000,
        Intel82543GC = 0x1001,
        Intel82543GC_1 = 0x1004,
        Intel82544EI = 0x1008,
        Intel82544EI_1 = 0x1009,
        Intel82543EI = 0x100C,
        Intel82544GC = 0x100D,
        Intel82540EM = 0x100E,
        Intel82545EM = 0x100F,
        Intel82546EB = 0x1010,
        Intel82545EM_1 = 0x1011,
        Intel82546EB_1 = 0x1012,
        Intel82541EI = 0x1013,
        Intel82541ER = 0x1014,
        Intel82540EM_1 = 0x1015,
        Intel82540EP = 0x1016,
        Intel82540EP_1 = 0x1017,
        Intel82541EI_1 = 0x1018,
        Intel82547EI = 0x1019,
        Intel82547EI_1 = 0x101A,
        Intel82546EB_2 = 0x101D,
        Intel82540EP_2 = 0x101E,
        Intel82545GM = 0x1026,
        Intel82545GM_1 = 0x1027,
        Intel82545GM_2 = 0x1028,
        Intel82566MM_ICH8 = 0x1049,
        Intel82566DM_ICH8 = 0x104A,
        Intel82566DC_ICH8 = 0x104B,
        Intel82562V_ICH8 = 0x104C,
        Intel82566MC_ICH8 = 0x104D,
        Intel82571EB = 0x105E,
        Intel82571EB_1 = 0x105F,
        Intel82571EB_2 = 0x1060,
        Intel82547EI_2 = 0x1075,
        Intel82541GI = 0x1076,
        Intel82547EI_3 = 0x1077,
        Intel82541ER_1 = 0x1078,
        Intel82546EB_3 = 0x1079,
        Intel82546EB_4 = 0x107A,
        Intel82546EB_5 = 0x107B,
        Intel82541PI = 0x107C,
        Intel82572EI = 0x107D,
        Intel82572EI_1 = 0x107E,
        Intel82572EI_2 = 0x107F,
        Intel82546GB = 0x108A,
        Intel82573E = 0x108B,
        Intel82573E_1 = 0x108C,
        Intel80003ES2LAN = 0x1096,
        Intel80003ES2LAN_1 = 0x1098,
        Intel82546GB_1 = 0x1099,
        Intel82573L = 0x109A,
        Intel82571EB_3 = 0x10A4,
        Intel82575 = 0x10A7,
        Intel82575_serdes = 0x10A9,
        Intel82546GB_2 = 0x10B5,
        Intel82572EI_3 = 0x10B9,
        Intel80003ES2LAN_2 = 0x10BA,
        Intel80003ES2LAN_3 = 0x10BB,
        Intel82571EB_4 = 0x10BC,
        Intel82566DM_ICH9 = 0x10BD,
        Intel82562GT_ICH8 = 0x10C4,
        Intel82562G_ICH8 = 0x10C5,
        Intel82576 = 0x10C9,
        Intel82574L = 0x10D3,
        Intel82575_quadcopper = 0x10A9,
        Intel82567V_ICH9 = 0x10CB,
        Intel82567LM_4_ICH9 = 0x10E5,
        Intel82577LM = 0x10EA,
        Intel82577LC = 0x10EB,
        Intel82578DM = 0x10EF,
        Intel82578DC = 0x10F0,
        Intel82567LM_ICH9_egDellE6400Notebook = 0x10F5,
        Intel82579LM = 0x1502,
        Intel82579V = 0x1503,
        Intel82576NS = 0x150A,
        Intel82580 = 0x150E,
        IntelI350 = 0x1521,
        IntelI210 = 0x1533,
        IntelI210_1 = 0x157B,
        IntelI217LM = 0x153A,
        IntelI217VA = 0x153B,
        IntelI218V = 0x1559,
        IntelI218LM = 0x155A,
        IntelI218LM2 = 0x15A0,
        IntelI218V_1 = 0x15A1,
        IntelI218LM3 = 0x15A2,
        IntelI218V3 = 0x15A3,
        IntelI219LM = 0x156F,
        IntelI219V = 0x1570,
        IntelI219LM2 = 0x15B7,
        IntelI219V2 = 0x15B8,
        IntelI219LM3 = 0x15BB,
        IntelI219LM_1 = 0x15D7,
        IntelI219LM_2 = 0x15E3
        #endregion
    }

    public class PCI
    {
        public static List<PCIDevice> Devices;

        public static uint Count
        {
            get { return (uint)Devices.Count; }
        }

        internal static void Setup()
        {
            Devices = new List<PCIDevice>();
            if ((PCIDevice.GetHeaderType(0x0, 0x0, 0x0) & 0x80) == 0)
            {
                CheckBus(0x0);
            }
            else
            {
                for (ushort fn = 0; fn < 8; fn++)
                {
                    if (PCIDevice.GetVendorID(0x0, 0x0, fn) != 0xFFFF)
                        break;

                    CheckBus(fn);
                }
            }
        }

        private static void CheckBus(ushort xBus)
        {
            for (ushort device = 0; device < 32; device++)
            {
                if (PCIDevice.GetVendorID(xBus, device, 0x0) == 0xFFFF)
                    continue;

                CheckFunction(new PCIDevice(xBus, device, 0x0));
                if ((PCIDevice.GetHeaderType(xBus, device, 0x0) & 0x80) != 0)
                {
                    for (ushort fn = 1; fn < 8; fn++)
                    {
                        if (PCIDevice.GetVendorID(xBus, device, fn) != 0xFFFF)
                            CheckFunction(new PCIDevice(xBus, device, fn));
                    }
                }
            }
        }

        private static void CheckFunction(PCIDevice xPCIDevice)
        {
            Devices.Add(xPCIDevice);

            if (xPCIDevice.ClassID == 0x6 && xPCIDevice.Subclass == 0x4)
                CheckBus(xPCIDevice.SecondaryBusNumber);
        }

        public static bool Exists(VendorID aVendorID, DeviceID aDeviceID)
        {
            return GetDevice(aVendorID, aDeviceID) != null;
        }

        public static PCIDevice GetDevice(VendorID aVendorID, DeviceID aDeviceID)
        {
            foreach (var xDevice in Devices)
            {
                if ((VendorID)xDevice.VendorID == aVendorID &&
                    (DeviceID)xDevice.DeviceID == aDeviceID)
                {
                    return xDevice;
                }
            }
            return null;
        }
    }
}
