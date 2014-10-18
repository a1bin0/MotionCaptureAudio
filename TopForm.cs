using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionCaptureAudio
{
    public partial class TopForm : Form
    {
        MotionCaptureAudio.Player player;
        MainWindow mainWindow;

        public TopForm()
        {
            InitializeComponent();
            this.player = new MotionCaptureAudio.Player();
            this.player.Show();
        }

        private void initializeEvent()
        {
            this.mainWindow.PlayRequest += new EventHandler(this.player.Play);
            this.mainWindow.PauseRequest += new EventHandler(this.player.Pause);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.mainWindow = new MainWindow();
            this.initializeEvent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.mainWindow.StartThread();
            this.mainWindow.Show();
        }
    }
}
