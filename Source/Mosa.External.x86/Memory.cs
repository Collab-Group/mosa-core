using Mosa.Kernel.x86;
using Mosa.Runtime;

namespace Mosa.External.x86
{
    public static class Memory
    {
        public static MemoryBlock GetPhysicalMemory(Pointer address, uint size)
        {
            MMIO.Map((uint)address, size);
            return new MemoryBlock(address, size);
        }

		public unsafe static uint GetAvailableMemory()
		{
            return ((PageFrameAllocator.TotalPages - PageFrameAllocator.TotalPagesInUse) * PageFrameAllocator.PageSize);
		}
	}
}
