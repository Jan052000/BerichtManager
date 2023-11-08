using System.Collections;
using System.Windows.Forms;

namespace BerichtManager.HelperClasses
{
	internal class TreeNodeSorter : IComparer
	{
		public int Compare(object x, object y)
		{
			TreeNode xNode = x as TreeNode;
			TreeNode yNode = y as TreeNode;
			if(xNode.Text.Contains("WochenberichtKW") && yNode.Text.Contains("WochenberichtKW"))
			{
				string xNumber = xNode.Text.Replace("WochenberichtKW", "").Replace(".docx", "");
				if (!int.TryParse(xNumber, out int xnumber))
					return string.Compare(xNode.Text, yNode.Text);
				string yNumber = yNode.Text.Replace("WochenberichtKW", "").Replace(".docx", "");
				if (!int.TryParse(yNumber, out int ynumber))
					return string.Compare(xNode.Text, yNode.Text);
				return xnumber.CompareTo(ynumber);
			}
			return string.Compare(xNode.Text, yNode.Text);
		}
	}
}
