/*
 * (c) 2012 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using Mosa.TinyCPUSimulator;
using System;
using System.Collections.Generic;

namespace Mosa.Tool.Simulator
{
	public partial class ControlView : SimulatorDockContent
	{
		private List<string> registerNames;

		public ControlView()
		{
			InitializeComponent();
		}

		public override void Update()
		{
			//Update(SimAdapter.GetState());
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MainForm.SimAdapter == null)
				return;

			MainForm.SimAdapter.Monitor.EnableStepping = true;

			MainForm.ExecuteSteps(1);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (MainForm.SimAdapter == null)
				return;

			MainForm.SimAdapter.Monitor.EnableStepping = true;

			int steps = Convert.ToInt32(tbSteps.Text);
			MainForm.ExecuteSteps(steps);
		}
	}
}