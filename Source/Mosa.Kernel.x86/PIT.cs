using Mosa.Runtime.x86;

namespace Mosa.Kernel.x86
{
    public class PIT
    {
        public const byte SquareWave = 0x36;
        public const uint Frequency = 1193182;
        //No More Than 1000
        public const ushort Hz = 200;
        public static ushort modeControlPort;
        public static ushort counter0Divisor;
        public static ulong tickCount;

        public static bool isWaitting = false;

        internal static void Setup()
        {
            modeControlPort = 0x43;
            counter0Divisor = 0x40;


            ushort timerCount = (ushort)(Frequency / Hz);

            IOPort.Out8(modeControlPort, SquareWave);
            IOPort.Out8(counter0Divisor, (byte)(timerCount & 0xFF));
            IOPort.Out8(counter0Divisor, (byte)((timerCount & 0xFF00) >> 8));

            tickCount = 0;
        }

        internal static void OnInterrupt()
        {
            if (isWaitting)
            {
                tickCount += 1000 / Hz;
            }
        }

        public static void Wait(uint millisecond)
        {
            tickCount = 0;
            isWaitting = true;
            while (tickCount < millisecond)
            {
            }
            isWaitting = false;
        }
    }
}