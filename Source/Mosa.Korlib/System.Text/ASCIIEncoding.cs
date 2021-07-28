// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Text
{
	/// <summary>
	/// Implementation of the "ASCIIEncoding" class.
	/// </summary>
	public class ASCIIEncoding : Encoding
	{
		// Decode a buffer of bytes into a string.
		public override string GetString(byte[] bytes, int byteIndex, int count)
		{
			if (count == 0)
				return string.Empty;

			string result = "";

			for (int index = byteIndex; index < byteIndex + count; index++)
			{
				byte b = bytes[index];
				result += new string((b <= 0x7F) ? (char)b : '?', 1);
			}

			return result;
		}
	}
}
