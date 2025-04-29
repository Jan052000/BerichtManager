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
			btStop = new OwnControls.FocusColoredFlatButton();
			paEvents = new Panel();
			rtbEvents = new RichTextBox();
			rtbStatus = new RichTextBox();
			paEvents.SuspendLayout();
			SuspendLayout();
			// 
			// btStop
			// 
			btStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btStop.Location = new Point(832, 479);
			btStop.Margin = new Padding(4, 3, 4, 3);
			btStop.Name = "btStop";
			btStop.Size = new Size(88, 27);
			btStop.TabIndex = 0;
			btStop.Text = "Stop";
			btStop.UseVisualStyleBackColor = true;
			btStop.Click += btStop_Click;
			// 
			// paEvents
			// 
			paEvents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			paEvents.Controls.Add(rtbEvents);
			paEvents.Location = new Point(14, 14);
			paEvents.Margin = new Padding(4, 3, 4, 3);
			paEvents.Name = "paEvents";
			paEvents.Size = new Size(905, 458);
			paEvents.TabIndex = 1;
			// 
			// rtbEvents
			// 
			rtbEvents.Dock = DockStyle.Fill;
			rtbEvents.Location = new Point(0, 0);
			rtbEvents.Margin = new Padding(4, 3, 4, 3);
			rtbEvents.Name = "rtbEvents";
			rtbEvents.ReadOnly = true;
			rtbEvents.Size = new Size(905, 458);
			rtbEvents.TabIndex = 0;
			rtbEvents.Text = "";
			// 
			// rtbStatus
			// 
			rtbStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			rtbStatus.Location = new Point(14, 479);
			rtbStatus.Margin = new Padding(4, 3, 4, 3);
			rtbStatus.Name = "rtbStatus";
			rtbStatus.ReadOnly = true;
			rtbStatus.Size = new Size(810, 27);
			rtbStatus.TabIndex = 2;
			rtbStatus.TabStop = false;
			rtbStatus.Text = "";
			// 
			// EventProgressForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(933, 519);
			Controls.Add(rtbStatus);
			Controls.Add(paEvents);
			Controls.Add(btStop);
			Margin = new Padding(4, 3, 4, 3);
			Name = "EventProgressForm";
			Text = "UploadProgressForm";
			FormClosing += UploadProgressForm_FormClosing;
			paEvents.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private OwnControls.FocusColoredFlatButton btStop;
		private System.Windows.Forms.Panel paEvents;
		private System.Windows.Forms.RichTextBox rtbEvents;
		private System.Windows.Forms.RichTextBox rtbStatus;
	}
}