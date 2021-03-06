// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Resources;
using System.Xml;
using System.Threading;
using System.Text;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using Crownwood.Magic.Menus;

namespace ICSharpCode.SharpDevelop.Gui.Pads 
{
	public class ClassScoutTag
	{
		int    line;
		string filename;
		
		public int Line {
			get {
				return line;
			}
		}
		
		public string FileName {
			get {
				return filename;
			}
		}
		
		public ClassScoutTag(int line, string filename)
		{
			this.line     = line;
			this.filename = filename;
		}
	}
	
	/// <summary>
	/// This class is the project scout tree view
	/// </summary>
	public class ClassScout : TreeView, IPadContent
	{
		Panel contentPanel = new Panel();
		int imageIndexOffset = -1;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		IClassScoutNodeBuilder[] classBrowserNodeBuilders = new IClassScoutNodeBuilder[] {
			new DefaultDotNetClassScoutNodeBuilder()
		};
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.ClassScoutLabel");
			}
		}
		
		public Bitmap Icon {
			get {
				return resourceService.GetBitmap("Icons.16x16.Class");
			}
		}
		
		public Control Control {
			get {
				return contentPanel;
			}
		}
				
		public ClassScout()
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			
			this.ImageList  = classBrowserIconService.ImageList;
			
			imageIndexOffset                      = ImageList.Images.Count;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (Image img in fileUtilityService.ImageList.Images) {
				ImageList.Images.Add(img);
			}
			
			LabelEdit     = false;
			HotTracking   = false;
			AllowDrop     = true;
			HideSelection = false;
			Dock          = DockStyle.Fill;
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			projectService.CombineOpened += new CombineEventHandler(OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
			
			IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			
			parserService.ParseInformationChanged += new ParseInformationEventHandler(UpdateParseInformation);
			
			contentPanel.Controls.Add(this);
			AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
			ambienceService.AmbienceChanged += new EventHandler(AmbienceChangedEvent);
		}
		
		void AmbienceChangedEvent(object sender, EventArgs e)
		{
			if (parseCombine != null) {
				DoPopulate();
			}
		}
		
		void UpdateParseInformation(TreeNodeCollection nodes, ParseInformationEventArgs e)
		{
			foreach (TreeNode node in nodes) {
				if (node.Tag is IProject) {
					IProject p = (IProject)node.Tag;
					if (p.IsFileInProject(e.FileName)) {
						foreach (IClassScoutNodeBuilder classBrowserNodeBuilder in classBrowserNodeBuilders) {
							if (classBrowserNodeBuilder.CanBuildClassTree(p)) {
								classBrowserNodeBuilder.UpdateClassTree(node, e);
							}
						}
					}
				} else {
					UpdateParseInformation(node.Nodes, e);
				}
			}
		}
		
		void UpdateParseInformation(object sender, ParseInformationEventArgs e)
		{
			/*
			BeginUpdate();
			UpdateParseInformation(Nodes, e);
			EndUpdate();*/
		}
		
		public void RedrawContent()
		{
		}
				
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			TreeNode node = SelectedNode;
			ClassScoutTag tag = node.Tag as ClassScoutTag;
			if (tag != null) {
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile(tag.FileName);
				
				IViewContent content = fileService.GetOpenFile(tag.FileName).ViewContent;
				if (content is IPositionable) {
					if (tag.Line > 0) {
						((IPositionable)content).JumpTo(new Point(0, tag.Line - 1));
						content.Control.Focus();
					}
				}
			}
		}
		
		void DoPopulate()
		{
			BeginUpdate();
			Nodes.Clear();
			try {
				Populate(parseCombine, Nodes);
			} catch (Exception e) {
				MessageBox.Show(e.ToString(), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			EndUpdate();
		}
		
		Combine parseCombine;
		
		delegate void MyD();
		
		void StartPopulating()
		{
			StartParser();
			Invoke(new MyD(DoPopulate));
		}
		
		void StartParser()
		{
			ParseCombine(parseCombine);
		}
		
		void StartCombineparse(Combine combine)
		{
			parseCombine = combine;
			
			System.Threading.Thread t = new Thread(new ThreadStart(StartPopulating));
			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}
		
		public void ParseCombine(Combine combine)
		{
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is ProjectCombineEntry) {
					ParseProject(((ProjectCombineEntry)entry).Project);
				} else { 
					ParseCombine(((CombineCombineEntry)entry).Combine);
				}
			}
		}
		
		void ParseProject(IProject p)
		{
			if (p.ProjectType == "C#") {
	 			foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.BuildAction == BuildAction.Compile) {
						IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
						parserService.ParseFile(finfo.Name);
					}
	 			}
			}
		}
		
		void Populate(IProject p, TreeNodeCollection nodes)
		{
			// only C# is currently supported.
			bool builderFound = false;
			foreach (IClassScoutNodeBuilder classBrowserNodeBuilder in classBrowserNodeBuilders) {
				if (classBrowserNodeBuilder.CanBuildClassTree(p)) {
					TreeNode prjNode = classBrowserNodeBuilder.BuildClassTreeNode(p, imageIndexOffset);
					nodes.Add(prjNode);
					prjNode.Tag = p;
					builderFound = true;
					break;
				}
			}
			
			// no builder found -> create 'dummy' node
			if (!builderFound) {
				TreeNode prjNode = new TreeNode(p.Name);
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				prjNode.SelectedImageIndex = prjNode.ImageIndex = imageIndexOffset + fileUtilityService.GetImageIndexForProjectType(p.ProjectType);
				prjNode.Nodes.Add(new TreeNode("No class builder found"));
				prjNode.Tag = p;
				nodes.Add(prjNode);
				
			}
		}

		public void Populate(Combine combine, TreeNodeCollection nodes)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			TreeNode combineNode = new TreeNode(combine.Name);
			combineNode.SelectedImageIndex = combineNode.ImageIndex = classBrowserIconService.CombineIndex;
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is ProjectCombineEntry) {
					Populate(((ProjectCombineEntry)entry).Project, combineNode.Nodes);
				} else { 
					Populate(((CombineCombineEntry)entry).Combine, combineNode.Nodes);
				}
			}
			nodes.Add(combineNode);
		}
								
		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			Nodes.Clear();
			Nodes.Add(new TreeNode("Loading..."));
			StartCombineparse(e.Combine);
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			Nodes.Clear();
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			TreeNode node = (TreeNode)GetNodeAt(e.X, e.Y);
			
			if (node != null) {
				SelectedNode = node;
			}
			base.OnMouseDown(e);
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && SelectedNode != null && SelectedNode is AbstractClassScoutNode) {
		    	AbstractClassScoutNode selectedBrowserNode = (AbstractClassScoutNode)SelectedNode;
				if (selectedBrowserNode.ContextmenuAddinTreePath != null && selectedBrowserNode.ContextmenuAddinTreePath.Length > 0) {
					MenuCommand[] contextMenu = (MenuCommand[])(AddInTreeSingleton.AddInTree.GetTreeNode(selectedBrowserNode.ContextmenuAddinTreePath).BuildChildItems(this)).ToArray(typeof(MenuCommand));
					if (contextMenu.Length > 0) {
						PopupMenu popup = new PopupMenu();
						PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
						popup.Style     = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
						popup.MenuCommands.AddRange(contextMenu);
						popup.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
						return;
					}
				}
			}
			
			base.OnMouseUp(e);
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		
		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}
		
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;
	}
}
