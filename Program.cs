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
                Application.Run(new MainWindow());
			}
			catch (System.Exception ex)
			{
                MessageBox.Show("Error: " + ex.Message, "MotionCaptureAudio", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}