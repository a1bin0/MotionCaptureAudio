using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionCaptureAudio
{
    public partial class PlayingStatusControl : UserControl
    {
        public PlayingStatusControl()
        {
            InitializeComponent();
        }

        internal void DetectedUser()
        {
            //this.Sign.BackColor = Color.Yellow;
            this.Refresh();
        }

        public void CalibrationCompleted()
        {
            //this.Sign.BackColor = Color.Green;
            this.PictPlay.Visible = true;
            this.PictPause.Visible = false;
            this.Refresh();
        }

        internal void LostUser()
        {
            //this.Sign.BackColor = Color.Transparent;
            this.PictPlay.Visible = false;
            this.PictPause.Visible = false;
            this.Refresh();
        }

        public void Play()
        {
            this.PictPlay.Visible = true;
            this.PictPause.Visible = false;
            this.Refresh();
        }

        public void Pause()
        {
            this.PictPlay.Visible = false;
            this.PictPause.Visible = true;
            this.Refresh();
        }
    }
}
