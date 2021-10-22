// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.Plug;
using System.Threading;

namespace $safeprojectname$
{
    public static class Program
    {
        public static string Input = "";

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
            // MOSA currently only supports FAT12 and FAT32
            //IDisk disk = new IDEDisk();
            //MBR mBR = new MBR();
            //mBR.Initialize(disk);
            //FileSystem Takes Time
            //FAT12 fs = new FAT12(disk, mBR.PartitionInfos[0]);
            //byte[] b = fs.ReadAllBytes("TEST1.TXT");

            Console.WriteLine("MOSA booted successfully! Type anything and get an echo of what you've typed.");

            PS2Keyboard.KeyCode keyCode;
            for (; ; )
            {
                keyCode = PS2Keyboard.GetKeyPressed();
                switch (keyCode)
                {
                    case PS2Keyboard.KeyCode.Delete:
                        if (Input.Length != 0)
                        {
                            Console.RemovePreviousOne();
                            Input = Input.Substring(0, Input.Length - 1);
                        }
                        break;

                    case PS2Keyboard.KeyCode.Enter:
                        Console.WriteLine();
                        Console.WriteLine("Input: " + Input);
                        Input = "";
                        break;

                    default:
                        if (PS2Keyboard.IsCapsLock)
                        {
                            Console.Write(PS2Keyboard.KeyCodeToString(keyCode));
                            Input += PS2Keyboard.KeyCodeToString(keyCode);
                        }
                        else
                        {
                            Console.Write(PS2Keyboard.KeyCodeToString(keyCode).ToLower());
                            Input += PS2Keyboard.KeyCodeToString(keyCode).ToLower();
                        }
                        break;
                }
            }
        }
    }
}
