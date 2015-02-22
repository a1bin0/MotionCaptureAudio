using MotionCaptureAudio.Controller;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Un4seen.Bass;

namespace MotionCaptureAudio
{
    public partial class Player : UserControl
    {
        private AudioPlayer audioPlayer = new AudioPlayer();
        
        internal Player()
        {
            InitializeComponent();
        }

        internal void Initialize()
        {
            this.PlayTime.Text = "00:00 / 00:00";
            this.PictPlay.Visible = false;
            this.PictPause.Visible = true;
            this.trackBarVolume.Value = (int)this.audioPlayer.Volume * 10;
        }

        private Result initializeInstance(int indexOfDevice, string fileName)
        {
            try
            {
                var result = this.audioPlayer.InitializeInstance(indexOfDevice, fileName);
                if (result == Result.OK)
                {
                    this.trackBarVolume.Value = (int)(this.audioPlayer.Volume * 10);
                }

                return result;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found. " + fileName);
                return Result.NG;
            }
        }

        internal void BackColorChange(bool isActive)
        {
            this.BackColor = isActive ? Color.Gray : Color.Transparent;
            this.trackBarVolume.BackColor = isActive ? Color.Gray : Color.Black;
        }

        internal BASS_DEVICEINFO[] GetDevice()
        {
            return this.audioPlayer.GetDevice();
        }

        internal void SetMusicFile(int indexOfdevice)
        {
            string fName = @"..\..\music.mp3";
            this.initializeInstance(indexOfdevice, fName);
        }

        internal void DetectedUser()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.DetectedUser));
                return;
            }

            this.Refresh();
        }

        internal void CalibrationCompleted()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.CalibrationCompleted));
                return;
            }

            this.PictPlay.Visible = true;
            this.PictPause.Visible = false;
            this.Refresh();
        }

        internal void LostUser()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.LostUser));
                return;
            }

            this.Pause();
            this.PictPlay.Visible = false;
            this.PictPause.Visible = false;
            this.Refresh();
        }

        internal void Play()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.Play));
                return;
            }

            this.PictPlay.Visible = false;
            this.PictPause.Visible = true;
            this.Refresh();

            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
            this.audioPlayer.Play();

            this.timer.Start();
        }

        internal void Pause()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.Pause));
                return;
            }

            this.PictPlay.Visible = true;
            this.PictPause.Visible = false;
            this.Refresh();

            this.audioPlayer.Pause();

            this.timer.Stop();
        }

        internal void PlayPauseChange()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.PlayPauseChange));
                return;
            }

            if (this.audioPlayer.PlayState == PlayState.Playing)
            {
                this.Pause();
            }
            else
            {
                this.Play();
            }
        }

        internal void VolumeUp()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.VolumeUp));
                return;
            }

            if (Math.Floor(this.audioPlayer.Volume * 10) > 9) return;
            
            this.audioPlayer.Volume += 0.1f;
            this.trackBarVolume.Value += 1;
        }

        internal void VolumeDown()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.VolumeDown));
                return;
            }

            if (Math.Floor(this.audioPlayer.Volume * 10) < 1) return;

            this.audioPlayer.Volume -= 0.1f;
            this.trackBarVolume.Value -= 1;
        }

        internal PlayState GetPlayState()
        {
            return this.audioPlayer.PlayState;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.PlayTime.Text
                = this.audioPlayer.CurrentTime.ToString(@"mm\:ss")
                + "/"
                + this.audioPlayer.Duration.ToString(@"mm\:ss");
        }
    }
}
