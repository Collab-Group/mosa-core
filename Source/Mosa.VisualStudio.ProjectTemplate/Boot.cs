// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86.Driver;
using Mosa.External.x86.FileSystem;
using Mosa.Kernel.x86;

namespace $safeprojectname$
{
    public static class Boot
    {
        public static string Input = "";

        public static void Main()
        {
            // Initialize the necessary stuff
            Kernel.Setup();
            //ACPI.Init();
            IDT.SetInterruptHandler(ProcessInterrupt);

            // Note: threads can't be created dynamically (for now)
            Scheduler.CreateThread(MainThread, PageFrameAllocator.PageSize);
            Scheduler.Start();
        }

        public static void MainThread()
        {
            // Initialize the PS/2 keyboard
            PS2Keyboard.Initialize();

            // Initialize the IDE hard drive
            // MOSA currently only supports FAT12 and FAT32 (but FAT32 doesn't work correctly in VirtualBox for now)
            //IDisk disk = new IDEDisk();
            //MBR mBR = new MBR();
            //mBR.Initialize(disk);
            //FileSystem Takes Time
            //FAT12 fs = new FAT12(disk, mBR.PartitionInfos[0]);
            //byte[] b = fs.ReadAllBytes("TEST1.TXT");

            Console.WriteLine("MOSA booted successfully! Type anything and get an echo of what you've typed.");

            // Type anything and get an echo of what you've typed
            PS2Keyboard.KeyCode keyCode;
            for (; ; )
            {
                if (PS2Keyboard.KeyAvailable)
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

        public static void ProcessInterrupt(uint interrupt, uint errorCode)
        {
            switch (interrupt)
            {
                case 0x21:
                    // PS/2 keyboard interrupt is 0x21 IRQ 1
                    PS2Keyboard.OnInterrupt();
                    break;
            }
        }
    }
}
