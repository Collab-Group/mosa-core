﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System
{
	/// <summary>
	/// Implementation of the "System.Delegate" class.
	/// </summary>
	public class Delegate
	{
		protected uint methodPointer = 0;
		protected object instance = null;

		internal Delegate()
		{
		}

		internal Delegate(object instance, uint methodPointer)
		{
			this.instance = instance;
			this.methodPointer = methodPointer;
		}

		public static Delegate Combine(Delegate a, Delegate b) 
		{
			return b;
		}
		public static Delegate Remove(Delegate source, Delegate value) 
		{
			return null;
		}
	}
}
