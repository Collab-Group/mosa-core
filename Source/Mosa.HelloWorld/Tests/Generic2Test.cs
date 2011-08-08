﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;

using Mosa.Platform.x86;
using Mosa.Kernel;
using Mosa.Kernel.x86;
using Mosa.ClassLib;

namespace Mosa.HelloWorld.Tests
{

	public class Generic2Test : KernelTest
	{
		public static void Test()
		{
			Screen.Color = Colors.Gray;
			Screen.Write(" Gen-N: ");

			PrintResult(GenericTest1());
			PrintResult(GenericTest2());
			PrintResult(GenericTest3());
			PrintResult(GenericTest4());
		}

		public static bool GenericTest1()
		{
			var node = new LinkedList<uint>.LinkedListNode<uint>(10, null, null);
			return node.value == 10;
		}

		public static bool GenericTest2()
		{
			var node1 = new LinkedList<uint>.LinkedListNode<uint>(10, null, null);
			var node2 = new LinkedList<uint>.LinkedListNode<uint>(20, node1, null);
			var node3 = new LinkedList<uint>.LinkedListNode<uint>(30, node2, null);
			node1.next = node2;
			node2.next = node3;
			return node1.next.next.value == 30;
		}

		public static bool GenericTest3()
		{
			var list = new LinkedList<int>();
			list.Add(10);
			list.Add(20);
			return list.First.value == 10;
		}

		public static bool GenericTest4()
		{
			var list = new LinkedList<int>();
			list.Add(10);
			list.Add(20);
			list.Add(30);
			list.Find(30);
			return true;
			//return list.First.value == 10 && list.Last.value == 20 && list.Find(30).value == 30;
		}
	}

}
