// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.CompilerServices;

namespace System
{
	public static class BitConverter
	{
		public static readonly bool IsLittleEndian = true;

		// Converts a Boolean into an array of bytes with length one.
		public static byte[] GetBytes(bool value)
		{
			byte[] r = new byte[1];
			r[0] = (value ? (byte)1 : (byte)0);
			return r;
		}

		// Converts a char into an array of bytes with length two.
		public static byte[] GetBytes(char value)
		{
			byte[] bytes = new byte[sizeof(char)];
			Unsafe.As<byte, char>(ref bytes[0]) = value;
			return bytes;
		}

		// Converts a short into an array of bytes with length
		// two.
		public static byte[] GetBytes(short value)
		{
			byte[] bytes = new byte[sizeof(short)];
			Unsafe.As<byte, short>(ref bytes[0]) = value;
			return bytes;
		}

		// Converts an int into an array of bytes with length
		// four.
		public static byte[] GetBytes(int value)
		{
			byte[] bytes = new byte[sizeof(int)];
			Unsafe.As<byte, int>(ref bytes[0]) = value;
			return bytes;
		}

		// Converts a long into an array of bytes with length
		// eight.
		public static byte[] GetBytes(long value)
		{
			byte[] bytes = new byte[sizeof(long)];
			Unsafe.As<byte, long>(ref bytes[0]) = value;
			return bytes;
		}

		// Converts an ushort into an array of bytes with
		// length two.
		public static byte[] GetBytes(ushort value)
		{
			byte[] bytes = new byte[sizeof(ushort)];
			Unsafe.As<byte, ushort>(ref bytes[0]) = value;
			return bytes;
		}

		// Converts an uint into an array of bytes with
		// length four.
		public static byte[] GetBytes(uint value)
		{
			byte[] bytes = new byte[sizeof(uint)];
			Unsafe.As<byte, uint>(ref bytes[0]) = value;
			return bytes;
		}

		// Converts an unsigned long into an array of bytes with
		// length eight.
		public static byte[] GetBytes(ulong value)
		{
			byte[] bytes = new byte[sizeof(ulong)];
			Unsafe.As<byte, ulong>(ref bytes[0]) = value;
			return bytes;
		}

		// Converts a float into an array of bytes with length
		// four.
		public static byte[] GetBytes(float value)
		{
			byte[] bytes = new byte[sizeof(float)];
			Unsafe.As<byte, float>(ref bytes[0]) = value;
			return bytes;
		}

		// Converts a double into an array of bytes with length
		// eight.
		public static byte[] GetBytes(double value)
		{
			byte[] bytes = new byte[sizeof(double)];
			Unsafe.As<byte, double>(ref bytes[0]) = value;
			return bytes;
		}

        /* https://referencesource.microsoft.com/#mscorlib/system/bitconverter.cs,e8230d40857425ba */

        // Converts an array of bytes into a short.
        public static unsafe short ToInt16(byte[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if ((uint)startIndex >= value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > value.Length - 2)
                throw new ArgumentException("Start index is too high.");

            fixed (byte* pbyte = &value[startIndex])
            {
                if (startIndex % 2 == 0)
                { // data is aligned 
                    return *((short*)pbyte);
                }
                else
                {
                    if (IsLittleEndian)
                    {
                        return (short)((*pbyte) | (*(pbyte + 1) << 8));
                    }
                    else
                    {
                        return (short)((*pbyte << 8) | (*(pbyte + 1)));
                    }
                }
            }
        }

        // Converts an array of bytes into an int.  
        public static unsafe int ToInt32(byte[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if ((uint)startIndex >= value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > value.Length - 4)
                throw new ArgumentException("Start index is too high.");

            fixed (byte* pbyte = &value[startIndex])
            {
                if (startIndex % 4 == 0)
                { // data is aligned 
                    return *((int*)pbyte);
                }
                else
                {
                    if (IsLittleEndian)
                    {
                        return (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);
                    }
                    else
                    {
                        return (*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3));
                    }
                }
            }
        }

        // Converts an array of bytes into a long.  
        public static unsafe long ToInt64(byte[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if ((uint)startIndex >= value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > value.Length - 8)
                throw new ArgumentException("Start index is too high.");

            fixed (byte* pbyte = &value[startIndex])
            {
                if (startIndex % 8 == 0)
                { // data is aligned 
                    return *((long*)pbyte);
                }
                else
                {
                    if (IsLittleEndian)
                    {
                        int i1 = (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);
                        int i2 = (*(pbyte + 4)) | (*(pbyte + 5) << 8) | (*(pbyte + 6) << 16) | (*(pbyte + 7) << 24);
                        return (uint)i1 | ((long)i2 << 32);
                    }
                    else
                    {
                        int i1 = (*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3));
                        int i2 = (*(pbyte + 4) << 24) | (*(pbyte + 5) << 16) | (*(pbyte + 6) << 8) | (*(pbyte + 7));
                        return (uint)i2 | ((long)i1 << 32);
                    }
                }
            }
        }


        // Converts an array of bytes into an ushort.
        [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if ((uint)startIndex >= value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > value.Length - 2)
                throw new ArgumentException("Start index is too high.");

            return (ushort)ToInt16(value, startIndex);
        }

        // Converts an array of bytes into an uint.
        // 
        [CLSCompliant(false)]
        public static uint ToUInt32(byte[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if ((uint)startIndex >= value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > value.Length - 4)
                throw new ArgumentException("Start index is too high.");

            return (uint)ToInt32(value, startIndex);
        }

        // Converts an array of bytes into an unsigned long.
        // 
        [CLSCompliant(false)]
        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if ((uint)startIndex >= value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > value.Length - 8)
                throw new ArgumentException("Start index is too high.");

            return (ulong)ToInt64(value, startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe long DoubleToInt64Bits(double value)
		{
			return *((long*)&value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe double Int64BitsToDouble(long value)
		{
			return *((double*)&value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe int SingleToInt32Bits(float value)
		{
			return *((int*)&value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe float Int32BitsToSingle(int value)
		{
			return *((float*)&value);
		}
	}
}
