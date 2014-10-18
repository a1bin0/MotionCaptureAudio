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
        private AudioPlayer audioPlayer;

        public Player()
        {
            InitializeComponent();

            this.createAudioPlayer();
            this.initializeComboBox();
        }

        public void Play(object sender, EventArgs e)
        {
            this.play();
        }

        public void Pause(object sender, EventArgs e)
        {
            this.pause();
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
                    string dname = "";
                    if (bdi.IsDefault) dname = "*";
                    dname += bdi.name;
                    comboBoxDevice.Items.Add(dname);
                    comboBoxDevice.SelectedItem = dname;
                }
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
            this.buttonSound.Enabled = true;
            this.buttonFile.Enabled = true;
            //this.buttonSound.Text = "Pause";
            //this.timer.Start();
            //this.audioPlayer.Play();
        }

        private void buttonSound_Click(object sender, EventArgs e)
        {
            if (buttonSound.Text == "Play")
            {
                this.play();
            }
            else
            {
                this.pause();
            }
        }

        public void play()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.play));
                return;
            }

            this.buttonSound.Text = "Pause";
            this.buttonFile.Text = "Reset";
            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
            this.timer.Start();
            this.audioPlayer.Play();
        }

        public void pause()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.pause));
                return;
            }

            this.buttonSound.Text = "Play";
            this.audioPlayer.Pause();
            this.timer.Stop();
        }

        private void buttonFile_Click(object sender, EventArgs e)
        {
            if (this.buttonFile.Text == "Reset")
            {
                this.buttonSound.Text = "Play";
                this.buttonFile.Text = "Select";
                this.audioPlayer.Stop();
                this.timer.Stop();
                this.clearTime();
            }
            else
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

                this.buttonFile.Text = "Reset";
                this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
                this.buttonSound.Enabled = true;
                this.buttonFile.Enabled = true;
                this.timer.Start();
                this.audioPlayer.Play();
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
    }
}
