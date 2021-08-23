using Mosa.Kernel.x86;
using Mosa.Runtime;
using System.Runtime.InteropServices;
using static Mosa.Runtime.x86.ASM;

namespace Mosa.External.x86.Networking
{
    public static unsafe class IPv4
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct IPv4Header
        {
            public byte VersionAndIHL;
            public byte DSCPAndECN;
            public ushort TotalLength;
            public ushort ID;
            public ushort FlagAndFragmentOffset;
            public byte TimeToLive;
            public byte Protocol;
            public ushort HeaderChecksum;
            public fixed byte SourceIP[4];
            public fixed byte DestIP[4];
        }

        public static void SendPacket(byte[] DestIP, byte Protocol, byte* Payload, uint PayloadLength)
        {
            byte* Buffer = (byte*)GC.AllocateObject(2048);
            MEMFILL((uint)Buffer, 2048, 0);

            IPv4Header* header = (IPv4Header*)Buffer;

            header->VersionAndIHL = 0x45;

            header->TotalLength = Ethernet.SwapLeftRight((ushort)(sizeof(IPv4Header) + PayloadLength));
            //Why 188? I just like 188 Because 188 sound like my name in Chinese. Try to use google translate and translate 1,8,8 in Chinese
            header->TimeToLive = 188;
            header->Protocol = Protocol;
            for (int i = 0; i < Ethernet.IPAddress.Length; i++)
            {
                header->SourceIP[i] = Ethernet.IPAddress[i];
            }
            for (int i = 0; i < DestIP.Length; i++)
            {
                header->DestIP[i] = DestIP[i];
            }
            header->HeaderChecksum = CalculateChecksum((byte*)header, sizeof(IPv4Header));

            for (int i = 0; i < PayloadLength; i++)
            {
                (Buffer + sizeof(IPv4Header))[i] = Payload[i];
            }

            byte[] MACAddr;

            if (IsSameSubnet(DestIP, Ethernet.IPAddress))
            {
                MACAddr = ARP.Lookup(DestIP);
            }
            else
            {
                MACAddr = ARP.Lookup(Ethernet.Gateway);
            }

            Ethernet.SendPacket(MACAddr, Ethernet.Type.IPv4, Buffer, (ushort)(sizeof(IPv4Header) + PayloadLength));

            Console.WriteLine("IP Packet Sent");
        }

        public struct Protocol
        {
            public const byte ICMP = 1;
            public const byte UDP = 17;
        }

        //Memory leak risks
        internal static void HandlePacket(byte* buffer, ushort length)
        {
            IPv4Header* header = (IPv4Header*)buffer;
            buffer += sizeof(IPv4Header);
            length -= (ushort)sizeof(IPv4Header);
            switch (header->Protocol)
            {
                case Protocol.ICMP:
                    //Request
                    if (buffer[0] == 8)
                    {
                        buffer[0] = 0;
                        *((ushort*)(buffer + 2)) = 0;
                        *((ushort*)(buffer + 2)) = CalculateChecksum(buffer, length);

                        byte[] srcIP = new byte[]
                        {
                            header->SourceIP[0],
                            header->SourceIP[1],
                            header->SourceIP[2],
                            header->SourceIP[3]
                        };
                        IPv4.SendPacket(srcIP, 1, buffer, length);

                        Console.WriteLine("ICMP Request Replied");
                    }
                    break;
                case Protocol.UDP:
                    UDP.HandlePacket(buffer, length);
                    break;
            }
        }

        public static bool IsSameSubnet(byte[] ip1, byte[] ip2)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((ip1[i] & Ethernet.Mask[i]) != (ip2[i] & Ethernet.Mask[i])) return false;
            }

            return true;
        }

        public static ushort CalculateChecksum(byte* addr, int count)
        {
            uint sum = 0;
            ushort* ptr = (ushort*)addr;

            while (count > 1)
            {
                sum += *ptr++;
                count -= 2;
            }

            if (count > 0)
                sum += *(byte*)ptr;

            while ((sum >> 16) != 0)
                sum = (sum & 0xffff) + (sum >> 16);

            return (ushort)~sum;
        }
    }
}
