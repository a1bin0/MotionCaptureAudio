namespace MotionCaptureAudio
{
	partial class MainWindow
	{
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panel1 = new System.Windows.Forms.Panel();
            this.player = new MotionCaptureAudio.Player();
            this.pictBox = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.player);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(191, 720);
            this.panel1.TabIndex = 1;
            // 
            // player
            // 
            this.player.BackColor = System.Drawing.Color.Black;
            this.player.Dock = System.Windows.Forms.DockStyle.Left;
            this.player.ForeColor = System.Drawing.Color.White;
            this.player.Location = new System.Drawing.Point(0, 0);
            this.player.Name = "player";
            this.player.Size = new System.Drawing.Size(190, 720);
            this.player.TabIndex = 13;
            // 
            // pictBox
            // 
            this.pictBox.BackColor = System.Drawing.Color.Black;
            this.pictBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictBox.Location = new System.Drawing.Point(191, 0);
            this.pictBox.Name = "pictBox";
            this.pictBox.Size = new System.Drawing.Size(800, 720);
            this.pictBox.TabIndex = 2;
            this.pictBox.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 720);
            this.Controls.Add(this.pictBox);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainWindow";
            this.Text = "mca";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictBox;
        private Player player;




    }
}

