// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Text
{
	/// <summary>
	/// Implementation of the "ASCIIEncoding" class.
	/// </summary>
	public class ASCIIEncoding : Encoding
	{
		public override string GetString(byte[] b, int byteIndex, int count)
		{
			if (count == 0)
				return string.Empty;

			string s = "";
			for (int i = byteIndex; i < byteIndex + count; i++)
			{
				s += (char)b[i];
			}
			return s;
		}
		public override string GetString(byte[] b)
		{
			string s = "";
			for (int i = 0;i<b.Length;i++) s += (char)b[i];
			return s;
		}
		public override byte[] GetBytes(string s)
		{
			byte[] b = new byte[s.Length];
			for (int i = 0;i<s.Length;i++) b[i] = (byte)s[i];
			return b;
		}
		public override char GetChar(byte b) => (char)b;
		public override byte GetByte(char c) => (byte)c;
	}
}
