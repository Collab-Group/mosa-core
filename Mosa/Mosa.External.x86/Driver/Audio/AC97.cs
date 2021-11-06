using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System;
using System.Runtime.InteropServices;
using static Mosa.Runtime.x86.Native;

namespace Mosa.External.x86.Driver.Audio
{
    public unsafe class AC97
    {
        private static uint NAM, NABM;

        private const ushort ListLength = 32;
        private const ushort BufferLength = 0xFFFE;

        public static bool Probe = false;

        private static Pointer BufferListAddr;

        private static byte* Buffer;

        //Issue: This Code Only Works On Virtual Box
        public static unsafe void Initialize()
        {
            PCIDevice device = PCI.GetDevice(VendorID.Intel, (DeviceID)0x2415);

            if (device == null) return;

            Console.WriteLine("Intel ICH AC97 Audio Controller Found");

            device.EnableDevice();
            Console.WriteLine($"INT:{device.InterruptLine}");

            NAM = device.BAR0 & ~(0xFU);
            NABM = device.BAR1 & ~(0xFU);

            IDT.INTs.Add(new IDT.INT(0x20u, OnInterrupt));

            //Reset
            Out32((ushort)(NABM + 0x2C), 0x00000002);
            Out16((ushort)NAM, 54188);

            BufferListAddr = GC.AllocateObject((uint)(ListLength * sizeof(BufferDescriptor)));
            Buffer = (byte*)GC.AllocateObject(1024 * 1024);

            Out16((ushort)(NAM + 0x02), 0x0F0F);
            Out16((ushort)(NAM + 0x18), 0x0F0F);
            Out16((ushort)(NAM + 0x2C), 48000);

            Probe = true;
        }

        private static int Status;

        private static void OnInterrupt()
        {
            if (!Probe) return;

            Status = In16((ushort)(NABM + 0x16));
            if (Status != 0) Out16((ushort)(NABM + 0x16), (ushort)(Status & 0x1E));
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BufferDescriptor
        {
            public uint Addr;
            public ushort Size;
            public ushort Attr;
        }

        //48Khz DualChannel
        public static unsafe void Play(byte[] PCM)
        {
            if (!Probe) return;

            int k = 0;

            fixed (byte* P = PCM) ASM.MEMCPY((uint)Buffer, (uint)P, (uint)PCM.Length);

            for (uint i = 0; i < Math.Clamp(PCM.Length, 0, 1024 * 1024) - (Math.Clamp(PCM.Length, 0, 1024 * 1024) % BufferLength); i += BufferLength * 2)
            {
                BufferDescriptor* desc = (BufferDescriptor*)(BufferListAddr + (sizeof(BufferDescriptor) * k));
                desc->Addr = (uint)(Buffer + i);
                desc->Size = BufferLength;
                desc->Attr = 0b0000_0000_0000_0011;
                k++;
            }

            if (k > 0) k--;
            Out8((ushort)(NABM + (ushort)0x1B), 0x2);

            Out32((ushort)(NABM + (ushort)0x10), (uint)BufferListAddr);

            //SetIndex
            Out8((ushort)(NABM + (ushort)0x15), (byte)(k & 0xFF));

            PCM.Dispose();

            //Play
            Out8((ushort)(NABM + (ushort)0x1B), 0x11);
        }
    }
}
