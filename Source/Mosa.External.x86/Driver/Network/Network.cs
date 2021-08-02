namespace Mosa.External.x86.Driver
{
    public unsafe abstract class Network
    {
        public int MaxPacketSize;

        public abstract void OnInterrupt();

        public unsafe abstract bool SendPacket(byte* buffer, uint length);
    }
}
