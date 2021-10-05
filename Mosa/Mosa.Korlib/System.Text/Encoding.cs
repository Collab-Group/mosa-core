﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Text
{
	/// <summary>
	/// Implementation of the "Encoder" class.
	/// </summary>
	public abstract class Encoding
	{
		public static ASCIIEncoding ASCII;

		public static void Setup()
        {
			ASCII = new ASCIIEncoding();
        }

		public abstract string GetString(byte[] bytes, int index, int count);

		public virtual string GetString(byte[] bytes)
		{
			return GetString(bytes, 0, bytes.Length);
		}
	}
}