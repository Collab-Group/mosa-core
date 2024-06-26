// Copyright (c) MOSA Project. Licensed under the New BSD License.
namespace Mosa.External.x86.FileSystem
{
    public interface IDisk
    {
        bool ReadBlock(uint sector, uint count, byte[] data);
        bool WriteBlock(uint sector, uint count, byte[] data);
    }
}

