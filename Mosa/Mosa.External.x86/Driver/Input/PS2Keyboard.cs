using Mosa.Kernel.x86;

namespace Mosa.External.x86.Driver
{
    public static class PS2Keyboard
    {
        private const byte Port_KeyData = 0x0060;
        public static byte KData = 0x00;

        public static void Initialize()
        {
            KData = 0x00;
        }

        public static void OnInterrupt()
        {
            KData = IOPort.In8(Port_KeyData);
            if (KeyCodeToString((KeyCode)KData) == "null") return;

            KeyAvailable = true;

            if (KData == (byte)KeyCode.CapsLock) IsCapsLock = !IsCapsLock;
        }

        public static bool KeyAvailable = false;
        public static bool IsCapsLock = false;

        public static KeyCode GetKeyPressed()
        {
            KeyAvailable = false;
            return (KeyCode)KData;
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
                KeyCode._1 => "1",
                KeyCode._2 => "2",
                KeyCode._3 => "3",
                KeyCode._4 => "4",
                KeyCode._5 => "5",
                KeyCode._6 => "6",
                KeyCode._7 => "7",
                KeyCode._8 => "8",
                KeyCode._9 => "9",
                KeyCode._0 => "0",
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
                KeyCode.KeypadPeriod => ".",
                KeyCode.Keypad0 => "0",
                KeyCode.Keypad1 => "1",
                KeyCode.Keypad2 => "2",
                KeyCode.Keypad3 => "3",
                KeyCode.Keypad4 => "4",
                KeyCode.Keypad5 => "5",
                KeyCode.Keypad6 => "6",
                KeyCode.Keypad7 => "7",
                KeyCode.Keypad8 => "8",
                KeyCode.Keypad9 => "9",
                KeyCode.KeypadMinus => "-",
                KeyCode.KeypadPlus => "+",
                KeyCode.F1 => "",
                KeyCode.F2 => "",
                KeyCode.F3 => "",
                KeyCode.F4 => "",
                KeyCode.F5 => "",
                KeyCode.F6 => "",
                KeyCode.F7 => "",
                KeyCode.F8 => "",
                KeyCode.F9 => "",
                KeyCode.F10 => "",
                KeyCode.F11 => "",
                KeyCode.F12 => "",
                KeyCode.LeftCTRL => "",
                KeyCode.LeftShift => "",
                KeyCode.RightShift => "",
                KeyCode.LeftAlt => "",
                KeyCode.NumLock => "",
                KeyCode.ScrollLock => "",
                KeyCode.CapsLock => "",
                KeyCode.Delete => "",
                KeyCode.Enter => "",
                KeyCode.ESC => "",
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
            _1 = 0x02,
            _2 = 0x03,
            _3 = 0x04,
            _4 = 0x05,
            _5 = 0x06,
            _6 = 0x07,
            _7 = 0x08,
            _8 = 0x09,
            _9 = 0x0A,
            _0 = 0x0B,
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
            KeypadPeriod = 0x53,
            Keypad0 = 0x52,
            Keypad3 = 0x51,
            Keypad2 = 0x50,
            Keypad1 = 0x4F,
            KeypadPlus = 0x4E,
            Keypad6 = 0x4D,
            Keypad5 = 0x4C,
            Keypad4 = 0x4B,
            KeypadMinus = 0x4A,
            Keypad9 = 0x49,
            Keypad8 = 0x48,
            Keypad7 = 0x47,
            CapsLock = 0x3A,
            Delete = 0x0E,
            Space = 0x39,
            Enter = 0x1C,
            ESC = 0x01
        }
    }
}
