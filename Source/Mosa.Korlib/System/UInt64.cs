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

        public override string ToString()
        {
			int count = 0;
			ulong tmp = this;
			do
			{
				tmp /= 10;
				count++;
			} while (tmp != 0);

			string s = "";
			ulong temp = this;

			for(int i = 0; i < count; i++) 
			{
				//ASCII
				s += (char)((temp % 10) + 0x30);
				temp /= 10;
			}

			string r = "";

			for (int i = 0; i < s.length; i++)
			{
				r += s[s.length - 1 - i];
			}

			return r;
		}
    }
}
