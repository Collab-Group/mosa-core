### Use System.Threading.Thread

### Example

```cs
// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.x86;
using Mosa.Runtime.Plug;
using System.Threading;

namespace MOSA28
{
    public static unsafe class Program
    {
        public static void Main() { }

        [Plug("Mosa.Runtime.StartUp::KMain")]
        public static void KMain()
        {
            new Thread(() =>
            {
                for(; ; ) 
                {
                    Console.WriteLine("Hello From Thread 1");
                }
            }).Start();
            new Thread(() =>
            {
                for(; ; )
                {
                    Console.WriteLine("Hello From Thread 2");
                }
            }).Start();
            for (; ; );
        }
    }
}

```