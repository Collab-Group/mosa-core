// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using System.Collections.Generic;

namespace Mosa.Compiler.Framework.Transform.Manual
{
	/// <summary>
	/// Transformations
	/// </summary>
	public static class ManualTransforms
	{
		public static readonly List<BaseTransformation> List = new List<BaseTransformation>
		{
			new ConstantMove.Compare32x32(),
			new ConstantMove.Compare32x64(),
			new ConstantMove.Compare64x32(),
			new ConstantMove.Compare64x64(),

			new ConstantMove.Branch32(),
			new ConstantMove.Branch64(),

			new ConstantFolding.Compare32x32(),
			new ConstantFolding.Compare32x64(),
			new ConstantFolding.Compare64x32(),
			new ConstantFolding.Compare64x64(),

			new ConstantFolding.Branch32(),
			new ConstantFolding.Branch64(),

			new Special.Deadcode(),
			new Special.GetLow32From64(),

			new Simplification.AddCarryOut32CarryNotUsed(),
			new Simplification.AddCarryOut64CarryNotUsed(),
			new Simplification.SubCarryOut32CarryNotUsed(),
			new Simplification.SubCarryOut64CarryNotUsed(),

			new Special.Move32Propagate(),
			new Special.Move32PropagateConstant(),
			new Special.Move64Propagate(),
			new Special.Move64PropagateConstant(),
			new Special.MoveR4Propagate(),
			new Special.MoveR8Propagate(),

			new Special.Phi32Propagate(),
			new Special.Phi64Propagate(),
			new Special.PhiObjectPropagate(),
			new Special.PhiR4Propagate(),
			new Special.PhiR8Propagate(),

			new Special.Phi32Dead(),
			new Special.Phi64Dead(),
			new Special.PhiR4Dead(),
			new Special.PhiR8Dead(),

			new Special.Phi32Update(),
			new Special.Phi64Update(),
			new Special.PhiR4Update(),
			new Special.PhiR8Update(),

			new Simplification.Branch32OnlyOneExit(),
			new Simplification.Branch64OnlyOneExit(),
			new Simplification.BranchObjectOnlyOneExit(),

			new Rewrite.Branch32(),
			new Rewrite.Branch64(),

			new Rewrite.Branch32Object(),
			new Rewrite.Branch64Object(),

			new Special.MoveCompoundPropagate(),

			new Rewrite.Branch32From64(),
			new Rewrite.Branch64From32(),

			new Special.MoveObjectPropagate(),
			new Special.MoveObjectPropagateConstant(),

			new Rewrite.Compare32x32Combine32x32(),
			new Rewrite.Compare32x32Combine64x32(),
			new Rewrite.Compare32x32Combine32x64(),
			new Rewrite.Compare64x64Combine32x32(),
			new Rewrite.Compare64x64Combine64x32(),
			new Rewrite.Compare64x64Combine64x64(),

			new Rewrite.Branch32Combine32x32(),
			new Rewrite.Branch32Combine32x64(),
			new Rewrite.Branch32Combine64x32(),
			new Rewrite.Branch32Combine64x64(),
			new Rewrite.Branch64Combine32x32(),
			new Rewrite.Branch64Combine32x64(),
			new Rewrite.Branch64Combine64x32(),
			new Rewrite.Branch64Combine64x64(),

			new Simplification.Compare64x32SameHigh(),
			new Simplification.Compare64x32SameLow(),

			// LowerTo32
			new LowerTo32.Add64(),
			new LowerTo32.And64(),
			new LowerTo32.Branch64(),
			new LowerTo32.Compare64x32EqualOrNotEqual(),
			new LowerTo32.Compare64x32Rest(),
			new LowerTo32.Compare64x32RestInSSA(),
			new LowerTo32.Compare64x64EqualOrNotEqual(),
			new LowerTo32.Compare64x64Rest(),
			new LowerTo32.Compare64x64RestInSSA(),

			//LowerTo32.Compare64x32UnsignedGreater(),
			new LowerTo32.Load64(),
			new LowerTo32.LoadParam64(),
			new LowerTo32.LoadParamSignExtend16x64(),
			new LowerTo32.LoadParamSignExtend32x64(),
			new LowerTo32.LoadParamSignExtend8x64(),
			new LowerTo32.LoadParamZeroExtend16x64(),
			new LowerTo32.LoadParamZeroExtend32x64(),
			new LowerTo32.LoadParamZeroExtend8x64(),
			new LowerTo32.Not64(),
			new LowerTo32.Or64(),
			new LowerTo32.SignExtend16x64(),
			new LowerTo32.SignExtend32x64(),
			new LowerTo32.SignExtend8x64(),
			new LowerTo32.Store64(),
			new LowerTo32.StoreParam64(),
			new LowerTo32.Sub64(),
			new LowerTo32.Truncate64x32(),
			new LowerTo32.Xor64(),
			new LowerTo32.ZeroExtend16x64(),
			new LowerTo32.ZeroExtend32x64(),

			new LowerTo32.Move64(),
		};
	}
}