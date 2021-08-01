using Mosa.Kernel.x86;

namespace Mosa.External.x86.Driver
{
    public static class PCSpeaker
    {
        public static void PlaySound(uint nFrequence)
        {
            uint Div;
            byte tmp;

            Div = PIT.Frequency / nFrequence;
            IOPort.Out8(0x43, 0xb6);
            IOPort.Out8(0x42, (byte)(Div));
            IOPort.Out8(0x42, (byte)(Div >> 8));

            tmp = IOPort.In8(0x61);
            if (tmp != (tmp | 3))
            {
                IOPort.Out8(0x61, (byte)(tmp | 3));
            }
        }

        public static void Shutup()
        {
            byte tmp = (byte)(IOPort.In8(0x61) & 0xFC);

            IOPort.Out8(0x61, tmp);
        }

        public static void Beep(uint Frequency,uint PlayMS)
        {
            PlaySound(Frequency);
            PIT.Wait(PlayMS);
            Shutup();
        }
    }
}
