using BerichtManager.ThemeManagement;
using System;
using System.Windows.Forms;

namespace BerichtManager.Forms
{
	public partial class EventProgressForm : Form
	{
		/// <summary>
		/// Internal events cache
		/// </summary>
		private string eventsText { get; set; } = "";
		/// <summary>
		/// Sets or gets the text of <see cref="rtbEvents"/>
		/// </summary>
		public string EventsText
		{
			get => eventsText;
			set
			{
				if (eventsText != value && !string.IsNullOrWhiteSpace(value))
				{
					bool wasEmpty = string.IsNullOrWhiteSpace(eventsText);
					eventsText = value;
					if (wasEmpty)
						eventsText = eventsText.Trim();
					if (rtbEvents.InvokeRequired)
						rtbEvents.Invoke(new Action(() => rtbEvents.Text = eventsText));
					else
						rtbEvents.Text = eventsText;
				}
			}
		}

		/// <summary>
		/// Internal status cache
		/// </summary>
		private string status { get; set; } = "";
		/// <summary>
		/// Sets a new trimmed status in <see cref="rtbStatus"/> and adds formatted previous status to <see cref="rtbEvents"/> or gets the formatted status
		/// </summary>
		public string Status
		{
			get => status;
			set
			{
				if (status != value)
				{
					EventsText += "\n" + status;
					status = value;
					if (rtbStatus.InvokeRequired)
						rtbStatus.Invoke(new Action(() => rtbStatus.Text = value.Trim()));
					else
						rtbStatus.Text = value.Trim();
				}
			}
		}

		/// <summary>
		/// Only sets the text of <see cref="rtbStatus"/>
		/// </summary>
		public string ManualStatus
		{
			set
			{
				if (status != value)
				{
					rtbStatus.Text = value;
				}
			}
		}

		/// <summary>
		/// Tells form if it should allow close or not
		/// </summary>
		private bool ShouldClose { get; set; } = false;

		public delegate void StopHandler();
		public event StopHandler Stop;

		public EventProgressForm(string title)
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this, ThemeManager.Instance.ActiveTheme);
			Text = title;
		}

		/// <summary>
		/// Tells the <see cref="EventProgressForm"/> that execution has finnished and the form should be allowed to close
		/// </summary>
		public void Done()
		{
			void done()
			{
				btStop.Text = "Close";
				btStop.Click -= btStop_Click;
				btStop.Click += (s, e) => Close();
			}
			ShouldClose = true;
			if (btStop.InvokeRequired)
				btStop.BeginInvoke(new MethodInvoker(() =>
				{
					done();
				}));
			else
			{
				done();
			}
			DialogResult = DialogResult.OK;
		}

		private void btStop_Click(object sender, EventArgs e)
		{
			ShouldClose = true;
			btStop.Text = "Close";
			btStop.Click -= btStop_Click;
			btStop.Click += (s, ev) => Close();
			DialogResult = DialogResult.Cancel;
			Stop();
		}

		private void UploadProgressForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !ShouldClose;
		}
	}
}
