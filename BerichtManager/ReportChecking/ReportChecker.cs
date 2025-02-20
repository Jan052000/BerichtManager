using BerichtManager.Config;
using BerichtManager.HelperClasses;
using BerichtManager.OwnControls;
using BerichtManager.ReportChecking.Discrepancies;
using BerichtManager.WordTemplate;
using System.Globalization;
using Word = Microsoft.Office.Interop.Word;

namespace BerichtManager.ReportChecking
{
	/// <summary>
	/// Class containing methods for checking reports for discrepancies
	/// </summary>
	internal class ReportChecker
	{
		/// <summary>
		/// <see cref="Word.Application"/> to open reports with
		/// </summary>
		private Word.Application WordApp { get; set; }

		/// <summary>
		/// Creates a new <see cref="ReportChecker"/> instance
		/// </summary>
		/// <param name="wordApp"><see cref="Word.Application"/> to open reports with</param>
		internal ReportChecker(Word.Application wordApp)
		{
			WordApp = wordApp;
		}

		/// <summary>
		/// Searches report numberf for discrepancies
		/// </summary>
		/// <param name="root">Root <see cref="TreeNode"/> of folder to check</param>
		/// <param name="reportDiscrepancies">List of found discrepancies</param>
		/// <returns><see langword="true"/> if checking was complete and <see langword="false"/> if an error was encountered and a warning was displayed</returns>
		internal bool Check(TreeNode root, out List<IReportDiscrepancy> reportDiscrepancies, CheckKinds check = CheckKinds.All)
		{
			reportDiscrepancies = new List<IReportDiscrepancy>();
			Dictionary<TreeNode, int> reportNumbers = new Dictionary<TreeNode, int>();
			Dictionary<TreeNode, DateTime> startDates = new Dictionary<TreeNode, DateTime>();

			List<TreeNode> reportNodes = FindReports(root);
			foreach (TreeNode report in reportNodes)
			{
				string path = GenerateTreePath(report);
				Word.Document doc = WordApp.Documents.Open(FileName: Path.Combine(ConfigHandler.Instance.ReportPath, path), ReadOnly: true);
				if (!FormFieldHandler.ValidFormFieldCount(doc))
				{
					ThemedMessageBox.Show(text: $"The report {path} does not contain the necessary form fields, checking was canceled", title: "Invalid report");
					doc.Close(SaveChanges: false);
					return false;
				}
				if (GetReportNumber(doc, out int reportNumber))
				{
					if (!reportNumbers.ContainsKey(report))
						reportNumbers.Add(report, reportNumber);
				}
				else
				{
					ThemedMessageBox.Show(text: $"Unable to read report number from {path}, checking was canceled", title: "Unable to read report number");
					doc.Close(SaveChanges: false);
					return false;
				}
				if (GetStartDate(doc, out DateTime startDate))
				{
					if (!startDates.ContainsKey(report))
						startDates.Add(report, startDate);
				}
				else
				{
					ThemedMessageBox.Show(text: $"Unable to read start date from {path}, checking was canceled", title: "Unable to read start date");
					doc.Close(SaveChanges: false);
					return false;
				}
				doc.Close(SaveChanges: false);
			}

			if (check.HasFlag(CheckKinds.Numbers))
			{
				//Add duplicate numbers
				var numberDuplicates = reportNumbers.GroupBy(kvp => kvp.Value).Where(group => group.Count() > 1).ToList();
				foreach (var dupe in numberDuplicates)
				{
					List<string> duplicates = new List<string>();
					foreach (KeyValuePair<TreeNode, int> kvp in dupe)
					{
						duplicates.Add(GenerateTreePath(kvp.Key));
					}
					reportDiscrepancies.Add(new DuplicateNumbersDiscrepancy(duplicates, dupe.Key));
				}
				//Add skipped numbers
				Dictionary<TreeNode, int> sortedNumbers = reportNumbers.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
				List<TreeNode> nodes = sortedNumbers.Keys.ToList().OrderBy(x => sortedNumbers[x]).ToList();
				for (int i = 0; i < nodes.Count - 1; i++)
				{
					if (sortedNumbers[nodes[i + 1]] - sortedNumbers[nodes[i]] > 1)
						reportDiscrepancies.Add(
							new NumberDiscrepancy(GenerateTreePath(nodes[i]), GenerateTreePath(nodes[i + 1]), reportNumbers[nodes[i]], reportNumbers[nodes[i + 1]])
							);
				}
			}
			if (check.HasFlag(CheckKinds.Dates))
			{
				//Add duplicate dates
				var dateDuplicates = startDates.GroupBy(kvp => kvp.Value).Where(group => group.Count() > 1).ToList();
				foreach (var dupe in dateDuplicates)
				{
					List<string> duplicates = new List<string>();
					foreach (KeyValuePair<TreeNode, DateTime> kvp in dupe)
					{
						duplicates.Add(GenerateTreePath(kvp.Key));
					}
					reportDiscrepancies.Add(new DuplicateStartDatesDiscrepancy(duplicates, dupe.Key));
				}
				//Add skipped dates
				var sortedDates = startDates.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
				List<TreeNode> datesList = sortedDates.Keys.ToList().OrderBy(x => sortedDates[x]).ToList();
				for (int i = 0; i < datesList.Count - 1; i++)
				{
					if ((sortedDates[datesList[i + 1]] - sortedDates[datesList[i]]).Days > 7)
						reportDiscrepancies.Add(
							new DateDiscrepancy(GenerateTreePath(datesList[i]), GenerateTreePath(datesList[i + 1]), startDates[datesList[i]], startDates[datesList[i + 1]])
							);
				}
			}
			return true;
		}

		/// <summary>
		/// Enum for checking options
		/// </summary>
		[Flags]
		public enum CheckKinds
		{
			/// <summary>
			/// Only report numbers will be ckecked
			/// </summary>
			Numbers = 1,
			/// <summary>
			/// Only report start dates will be checked
			/// </summary>
			Dates,
			/// <summary>
			/// Report numbers and start dates will be checked
			/// </summary>
			All
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
			while (currentNode.Parent.Parent != null)
			{
				currentNode = currentNode.Parent;
				path = Path.Combine(currentNode.Text, path);
			}
			return path;
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
				if (ReportUtils.IsNameValid(child.Text))
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
			if (!int.TryParse(FormFieldHandler.GetValueFromDoc<string>(Fields.Number, document), out int reportNumber))
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
			if (!DateTime.TryParseExact(FormFieldHandler.GetValueFromDoc<string>(Fields.StartDate, document), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime rstartDate))
			{
				startDate = new DateTime();
				return false;
			}
			startDate = rstartDate;
			return true;
		}
	}
}
