using Mosa.External.x86.Driver;
using Mosa.Runtime.x86;

namespace Mosa.External.x86
{
    public static class Power
    {
        public static void Reboot() 
        {
            //Use PS2 Controller To Reboot
            while ((Native.In8(0x64) & 0x02) != 0) ;
            Native.Out8(0x64, 0xFE);
            Native.Hlt();
        }

        public static void Shutdown() 
        {
            ACPI.Shutdown();
        }
    }
}
