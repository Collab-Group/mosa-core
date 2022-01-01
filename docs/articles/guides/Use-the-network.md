### Configuration
- First. Get know your Host IP
![image](https://github.com/nifanfa/MOSA-Core/blob/master/Wiki/Images/GIF%202021-8-23%20%E4%B8%8B%E5%8D%88%2005-49-37.gif?raw=true)
- Second. Switch network mode to Host-Only
![image](https://github.com/nifanfa/MOSA-Core/blob/master/Wiki/Images/GIF%202021-8-23%20%E4%B8%8B%E5%8D%88%2005-50-50.gif?raw=true)

### Initialization
```cs
            Ethernet.Initialize(
                new byte[] { 192, 168, 56, 188 }, //Your IP. Choose one whatever you like !
                new byte[] { 192, 168, 56, 1 },   //The Host IP.
                new byte[] { 255, 255, 255, 0 }); //NetMask
            EthernetController.Initialize();
            ARP.Initialize();
```

- After that let's try if it is ready
- You can ping it. it should response your ping
![image](https://github.com/nifanfa/MOSA-Core/blob/master/Wiki/Images/QQ%E6%88%AA%E5%9B%BE20210823181456.png?raw=true)

## Example of communication between Windows and MOSA
### MOSA
```cs
// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86.Networking;
using Mosa.Kernel.x86;
using Mosa.Runtime.Plug;
using Mosa.Runtime.x86;
using System.Threading;

namespace MOSA28
{
    public static unsafe class Program
    {
        public static void Main() { }

        [Plug("Mosa.Runtime.StartUp::KMain")]
        public static void KMain() 
        {
            new Thread(MainThread).Start();
            for (; ; );
        }

        public static void MainThread()
        {
            Ethernet.Initialize(
                new byte[] { 192, 168, 56, 188 },
                new byte[] { 192, 168, 56, 1 },
                new byte[] { 255, 255, 255, 0 });
            EthernetController.Initialize();
            ARP.Initialize();

            UDP.OnReceived += ReceivedHandler;

            for (; ; ) Native.Hlt();
        }

        public static void ReceivedHandler(byte[] Buffer, ushort Port)
        {
            if (Port == 54188)
            {
                foreach (var v in Buffer) Console.Write(((char)v).ToString());
                Console.WriteLine();
            }
        }
    }
}
```
### Windows Console App
###
```cs
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient udp = new UdpClient();

            IPEndPoint host = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 54188);
            IPEndPoint endp = new IPEndPoint(IPAddress.Parse("192.168.56.188"), 54188);

            udp.Client.Bind(host);
            byte[] buffer;
            new Thread(() =>
            {
                while (((buffer = udp.Receive(ref endp)) != null)) Console.WriteLine(Encoding.ASCII.GetString(buffer));
            }).Start();

            for(; ; ) 
            {
                string s = Console.ReadLine();
                byte[] b = new byte[s.Length];
                for (int i = 0; i < s.Length; i++)
                {
                    b[i] = (byte)s[i];
                }
                udp.Send(b, b.Length, endp);
            }
        }
    }
}

```
### Supported Controllers:
- Intel82542(0x1000)
- Intel82543GC(0x1001)
- Intel82543GC(0x1004)
- Intel82544EI(0x1008)
- Intel82544EI(0x1009)
- Intel82543EI(0x100C)
- Intel82544GC(0x100D)
- Intel82540EM(0x100E)
- Intel82545EM(0x100F)
- Intel82546EB(0x1010)
- Intel82545EM(0x1011)
- Intel82546EB(0x1012)
- Intel82541EI(0x1013)
- Intel82541ER(0x1014)
- Intel82540EM(0x1015)
- Intel82540EP(0x1016)
- Intel82540EP(0x1017)
- Intel82541EI(0x1018)
- Intel82547EI(0x1019)
- Intel82547EI(0x101A)
- Intel82546EB(0x101D)
- Intel82540EP(0x101E)
- Intel82545GM(0x1026)
- Intel82545GM(0x1027)
- Intel82545GM(0x1028)
- Intel82566MM_ICH8(0x1049)
- Intel82566DM_ICH8(0x104A)
- Intel82566DC_ICH8(0x104B)
- Intel82562V_ICH8(0x104C)
- Intel82566MC_ICH8(0x104D)
- Intel82571EB(0x105E)
- Intel82571EB(0x105F)
- Intel82571EB(0x1060)
- Intel82547EI(0x1075)
- Intel82541GI(0x1076)
- Intel82547EI(0x1077)
- Intel82541ER(0x1078)
- Intel82546EB(0x1079)
- Intel82546EB(0x107A)
- Intel82546EB(0x107B)
- Intel82541PI(0x107C)
- Intel82572EI(0x107D)
- Intel82572EI(0x107E)
- Intel82572EI(0x107F)
- Intel82546GB(0x108A)
- Intel82573E(0x108B)
- Intel82573E(0x108C)
- Intel80003ES2LAN(0x1096)
- Intel80003ES2LAN(0x1098)
- Intel82546GB(0x1099)
- Intel82573L(0x109A)
- Intel82571EB(0x10A4)
- Intel82575(0x10A7)
- Intel82575_serdes(0x10A9)
- Intel82546GB(0x10B5)
- Intel82572EI(0x10B9)
- Intel80003ES2LAN(0x10BA)
- Intel80003ES2LAN(0x10BB)
- Intel82571EB(0x10BC)
- Intel82566DM_ICH9(0x10BD)
- Intel82562GT_ICH8(0x10C4)
- Intel82562G_ICH8(0x10C5)
- Intel82576(0x10C9)
- Intel82574L(0x10D3)
- Intel82575_quadcopper(0x10A9)
- Intel82567V_ICH9(0x10CB)
- Intel82567LM_4_ICH9(0x10E5)
- Intel82577LM(0x10EA)
- Intel82577LC(0x10EB)
- Intel82578DM(0x10EF)
- Intel82578DC(0x10F0)
- Intel82567LM_ICH9_egDellE6400Notebook(0x10F5)
- Intel82579LM(0x1502)
- Intel82579V(0x1503)
- Intel82576NS(0x150A)
- Intel82580(0x150E)
- IntelI350(0x1521)
- IntelI210(0x1533)
- IntelI210(0x157B)
- IntelI217LM(0x153A)
- IntelI217VA(0x153B)
- IntelI218V(0x1559)
- IntelI218LM(0x155A)
- IntelI218LM2(0x15A0)
- IntelI218V(0x15A1)
- IntelI218LM3(0x15A2)
- IntelI218V3(0x15A3)
- IntelI219LM(0x156F)
- IntelI219V(0x1570)
- IntelI219LM2(0x15B7)
- IntelI219V2(0x15B8)
- IntelI219LM3(0x15BB)
- IntelI219LM(0x15D7)
- IntelI219LM(0x15E3)