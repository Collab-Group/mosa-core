// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Compiler.Framework.Transform.Auto.ConstantFolding
{
	/// <summary>
	/// AddCarryIn32Outside1
	/// </summary>
	public sealed class AddCarryIn32Outside1 : BaseTransformation
	{
		public AddCarryIn32Outside1() : base(IRInstruction.AddCarryIn32)
		{
		}

		public override bool Match(Context context, TransformContext transformContext)
		{
			if (!IsResolvedConstant(context.Operand1))
				return false;

			if (!IsResolvedConstant(context.Operand3))
				return false;

			return true;
		}

		public override void Transform(Context context, TransformContext transformContext)
		{
			var result = context.Result;

			var t1 = context.Operand1;
			var t2 = context.Operand2;
			var t3 = context.Operand3;

			var e1 = transformContext.CreateConstant(Add32(To32(t1), BoolTo32(To32(t3))));

			context.SetInstruction(IRInstruction.Add32, result, t2, e1);
		}
	}
}