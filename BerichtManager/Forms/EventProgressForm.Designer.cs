namespace BerichtManager.Forms
{
	partial class EventProgressForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
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
			this.btStop = new System.Windows.Forms.Button();
			this.paEvents = new System.Windows.Forms.Panel();
			this.rtbEvents = new System.Windows.Forms.RichTextBox();
			this.rtbStatus = new System.Windows.Forms.RichTextBox();
			this.paEvents.SuspendLayout();
			this.SuspendLayout();
			// 
			// btStop
			// 
			this.btStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btStop.Location = new System.Drawing.Point(713, 415);
			this.btStop.Name = "btStop";
			this.btStop.Size = new System.Drawing.Size(75, 23);
			this.btStop.TabIndex = 0;
			this.btStop.Text = "Stop";
			this.btStop.UseVisualStyleBackColor = true;
			this.btStop.Click += new System.EventHandler(this.btStop_Click);
			// 
			// paEvents
			// 
			this.paEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.paEvents.Controls.Add(this.rtbEvents);
			this.paEvents.Location = new System.Drawing.Point(12, 12);
			this.paEvents.Name = "paEvents";
			this.paEvents.Size = new System.Drawing.Size(776, 397);
			this.paEvents.TabIndex = 1;
			// 
			// rtbEvents
			// 
			this.rtbEvents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbEvents.Location = new System.Drawing.Point(0, 0);
			this.rtbEvents.Name = "rtbEvents";
			this.rtbEvents.ReadOnly = true;
			this.rtbEvents.Size = new System.Drawing.Size(776, 397);
			this.rtbEvents.TabIndex = 0;
			this.rtbEvents.Text = "";
			// 
			// rtbStatus
			// 
			this.rtbStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtbStatus.Location = new System.Drawing.Point(12, 415);
			this.rtbStatus.Name = "rtbStatus";
			this.rtbStatus.ReadOnly = true;
			this.rtbStatus.Size = new System.Drawing.Size(695, 23);
			this.rtbStatus.TabIndex = 2;
			this.rtbStatus.Text = "";
			// 
			// UploadProgressForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.rtbStatus);
			this.Controls.Add(this.paEvents);
			this.Controls.Add(this.btStop);
			this.Name = "UploadProgressForm";
			this.Text = "UploadProgressForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UploadProgressForm_FormClosing);
			this.paEvents.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btStop;
		private System.Windows.Forms.Panel paEvents;
		private System.Windows.Forms.RichTextBox rtbEvents;
		private System.Windows.Forms.RichTextBox rtbStatus;
	}
}