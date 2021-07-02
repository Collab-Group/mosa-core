// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Compiler.Framework.Transform.Auto.ConstantFolding
{
	/// <summary>
	/// ConvertU64ToR8
	/// </summary>
	public sealed class ConvertU64ToR8 : BaseTransformation
	{
		public ConvertU64ToR8() : base(IRInstruction.ConvertU64ToR8)
		{
		}

		public override bool Match(Context context, TransformContext transformContext)
		{
			if (!IsResolvedConstant(context.Operand1))
				return false;

			return true;
		}

		public override void Transform(Context context, TransformContext transformContext)
		{
			var result = context.Result;

			var t1 = context.Operand1;

			var e1 = transformContext.CreateConstant(ToR8(To64(t1)));

			context.SetInstruction(IRInstruction.MoveR8, result, e1);
		}
	}
}