// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86;
using Mosa.External.x86.Drawing;
using Mosa.External.x86.Drawing.Fonts;
using Mosa.External.x86.Driver;
using Mosa.External.x86.FileSystem;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;

namespace $safeprojectname$
{
    public static class Boot
    {
        public static int Width = 640;
        public static int Height = 480;

        public static int[] cursor = new int[]
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

        // Cache colors, we currently do not cache the ARGB values in the Color class
        public static uint black = (uint)Color.Black.ToArgb();
        public static uint white = (uint)Color.White.ToArgb();

        public static void Main()
        {
            // Initialize the kernel and interrupts
            Kernel.Setup();
            IDT.SetInterruptHandler(ProcessInterrupt);

            // Initialize the PS/2 peripherals
            PS2Keyboard.Initialize();
            PS2Mouse.Initialize(Width, Height);

            // Initialize the IDE hard drive
            // MOSA currently only supports FAT12
            IDisk disk = new IDEDisk();
            MBR.Initialize(disk);
            FAT12 fs = new FAT12(disk, MBR.PartitionInfos[0]);
            
            // Initialize graphics (default width and height is 640 and 480 respectively)
            Graphics graphics = GraphicsSelector.GetGraphics(); //GraphicsSelector.GetGraphics(Width, Height);

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
                graphics.Clear(black);

                // Draw BitFont strings
                graphics.DrawBitFontString("ArialCustomCharset16", white, "Current Driver is " + graphics.CurrentDriver, 10, 10);
                graphics.DrawBitFontString("ArialCustomCharset16", white, "FPS is " + FPSMeter.FPS, 10, 26);
                graphics.DrawBitFontString("ArialCustomCharset16", white, "Available Memory is " + Memory.GetAvailableMemory() / 1048576 + " MB", 10, 42);

                // Draw cursor
                DrawCursor(graphics, PS2Mouse.X, PS2Mouse.Y);

                // Update graphics (necessary if double buffering) and FPS meter
                graphics.Update();
                FPSMeter.Update();

                // If the FPS is superior to the mouse speed the mouse won't be smooth
                // Note that if you remove this you'll have more FPS (the desktop will be smoother)
                Native.Hlt();
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
                        graphics.DrawPoint(black, w + x, h + y);

                    // Draw the contents of the cursor (excluding the borders)
                    if (cursor[h * 12 + w] == 2)
                        graphics.DrawPoint(white, w + x, h + y);
                }
        }
    }
}
