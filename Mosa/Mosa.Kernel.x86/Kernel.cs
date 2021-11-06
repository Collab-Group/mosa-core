// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.x86.Smbios;
using Mosa.Runtime;
using Mosa.Runtime.Plug;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mosa.Kernel.x86
{
    /// <summary>
    /// X86 Kernel
    /// </summary>
    public static class Kernel
    {
        private static void InitializeACPI() { }

        [Plug("Mosa.Runtime.StartUp::KernelEntryPoint")]
        public static void Setup()
        {
            Console.Setup();

            IDT.SetInterruptHandler(null);

            // Initialize GDT before IDT, because IDT Entries requires a valid Segment Selector
            Multiboot.Setup();
            GDT.Setup();

            // At this stage, allocating memory does not work, so you are only allowed to use ValueTypes or static classes.
            Panic.Setup();

            // Initialize interrupts
            PIC.Setup();
            IDT.Setup();
            PIT.Setup();

            // Initializing the memory management
            PageFrameAllocator.Setup();
            PageTable.Setup();
            VirtualPageAllocator.Setup();
            //PAE.Setup();
            GC.Setup(Address.GCMemoryDescriptors);

            IDT.INTs = new List<IDT.INT>();

            ThreadPool.Setup();
            SmbiosManager.Setup();

            // Setup PCI
            PCI.Setup();
            
            // Setup Encoding static variables
            Encoding.Setup();

            //Mosa.External.x86.Driver.ACPI.Initialize
            InitializeACPI();

            ThreadPool.Setup();
            ThreadPool.CreateThread((() =>
            {
                StartUp.KMain();
            }), PageFrameAllocator.PageSize);
            ThreadPool.Start();
        }
    }
}
