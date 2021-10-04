using Mosa.Runtime.x86;
using System.Runtime.InteropServices;

namespace Mosa.Kernel.x86
{
    //Physical Address Extension
    //Allow 32 bit operating systems to make use of more than 4 GB of memory.
    //At least 64GB up to 17179869184GB(As same as 64bit operating system)
    //TODO - How?
    /*
    public static class PAE
    {
        [DllImport("PAE.o")]
        public static extern void EnablePAE();

        public static void Setup() 
        {
        }
    }
    */
}
