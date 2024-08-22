namespace BerichtManager.WordTemplate
{
	partial class WordTemplateForm
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
			this.scMainView = new System.Windows.Forms.SplitContainer();
			this.flpOrder = new System.Windows.Forms.FlowLayoutPanel();
			this.paTitleOrder = new System.Windows.Forms.Panel();
			this.laOrdered = new System.Windows.Forms.Label();
			this.flpFieldOptions = new System.Windows.Forms.FlowLayoutPanel();
			this.paTitleOptions = new System.Windows.Forms.Panel();
			this.laFieldOptions = new System.Windows.Forms.Label();
			this.paButtons = new System.Windows.Forms.Panel();
			this.btSave = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btClose = new BerichtManager.OwnControls.FocusColoredFlatButton();
			((System.ComponentModel.ISupportInitialize)(this.scMainView)).BeginInit();
			this.scMainView.Panel1.SuspendLayout();
			this.scMainView.Panel2.SuspendLayout();
			this.scMainView.SuspendLayout();
			this.paTitleOrder.SuspendLayout();
			this.paTitleOptions.SuspendLayout();
			this.paButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// scMainView
			// 
			this.scMainView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scMainView.Location = new System.Drawing.Point(0, 0);
			this.scMainView.Name = "scMainView";
			// 
			// scMainView.Panel1
			// 
			this.scMainView.Panel1.Controls.Add(this.flpOrder);
			this.scMainView.Panel1.Controls.Add(this.paTitleOrder);
			// 
			// scMainView.Panel2
			// 
			this.scMainView.Panel2.Controls.Add(this.flpFieldOptions);
			this.scMainView.Panel2.Controls.Add(this.paTitleOptions);
			this.scMainView.Size = new System.Drawing.Size(800, 409);
			this.scMainView.SplitterDistance = 266;
			this.scMainView.SplitterWidth = 2;
			this.scMainView.TabIndex = 0;
			// 
			// flpOrder
			// 
			this.flpOrder.AllowDrop = true;
			this.flpOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.flpOrder.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flpOrder.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flpOrder.Location = new System.Drawing.Point(0, 23);
			this.flpOrder.Name = "flpOrder";
			this.flpOrder.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.flpOrder.Size = new System.Drawing.Size(266, 386);
			this.flpOrder.TabIndex = 0;
			this.flpOrder.DragDrop += new System.Windows.Forms.DragEventHandler(this.PanelDragDrop);
			this.flpOrder.DragEnter += new System.Windows.Forms.DragEventHandler(this.PanelDragEnter);
			// 
			// paTitleOrder
			// 
			this.paTitleOrder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.paTitleOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.paTitleOrder.Controls.Add(this.laOrdered);
			this.paTitleOrder.Dock = System.Windows.Forms.DockStyle.Top;
			this.paTitleOrder.Location = new System.Drawing.Point(0, 0);
			this.paTitleOrder.Name = "paTitleOrder";
			this.paTitleOrder.Size = new System.Drawing.Size(266, 23);
			this.paTitleOrder.TabIndex = 1;
			// 
			// laOrdered
			// 
			this.laOrdered.AutoSize = true;
			this.laOrdered.Location = new System.Drawing.Point(3, 8);
			this.laOrdered.Name = "laOrdered";
			this.laOrdered.Size = new System.Drawing.Size(72, 13);
			this.laOrdered.TabIndex = 0;
			this.laOrdered.Text = "Order of fields";
			// 
			// flpFieldOptions
			// 
			this.flpFieldOptions.AllowDrop = true;
			this.flpFieldOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.flpFieldOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flpFieldOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flpFieldOptions.Location = new System.Drawing.Point(0, 23);
			this.flpFieldOptions.Name = "flpFieldOptions";
			this.flpFieldOptions.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.flpFieldOptions.Size = new System.Drawing.Size(532, 386);
			this.flpFieldOptions.TabIndex = 0;
			this.flpFieldOptions.DragDrop += new System.Windows.Forms.DragEventHandler(this.PanelDragDrop);
			this.flpFieldOptions.DragEnter += new System.Windows.Forms.DragEventHandler(this.PanelDragEnter);
			// 
			// paTitleOptions
			// 
			this.paTitleOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.paTitleOptions.Controls.Add(this.laFieldOptions);
			this.paTitleOptions.Dock = System.Windows.Forms.DockStyle.Top;
			this.paTitleOptions.Location = new System.Drawing.Point(0, 0);
			this.paTitleOptions.Name = "paTitleOptions";
			this.paTitleOptions.Size = new System.Drawing.Size(532, 23);
			this.paTitleOptions.TabIndex = 2;
			// 
			// laFieldOptions
			// 
			this.laFieldOptions.AutoSize = true;
			this.laFieldOptions.Location = new System.Drawing.Point(3, 9);
			this.laFieldOptions.Name = "laFieldOptions";
			this.laFieldOptions.Size = new System.Drawing.Size(66, 13);
			this.laFieldOptions.TabIndex = 1;
			this.laFieldOptions.Text = "Field options";
			// 
			// paButtons
			// 
			this.paButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.paButtons.Controls.Add(this.btSave);
			this.paButtons.Controls.Add(this.btClose);
			this.paButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.paButtons.Location = new System.Drawing.Point(0, 409);
			this.paButtons.Name = "paButtons";
			this.paButtons.Size = new System.Drawing.Size(800, 41);
			this.paButtons.TabIndex = 3;
			// 
			// btSave
			// 
			this.btSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSave.Location = new System.Drawing.Point(631, 5);
			this.btSave.Name = "btSave";
			this.btSave.Size = new System.Drawing.Size(75, 23);
			this.btSave.TabIndex = 1;
			this.btSave.Text = "Save";
			this.btSave.UseVisualStyleBackColor = true;
			this.btSave.Click += new System.EventHandler(this.OnSaveClicked);
			// 
			// btClose
			// 
			this.btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btClose.Location = new System.Drawing.Point(712, 5);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(75, 23);
			this.btClose.TabIndex = 0;
			this.btClose.Text = "Close";
			this.btClose.UseVisualStyleBackColor = true;
			this.btClose.Click += new System.EventHandler(this.OnCloseClicked);
			// 
			// WordTemplateForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.scMainView);
			this.Controls.Add(this.paButtons);
			this.MinimumSize = new System.Drawing.Size(196, 41);
			this.Name = "WordTemplateForm";
			this.Text = "Pick the order of text fields in Word template";
			this.scMainView.Panel1.ResumeLayout(false);
			this.scMainView.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scMainView)).EndInit();
			this.scMainView.ResumeLayout(false);
			this.paTitleOrder.ResumeLayout(false);
			this.paTitleOrder.PerformLayout();
			this.paTitleOptions.ResumeLayout(false);
			this.paTitleOptions.PerformLayout();
			this.paButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer scMainView;
		private System.Windows.Forms.FlowLayoutPanel flpOrder;
		private System.Windows.Forms.FlowLayoutPanel flpFieldOptions;
		private System.Windows.Forms.Panel paTitleOrder;
		private System.Windows.Forms.Panel paButtons;
		private OwnControls.FocusColoredFlatButton btSave;
		private OwnControls.FocusColoredFlatButton btClose;
		private System.Windows.Forms.Label laFieldOptions;
		private System.Windows.Forms.Label laOrdered;
		private System.Windows.Forms.Panel paTitleOptions;
	}
}