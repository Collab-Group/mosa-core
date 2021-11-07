// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Runtime
{
    public static class ObjectImpl
    {
        private static uint Size;
        private static uint Address;

        public static unsafe void Dispose(this object obj) 
        {
            if (obj == null) return;

            Size = obj.TypeDefinition->SizeOf;
            Address = (uint)Intrinsic.GetObjectAddress(obj);
            obj = null;
            GC.Dispose(Address, Size);
        }
    }
}
