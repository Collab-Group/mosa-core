// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Transform;

namespace Mosa.Platform.x86.Transform.Manual
{
	/// <summary>
	/// Dec32
	/// </summary>
	public sealed class Sub32ToDec32 : BaseTransformation
	{
		public Sub32ToDec32() : base(X86.Sub32, true)
		{
		}

		public override bool Match(Context context, TransformContext transformContext)
		{
			if (!context.Operand2.IsResolvedConstant)
				return false;

			if (context.Operand2.ConstantUnsigned64 != 1)
				return false;

			if (!(AreStatusFlagsUsed(context.Node.Next, true, false, true, true, true) == TriState.No))
				return false;

			return true;
		}

		public override void Transform(Context context, TransformContext transformContext)
		{
			var result = context.Result;

			var t1 = context.Operand1;

			context.SetInstruction(X86.Dec32, result, t1, t1);
		}
	}
}