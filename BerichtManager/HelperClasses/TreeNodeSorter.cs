using System.Collections;

namespace BerichtManager.HelperClasses
{
	internal class TreeNodeSorter : IComparer
	{
		public int Compare(object x, object y)
		{
			TreeNode xNode = x as TreeNode;
			TreeNode yNode = y as TreeNode;
			if (ReportUtils.IsNameValid(xNode.Text) && ReportUtils.IsNameValid(yNode.Text))
			{
				int result = 0;
				ResolvedValues xResult = NamingPatternResolver.GetValuesFromName(xNode.Text);
				ResolvedValues yResult = NamingPatternResolver.GetValuesFromName(yNode.Text);
				if (xResult.ReportNumber > 0 && yResult.ReportNumber > 0)
					switch (xResult.ReportNumber - yResult.ReportNumber)
					{
						case int diff when diff < 0:
							result--;
							break;
						case int diff when diff > 0:
							result++;
							break;
					}
				if (xResult.CalendarWeek > 0 && yResult.CalendarWeek > 0)
					switch (xResult.CalendarWeek - yResult.CalendarWeek)
					{
						case int diff when diff < 0:
							result--;
							break;
						case int diff when diff > 0:
							result++;
							break;
					}
				return result;
			}
			return string.Compare(xNode.Text, yNode.Text);
		}
	}
}
