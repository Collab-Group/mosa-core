using Mosa.Kernel.x86;
using System;

namespace Mosa.External.x86.Driver
{
    public static class PS2Mouse
    {
        private const byte Port_KeyData = 0x0060;
        private const byte Port_KeyCommand = 0x0064;
        private const byte KeyStatus_Send_NotReady = 0x02;
        private const byte KeyCommand_Write_Mode = 0x60;
        private const byte KBC_Mode = 0x47;
        private const byte KeyCommand_SendTo_Mouse = 0xd4;
        private const byte MouseCommand_Enable = 0xf4;
        private const byte Mouse_SetSampleRate = 0xF3;

        public static void Wait_KBC()
        {
            while ((IOPort.In8(Port_KeyCommand) & KeyStatus_Send_NotReady) != 0) ;
        }

        public static void Initialize(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;

            X = ScreenWidth / 2;
            Y = ScreenHeight / 2;

            // Set KBC mode
            WriteCommand(KeyCommand_Write_Mode, KBC_Mode);

            // Set sample rate
            WriteCommand(Mouse_SetSampleRate, 200);

            // Enable automatic packet streaming
            WriteCommand(KeyCommand_SendTo_Mouse, MouseCommand_Enable);

            Btn = "";
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

        public static void WriteCommand(byte command, byte value)
        {
            Wait_KBC();
            IOPort.Out8(Port_KeyCommand, command);
            Wait_KBC();
            IOPort.Out8(Port_KeyData, value);
        }

        public static void OnInterrupt()
        {
            byte D = IOPort.In8(Port_KeyData);

            if (Phase == 0)
            {
                if (D == 0xfa)
                    Phase = 1;
                return;
            }

            if (Phase == 1)
            {
                if ((D & 0xc8) == 0x08)
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
