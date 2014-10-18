using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace MotionCaptureAudio
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
                Application.Run(new TopForm());
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message, "SimpleViewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}