using System.Collections.Generic;
using System.Windows.Forms;

namespace BerichtManager.HelperClasses
{
	/// <summary>
	/// Functions to find Reports in trees
	/// </summary>
	public class ReportFinder
	{
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
		public static void FindReports(TreeNode node, out List<TreeNode> reports)
		{
			reports = new List<TreeNode>();
			foreach (TreeNode child in node.Nodes)
			{
				if (IsReportValid(child.Text))
					reports.Add(child);
				else
				{
					FindReports(child, out List<TreeNode> childReports);
					reports.AddRange(childReports);
				}
			}
		}
	}
}
