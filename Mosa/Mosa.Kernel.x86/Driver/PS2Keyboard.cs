using Mosa.Kernel.x86;

namespace Mosa.External.x86.Driver
{
    public static partial class PS2Keyboard
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
    }
}
