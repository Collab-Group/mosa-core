using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System;
using System.Runtime.InteropServices;
using static Mosa.Runtime.x86.Native;

namespace Mosa.External.x86.Driver.Audio
{
    struct Options
    {
        public const byte MasterVolume = 0x0002;
        public const byte GlobalControlStat = 0x0060;
    }

    struct PCM
    {
        public const byte OutBufferDescriptorBar = 0x10;
        public const byte OutLastValidIndex = 0x15;
        public const byte OutStatusRegister = 0x16;
        public const byte OutControlRegister = 0x1B;
    }

    public unsafe class AC97
    {
        private static uint NAM, NABM;

        public const ushort ListLength = 32;
        public const ushort BufferLength = 0xFFFE;

        public static bool Exists = false;

        public static byte max = 0;

        public static Pointer BufferListAddr;

        public static byte* Buffer;

        public static byte IRQ;

        public static unsafe void Initialize()
        {
            foreach (var device in PCI.Devices)
                if (device.ClassID == 0x04 && device.Subclass == 0x01)
                {
                    Console.WriteLine("AC97 device found!");

                    device.EnableDevice();
                    IRQ = device.InterruptLine;

                    Console.WriteLine($"IRQ: {IRQ}");

                    NAM = device.BaseAddressBar[0].BaseAddress;
                    NABM = device.BaseAddressBar[1].BaseAddress;

                    Out8((ushort)(NABM + (ushort)Options.GlobalControlStat), 0x02);

                    Out8((ushort)(NABM + (ushort)PCM.OutControlRegister), 0xF0);

                    BufferListAddr = GC.AllocateObject((uint)(ListLength * sizeof(BufferDescriptor)));

                    Out32((ushort)(NAM + (ushort)Options.MasterVolume), 0x2020);

                    Buffer = (byte*)GC.AllocateObject(1024 * 1024);

                    Console.WriteLine("Successfully initialized the AC97 device!");
                    Exists = true;
                }
        }

        private static int Status;
        public static bool Finished { get => Status == 7; }

        public static void OnInterrupt()
        {
            if (!Exists) return;

            Status = In16((ushort)(NABM + PCM.OutStatusRegister));
            if (Status != 0) Out16((ushort)(NABM + PCM.OutStatusRegister), (ushort)(Status & 0x1E));
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BufferDescriptor
        {
            public uint Addr;
            public ushort Size;
            public ushort Attr;
        }

        //48Khz DualChannel
        public static unsafe void Play(byte[] Data)
        {
            if (!Exists) return;

            int k = 0;

            fixed (byte* P = Data) ASM.MEMCPY((uint)Buffer, (uint)P, (uint)Data.Length);

            for (uint i = 0; i < Math.Clamp(Data.Length, 0, 1024 * 1024) - (Math.Clamp(Data.Length, 0, 1024 * 1024) % BufferLength); i += BufferLength * 2)
            {
                BufferDescriptor* desc = (BufferDescriptor*)(BufferListAddr + (sizeof(BufferDescriptor) * k));
                desc->Addr = (uint)(Buffer + i);
                desc->Size = BufferLength;
                desc->Attr = 0b0000_0000_0000_0011;
                k++;
            }

            if (k > 0) k--;
            max = (byte)(k & 0xFF);

            Out8((ushort)(NABM + (ushort)PCM.OutControlRegister), 0x2);

            Out32((ushort)(NABM + (ushort)PCM.OutBufferDescriptorBar), (uint)BufferListAddr);

            SetIndex(max);

            GC.DisposeObject(Data);

            Play();
        }

        private static void SetIndex(byte index)
        {
            Out8((ushort)(NABM + (ushort)PCM.OutLastValidIndex), index);
        }

        private static void Play()
        {
            Out8((ushort)(NABM + (ushort)PCM.OutControlRegister), 0x11);
        }
    }
}
