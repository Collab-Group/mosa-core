// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime;

namespace Mosa.Kernel.x86
{
	public enum ThreadStatus { Empty = 0, Running, Terminating, Terminated, Waiting };

	public class Thread
	{
		public ThreadStatus Status = ThreadStatus.Empty;
		public Pointer StackTop;
		public Pointer StackBottom;
		public Pointer StackStatePointer;
		public uint Ticks;

		/// <summary>
		/// PIT.Wait(uint millisecond)
		/// </summary>
		/// <param name="millisecond"></param>
		//Static Method Won't Appears In The Class Instance It Won't Be A Part Of Class Instance
		public static void Sleep(uint millisecond)
		{
			PIT.Wait(millisecond);
		}
	}
}
