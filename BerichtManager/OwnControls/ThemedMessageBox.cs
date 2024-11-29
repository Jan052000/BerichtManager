using System;
using System.Drawing;
using System.Windows.Forms;
using BerichtManager.HelperClasses;
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
		/// <param name="text">Text to be displayed on the message box</param>
		/// <param name="title">Title of the message box</param>
		/// <param name="buttons">Configuration of the buttons on message box</param>
		/// <param name="allowMessageHighlight">Wether or not to allow the user to highlight text in the message field</param>
		/// <returns><see cref="DialogResult"/> of clicked button</returns>
		public static DialogResult Show(string text = "", string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, bool allowMessageHighlight = false)
		{
			return new ThemedMessageBox(text: text, title: title, buttons: buttons, allowMessageHighlight: allowMessageHighlight).ShowDialog();
		}

		/// <summary>
		/// Shows a themed message box and does not block execution
		/// </summary>
		/// <inheritdoc cref="Show(string, string, MessageBoxButtons, bool)" path="/param"/>
		public static void Info(string text = "", string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, bool allowMessageHighlight = false)
		{
			((Control)new ThemedMessageBox(text: text, title: title, buttons: buttons, allowMessageHighlight: allowMessageHighlight)).Show();
		}

		/// <summary>
		/// Shows a themed message box containing an <see cref="Exception"/> as its content and title
		/// </summary>
		/// <param name="ex"><see cref="Exception"/> to display to the user</param>
		/// <param name="blockExecution">Wether or not execution should wait for user to close the <see cref="ThemedMessageBox"/></param>
		/// <inheritdoc cref="Show(string, string, MessageBoxButtons, bool)" path="/param"/>
		/// <param name="createLogFile">Wether or not a file containing the <see cref="Exception"/> should be created</param>
		public static void Error(Exception ex, bool blockExecution = true, bool allowMessageHighlight = false, bool createLogFile = true)
		{
			string errorMessage = "An unexpected exception has occurred";
			if (createLogFile)
				errorMessage += $", a complete log has been saved to\n{Logger.LogError(ex)}:";
			errorMessage += $"{ex.GetType().Name}, {ex.Message}: \n{ex.StackTrace}";
			ThemedMessageBox mb = new ThemedMessageBox(text: errorMessage, title: ex.GetType().Name, allowMessageHighlight: allowMessageHighlight);
			if (blockExecution)
				mb.ShowDialog();
			else
				((Control)mb).Show();
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
					btCancel.Select();
					break;
				case MessageBoxButtons.OKCancel:
				case MessageBoxButtons.AbortRetryIgnore:
				case MessageBoxButtons.YesNo:
				case MessageBoxButtons.RetryCancel:
					btNo.Select();
					break;
				case MessageBoxButtons.YesNoCancel:
					btYes.Select();
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
