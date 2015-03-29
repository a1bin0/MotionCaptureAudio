using System.Windows.Forms;

namespace MotionCaptureAudio
{
    public partial class PlayingStatusControl : UserControl
    {
        public PlayingStatusControl()
        {
            InitializeComponent();
        }

        internal bool CanUp
        {
            get
            {
                return (this.trackBarVolume.Value < this.trackBarVolume.Maximum);
            }
        }

        internal bool CanDown
        {
            get
            {
                return (this.trackBarVolume.Value > this.trackBarVolume.Minimum);
            }
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
            this.Pause();
            this.PictPlay.Visible = false;
            this.PictPause.Visible = false;
            this.Refresh();
        }

        public void Play()
        {
            this.PictPlay.Visible = false;
            this.PictPause.Visible = true;
            this.Refresh();
        }

        public void Pause()
        {
            this.PictPlay.Visible = true;
            this.PictPause.Visible = false;
            this.Refresh();
        }
    }
}
