using Mosa.External.x86.Networking;
using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;

namespace Mosa.External.x86.Driver
{
    //https://wiki.osdev.org/AMD_PCNET
    // Some code is from Cosmos, see below
    public unsafe class PCNETII : Network
    {
        private uint IOBase;

        private MemoryBlock InitBlock;

        private uint RxBufferPtr = 0;                // Pointer to receive buffers
        private uint TxBufferPtr = 0;                // Pointer to transmit buffers

        private int RxBufferCount = 32;              // Maximum amount of receive buffers
        private int TxBufferCount = 8;               // Maximum amount of transmit buffers

        private const int DESize = 16;                      // Length of descriptor entry

        private byte* Rdes;                          // Pointer to ring buffer of receive DEs
        private byte* Tdes;                          // Pointer to ring buffer of transmit DEs

        private byte* RxBuffers;                   // Physical address of actual receive buffers (< 4 GiB)
        private byte* TxBuffers;                   // Physical address of actual transmit buffers (< 4 GiB)

        public byte IRQ { get; private set; }

        public PCNETII()
        {
            PCIDevice device = PCI.GetDevice(VendorID.AMD, DeviceID.PCNETII);

            if (device == null)
            {
                Console.WriteLine("PCI device AMD PCNETII not found :/");
                return;
            }

            device.EnableDevice();

            IOBase = device.BAR0;
            IRQ = device.InterruptLine;

            MaxPacketSize = 1520;

            // Read from reset register to reset card (32-bit first then 16-bit)
            _ = Native.In32((ushort)(IOBase + 0x18));
            _ = Native.In16((ushort)(IOBase + 0x14));

            PIT.Wait(10); // Wait 10ms for the card to reset, just to be sure

            // Program card into 32-bit mode
            Native.Out32((ushort)(IOBase + 0x10), 0);

            // Set SWSTYLE to 2 so that it can access 4 GB of physical memory (it sets to 32-bit mode)
            uint csr58 = ReadCSR32(58);
            csr58 &= 0xFF00; // SWSTYLE is 8 bits (7:0)
            csr58 |= 2;
            WriteCSR32(58, csr58);

            // ASEL
            uint bcr2 = ReadBCR32(2);
            bcr2 |= 0x2;
            WriteBCR32(2, bcr2);

            // Generate random MAC address
            string[] mac = MACAddress.GetRandomAddress();
            byte[] macAddress = new byte[mac.Length];

            for (int i = 0; i < mac.Length; i++)
                macAddress[i] = (byte)TableConvert.Convert(mac[i]);

            // Initialize receive and transmit descriptors
            Rdes = (byte*)GC.AllocateObject(DESize);
            Tdes = (byte*)GC.AllocateObject(DESize);

            InitDE(Rdes, (int)Rdes, false);
            InitDE(Tdes, (int)Tdes, true);

            // Initialize receive and transmit buffers
            RxBuffers = (byte*)GC.AllocateObject((uint)RxBufferCount);
            TxBuffers = (byte*)GC.AllocateObject((uint)TxBufferCount);

            RxBufferPtr = (uint)RxBuffers;
            TxBufferPtr = (uint)TxBuffers;

            // Initialize init structure
            /* Start Cosmos code */
            InitBlock = new MemoryBlock(28);

            InitBlock.Write32(0x00, (0x4 << 28) | (0x4 << 20));
            InitBlock.Write32(0x04, (uint)(macAddress[0] | (macAddress[1] << 8) | (macAddress[2] << 16) | (macAddress[3] << 24)));
            InitBlock.Write32(0x08, (uint)(macAddress[4] | (macAddress[5] << 8)));
            InitBlock.Write32(0x0C, 0x0);
            InitBlock.Write32(0x10, 0x0);
            InitBlock.Write32(0x14, (uint)Rdes);
            InitBlock.Write32(0x18, (uint)Tdes);
            /* End Cosmos code */

            uint address = (uint)InitBlock.Address;

            // Actually set up the card registers
            WriteCSR32(1, address >> 16 & 0xFF);
            WriteCSR32(2, address & 0xFF);

            Console.WriteLine("Successfully initialized and enabled the AMD PCNETII card!");
        }

        // Does the driver own the particular buffer?
        private bool DriverOwns(byte* des, uint idx)
        {
            return (des[DESize * idx + 7] & 0x80) == 0;
        }

        // Get the next transmit buffer index
        private uint NextTxIdx(uint curTtxIdx)
        {
            uint ret = curTtxIdx + 1;
            if (ret == TxBufferCount)
                ret = 0;
            return ret;
        }

        // Get the next receive buffer index
        private uint NextRxIdx(uint curRxIdx)
        {
            uint ret = curRxIdx + 1;
            if (ret == RxBufferCount)
                ret = 0;
            return ret;
        }

