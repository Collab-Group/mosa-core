// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System
{
	/// <summary>
	///
	/// </summary>
	public struct UInt64
	{
		public const ulong MaxValue = 0xffffffffffffffff;
		public const ulong MinValue = 0;

		internal ulong _value;

		public int CompareTo(ulong value)
		{
			if (_value < value) return -1;
			else if (_value > value) return 1;
			return 0;
		}

		public bool Equals(ulong obj)
		{
			return Equals((object)obj);
		}

		public override bool Equals(object obj)
		{
			return ((ulong)obj) == _value;
		}

		public override int GetHashCode()
		{
			return (int)_value;
		}

		//Not Available Until GC Is Setup
        public override string ToString()
        {
			string s = "";
			ulong temp = this;

			do
			{
				s += (char)((temp % 10) + 0x30);
				temp /= 10;
			} while (temp != 0);

			string r = "";

			for (int i = 0; i < s.Length; i++)
			{
				r += s[s.Length - 1 - i];
			}

			return r;
		}
    }
}
