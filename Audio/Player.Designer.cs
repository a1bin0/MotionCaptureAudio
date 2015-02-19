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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.playingStatusControl3 = new MotionCaptureAudio.PlayingStatusControl();
            this.playingStatusControl2 = new MotionCaptureAudio.PlayingStatusControl();
            this.playingStatusControl1 = new MotionCaptureAudio.PlayingStatusControl();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer_Tick);
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
            // comboBoxDevice
            // 
            this.comboBoxDevice.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.comboBoxDevice.FormattingEnabled = true;
            this.comboBoxDevice.Location = new System.Drawing.Point(3, 666);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(156, 21);
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

        private System.Windows.Forms.Timer timer1;
        private PlayingStatusControl playingStatusControl1;
        private PlayingStatusControl playingStatusControl2;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private PlayingStatusControl playingStatusControl3;
        private System.Windows.Forms.ComboBox comboBoxDevice;
    }
}

