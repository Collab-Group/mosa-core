// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Runtime
{
    public static class ObjectImpl 
    {
        public static void Dispose(this object obj) 
        {
#pragma warning disable 
            GC.Dispose(obj);
#pragma warning restore
        }
    }
}