        private void InitDE(byte* des, int idx, bool is_tx)
        {
            des[idx * DESize] = 0;

            // First 4 bytes are the physical address of the actual buffer
            uint buf_addr = (uint)RxBuffers;
            if (is_tx)
                buf_addr = (uint)TxBuffers;
            *(uint*)&des[idx * DESize] = (uint)(buf_addr + idx * NetworkManager.BufferSize);

            // Next 2 bytes are 0xf000 OR'd with the first 12 bits of the 2s complement of the length
            ushort bcnt = unchecked((ushort)-NetworkManager.BufferSize);
            bcnt &= 0x0fff;
            bcnt |= 0xf000;
            *(ushort*)&des[idx * DESize + 4] = bcnt;

            // Finally, set ownership bit - transmit buffers are owned by us, receive buffers by the card
            if (!is_tx)
                des[idx * DESize + 7] = 0x80;
        }

        public override void OnInterrupt()
        {
            while (DriverOwns(Rdes, RxBufferPtr))
            {
                // Packet length is given by bytes 8 and 9 of the descriptor
                //  (no need to negate it unlike BCNT above)
                ushort plen = *(ushort*)&Rdes[RxBufferPtr * DESize + 8]; // We don't have any use for it, for now

                // The packet itself is written somewhere in the receive buffer
                byte* pbuf = (byte*)((uint)RxBuffers + RxBufferPtr * NetworkManager.BufferSize);

                // Handle the packet
                NetworkManager.AwaitingBuffers.Add(NetworkManager.BytePointer2ByteArray(pbuf, plen));

                // Hand the buffer back to the card
                Rdes[RxBufferPtr * DESize + 7] = 0x80;

                // Increment rx_buffer_ptr;
                RxBufferPtr = NextRxIdx(RxBufferPtr);
            }

            // Set interrupt as handled
            WriteCSR32(0, ReadCSR32(0) | 0x0400);

            // EOI is sent automatically, so no need to do it manually
        }

        public override bool SendPacket(byte* packet, uint len)
        {
            // The next available descriptor entry index is in tx_buffer_ptr
            if (!DriverOwns(Tdes, TxBufferPtr))
            {
                // We don't own the next buffer, this implies all the transmit
                //  buffers are full and the card hasn't sent them yet.
                // A fully functional driver would therefore add the packet to
                //  a queue somewhere, and wait for the transmit done interrupt
                //  then try again.  We simply fail and return.  You can set
                //  bit 3 of CSR0 here to encourage the card to send all buffers.
                return false;
            }

            // Copy the packet data to the transmit buffer.  An alternative would
            //  be to update the appropriate transmit DE to point to 'packet', but
            //  then you would need to ensure that packet is not invalidated before
            //  the card has a chance to send the data.
            ASM.MEMCPY((uint)((uint)TxBuffers + TxBufferPtr * NetworkManager.BufferSize), (uint)packet, len);

            // Set the STP bit in the descriptor entry (signals this is the first
            //  frame in a split packet - we only support single frames)
            Tdes[TxBufferPtr * DESize + 7] |= 0x2;

            // Similarly, set the ENP bit to state this is also the end of a packet
            Tdes[TxBufferPtr * DESize + 7] |= 0x1;

            // Set the BCNT member to be 0xf000 OR'd with the first 12 bits of the
            //  two's complement of the length of the packet
            ushort bcnt = (ushort)-len;
            bcnt &= 0xfff;
            bcnt |= 0xf000;
            *(ushort*)&Tdes[TxBufferPtr * DESize + 4] = bcnt;

            // Finally, flip the ownership bit back to the card
            Tdes[TxBufferPtr * DESize + 7] |= 0x80;

            // Update the next transmit pointer
            TxBufferPtr = NextTxIdx(TxBufferPtr);

            return true;
        }

        public void WriteRAP32(uint value)
        {
            Native.Out32((ushort)(IOBase + 0x14), value);
        }

        public uint ReadCSR32(uint csrNo)
        {
            WriteRAP32(csrNo);
            return Native.In32((ushort)(IOBase + 0x10));
        }

        public void WriteCSR32(uint csrNo, uint value)
        {
            WriteRAP32(csrNo);
            Native.Out32((ushort)(IOBase + 0x10), value);
        }

        public uint ReadBCR32(uint bcrNo)
        {
            WriteRAP32(bcrNo);
            return Native.In32((ushort)(IOBase + 0x1c));
        }

        public void WriteBCR32(uint bcrNo, uint value)
        {
            WriteRAP32(bcrNo);
            Native.Out32((ushort)(IOBase + 0x1c), value);
        }
    }
}
