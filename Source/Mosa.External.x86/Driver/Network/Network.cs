using Mosa.External.x86.Networking;
using System.Runtime.InteropServices;

namespace Mosa.External.x86.Driver
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Buffer
    {
        public fixed byte Data[NetworkManager.BufferSize - 1 /* There's the pack of the variable */];
    }

    public unsafe abstract class Network
    {
        public int MaxPacketSize;

        public abstract void OnInterrupt();

        public unsafe abstract bool SendPacket(Buffer* buffer, uint length);
    }
}
