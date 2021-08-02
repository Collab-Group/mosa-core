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
            KeyAvailable = false;
            KData = IOPort.In8(Port_KeyData);
            KeyAvailable = true;

            IsCapsLock = KData == (byte)KeyCode.CapsLock;
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
                _ => "",
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
            CapsLock = 0x3A,
            Delete = 0x0E,
            Space = 0x39,
            Enter = 0x1C,
            ESC = 0x01
        }
    }
}
