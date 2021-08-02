using Mosa.External.x86;
using Mosa.External.x86.Driver;
using Mosa.External.x86.FileSystem;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System.Collections.Generic;
using System.Text;

namespace Mosa.External.x86.FileSystem
{
    public unsafe class FAT12
    {
        struct FAT12Header
        {
            public string OEMName;
            public ushort RootEntryCount;
            public byte NumberOfFATs;
            public ushort SectorsPerFATs;
            public byte SectorsPerCluster;
            public ushort ResvdSector;
        }

        public struct FileInfo
        {
            public string Name;
            public bool IsDirectory;
            public ushort Cluster;
            public uint Size;
            public string ParentPath;
        }

        public IDisk Disk;

        public uint fileListSectorLength;
        public uint fileListSector0ffset;
        public uint fileAreaSectorOffset;
        public PartitionInfo partitionInfo;

        FAT12Header fAT12Header;

        public FAT12(IDisk disk, PartitionInfo _partitionInfo)
        {
            Disk = disk;
            partitionInfo = _partitionInfo;

            byte[] header = new byte[IDE.SectorSize];
            disk.ReadBlock(partitionInfo.LBA, 1, header);

            MemoryBlock memoryBlock = new MemoryBlock(header);

            fAT12Header = new FAT12Header() { OEMName = "" };
            for (int i = 0; i < 8; i++)
                fAT12Header.OEMName += Encoding.ASCII.GetChar(memoryBlock.Read8((uint)(0x3 + i)));

            if (fAT12Header.OEMName == "")
                Console.WriteLine("FAT12 OEM name is empty.");
            else
                Console.WriteLine("FAT12 OEM: " + fAT12Header.OEMName);

            fAT12Header.SectorsPerCluster = memoryBlock.Read8(0xD);
            fAT12Header.RootEntryCount = memoryBlock.Read16(0x11);
            fAT12Header.NumberOfFATs = memoryBlock.Read8(0x10);
            fAT12Header.SectorsPerFATs = memoryBlock.Read16(0x16);
            fAT12Header.ResvdSector = memoryBlock.Read16(0x0e);

            fileListSectorLength = (uint)((fAT12Header.RootEntryCount * FAT12Item.SizePerItem + (IDE.SectorSize - 1)) / IDE.SectorSize);

            /*
             * |    Boot   |
             * |    FAT1   |
             * |    FAT2   |
             * | Directory |
             */
            fileListSector0ffset = (uint)(partitionInfo.LBA + fAT12Header.ResvdSector + fAT12Header.SectorsPerFATs * 2);

            fileAreaSectorOffset = (fAT12Header.ResvdSector + ((uint)fAT12Header.NumberOfFATs * fAT12Header.SectorsPerFATs) + ((fAT12Header.RootEntryCount * 32u) / IDE.SectorSize));
        }

        public bool Exists(string Name)
        {
            Name = Name.ToUpper();

            string Path;
            string FileName;

            if (Name.IndexOf('/') == -1)
            {
                Path = "/";
                FileName = Name;
            }
            else
            {
                Path = Name.Substring(0, Name.LastIndexOf('/') + 1);
                FileName = Name.Substring(Path.Length);
            }

            foreach (var v in GetFiles("/"))
                if (v.Name == FileName && v.ParentPath == Path)
                    return true;

            return false;
        }

        public byte[] ReadAllBytes(string Name)
        {
            Name = Name.ToUpper();

            string Path;
            string FileName;

            if (Name.IndexOf('/') == -1)
            {
                Path = "/";
                FileName = Name;
            }
            else
            {
                Path = Name.Substring(0, Name.LastIndexOf('/') + 1);
                FileName = Name.Substring(Path.Length);
            }

            FileInfo fileInfo = new FileInfo() { Name = "" };
            foreach (var v in GetFiles("/"))
                if (v.Name == FileName && v.ParentPath == Path)
                    fileInfo = v;

            if (fileInfo.Name == "")
                return null;

            uint count;
            if (fileInfo.Size <= IDE.SectorSize)
                count = 1;
            else
            {
                if (fileInfo.Size % IDE.SectorSize != 0)
                    count = fileInfo.Size / IDE.SectorSize + 1;
                else
                    count = fileInfo.Size / IDE.SectorSize;
            }

            byte[] data = new byte[IDE.SectorSize * count];
            Disk.ReadBlock(GetFileSectorOffset(fileInfo.Cluster), count, data);

            byte[] result = new byte[fileInfo.Size];

            for (int i = 0; i < fileInfo.Size; i++)
                result[i] = data[i];

            GC.DisposeObject(data);

            return result;
        }

