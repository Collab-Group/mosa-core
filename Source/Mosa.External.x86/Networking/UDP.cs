using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System.Runtime.InteropServices;
using static Mosa.Runtime.x86.ASM;

namespace Mosa.External.x86.Networking
{
    public static unsafe class UDP
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UDPHeader
        {
            public ushort SrcPort;
            public ushort DestPort;
            public ushort Length;
            public ushort Checksum;
        }

        public static void SendPacket(byte[] DestIP, ushort SourcePort, ushort DestPort, byte[] Data)
        {
            uint PacketLen = (uint)(sizeof(UDPHeader) + Data.Length);
            byte* Buffer = (byte*)GC.AllocateObject(PacketLen);
            UDPHeader* header = (UDPHeader*)Buffer;
            MEMFILL((uint)header, PacketLen, 0);
            header->SrcPort = Ethernet.SwapLeftRight(SourcePort);
            header->DestPort = Ethernet.SwapLeftRight(DestPort);
            header->Length = Ethernet.SwapLeftRight(((ushort)PacketLen));
            header->Checksum = 0;
            for (int i = 0; i < Data.Length; i++) (Buffer + sizeof(UDPHeader))[i] = Data[i];

            IPv4.SendPacket(DestIP, 17, Buffer, PacketLen);

            Console.WriteLine("UDP Packet Sent");
        }

        public delegate void PacketReceivedHandler(byte[] Buffer, ushort Port);
        public static PacketReceivedHandler ReceivedHandler;

        public static void SetReceivedHandler(PacketReceivedHandler handler) 
        {
            ReceivedHandler = handler;
        }

        internal static void HandlePacket(byte* frame, ushort length)
        {
            UDPHeader* header = (UDPHeader*)frame;
            Console.WriteLine("UDP Packet Received");
            frame += sizeof(UDPHeader);
            length -= (ushort)sizeof(UDPHeader);

            byte[] Buffer = new byte[length];
            fixed (byte* P = Buffer)
            {
                ASM.MEMCPY((uint)P, (uint)frame, length);
            }
            ReceivedHandler?.Invoke(Buffer, Ethernet.SwapLeftRight(header->DestPort));
            GC.DisposeObject(Buffer);
        }
    }
}
