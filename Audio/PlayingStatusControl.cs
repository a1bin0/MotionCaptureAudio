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
            this.Refresh();
        }

        public void CalibrationCompleted()
        {
            this.PictPlay.Visible = true;
            this.PictPause.Visible = false;
            this.Refresh();
        }

        internal void LostUser()
        {
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
