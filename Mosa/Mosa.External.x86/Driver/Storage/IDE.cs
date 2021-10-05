using Mosa.External.x86;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Driver
{
    public unsafe class IDE
    {
        #region Definitions
        private struct IDECommand
        {
            public const byte ReadSectorsWithRetry = 0x20;
            public const byte WriteSectorsWithRetry = 0x30;
            public const byte IdentifyDrive = 0xEC;
            public const byte CacheFlush = 0xE7;
        }

        private struct StatusRegister
        {
            public const byte Busy = 1 << 7;
            public const byte DriveReady = 1 << 6;
            public const byte DriveWriteFault = 1 << 5;
            public const byte DriveSeekComplete = 1 << 4;
            public const byte DataRequest = 1 << 3;
            public const byte CorrectedData = 1 << 2;
            public const byte Index = 1 << 1;
            public const byte Error = 1 << 0;
        }

        private struct IdentifyDrive
        {
            public const uint GeneralConfig = 0x00;
            public const uint LogicalCylinders = 0x02;
            public const uint LogicalHeads = 0x08;
            public const uint LogicalSectors = 0x06 * 2;
            public const uint SerialNbr = 0x14;
            public const uint ControllerType = 0x28;
            public const uint BufferSize = 0x15 * 2;
            public const uint FirmwareRevision = 0x17 * 2;
            public const uint ModelNumber = 0x1B * 2;
            public const uint SupportDoubleWord = 0x30 * 2;

            public const uint CommandSetSupported83 = 83 * 2;
            public const uint MaxLBA28 = 60 * 2;
            public const uint MaxLBA48 = 100 * 2;

            public const uint CommandSet = 164;
        }

        public const uint DrivesPerConroller = 2;

        protected ushort DataPort;
        protected ushort FeaturePort;
        protected ushort ErrorPort;
        protected ushort SectorCountPort;
        protected ushort LBALowPort;
        protected ushort LBAMidPort;
        protected ushort LBAHighPort;
        protected ushort DeviceHeadPort;
        protected ushort StatusPort;
        protected ushort CommandPort;
        protected ushort ControlPort;
        protected ushort AltStatusPort;

        private uint MaximumDriveCount { get; set; }

        private enum AddressingMode
        {
            LBA28
        }

        private struct DriveInfo
        {
            public bool Present;
            public ulong Size;
            public AddressingMode AddressingMode;
        }

        private DriveInfo[] driveInfo = new DriveInfo[DrivesPerConroller];

        public enum Drive
        {
            Master = 0,
            Slave = 1
        }

        public const uint SectorSize = 512;

        private struct BaseOffset
        {
            public const byte DataPort = 0;
            public const byte ErrorPort = 1;
            public const byte FeaturePort = 1;
            public const byte SectorCountPort = 2;
            public const byte LBALowPort = 3;
            public const byte LBAMidPort = 4;
            public const byte LBAHighPort = 5;
            public const byte DeviceHeadPort = 6;
            public const byte CommandPort = 7;
            public const byte StatusPort = 7;
        }

        private struct ControlOffset
        {
            public const byte ControlPort = 0;
            public const byte AltStatusPort = 6;
        }

        public enum ControllerIndex
        {
            One,
            Two
        }
        #endregion

        public IDE(ControllerIndex index)
        {
            ushort BasePort = 0, ControlPort = 0;

            switch (index)
            {
                case ControllerIndex.One:
                    BasePort = 0x1F0;
                    ControlPort = 0x3F6;
                    break;
                case ControllerIndex.Two:
                    BasePort = 0x170;
                    ControlPort = 0x376;
                    break;
            }

            DataPort = (ushort)(BasePort + BaseOffset.DataPort);
            ErrorPort = (ushort)(BasePort + BaseOffset.ErrorPort);
            FeaturePort = (ushort)(BasePort + BaseOffset.FeaturePort);
            SectorCountPort = (ushort)(BasePort + BaseOffset.SectorCountPort);
            LBALowPort = (ushort)(BasePort + BaseOffset.LBALowPort);
            LBAMidPort = (ushort)(BasePort + BaseOffset.LBAMidPort);
            LBAHighPort = (ushort)(BasePort + BaseOffset.LBAHighPort);
            DeviceHeadPort = (ushort)(BasePort + BaseOffset.DeviceHeadPort);
            CommandPort = (ushort)(BasePort + BaseOffset.CommandPort);
            StatusPort = (ushort)(BasePort + BaseOffset.StatusPort);
            ControlPort = (ushort)(ControlPort + ControlOffset.ControlPort);
            AltStatusPort = (ushort)(ControlPort + ControlOffset.AltStatusPort);

            MaximumDriveCount = 2;

            for (var drive = 0; drive < DrivesPerConroller; drive++)
            {
                driveInfo[drive].Present = false;
                driveInfo[drive].Size = 0;
            }
        }

        public void Initialize()
        {
            //Start Device
            IOPort.Out8(ControlPort, 0);

            for (byte drive = 0; drive < MaximumDriveCount; drive++)
            {
                DoIdentifyDrive(drive);
            }
        }

        public bool Available()
        {
            IOPort.Out8(LBALowPort, 0x88);

            var found = IOPort.In8(LBALowPort) == 0x88;

            return found;
        }

        private void DoIdentifyDrive(byte index)
        {
            driveInfo[index].Present = false;

            IOPort.Out8(DeviceHeadPort, (byte)((index == 0) ? 0xA0 : 0xB0));
            IOPort.Out8(SectorCountPort, 0);
            IOPort.Out8(LBALowPort, 0);
            IOPort.Out8(LBAMidPort, 0);
            IOPort.Out8(LBAHighPort, 0);
            IOPort.Out8(CommandPort, IDECommand.IdentifyDrive);

            if (IOPort.In8(StatusPort) == 0)
            {
                return;
            }

            if (!WaitForReadyStatus())
            {
                return;
            }

            if (IOPort.In8(LBAMidPort) != 0 && IOPort.In8(LBAHighPort) != 0) //Check if the drive is ATA
            {
                //In this case the drive is ATAPI
                return;
            }

            if (!WaitForIdentifyData())
            {
                return;
            }

            driveInfo[index].Present = true;

            //Read the identification info
            var info = new MemoryBlock(512);
            ASM.INSD(DataPort, (uint)info.Address, info.Size);

            //For Some Unknown Reason LBA48 Won't Be Detected :(
            AddressingMode aMode = AddressingMode.LBA28;

            driveInfo[index].AddressingMode = aMode;
            driveInfo[index].Size = info.Read32(IdentifyDrive.MaxLBA28) * SectorSize;

            Console.WriteLine($"Found Drive:{string.FromPointer((byte*)(info.Address + IdentifyDrive.ModelNumber), 40, 0x20)} Size:{(uint)(driveInfo[index].Size / (1024 * 1024))}MB");

            info.Free();
        }

        private bool WaitForReadyStatus()
        {
            byte status;
            do
            {
                status = IOPort.In8(StatusPort);
            }
            while ((status & StatusRegister.Busy) == StatusRegister.Busy);

            return true;
        }

        private bool WaitForIdentifyData()
        {
            byte status;
            do
            {
                status = IOPort.In8(StatusPort);
            }
            while ((status & StatusRegister.DataRequest) != StatusRegister.DataRequest && (status & StatusRegister.Error) != StatusRegister.Error);

            return ((status & StatusRegister.Error) != StatusRegister.Error);
        }

        private bool DoCacheFlush()
        {
            IOPort.Out8(CommandPort, IDECommand.CacheFlush);

            return WaitForReadyStatus();
        }

        public enum SectorOperation { Read, Write }

        public bool PerformLBA28(SectorOperation operation, Drive adrive, uint lba, byte* data)
        {
        Retry:
            uint drive = (uint)adrive;
            if (drive >= MaximumDriveCount || !driveInfo[drive].Present)
                return false;

            IOPort.Out8(DeviceHeadPort, (byte)(0xE0 | (drive << 4) | ((lba >> 24) & 0x0F)));
            //IOPort.Out8(FeaturePort, 0);
            IOPort.Out8(SectorCountPort, 1);
            IOPort.Out8(LBAHighPort, (byte)((lba >> 16) & 0xFF));
            IOPort.Out8(LBAMidPort, (byte)((lba >> 8) & 0xFF));
            IOPort.Out8(LBALowPort, (byte)(lba & 0xFF));

            IOPort.Out8(CommandPort, (operation == SectorOperation.Write) ? IDECommand.WriteSectorsWithRetry : IDECommand.ReadSectorsWithRetry);

            if (!WaitForReadyStatus())
                return false;

            Native.Cli();

            while ((IOPort.In8(StatusPort) & 0x80) != 0) ;

            if (operation == SectorOperation.Read)
            {
                ASM.INSD(DataPort, (uint)data, SectorSize);
            }
            else
            {
                ASM.OUTSD(DataPort, (uint)data, SectorSize);

                DoCacheFlush();
            }

            Native.Nop();
            if ((IOPort.In8(StatusPort) & 0x1) != 0)
            {
                Console.WriteLine($"Bad Status");
                goto Retry;
            }

            Native.Sti();

            return true;
        }
    }
}
