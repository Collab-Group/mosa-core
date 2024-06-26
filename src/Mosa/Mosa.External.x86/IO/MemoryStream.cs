// Copyright (c) MOSA Project. Licensed under the New BSD License.
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System.Collections.Generic;

namespace System.IO
{
    // Implementation of MemoryStream as MemoryTributary

    /// <summary>
    /// MemoryTributary is a re-implementation of MemoryStream that uses a dynamic list of byte arrays as a backing store, instead of a single byte array, the allocation
    /// of which will fail for relatively small streams as it requires contiguous memory.
    /// </summary>
    public unsafe class MemoryStream : Stream       /* http://msdn.microsoft.com/en-us/library/system.io.stream.aspx */
    {
        #region Constructors

        public MemoryStream()
        {
            Position = 0;
        }

        public MemoryStream(byte[] source)
        {
            this.Write(source, 0, source.Length);
            Position = 0;
        }

        /* length is ignored because capacity has no meaning unless we implement an artifical limit */
        public MemoryStream(int length)
        {
            SetLength(length);
            Position = length;
            byte[] d = block;   //access block to prompt the allocation of memory
            Position = 0;
        }

        #endregion

        #region Status Properties

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        #endregion

        #region Public Properties

        public override long Length
        {
            get { return length; }
        }

        public override long Position { get; set; }

        #endregion

        #region Members

        protected long length = 0;

        protected long blockSize = 65536;

        protected List<byte[]> blocks = new List<byte[]>();

        #endregion

        #region Internal Properties

        /* Use these properties to gain access to the appropriate block of memory for the current Position */

        /// <summary>
        /// The block of memory currently addressed by Position
        /// </summary>
        protected byte[] block
        {
            get
            {
                while (blocks.Count <= blockId)
                    blocks.Add(new byte[blockSize]);
                return blocks[(int)blockId];
            }
        }
        /// <summary>
        /// The id of the block currently addressed by Position
        /// </summary>
        protected long blockId
        {
            get { return Position / blockSize; }
        }
        /// <summary>
        /// The offset of the byte currently addressed by Position, into the block that contains it
        /// </summary>
        protected long blockOffset
        {
            get { return Position % blockSize; }
        }

        #endregion

        #region Public Stream Methods

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long lcount = (long)count;

            if (lcount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Number of bytes to copy cannot be negative.");
            }

            long remaining = (length - Position);
            if (lcount > remaining)
                lcount = remaining;

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "Buffer cannot be null.");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Destination offset cannot be negative.");
            }

            int read = 0;
            long copysize = 0;
            do
            {
                copysize = Math.Min(lcount, (blockSize - blockOffset));

                fixed (byte* ptr = buffer)
                    fixed (byte* b = block)
                        ASM.MEMCPY((uint)((uint)ptr + offset),
                        (uint)((uint)b + (int)blockOffset),
                        (uint)copysize);

                lcount -= copysize;
                offset += (int)copysize;

                read += (int)copysize;
                Position += copysize;

            } while (lcount > 0);

            return read;

        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            long initialPosition = Position;
            int copysize;
            try
            {
                do
                {
                    copysize = Math.Min(count, (int)(blockSize - blockOffset));

                    EnsureCapacity(Position + copysize);

                    fixed (byte* ptr = buffer)
                        fixed (byte* b = block)
                            ASM.MEMCPY((uint)((uint)b + (int)blockOffset),
                            (uint)((uint)ptr + offset),
                            (uint)copysize);

                    count -= copysize;
                    offset += copysize;

                    Position += copysize;

                } while (count > 0);
            }
            catch (Exception e)
            {
                Position = initialPosition;
                throw e;
            }
        }

        public override int ReadByte()
        {
            if (Position >= length)
                return -1;

            byte b = block[blockOffset];
            Position++;

            return b;
        }

        public override void WriteByte(byte value)
        {
            EnsureCapacity(Position + 1);
            block[blockOffset] = value;
            Position++;
        }

        // TODO: Implement
        public override void CopyTo(Stream destination)
        {
            throw new NotImplementedException();
        }

        protected void EnsureCapacity(long intended_length)
        {
            if (intended_length > length)
                length = (intended_length);
        }

        #endregion

        #region IDispose

        /* http://msdn.microsoft.com/en-us/library/fs2xkftw.aspx */
        protected override void Dispose(bool disposing)
        {
            if (disposing) // If Dispose() is called from another Dispose method and not a finalizer
                blocks.Dispose();
                //GC.Dispose(blocks);

                /* We do not currently use unmanaged resources */
            base.Dispose(disposing);
        }

        #endregion

        #region Public Additional Helper Methods

        /// <summary>
        /// Returns the entire content of the stream as a byte array. This is not safe because the call to new byte[] may 
        /// fail if the stream is large enough. Where possible use methods which operate on streams directly instead.
        /// </summary>
        /// <returns>A byte[] containing the current data in the stream</returns>
        public byte[] ToArray()
        {
            long firstposition = Position;
            Position = 0;
            byte[] destination = new byte[Length];
            Read(destination, 0, (int)Length);
            Position = firstposition;
            return destination;
        }

        public List<byte[]> GetRawBlocks()
        {
            return blocks;
        }

        /// <summary>
        /// Reads length bytes from source into the this instance at the current position.
        /// </summary>
        /// <param name="source">The stream containing the data to copy</param>
        /// <param name="length">The number of bytes to copy</param>
        public void ReadFrom(Stream source, long length)
        {
            byte[] buffer = new byte[4096];
            int read;
            do
            {
                read = source.Read(buffer, 0, (int)Math.Min(4096, length));
                length -= read;
                this.Write(buffer, 0, read);

            } while (length > 0);
        }

        /// <summary>
        /// Writes the entire stream into destination, regardless of Position, which remains unchanged.
        /// </summary>
        /// <param name="destination">The stream to write the content of this stream to</param>
        public void WriteTo(Stream destination)
        {
            long initialpos = Position;
            Position = 0;
            this.CopyTo(destination);
            Position = initialpos;
        }

        #endregion
    }
}

