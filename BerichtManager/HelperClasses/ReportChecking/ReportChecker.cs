using BerichtManager.Config;
using BerichtManager.OwnControls;
using BerichtManager.ThemeManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		/// Searches report numberf for discrepancies
		/// </summary>
		/// <param name="root">Root <see cref="TreeNode"/> of folder to check</param>
		/// <param name="wordApp"><see cref="Word.Application"/> to open documents in</param>
		/// <returns><see cref="List{T}"/> of <see cref="ReportDiscrepancy"/></returns>
		internal static List<ReportDiscrepancy> SearchNumbers(TreeNode root, Word.Application wordApp)
		{
			Dictionary<TreeNode, int> reportNumbers = new Dictionary<TreeNode, int>();
			List<ReportDiscrepancy> reportDiscrepancies = new List<ReportDiscrepancy>();

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
				Word.Document doc = wordApp.Documents.Open(FileName: Path.Combine(ConfigHandler.Instance.ReportPath(), path), ReadOnly: true);
				if (doc.FormFields.Count >= 10 && GetReportNumber(doc, out int reportNumber))
					reportNumbers.Add(report, reportNumber);
				else
					ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, text: $"Unable to read report number from {path}", title: "Unable to read report number");
				doc.Close();
			}

			reportNumbers = reportNumbers.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
			List<TreeNode> nodes = reportNumbers.Keys.ToList().OrderBy(x => reportNumbers[x]).ToList();
			for (int i = 0; i < nodes.Count - 1; i++)
			{
				if (reportNumbers[nodes[i + 1]] - reportNumbers[nodes[i]] > 1)
					reportDiscrepancies.Add(new ReportDiscrepancy(GenerateTreePath(nodes[i]), GenerateTreePath(nodes[i + 1]), ReportDiscrepancy.DiscrepancyKind.Number));
			}

			return reportDiscrepancies;
		}

		/// <summary>
		/// Generates a file path relative to but excluding root
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to generate path for</param>
		/// <returns>Path relative to root</returns>
		private static string GenerateTreePath(TreeNode node)
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
		private static bool IsReportValid(string name)
		{
			return name.EndsWith(".docx") && !name.StartsWith("~$");
		}

		/// <summary>
		/// Finds reports in <paramref name="node"/> and its cildren
		/// </summary>
		/// <param name="node">Directory to search</param>
		/// <param name="reports"><see cref="List{T}"/> of found reports to fill</param>
		/// <returns><see cref="List{T}"/> of <see cref="TreeNode"/>s that represent all valid reports</returns>
		private static List<TreeNode> FindReports(TreeNode node, List<TreeNode> reports = null)
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
		private static bool GetReportNumber(Word.Document document, out int number)
		{
			if (!int.TryParse(document.FormFields[ReportFields.ReportNumber + 1].Result, out int reportNumber))
			{
				ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, text: $"Unable to read report number of {Path.Combine(document.Path, document.Name)}", title: "Unable to read number");
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
		private static DateTime GetStartDate(Word.Document document)
		{
			if (!DateTime.TryParse(document.FormFields[ReportFields.StartDate].Result, out DateTime startDate))
			{
				ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, text: $"Unable to read start date of {Path.Combine(document.Path, document.Name)}", title: "Unable to read date");
				return new DateTime();
			}
			return startDate;
		}

		/// <summary>
		/// Gets end date of report
		/// </summary>
		/// <param name="document"><see cref="Word.Document"/> to get end date from</param>
		/// <returns>End date of report</returns>
		private static DateTime GerEndDate(Word.Document document)
		{
			if (!DateTime.TryParse(document.FormFields[ReportFields.StartDate].Result, out DateTime endDate))
			{
				ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, text: $"Unable to read end date of {Path.Combine(document.Path, document.Name)}", title: "Unable to read date");
				return new DateTime();
			}
			return endDate;
		}
	}
}
