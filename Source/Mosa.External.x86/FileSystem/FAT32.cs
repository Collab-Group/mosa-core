using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mosa.External.x86.FileSystem
//namespace MOSA2
{
    //Reference:https://blog.csdn.net/liyun123gx/article/details/38440225
    //Reference:https://blog.csdn.net/tq384998430/article/details/53414142
    public unsafe class FAT32
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct FAT32_DBR
        {
            public fixed byte BS_jmpBoot[3];        //跳转指令 offset: 0
            public fixed byte BS_OEMName[8];        //原始制造商 offset: 3
            public fixed byte BPB_BytesPerSec[2];   //每扇区字节数 offset:11
            public byte BPB_SecPerClus;             //每簇扇区数 offset:13
            public ushort BPB_RsvdSecCnt;           //保留扇区数目 offset:14
            public byte BPB_NumFATs;                //此卷中FAT表数 offset:16
            public ushort BPB_RootEntCnt;           //FAT32为0 offset:17
            public fixed byte BPB_TotSec16[2];      //FAT32为0 offset:19
            public byte BPB_Media;                  //存储介质 offset:21
            public fixed byte BPB_FATSz16[2];       //FAT32为 0 offset:22
            public fixed byte BPB_SecPerTrk[2];     //磁道扇区数 offset:24
            public fixed byte BPB_NumHeads[2];      //磁头数 offset:26
            public fixed byte BPB_HiddSec[4];       //FAT区前隐扇区数 offset:28
            public fixed byte BPB_TotSec32[4];      //该卷总扇区数 offset:32
            public uint BPB_FATSz32;                //一个FAT表扇区数 offset:36
            public fixed byte BPB_ExtFlags[2];      //FAT32特有 offset:40
            public fixed byte BPB_FSVer[2];         //FAT32特有 offset:42
            public uint BPB_RootClus;               //根目录簇号 offset:44
            public fixed byte FSInfo[2];            //保留扇区FSINFO扇区数 offset:48
            public fixed byte BPB_BkBootSec[2];     //通常为6 offset:50
            public fixed byte BPB_Reserved[12];     //扩展用 offset:52
            public byte BS_DrvNum;                  //offset:64
            public byte BS_Reserved1;               //offset:65
            public byte BS_BootSig;                 //offset:66
            public fixed byte BS_VolID[4];          //offset:67
            public fixed byte BS_FilSysType[11];    //offset:71
            public fixed byte BS_FilSysType1[8];    //"FAT32 " offset:82
        };


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DirectoryItem
        {
            public fixed byte Name[11];
            public byte Attribute;
            public byte Reserved;
            public byte CreateTimeTenMS;
            public ushort CreateTime;
            public ushort CreateDate;
            public ushort LastAccessDate;
            public ushort ClusterHigh;
            public ushort LastModifyTime;
            public ushort LastModifyDate;
            public ushort ClusterLow;
            public uint Size;
        }

        public class ADirectoryItem
        {
            public string Name;
            public string Parent;
            public DirectoryItem* Item;

            public uint FountAtSec;
            public uint FountAtOffset;

            public uint Cluster
            {
                get
                {
                    return (uint)(Item->ClusterHigh << 16 | Item->ClusterLow);
                }
            }
            public uint Size
            {
                get
                {
                    return Item->Size;
                }
            }

            public void Free()
            {
                GC.Free((uint)Item, (uint)sizeof(DirectoryItem));
                GC.DisposeObject(this);
            }
        }

        public struct Attributes
        {
            public const byte ReadWrite = 0x00;
            public const byte ReadOnly = 0x01;
            public const byte Hidden = 0x02;
            public const byte System = 0x04;
            public const byte VolumeLabel = 0x08;
            public const byte SubDirectory = 0x10;
            public const byte Archive = 0x20;
        }

        public struct Status
        {
            public const byte Empty = 0x00;
            public const byte Deleted = 0xE5;
            public const byte SpecialFile = 0x2E;
        }

        IDisk Disk;
        PartitionInfo Partition;

        FAT32_DBR* DBR;

        public const ushort SectorSize = 512;

        //Maybe
        //public uint DirectorySize = DBR->BPB_SecPerClus * SectorSize;

        public List<ADirectoryItem> Items;

        public FAT32(IDisk disk, PartitionInfo partition)
        {
            Disk = disk;
            Partition = partition;
            Items = new List<ADirectoryItem>();

            DBR = (FAT32_DBR*)ReadBlock(Partition.LBA, 1).Address;
            Console.WriteLine($"OEMName:{string.FromPointer(DBR->BS_OEMName, 8)}");
            Console.WriteLine($"FileSystemType:{string.FromPointer(DBR->BS_FilSysType1, 8)}");

            uint RootDirectorySector = GetSectorOffset(DBR->BPB_RootClus);
            Console.WriteLine($"Root Directory Sector:{RootDirectorySector}");

            Console.WriteLine($"Sectors Per Cluster:{DBR->BPB_SecPerClus}");

            ReadList(RootDirectorySector, "/");
        }

        public uint GetSectorOffset(uint Cluster)
        {
            return Partition.LBA + DBR->BPB_RsvdSecCnt + DBR->BPB_FATSz32 * DBR->BPB_NumFATs + (Cluster - 2) * DBR->BPB_SecPerClus;
        }

        public byte[] ReadAllBytes(string FileName)
        {
            if (FileName[0] != '/') FileName = "/" + FileName;

            foreach (var v in Items)
            {
                if (v.Parent + v.Name == FileName)
                {
                    uint count = GetSectorsWillUse(v.Size);
                    Console.WriteLine($"Count:{count}");
                    byte[] buffer = new byte[count * SectorSize];
                    Disk.ReadBlock(GetSectorOffset(v.Cluster), count, buffer);

                    byte[] result = new byte[v.Size];
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = buffer[i];
                    }

                    GC.DisposeObject(buffer);

                    return result;
                }
            }

            return null;
        }

        public uint GetSectorsWillUse(uint size)
        {
            uint result = 1;
            if (size > SectorSize)
            {
                result = size / SectorSize;
                if (size % SectorSize != 0)
                {
                    result++;
                }
            }
            return result;
        }

        public uint GetClustersWillUse(uint size)
        {
            uint result = 1;
            uint sectors = GetSectorsWillUse(size);
            if (sectors > DBR->BPB_SecPerClus)
            {
                result = sectors / DBR->BPB_SecPerClus;
                if (sectors % DBR->BPB_SecPerClus != 0)
                {
                    result++;
                }
            }
            return result;
        }

        public void WriteAllBytes(string FileName, byte[] Data)
        {
            if (FileName[0] != '/') FileName = "/" + FileName;

            foreach (var v in Items)
            {
                if (v.Parent + v.Name == FileName)
                {
                    byte[] _Buffer = new byte[512];
                    fixed (byte* _P = _Buffer)
                    {
                        Disk.ReadBlock(v.FountAtSec, 1, _Buffer);
                        DirectoryItem* _Item = (DirectoryItem*)(_P + v.FountAtOffset);
                        _Item->Size = (uint)Data.Length;
                        uint _Clus = (uint)(_Item->ClusterHigh << 16 | _Item->ClusterLow);

                        uint _CNT = (uint)(_Item->Size / IDE.SectorSize + ((_Item->Size % IDE.SectorSize) != 0 ? 1 : 0));
                        byte[] _B = new byte[IDE.SectorSize * _CNT];
                        for (int i = 0; i < Data.Length; i++) _B[i] = Data[i];
                        Disk.WriteBlock(GetSectorOffset(_Clus), _CNT, _B);

                        GC.DisposeObject(_B);
                        GC.DisposeObject(_Buffer);

                        return;
                    }
                }
            }

            uint SectorsWillUse = GetSectorsWillUse((uint)Data.Length);
            uint ClusterWillUse = GetClustersWillUse((uint)Data.Length);

            string Path = FileName.Substring(0, FileName.LastIndexOf('/') + 1);
            string Name = FileName.Substring(Path.Length);
            //Name must be like XXXX.XXX
            string[] vs = Name.Split('.');
            Console.WriteLine($"Path:{Path} Name:{Name}");

            uint Cluster = 3;
            uint TargetDirectorySector = 0;

            uint LastClusters = 0;
            uint LastClusterInItems = 0;

            foreach (var v in Items)
            {
                if (v.Cluster > LastClusterInItems)
                {
                    LastClusterInItems = v.Cluster;
                    LastClusters = GetClustersWillUse(v.Size);
                }

                if (Path != "/")
                {
                    if (v.Parent + v.Name + "/" == Path)
                    {
                        TargetDirectorySector = GetSectorOffset(v.Cluster);
                    }
                }
                else
                {
                    TargetDirectorySector = GetSectorOffset(DBR->BPB_RootClus);
                }
            }

            Cluster = LastClusterInItems;
            Cluster += LastClusters;

            DirectoryItem* item = (DirectoryItem*)GC.AllocateObject((uint)sizeof(DirectoryItem));
            ASM.MEMFILL((uint)item, 11, 0x20);
            for (int i = 0; i < 8; i++)
            {
                if (i > vs[0].Length - 1)
                {
                    item->Name[i] = 0x20;
                    continue;
                }
                item->Name[i] = (byte)vs[0][i];
            }
            for (int i = 0; i < 3; i++)
            {
                if (i > vs[1].Length - 1)
                {
                    item->Name[8 + i] = 0x20;
                    continue;
                }
                item->Name[8 + i] = (byte)vs[1][i];
            }
            item->Attribute = Attributes.Archive;
            item->ClusterLow = (ushort)(Cluster & 0xFFFF);
            item->ClusterHigh = (ushort)((Cluster >> 16) & 0xFFFF);
            item->Size = (uint)Data.Length;

            //WriteFile
            byte[] bufferToWrite = new byte[SectorSize * SectorsWillUse];
            for (int i = 0; i < Data.Length; i++)
            {
                bufferToWrite[i] = Data[i];
            }
            Disk.WriteBlock(GetSectorOffset(Cluster), SectorsWillUse, bufferToWrite);
            //WriteItem
            uint Index = 0;
            byte[] buffer = new byte[SectorSize];
        Retry:
            Disk.ReadBlock(TargetDirectorySector + Index, 1, buffer);
            for (int i = 0; i < buffer.Length; i += sizeof(DirectoryItem))
            {
                if (buffer[i] == Status.Empty)
                {
                    for (int k = 0; k < sizeof(DirectoryItem); k++)
                    {
                        buffer[i + k] = ((byte*)item)[k];
                    }
                    Disk.WriteBlock(TargetDirectorySector, 1, buffer);

                    //Disposing
                    GC.DisposeObject(buffer);
                    GC.DisposeObject(bufferToWrite);
                    GC.Free((uint)item, (uint)sizeof(DirectoryItem));

                    Items.Add(new ADirectoryItem()
                    {
                        Item = item,
                        Name = Name,
                        Parent = Path
                    });
                    return;
                }
            }
            Index++;
            goto Retry;
        }

        public bool Exist(string FileName)
        {
            string Path = "/";
            if (FileName[0] == '/')
            {
                Path = FileName.Substring(0, FileName.LastIndexOf('/') + 1);
            }
            string Name = FileName.Substring(Path.Length);

            foreach (var v in Items)
            {
                if (v.Name == Name && v.Parent == Path) return true;
            }

            return false;
        }

        public void ReadList(uint sector, string parent)
        {
            //When it is root directory
            if (parent == "/")
            {
                foreach (var v in Items)
                {
                    v.Free();
                }
                Items.Clear();
            }

            uint Index = 0;
            do
            {
                //Use Cluster Per Sector? 
                MemoryBlock block = ReadBlock(sector + Index, 1);
                for (uint i = 0; i < 512; i += 32)
                {
                    switch (Native.Get8((uint)(block.Address + i)))
                    {
                        case Status.Empty:
                            block.Free();
                            Console.WriteLine("Finish");
                            return;
                        case Status.SpecialFile:
                            continue;
                        case Status.Deleted:
                            continue;
                    }
                    DirectoryItem* item = (DirectoryItem*)GC.AllocateObject((uint)sizeof(DirectoryItem));
                    ASM.MEMCPY((uint)item, (uint)(block.Address + i), 32);

                    ADirectoryItem aDirectoryItem = new ADirectoryItem()
                    {
                        Item = item,
                        Parent = parent,

                        FountAtSec = sector + Index,
                        FountAtOffset = i
                    };
                    if (item->Attribute == Attributes.SubDirectory)
                    {
                        aDirectoryItem.Name = string.FromPointer(item->Name, 8, 0x20) + string.FromPointer(item->Name + 8, 3, 0x20);
                    }
                    else
                    {
                        aDirectoryItem.Name = string.FromPointer(item->Name, 8, 0x20) + "." + string.FromPointer(item->Name + 8, 3, 0x20);
                    }
                    Items.Add(aDirectoryItem);

                    if (item->Attribute == Attributes.SubDirectory)
                    {
                        //Maybe Bugs
                        ReadList(GetSectorOffset(Items[Items.Count - 1].Cluster), parent + string.FromPointer(item->Name, 8, 0x20) + "/");
                    }
                }
                block.Free();
            } while (Index++ != -1);
        }

        public MemoryBlock ReadBlock(uint sector, uint count)
        {
            byte[] buffer = new byte[SectorSize * count];
            Disk.ReadBlock(sector, count, buffer);
            MemoryBlock block = new MemoryBlock(buffer);
            GC.DisposeObject(buffer);
            return block;
        }
    }
}
