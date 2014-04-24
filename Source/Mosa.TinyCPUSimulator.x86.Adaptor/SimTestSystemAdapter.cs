﻿/*
 * (c) 2014 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using Mosa.TinyCPUSimulator.Adaptor;
using Mosa.TinyCPUSimulator.x86.Emulate;

namespace Mosa.TinyCPUSimulator.x86.Adaptor
{
	public class SimTestSystemAdapter : SimAdapter
	{
		public SimTestSystemAdapter()
		{
			CPU.AddDevice(new PowerUp(CPU));
			CPU.AddDevice(new CMOS(CPU));
			CPU.AddDevice(new Multiboot(CPU));

			CPU.AddDevice(new MosaKernel(CPU));
		}
	}
}