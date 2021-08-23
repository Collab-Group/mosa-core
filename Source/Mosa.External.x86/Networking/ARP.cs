using Mosa.External.x86;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Mosa.Runtime.x86.ASM;

namespace Mosa.External.x86.Networking
{
    public static unsafe class ARP
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ARPHeader
        {
            public ushort HardwareType;
            public ushort Protocol;
            public byte HardwareAddrLen;
            public byte ProtocolAddrLen;
            public ushort Operation;
            public fixed byte SourceMAC[6];
            public fixed byte SourceIP[4];
            public fixed byte DestMAC[6];
            public fixed byte DestIP[4];
        }

        public static void Init()
        {
            ARPEntries = new List<ARPEntry>();
            Lookup(Ethernet.Gateway);
        }

        public struct ARPEntry
        {
            public byte[] IP;
            public byte[] MAC;
        }

        public const ushort Reply = 2;
        public const ushort Request = 1;

        public static List<ARPEntry> ARPEntries;

        //Memory leak risks
        internal static void HandlePacket(byte* addr)
        {
            ARPHeader* header = (ARPHeader*)addr;
            if (Ethernet.SwapLeftRight(header->Operation) == Request) goto Request;

            Reply:
            Console.WriteLine("ARP Reply Received");
            for (int i = 0; i < 4; i++) Console.Write(header->SourceIP[i] + ((i == 3) ? "" : "."));
            Console.Write(" Is Pointing To ");
            for (int i = 0; i < 6; i++) Console.Write(header->SourceMAC[i].ToString("x2").PadLeft(2, '0') + ((i == 5) ? "" : ":"));
            Console.WriteLine();

            ARPEntry entry = new ARPEntry()
            {
                IP = new byte[]
                {
                    header->SourceIP[0],
                    header->SourceIP[1],
                    header->SourceIP[2],
                    header->SourceIP[3]
                },
                MAC = new byte[]
                {
                    header->SourceMAC[0],
                    header->SourceMAC[1],
                    header->SourceMAC[2],
                    header->SourceMAC[3],
                    header->SourceMAC[4],
                    header->SourceMAC[5]
                }
            };
            ARPEntries.Add(entry);
            return;

        Request:
            header->Operation = Ethernet.SwapLeftRight(Reply);
            MEMCPY((uint)header->DestMAC, (uint)header->SourceMAC, 6);
            for (int i = 0; i < 6; i++) header->SourceMAC[i] = Ethernet.MACAddress[i];

            MEMCPY((uint)header->DestIP, (uint)header->SourceIP, 4);
            for (int i = 0; i < 4; i++) header->SourceIP[i] = Ethernet.IPAddress[i];

            byte[] DMAC = new byte[]
            {
                header->DestMAC[0],
                header->DestMAC[1],
                header->DestMAC[2],
                header->DestMAC[3],
                header->DestMAC[4],
                header->DestMAC[5]
            };
            Ethernet.SendPacket(DMAC, Ethernet.Type.ARP, addr, (ushort)sizeof(ARPHeader));
            Console.WriteLine("ARP Reply Sent");
        }

        public static byte[] Lookup(byte[] IP)
        {
            foreach (var v in ARPEntries)
            {
                if (
                    v.IP[0] == IP[0] &&
                    v.IP[1] == IP[1] &&
                    v.IP[2] == IP[2] &&
                    v.IP[3] == IP[3]
                    )
                {
                    return v.MAC;
                }
            }

            Console.WriteLine("Waitting For ARP Packet Received");
            SendRequest(IP);

        Retry:
            foreach (var v in ARPEntries)
            {
                if (
                    v.IP[0] == IP[0] &&
                    v.IP[1] == IP[1] &&
                    v.IP[2] == IP[2] &&
                    v.IP[3] == IP[3]
                    )
                {
                    return v.MAC;
                }
            }
            goto Retry;
        }

        public static void SendRequest(byte[] DestIP)
        {
            byte* Buffer = (byte*)GC.AllocateObject((uint)sizeof(ARPHeader));
            ARPHeader* header = (ARPHeader*)Buffer;

            for (int i = 0; i < 6; i++) header->SourceMAC[i] = Ethernet.MACAddress[i];

            header->SourceIP[0] = Ethernet.IPAddress[0];
            header->SourceIP[1] = Ethernet.IPAddress[1];
            header->SourceIP[2] = Ethernet.IPAddress[2];
            header->SourceIP[3] = Ethernet.IPAddress[3];

            for (int i = 0; i < 4; i++) header->DestIP[i] = DestIP[i];
            for (int i = 0; i < 6; i++) header->DestMAC[i] = Ethernet.BroadMAC[i];

            header->Operation = Ethernet.SwapLeftRight(Request);

            header->HardwareAddrLen = 6;
            header->ProtocolAddrLen = 4;

            header->HardwareType = Ethernet.SwapLeftRight(1);

            header->Protocol = Ethernet.SwapLeftRight(Ethernet.Type.IPv4);

            Ethernet.SendPacket(Ethernet.BroadMAC, Ethernet.Type.ARP, Buffer, (ushort)sizeof(ARPHeader));

            GC.Free((uint)Buffer, (uint)sizeof(ARPHeader));
        }
    }
}
