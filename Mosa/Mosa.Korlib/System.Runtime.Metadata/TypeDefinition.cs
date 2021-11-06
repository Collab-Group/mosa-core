// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.InteropServices;

namespace System.Runtime.Metadata
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TypeDefinition
    {
        public void* Name;
        public void* CustomAttributes;
        public void* TypeCodeAndAttributes;
        private uint _SizeOf;
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

        public uint SizeOf { get => _SizeOf + (uint)(sizeof(void*) * 2); }
    }
}
