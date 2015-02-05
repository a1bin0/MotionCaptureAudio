namespace MotionCaptureAudio
{
    partial class Player
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Player));
            this.buttonSelect = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictVolumeUp = new System.Windows.Forms.PictureBox();
            this.pictVolumeDown = new System.Windows.Forms.PictureBox();
            this.playingStatusControl2 = new MotionCaptureAudio.PlayingStatusControl();
            this.playingStatusControl1 = new MotionCaptureAudio.PlayingStatusControl();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.playingStatusControl3 = new MotionCaptureAudio.PlayingStatusControl();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictVolumeUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictVolumeDown)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSelect
            // 
            this.buttonSelect.BackColor = System.Drawing.Color.Black;
            this.buttonSelect.Font = new System.Drawing.Font("MS UI Gothic", 18F);
            this.buttonSelect.Location = new System.Drawing.Point(13, 356);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(160, 33);
            this.buttonSelect.TabIndex = 13;
            this.buttonSelect.Text = "Select";
            this.buttonSelect.UseVisualStyleBackColor = false;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // comboBoxDevice
            // 
            this.comboBoxDevice.BackColor = System.Drawing.Color.Black;
            this.comboBoxDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxDevice.ForeColor = System.Drawing.Color.White;
            this.comboBoxDevice.FormattingEnabled = true;
            this.comboBoxDevice.Location = new System.Drawing.Point(13, 330);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(160, 20);
            this.comboBoxDevice.TabIndex = 1;
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.BackColor = System.Drawing.Color.Black;
            this.trackBarVolume.Location = new System.Drawing.Point(70, 466);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarVolume.Size = new System.Drawing.Size(45, 168);
            this.trackBarVolume.TabIndex = 15;
            this.trackBarVolume.Value = 3;
            this.trackBarVolume.Scroll += new System.EventHandler(this.trackBarVolume_Scroll);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // pictVolumeUp
            // 
            this.pictVolumeUp.Image = ((System.Drawing.Image)(resources.GetObject("pictVolumeUp.Image")));
            this.pictVolumeUp.Location = new System.Drawing.Point(70, 415);
            this.pictVolumeUp.Name = "pictVolumeUp";
            this.pictVolumeUp.Size = new System.Drawing.Size(45, 45);
            this.pictVolumeUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictVolumeUp.TabIndex = 3;
            this.pictVolumeUp.TabStop = false;
            // 
            // pictVolumeDown
            // 
            this.pictVolumeDown.Image = ((System.Drawing.Image)(resources.GetObject("pictVolumeDown.Image")));
            this.pictVolumeDown.Location = new System.Drawing.Point(70, 640);
            this.pictVolumeDown.Name = "pictVolumeDown";
            this.pictVolumeDown.Size = new System.Drawing.Size(45, 45);
            this.pictVolumeDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictVolumeDown.TabIndex = 18;
            this.pictVolumeDown.TabStop = false;
            // 
            // playingStatusControl2
            // 
            this.playingStatusControl2.BackColor = System.Drawing.Color.Transparent;
            this.playingStatusControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.playingStatusControl2.Location = new System.Drawing.Point(0, 108);
            this.playingStatusControl2.Name = "playingStatusControl2";
            this.playingStatusControl2.Size = new System.Drawing.Size(190, 108);
            this.playingStatusControl2.TabIndex = 20;
            // 
            // playingStatusControl1
            // 
            this.playingStatusControl1.BackColor = System.Drawing.Color.Transparent;
            this.playingStatusControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.playingStatusControl1.Location = new System.Drawing.Point(0, 0);
            this.playingStatusControl1.Name = "playingStatusControl1";
            this.playingStatusControl1.Size = new System.Drawing.Size(190, 108);
            this.playingStatusControl1.TabIndex = 19;
            // 
            // timer2
            // 
            this.timer2.Interval = 500;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Interval = 500;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // playingStatusControl3
            // 
            this.playingStatusControl3.BackColor = System.Drawing.Color.Transparent;
            this.playingStatusControl3.Dock = System.Windows.Forms.DockStyle.Top;
            this.playingStatusControl3.Location = new System.Drawing.Point(0, 216);
            this.playingStatusControl3.Name = "playingStatusControl3";
            this.playingStatusControl3.Size = new System.Drawing.Size(190, 108);
            this.playingStatusControl3.TabIndex = 21;
            // 
            // Player
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.playingStatusControl3);
            this.Controls.Add(this.trackBarVolume);
            this.Controls.Add(this.playingStatusControl2);
            this.Controls.Add(this.comboBoxDevice);
            this.Controls.Add(this.playingStatusControl1);
            this.Controls.Add(this.buttonSelect);
            this.Controls.Add(this.pictVolumeDown);
            this.Controls.Add(this.pictVolumeUp);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Player";
            this.Size = new System.Drawing.Size(190, 709);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictVolumeUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictVolumeDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ComboBox comboBoxDevice;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pictVolumeUp;
        private System.Windows.Forms.PictureBox pictVolumeDown;
        private PlayingStatusControl playingStatusControl1;
        private PlayingStatusControl playingStatusControl2;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private PlayingStatusControl playingStatusControl3;
    }
}

