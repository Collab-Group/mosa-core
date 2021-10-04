using Mosa.Kernel.x86;
using Mosa.Runtime;
using System.Runtime.InteropServices;
using static Mosa.Runtime.x86.ASM;

namespace Mosa.External.x86.Networking
{
    public static unsafe class Ethernet
    {
        public static byte[] Mask;
        public static byte[] Gateway;
        public static byte[] IPAddress;
        //Must be set by network device
        public static byte[] MACAddress;

        public static byte[] BroadIP;
        public static byte[] BroadMAC;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct EthernetHeader
        {
            public fixed byte DestMAC[6];
            public fixed byte SrcMAC[6];
            public ushort EthernetType;
        }

        public static ushort SwapLeftRight(ushort Value)
        {
            return ((ushort)((((Value) & 0xff) << 8) | (((Value) & 0xff00) >> 8)));
        }

        public struct Type
        {
            public const ushort IPv4 = 0x0800;
            public const ushort IPv6 = 0x86DD;
            public const ushort ARP = 0x0806;
        }

        public static void Init(byte[] aIPAddress,byte[] aGateway,byte[] aMask)
        {
            Mask = aMask;
            Gateway = aGateway;
            IPAddress = aIPAddress;
            BroadIP = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            BroadMAC = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            MACAddress = new byte[6];

            Console.Write("IP Address: ");
            for (int i = 0; i < 4; i++) Console.Write($"{IPAddress[i]}{((i == 3) ? "" : ".")}");
            Console.WriteLine();
        }

        internal static unsafe void HandlePacket(byte* buffer, ushort length)
        {
            EthernetHeader* header = (EthernetHeader*)buffer;
            if (
                (header->DestMAC[0] == 0xFF &&
                header->DestMAC[1] == 0xFF &&
                header->DestMAC[2] == 0xFF &&
                header->DestMAC[3] == 0xFF &&
                header->DestMAC[4] == 0xFF &&
                header->DestMAC[5] == 0xFF)
                ||
                (header->DestMAC[0] == MACAddress[0] &&
                header->DestMAC[1] == MACAddress[1] &&
                header->DestMAC[2] == MACAddress[2] &&
                header->DestMAC[3] == MACAddress[3] &&
                header->DestMAC[4] == MACAddress[4] &&
                header->DestMAC[5] == MACAddress[5]
                )
                )
            {
                buffer += sizeof(EthernetHeader);
                length -= (ushort)sizeof(EthernetHeader);
                switch (SwapLeftRight(header->EthernetType))
                {
                    case Type.IPv4:
                        IPv4.HandlePacket(buffer, length);
                        break;
                    case Type.ARP:
                        ARP.HandlePacket(buffer);
                        break;
                }
            }
        }

        public static void SendPacket(byte[] DestMAC, ushort Type, byte* Payload, ushort PayloadLength)
        {
            if (EthernetController.Controller == null) return;

            //Max Packet Size
            byte* buffer = (byte*)GC.AllocateObject(ushort.MaxValue);
            EthernetHeader* header = (EthernetHeader*)buffer;
            for (int i = 0; i < 6; i++) header->DestMAC[i] = DestMAC[i];
            for (int i = 0; i < 6; i++) header->SrcMAC[i] = MACAddress[i];
            header->EthernetType = SwapLeftRight(Type);
            MEMCPY((uint)(buffer + sizeof(EthernetHeader)), (uint)Payload, PayloadLength);

            EthernetController.Controller.Send(buffer, (ushort)(sizeof(EthernetHeader) + PayloadLength));
            GC.Dispose((uint)buffer, ushort.MaxValue);
        }
    }
}
