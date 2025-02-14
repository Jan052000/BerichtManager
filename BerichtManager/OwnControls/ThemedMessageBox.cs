using System;
using System.Drawing;
using System.Windows.Forms;
using BerichtManager.HelperClasses;
using BerichtManager.ThemeManagement;
using Newtonsoft.Json.Linq;

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
			ThemeSetter.SetThemes(this);
			ThemeSetter.SetThemes(toolTip1);
			SetMinSize();
			this.Text = title;
			rtbText.Text = text;
			Message = text;
			Buttons = buttons;
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
		/// <param name="additionalData">Additional data to display</param>
		/// <param name="errorWhileActivity">Description of process that threw the error ("while" is added if not contained in string) e.g. "while editing file..."</param>
		public static void Error(Exception ex, string title = "", bool blockExecution = true, bool allowMessageHighlight = false, bool createLogFile = true, JObject additionalData = null, string errorWhileActivity = "")
		{
			string errorMessage = "An unexpected exception has occurred";
			if (!string.IsNullOrEmpty(errorWhileActivity))
			{
				if (errorWhileActivity.StartsWith("while"))
					errorMessage += $" {errorWhileActivity}";
				else
					errorMessage += $"while {errorWhileActivity}";
			}
			if (createLogFile)
				errorMessage += $", a complete log has been saved to\n{Logger.LogError(ex, additionalData: additionalData)}:";
			else
				errorMessage += ":";
			errorMessage += $"\n{ex.GetType().Name}, {ex.Message}: \n{ex.StackTrace}";
			ThemedMessageBox mb = new ThemedMessageBox(text: errorMessage, title: string.IsNullOrEmpty(title) ? ex.GetType().Name : title, allowMessageHighlight: allowMessageHighlight);
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
		private void SetMinSize()
		{
			int minTextHeight = TextRenderer.MeasureText("T", rtbText.Font).Height;
			int minHeight = minTextHeight + paButtons.Height + paButtons.Margin.Top + paButtons.Margin.Bottom + Size.Height - ClientSize.Height;
			Button firstButton = FindFirstButton();
			int designMarginClipboard = firstButton.Location.X - paButtons.Location.X - firstButton.Margin.Left;
			Button lastButton = FindLastButton();
			int designMarginCancel = paButtons.Location.X + paButtons.Width - (lastButton.Location.X + lastButton.Width) - lastButton.Margin.Right;
			int minwidth = Size.Width - ClientSize.Width + Padding.Right + Padding.Left + designMarginClipboard + designMarginCancel;
			foreach (Control control in paButtons.Controls)
			{
				switch (control)
				{
					case Button button:
						minwidth += button.Width + button.Margin.Left + button.Margin.Right;
						break;
				}
			}
			MinimumSize = new Size(minwidth, minHeight);
		}

		/// <summary>
		/// Searches <see cref="Button"/> in <see cref="paButtons"/> with largest x location
		/// </summary>
		/// <returns><see cref="Button"/> in <see cref="paButtons"/> with largest x location</returns>
		private Button FindLastButton()
		{
			return FindButtonFromLocation((result, bt) => result?.Location.X < bt.Location.X);
		}

		/// <summary>
		/// Searches <see cref="Button"/> in <see cref="paButtons"/> with smallest x location
		/// </summary>
		/// <returns><see cref="Button"/> in <see cref="paButtons"/> with smallest x location</returns>
		private Button FindFirstButton()
		{
			return FindButtonFromLocation((result, bt) => result?.Location.X > bt.Location.X);
		}

		/// <summary>
		/// Searches <see cref="paButtons"/> controls for a <see cref="Button"/> based on its' location
		/// </summary>
		/// <param name="predicate"><see cref="ButtonLocationDelegate"/> to use in determining returned button</param>
		/// <returns>Found button <see cref="Button"/> or <see langword="null"/> if no button was found</returns>
		private Button FindButtonFromLocation(ButtonLocationDelegate predicate)
		{
			Button result = null;
			foreach (Control control in paButtons.Controls)
			{
				if (control is Button bt)
				{
					if (result == null || predicate(result, bt))
						result = bt;
				}
			}
			return result;
		}

		/// <summary>
		/// Predicate to find <see cref="Button"/>s from <see cref="paButtons"/> based on location
		/// </summary>
		/// <param name="bt1">Button that is cached as return value</param>
		/// <param name="bt2">Button that is to be compared</param>
		/// <returns><see langword="true"/> if <paramref name="bt2"/> should be returned instead of <paramref name="bt1"/> and <see langword="false"/> otherwise</returns>
		private delegate bool ButtonLocationDelegate(Button bt1, Button bt2);

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
