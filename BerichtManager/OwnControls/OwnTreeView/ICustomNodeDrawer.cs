using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BerichtManager.OwnControls.OwnTreeView
{
	public interface ICustomNodeDrawer
	{
		/// <summary>
		/// Draws nodes to <see cref="TreeView"/>
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		void DrawNode(DrawTreeNodeEventArgs e);
		/// <summary>
		/// Draws node text to <see cref="TreeView"/>
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		void DrawNodeText(DrawTreeNodeEventArgs e);
	}
}
