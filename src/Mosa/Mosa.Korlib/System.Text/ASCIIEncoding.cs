// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Text
{
	/// <summary>
	/// Implementation of the "ASCIIEncoding" class.
	/// </summary>
	public static class ASCIIEncoding : Encoding
	{
		public static override string GetString(byte[] b)
		{
			string s = "";
			for (int i = 0;i<b.Length;i++) s += (char)bytes[i];
			return s;
		}
		public static override byte[] GetBytes(string s)
		{
			byte[] b = new byte[s.Length];
			for (int i = 0;i<s.Length;i++) b[i] = (byte)s[i];
			return b;
		}
		public static char GetChar(byte b) => (char)b;
		public static byte GetByte(char c) => (byte)c;
	}
}
