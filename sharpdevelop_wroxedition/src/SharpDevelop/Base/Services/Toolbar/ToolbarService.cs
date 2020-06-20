// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	public class ToolbarService : AbstractService
	{
		readonly static string toolBarPath     = "/SharpDevelop/Workbench/ToolBar";
		
		IAddInTreeNode node;
		
		public ToolbarService()
		{
			this.node  = AddInTreeSingleton.AddInTree.GetTreeNode(toolBarPath);
		}
		
		public ToolBar[] CreateToolbars()
		{
			ToolbarItemCodon[] codons = (ToolbarItemCodon[])(node.BuildChildItems(this)).ToArray(typeof(ToolbarItemCodon));
			
			ToolBar[] toolBars = new ToolBar[codons.Length];
			
			for (int i = 0; i < codons.Length; ++i) {
				toolBars[i] = CreateToolBarFromCodon(WorkbenchSingleton.Workbench, codons[i]);
			}
			return toolBars;
		}
		
		public ToolBar CreateToolBarFromCodon(object owner, ToolbarItemCodon codon)
		{
			ToolBar bar = new ToolBar();
			ImageList imgList = new ImageList();
			bar.AutoSize  = true;
			bar.ButtonClick += new ToolBarButtonClickEventHandler(ToolBarButtonClick);
			
			bar.Appearance = ToolBarAppearance.Flat;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			foreach (ToolbarItemCodon childCodon in codon.SubItems) {
				ToolBarButton button = new ToolBarButton();
				
				if (childCodon.Icon != null) {
					button.ImageIndex = imgList.Images.Count;
					imgList.Images.Add(resourceService.GetBitmap(childCodon.Icon));
				}
				
				if (childCodon.ToolTip != null) {
					if (childCodon.ToolTip == "-") {
						button.Style       = ToolBarButtonStyle.Separator;
					} else {
						button.ToolTipText = stringParserService.Parse(childCodon.ToolTip);
					}
				}
				button.Enabled     = childCodon.Enabled;
				if (childCodon.Class != null) {
					button.Tag = childCodon.AddIn.CreateObject(childCodon.Class);
				}
				bar.Buttons.Add(button);
			}
			bar.ImageList = imgList;
			return bar;
		}
		
		void ToolBarButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button.Tag != null) {
				((ICommand)e.Button.Tag).Run();
			}
		}
	}
}
