// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace System.Runtime.InteropServices
{
    public sealed class UnmanagedCallersOnlyAttribute : Attribute
    {
        public string EntryPoint;
        public CallingConvention CallingConvention;

        public UnmanagedCallersOnlyAttribute() { }
    }
}