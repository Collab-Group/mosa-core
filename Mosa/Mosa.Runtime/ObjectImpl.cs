// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Runtime
{
    public static class ObjectImpl 
    {
        public static unsafe void Dispose(this object obj) 
        {
            if (obj == null) return;

            uint Address = (uint)Intrinsic.GetObjectAddress(obj);
            GC.Dispose(Address, obj.TypeDefinition->SizeOf);
        }
    }
}
