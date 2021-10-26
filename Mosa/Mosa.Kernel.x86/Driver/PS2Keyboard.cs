using Mosa.Kernel.x86;

namespace Mosa.External.x86.Driver
{
    public static class PS2Keyboard
    {
        public static KeyCode Code;
        public static byte KData = 0x00;

        public static void OnInterrupt()
        {
            KData = IOPort.In8(0x60);
            if (KData == (byte)KeyCode.CapsLock) IsCapsLock = !IsCapsLock;
        }

        public static bool IsCapsLock = false;

        public static KeyCode GetKeyPressed()
        {
            Code = (KeyCode)KData;
            KData = 0;
            return Code;
        }

        public static string KeyCodeToString(this KeyCode keyCode)
        {
            return keyCode switch
            {
                KeyCode.A => "A",
                KeyCode.B => "B",
                KeyCode.C => "C",
                KeyCode.D => "D",
                KeyCode.E => "E",
                KeyCode.F => "F",
                KeyCode.G => "G",
                KeyCode.H => "H",
                KeyCode.I => "I",
                KeyCode.J => "J",
                KeyCode.K => "K",
                KeyCode.L => "L",
                KeyCode.M => "M",
                KeyCode.N => "N",
                KeyCode.O => "O",
                KeyCode.P => "P",
                KeyCode.Q => "Q",
                KeyCode.R => "R",
                KeyCode.S => "S",
                KeyCode.T => "T",
                KeyCode.U => "U",
                KeyCode.V => "V",
                KeyCode.W => "W",
                KeyCode.X => "X",
                KeyCode.Y => "Y",
                KeyCode.Z => "Z",
                KeyCode.Space => " ",
                KeyCode.Num1 => "1",
                KeyCode.Num2 => "2",
                KeyCode.Num3 => "3",
                KeyCode.Num4 => "4",
                KeyCode.Num5 => "5",
                KeyCode.Num6 => "6",
                KeyCode.Num7 => "7",
                KeyCode.Num8 => "8",
                KeyCode.Num9 => "9",
                KeyCode.Num0 => "0",
                KeyCode.Dash => "-",
                KeyCode.Equals => "=",
                KeyCode.Tab => "    ",
                KeyCode.RightFacedSquareBracket => "[",
                KeyCode.LeftFacedSquareBracket => "]",
                KeyCode.SemiColon => ";",
                KeyCode.SingleQuote => "'",
                KeyCode.BackTick => "`",
                KeyCode.BackSlash => "\\",
                KeyCode.Comma => ",",
                KeyCode.Period => ".",
                KeyCode.ForwardSlash => "/",
                KeyCode.Star => "*",
                KeyCode.NumPadPeriod => ".",
                KeyCode.NumPad0 => "0",
                KeyCode.NumPad1 => "1",
                KeyCode.NumPad2 => "2",
                KeyCode.NumPad3 => "3",
                KeyCode.NumPad4 => "4",
                KeyCode.NumPad5 => "5",
                KeyCode.NumPad6 => "6",
                KeyCode.NumPad7 => "7",
                KeyCode.NumPad8 => "8",
                KeyCode.NumPad9 => "9",
                KeyCode.NumPadMinus => "-",
                KeyCode.NumPadPlus => "+",
                _ => string.Empty
            };
        }

        public enum KeyCode
        {
            A = 0x1E,
            B = 0x30,
            C = 0x2E,
            D = 0x20,
            E = 0x12,
            F = 0x21,
            G = 0x22,
            H = 0x23,
            I = 0x17,
            J = 0x24,
            K = 0x25,
            L = 0x26,
            M = 0x32,
            N = 0x31,
            O = 0x18,
            P = 0x19,
            Q = 0x10,
            R = 0x13,
            S = 0x1F,
            T = 0x14,
            U = 0x16,
            V = 0x2F,
            W = 0x11,
            X = 0x2D,
            Y = 0x15,
            Z = 0x2C,
            Num1 = 0x02,
            Num2 = 0x03,
            Num3 = 0x04,
            Num4 = 0x05,
            Num5 = 0x06,
            Num6 = 0x07,
            Num7 = 0x08,
            Num8 = 0x09,
            Num9 = 0x0A,
            Num0 = 0x0B,
            F1 = 0x3B,
            F2 = 0x3C,
            F3 = 0x3D,
            F4 = 0x3E,
            F5 = 0x3F,
            F6 = 0x40,
            F7 = 0x41,
            F8 = 0x42,
            F9 = 0x43,
            F10 = 0x44,
            F11 = 0x57,
            F12 = 0x58,
            Dash = 0x0C,
            Equals = 0x0D,
            Tab = 0x0F,
            RightFacedSquareBracket = 0x1A,
            LeftFacedSquareBracket = 0x1B,
            LeftCTRL = 0x1D,
            SemiColon = 0x27,
            SingleQuote = 0x28,
            BackTick = 0x29,
            LeftShift = 0x2A,
            BackSlash = 0x2B,
            Comma = 0x33,
            Period = 0x34,
            ForwardSlash = 0x35,
            RightShift = 0x36,
            Star = 0x37,
            LeftAlt = 0x38,
            NumLock = 0x45,
            ScrollLock = 0x46,
            NumPadPeriod = 0x53,
            NumPad0 = 0x52,
            NumPad3 = 0x51,
            NumPad2 = 0x50,
            NumPad1 = 0x4F,
            NumPadPlus = 0x4E,
            NumPad6 = 0x4D,
            NumPad5 = 0x4C,
            NumPad4 = 0x4B,
            NumPadMinus = 0x4A,
            NumPad9 = 0x49,
            NumPad8 = 0x48,
            NumPad7 = 0x47,
            CapsLock = 0x3A,
            Delete = 0x0E,
            Space = 0x39,
            Enter = 0x1C,
            ESC = 0x01
        }
    }
}
