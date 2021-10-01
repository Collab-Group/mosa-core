// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86;
using Mosa.External.x86.Drawing;
using Mosa.External.x86.Drawing.Fonts;
using Mosa.External.x86.Driver;
using Mosa.External.x86.FileSystem;
using Mosa.Kernel.x86;
using System.Drawing;

namespace $safeprojectname$
{
    public static class Boot
    {
        public static int Width;
        public static int Height;

        public static int[] cursor;

        public static void Main()
        {
            // Initialize the necessary stuff
            Kernel.Setup();
            //ACPI.Init();
            IDT.SetInterruptHandler(ProcessInterrupt);

            cursor = new int[]
            {
                1,0,0,0,0,0,0,0,0,0,0,0,
                1,1,0,0,0,0,0,0,0,0,0,0,
                1,2,1,0,0,0,0,0,0,0,0,0,
                1,2,2,1,0,0,0,0,0,0,0,0,
                1,2,2,2,1,0,0,0,0,0,0,0,
                1,2,2,2,2,1,0,0,0,0,0,0,
                1,2,2,2,2,2,1,0,0,0,0,0,
                1,2,2,2,2,2,2,1,0,0,0,0,
                1,2,2,2,2,2,2,2,1,0,0,0,
                1,2,2,2,2,2,2,2,2,1,0,0,
                1,2,2,2,2,2,2,2,2,2,1,0,
                1,2,2,2,2,2,2,2,2,2,2,1,
                1,2,2,2,2,2,2,1,1,1,1,1,
                1,2,2,2,1,2,2,1,0,0,0,0,
                1,2,2,1,0,1,2,2,1,0,0,0,
                1,2,1,0,0,1,2,2,1,0,0,0,
                1,1,0,0,0,0,1,2,2,1,0,0,
                0,0,0,0,0,0,1,2,2,1,0,0,
                0,0,0,0,0,0,0,1,2,2,1,0,
                0,0,0,0,0,0,0,1,2,2,1,0,
                0,0,0,0,0,0,0,0,1,1,0,0
            };

            Scheduler.CreateThread(MainThread, PageFrameAllocator.PageSize);
            Scheduler.Start();
        }

