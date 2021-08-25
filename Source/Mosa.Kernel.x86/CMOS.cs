// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime.x86;

namespace Mosa.Kernel.x86
{
    /// <summary>
    ///
    /// </summary>
    public static class CMOS
    {
        private static byte B;

        /// <summary>
        /// Gets the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static byte Get(byte index)
        {
            //Native.Cli();
            Native.Out8(0x70, index);
            Native.Nop();
            Native.Nop();
            Native.Nop();
            byte result = Native.In8(0x71);

            //Native.Sti();
            return result;
        }

        /// <summary>
        /// Sets the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static void Set(byte index, byte value)
        {
            //Native.Cli();
            Native.Out8(0x70, index);
            Native.Nop();
            Native.Nop();
            Native.Nop();
            Native.Out8(0x71, value);

            //Native.Sti();
        }

        /// <summary>
        /// Delays the io bus.
        /// </summary>
        private static void Delay()
        {
            Native.In8(0x80);
            Native.Out8(0x80, 0);
        }

        /// <summary>
        /// Gets the second.
        /// </summary>
        /// <value>The second.</value>
        public static byte Second
        {
            get
            {
                B = Get(0);
                return (byte)((B & 0x0F) + ((B / 16) * 10));
            }
        }

        /// <summary>
        /// Gets the minute.
        /// </summary>
        /// <value>The minute.</value>
        public static byte Minute
        {
            get
            {
                B = Get(2);
                return (byte)((B & 0x0F) + ((B / 16) * 10));
            }
        }

        /// <summary>
        /// Gets the hour.
        /// </summary>
        /// <value>The hour.</value>
        public static byte Hour
        {
            get
            {
                B = Get(4);
                return (byte)(((B & 0x0F) + ((B & 0x70) / 16 * 10)) | (B & 0x80));
            }
        }

        /// <summary>
        /// Gets the year.
        /// </summary>
        /// <value>The year.</value>
        public static byte Year
        {
            get
            {
                B = Get(9);
                return (byte)((B & 0x0F) + ((B / 16) * 10));
            }
        }

        /// <summary>
        /// Gets the month.
        /// </summary>
        /// <value>The month.</value>
        public static byte Month
        {
            get
            {
                B = Get(8);
                return (byte)((B & 0x0F) + ((B / 16) * 10));
            }
        }

        /// <summary>
        /// Gets the day.
        /// </summary>
        /// <value>The day.</value>
        public static byte Day
        {
            get
            {
                B = Get(7);
                return (byte)((B & 0x0F) + ((B / 16) * 10));
            }
        }

        /// <summary>
        /// Gets the BCD.
        /// </summary>
        /// <value>The BCD.</value>
        public static bool BCD { get { return (Get(0x0B) & 0x04) == 0x00; } }
    }
}
