// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.CompilerServices;

namespace System.Threading
{
	public delegate void ThreadStart();
	public delegate void ParameterizedThreadStart(object obj);

    public class Thread
    {
        private ThreadStart ts;

        public Thread(ThreadStart ts)
        {
            this.ts = ts;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern uint CreateThread(ThreadStart thread, uint stackSize);

        public void Start(uint stackSize = 4096)
        {
            CreateThread(ts, stackSize);
        }
    }
}
