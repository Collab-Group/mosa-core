// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Compiler.Framework.Transform.Auto.StrengthReduction
{
	/// <summary>
	/// Add32Zero
	/// </summary>
	public sealed class Add32Zero : BaseTransformation
	{
		public Add32Zero() : base(IRInstruction.Add32)
		{
		}

		public override bool Match(Context context, TransformContext transformContext)
		{
			if (!context.Operand2.IsResolvedConstant)
				return false;

			if (context.Operand2.ConstantUnsigned64 != 0)
				return false;

			return true;
		}

		public override void Transform(Context context, TransformContext transformContext)
		{
			var result = context.Result;

			var t1 = context.Operand1;

			context.SetInstruction(IRInstruction.Move32, result, t1);
		}
	}

	/// <summary>
	/// Add32Zero_v1
	/// </summary>
	public sealed class Add32Zero_v1 : BaseTransformation
	{
		public Add32Zero_v1() : base(IRInstruction.Add32)
		{
		}

		public override bool Match(Context context, TransformContext transformContext)
		{
			if (!context.Operand1.IsResolvedConstant)
				return false;

			if (context.Operand1.ConstantUnsigned64 != 0)
				return false;

			return true;
		}

		public override void Transform(Context context, TransformContext transformContext)
		{
			var result = context.Result;

			var t1 = context.Operand2;

			context.SetInstruction(IRInstruction.Move32, result, t1);
		}
	}
}