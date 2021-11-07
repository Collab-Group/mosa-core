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

            for (int i = 0; i < DescriptorsNumber; i++)
            {
                if ((&MemoryDescriptors[i])->Size >= size)
                {
                    Pointer RESULT = new Pointer((&MemoryDescriptors[i])->Address);

                    TotalReuse++;

                    //Clear
                    (&MemoryDescriptors[i])->Size -= size;
                    (&MemoryDescriptors[i])->Address += size;

                    //The size of object won't less than 16
                    if ((&MemoryDescriptors[i])->Size < 16)
                    {
                        (&MemoryDescriptors[i])->Size = 0;
                    }

                    if ((&MemoryDescriptors[i])->Size == 0) TotalFullUsed++;

                    return RESULT;
                }
            }

            TotalAlloc++;
            TotalAllocPtr = AllocateMemory(size);
            return TotalAllocPtr;
        }

        public static MemoryDescriptor* MemoryDescriptors;
        public static uint DescriptorsNumber = 0;

        public static void Setup(uint DescriptorStartAddress)
        {
            MemoryDescriptors = (MemoryDescriptor*)DescriptorStartAddress;
        }

        public static int Index = 0;

        public static void Dispose(uint Address, uint Size)
        {
            Index = 0;
            for (; Index < DescriptorsNumber; Index++)
            {
                if ((&MemoryDescriptors[Index])->Size == 0)
                {
                    DoDispose(Address, Size, Index);
                    return;
                }
            }
            DoDispose(Address, Size, Index);
            DescriptorsNumber++;
            return;
        }

        private static void DoDispose(uint Address, uint Size, int Index)
        {
            Internal.MemoryClear((Pointer)Address, Size);
            (&MemoryDescriptors[Index])->Size = Size;
            (&MemoryDescriptors[Index])->Address = Address;
        }
    }
}
