// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mosa.Runtime
{
    public unsafe static class GC
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FreeMemoryDescriptor
        {
            public uint Address;
            public uint Size;
        }

        // This method will be plugged by the platform specific implementation;
        // On x86, it is be Mosa.Kernel.x86.KernelMemory._AllocateMemory
        private static Pointer AllocateMemory(uint size)
        {
            return Pointer.Zero;
        }

        public static ulong TotalAlloc = 0;
        public static ulong TotalReuse = 0;
        public static ulong TotalAllocSize = 0;
        public static Pointer TotalAllocPtr;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Pointer AllocateObject(uint size)
        {
            TotalAllocSize = size;

            if (READY)
            {
                for (int u = 0; u < DescriptorsSize; u += sizeof(FreeMemoryDescriptor))
                {
                    if (((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Size >= size)
                    {
                        Pointer RESULT = new Pointer(((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Address);

                        TotalReuse++;

                        //Clear
                        ((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Size -= size;
                        ((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Address += size;

                        return RESULT;
                    }
                }
            }

            TotalAlloc++;

            TotalAllocPtr = AllocateMemory(size);

            return TotalAllocPtr;
        }

        private static uint DescriptorsAddress;
        private static uint DescriptorsSize;
        private const uint DescriptorsNumber = 0x20000;
        private static bool READY = false;

        public static void Setup(uint DescriptorStartAddress)
        {
            DescriptorsSize = (uint)(DescriptorsNumber * sizeof(FreeMemoryDescriptor));
            DescriptorsAddress = DescriptorStartAddress;

            for (int u = 0; u < DescriptorsSize; u += sizeof(FreeMemoryDescriptor))
            {
                ((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Address = 0;
                ((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Size = 0;
            }

            READY = true;
        }

        public static void Dispose(uint Address, uint Size)
        {
            for (int u = 0; u < DescriptorsSize; u += sizeof(FreeMemoryDescriptor))
            {
                if (((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Size == 0)
                {
                    Internal.MemoryClear((Pointer)Address, Size);
                    ((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Address = Address;
                    ((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Size = Size;
                    break;
                }
            }
        }
    }
}
