namespace MotionCaptureAudio
{
    partial class IntegratedPlayer
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
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.playingStatusControl3 = new MotionCaptureAudio.Player();
            this.playingStatusControl2 = new MotionCaptureAudio.Player();
            this.playingStatusControl1 = new MotionCaptureAudio.Player();
            this.SuspendLayout();
            // 
            // comboBoxDevice
            // 
            this.comboBoxDevice.FormattingEnabled = true;
            this.comboBoxDevice.Location = new System.Drawing.Point(3, 666);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(156, 20);
            this.comboBoxDevice.TabIndex = 22;
            // 
            // playingStatusControl3
            // 
            this.playingStatusControl3.BackColor = System.Drawing.Color.Transparent;
            this.playingStatusControl3.Dock = System.Windows.Forms.DockStyle.Top;
            this.playingStatusControl3.Location = new System.Drawing.Point(0, 440);
            this.playingStatusControl3.Name = "playingStatusControl3";
            this.playingStatusControl3.Size = new System.Drawing.Size(162, 220);
            this.playingStatusControl3.TabIndex = 21;
            // 
            // playingStatusControl2
            // 
            this.playingStatusControl2.BackColor = System.Drawing.Color.Transparent;
            this.playingStatusControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.playingStatusControl2.Location = new System.Drawing.Point(0, 220);
            this.playingStatusControl2.Name = "playingStatusControl2";
            this.playingStatusControl2.Size = new System.Drawing.Size(162, 220);
            this.playingStatusControl2.TabIndex = 20;
            // 
            // playingStatusControl1
            // 
            this.playingStatusControl1.BackColor = System.Drawing.Color.Transparent;
            this.playingStatusControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.playingStatusControl1.Location = new System.Drawing.Point(0, 0);
            this.playingStatusControl1.Name = "playingStatusControl1";
            this.playingStatusControl1.Size = new System.Drawing.Size(162, 220);
            this.playingStatusControl1.TabIndex = 19;
            // 
            // Player
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.comboBoxDevice);
            this.Controls.Add(this.playingStatusControl3);
            this.Controls.Add(this.playingStatusControl2);
            this.Controls.Add(this.playingStatusControl1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Player";
            this.Size = new System.Drawing.Size(162, 709);
            this.Load += new System.EventHandler(this.Player_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Player playingStatusControl1;
        private Player playingStatusControl2;
        private Player playingStatusControl3;
        private System.Windows.Forms.ComboBox comboBoxDevice;
    }
}

