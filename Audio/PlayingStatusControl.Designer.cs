namespace MotionCaptureAudio
{
    partial class PlayingStatusControl
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayingStatusControl));
            this.PictPause = new System.Windows.Forms.PictureBox();
            this.PictPlay = new System.Windows.Forms.PictureBox();
            this.PlayTime = new System.Windows.Forms.Label();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.PictPause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictPlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // PictPause
            // 
            this.PictPause.BackColor = System.Drawing.Color.Transparent;
            this.PictPause.Image = ((System.Drawing.Image)(resources.GetObject("PictPause.Image")));
            this.PictPause.Location = new System.Drawing.Point(13, 126);
            this.PictPause.Name = "PictPause";
            this.PictPause.Size = new System.Drawing.Size(90, 80);
            this.PictPause.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictPause.TabIndex = 23;
            this.PictPause.TabStop = false;
            this.PictPause.Visible = false;
            // 
            // PictPlay
            // 
            this.PictPlay.BackColor = System.Drawing.Color.Transparent;
            this.PictPlay.Image = ((System.Drawing.Image)(resources.GetObject("PictPlay.Image")));
            this.PictPlay.Location = new System.Drawing.Point(13, 40);
            this.PictPlay.Name = "PictPlay";
            this.PictPlay.Size = new System.Drawing.Size(90, 80);
            this.PictPlay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictPlay.TabIndex = 21;
            this.PictPlay.TabStop = false;
            this.PictPlay.Visible = false;
            // 
            // PlayTime
            // 
            this.PlayTime.AutoSize = true;
            this.PlayTime.BackColor = System.Drawing.Color.Transparent;
            this.PlayTime.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.PlayTime.Location = new System.Drawing.Point(10, 15);
            this.PlayTime.Name = "PlayTime";
            this.PlayTime.Size = new System.Drawing.Size(84, 13);
            this.PlayTime.TabIndex = 2;
            this.PlayTime.Text = "00:00 / 01:23";
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.BackColor = System.Drawing.Color.Black;
            this.trackBarVolume.Location = new System.Drawing.Point(109, 0);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarVolume.Size = new System.Drawing.Size(45, 217);
            this.trackBarVolume.TabIndex = 24;
            this.trackBarVolume.Value = 6;
            // 
            // PlayingStatusControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.trackBarVolume);
            this.Controls.Add(this.PlayTime);
            this.Controls.Add(this.PictPause);
            this.Controls.Add(this.PictPlay);
            this.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.Name = "PlayingStatusControl";
            this.Size = new System.Drawing.Size(160, 220);
            ((System.ComponentModel.ISupportInitialize)(this.PictPause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictPlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label PlayTime;
        internal System.Windows.Forms.PictureBox PictPause;
        internal System.Windows.Forms.PictureBox PictPlay;
        public System.Windows.Forms.TrackBar trackBarVolume;
    }
}
