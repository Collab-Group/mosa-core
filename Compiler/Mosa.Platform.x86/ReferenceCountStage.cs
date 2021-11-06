using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86
{
    public static class ReferenceCountStage
    {
        public static void AddReferenceCount(InstructionNode node, OpcodeEncoder opcodeEncoder)
        {
            //Maybe not safe
            //if (node.Operand1.Type.Namespace.Contains("MOSA1")) 
            if (
                !node.Operand1.IsStaticField &&
                node.Operand1.IsReferenceType &&
                node.Operand1.Type.BaseType != null &&
                node.Operand1.Register != null &&
                node.Operand1.Register.RegisterCode == 0 && //0 Is EAX Register
                node.Operand1.Type.BaseType.FullName == "System.Object" &&
                //TODO - String ReferenceCount
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
