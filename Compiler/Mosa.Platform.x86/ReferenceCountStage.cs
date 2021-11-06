using Mosa.Compiler.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mosa.Platform.x86
{
    public static class ReferenceCountStage
    {
        public static void AddReferenceCount(InstructionNode node, OpcodeEncoder opcodeEncoder)
		{
			//if (node.Operand1.Type.Namespace.Contains("MOSA1")) 
			if (
				node.Operand1.Type.BaseType != null &&
				node.Operand1.Type.BaseType.FullName == "System.Object" &&
				node.Operand1.Type.FullName != "System.String"
				)
				//Debugger.Break();
				opcodeEncoder.AppendBytes(new byte[]
				{
					0x83 ,0x40 ,0x04 ,0x01
				});
		}
    }
}
