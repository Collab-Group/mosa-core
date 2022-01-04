// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Text
{
	/// <summary>
	/// Implementation of the "Encoding" class.
	/// </summary>
	public abstract class Encoding
	{
		public static ASCIIEncoding ASCII;
		public static void Setup() => ASCII = new ASCIIEncoding();
		public abstract string GetString(byte[] b);
		public abstract byte[] GetBytes(string s);
		public abstract char GetChar(byte b);
		public abstract byte GetByte(char c);
	}
}
