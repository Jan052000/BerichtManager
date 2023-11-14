using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BerichtManager.ThemeManagement;
using BerichtManager.ThemeManagement.DefaultThemes;

namespace BerichtManager.OwnControls
{
	public partial class ThemedMessageBox : Form
	{
		/// <summary>
		/// Message inside of <see cref="rtbText"/>
		/// </summary>
		private string Message { get; set; }
		/// <summary>
		/// Title in title bar
		/// </summary>
		private string Title { get; set; }
		/// <summary>
		/// Configuration of buttons
		/// </summary>
		private MessageBoxButtons Buttons { get; set; }

		private ThemedMessageBox(ITheme theme, string text = "", string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK)
		{
			InitializeComponent();
			InitializeButtons(buttons);
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			if (theme == null) theme = new DarkMode();
			ThemeSetter.SetThemes(this, theme);
			this.Text = title;
			rtbText.Text = text;
			Message = text;
			Title = title;
			Buttons = buttons;
			AutoSizeBox();
			rtbText.SelectAll();
			rtbText.SelectionAlignment = HorizontalAlignment.Center;
			rtbText.DeselectAll();
			rtbText.Enter += UnfocusOnEnter;
		}

		/// <summary>
		/// Shows a themed message box
		/// </summary>
		/// <param name="theme">Theme to style the message box after</param>
		/// <param name="text">Text to be displayed on the message box</param>
		/// <param name="title">Title of the message box</param>
		/// <param name="buttons">Configuration of the buttons on message box</param>
		/// <returns></returns>
		public static DialogResult Show(ITheme theme, string text = "", string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK)
		{
			return new ThemedMessageBox(theme, text: text, title: title, buttons: buttons).ShowDialog();
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
		/// Calculates and sets the width and height of the message box
		/// </summary>
		private void AutoSizeBox()
		{
			using (Graphics g = rtbText.CreateGraphics())
			{
				SizeF size = g.MeasureString(Message, rtbText.Font);
				int titleBarHeight = 32;
				int titleBarWidth = 8;
				int paddingBottom = 20;
				int paddingTop = 10;
				int messageBoxWidth = titleBarWidth * 2 + (int)size.Width + Padding.Left + Padding.Right + rtbText.Margin.Left + rtbText.Margin.Right;
				int messageBoxHeight = titleBarHeight + (int)size.Height + Padding.Top + Padding.Bottom + rtbText.Margin.Top + rtbText.Margin.Bottom + btYes.Padding.Top + btYes.Padding.Bottom + btYes.Height + paddingBottom + paddingTop;
				if (Width < messageBoxWidth) Width = messageBoxWidth;
				if (Height < messageBoxHeight) Height = messageBoxHeight;
			}
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
	}
}
