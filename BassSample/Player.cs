using MotionCaptureAudio.Controller;
using System;
using System.IO;
using System.Windows.Forms;

namespace MotionCaptureAudio
{
    public enum Result
    {
        OK,
        NG,
    }

    public partial class Player : Form
    {
        public bool CanPlay;
        private AudioPlayer audioPlayer;

        public Player()
        {
            InitializeComponent();
            this.createAudioPlayer();
            this.initializeComboBox();
            this.Show();
        }

        private void createAudioPlayer()
        {
            this.audioPlayer = new AudioPlayer();
        }

        private void initializeComboBox()
        {
            try
            {
                //foreach (var bdi in this.audioPlayer.GetDevice())
                //{
                //    string dname = "";
                //    if (bdi.IsDefault) dname = "*";
                //    dname += bdi.name;
                //    comboBoxDevice.Items.Add(dname);
                //    comboBoxDevice.SelectedItem = dname;
                //}
            }
            catch (Exception e)
            {
                return;
            }
        }

        private Result initializeInstance(string fileName)
        {
            try
            {
                var result = this.audioPlayer.InitializeInstance(this.comboBoxDevice.SelectedIndex, fileName);
                return result;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found.");
                return Result.NG;
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            if (this.comboBoxDevice.SelectedIndex < 0)
            {
                MessageBox.Show("Please select device");
                return;
            }

            this.openFileDialog.InitialDirectory = @"";
            this.openFileDialog.Filter = "MP3File(*.mp3)|*.mp3";
            var dialogResult = this.openFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK) return;
            if (this.initializeInstance(this.openFileDialog.FileName) != Result.OK) return;

            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
            this.buttonPlayPause.Enabled = true;
            this.buttonSelect.Enabled = true;
            this.CanPlay = true;
        }

        public void Play()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.Play));
                return;
            }

            this.buttonPlayPause.Text = "Pause";
            this.buttonSelect.Text = "Reset";
            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
            this.timer.Start();
            this.audioPlayer.Play();
        }

        public void Pause()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.Pause));
                return;
            }

            this.buttonPlayPause.Text = "Play";
            this.audioPlayer.Pause();
            this.timer.Stop();
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

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Text = this.audioPlayer.CurrentTime.ToString(@"mm\:ss") + "/" + this.audioPlayer.Duration.ToString(@"mm\:ss");
        }

        private void clearTime()
        {
            this.Text = "00:00/" + this.audioPlayer.Duration.ToString(@"mm\:ss");
        }

        private void buttonPlayPause_Click(object sender, EventArgs e)
        {
            if (buttonPlayPause.Text == "Play")
            {
                this.Play();
            }
            else
            {
                this.Pause();
            }
        }
    }
}
