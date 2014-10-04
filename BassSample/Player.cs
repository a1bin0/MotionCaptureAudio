using BassSample.Controller;
using System;
using System.IO;
using System.Windows.Forms;

namespace BassSample
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

        private void createAudioPlayer()
        {
            this.audioPlayer = new AudioPlayer();
            if (this.audioPlayer == null)
            {
                Console.WriteLine("nullだよ");
            }
            else
            {
                Console.WriteLine("playerある");
            }
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
            catch(Exception e)
            {
                MessageBox.Show("Exception occurred!" + Environment.NewLine + e.ToString());
                return;
            }
        }

        private Result initializeInstance()
        {
            try
            {
                var result =this.audioPlayer.InitializeInstance(this.comboBoxDevice.SelectedIndex, this.labelName.Text);
                return result;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found.");
                this.labelName.Text = string.Empty;
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

            this.labelName.Text = this.openFileDialog.FileName;

            if (this.initializeInstance() != Result.OK) return;

            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
            this.timer.Start();
            this.audioPlayer.Play();
            this.updateButtonStatus();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
            this.timer.Start();
            this.audioPlayer.Play();
            this.updateButtonStatus();
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            this.audioPlayer.Pause();
            this.timer.Stop();
            this.updateButtonStatus();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.audioPlayer.Stop();
            this.timer.Stop();
            this.clearTime();
            this.updateButtonStatus();
        }

        private void updateButtonStatus()
        {
            this.buttonPlay.Enabled = this.labelName.Text != string.Empty && this.audioPlayer.PlayState != PlayState.Playing;
            this.buttonPause.Enabled = this.audioPlayer.PlayState == PlayState.Playing;
            this.buttonStop.Enabled = this.labelName.Text != string.Empty && this.audioPlayer.PlayState != PlayState.Stopped;
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            this.audioPlayer.Volume = float.Parse(this.trackBarVolume.Value.ToString()) / 10;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.labelCurrentTimeValue.Text = this.audioPlayer.CurrentTime.ToString(@"mm\:ss") + " / " + this.audioPlayer.Duration.ToString(@"mm\:ss");
        }

        private void clearTime()
        {
            this.labelCurrentTimeValue.Text = string.Empty;
        }
    }
}
