﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.Plug;
using System.Threading;
using System.Runtime.InteropServices;

namespace $safeprojectname$
{
    public static class Program
    {
        public static void Main() { }

        [Plug("Mosa.Runtime.StartUp::KMain")]
        [UnmanagedCallersOnly(EntryPoint = "KMain", CallingConvention = CallingConvention.StdCall)]
        public static void KMain()
        {
            IDT.OnInterrupt += IDT_OnInterrupt;
            new Thread(MainThread).Start();
            for (; ; );
        }

        private static void IDT_OnInterrupt(uint irq, uint error)
        {
            switch (irq)
            {
                case 0x21:
                    PS2Keyboard.OnInterrupt();
                    break;
            }
        }

        public static void MainThread()
        {
            // Initialize the IDE hard drive
            // MOSA currently only supports FAT12 ( obsolete ) and FAT32
            //IDisk disk = new IDEDisk();
            //MBR mBR = new MBR();
            //mBR.Initialize(disk);
            //FAT12 fs = new FAT12(disk, mBR.PartitionInfos[0]);

            Console.WriteLine("MOSA booted successfully! Type anything and get an echo of what you've typed.");

            for (; ; )
            {
                Console.WriteLine(Console.ReadLine());
            }
        }
    }
}
