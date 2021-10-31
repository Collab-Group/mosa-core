﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Add32
	/// </summary>
	/// <seealso cref="Mosa.Platform.x86.X86Instruction" />
	public sealed class ReferenceCountAdd : X86Instruction
	{
		internal ReferenceCountAdd()
			: base(1, 1)
		{
		}

		public override bool IsCommutative { get { return true; } }

		public override bool ThreeTwoAddressConversion { get { return true; } }

		public override bool IsZeroFlagModified { get { return true; } }

		public override bool IsCarryFlagModified { get { return true; } }

		public override bool IsSignFlagModified { get { return true; } }

		public override bool IsOverflowFlagModified { get { return true; } }

		public override bool IsParityFlagModified { get { return true; } }

		public override void Emit(InstructionNode node, OpcodeEncoder opcodeEncoder)
		{
			//MOV [EAX+4],1

			opcodeEncoder.Append8Bits(0x83);
			opcodeEncoder.Append8Bits(0x40);
			opcodeEncoder.Append8Bits(0x04);
			opcodeEncoder.Append8Bits(0x01);
		}
	}
}