        public uint GetFileSectorOffset(uint Cluster)
        {
            return partitionInfo.LBA + fileAreaSectorOffset + ((Cluster - 2) * fAT12Header.SectorsPerCluster);
        }

        public List<FileInfo> GetFiles(string path)
        {
            return ReadFileList(fileListSector0ffset, path);
        }

        public List<FileInfo> ReadFileList(uint startSector, string parentPath)
        {
            List<FileInfo> FileInfos = new List<FileInfo>();

            byte[] data = new byte[fileListSectorLength * IDE.SectorSize];
            Disk.ReadBlock(startSector, fileListSectorLength, data);

            uint T = 0;
            byte[] _data = new byte[FAT12Item.SizePerItem];

            for (; ; )
            {
                for (uint u = 0; u < FAT12Item.SizePerItem; u++)
                    _data[u] = data[u + T];

                T += FAT12Item.SizePerItem;

                byte b = _data[0];

                if (b == 0xE5 || b == 0x2E)
                    continue;

                if (_data[0] == 0x00)
                    break;

                FileInfo fileInfo = GetFileInfo(_data, parentPath);

                /*if (!fileInfo.IsDirectory)
                {
                    if (fileInfo.Size == 0 || fileInfo.Size == uint.MaxValue || fileInfo.Cluster == 0)
                        continue;
                }*/

                if (fileInfo.IsDirectory)
                {
                    uint offset = (uint)(partitionInfo.LBA + fileAreaSectorOffset + ((fileInfo.Cluster - 2) * fAT12Header.SectorsPerCluster));
                    ReadFileList(offset, parentPath + fileInfo.Name + "/");
                }

                FileInfos.Add(fileInfo);

                //GC.DisposeObject(fileInfo);
            }

            return FileInfos;
        }

        private FileInfo GetFileInfo(byte[] _data, string parentPath)
        {
            FileInfo fileInfo = new FileInfo() { Name = "", IsDirectory = false };

            // Name
            for (int i = 0; i < 8; i++)
            {
                if (_data[i] == 0x20)
                    break;

                fileInfo.Name += Encoding.ASCII.GetChar(_data[i]);
            }

            if (_data[8] != 0x20)
                fileInfo.Name += ".";

            // Extension
            for (int i = 8; i < 11; i++)
            {
                if (_data[i] == 0x20)
                    break;

                fileInfo.Name += Encoding.ASCII.GetChar(_data[i]);
            }

            // Type
            if (_data[0xB] == 0x10)
                fileInfo.IsDirectory = true;

            // Cluster
            ushort Cluster = (ushort)(_data[26] | _data[27] << 8);
            fileInfo.Cluster = Cluster;

            // Size
            uint Size = (uint)(_data[28] | _data[29] << 8 | _data[30] << 16 | _data[31] << 24);
            fileInfo.Size = Size;

            // Path
            fileInfo.ParentPath = parentPath;

            return fileInfo;
        }

        public void WriteAllBytes(string Name, byte[] Buffer)
        {
            Name = Name.ToUpper();

            string Path;
            string FileName;

            if (Name.IndexOf('/') == -1)
            {
                Path = "/";
                FileName = Name;
            }
            else
            {
                Path = Name.Substring(0, Name.LastIndexOf('/') + 1);
                FileName = Name.Substring(Path.Length);
            }

            FileInfo fileInfo = new FileInfo() { Name = "" };
            foreach (var v in GetFiles("/"))
                if (v.Name == FileName && v.ParentPath == Path)
                    fileInfo = v;

            if (fileInfo.Name == "")
                return;

            Disk.WriteBlock(fileListSector0ffset, 1, Buffer);

            byte[] buf = new byte[IDE.SectorSize];
            for (int i = 0; i < Buffer.Length; i++)
                buf[i] = Buffer[i];

            Disk.WriteBlock(GetFileSectorOffset(fileInfo.Cluster), 1, buf);
        }

