using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System;

namespace Mosa.External.x86
{
    public static class Memory
    {
        public static MemoryBlock GetPhysicalMemory(Pointer address, uint size)
        {
            var start = (uint)address.ToInt32();

            for (var at = start; at < start + size; at += PageFrameAllocator.PageSize)
            {
                PageTable.MapVirtualAddressToPhysical(at, at);
            }

            return new MemoryBlock(address, size);
        }

		public unsafe static uint GetAvailableMemory()
		{
			return ((PageFrameAllocator.TotalPages - PageFrameAllocator.TotalPagesInUse) * PageFrameAllocator.PageSize) + GC.GCFreeMemory();
		}
	}
}
