using BerichtManager.Config;
using BerichtManager.OwnControls;
using BerichtManager.ThemeManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace BerichtManager.HelperClasses.ReportChecking
{
	/// <summary>
	/// Class containing methods for checking reports for discrepancies
	/// </summary>
	internal class ReportChecker
	{
		/// <summary>
		/// Contents of <see cref="Word.FormField"/> in <see cref="Word.Document"/>
		/// </summary>
		internal enum ReportFields
		{
			Name = 1,
			ReportNumber,
			StartDate,
			EndDate,
			Year,
			WorkField,
			SeminarsField,
			SchoolField,
			SignDateY,
			SignDateS
		}
		/// <summary>
		/// <see cref="Word.Application"/> to open reports with
		/// </summary>
		private Word.Application WordApp { get; set; }
		/// <summary>
		/// Cache for report numbers and start dates
		/// </summary>
		public Dictionary<TreeNode, (int ReportNumber, DateTime StartDate)> ReportsCache { get; } = new Dictionary<TreeNode, (int ReportNumber, DateTime StartDate)>();

		internal ReportChecker(Word.Application wordApp)
		{
			WordApp = wordApp;
		}

		/// <summary>
		/// Searches report numberf for discrepancies
		/// </summary>
		/// <param name="root">Root <see cref="TreeNode"/> of folder to check</param>
		/// <param name="wordApp"><see cref="Word.Application"/> to open documents in</param>
		/// <returns><see cref="List{T}"/> of <see cref="ReportDiscrepancy"/> or <see langword="null"/> if an error was encountered</returns>
		internal List<ReportDiscrepancy> SearchNumbers(TreeNode root)
		{
			ReportsCache.Clear();
			Dictionary<TreeNode, int> reportNumbers = new Dictionary<TreeNode, int>();
			Dictionary<TreeNode, DateTime> startDates = new Dictionary<TreeNode, DateTime>();
			List<ReportDiscrepancy> reportDiscrepancies = new List<ReportDiscrepancy>();
			//List for duplicate report numbers
			Dictionary<TreeNode, int> duplicateNumbers = new Dictionary<TreeNode, int>();
			//List for duplicate start dates
			Dictionary<TreeNode, DateTime> duplicateStartDates = new Dictionary<TreeNode, DateTime>();

			List<TreeNode> reportNodes = FindReports(root);
			foreach (TreeNode report in reportNodes)
			{
				string path = report.Text;
				TreeNode currentNode = report;
				while (currentNode.Parent != null)
				{
					if (currentNode.Parent != root)
						path = Path.Combine(currentNode.Parent.Text, path);
					currentNode = currentNode.Parent;
				}
				Word.Document doc = WordApp.Documents.Open(FileName: Path.Combine(ConfigHandler.Instance.ReportPath(), path), ReadOnly: true);
				if (doc.FormFields.Count < 10)
				{
					ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, text: $"The report {path} does not contain the necessary form fields, checking was canceled", title: "Invalid report");
					doc.Close(SaveChanges: false);
					return null;
				}
				if (GetReportNumber(doc, out int reportNumber))
				{
					if (!reportNumbers.ContainsValue(reportNumber))
						reportNumbers.Add(report, reportNumber);
					else
						duplicateNumbers.Add(report, reportNumber);
				}
				else
				{
					ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, text: $"Unable to read report number from {path}, checking was canceled", title: "Unable to read report number");
					doc.Close(SaveChanges: false);
					return null;
				}
				if (GetStartDate(doc, out DateTime startDate))
				{
					if (!startDates.ContainsValue(startDate))
						startDates.Add(report, startDate);
					else
						duplicateStartDates.Add(report, startDate);
				}
				else
				{
					ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, text: $"Unable to read start date from {path}, checking was canceled", title: "Unable to read start date");
					doc.Close(SaveChanges: false);
					return null;
				}
				ReportsCache.Add(report, (reportNumber, startDate));
				doc.Close(SaveChanges: false);
			}

			if (duplicateNumbers.Count > 0 || duplicateStartDates.Count > 0)
			{
				StringBuilder message = new StringBuilder();
				message.AppendLine("At least one duplicate start date or report number was found, checking was canceled: ");
				duplicateNumbers.Keys.ToList().ForEach(node =>
				{
					message.AppendLine($"Number: {GenerateTreePath(node)} = {GenerateTreePath(reportNumbers.First(kvp => kvp.Value == duplicateNumbers[node]).Key)}");
				});
				duplicateStartDates.Keys.ToList().ForEach(node =>
				{
					message.AppendLine($"Start date: {GenerateTreePath(node)} = {GenerateTreePath(startDates.First(kvp => kvp.Value == duplicateStartDates[node]).Key)}");
				});
				ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, text: message.ToString(), title: "Duplicates found");
				return null;
			}

			reportNumbers = reportNumbers.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
			List<TreeNode> nodes = reportNumbers.Keys.ToList().OrderBy(x => reportNumbers[x]).ToList();
			for (int i = 0; i < nodes.Count - 1; i++)
			{
				if (reportNumbers[nodes[i + 1]] - reportNumbers[nodes[i]] > 1)
					reportDiscrepancies.Add(
						new ReportDiscrepancy(GenerateTreePath(nodes[i]),
						GenerateTreePath(nodes[i + 1]),
						ReportDiscrepancy.DiscrepancyKind.Number,
						reportNumbers[nodes[i]],
						startDates[nodes[i]],
						reportNumbers[nodes[i + 1]],
						startDates[nodes[i + 1]])
					);
			}

			return reportDiscrepancies;
		}

		/// <summary>
		/// Generates a file path relative to but excluding root
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to generate path for</param>
		/// <returns>Path relative to root</returns>
		public string GenerateTreePath(TreeNode node)
		{
			string path = node.Text;
			TreeNode currentNode = node;
			while (currentNode.Parent != null)
			{
				if (currentNode.Parent != node)
					path = Path.Combine(currentNode.Parent.Text, path);
				currentNode = currentNode.Parent;
			}
			return path;
		}

		/// <summary>
		/// Checks if report name is valid and not a temp
		/// </summary>
		/// <param name="name">Name of report to check</param>
		/// <returns><see langword="true"/> if report is valid and <see langword="false"/> otherise</returns>
		private bool IsReportValid(string name)
		{
			return name.EndsWith(".docx") && !name.StartsWith("~$");
		}

		/// <summary>
		/// Finds reports in <paramref name="node"/> and its cildren
		/// </summary>
		/// <param name="node">Directory to search</param>
		/// <param name="reports"><see cref="List{T}"/> of found reports to fill</param>
		/// <returns><see cref="List{T}"/> of <see cref="TreeNode"/>s that represent all valid reports</returns>
		public List<TreeNode> FindReports(TreeNode node, List<TreeNode> reports = null)
		{
			if (reports == null)
				reports = new List<TreeNode>();
			foreach (TreeNode child in node.Nodes)
			{
				if (IsReportValid(child.Text))
					reports.Add(child);
				else
					FindReports(child, reports);
			}
			return reports;
		}

		/// <summary>
		/// Gets report number from report
		/// </summary>
		/// <param name="document"><see cref="Word.Document"/> to get number from</param>
		/// <param name="number">Number of report</param>
		/// <returns><see langword="true"/> if number was found and <see langword="false"/> otherwise</returns>
		private bool GetReportNumber(Word.Document document, out int number)
		{
			if (!int.TryParse(document.FormFields[ReportFields.ReportNumber].Result, out int reportNumber))
			{
				number = -1;
				return false;
			}
			number = reportNumber;
			return true;
		}

		/// <summary>
		/// Gets start date of report
		/// </summary>
		/// <param name="document"><see cref="Word.Document"/> to get start date from</param>
		/// <returns>Start date of report</returns>
		private bool GetStartDate(Word.Document document, out DateTime startDate)
		{
			if (!DateTime.TryParse(document.FormFields[ReportFields.StartDate].Result, out DateTime rstartDate))
			{
				startDate = new DateTime();
				return false;
			}
			startDate = rstartDate;
			return true;
		}

		/// <summary>
		/// Gets end date of report
		/// </summary>
		/// <param name="document"><see cref="Word.Document"/> to get end date from</param>
		/// <returns>End date of report</returns>
		private DateTime GerEndDate(Word.Document document)
		{
			if (!DateTime.TryParse(document.FormFields[ReportFields.StartDate].Result, out DateTime endDate))
			{
				return new DateTime();
			}
			return endDate;
		}
	}
}
