using MotionCaptureAudio.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace MotionCaptureAudio
{
    public enum Result
    {
        OK,
        NG,
    }

    public partial class Player : UserControl
    {
        // mm add start
        private bool canplay;
        // mm add end
        public bool CanPlay
        {
            get
            {
                return this.canplay;
            }
            set
            {
                this.canplay = value;
            }
        }

        private List<PlayingStatusControl> playingStatusControls = new List<PlayingStatusControl>();
        private List<Timer> timers = new List<Timer>();
        private AudioPlayer audioPlayer;

        public Player()
        {
            InitializeComponent();
            this.createplayingStatusControls();
            this.createAudioPlayer();
            this.initializeComboBox();
            this.initializeTrackBar();

            this.timers.Add(this.timer1);
            this.timers.Add(this.timer2);
            this.timers.Add(this.timer3);
        }

        private void createplayingStatusControls()
        {
            this.playingStatusControls.Add(this.playingStatusControl1);
            this.playingStatusControls.Add(this.playingStatusControl2);
            this.playingStatusControls.Add(this.playingStatusControl3);

            var count = 1;
            this.playingStatusControls.ForEach(e =>
            {
                e.User.Text = "user" + count.ToString();
                count++;
                e.PlayTime.Text = "00:00 / 00:00";
                e.PictPlay.Visible = false;
                e.PictPause.Visible = false;
            });
        }

        private void createAudioPlayer()
        {
            this.audioPlayer = new AudioPlayer();
        }

        private void initializeComboBox()
        {
            try
            {
                foreach (var bdi in this.audioPlayer.GetDevice())
                {
                    var dname = bdi.IsDefault ? "*" : string.Empty;
                    dname += bdi.name;
                    this.comboBoxDevice.Items.Add(dname);
                    this.comboBoxDevice.SelectedItem = dname;
                }

                if (this.comboBoxDevice.Items.Count >= 1)
                {
                    this.comboBoxDevice.SelectedIndex = 1;
                }
            }
            catch
            {
                return;
            }
        }

        private Result initializeInstance(string fileName)
        {
            try
            {
                return
                    this.audioPlayer.InitializeInstance(
                        this.comboBoxDevice.SelectedIndex,
                        fileName);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found.");
                return Result.NG;
            }
        }

        private void initializeTrackBar()
        {
            this.trackBarVolume.Value = int.Parse((this.audioPlayer.Volume * 10).ToString());
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            if (this.comboBoxDevice.SelectedIndex < 0)
            {
                MessageBox.Show("Please select device");
                return;
            }

            this.openFileDialog.InitialDirectory = string.Empty;
            this.openFileDialog.Filter = "MP3File(*.mp3)|*.mp3";
            var dialogResult = this.openFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK) return;
            if (this.initializeInstance(this.openFileDialog.FileName) != Result.OK) return;

            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
            this.buttonSelect.Enabled = true;
            this.CanPlay = true;
            Console.WriteLine("aaa" + this.CanPlay.GetHashCode().ToString());
        }

        public void CalibrationCompleted(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.CalibrationCompleted), userId);
                return;
            }

            this.playingStatusControls[userId - 1].CalibrationCompleted();
        }

        public void DetectedUser(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.DetectedUser), userId);
                return;
            }

            this.playingStatusControls[userId - 1].DetectedUser();
        }

        public void LostUser(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.LostUser), userId);
                return;
            }

            this.playingStatusControls[userId - 1].LostUser();
        }

        public void Play(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.Play), userId);
                return;
            }

            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
            this.playingStatusControls[userId - 1].Play();
            this.startTimer(userId);
            this.audioPlayer.Play();
        }

        public void Pause(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.Pause), userId);
                return;
            }

            this.playingStatusControls[userId - 1].Pause();
            this.audioPlayer.Pause();

            this.timers[userId - 1].Stop();
        }

        public void PlayPauseChange(int userId)
        {
            if (this.audioPlayer.PlayState == PlayState.Playing)
            {
                this.Pause(userId);
            }
            else
            {
                this.Play(userId);
            }
        }

        public void VolumeUp()
        {
            if (this.trackBarVolume.Value >= 10) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.VolumeUp));
                return;
            }

            if (this.audioPlayer.Volume < 1.0)
            {
                this.audioPlayer.Volume += 0.1f;
                this.trackBarVolume.Value += 1;
            }
        }

        public void VolumeDown()
        {
            if ((this.audioPlayer.Volume == 0 ) || (this.trackBarVolume.Value == 0)) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.VolumeDown));
                return;
            }

            if (this.audioPlayer.Volume > 0.0)
            {
                this.audioPlayer.Volume -= 0.1f;
                this.trackBarVolume.Value -= 1;
            }
        }

        public void UserChange(int userId, Color color)
        {
            this.playingStatusControls[userId - 1].Sign.BackColor = color;
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
        }

        private void startTimer(int userId)
        {
            this.timers[userId - 1].Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.playingStatusControls[0].PlayTime.Text
                = this.audioPlayer.CurrentTime.ToString(@"mm\:ss")
                + "/"
                + this.audioPlayer.Duration.ToString(@"mm\:ss");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.playingStatusControls[1].PlayTime.Text
                = this.audioPlayer.CurrentTime.ToString(@"mm\:ss")
                + "/"
                + this.audioPlayer.Duration.ToString(@"mm\:ss");

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.playingStatusControls[2].PlayTime.Text
                = this.audioPlayer.CurrentTime.ToString(@"mm\:ss")
                + "/"
                + this.audioPlayer.Duration.ToString(@"mm\:ss");
        }

        private void clearTime()
        {
            this.playingStatusControls.ForEach(e =>
            {
                e.PlayTime.Text = "00:00 / 00:00";
            });
        }
    }
}