        public static void MainThread() 
        {
            // Initialize the IDE hard drive
            // MOSA currently only supports FAT12 and FAT32 (but FAT32 doesn't work correctly in VirtualBox for now)
            /*
            IDisk disk = new IDEDisk();
            MBR mBR = new MBR();
            mBR.Initialize(disk);
            //FileSystem Takes Time
            FAT12 fs = new FAT12(disk, mBR.PartitionInfos[0]);
            */

            // Initialize graphics (default width and height is 640 and 480 respectively)
            // Make sure you've already enabled VMSVGA(in VirtualBox) or VBE(in Run.bat)
            Graphics graphics = GraphicsSelector.GetGraphics();

            Width = graphics.Width;
            Height = graphics.Height;

            // Initialize the PS/2 peripherals
            PS2Keyboard.Initialize();
            PS2Mouse.Initialize(Width, Height);

            // BitFont generator : https://github.com/nifanfa/BitFont
            string CustomCharset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            byte[] ArialCustomCharset16 = Convert.FromBase64String("AAAAAAAAAAAAAAAAHAAiACIAIgAiACIAIgAcAAAAAAAAAAAAAAAAAAAAAAAIABgAKAAIAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAABwAIgACAAIABAAIABAAPgAAAAAAAAAAAAAAAAAAAAAAHAAiAAIADAACAAIAIgAcAAAAAAAAAAAAAAAAAAAAAAAEAAwAFAAUACQAPgAEAAQAAAAAAAAAAAAAAAAAAAAAAB4AEAAgADwAAgACACIAHAAAAAAAAAAAAAAAAAAAAAAAHAAiACAAPAAiACIAIgAcAAAAAAAAAAAAAAAAAAAAAAA+AAQABAAIAAgAEAAQABAAAAAAAAAAAAAAAAAAAAAAABwAIgAiABwAIgAiACIAHAAAAAAAAAAAAAAAAAAAAAAAHAAiACIAIgAeAAIAIgAcAAAAAAAAAAAAAAAAAAAAAAAEAAoACgAKABEAHwAggCCAAAAAAAAAAAAAAAAAAAAAAD4AIQAhAD8AIQAhACEAPgAAAAAAAAAAAAAAAAAAAAAADgARACAAIAAgACAAEQAOAAAAAAAAAAAAAAAAAAAAAAA8ACIAIQAhACEAIQAiADwAAAAAAAAAAAAAAAAAAAAAAD4AIAAgAD4AIAAgACAAPgAAAAAAAAAAAAAAAAAAAAAAPgAgACAAPAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAOABEAIIAgACOAIIARAA4AAAAAAAAAAAAAAAAAAAAAACEAIQAhAD8AIQAhACEAIQAAAAAAAAAAAAAAAAAAAAAAIAAgACAAIAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAEAAQABAAEAAQAJAAkABgAAAAAAAAAAAAAAAAAAAAAACEAIgAkACwANAAiACIAIQAAAAAAAAAAAAAAAAAAAAAAIAAgACAAIAAgACAAIAA+AAAAAAAAAAAAAAAAAAAAAAAggDGAMYAqgCqAKoAkgCSAAAAAAAAAAAAAAAAAAAAAACEAMQApACkAJQAlACMAIQAAAAAAAAAAAAAAAAAAAAAADgARACCAIIAggCCAEQAOAAAAAAAAAAAAAAAAAAAAAAA8ACIAIgAiADwAIAAgACAAAAAAAAAAAAAAAAAAAAAAAA4AEQAggCCAIIAmgBEADoAAAAAAAAAAAAAAAAAAAAAAPgAhACEAPgAkACIAIgAhAAAAAAAAAAAAAAAAAAAAAAAeACEAIAAYAAYAAQAhAB4AAAAAAAAAAAAAAAAAAAAAAD4ACAAIAAgACAAIAAgACAAAAAAAAAAAAAAAAAAAAAAAIQAhACEAIQAhACEAIQAeAAAAAAAAAAAAAAAAAAAAAAAggCCAEQARAAoACgAEAAQAAAAAAAAAAAAAAAAAAAAAAEIQRRAlICUgKKAooBBAEEAAAAAAAAAAAAAAAAAAAAAAIQASABIADAAMABIAEgAhAAAAAAAAAAAAAAAAAAAAAAAggBEAEQAKAAQABAAEAAQAAAAAAAAAAAAAAAAAAAAAAB8AAgAEAAQACAAIABAAPwAAAAAAAAAAAAAAAAAAAAAAAAAAABwAIgAeACIAJgAaAAAAAAAAAAAAAAAAAAAAAAAgACAALAAyACIAIgAyACwAAAAAAAAAAAAAAAAAAAAAAAAAAAAcACIAIAAgACIAHAAAAAAAAAAAAAAAAAAAAAAAAgACABoAJgAiACIAJgAaAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAiAD4AIAAiABwAAAAAAAAAAAAAAAAAAAAAAAgAEAA4ABAAEAAQABAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAABoAJgAiACIAJgAaAAIAPAAAAAAAAAAAAAAAAAAgACAALAAyACIAIgAiACIAAAAAAAAAAAAAAAAAAAAAACAAAAAgACAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAIAAAACAAIAAgACAAIAAgACAAQAAAAAAAAAAAAAAAAAAgACAAJAAoADAAKAAoACQAAAAAAAAAAAAAAAAAAAAAACAAIAAgACAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAC8ANIAkgCSAJIAkgAAAAAAAAAAAAAAAAAAAAAAAAAAAPAAiACIAIgAiACIAAAAAAAAAAAAAAAAAAAAAAAAAAAAcACIAIgAiACIAHAAAAAAAAAAAAAAAAAAAAAAAAAAAACwAMgAiACIAMgAsACAAIAAAAAAAAAAAAAAAAAAAAAAAGgAmACIAIgAmABoAAgACAAAAAAAAAAAAAAAAAAAAAAAoADAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAIgAYAAQAIgAcAAAAAAAAAAAAAAAAAAAAAAAgACAAcAAgACAAIAAgADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAiACIAIgAiACYAGgAAAAAAAAAAAAAAAAAAAAAAAAAAACIAIgAUABQACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAIiAlIBVAFUAIgAiAAAAAAAAAAAAAAAAAAAAAAAAAAAAiABQACAAIABQAIgAAAAAAAAAAAAAAAAAAAAAAAAAAACIAIgAUABQACAAIAAgAEAAAAAAAAAAAAAAAAAAAAAAAPgAEAAgACAAQAD4AAAAAAA==");
            BitFont.RegisterBitFont(new BitFontDescriptor("ArialCustomCharset16", CustomCharset, ArialCustomCharset16, 16));

            // You can always uncomment this and the DrawImage() line below to draw a bitmap as background
            //Bitmap bitmap = new Bitmap(fs.ReadAllBytes("WALLP.BMP"));

            for (; ; )
            {
                // Clear screen (either with color or bitmap)
                //graphics.DrawImage(bitmap, 0, 0);
                graphics.Clear((uint)Color.Black.ToArgb());

                // Draw BitFont strings
                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.White.ToArgb(), "Current Driver is " + graphics.CurrentDriver, 10, 10);
                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.White.ToArgb(), "FPS is " + FPSMeter.FPS, 10, 26);
                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.White.ToArgb(), "Available Memory is " + Memory.GetAvailableMemory() / 1048576 + " MB", 10, 42);

                // Draw cursor
                DrawCursor(graphics, PS2Mouse.X, PS2Mouse.Y);

                // Update graphics (necessary if double buffering) and FPS meter
                graphics.Update();
                FPSMeter.Update();
            }
        }

        private static void ProcessInterrupt(uint interrupt, uint errorCode)
        {
            switch (interrupt)
            {
                case 0x21:
                    // PS/2 keyboard interrupt is 0x21 IRQ 1
                    PS2Keyboard.OnInterrupt();
                    break;

                case 0x2C:
                    // PS/2 mouse interrupt is 0x2C IRQ 12
                    PS2Mouse.OnInterrupt();
                    break;
            }
        }

        private static void DrawCursor(Graphics graphics, int x, int y)
        {
            for (int h = 0; h < 21 /*Cursor array height*/; h++)
                for (int w = 0; w < 12 /*Cursor array width*/; w++)
                {
                    // Draw the borders of the cursor
                    if (cursor[h * 12 + w] == 1)
                        graphics.DrawPoint((uint)Color.Black.ToArgb(), w + x, h + y);

                    // Draw the contents of the cursor (excluding the borders)
                    if (cursor[h * 12 + w] == 2)
                        graphics.DrawPoint((uint)Color.White.ToArgb(), w + x, h + y);
                }
        }
    }
}
