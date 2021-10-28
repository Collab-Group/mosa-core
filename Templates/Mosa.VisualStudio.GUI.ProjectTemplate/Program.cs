// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86;
using Mosa.External.x86.Drawing;
using Mosa.External.x86.Drawing.Fonts;
using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.Plug;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace $safeprojectname$
{
    public static class Program
    {
        public static bool Panicked = false;
        public static int Width;
        public static int Height;
        public static Graphics graphics;

        public static int[] cursor;

        public static void Main() { }

        [VBERequire(800, 600)]
        [Plug("Mosa.Runtime.StartUp::KMain")]
        [UnmanagedCallersOnly(EntryPoint = "KMain", CallingConvention = CallingConvention.StdCall)]
        public static void KMain()
        {
            IDT.OnInterrupt += IDT_OnInterrupt;
            Panic.OnPanic += Panic_OnPanic;

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

            new Thread(MainThread).Start();

            for (; ; );
        }

        private static void IDT_OnInterrupt(uint irq, uint error)
        {
            switch (irq)
            {
                case 0x21:
                    PS2Keyboard.OnInterrupt();
                    PS2Keyboard.KeyCode keyCode = PS2Keyboard.GetKeyPressed();
                    if (keyCode == PS2Keyboard.KeyCode.F12)
                    {
                        Panic.Error("Hello World!!!");
                    }
                    break;

                case 0x2C:
                    PS2Mouse.OnInterrupt();
                    break;
            }
        }

        // VBE Panic, this gets called if a panic happens!
        private static void Panic_OnPanic(string message)
        {
            Panicked = true;
            string CustomCharset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!:";
            byte[] ArialCustomCharset24 = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAAAAA4AABEAACCAACCAACCAACCAACCAACCAACCAACCAABEAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAYAAAoAABIAAAIAAAIAAAIAAAIAAAIAAAIAAAIAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA8AABEAACCAAACAAACAAAEAAAEAAAIAAAQAAAgAABAAAD+AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AABEAACEAAAEAAAIAAA4AAAEAAACAAACAACCAADEAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAYAAAoAAAoAABIAABIAACIAAEIAAH+AAAIAAAIAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB+AABAAABAAACAAAD4AACEAAACAAACAAACAACCAABEAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AABGAACCAACAAAC4AADEAACCAACCAACCAACCAABEAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD+AAAEAAAEAAAIAAAIAAAQAAAQAAAQAAAQAAAgAAAgAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AABEAACCAACCAABEAAA4AABEAACCAACCAACCAABEAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AABEAACCAACCAACCAACCAABGAAA6AAACAACCAABEAAB4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAUAAAUAAAiAAAiAAAiAABBAAB/AACAgACAgAEAQAEAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD+AACBAACAgACAgACBAAD/AACBAACAgACAgACAgACBAAD+AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAfAAAggABAQACAAACAAACAAACAAACAAACAAABAQAAggAAfAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD+AACBAACAgACAQACAQACAQACAQACAQACAQACAgACBAAD+AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD/gACAAACAAACAAACAAAD/AACAAACAAACAAACAAACAAAD/gAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD/AACAAACAAACAAACAAAD+AACAAACAAACAAACAAACAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAeAAAhAABAgACAAACAAACAAACHwACAQACAQABAgAAhAAAeAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAgACAgACAgACAgACAgAD/gACAgACAgACAgACAgACAgACAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAABAAABAAABAAABAAABAAABAAABAAABAAABAAABAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAACAAACAAACAAACAAACAAACAACCAACCAABEAAB4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAgACBAACCAACEAACIAACYAACkAADCAACCAACBAACBAACAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAACAAACAAACAAACAAACAAACAAACAAACAAACAAAD+AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAIADAYADAYACgoACgoACRIACRIACSIACKIACKIACEIACEIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAgADAgACggACggACQgACIgACIgACEgACCgACCgACBgACAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAeAAAhAABAgACAQACAQACAQACAQACAQACAQABAgAAhAAAeAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD+AACBAACAgACAgACAgACBAAD+AACAAACAAACAAACAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAeAAAhAABAgACAQACAQACAQACAQACAQACAQABGwAAhgAAewAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD+AACBAACAgACAgACAgACBAAD+AACEAACCAACCAACBAACAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA+AABBAACAgACAAABAAAA4AAAHAAAAgACAgACAgABBAAA+AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH/AAAQAAAQAAAQAAAQAAAQAAAQAAAQAAAQAAAQAAAQAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAgACAgACAgACAgACAgACAgACAgACAgACAgACAgABBAAA+AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAgACAgABBAABBAABBAAAiAAAiAAAiAAAUAAAUAAAIAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQCCggCCggCCggBERABERABERAAoKAAoKAAoKAAQEAAQEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAgABBAAAiAAAiAAAUAAAIAAAUAAAiAAAiAABBAACAgAEAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAgABBAABBAAAiAAAUAAAUAAAIAAAIAAAIAAAIAAAIAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD/AAACAAAEAAAEAAAIAAAQAAAQAAAgAABAAABAAACAAAH/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA8AABCAACCAAAOAAByAACCAACCAACGAAB6AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAACAAAC4AADEAACCAACCAACCAACCAACCAADEAAC4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AABEAACCAACAAACAAACAAACCAABEAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAACAAA6AABGAACCAACCAACCAACCAACCAABGAAA6AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AABEAACCAACCAAD+AACAAACCAABEAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwAABAAABAAADwAABAAABAAABAAABAAABAAABAAABAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA6AABGAACCAACCAACCAACCAACCAABGAAA6AAACAACEAAB4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAACAAAC8AADCAACCAACCAACCAACCAACCAACCAACCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAAAAAAAAAACAAACAAACAAACAAACAAACAAACAAACAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAAAAAAAAAACAAACAAACAAACAAACAAACAAACAAACAAACAAACAAACAAAMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAACAAACCAACEAACIAACQAACwAADIAACEAACEAACCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAACAAACAAACAAACAAACAAACAAACAAACAAACAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC5wADGIACEIACEIACEIACEIACEIACEIACEIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC8AADCAACCAACCAACCAACCAACCAACCAACCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AABEAACCAACCAACCAACCAACCAABEAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC4AADEAACCAACCAACCAACCAACCAADEAAC4AACAAACAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA6AABGAACCAACCAACCAACCAACCAABGAAA6AAACAAACAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC4AADAAACAAACAAACAAACAAACAAACAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB4AACEAACAAACAAAB4AAAEAAAEAACEAAB4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAACAAAHgAACAAACAAACAAACAAACAAACAAACAAADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACCAACCAACCAACCAACCAACCAACCAACGAAB6AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEEAAEEAACIAACIAACIAABQAABQAAAgAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEIQAEIQACUgACUgACigACigACigABBAABBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEEAACIAABQAABQAAAgAABQAABQAACIAAEEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACCAACCAACCAABEAABEAABMAAAoAAAoAAAQAAAQAAAgAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH8AAAIAAAQAAAQAAAgAABAAABAAACAAAH8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAABAAABAAABAAABAAABAAABAAABAAABAAABAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            BitFont.RegisterBitFont(new BitFontDescriptor("ArialCustomCharset24", CustomCharset, ArialCustomCharset24, 24));
            for (; ; )
            {
                graphics.Clear(Color.Blue.ToArgb());

                graphics.DrawBitFontString("ArialCustomCharset24", Color.White.ToArgb(), "Kernel Panic!", 10, 10);
                graphics.DrawBitFontString("ArialCustomCharset24", Color.White.ToArgb(), "Message: " + message, 10, 30);

                graphics.Update();
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

            // Initialize graphics (default width and height is 640 and 480 respectively)
            // Make sure you've already enabled VMSVGA(in VirtualBox) or VBE(in Run.bat)
            graphics = GraphicsSelector.GetGraphics();

            Width = graphics.Width;
            Height = graphics.Height;

            PS2Mouse.Initialize(Width, Height);

            // BitFont generator : https://github.com/nifanfa/BitFont
            string CustomCharset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            byte[] ArialCustomCharset16 = Convert.FromBase64String("AAAAAAAAAAAAAAAAHAAiACIAIgAiACIAIgAcAAAAAAAAAAAAAAAAAAAAAAAIABgAKAAIAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAABwAIgACAAIABAAIABAAPgAAAAAAAAAAAAAAAAAAAAAAHAAiAAIADAACAAIAIgAcAAAAAAAAAAAAAAAAAAAAAAAEAAwAFAAUACQAPgAEAAQAAAAAAAAAAAAAAAAAAAAAAB4AEAAgADwAAgACACIAHAAAAAAAAAAAAAAAAAAAAAAAHAAiACAAPAAiACIAIgAcAAAAAAAAAAAAAAAAAAAAAAA+AAQABAAIAAgAEAAQABAAAAAAAAAAAAAAAAAAAAAAABwAIgAiABwAIgAiACIAHAAAAAAAAAAAAAAAAAAAAAAAHAAiACIAIgAeAAIAIgAcAAAAAAAAAAAAAAAAAAAAAAAEAAoACgAKABEAHwAggCCAAAAAAAAAAAAAAAAAAAAAAD4AIQAhAD8AIQAhACEAPgAAAAAAAAAAAAAAAAAAAAAADgARACAAIAAgACAAEQAOAAAAAAAAAAAAAAAAAAAAAAA8ACIAIQAhACEAIQAiADwAAAAAAAAAAAAAAAAAAAAAAD4AIAAgAD4AIAAgACAAPgAAAAAAAAAAAAAAAAAAAAAAPgAgACAAPAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAOABEAIIAgACOAIIARAA4AAAAAAAAAAAAAAAAAAAAAACEAIQAhAD8AIQAhACEAIQAAAAAAAAAAAAAAAAAAAAAAIAAgACAAIAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAEAAQABAAEAAQAJAAkABgAAAAAAAAAAAAAAAAAAAAAACEAIgAkACwANAAiACIAIQAAAAAAAAAAAAAAAAAAAAAAIAAgACAAIAAgACAAIAA+AAAAAAAAAAAAAAAAAAAAAAAggDGAMYAqgCqAKoAkgCSAAAAAAAAAAAAAAAAAAAAAACEAMQApACkAJQAlACMAIQAAAAAAAAAAAAAAAAAAAAAADgARACCAIIAggCCAEQAOAAAAAAAAAAAAAAAAAAAAAAA8ACIAIgAiADwAIAAgACAAAAAAAAAAAAAAAAAAAAAAAA4AEQAggCCAIIAmgBEADoAAAAAAAAAAAAAAAAAAAAAAPgAhACEAPgAkACIAIgAhAAAAAAAAAAAAAAAAAAAAAAAeACEAIAAYAAYAAQAhAB4AAAAAAAAAAAAAAAAAAAAAAD4ACAAIAAgACAAIAAgACAAAAAAAAAAAAAAAAAAAAAAAIQAhACEAIQAhACEAIQAeAAAAAAAAAAAAAAAAAAAAAAAggCCAEQARAAoACgAEAAQAAAAAAAAAAAAAAAAAAAAAAEIQRRAlICUgKKAooBBAEEAAAAAAAAAAAAAAAAAAAAAAIQASABIADAAMABIAEgAhAAAAAAAAAAAAAAAAAAAAAAAggBEAEQAKAAQABAAEAAQAAAAAAAAAAAAAAAAAAAAAAB8AAgAEAAQACAAIABAAPwAAAAAAAAAAAAAAAAAAAAAAAAAAABwAIgAeACIAJgAaAAAAAAAAAAAAAAAAAAAAAAAgACAALAAyACIAIgAyACwAAAAAAAAAAAAAAAAAAAAAAAAAAAAcACIAIAAgACIAHAAAAAAAAAAAAAAAAAAAAAAAAgACABoAJgAiACIAJgAaAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAiAD4AIAAiABwAAAAAAAAAAAAAAAAAAAAAAAgAEAA4ABAAEAAQABAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAABoAJgAiACIAJgAaAAIAPAAAAAAAAAAAAAAAAAAgACAALAAyACIAIgAiACIAAAAAAAAAAAAAAAAAAAAAACAAAAAgACAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAIAAAACAAIAAgACAAIAAgACAAQAAAAAAAAAAAAAAAAAAgACAAJAAoADAAKAAoACQAAAAAAAAAAAAAAAAAAAAAACAAIAAgACAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAC8ANIAkgCSAJIAkgAAAAAAAAAAAAAAAAAAAAAAAAAAAPAAiACIAIgAiACIAAAAAAAAAAAAAAAAAAAAAAAAAAAAcACIAIgAiACIAHAAAAAAAAAAAAAAAAAAAAAAAAAAAACwAMgAiACIAMgAsACAAIAAAAAAAAAAAAAAAAAAAAAAAGgAmACIAIgAmABoAAgACAAAAAAAAAAAAAAAAAAAAAAAoADAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAIgAYAAQAIgAcAAAAAAAAAAAAAAAAAAAAAAAgACAAcAAgACAAIAAgADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAiACIAIgAiACYAGgAAAAAAAAAAAAAAAAAAAAAAAAAAACIAIgAUABQACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAIiAlIBVAFUAIgAiAAAAAAAAAAAAAAAAAAAAAAAAAAAAiABQACAAIABQAIgAAAAAAAAAAAAAAAAAAAAAAAAAAACIAIgAUABQACAAIAAgAEAAAAAAAAAAAAAAAAAAAAAAAPgAEAAgACAAQAD4AAAAAAA==");
            BitFont.RegisterBitFont(new BitFontDescriptor("ArialCustomCharset16", CustomCharset, ArialCustomCharset16, 16));

            // You can always uncomment this and the DrawImage() line below to draw a bitmap as background
            //Bitmap bitmap = new Bitmap(fs.ReadAllBytes("WALLP.BMP"));

            for (; ; )
            {
                if (!Panicked)
                {
                    // Clear screen (either with color or bitmap)
                    //graphics.DrawImageASM(bitmap, 0, 0);
                    graphics.Clear(Color.Black.ToArgb());

                    graphics.DrawBitFontString("ArialCustomCharset16", Color.White.ToArgb(), "Current Driver is " + graphics.CurrentDriver, 10, 10);
                    graphics.DrawBitFontString("ArialCustomCharset16", Color.White.ToArgb(), "FPS is " + FPSMeter.FPS, 10, 26);
                    graphics.DrawBitFontString("ArialCustomCharset16", Color.White.ToArgb(), "Available Memory is " + Memory.GetAvailableMemory() / 1048576 + " MB", 10, 42);

                    DrawCursor(graphics, PS2Mouse.X, PS2Mouse.Y);

                    graphics.Update();
                    FPSMeter.Update();
                }
            }
        }

        private static void DrawCursor(Graphics graphics, int x, int y)
        {
            for (int h = 0; h < 21; h++)
                for (int w = 0; w < 12; w++)
                {
                    if (cursor[h * 12 + w] == 1)
                        graphics.DrawPoint((uint)Color.Black.ToArgb(), w + x, h + y);

                    if (cursor[h * 12 + w] == 2)
                        graphics.DrawPoint((uint)Color.White.ToArgb(), w + x, h + y);
                }
        }
    }
}
