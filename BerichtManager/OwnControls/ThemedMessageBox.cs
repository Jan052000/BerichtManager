﻿using System;
using System.Drawing;
using System.Windows.Forms;
using BerichtManager.ThemeManagement;

namespace BerichtManager.OwnControls
{
	public partial class ThemedMessageBox : Form
	{
		/// <summary>
		/// Message inside of <see cref="rtbText"/>
		/// </summary>
		private string Message { get; set; }
		/// <summary>
		/// Configuration of buttons
		/// </summary>
		private MessageBoxButtons Buttons { get; set; }

		private ThemedMessageBox(string text = "", string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, bool allowMessageHighlight = false)
		{
			InitializeComponent();
			InitializeButtons(buttons);
			SizeToButtons();
			ThemeSetter.SetThemes(this);
			ThemeSetter.SetThemes(toolTip1);
			this.Text = title;
			rtbText.Text = text;
			Message = text;
			Buttons = buttons;
			rtbText.Size = TextRenderer.MeasureText(Message, rtbText.Font);
			if (!allowMessageHighlight)
				rtbText.Enter += UnfocusOnEnter;
		}

		/// <summary>
		/// Shows a themed message box
		/// </summary>
		/// <param name="theme">Theme to style the message box after</param>
		/// <param name="text">Text to be displayed on the message box</param>
		/// <param name="title">Title of the message box</param>
		/// <param name="buttons">Configuration of the buttons on message box</param>
		/// <returns><see cref="DialogResult"/> of clicked button</returns>
		public static DialogResult Show(string text = "", string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, bool allowMessageHighlight = false)
		{
			return new ThemedMessageBox(text: text, title: title, buttons: buttons, allowMessageHighlight: allowMessageHighlight).ShowDialog();
		}

		/// <summary>
		/// Shows a themed message box and does not block execution
		/// </summary>
		/// <param name="theme">Theme to style the message box after</param>
		/// <param name="text">Text to be displayed on the message box</param>
		/// <param name="title">Title of the message box</param>
		/// <param name="buttons">Configuration of the buttons on message box</param>
		public static void Info(string text = "", string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, bool allowMessageHighlight = false)
		{
			((Control)new ThemedMessageBox(text: text, title: title, buttons: buttons, allowMessageHighlight: allowMessageHighlight)).Show();

		}

		/// <summary>
		/// Method to unfocus text box
		/// </summary>
		/// <param name="sender">Sender of event</param>
		/// <param name="e">Event arguments</param>
		private void UnfocusOnEnter(object sender, EventArgs e)
		{
			((Control)sender).Parent.Focus();
		}

		/// <summary>
		/// Sets minimum size to match width of all enabled buttons
		/// </summary>
		private void SizeToButtons()
		{
			Size newSize = new Size(115, 25 + btCopyToClipboard.Height + btCopyToClipboard.Margin.Top + btCopyToClipboard.Margin.Bottom + 32);
			foreach (Control control in Controls)
			{
				switch (control)
				{
					case Button button:
						if (button.Enabled && button != btCopyToClipboard)
							newSize.Width += button.Width + button.Margin.Left + button.Margin.Right;
						break;
				}
			}
			MinimumSize = newSize;
		}

		/// <summary>
		/// Sets texts of buttons and click events using <see cref="MessageBoxButtons"/>
		/// </summary>
		/// <param name="buttons">Configuration of buttons</param>
		private void InitializeButtons(MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				case MessageBoxButtons.OK:
					btYes.Enabled = false;
					btYes.Visible = false;
					btNo.Enabled = false;
					btNo.Visible = false;
					btCancel.Text = "Ok";
					btCancel.Click += OkClicked;
					break;
				case MessageBoxButtons.OKCancel:
					btYes.Enabled = false;
					btYes.Visible = false;
					btNo.Text = "Ok";
					btNo.Click += OkClicked;
					btCancel.Click += CancelClicked;
					break;
				case MessageBoxButtons.AbortRetryIgnore:
					btYes.Text = "Abort";
					btYes.Click += AbortClicked;
					btNo.Text = "Retry";
					btNo.Click += RetryClicked;
					btCancel.Text = "Ignore";
					btCancel.Click += IgnoreClicked;
					break;
				case MessageBoxButtons.YesNoCancel:
					btYes.Click += YesClicked;
					btNo.Click += NoClicked;
					btCancel.Click += CancelClicked;
					break;
				case MessageBoxButtons.YesNo:
					btYes.Enabled = false;
					btYes.Visible = false;
					btNo.Text = "Yes";
					btNo.Click += YesClicked;
					btCancel.Text = "No";
					btCancel.Click += NoClicked;
					break;
				case MessageBoxButtons.RetryCancel:
					btYes.Enabled = false;
					btYes.Visible = false;
					btNo.Text = "Retry";
					btNo.Click += RetryClicked;
					btCancel.Text = "Cancel";
					btCancel.Click += CancelClicked;
					break;
			}
		}

		/// <summary>
		/// Sets button focus when message box is shown
		/// </summary>
		/// <param name="sender">This message box</param>
		/// <param name="e">Event srguments of show event</param>
		private void FocusButton(object sender, EventArgs e)
		{
			switch (Buttons)
			{
				case MessageBoxButtons.OK:
					btCancel.Focus();
					break;
				case MessageBoxButtons.OKCancel:
				case MessageBoxButtons.AbortRetryIgnore:
				case MessageBoxButtons.YesNo:
				case MessageBoxButtons.RetryCancel:
					btNo.Focus();
					break;
				case MessageBoxButtons.YesNoCancel:
					btYes.Focus();
					break;
			}
		}

		/// <summary>
		/// Sets dialogresult
		/// </summary>
		private void OkClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		/// <summary>
		/// Sets dialogresult
		/// </summary>
		private void CancelClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		/// <summary>
		/// Sets dialogresult
		/// </summary>
		private void AbortClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Abort;
			Close();
		}

		/// <summary>
		/// Sets dialogresult
		/// </summary>
		private void RetryClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Retry;
			Close();
		}

		/// <summary>
		/// Sets dialogresult
		/// </summary>
		private void IgnoreClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Ignore;
			Close();
		}

		/// <summary>
		/// Sets dialogresult
		/// </summary>
		private void YesClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Yes;
			Close();
		}

		/// <summary>
		/// Sets dialogresult
		/// </summary>
		private void NoClicked(object sender, EventArgs e)
		{
			DialogResult = DialogResult.No;
			Close();
		}

		/// <summary>
		/// Copies the content of <see cref="Message"/> to the clipboard
		/// </summary>
		private void CopyToClipboard(object sender, EventArgs e)
		{
			Clipboard.SetText(Message);
			toolTip1.Show("Conent copied to clipboard", this, btCopyToClipboard.Location, 1500);
		}
	}
}
