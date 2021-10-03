
using System.Runtime.InteropServices;

namespace Mosa.External.x86.Driver
{
    public class AHCI
    {
        const byte MaxBus = 0xFF;
        const byte MaxSlot = 0x1F;
        const byte MaxFunc = 0x7;
        const ushort HBAPort = 0xCF8;
        const uint Base = 0x400000;
        const ushort PxCMDSt = 0x0001;
        const ushort PxCMDFre = 0x0010;
        const ushort PxCMDFr = 0x4000;
        const ushort PxCMDCr = 0x8000;
        const uint PxISTFES = 0x40000000;

        const byte REGH2D = 0x27;
        const byte CMD_READ_DMA_EX = 0x25;

        const byte IPMActive = 0x1;
        const byte DETPresent = 0x3;

        const uint ATAPI = 0xEB140101;
        const uint SEMB = 0xC33C0101;
        const uint PM = 0x96690101;

        const byte DEVBusy = 0x80;
        const byte DEVDRQ = 0x08;

        const byte GHCOffset = 0x4;
        uint AHCI_GHC_MASK(uint val)
        {
            return val >> 31;
        }

        const byte VendorOffset = 0x0;
        const byte DeviceOffset = 0x02;

        const byte BAR5Offset = 0x24;

        // Vendors
        const int VendorIntel = 0x8086;
        const int VendorVMWARE = 0x15AD;

        public class tagHBA_PORT
        {
            public uint clb;        // 0x00, command list base address, 1K-byte aligned
            public uint clbu;       // 0x04, command list base address upper 32 bits
            public uint fb;         // 0x08, FIS base address, 256-byte aligned
            public uint fbu;        // 0x0C, FIS base address upper 32 bits
            public uint Is;         // 0x10, interrupt status
            public uint ie;         // 0x14, interrupt enable
            public uint cmd;        // 0x18, command and status
            public uint rsv0;       // 0x1C, Reserved
            public uint tfd;        // 0x20, task file data
            public uint sig;        // 0x24, signature
            public uint ssts;       // 0x28, SATA status (SCR0:SStatus)
            public uint sctl;       // 0x2C, SATA control (SCR2:SControl)
            public uint serr;       // 0x30, SATA error (SCR1:SError)
            public uint sact;       // 0x34, SATA active (SCR3:SActive)
            public uint ci;         // 0x38, command issue
            public uint sntf;       // 0x3C, SATA notification (SCR4:SNotification)
            public uint fbs;        // 0x40, FIS-based switch control
            public uint[] rsv1 = new uint[11];   // 0x44 ~ 0x6F, Reserved
            public uint[] vendor = new uint[4];  // 0x70 ~ 0x7F, vendor specific
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct tagHBA_CMD_HEADER
        {
            public byte cflawp;

            public byte rbcrsv0pmp;

            public uint prdtl;   // Physical region descriptor table length in entries

            public uint prdbc;   // Physical region descriptor byte count transferred

            public uint ctba;    // Command table descriptor base address
            public uint ctbau;   // Command table descriptor base address upper 32 bits

            fixed uint rsv1[4];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct tagFIS_REG_H2D
        {
            public byte fis_type;  // FIS_TYPE_REG_H2D

            public byte pmportrsv0c;

            public byte command;   // Command register
            public byte featurel;  // Feature register, 7:0

            public byte lba0;      // LBA low register, 7:0
            public byte lba1;      // LBA mid register, 15:8
            public byte lba2;      // LBA high register, 23:16
            public byte device;    // Device register

            public byte lba3;     // LBA register, 31:24
            public byte lba4;     // LBA register, 39:32
            public byte lba5;     // LBA register, 47:40
            public byte featureh; // Feature register, 15:8

            public byte countl;   // Count register, 7:0
            public byte counth;   // Count register, 15:8
            public byte icc;      // Isochronous command completion
            public byte control;  // Control register

            fixed byte rsv1[4];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct tagHBA_PRDT_ENTRY
        {
            public uint dba;    // Data base address
            public uint dbau;   // Data base address upper 32 bits
            public uint rsv0;    // Reserved

            public uint dbcrsv1i;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct tagHBA_MEM
        {
            public uint cap;     // 0x00, Host capability
            public uint ghc;     // 0x04, Global host control
            public uint Is;     // 0x08, Interrupt status
            public uint pi;      // 0x0C, Port implemented
            public uint vs;      // 0x10, Version
            public uint ccc_ctl; // 0x14, Command completion coalescing control
            public uint ccc_pts; // 0x18, Command completion coalescing ports
            public uint em_loc;      // 0x1C, Enclosure management location
            public uint em_ctl;      // 0x20, Enclosure management control
            public uint cap2;        // 0x24, Host capabilities extended
            public uint bohc;        // 0x28, BIOS/OS handoff control and status

            // 0x2C - 0x9F, Reserved
            public fixed byte rsv[0xA0 - 0x2C];

            // 0xA0 - 0xFF, Vendor specific registers
            public fixed byte vendor[0x100 - 0xA0];

            // 0x100 - 0x10FF, Port control registers
            //tagHBA_PORT	ports[1];	// 1 ~ 32
            public fixed byte ports[0x10FF - 0x100];
        }
    }
}
