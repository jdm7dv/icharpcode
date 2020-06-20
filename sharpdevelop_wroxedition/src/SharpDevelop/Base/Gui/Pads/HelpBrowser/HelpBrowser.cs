// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using System.Xml;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class HelpBrowserWindow : BrowserPane
	{
		public HelpBrowserWindow() : base(false)
		{
			ContentName = "Help";
		}
	}
	
	public class HelpBrowser : AbstractPadContent
	{
		static readonly string helpPath = Application.StartupPath +
		                                  Path.DirectorySeparatorChar + ".." +
		                                  Path.DirectorySeparatorChar + "doc" +
		                                  Path.DirectorySeparatorChar + "help" +
		                                  Path.DirectorySeparatorChar;
		
		static readonly string helpFileName = helpPath + "HelpConv.xml";
		
		Panel     browserPanel = new Panel();
		TreeView  treeView     = new TreeView();
		
		HelpBrowserWindow helpBrowserWindow = null;
		
		public override Control Control {
			get {
				return browserPanel;
			}
		}
		
		public HelpBrowser() : base("${res:MainWindow.Windows.HelpScoutLabel}", "Icons.16x16.HelpIcon")
		{
			treeView.Dock = DockStyle.Fill;
			treeView.ImageList = new ImageList();
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			
			treeView.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.HelpClosedFolder"));
			treeView.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.HelpOpenFolder"));
			
			treeView.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.HelpTopic"));
			treeView.BeforeExpand   += new TreeViewCancelEventHandler(BeforeExpand);			
			treeView.BeforeCollapse += new TreeViewCancelEventHandler(BeforeCollapse);
			treeView.DoubleClick += new EventHandler(DoubleClick);
			browserPanel.Controls.Add(treeView);
			
			LoadHelpfile();
		}
		
		/// <remarks>
		/// Parses the xml tree and generates a TreeNode tree out of it.
		/// </remarks>
		void ParseTree(TreeNodeCollection nodeCollection, XmlNode parentNode)
		{
			foreach (XmlNode node in parentNode.ChildNodes) {
				switch (node.Name) {
					case "HelpFolder":
						TreeNode newFolderNode = new TreeNode(node.Attributes["name"].InnerText);
						newFolderNode.ImageIndex = newFolderNode.SelectedImageIndex = 0;
						ParseTree(newFolderNode.Nodes, node);
						nodeCollection.Add(newFolderNode);
						break;
					case "HelpTopic":
						TreeNode newNode = new TreeNode(node.Attributes["name"].InnerText);
						newNode.ImageIndex = newNode.SelectedImageIndex = 2;
						newNode.Tag = node.Attributes["link"].InnerText;
						nodeCollection.Add(newNode);
						break;
				}
			}
		}
		
		void LoadHelpfile()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(helpFileName);
			ParseTree(treeView.Nodes, doc.DocumentElement);
		}
		
		void HelpBrowserClose(object sender, EventArgs e)
		{
			helpBrowserWindow = null;
		}
		
		void DoubleClick(object sender, EventArgs e)
		{
			TreeNode node = treeView.SelectedNode;
			if (node.Tag != null) {
				string navigationName = "mk:@MSITStore:" + helpPath + node.Tag.ToString();
				if (helpBrowserWindow == null) {
					helpBrowserWindow = new HelpBrowserWindow();
					WorkbenchSingleton.Workbench.ShowView(helpBrowserWindow);
					helpBrowserWindow.WorkbenchWindow.CloseEvent += new EventHandler(HelpBrowserClose);
				}
				helpBrowserWindow.LoadFile(navigationName);
			}
		}
		
		void BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.ImageIndex < 2) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
			}
		}
		
		void BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{ 
			if (e.Node.ImageIndex < 2) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
			}
		}
	}
}
