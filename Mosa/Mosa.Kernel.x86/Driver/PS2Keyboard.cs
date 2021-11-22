using Mosa.Kernel.x86;

namespace Mosa.External.x86.Driver
{
    public static partial class PS2Keyboard
    {
        internal static KeyCode Code;
        internal static byte KData = 0x00;

        public static void OnInterrupt()
        {
            KData = IOPort.In8(0x60);
            if (KData == (byte)KeyCode.CapsLock) IsCapsLock = !IsCapsLock;
            else if (KData == 0x2A /*Left Shift*/ || KData == 0x36 /*Right Shift*/) /* Pressed */
            {
                IsShiftHeld = true;
            }
            else if (KData == 0xAA /*Left Shift*/ || KData == 0xB6 /*Right Shift*/) /* Pressed */
            {
                IsShiftHeld = false;
            }
        }

        public static bool IsCapsLock = false;
        public static bool IsShiftHeld = false;

        public static KeyCode GetKeyPressed()
        {
            Code = (KeyCode)KData;
            KData = 0;
            return Code;
        }

        public static string KeyCodeToString(this KeyCode keyCode)
        {
            bool shouldUppercase = IsShiftHeld ^ IsCapsLock;

            if (keyCode == KeyCode.A) return shouldUppercase ? "A" : "a";
            if (keyCode == KeyCode.B) return shouldUppercase ? "B" : "b";
            if (keyCode == KeyCode.C) return shouldUppercase ? "C" : "c";
            if (keyCode == KeyCode.D) return shouldUppercase ? "D" : "d";
            if (keyCode == KeyCode.E) return shouldUppercase ? "E" : "e";
            if (keyCode == KeyCode.F) return shouldUppercase ? "F" : "f";
            if (keyCode == KeyCode.G) return shouldUppercase ? "G" : "g";
            if (keyCode == KeyCode.H) return shouldUppercase ? "H" : "h";
            if (keyCode == KeyCode.I) return shouldUppercase ? "I" : "i";
            if (keyCode == KeyCode.J) return shouldUppercase ? "J" : "j";
            if (keyCode == KeyCode.K) return shouldUppercase ? "K" : "k";
            if (keyCode == KeyCode.L) return shouldUppercase ? "L" : "l";
            if (keyCode == KeyCode.M) return shouldUppercase ? "M" : "m";
            if (keyCode == KeyCode.N) return shouldUppercase ? "N" : "n";
            if (keyCode == KeyCode.O) return shouldUppercase ? "O" : "o";
            if (keyCode == KeyCode.P) return shouldUppercase ? "P" : "p";
            if (keyCode == KeyCode.Q) return shouldUppercase ? "Q" : "q";
            if (keyCode == KeyCode.R) return shouldUppercase ? "R" : "r";
            if (keyCode == KeyCode.S) return shouldUppercase ? "S" : "s";
            if (keyCode == KeyCode.T) return shouldUppercase ? "T" : "t";
            if (keyCode == KeyCode.U) return shouldUppercase ? "U" : "u";
            if (keyCode == KeyCode.V) return shouldUppercase ? "V" : "v";
            if (keyCode == KeyCode.W) return shouldUppercase ? "W" : "w";
            if (keyCode == KeyCode.X) return shouldUppercase ? "X" : "x";
            if (keyCode == KeyCode.Y) return shouldUppercase ? "Y" : "y";
            if (keyCode == KeyCode.Z) return shouldUppercase ? "Z" : "z";
            if (keyCode == KeyCode.Space) return " ";
            if (keyCode == KeyCode.Num1) return IsShiftHeld ? "!" : "1";
            if (keyCode == KeyCode.Num2) return IsShiftHeld ? "@" : "2";
            if (keyCode == KeyCode.Num3) return IsShiftHeld ? "#" : "3";
            if (keyCode == KeyCode.Num4) return IsShiftHeld ? "$" : "4";
            if (keyCode == KeyCode.Num5) return IsShiftHeld ? "%" : "5";
            if (keyCode == KeyCode.Num6) return IsShiftHeld ? "^" : "6";
            if (keyCode == KeyCode.Num7) return IsShiftHeld ? "&" : "7";
            if (keyCode == KeyCode.Num8) return IsShiftHeld ? "*" : "8";
            if (keyCode == KeyCode.Num9) return IsShiftHeld ? "(" : "9";
            if (keyCode == KeyCode.Num0) return IsShiftHeld ? ")" : "0";
            if (keyCode == KeyCode.Dash) return IsShiftHeld ? "_" : "-";
            if (keyCode == KeyCode.Equals) return IsShiftHeld ? "+" : "=";
            if (keyCode == KeyCode.Tab) return "    ";
            if (keyCode == KeyCode.RightFacedSquareBracket) return IsShiftHeld ? "{" : "[";
            if (keyCode == KeyCode.LeftFacedSquareBracket) return IsShiftHeld ? "}" : "]";
            if (keyCode == KeyCode.SemiColon) return IsShiftHeld ? ":" : ";";
            if (keyCode == KeyCode.SingleQuote) return IsShiftHeld ? "\"" : "'";
            if (keyCode == KeyCode.BackTick) return IsShiftHeld ? "~" : "`";
            if (keyCode == KeyCode.BackSlash) return IsShiftHeld ? "|" : "\\";
            if (keyCode == KeyCode.Comma) return IsShiftHeld ? "<" : ",";
            if (keyCode == KeyCode.Period) return IsShiftHeld ? ">" : ".";
            if (keyCode == KeyCode.ForwardSlash) return IsShiftHeld ? "?" : "/";
            if (keyCode == KeyCode.Star) return "*";
            if (keyCode == KeyCode.NumPadPeriod) return ".";
            if (keyCode == KeyCode.NumPad0) return "0";
            if (keyCode == KeyCode.NumPad1) return "1";
            if (keyCode == KeyCode.NumPad2) return "2";
            if (keyCode == KeyCode.NumPad3) return "3";
            if (keyCode == KeyCode.NumPad4) return "4";
            if (keyCode == KeyCode.NumPad5) return "5";
            if (keyCode == KeyCode.NumPad6) return "6";
            if (keyCode == KeyCode.NumPad7) return "7";
            if (keyCode == KeyCode.NumPad8) return "8";
            if (keyCode == KeyCode.NumPad9) return "9";
            if (keyCode == KeyCode.NumPadMinus) return "-";
            if (keyCode == KeyCode.NumPadPlus) return "+";
            else return string.Empty;

        }
    }

}
