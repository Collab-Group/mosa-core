// Copyright (c) MOSA Project. Licensed under the New BSD License.
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System.Collections.Generic;

namespace Mosa.External.x86
{
    public readonly struct MemoryBlock
    {
        private readonly Pointer address;
        private readonly uint size;
        private readonly bool IsManaged;

        public Pointer Address { get { return address; } }

        public uint Size { get { return size; } }

        public MemoryBlock(Pointer address, uint size)
        {
            this.size = size;
            this.address = address;

            IsManaged = false;
        }

        public MemoryBlock(uint size)
        {
            this.size = size;

            address = GC.AllocateObject(size);
            IsManaged = true;
        }

        public MemoryBlock(byte[] data)
        {
            size = (uint)data.Length;

            address = GC.AllocateObject(size);
            IsManaged = true;

            for (int i = 0; i < data.Length; i++)
                Write8((uint)i, data[i]);
        }

        public MemoryBlock(List<byte> data)
        {
            size = (uint)data.Count;

            address = GC.AllocateObject(size);
            IsManaged = true;

            for (int i = 0; i < data.Count; i++)
                Write8((uint)i, data[i]);
        }

        public MemoryBlock(int[] data)
        {
            size = (uint)data.Length * 4;

            address = GC.AllocateObject(size);
            IsManaged = true;

            for (int i = 0; i < data.Length; i++)
                this[(uint)i] = data[i];
        }

        public MemoryBlock(List<int> data)
        {
            size = (uint)data.Count * 4;

            address = GC.AllocateObject(size);
            IsManaged = true;

            for (int i = 0; i < data.Count; i++)
                this[(uint)i] = data[i];
        }

        public void Free()
        {
            if (IsManaged)
                GC.Dispose((uint)address, size);
            this.Dispose();
            //GC.Dispose(this);
        }

        public int this[uint offset]
        {
            get { return (int)address.Load32(offset * 4); }
            set { address.Store32(offset * 4, value); }
        }

        public void Clear()
        {
            ASM.MEMFILL((uint)address, size, 0);
        }

        public void FlushToArray(byte[] dest)
        {
            for (uint i = 0; i < dest.Length; i++)
                dest[i] = Read8(i);
        }

        public byte Read8(uint offset)
        {
            return address.Load8(offset);
        }

        public void Write8(uint offset, byte value)
        {
            if (offset < size)
            {
                address.Store8(offset, value);
            }
        }

        public ushort Read16(uint offset)
        {
            return address.Load16(offset);
        }

        public void Write16(uint offset, ushort value)
        {
            if (offset < size)
            {
                address.Store16(offset, value);
            }
        }

        public uint Read24(uint offset)
        {
            return address.Load16(offset) | (uint)(address.Load8(offset + 2) << 16);
        }

        public void Write24(uint offset, uint value)
        {
            if (offset < size)
            {
                address.Store16(offset, (ushort)(value & 0xFFFF));
                address.Store8(offset + 2, (byte)((value >> 16) & 0xFF));
            }
        }

        public uint Read32(uint offset)
        {
            return address.Load32(offset);
        }

        public void Write32(uint offset, uint value)
        {
            if (offset < size)
            {
                address.Store32(offset, value);
            }
        }
    }
}

