// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Metadata
{
	[StructLayout(LayoutKind.Sequential,Pack = 1)]
	public unsafe struct TypeDefinitionNew
	{
		public void* Name;
		public void* CustomAttributes;
		public void* TypeCodeAndAttributes;
		public uint SizeOf;
		public void* Assembly;
		public void* ParentType;
		public void* DeclaringType;
		public void* ElementType;
		public void* DefaultConstructor;
		public void* Properties;
		public void* Fields;
		public void* SlotTable;
		public void* Bitmap;
		public uint NumberOfMethods;
	}
}
