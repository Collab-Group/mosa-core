using Mosa.Kernel.x86;
using System.Collections.Generic;

namespace Mosa.External.x86.FileSystem
{
    public struct PartitionInfo
    {
        public bool IsBootable;
        public uint LBA;
        public uint Size;
    }

    public unsafe class MBR
    {
        public List<PartitionInfo> PartitionInfos { get; private set; }
        public MemoryBlock MBRDataBlock { get; private set; }

        public bool Initialized { get; private set; }

        public void Initialize(IDisk disk)
        {
            if (Initialized)
                return;

            byte[] mbrData = new byte[512];
            disk.ReadBlock(0, 1, mbrData);

            MBRDataBlock = new MemoryBlock(mbrData);
            PartitionInfos = new List<PartitionInfo>();

            LoadPartitionInfo();
            Initialized = true;
        }

        private void LoadPartitionInfo()
        {
            for (int i = 0x1BE; i < 0x1FE; i += 16)
            {
                bool _IsBootable = MBRDataBlock.Read8((uint)i) == 0x80;

                uint _LBA = MBRDataBlock.Read32((uint)(i + 8));
                uint _Size = MBRDataBlock.Read32((uint)(i + 12));

                if (_Size == 0 || _LBA == 0)
                    continue;

                PartitionInfos.Add(new PartitionInfo()
                {
                    IsBootable = _IsBootable,
                    LBA = _LBA,
                    Size = _Size,
                });
                Console.WriteLine("Partition: " + PartitionInfos.Count + " | Bootable: " + _IsBootable + " | LBA: " + _LBA + " | Size: " + _Size);
            }
        }
    }
}
