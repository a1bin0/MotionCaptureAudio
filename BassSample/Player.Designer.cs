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
            this.buttonSound = new System.Windows.Forms.Button();
            this.buttonFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSound
            // 
            this.buttonSound.BackColor = System.Drawing.Color.Black;
            this.buttonSound.Enabled = false;
            this.buttonSound.Location = new System.Drawing.Point(6, 32);
            this.buttonSound.Name = "buttonSound";
            this.buttonSound.Size = new System.Drawing.Size(48, 23);
            this.buttonSound.TabIndex = 11;
            this.buttonSound.Text = "Pause";
            this.buttonSound.UseVisualStyleBackColor = false;
            this.buttonSound.Click += new System.EventHandler(this.buttonSound_Click);
            // 
            // buttonFile
            // 
            this.buttonFile.BackColor = System.Drawing.Color.Black;
            this.buttonFile.Location = new System.Drawing.Point(60, 32);
            this.buttonFile.Name = "buttonFile";
            this.buttonFile.Size = new System.Drawing.Size(48, 23);
            this.buttonFile.TabIndex = 13;
            this.buttonFile.Text = "Select";
            this.buttonFile.UseVisualStyleBackColor = false;
            this.buttonFile.Click += new System.EventHandler(this.buttonFile_Click);
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
            this.comboBoxDevice.Location = new System.Drawing.Point(6, 6);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(208, 20);
            this.comboBoxDevice.TabIndex = 1;
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.BackColor = System.Drawing.Color.Black;
            this.trackBarVolume.Location = new System.Drawing.Point(169, 32);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarVolume.Size = new System.Drawing.Size(45, 254);
            this.trackBarVolume.TabIndex = 15;
            this.trackBarVolume.Value = 3;
            this.trackBarVolume.Scroll += new System.EventHandler(this.trackBarVolume_Scroll);
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(638, 326);
            this.ControlBox = false;
            this.Controls.Add(this.trackBarVolume);
            this.Controls.Add(this.comboBoxDevice);
            this.Controls.Add(this.buttonFile);
            this.Controls.Add(this.buttonSound);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Player";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "00:00/00:00";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSound;
        private System.Windows.Forms.Button buttonFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ComboBox comboBoxDevice;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Timer timer;
    }
}

