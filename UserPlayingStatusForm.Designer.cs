namespace MotionCaptureAudio
{
    partial class UserPlayingStatusForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserPlayingStatusForm));
            this.PictPause = new System.Windows.Forms.PictureBox();
            this.PictPlay = new System.Windows.Forms.PictureBox();
            this.Sign = new System.Windows.Forms.Panel();
            this.User = new System.Windows.Forms.Label();
            this.PlayTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictPause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictPlay)).BeginInit();
            this.SuspendLayout();
            // 
            // PictPause
            // 
            this.PictPause.Image = ((System.Drawing.Image)(resources.GetObject("PictPause.Image")));
            this.PictPause.Location = new System.Drawing.Point(96, 30);
            this.PictPause.Name = "PictPause";
            this.PictPause.Size = new System.Drawing.Size(75, 75);
            this.PictPause.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictPause.TabIndex = 23;
            this.PictPause.TabStop = false;
            // 
            // PictPlay
            // 
            this.PictPlay.BackColor = System.Drawing.SystemColors.Control;
            this.PictPlay.Image = ((System.Drawing.Image)(resources.GetObject("PictPlay.Image")));
            this.PictPlay.Location = new System.Drawing.Point(13, 29);
            this.PictPlay.Name = "PictPlay";
            this.PictPlay.Size = new System.Drawing.Size(75, 75);
            this.PictPlay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictPlay.TabIndex = 21;
            this.PictPlay.TabStop = false;
            // 
            // Sign
            // 
            this.Sign.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Sign.Location = new System.Drawing.Point(52, 11);
            this.Sign.Name = "Sign";
            this.Sign.Size = new System.Drawing.Size(36, 12);
            this.Sign.TabIndex = 1;
            // 
            // User
            // 
            this.User.AutoSize = true;
            this.User.Location = new System.Drawing.Point(11, 11);
            this.User.Name = "User";
            this.User.Size = new System.Drawing.Size(35, 12);
            this.User.TabIndex = 0;
            this.User.Text = "User1";
            // 
            // PlayTime
            // 
            this.PlayTime.AutoSize = true;
            this.PlayTime.Location = new System.Drawing.Point(94, 11);
            this.PlayTime.Name = "PlayTime";
            this.PlayTime.Size = new System.Drawing.Size(71, 12);
            this.PlayTime.TabIndex = 2;
            this.PlayTime.Text = "00:00 / 01:23";
            // 
            // UserPlayingStatusForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.PlayTime);
            this.Controls.Add(this.PictPause);
            this.Controls.Add(this.PictPlay);
            this.Controls.Add(this.Sign);
            this.Controls.Add(this.User);
            this.Name = "UserPlayingStatusForm";
            this.Size = new System.Drawing.Size(190, 108);
            ((System.ComponentModel.ISupportInitialize)(this.PictPause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictPlay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Panel Sign;
        internal System.Windows.Forms.Label User;
        internal System.Windows.Forms.Label PlayTime;
        internal System.Windows.Forms.PictureBox PictPause;
        internal System.Windows.Forms.PictureBox PictPlay;
    }
}
