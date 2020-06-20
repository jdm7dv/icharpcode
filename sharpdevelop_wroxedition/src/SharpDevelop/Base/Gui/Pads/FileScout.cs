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
using System.Drawing.Drawing2D;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Xml;


using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class DriveObject 
	{
		string text  = null;
		string drive = null;
		
		public string Drive {
			get {
				return drive;
			}
		}
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		/* commented out, because it gives strange noises on some zip drives, floppies etc.
		/// <summary>
		/// For reading the volume label I create a thread, which prevents
		/// too long wait times for getting a volume lable (e.g. floppys, network devices
		/// are slow).
		/// </summary>
		void GetBetterName()
		{
			string label = FileUtility.VolumeLabel(drive); // contributed from Patrick Steele
			if (label.Length > 0) {
				text = label + " ("+ drive.Substring(0, 2) + ")";
			} else 
				return;
		}*/
		
		public DriveObject(string drive) 
		{
			this.drive = drive;
			
			// string label = FSTypeUtility.VolumeLabel(drive); // contributed from Patrick Steele
			string label = "";
			if (label.Length > 0) {
				text = label;
			} else {
				switch(fileUtilityService.GetDriveType(drive)) {
					case DriveType.Removeable:
						text += "Removeable";
					break;
					case DriveType.Fixed:
						text += "Fixed";
					break;
					case DriveType.Cdrom:
						text += "CD";
					break;
					case DriveType.Remote:
						text += "Remote";
					break;
					default:
						text += "Unknown";
					break;
				}
			}
			text += " ("+ drive.Substring(0, 2) + ")";
//			Thread t = new Thread(new ThreadStart(GetBetterName));
//			t.IsBackground  = true;			
//			t.Start();
		}
		
		public override string ToString()
		{
			return text;
		}
	}
	
	public class DriveSelector : ComboBox
	{
		public DriveSelector()
		{
			DropDownStyle          = ComboBoxStyle.DropDownList;
			ScanDrives();
		}
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
	
		void ScanDrives()
		{
			string[] drivelist     = Directory.GetLogicalDrives();
			int      selecteddrive = 0;
			
			foreach (string drive in drivelist) {
				Items.Add(new DriveObject(drive));
				if (fileUtilityService.GetDriveType(drive) == DriveType.Fixed) {
					selecteddrive = Items.Count - 1;
				}
			}
			SelectedIndex = selecteddrive;
		}
		
		public string GetDrive()
		{
			return ((DriveObject)SelectedItem).Drive;
		}
	}
	
	public class FileList : ListView
	{
		public FileList()
		{
			ResourceManager resources = new ResourceManager("ProjectComponentResources", this.GetType().Module.Assembly);
			
			Columns.Add("File", 100, HorizontalAlignment.Left);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			SmallImageList  = fileUtilityService.ImageList;
			HeaderStyle     = ColumnHeaderStyle.None;
			View = View.Details;
			Alignment = ListViewAlignment.Left;
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (Columns != null && Columns.Count > 0)
				Columns[0].Width = Width;
		}
		
		public void ShowFilesInPath(string path)
		{
			string[] files;
			try {
				files = Directory.GetFiles(path);
			} catch (Exception) {
				return;
			}
			BeginUpdate();
			Items.Clear();
			foreach (string file in files) {
				Items.Add(new FileListItem(file));
			}
			EndUpdate();
		}
		
		public class FileListItem : ListViewItem
		{
			string fullname;
			public string FullName {
				get {
					return fullname;
				}
			}
			
			public FileListItem(string fullname) : base(Path.GetFileName(fullname))
			{
				this.fullname = fullname;
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				ImageIndex = fileUtilityService.GetImageIndexForFile(fullname);
			}
		}
		
	}
	
	public class FileScout : UserControl, IPadContent
	{
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		public Control Control {
			get {
				return this;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.FileScoutLabel");
			}
		}
		
		public Bitmap Icon {
			get {
				return resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap");
			}
		}
		
		public void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		DriveSelector driveselector = new DriveSelector();
		Splitter      splitter1     = new Splitter();
		
		FileList   filelister = new FileList();
		ShellTree  filetree   = new ShellTree();
		
		public FileScout()
		{
			Dock      = DockStyle.Fill;
			
			filetree.Dock = DockStyle.Fill;
			filetree.BorderStyle = BorderStyle.Fixed3D;
			filetree.Location = new System.Drawing.Point(0, 22);
			filetree.Size = new System.Drawing.Size(184, 157);
			filetree.TabIndex = 1;
			filetree.AfterSelect += new TreeViewEventHandler(DirectorySelected);
			ImageList imglist = new ImageList();
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.FLOPPY"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.DRIVE"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.CDROM"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.NETWORK"));
			
			filetree.ImageList = imglist;
			
			filelister.Dock = DockStyle.Bottom;
			filelister.BorderStyle = BorderStyle.Fixed3D;
			filelister.Location = new System.Drawing.Point(0, 184);
			
			filelister.Sorting = SortOrder.Ascending;
			filelister.Size = new System.Drawing.Size(184, 450);
			filelister.TabIndex = 3;
			filelister.ItemActivate += new EventHandler(FileSelected);
			
			splitter1.Dock = DockStyle.Bottom;
			splitter1.Location = new System.Drawing.Point(0, 179);
			splitter1.Size = new System.Drawing.Size(184, 5);
			splitter1.TabIndex = 2;
			splitter1.TabStop = false;
			
			driveselector.Size = new System.Drawing.Size(184, 21);
			driveselector.Dock = DockStyle.Top;
			driveselector.TabIndex = 0;
			driveselector.SelectedIndexChanged += new EventHandler(DriveSelectionChange);
			
			this.Controls.Add(filetree);
			this.Controls.Add(splitter1);
			this.Controls.Add(filelister);
			this.Controls.Add(driveselector);
			DriveSelectionChange(null, null);
		}
		
		void DriveSelectionChange(object sender, EventArgs e)
		{		
			filetree.InitializeShellTree(driveselector.GetDrive());
			
			filelister.Items.Clear(); // clear all file entries
		}
		
		void DirectorySelected(object sender, TreeViewEventArgs e)
		{
			filelister.ShowFilesInPath(filetree.NodePath + Path.DirectorySeparatorChar);
		}
		
		void FileSelected(object sender, EventArgs e)
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IFileService    fileService    = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			foreach (FileList.FileListItem item in filelister.SelectedItems) {
				
				switch (Path.GetExtension(item.FullName)) {
					case ".cmbx":
					case ".prjx":
						projectService.OpenCombine(item.FullName);
						break;
					default:
						fileService.OpenFile(item.FullName);
						break;
				}
			}
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
	
	public class ShellTree : TreeView
	{
		public string NodePath {
			get {
				return SelectedNode.FullPath;
			}
			set {
				PopulateShellTree(value);
			}
		}
		
		public ShellTree()
		{
			InitializeComponent();
			Sorted = true;
		}
		
		void InitializeComponent ()
		{
			BeforeSelect   += new TreeViewCancelEventHandler(SetClosedIcon);
			AfterSelect    += new TreeViewEventHandler(SetOpenedIcon);
		}
		
		void SetClosedIcon(object sender, TreeViewCancelEventArgs e) // Set icon as closed
		{
			if (SelectedNode != null && SelectedNode.Parent != null) 
				SelectedNode.ImageIndex = SelectedNode.SelectedImageIndex = 0;
		}
		
		void SetOpenedIcon(object sender, TreeViewEventArgs e) // Set icon as opened
		{
			if (e.Node.Parent != null) 
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
		}
		
		void PopulateShellTree(string path)
		{
			string[]  pathlist = path.Split(new char[] { Path.DirectorySeparatorChar });
			TreeNodeCollection  curnode = Nodes;
			
			foreach(string dir in pathlist) {
				
				foreach(TreeNode childnode in curnode) {
					if (childnode.Text.ToUpper().Equals(dir.ToUpper())) {
						SelectedNode = childnode;
						
						PopulateSubDirectory(childnode, 2);
						childnode.Expand();
						
						curnode = childnode.Nodes;
						break;
					}
				}
			}
		}
		
		void PopulateSubDirectory(TreeNode curNode, int depth)
		{
			if (--depth < 0) {
				return;
			}
			
			if (curNode.Nodes.Count == 1 && curNode.Nodes[0].Text.Equals("")) {
				
				string[] directories = null;
				try {
					directories  = Directory.GetDirectories(curNode.FullPath + Path.DirectorySeparatorChar);
				} catch (Exception) {
					return;
				}
				
				curNode.Nodes.Clear();
				
				foreach (string fulldir in directories) {
					try {
						string dir = System.IO.Path.GetFileName(fulldir);
						
						FileAttributes attr = File.GetAttributes(fulldir);
						if ((attr & FileAttributes.Hidden) == 0) {
							TreeNode node   = curNode.Nodes.Add(dir);
							node.ImageIndex = node.SelectedImageIndex = 0;
							
							node.Nodes.Add(""); // Add dummy child node to make node expandable
							
							PopulateSubDirectory(node, depth);
						}
					} catch (Exception) {
					}
				}
			} else {
				foreach (TreeNode node in curNode.Nodes) {
					PopulateSubDirectory(node, depth); // Populate sub directory
				}
			}
		}
		
		
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			
			try {
				PopulateSubDirectory(e.Node, 2);
				Cursor.Current = Cursors.Default;
				//				SetOpenedIcon(null, null); // optional
			} catch (Exception excpt) {
				MessageBox.Show(excpt.Message, "Device error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				e.Cancel = true;
			}
			
			Cursor.Current = Cursors.Default;
		}
		
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			if (e.Node.Parent != null) { // Set icon as closed
				//				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
			}
		}
		
		public void InitializeShellTree(string drive)
		{
			Nodes.Clear();
			
			TreeNode node = new TreeNode(drive.Substring(0, drive.Length - 1));
			Nodes.Add(node);
			node.Nodes.Add(""); // Add dummy child (make the node expandable)
			node.Expand();      // Expand node
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			switch(fileUtilityService.GetDriveType(drive)) {
				case DriveType.Removeable:
					node.ImageIndex = node.SelectedImageIndex = 2;
					break;
				case DriveType.Fixed:
					node.ImageIndex = node.SelectedImageIndex = 3;
					break;
				case DriveType.Cdrom:
					node.ImageIndex = node.SelectedImageIndex = 4;
					break;
				case DriveType.Remote:
					node.ImageIndex = node.SelectedImageIndex = 5;
					break;
				default:
					node.ImageIndex = node.SelectedImageIndex = 3;
					break;
			}
		}
	}
}
