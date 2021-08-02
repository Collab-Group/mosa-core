using System;

namespace Mosa.External.x86.Networking
{
    public class MACAddress
    {
        private static Random Random;

        public static string[] GetRandomAddress()
        {
            if (Random == null)
                Random = new Random();

            string[] macBytes = new[]
            {
                Random.Next(1, 256).ToString("X2"),
                Random.Next(1, 256).ToString("X2"),
                Random.Next(1, 256).ToString("X2"),
                Random.Next(1, 256).ToString("X2"),
                Random.Next(1, 256).ToString("X2"),
                Random.Next(1, 256).ToString("X2")
            };

            return macBytes;
        }
    }
}