        public void CreateFile(string FileName, byte[] Buffer)
        {
            if (Exists(FileName))
                return;

            FileName = FileName.ToUpper();

            string Path;
            string Name;
            string Extension;

            if (FileName.IndexOf('/') == -1)
            {
                Path = "/";
                Name = FileName;
            }
            else
            {
                Path = FileName.Substring(0, FileName.LastIndexOf('/') + 1);
                Name = FileName.Substring(Path.Length);
            }

            string[] array = Name.Split('.');
            Name = array[0];
            Extension = array[1];

            byte[] Data = new byte[IDE.SectorSize];
            Disk.ReadBlock(fileListSector0ffset, 1, Data);

            MemoryBlock block = new MemoryBlock(Data);

            uint offset = 0;
            while (block.Read8(offset) != 0)
            {
                if (offset == Data.Length)
                    break;

                offset += FAT12Item.SizePerItem;
            }

            ushort cluster = 0;
            foreach (var v in GetFiles("/"))
                if (v.Cluster > cluster && !v.IsDirectory && v.ParentPath == Path)
                    cluster = v.Cluster;

            cluster++;

            FAT12Item item = new FAT12Item
            {
                Name = Name,
                Extension = Extension,
                Attribute = FAT12Item.ItemAttribute.File,
                Cluster = cluster,
                Size = (uint)Buffer.Length
            };

            ASM.MEMCPY((uint)(block.Address + offset), (uint)item.Ptr, FAT12Item.SizePerItem);
            block.FlushToArray(Data);
            Disk.WriteBlock(fileListSector0ffset, 1, Data);

            byte[] buf = new byte[IDE.SectorSize];
            for (int i = 0; i < Buffer.Length; i++)
                buf[i] = Buffer[i];

            Disk.WriteBlock(GetFileSectorOffset(cluster), 1, buf);
        }

        public class FAT12Item
        {
            public const byte NameOffset = 0;
            public const byte ExtensionOffset = 8;
            public const byte AttributeOffset = 11;
            public const byte ReservedOffset = 12;
            public const byte LastWriteTimeOffset = 22;
            public const byte LastWriteDateOffset = 24;
            public const byte ClusterOffset = 26;
            public const byte SizeOffset = 28;

            public const byte NameLength = 8;
            public const byte ExtensionLength = 3;

            public const byte SizePerItem = 32;

            public Pointer Ptr;

            public enum ItemAttribute : byte
            {
                File = 0x20,
                Directory = 0x10
            }

            public FAT12Item()
            {
                Ptr = GC.AllocateObject(SizePerItem);
                ASM.MEMFILL((uint)Ptr, SizePerItem, 0);
            }

            public string Name
            {
                set
                {
                    for (int i = 0; i < NameLength; i++)
                    {
                        if (i >= value.Length)
                        {
                            Ptr.Store8(NameOffset + i, 0x20);
                            continue;
                        };
                        Ptr.Store8(NameOffset + i, (byte)value[i]);
                    }
                }
            }

            public string Extension
            {
                set
                {
                    for (int i = 0; i < ExtensionLength; i++)
                    {
                        if (i >= value.Length)
                        {
                            Ptr.Store8(ExtensionOffset + i, 0x20);
                            continue;
                        }
                        Ptr.Store8(ExtensionOffset + i, (byte)value[i]);
                    }
                }
            }

            public ItemAttribute Attribute
            {
                set
                {
                    Ptr.Store8(AttributeOffset, (byte)value);
                }
            }

            public ushort Cluster
            {
                set
                {
                    Ptr.Store16(ClusterOffset, value);
                }
            }

            public uint Size
            {
                set
                {
                    Ptr.Store32(SizeOffset, value);
                }
            }
        }
    }
}