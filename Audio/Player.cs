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
        private const int playerCount = 3;
        private const float maxVolume = 10f;

        public bool canPlay = false;
        public bool[] canUp = new bool[playerCount];
        public bool[] canDown = new bool[playerCount];

        private List<PlayingStatusControl> playingControls = new List<PlayingStatusControl>();
        private List<Timer> timers = new List<Timer>();
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

        public void backColorChange(int playerId)
        {
            for (int i = 0; i < playerCount; i++)
            {
                this.playingControls[i].BackColor = (i == playerId) ? Color.Gray : Color.Transparent;
                this.playingControls[i].trackBarVolume.BackColor = (i == playerId) ? Color.Gray : Color.Black;
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
                e.trackBarVolume.Value = (int)this.audioPlayers[count].Volume * 10;
                count++;
            });
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

        public void CalibrationCompleted(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.CalibrationCompleted), userId);
                return;
            }

            this.playingControls[userId].CalibrationCompleted();
        }

        public void DetectedUser(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.DetectedUser), userId);
                return;
            }

            this.playingControls[userId].DetectedUser();
        }

        public void LostUser(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.LostUser), userId);
                return;
            }

            this.playingControls[userId].LostUser();
        }

        public void Play(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.Play), userId);
                return;
            }

            this.audioPlayers[userId].Volume = float.Parse(this.playingControls[userId].trackBarVolume.Value.ToString()) / 10;

            this.playingControls[userId].Play();
            this.startTimer(userId);
            this.audioPlayers[userId].Play();
        }

        public void Pause(int userId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.Pause), userId);
                return;
            }

            this.playingControls[userId].Pause();
            this.audioPlayers[userId].Pause();

            this.timers[userId].Stop();
        }

        public void PlayPauseChange(int userId)
        {
            if (this.IsPlaying(userId))
            {
                this.Pause(userId);
            }
            else
            {
                this.Play(userId);
            }
        }

        public void VolumeUp(int playerId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.VolumeUp), playerId);
                return;
            }

            this.audioPlayers[playerId].Volume += maxVolume / 10;
            this.playingControls[playerId].trackBarVolume.Value += 1;
            this.canUp[playerId] = this.audioPlayers[playerId].Volume < maxVolume;
            this.canDown[playerId] = true;
        }

        public void VolumeDown(int playerId)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(this.VolumeDown), playerId);
                return;
            }

            this.audioPlayers[playerId].Volume -= maxVolume / 10;
            this.playingControls[playerId].trackBarVolume.Value -= 1;
            this.canDown[playerId] = this.audioPlayers[playerId].Volume >= maxVolume / 10;
            this.canUp[playerId] = true;
        }

        private void startTimer(int userId)
        {
            this.timers[userId].Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.playingControls[0].PlayTime.Text
                = this.audioPlayers[0].CurrentTime.ToString(@"mm\:ss")
                + "/"
                + this.audioPlayers[0].Duration.ToString(@"mm\:ss");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.playingControls[1].PlayTime.Text
                = this.audioPlayers[1].CurrentTime.ToString(@"mm\:ss")
                + "/"
                + this.audioPlayers[1].Duration.ToString(@"mm\:ss");

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.playingControls[2].PlayTime.Text
                = this.audioPlayers[2].CurrentTime.ToString(@"mm\:ss")
                + "/"
                + this.audioPlayers[2].Duration.ToString(@"mm\:ss");
        }

        private void setMusicFile()
        {
            if (this.initializeInstance() != Result.OK) return;
            this.canPlay = true;
        }

        private void Player_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.setMusicFile();
                this.backColorChange(0);

                this.timers.Add(this.timer1);
                this.timers.Add(this.timer2);
                this.timers.Add(this.timer3);

                for (int i = 0; i < playerCount; i++)
                {
                    this.playingControls[i].trackBarVolume.Value = (int)(this.audioPlayers[i].Volume * 10);
                    this.canUp[i] = this.canPlay && this.audioPlayers[i].Volume < maxVolume;
                    this.canDown[i] = this.canPlay && this.audioPlayers[i].Volume >= maxVolume / 10;
                }
            }
        }

        public bool IsPlaying(int playerId)
        {
            return this.audioPlayers[playerId].PlayState == PlayState.Playing;
        }

        private bool existActivePlayer()
        {
            foreach (var item in this.audioPlayers)
            {
                if (item.PlayState == PlayState.Playing) return true;
            }

            return false;
        }
    }
}
