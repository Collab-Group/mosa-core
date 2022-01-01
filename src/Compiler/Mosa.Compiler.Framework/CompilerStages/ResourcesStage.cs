using Mosa.Compiler.Common;
using Mosa.Compiler.Framework.Linker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mosa.Compiler.Framework.CompilerStages
{
    class ResourcesStage : BaseCompilerStage
	{
		private PatchType NativePatchType;

		protected override void Initialization()
		{
			NativePatchType = (TypeLayout.NativePointerSize == 4) ? PatchType.I32 : NativePatchType = PatchType.I64;
		}

		protected override void Finalization()
        {
            var v = Linker.DefineSymbol(Metadata.ResourcesTable, SectionKind.ROData, TypeLayout.NativePointerAlignment, 0);
            BinaryWriter writer = new BinaryWriter(v.Stream);

            writer.Write(Files.Count, 4);

            foreach (var val in Files) 
            {
                AddFile(writer, val, Compiler.SearchPathsForFileAndLoad(val));
            }
        }

        public static List<string> Files = new List<string>();

        private static void AddFile(BinaryWriter writer,string Name,byte[] Content)
        {
            if(Content == null) throw new FileNotFoundException($"Please Copy The File: {Name} To bin Folder");

            byte[] NameB = Encoding.ASCII.GetBytes(Name);

            //namesize:uint
            //filesize:uint
            //byte[] name
            //byte[] data

            writer.Write(NameB.Length, 4);
            writer.Write(Content.Length, 4);

            writer.Write(NameB);
            writer.Write(Content);
        }
    }
}
