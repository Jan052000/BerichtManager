using System.Diagnostics.CodeAnalysis;

namespace BerichtManager.HelperClasses
{
	/// <summary>
	/// Functions to find Reports in trees
	/// </summary>
	public class ReportFinder
	{
		/// <summary>
		/// Finds reports in <paramref name="node"/> and its cildren
		/// </summary>
		/// <param name="node">Directory to search</param>
		/// <param name="reports"><see cref="List{T}"/> of found reports to fill</param>
		/// <returns><see cref="List{T}"/> of <see cref="TreeNode"/>s that represent all valid reports</returns>
		public static void FindReports(TreeNode? node, [NotNull] out List<TreeNode> reports)
		{
			reports = new List<TreeNode>();
			if (node == null)
				return;
			foreach (TreeNode child in node.Nodes)
			{
				if (ReportUtils.IsNameValid(child.Text))
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
