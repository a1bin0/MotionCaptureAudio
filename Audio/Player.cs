using MotionCaptureAudio.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace MotionCaptureAudio
{
    public enum Result
    {
        OK,
        NG,
    }

    public partial class Player : UserControl
    {
        private const int playerCount = 3;
        private TimeSpan currentTime;

        public bool canPlay = false;

        private List<PlayingStatusControl> playingControls = new List<PlayingStatusControl>();
        private List<AudioPlayer> audioPlayers;

        public Player()
        {
            InitializeComponent();
            if (!this.DesignMode)
            {
                this.createAudioPlayer();
                this.createPlayingStatusControls();
                this.initializeComboBox();
            }
        }

        public bool CanUp(int playerId)
        {
            return this.playingControls[playerId].CanUp;
        }

        public bool CanDown(int playerId)
        {
            return this.playingControls[playerId].CanDown;
        }

        public void backColorChange(int playerId)
        {
            for (int i = 0; i < playerCount; i++)
            {
                var backColor = (i == playerId) ? SystemColors.Highlight : Color.Black;
                this.playingControls[i].BackColor = backColor;
                this.playingControls[i].trackBarVolume.BackColor = backColor;
            }
        }

        private void createPlayingStatusControls()
        {
            this.playingControls.Add(this.playingStatusControl1);
            this.playingControls.Add(this.playingStatusControl2);
            this.playingControls.Add(this.playingStatusControl3);

            var count = 0;
            this.playingControls.ForEach(e =>
            {
                e.PlayTime.Text = "00:00 / 00:00";
                e.PictPlay.Visible = false;
                e.PictPause.Visible = true;
                e.trackBarVolume.Value = 5;
                count++;
            });

            this.currentTime = new TimeSpan(0, 0, 0);
        }

        private void createAudioPlayer()
        {
            this.audioPlayers = new List<AudioPlayer>() { new AudioPlayer(), new AudioPlayer(), new AudioPlayer() };
        }

        private void initializeComboBox()
        {
            try
            {
                foreach (var device in this.audioPlayers[0].GetDevice())
                {
                    var dname = device.IsDefault ? "*" : string.Empty;
                    dname += device.name;
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

        private Result initializeInstance()
        {
            try
            {
                string path = Application.ExecutablePath;
                string fName1 = path.Remove(path.LastIndexOf("\\")) + "\\a.mp3";
                string fName2 = path.Remove(path.LastIndexOf("\\")) + "\\b.mp3";
                string fName3 = path.Remove(path.LastIndexOf("\\")) + "\\c.mp3";
                string[] fName = { fName1, fName2, fName3 };

                int count = 0;

                foreach (var item in this.audioPlayers)
                {
                    var result = item.InitializeInstance(this.comboBoxDevice.SelectedIndex, fName[count]);
                    if (result == Result.NG) return result;

                    count++;
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found.");
                return Result.NG;
            }

            return Result.OK;
        }

        public void CalibrationCompleted()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.CalibrationCompleted));
                return;
            }

            foreach(var item in this.playingControls)
            {
                item.CalibrationCompleted();
            }
        }

        public void DetectedUser()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.DetectedUser));
                return;
            }

            foreach (var item in this.playingControls)
            {
                item.DetectedUser();
            }
        }

        public void LostUser()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.LostUser));
                return;
            }

            foreach (var item in this.playingControls)
            {
                item.LostUser();
            }
        }

        public void Play(int playerId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.Play), playerId);
                return;
            }

            if (!this.existActivePlayer())
            {
                this.timer1.Start();
            }

            this.setCurrentTime();

            this.playingControls[playerId].Play();
            this.audioPlayers[playerId].Play();
        }

        public void Pause(int playerId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.Pause), playerId);
                return;
            }

            this.playingControls[playerId].Pause();
            this.audioPlayers[playerId].Pause();

            if (!this.existActivePlayer())
            {
                this.timer1.Stop();
            }
        }

        public void PlayPauseChange(int playerId)
        {
            if (this.IsPlaying(playerId))
            {
                this.Pause(playerId);
            }
            else
            {
                this.Play(playerId);
            }
        }

        public void VolumeUp(int playerId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.VolumeUp), playerId);
                return;
            }

            this.audioPlayers[playerId].Volume += 1.0f;
            this.playingControls[playerId].trackBarVolume.Value++;
        }

        public void VolumeDown(int playerId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.VolumeDown), playerId);
                return;
            }

            this.audioPlayers[playerId].Volume -= 1.0f;
            this.playingControls[playerId].trackBarVolume.Value--;
        }

        public void SetMaxVolume(int playerId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.SetMaxVolume), playerId);
                return;
            }

            while (this.CanUp(playerId))
            {
                this.VolumeUp(playerId);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (this.audioPlayers[i].PlayState == PlayState.Playing)
                {
                    string timeText
                        = this.audioPlayers[i].CurrentTime.ToString(@"mm\:ss")
                        + "/"
                        + this.audioPlayers[0].Duration.ToString(@"mm\:ss");

                    this.playingControls[i].PlayTime.Text = timeText;
                }
            }
        }

        private void setMusicFile()
        {
            this.canPlay = (this.initializeInstance() == Result.OK);
        }

        private void Player_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.setMusicFile();
                this.backColorChange(0);

                for (int i = 0; i < playerCount; i++)
                {
                    this.audioPlayers[i].Volume = 5.0f;
                    this.playingControls[i].trackBarVolume.Value = 5;
                }
            }
        }

        public bool IsPlaying(int playerId)
        {
            return this.audioPlayers[playerId].PlayState == PlayState.Playing;
        }

        private bool existActivePlayer()
        {
            return this.audioPlayers.Any(item => item.PlayState == PlayState.Playing);
        }

        private void setCurrentTime()
        {
            this.currentTime = this.audioPlayers.Max(item => item.CurrentTime);
            this.audioPlayers.ForEach(item => item.CurrentTime = this.currentTime);
        }
    }
}
