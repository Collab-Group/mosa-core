using Mosa.Runtime;
using System;
using System.Collections.Generic;

namespace Mosa.Kernel.x86
{
    // This is a crude implementation of a format string based struct converter for C#.
    // This is probably not the best implementation, the fastest implementation, the most bug-proof implementation, or even the most functional implementation.
    // It's provided as-is for free. Enjoy.

    // https://stackoverflow.com/questions/28225303/equivalent-in-c-sharp-of-pythons-struct-pack-unpack#28418846

    public class StructConverter
    {
        // We use this function to provide an easier way to type-agnostically call the GetBytes method of the BitConverter class.
        // This means we can have much cleaner code below.
        private static byte[] TypeAgnosticGetBytes(object o)
        {
            if (o is int @int) return BitConverter.GetBytes(@int);
            if (o is uint int1) return BitConverter.GetBytes(int1);
            if (o is long int2) return BitConverter.GetBytes(int2);
            if (o is ulong int3) return BitConverter.GetBytes(int3);
            if (o is short int4) return BitConverter.GetBytes(int4);
            if (o is ushort int5) return BitConverter.GetBytes(int5);
            if (o is byte || o is sbyte) return new byte[] { (byte)o };

            throw new ArgumentException("Unsupported object type found");
        }

        private static string GetFormatSpecifierFor(object o)
        {
            if (o is int) return "i";
            if (o is uint) return "I";
            if (o is long) return "q";
            if (o is ulong) return "Q";
            if (o is short) return "h";
            if (o is ushort) return "H";
            if (o is byte) return "B";
            if (o is sbyte) return "b";

            throw new ArgumentException("Unsupported object type found");
        }

        /// <summary>
        /// Convert a byte array into an array of objects based on Python's "struct.unpack" protocol.
        /// </summary>
        /// <param name="fmt">A "struct.pack"-compatible format string</param>
        /// <param name="bytes">An array of bytes to convert to objects</param>
        /// <returns>Array of objects.</returns>
        /// <remarks>You are responsible for casting the objects in the array back to their proper types.</remarks>
        public static object[] Unpack(string fmt, byte[] bytes)
        {
            // First we parse the format string to make sure it's proper.
            if (fmt.Length < 1)
                throw new ArgumentException("Format string cannot be empty.");

            bool endianFlip;
            if (fmt.Substring(0, 1) == "<")
            {
                // Little endian.
                // Do we need to flip endianness?
                if (!BitConverter.IsLittleEndian)
                    endianFlip = true;

                fmt = fmt.Substring(1);
            }
            else if (fmt.Substring(0, 1) == ">")
            {
                // Big endian.
                // Do we need to flip endianness?
                if (BitConverter.IsLittleEndian)
                    endianFlip = true;

                fmt = fmt.Substring(1);
            }

            // Now, we find out how long the byte array needs to be
            int totalByteLength = 0;
            foreach (char c in fmt)
                switch (c)
                {
                    case 'q':
                    case 'Q':
                        totalByteLength += 8;
                        break;
                    case 'i':
                    case 'I':
                        totalByteLength += 4;
                        break;
                    case 'h':
                    case 'H':
                        totalByteLength += 2;
                        break;
                    case 'b':
                    case 'B':
                    case 'x':
                        totalByteLength += 1;
                        break;
                    default:
                        throw new ArgumentException("Invalid character found in format string.");
                }

            // Test the byte array length to see if it contains as many bytes as is needed for the string.
            if (bytes.Length != totalByteLength)
                throw new ArgumentException("The number of bytes provided does not match the total length of the format string.");

            // Ok, we can go ahead and start parsing bytes!
            int byteArrayPosition = 0;
            List<object> outputList = new List<object>();
            byte[] buf;

            foreach (char c in fmt)
                switch (c)
                {
                    case 'q':
                        outputList.Add(BitConverter.ToInt64(bytes, byteArrayPosition));
                        byteArrayPosition += 8;
                        break;
                    case 'Q':
                        outputList.Add(BitConverter.ToUInt64(bytes, byteArrayPosition));
                        byteArrayPosition += 8;
                        break;
                    case 'l':
                        outputList.Add(BitConverter.ToInt32(bytes, byteArrayPosition));
                        byteArrayPosition += 4;
                        break;
                    case 'L':
                        outputList.Add(BitConverter.ToUInt32(bytes, byteArrayPosition));
                        byteArrayPosition += 4;
                        break;
                    case 'h':
                        outputList.Add(BitConverter.ToInt16(bytes, byteArrayPosition));
                        byteArrayPosition += 2;
                        break;
                    case 'H':
                        outputList.Add(BitConverter.ToUInt16(bytes, byteArrayPosition));
                        byteArrayPosition += 2;
                        break;
                    case 'b':
                        buf = new byte[1];
                        Array.Copy(bytes, byteArrayPosition, buf, 0, 1);
                        outputList.Add((sbyte)buf[0]);
                        byteArrayPosition++;
                        break;
                    case 'B':
                        buf = new byte[1];
                        Array.Copy(bytes, byteArrayPosition, buf, 0, 1);
                        outputList.Add(buf[0]);
                        byteArrayPosition++;
                        break;
                    case 'x':
                        byteArrayPosition++;
                        break;
                    default:
                        throw new ArgumentException("You should not be here.");
                }

            return outputList.ToArray();
        }

        /// <summary>
        /// Convert an array of objects to a byte array, along with a string that can be used with Unpack.
        /// </summary>
        /// <param name="items">An object array of items to convert</param>
        /// <param name="LittleEndian">Set to False if you want to use big endian output.</param>
        /// <param name="NeededFormatStringToRecover">Variable to place an 'Unpack'-compatible format string into.</param>
        /// <returns>A Byte array containing the objects provided in binary format.</returns>
        public static byte[] Pack(object[] items, bool LittleEndian, out string NeededFormatStringToRecover)
        {
            // make a byte list to hold the bytes of output
            List<byte> outputBytes = new List<byte>();

            // should we be flipping bits for proper endinanness?
            bool endianFlip = LittleEndian != BitConverter.IsLittleEndian;

            // start working on the output string
            string outString = !LittleEndian ? ">" : "<";

            // convert each item in the objects to the representative bytes
            foreach (object o in items)
            {
                byte[] theseBytes = TypeAgnosticGetBytes(o);

                if (endianFlip)
                {
                    int n = theseBytes.Length;
                    byte[] current = theseBytes;

                    for (int i = 0; i < n; i++)
                        theseBytes[n - 1 - i] = current[i];

                    GC.DisposeObject(current);
                }

                outString += GetFormatSpecifierFor(o);
                outputBytes.AddRange(theseBytes);
            }

            NeededFormatStringToRecover = outString;

            return outputBytes.ToArray();

        }

        public static byte[] Pack(object[] items)
        {
            return Pack(items, true, out _);
        }
    }
}