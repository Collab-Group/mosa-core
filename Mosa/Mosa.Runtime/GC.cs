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
                for (uint u = 0; u < DescriptorsSize; u += (2 * sizeof(uint)))
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
        private const uint DescriptorsNumber = 4096;
        private static bool READY = false;

        public static void Setup()
        {
            DescriptorsSize = DescriptorsNumber * (2 * sizeof(uint));
            DescriptorsAddress = (uint)AllocateMemory(DescriptorsSize);

            for (uint u = 0; u < DescriptorsSize; u += (2 * sizeof(uint)))
            {
                ((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Address = 0;
                ((FreeMemoryDescriptor*)(DescriptorsAddress + u))->Size = 0;
            }

            READY = true;
        }

        [Obsolete("use object.Dispose()")]
        public static void Dispose(object obj)
        {
            // An object has the following memory layout:
            //   - Pointer TypeDef
            //   - Pointer SyncBlock
            //   - 0 .. n object data fields
            if (obj == null) return;

            uint Address = (uint)Intrinsic.GetObjectAddress(obj);
            //                   ///                      Size Of Object Data                ///Size Of  TypeDef And SyncBlock///              
            uint Size = (uint)((*((uint*)(obj.GetType().TypeHandle.Value + (Pointer.Size * 3)))) + 2 * sizeof(Pointer));
            Dispose(Address, Size);
        }

        public static void Dispose(uint Address, uint Size)
        {
            for (uint u = 0; u < DescriptorsSize; u += (2 * sizeof(uint)))
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
