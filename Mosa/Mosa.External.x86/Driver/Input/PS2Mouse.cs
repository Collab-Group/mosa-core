using Mosa.Kernel.x86;
using Mosa.Runtime.x86;
using System;
using static Mosa.Runtime.x86.Native;

namespace Mosa.External.x86.Driver
{
    public static class PS2Mouse
    {
        private const byte Data = 0x0060;
        private const byte Command = 0x0064;

        private const byte SetDefaults = 0xF6;
        private const byte EnableDataReporting = 0xF4;

        public static void Initialize(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;

            X = ScreenWidth / 2;
            Y = ScreenHeight / 2;

            byte _status;

            Hlt();
            Out8(Command, 0xA8);

            Hlt();
            Out8(Command, 0x20);
            Hlt();
            _status = ((byte)(In8(0x60) | 3));
            Hlt();
            Out8(Command, 0x60);
            Hlt();
            Out8(Data, _status);

            WriteRegister(SetDefaults);
            WriteRegister(EnableDataReporting);

            WriteRegister(0xF2);

            WriteRegister(0xF3);
            WriteRegister(200);

            Nop();

            WriteRegister(0xF2);

            byte result = ReadRegister();
            if (result == 3)
            {
                Console.WriteLine("Wheel Available");
            }

            Btn = "None";
            Console.WriteLine("PS/2 mouse enabled!");
        }

        private static int Phase = 0;
        public static byte[] MData = new byte[3];
        private static int aX;
        private static int aY;
        public static string Btn;

        public static int X = 0;
        public static int Y = 0;

        public static int ScreenWidth = 0;
        public static int ScreenHeight = 0;

        public static void WriteRegister(byte value)
        {
            Hlt();
            Out8(Command, 0xD4);
            Hlt();
            Out8(Data, value);

            ReadRegister();
        }

        public static byte ReadRegister()
        {
            Hlt();
            return In8(Data);
        }

        public static void OnInterrupt()
        {
            byte D = IOPort.In8(Data);

            if (Phase == 0)
            {
                if (D == 0xfa)
                    Phase = 1;
                return;
            }

            if (Phase == 1)
            {
                if ((D & (1 << 3)) == (1 << 3))
                {
                    MData[0] = D;
                    Phase = 2;
                }
                return;
            }

            if (Phase == 2)
            {
                MData[1] = D;
                Phase = 3;
                return;
            }

            if (Phase == 3)
            {
                MData[2] = D;
                Phase = 1;

                MData[0] &= 0x07;
                Btn = MData[0] switch
                {
                    0x01 => "Left",
                    0x02 => "Right",
                    _ => "None",
                };

                if (MData[1] > 127)
                    aX = -(255 - MData[1]);
                else
                    aX = MData[1];

                if (MData[2] > 127)
                    aY = -(255 - MData[2]);
                else
                    aY = MData[2];

                X = Math.Clamp(X + aX, 0, ScreenWidth);
                Y = Math.Clamp(Y - aY, 0, ScreenHeight);
            }

            return;
        }
    }
}
