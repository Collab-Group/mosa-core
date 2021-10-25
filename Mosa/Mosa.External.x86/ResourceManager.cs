using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mosa.External.x86
{
    public static class ResourceManager
    {
        private struct Res 
        {
            public string Name;
            public byte[] Data;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Pointer GetResourcesPtr();

        private static List<Res> Resources = null;

        public static byte[] GetObject(string Name) 
        {
            if (Resources == null) Initialize();

            for(int i = 0; i < Resources.Count; i++) 
            {
                if(Resources[i].Name == Name) 
                {
                    return Resources[i].Data;
                }
            }

            return null;
        }

        private static unsafe void Initialize() 
        {
            Resources = new List<Res>();

            Pointer ptr = GetResourcesPtr();

            uint ResourcesNum = ptr.Load32();

            ptr += sizeof(uint);

            for (int i = 0; i < ResourcesNum; i++)
            {
                uint NameSize = ptr.Load32();
                ptr += sizeof(uint);
                uint FileSize = ptr.Load32();
                ptr += sizeof(uint);

                Res res = new Res();
                res.Name = string.FromPointer((byte*)ptr, (int)NameSize);
                ptr += NameSize;

                res.Data = new byte[FileSize];
                fixed (byte* p = res.Data) 
                {
                    ASM.MEMCPY((uint)p, (uint)ptr, FileSize);
                }
                ptr += FileSize;

                Resources.Add(res);
            }
            
            Console.WriteLine($"ResourcesNum: {ResourcesNum}");
        }
    }
}
