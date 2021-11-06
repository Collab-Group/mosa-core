// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.InteropServices;

namespace System.Runtime.Metadata
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TypeDefinition
    {
        public void* Name { get; }
        public void* CustomAttributes { get; }
        public void* TypeCodeAndAttributes { get; }
        private uint _SizeOf { get; }
        public void* Assembly { get; }
        public void* ParentType { get; }
        public void* DeclaringType { get; }
        public void* ElementType { get; }
        public void* DefaultConstructor { get; }
        public void* Properties { get; }
        public void* Fields { get; }
        public void* SlotTable { get; }
        public void* Bitmap { get; }
        public uint NumberOfMethods { get; }

        public uint SizeOf { get => _SizeOf + (uint)(sizeof(void*) * 2); }
    }
}
