using MotionCaptureAudio.Controller;
using System;
using System.Windows.Forms;

namespace MotionCaptureAudio
{
    public enum Result
    {
        OK,
        NG,
    }

    public partial class UserPlayingStatusForm : UserControl
    {
        public bool CanPlay;
        private AudioPlayer audioPlayer;

        public UserPlayingStatusForm()
        {
            InitializeComponent();
            this.createAudioPlayer();
            this.Show();
        }

        private void createAudioPlayer()
        {
            this.audioPlayer = new AudioPlayer();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Text = this.audioPlayer.CurrentTime.ToString(@"mm\:ss") + "/" + this.audioPlayer.Duration.ToString(@"mm\:ss");
        }

        private void clearTime()
        {
            this.Text = "00:00/" + this.audioPlayer.Duration.ToString(@"mm\:ss");
        }
    }
}
