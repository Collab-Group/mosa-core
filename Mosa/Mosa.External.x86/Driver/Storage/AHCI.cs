
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct HBA_PORT
        {
            public uint clb;
            public uint clbu;
            public uint fb;
            public uint fbu;
            public int Is;
            public uint ie;
            public uint cmd;
            public uint rsv0;
            public uint tfd;
            public uint sig;
            public uint ssts;
            public uint sctl;
            public uint serr;
            public uint sact;
            public uint ci;
            public uint sntf;
            public uint fbs;
            fixed uint rsv1[11];
            fixed uint vendor[4];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct HBA_CMD_HEADER
        {
            public byte cflawp;

            public byte rbcrsv0pmp;

            public uint prdtl;

            public uint prdbc;

            public uint ctba;
            public uint ctbau;

            fixed uint rsv1[4];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct FIS_REG_H2D
        {
            public byte fis_type;

            public byte pmportrsv0c;

            public byte command;
            public byte featurel;

            public byte lba0;
            public byte lba1;
            public byte lba2;
            public byte device;

            public byte lba3;
            public byte lba4;
            public byte lba5;
            public byte featureh;

            public byte countl;
            public byte counth;
            public byte icc;
            public byte control;

            fixed byte rsv1[4];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HBA_PRDT_ENTRY
        {
            public uint dba;
            public uint dbau;
            public uint rsv0;

            public uint dbcrsv1i;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct HBA_MEM
        {
            public uint cap;
            public uint ghc;
            public uint Is;
            public uint pi;
            public uint vs;
            public uint ccc_ctl;
            public uint ccc_pts;
            public uint em_loc;
            public uint em_ctl;
            public uint cap2;
            public uint bohc;

            public fixed byte rsv[0xA0 - 0x2C];

            public fixed byte vendor[0x100 - 0xA0];

            public fixed byte ports[0x10FF - 0x100];
        }

        public const uint SATA_SIG_ATA = 0x00000101;
        public const uint SATA_SIG_ATAPI = 0xEB140101;
        public const uint SATA_SIG_SEMB = 0xC33C0101;
        public const uint SATA_SIG_PM = 0x96690101;

        public const byte AHCI_DEV_NULL = 0;
        public const byte AHCI_DEV_SATA = 1;
        public const byte AHCI_DEV_SEMB = 2;
        public const byte AHCI_DEV_PM = 3;
        public const byte AHCI_DEV_SATAPI = 4;

        public const byte HBA_PORT_IPM_ACTIVE = 1;
        public const byte HBA_PORT_DET_PRESENT = 3;

        public unsafe void ProbePort(HBA_MEM *abar)
        {
            uint pi = abar->pi;
            int i = 0;
            while (i < 32)
            {
                if ((pi & 1) != 0)
                {
                    int dt = CheckType((HBA_PORT*)&abar->ports[i]);
                    if (dt == AHCI_DEV_SATA)
                    {
                        Console.WriteLine("SATA drive found at port " + i);
                    }
                    else if (dt == AHCI_DEV_SATAPI)
                    {
                        Console.WriteLine("SATAPI drive found at port " + i);
                    }
                    else if (dt == AHCI_DEV_SEMB)
                    {
                        Console.WriteLine("SEMB drive found at port " + i);
                    }
                    else if (dt == AHCI_DEV_PM)
                    {
                        Console.WriteLine("PM drive found at port " + i);
                    }
                    else
                    {
                        Console.WriteLine("No drive found at port " + i);
                    }
                }

                pi >>= 1;
                i++;
            }
        }

        public unsafe int CheckType(HBA_PORT *port)
        {
            uint ssts = port->ssts;

            byte ipm = (byte)((ssts >> 8) & 0x0F);
            byte det = (byte)(ssts & 0x0F);

            if (det != HBA_PORT_DET_PRESENT)
                return AHCI_DEV_NULL;
            if (ipm != HBA_PORT_IPM_ACTIVE)
                return AHCI_DEV_NULL;

            switch (port->sig)
            {
                case SATA_SIG_ATAPI:
                    return AHCI_DEV_SATAPI;
                case SATA_SIG_SEMB:
                    return AHCI_DEV_SEMB;
                case SATA_SIG_PM:
                    return AHCI_DEV_PM;
                default:
                    return AHCI_DEV_SATA;
            }
        }

        public const uint AHCI_BASE = 0x400000;

        public const byte HBA_PxCMD_ST = 0x0001;
        public const byte HBA_PxCMD_FRE = 0x0010;
        public const short HBA_PxCMD_FR = 0x4000;
        public const uint HBA_PxCMD_CR = 0x8000;

        public unsafe void PortRebase(HBA_PORT *port, int portno)
        {
            StopCMD(port);

            port->clb = (uint)(AHCI_BASE + (portno << 10));
            port->clbu = 0;
            ASM.MEMFILL((uint)(void*)port->clb, 0, 1024);

            port->fb = (uint)(AHCI_BASE + (32 << 10) + (portno << 8));
            port->fbu = 0;
            ASM.MEMFILL((uint)(void*)port->fb, 0, 256);

            HBA_CMD_HEADER* cmdheader = (HBA_CMD_HEADER*)(port->clb);
            for (int i = 0; i < 32; i++)
            {
                cmdheader[i].prdtl = 8;

                cmdheader[i].ctba = (uint)(AHCI_BASE + (40 << 10) + (portno << 13) + (i << 8));
                cmdheader[i].ctbau = 0;
                ASM.MEMFILL((uint)(void*)cmdheader[i].ctba, 0, 256);
            }
            StartCMD(port);
        }

        public unsafe void StartCMD(HBA_PORT *port)
        {
            while ((port->cmd & HBA_PxCMD_CR) != 0)
                ;
            port->cmd |= HBA_PxCMD_FRE;
            port->cmd |= HBA_PxCMD_ST;
        }

        public unsafe void StopCMD(HBA_PORT *port)
        {
            //port->cmd &= ~HBA_PxCMD_ST;
            //port->cmd &= ~HBA_PxCMD_FRE;

            for (; ; )
            {
                if ((port->cmd & HBA_PxCMD_FR) != 0)
                    continue;
                if ((port->cmd & HBA_PxCMD_CR) != 0)
                    continue;
                break;
            }
        }
    }
}
