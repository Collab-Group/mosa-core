// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework.IR;

namespace Mosa.Compiler.Framework.Transform.Auto.IR.Simplification
{
	/// <summary>
	/// MoveObjectPropagation
	/// </summary>
	public sealed class MoveObjectPropagation : BaseTransformation
	{
		public MoveObjectPropagation() : base(IRInstruction.MoveObject)
		{
		}

		public override bool Match(Context context, TransformContext transformContext)
		{
			if (!context.Operand1.IsVirtualRegister)
				return false;

			if (context.Operand1.Definitions.Count != 1)
				return false;

			if (context.Operand1.Definitions[0].Instruction != IRInstruction.MoveObject)
				return false;

			return true;
		}

		public override void Transform(Context context, TransformContext transformContext)
		{
			var result = context.Result;

			var t1 = context.Operand1.Definitions[0].Operand1;

			context.SetInstruction(IRInstruction.MoveObject, result, t1);
		}
	}
}
