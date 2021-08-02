using Mosa.Kernel.x86;
using Mosa.Runtime;
using System.Runtime.InteropServices;
using Mosa.Runtime.x86;
using Mosa.Kernel;
using System;

namespace Mosa.External.x86.Driver
{
    //https://wiki.osdev.org/ACPI
    //https://github.com/CosmosOS/Cosmos/blob/master/source/Cosmos.Core/ACPI.cs (for some functions)

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RSDPDescriptor
    {
        public fixed sbyte Signature[8];
        public byte Checksum;
        public fixed sbyte OEMID[6];
        public byte Revision;
        public uint RsdtAddress;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RSDPDescriptor20
    {
        public RSDPDescriptor firstPart;

        public uint Length;
        public ulong XsdtAddress;
        public byte ExtendedChecksum;
        public fixed byte reserved[3];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ACPISDTHeader
    {
        public fixed sbyte Signature[4];
        public uint Length;
        public byte Revision;
        public byte Checksum;
        public fixed sbyte OEMID[6];
        public fixed sbyte OEMTableID[8];
        public uint OEMRevision;
        public uint CreatorID;
        public uint CreatorRevision;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RSDT
    {
        public ACPISDTHeader h;
        public fixed uint PointerToOtherSDT[8]; // This is problematic, we need a way to statically initialize this array's size with h.Length and stuff

        /*public void Init()
        {
            fixed (uint* ptr = new uint[(int)((h.Length - sizeof(ACPISDTHeader)) / 4)])
                PointerToOtherSDT = ptr;
        }*/
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FADT
    {
        public ACPISDTHeader h;
        public uint FirmwareCtrl;
        public uint Dsdt;

        // Field used in ACPI 1.0; no longer in use, for compatibility only
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

        // Reserved in ACPI 1.0; used since ACPI 2.0+
        public ushort BootArchitectureFlags;

        public byte Reserved2;
        public uint Flags;

        // 12 byte structure; see below for details
        public GenericAddressStructure ResetReg;

        public byte ResetValue;
        public fixed byte Reserved3[3];

        // 64bit pointers - Available on ACPI 2.0+
        public ulong X_FirmwareControl;
        public ulong X_Dsdt;

        public GenericAddressStructure X_PM1aEventBlock;
        public GenericAddressStructure X_PM1bEventBlock;
        public GenericAddressStructure X_PM1aControlBlock;
        public GenericAddressStructure X_PM1bControlBlock;
        public GenericAddressStructure X_PM2ControlBlock;
        public GenericAddressStructure X_PMTimerBlock;
        public GenericAddressStructure X_GPE0Block;
        public GenericAddressStructure X_GPE1Block;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct GenericAddressStructure
    {
        public byte AddressSpace;
        public byte BitWidth;
        public byte BitOffset;
        public byte AccessSize;
        public ulong Address;
    }

    public unsafe class ACPI
    {
        public static RSDPDescriptor* Descriptor { get; private set; }
        public static RSDPDescriptor20* Descriptor20 { get; private set; }

        public static RSDT* RSDT { get; private set; }
        public static FADT* FADT { get; private set; }

        public static bool Enabled { get; private set; }

        public static void Initialize()
        {
            Descriptor = null;
            Descriptor20 = null;

            RSDT = null;
            FADT = null;

            Enabled = false;

            // Get RSDP address
            uint rsdp = 0x00000000;

            // Check in the memory region between 0x000E0000 and 0x000FFFFF
            for (uint i = 0x000E0000; i <= 0x000FFFFF; i += 4)
                if (Compare("RSD PTR ", i))
                    if (CheckRSD(i))
                    {
                        rsdp = i;
                        break;
                    }

            // Check in the EBDA (Extended BIOS Data Area)
            // (broken code, null pointer exception)
            /*uint ebda = ((*(uint*)0x040E) * 0x10) & 0x000FFFFF;
            for (uint i = ebda; i < ebda + 1024; i += 4) // address + 1KB (1024)
                if (Compare("RSD PTR ", i))
                {
                    rsdp = i;
                    break;
                }*/

            if (rsdp == 0x00000000)
            {
                Console.WriteLine("ACPI RSDP address couldn't be found :/");
                return;
            }

            // RSDP table
            Pointer pointer = new Pointer(rsdp);
            Descriptor = (RSDPDescriptor*)pointer;

            // Checksum validation
            byte result = (byte)(((sbyte)Descriptor->Signature + Descriptor->Checksum + (sbyte)Descriptor->OEMID + Descriptor->Revision + Descriptor->RsdtAddress) >> 8);

            if (result != 0)
                Console.WriteLine("Checksum of ACPI table RSDP is invalid!!!!\n");

            // ACPI revision 2
            if (Descriptor->Revision == 2)
            {
                Descriptor20 = (RSDPDescriptor20*)pointer;

                // Checksum validation 2
                byte result2 = (byte)((result + (byte)Descriptor20->Length + Descriptor20->ExtendedChecksum + (uint)Descriptor20->XsdtAddress + (byte)Descriptor20->reserved) >> 8);
                
                if (result2 != 0)
                    Console.WriteLine("Second checksum of ACPI table RSDP is invalid!!!!\n");
            }

            Console.WriteLine("Initializing ACPI RSDT...");

            MemoryBlock rsdtBlock = Memory.GetPhysicalMemory(new Pointer(Descriptor->RsdtAddress), (uint)sizeof(RSDT));
            RSDT = (RSDT*)rsdtBlock.Address;

            // Won't work, we need to initialize array the same time we cast to RSDT*
            //rsdt->Init();

            byte result3 = (byte)(((sbyte)RSDT->h.Signature + RSDT->h.Length + RSDT->h.Revision + RSDT->h.Checksum + (sbyte)RSDT->h.OEMID + (sbyte)RSDT->h.OEMTableID + RSDT->h.OEMRevision + RSDT->h.CreatorID + RSDT->h.CreatorRevision + (long)RSDT->PointerToOtherSDT) >> 8);
            if (result3 != 0)
                Console.WriteLine("Checksum of ACPI table RSDT is invalid!!!!\n");

            Console.WriteLine("Initializing ACPI FADT...");

            FADT = (FADT*)FindBySignature(RSDT, "FACP");
            if (FADT == null)
            {
                Console.WriteLine("Couldn't find FADT table :/");
                return;
            }

            if (ToStringFromAddress(4, FADT->Dsdt) != "DSDT")
            {
                Console.WriteLine("Couldn't find DSDT table :/");
                return;
            }

            Console.WriteLine("Enabling ACPI...");
            EnableACPI();
        }

        //https://github.com/mintsuki-org/facp_shutdown_hack/blob/master/facp_shutdown_hack.c
        public static void Shutdown()
        {
            if (FADT != null && Enabled)
            {
                if (PCI.Exists(VendorID.VirtualBox, DeviceID.VBoxGuest))
                    Native.Out16(0x4004, 0x3400);
                else if (PCI.Exists(VendorID.Bochs, DeviceID.BGA))
                    Native.Out16(0xB004, 0x2000);
                else { } // We might support QEMU (address 0x604, value 0x2000 for newer versions of QEMU) but we also need to detect QEMU
            }
        }

        public static void EnableACPI()
        {
            if (FADT != null && !Enabled)
            {
                Enabled = true;

                // Maybe check if bit 0 (value 1) if PM1a control block I/O port is set
                if (FADT->SMI_CommandPort != 0 && FADT->AcpiEnable != 0)
                {
                    Native.Out8((ushort)FADT->SMI_CommandPort, FADT->AcpiEnable);
                    PIT.Wait(3000); // Wait 3 seconds to change modes
                }
            }
        }

        public static void DisableACPI()
        {
            if (FADT != null && Enabled)
            {
                Enabled = false;

                if (FADT->SMI_CommandPort != 0 && FADT->AcpiDisable != 0)
                {
                    Native.Out8((ushort)FADT->SMI_CommandPort, FADT->AcpiDisable);
                    PIT.Wait(3000); // Wait 3 seconds to change modes
                }
            }
        }

        private static void* FindBySignature(void* RootSDT, string signature)
        {
            RSDT* rsdt = (RSDT*)RootSDT;

            //int entries = (int)((rsdt->h.Length - sizeof(ACPISDTHeader)) / 4);
            int entries = 8;
            for (int i = 0; i < entries; i++)
            {
                ACPISDTHeader* h = (ACPISDTHeader*)rsdt->PointerToOtherSDT[i];

                if (h != null && ToStringFromCharPointer(4, h->Signature) == signature)
                    return h;
            }

            // No SDT found (by signature)
            return null;
        }

        private static bool CheckRSD(uint address)
        {
            byte sum = 0;
            byte* check = (byte*)address;

            for (int i = 0; i < 20; i++)
                sum += *check++;

            return sum == 0;
        }

        private static string ToStringFromAddress(int length, uint address)
        {
            byte* pointer = (byte*)address;
            string str = "";

            for (int i = 0; i < length; i++)
                str += (char)pointer[i];

            return str;
        }

        private static string ToStringFromCharPointer(int length, sbyte* pointer)
        {
            string str = "";

            for (int i = 0; i < length; i++)
                str += (char)pointer[i];

            return str;
        }

        private static bool Compare(string str, uint address)
        {
            byte* value = (byte*)address;

            for (int i = 0; i < str.Length; i++)
                if (str[i] != value[i])
                    return false;

            return true;
        }
    }
}
