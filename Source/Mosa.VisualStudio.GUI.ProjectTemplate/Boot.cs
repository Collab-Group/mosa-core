﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86;
using Mosa.External.x86.Drawing;
using Mosa.External.x86.Drawing.Fonts;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;

namespace $safeprojectname$
{
    public static class Boot
    {
        static int Width = 640;
        static int Height = 480;

        static int[] cursor = new int[]
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

        public static void Main()
        {
            Mosa.Kernel.x86.Kernel.Setup();

            IDT.SetInterruptHandler(ProcessInterrupt);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Color = ConsoleColor.White;

            PS2Keyboard.Initialize();
            PS2Mouse.Initialize(Width, Height);

            PCI.Setup();

            Graphics graphics = new VMWareSVGAIIGraphics(Width, Height);

            //Generator https://github.com/nifanfa/BitFont
            string CustomCharset = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            byte[] ArialCustomCharset16 = Convert.FromBase64String("AAAAAAAAAACAAIAAgACAAIAAgAAAAIAAAAAAAAAAAAAAAAAAAAAAAKAAoACgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKAAoAPgAUABQAPgAoACgAAAAAAAAAAAAAAAAAAAAAABwAKgAoABwACgAKACoAHAAIAAAAAAAAAAAAAAAAAAAAGIAlACUAGgACwAUgBSAIwAAAAAAAAAAAAAAAAAAAAAAMABIAEgAMABQAIwAiAB0AAAAAAAAAAAAAAAAAAAAAACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAQACAAIAAgACAAIAAgABAACAAAAAAAAAAAAAAAAAAgABAACAAIAAgACAAIAAgAEAAgAAAAAAAAAAAAAAAAABAAOAAQACgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgACAA+AAgACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAEAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAIAAgAEAAQABAAEAAgACAAAAAAAAAAAAAAAAAAAAAAABwAIgAiACIAIgAiACIAHAAAAAAAAAAAAAAAAAAAAAAACAAYACgACAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAcACIAAgACAAQACAAQAD4AAAAAAAAAAAAAAAAAAAAAABwAIgACAAwAAgACACIAHAAAAAAAAAAAAAAAAAAAAAAABAAMABQAFAAkAD4ABAAEAAAAAAAAAAAAAAAAAAAAAAAeABAAIAA8AAIAAgAiABwAAAAAAAAAAAAAAAAAAAAAABwAIgAgADwAIgAiACIAHAAAAAAAAAAAAAAAAAAAAAAAPgAEAAQACAAIABAAEAAQAAAAAAAAAAAAAAAAAAAAAAAcACIAIgAcACIAIgAiABwAAAAAAAAAAAAAAAAAAAAAABwAIgAiACIAHgACACIAHAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAAAAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAAAAAAAAACAAIAAgAAAAAAAAAAAAAAAAAAAAAAACABwAIAAcAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPgAAAD4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAcAAIAHAAgAAAAAAAAAAAAAAAAAAAAAAAAABwAIgACAAQACAAIAAAACAAAAAAAAAAAAAAAAAAAAAAAB8AYIBNQJNAokCiQKaAmwBAQD+AAAAAAAAAAAAAAAAAEAAoACgAKABEAHwAggCCAAAAAAAAAAAAAAAAAAAAAAD4AIQAhAD8AIQAhACEAPgAAAAAAAAAAAAAAAAAAAAAADgARACAAIAAgACAAEQAOAAAAAAAAAAAAAAAAAAAAAAA8ACIAIQAhACEAIQAiADwAAAAAAAAAAAAAAAAAAAAAAD4AIAAgAD4AIAAgACAAPgAAAAAAAAAAAAAAAAAAAAAAPgAgACAAPAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAOABEAIIAgACOAIIARAA4AAAAAAAAAAAAAAAAAAAAAACEAIQAhAD8AIQAhACEAIQAAAAAAAAAAAAAAAAAAAAAAIAAgACAAIAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAEAAQABAAEAAQAJAAkABgAAAAAAAAAAAAAAAAAAAAAACEAIgAkACwANAAiACIAIQAAAAAAAAAAAAAAAAAAAAAAIAAgACAAIAAgACAAIAA+AAAAAAAAAAAAAAAAAAAAAAAggDGAMYAqgCqAKoAkgCSAAAAAAAAAAAAAAAAAAAAAACEAMQApACkAJQAlACMAIQAAAAAAAAAAAAAAAAAAAAAADgARACCAIIAggCCAEQAOAAAAAAAAAAAAAAAAAAAAAAA8ACIAIgAiADwAIAAgACAAAAAAAAAAAAAAAAAAAAAAAA4AEQAggCCAIIAmgBEADoAAAAAAAAAAAAAAAAAAAAAAPgAhACEAPgAkACIAIgAhAAAAAAAAAAAAAAAAAAAAAAAeACEAIAAYAAYAAQAhAB4AAAAAAAAAAAAAAAAAAAAAAD4ACAAIAAgACAAIAAgACAAAAAAAAAAAAAAAAAAAAAAAIQAhACEAIQAhACEAIQAeAAAAAAAAAAAAAAAAAAAAAAAggCCAEQARAAoACgAEAAQAAAAAAAAAAAAAAAAAAAAAAAIQBRAlICUgKKAooBBAEEAAAAAAAAAAAAAAAAAAAAAAIQASABIADAAMABIAEgAhAAAAAAAAAAAAAAAAAAAAAAAggBEAEQAKAAQABAAEAAQAAAAAAAAAAAAAAAAAAAAAAB8AAgAEAAQACAAIABAAPwAAAAAAAAAAAAAAAAAAAAAAMAAgACAAIAAgACAAIAAgACAAMAAAAAAAAAAAAAAAAAAgACAAEAAQABAAEAAIAAgAAAAAAAAAAAAAAAAAAAAAADAAEAAQABAAEAAQABAAEAAQADAAAAAAAAAAAAAAAAAACAAUABQAIgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/AAAAAAAAAAAAAAAAACAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAIgAeACIAJgAaAAAAAAAAAAAAAAAAAAAAAAAgACAALAAyACIAIgAyACwAAAAAAAAAAAAAAAAAAAAAAAAAAAAcACIAIAAgACIAHAAAAAAAAAAAAAAAAAAAAAAAAgACABoAJgAiACIAJgAaAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAiAD4AIAAiABwAAAAAAAAAAAAAAAAAAAAAAAgAEAA4ABAAEAAQABAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAABoAJgAiACIAJgAaAAIAPAAAAAAAAAAAAAAAAAAgACAALAAyACIAIgAiACIAAAAAAAAAAAAAAAAAAAAAACAAAAAgACAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAIAAAACAAIAAgACAAIAAgACAAAAAAAAAAAAAAAAAAAAAgACAAJAAoADAAKAAoACQAAAAAAAAAAAAAAAAAAAAAACAAIAAgACAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAC8ANIAkgCSAJIAkgAAAAAAAAAAAAAAAAAAAAAAAAAAAPAAiACIAIgAiACIAAAAAAAAAAAAAAAAAAAAAAAAAAAAcACIAIgAiACIAHAAAAAAAAAAAAAAAAAAAAAAAAAAAACwAMgAiACIAMgAsACAAIAAAAAAAAAAAAAAAAAAAAAAAGgAmACIAIgAmABoAAgACAAAAAAAAAAAAAAAAAAAAAAAoADAAIAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAIgAYAAQAIgAcAAAAAAAAAAAAAAAAAAAAAAAgACAAMAAgACAAIAAgADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAiACIAIgAiACYAGgAAAAAAAAAAAAAAAAAAAAAAAAAAACIAIgAUABQACAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAIiAlIBVAFUAIgAiAAAAAAAAAAAAAAAAAAAAAAAAAAAAiABQACAAIABQAIgAAAAAAAAAAAAAAAAAAAAAAAAAAACIAIgAUABQACAAIAAgAEAAAAAAAAAAAAAAAAAAAAAAAPgAEAAgACAAQAD4AAAAAAAAAAAAAAAAAAAAAAAgAEAAQABAAIAAQABAAEAAQAAgAAAAAAAAAAAAAAAAAIAAgACAAIAAgACAAIAAgACAAIAAAAAAAAAAAAAAAAAAQAAgACAAIAAQACAAIAAgACAAQAAAAAAAAAAAAAAAAAAAAAAAAADoALAAAAAAAAAAAAAAAAAAAAA=");
            BitFont.RegisterBitFont(new BitFontDescriptor("ArialCustomCharset16", CustomCharset, ArialCustomCharset16, 16));

            for (; ; )
            {
                graphics.Clear((uint)Color.FromArgb(0, 0, 0).ToArgb());

                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.FromArgb(255, 255, 255).ToArgb(), "Hello MOSA", 10, 10);
                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.FromArgb(255, 255, 255).ToArgb(), "FPS:"+FPSMeter.FPS, 10, 26);
                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.FromArgb(255, 255, 255).ToArgb(), "Available Memory:" + Memory.GetAvailableMemory() / 1048576 + "MB", 10, 42);

                DrawCursor(graphics, PS2Mouse.X, PS2Mouse.Y);

                graphics.Update();

                FPSMeter.Update();

                //If The FPS > Mouse Interrupt Speed The Mouse Will Be Not Smooth
                Native.Hlt();
            }
        }

        public static void ProcessInterrupt(uint interrupt, uint errorCode)
        {
            switch (interrupt)
            {
                case 0x21:
                    //PS/2 Mouse Interrupt Is 0x21 IRQ 1
                    PS2Keyboard.OnInterrupt();
                    break;
                case 0x2C:
                    //PS/2 Mouse Interrupt Is 0x2C IRQ 12
                    PS2Mouse.OnInterrupt();
                    break;
            }
        }

        public static void DrawCursor(Graphics graphics, int x, int y)
        {
            for (int h = 0; h < 21; h++)
            {
                for (int w = 0; w < 12; w++)
                {
                    if (cursor[h * 12 + w] == 1)
                    {
                        graphics.DrawPoint(0x0, w + x, h + y);
                    }
                    if (cursor[h * 12 + w] == 2)
                    {
                        graphics.DrawPoint(0xFFFFFFFF, w + x, h + y);
                    }
                }
            }
        }
    }
}
