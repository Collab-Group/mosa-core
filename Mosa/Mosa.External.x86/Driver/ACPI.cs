using Mosa.Kernel.x86;
using Mosa.Runtime.Plug;
using Mosa.Runtime.x86;
using System;
using System.Runtime.InteropServices;

namespace Mosa.External.x86.Driver
{
    public unsafe class ACPI
    {
        private static short SLP_TYPa;
        private static short SLP_TYPb;
        private static short SLP_EN;

        private static ACPI_FADT* FADT;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ACPI_RSDP
        {
            public fixed sbyte Signature[8];
            public byte Checksum;
            public fixed sbyte OEMID[6];
            public byte Revision;
            public uint RsdtAddress;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ACPI_SDT
        {
            public fixed sbyte Signature[4];
            public uint Length;
            public byte Revision;
            public byte Checksum;
            public fixed byte OEMID[6];
            public fixed sbyte OEMTableID[8];
            public uint OEMRevision;
            public uint CreatorID;
            public uint CreatorRevision;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ACPI_FADT
        {
            public fixed byte Signature[4];
            public uint Length;
            public byte Revision;
            public byte Checksum;
            public fixed byte OEMID[6];
            public fixed byte OEMTableID[8];
            public uint OEMRevision;
            public uint CreatorID;
            public uint CreatorRevision;

            public uint FirmwareCtrl;
            public uint Dsdt;

            public byte Reserved;

            public byte PreferredPowerManagementProfile;
            public ushort SCI_Interrupt;
            public uint SMI_CommandPort;
            public byte AcpiEnable;
            public byte AcpiDisable;
            public byte S4BIOS_REQ;
            public byte PSTATE_Control;
            public uint PM1aEventBlock;
            public uint PM1bEventBlock;
            public uint PM1aControlBlock;
            public uint PM1bControlBlock;
            public uint PM2ControlBlock;
            public uint PMTimerBlock;
            public uint GPE0Block;
            public uint GPE1Block;
            public byte PM1EventLength;
            public byte PM1ControlLength;
            public byte PM2ControlLength;
            public byte PMTimerLength;
            public byte GPE0Length;
            public byte GPE1Length;
            public byte GPE1Base;
            public byte CStateControl;
            public ushort WorstC2Latency;
            public ushort WorstC3Latency;
            public ushort FlushSize;
            public ushort FlushStride;
            public byte DutyOffset;
            public byte DutyWidth;
            public byte DayAlarm;
            public byte MonthAlarm;
            public byte Century;

            public ushort BootArchitectureFlags;

            public byte Reserved2;
            public uint Flags;
        }

        public static void Shutdown()
        {
            IOPort.Out16((ushort)FADT->PM1aControlBlock, (ushort)(SLP_TYPa | SLP_EN));
            IOPort.Out16((ushort)FADT->PM1bControlBlock, (ushort)(SLP_TYPb | SLP_EN));
            Native.Hlt();
            Panic.Error("ACPI Shutdown Failed");
        }

        [Plug("Mosa.Kernel.x86.Kernel::InitializeACPI")]
        private static bool Initialize()
        {
            ACPI_RSDP* rsdp = GetRSDP();
            //MMIO.Map(rsdp->RsdtAddress, ushort.MaxValue);
            ACPI_SDT* hdr = (ACPI_SDT*)rsdp->RsdtAddress;
            byte* rsdt = (byte*)rsdp->RsdtAddress;

            if (rsdt != null && *(uint*)rsdt == 0x54445352) //RSDT
            {
                uint addr = 0;
                int entries = (int)hdr->Length;
                entries = (entries - sizeof(ACPI_SDT)) / 4;
                rsdt += sizeof(ACPI_SDT);

                while (0 < entries--)
                {
                    for (int i = 3; i >= 0; i--)
                    {
                        addr += (*(rsdt + i));
                        addr = (i == 0) ? addr : addr << 8;
                    }

                    //MMIO.Map(addr, ushort.MaxValue);

                    FADT = (ACPI_FADT*)addr;

                    if (*(uint*)FADT->Dsdt == 0x54445344) //DSDT
                    {
                        byte* S5Addr = (byte*)FADT->Dsdt + sizeof(ACPI_SDT);
                        int dsdtLength = *((int*)FADT->Dsdt + 1) - sizeof(ACPI_SDT);

                        while (0 < dsdtLength--)
                        {
                            if (*(uint*)S5Addr == 0x5f35535f) //_S5_
                                break;
                            S5Addr++;
                        }

                        if (dsdtLength > 0)
                        {
                            if ((*(S5Addr - 1) == 0x08 || (*(S5Addr - 2) == 0x08 && *(S5Addr - 1) == '\\')) && *(S5Addr + 4) == 0x12)
                            {
                                S5Addr += 5;
                                S5Addr += ((*S5Addr & 0xC0) >> 6) + 2;
                                if (*S5Addr == 0x0A)
                                    S5Addr++;
                                SLP_TYPa = (short)(*(S5Addr) << 10);
                                S5Addr++;
                                if (*S5Addr == 0x0A)
                                    S5Addr++;
                                SLP_TYPb = (short)(*(S5Addr) << 10);
                                SLP_EN = 1 << 13;

                                return true;
                            }
                        }
                    }
                    rsdt += 4;
                }
            }

            return false;
        }

        private static unsafe ACPI_RSDP* GetRSDP()
        {
            byte* p = (byte*)0xE0000;
            byte* end = (byte*)0xFFFFF;

            while (p < end)
            {
                ulong signature = *(ulong*)p;

                if (signature == 0x2052545020445352) // 'RSD PTR '
                {
                    return (ACPI_RSDP*)p;
                }

                p += 16;
            }

            return null;
        }
    }
}
