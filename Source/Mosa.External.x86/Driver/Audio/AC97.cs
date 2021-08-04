using Mosa.External.x86;
using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime;
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

        public static bool Exsist = false;

        public static byte index = 0;
        public static byte max = 0;

        public static Pointer BufferListAddr;

        public static unsafe void Initialize()
        {
            foreach (var v in PCI.Devices)
            {
                if (v.ClassID == 0x04 && v.Subclass == 0x01)
                {
                    Console.WriteLine("AC97 Device Found");

                    v.EnableDevice();

                    NAM = v.BaseAddressBar[0].BaseAddress;
                    NABM = v.BaseAddressBar[1].BaseAddress;


                    Out8((ushort)(NABM + (ushort)Options.GlobalControlStat), 0x02);

                    //                                                   11110000
                    Out8((ushort)(NABM + (ushort)PCM.OutControlRegister), 0xF0);

                    BufferListAddr = GC.AllocateObject((uint)(ListLength * sizeof(BufferDescriptor)));

                    Out32((ushort)(NAM + (ushort)Options.MasterVolume), 0x2020);

                    Console.WriteLine("AC97 Initialized");
                    Exsist = true;
                }
            }
        }

        public static ushort status = 0;

        public static void OnInterrupt()
        {
            if (!Exsist) return;

            status = In16((ushort)(NABM + PCM.OutStatusRegister));
            if (status != 0) Out16((ushort)(NABM + PCM.OutStatusRegister), (ushort)(status & 0x1E));

            if (status == 7)
            {
                index = (byte)((index + 1) % (max - 1));
                UpdateIndex();
            }

            Console.WriteLine($"Status:{status}");
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BufferDescriptor
        {
            public uint Addr;
            public ushort Size;
            public ushort Attr;
        }

        //48Khz DualChannel
        public static unsafe void Play(MemoryBlock PCMRawData)
        {
            if (!Exsist) return;

            uint k = 0;
            for (uint i = 0; i < PCMRawData.Size - (PCMRawData.Size % BufferLength); i += BufferLength * 2)
            {
                BufferDescriptor* desc = (BufferDescriptor*)(BufferListAddr + (sizeof(BufferDescriptor) * k));
                desc->Addr = (uint)(PCMRawData.Address + i);
                desc->Size = BufferLength;
                desc->Attr = 0b0000_0000_0000_0011;
                k++;
            }
            max = (byte)(k & 0xFF);

            Out8((ushort)(NABM + (ushort)PCM.OutControlRegister), 0x2);
            PIT.Wait(10);

            Out32((ushort)(NABM + (ushort)PCM.OutBufferDescriptorBar), (uint)BufferListAddr);

            index = 0;
            UpdateIndex();

            Play();
        }

        private static void UpdateIndex()
        {
            Out8((ushort)(NABM + (ushort)PCM.OutLastValidIndex), index);
        }

        private static void Play()
        {
            Out8((ushort)(NABM + (ushort)PCM.OutControlRegister), 0x11);
        }
    }
}
