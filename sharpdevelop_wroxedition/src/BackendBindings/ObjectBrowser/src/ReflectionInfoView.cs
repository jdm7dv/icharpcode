// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using WinForms = System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;

namespace SharpDevelop.Gui.Edit.Reflection
{
	public class ReflectionInfoView : Panel
	{
		LinkLabel      namelabel = new LinkLabel (); 
		WinForms.Label statlabel = new WinForms.Label();
		ReflectionTree tree;
		
		public ReflectionInfoView(ReflectionTree tree)
		{
			this.tree = tree;
			
			Dock = DockStyle.Top;
			BorderStyle = BorderStyle.Fixed3D;
			
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
			
			namelabel.Location = new Point(10, 10);
			namelabel.Size     = new Size(Width - 10, 20);
			namelabel.Anchor   = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			
			statlabel.Location = new Point(10, 28);
			statlabel.Size     = new Size(Width - 10, 20);
			statlabel.Anchor   = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			
			Controls.AddRange(new Control[] { namelabel, statlabel });
		}
		
		void AddText(WinForms.Label label, string text)
		{
			label.Text += text;
		}
		
		void AddLink(LinkLabel label, string linktext)
		{
			int startpos = label.Text.Length;
			label.Text += linktext;
			label.Links.Add(startpos, linktext.Length);
		}
		
		void AddBold(WinForms.Label label, string text)
		{
			Font oldfont = label.Font;
			label.Font = new Font(oldfont, FontStyle.Bold);
			label.Text += text;
			label.Font = oldfont;
		}

		void SelectNode(object sender, TreeViewEventArgs e)
		{
			ReflectionNode node = (ReflectionNode)e.Node;
			namelabel.Text = "";
			statlabel.Text = "";
			namelabel.Links.Clear();
			
			switch (node.Type) {
				case ReflectionNodeType.Link:
					break;
				case ReflectionNodeType.Constructor:
					break;
				case ReflectionNodeType.Folder:
					break;
				case ReflectionNodeType.Library:
					break;
				case ReflectionNodeType.SubTypes:
					break;
				case ReflectionNodeType.SuperTypes:
					break;
				case ReflectionNodeType.Resource:
					AddText(namelabel, "Resource ");
					AddBold(namelabel, node.Text);
					break;
				case ReflectionNodeType.Module:
					AddText(namelabel, "Module ");
					AddBold(namelabel, node.Text);
					break;
				case ReflectionNodeType.Type:
					AddText(namelabel, "Type ");
					AddBold(namelabel, node.Text);
					break;
				case ReflectionNodeType.Method:
					AddText(namelabel, "Method ");
					AddBold(namelabel, node.Text);
					break;
				case ReflectionNodeType.Field:
					AddText(namelabel, "Field ");
					AddBold(namelabel, node.Text);
					break;
				case ReflectionNodeType.Property:
					AddText(namelabel, "Property ");
					AddBold(namelabel, node.Text);
					break;
				case ReflectionNodeType.Reference:
					AddText(namelabel, "Reference ");
					AddBold(namelabel, node.Text);
					break;
				case ReflectionNodeType.Event:
					AddText(namelabel, "Event ");
					AddBold(namelabel, node.Text);
					break;
				
				case ReflectionNodeType.Assembly:
					AddText(namelabel, "Assembly ");
					AddBold(namelabel, node.Text);
					break;
					
				case ReflectionNodeType.Namespace:
					AddText(namelabel, "Namespace ");
					AddBold(namelabel, node.Text);
					break;
				
				default:
					throw new Exception("unknown node type " + node.Type.ToString());
			}
		}
	}
}
