// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;

using System.Drawing.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public class DesignPanel : Panel
	{
		public DesignPanel(IDesignerHost host)
		{
			Debug.Assert(host != null);
			
			IRootDesigner rootDesigner = host.GetDesigner(host.RootComponent) as IRootDesigner;
			if (rootDesigner == null) {
				MessageBox.Show("Can't create root designer for " + host.RootComponent, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			if (!TechnologySupported(rootDesigner.SupportedTechnologies, ViewTechnology.WindowsForms)) {
				MessageBox.Show("Root designer does not support Windows Forms view technology.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			this.BackColor = Color.White;
			Control view = (Control)rootDesigner.GetView(ViewTechnology.WindowsForms);
			view.BackColor = Color.White;
			view.Dock = DockStyle.Fill;
			this.Controls.Add(view);
		}
		
		bool TechnologySupported(ViewTechnology[] technologies, ViewTechnology requiredTechnology)
		{
			foreach (ViewTechnology technology in technologies) {
				if (technology == requiredTechnology) {
					return true;
				}
			}
			return false;
		}
	}
}
