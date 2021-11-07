// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mosa.Runtime
{
    public unsafe static class GC
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MemoryDescriptor
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
        public static ulong TotalFullUsed = 0;
        public static Pointer TotalAllocPtr;
        public static TypeCode TotalAllocType;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Pointer AllocateObject(uint size, TypeCode type = TypeCode.Object)
        {
            TotalAllocSize = size;
            TotalAllocType = type;

            if (READY)
            {
                for (int i = 0; i < DescriptorsNumber; i++)
                {
                    if ((&MemoryDescriptors[i])->Size >= size)
                    {
                        Pointer RESULT = new Pointer((&MemoryDescriptors[i])->Address);

                        TotalReuse++;

                        //Clear
                        (&MemoryDescriptors[i])->Size -= size;
                        (&MemoryDescriptors[i])->Address += size;

                        //Not sure
                        if((&MemoryDescriptors[i])->Size <= sizeof(ulong)) 
                        {
                            (&MemoryDescriptors[i])->Size = 0;
                        }

                        if ((&MemoryDescriptors[i])->Size == 0) TotalFullUsed++;

                        return RESULT;
                    }
                }
            }

            TotalAlloc++;

            TotalAllocPtr = AllocateMemory(size);

            return TotalAllocPtr;
        }

        public static MemoryDescriptor* MemoryDescriptors;
        private const uint DescriptorsNumber = 0x20000;
        private static bool READY = false;

        public static void Setup(uint DescriptorStartAddress)
        {
            MemoryDescriptors = (MemoryDescriptor*)DescriptorStartAddress;

            for (int i = 0; i < DescriptorsNumber; i++)
            {
                (&MemoryDescriptors[i])->Size = 0;
                (&MemoryDescriptors[i])->Address = 0;
            }

            READY = true;
        }

        public static void Dispose(uint Address, uint Size)
        {
            for (int i = 0; i < DescriptorsNumber; i++)
            {
                if ((&MemoryDescriptors[i])->Size == 0)
                {
                    Internal.MemoryClear((Pointer)Address, Size);
                    (&MemoryDescriptors[i])->Size = Size;
                    (&MemoryDescriptors[i])->Address = Address;
                    break;
                }
            }
        }
    }
}
