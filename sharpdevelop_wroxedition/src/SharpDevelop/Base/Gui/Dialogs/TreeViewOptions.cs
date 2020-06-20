// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class GradientHeaderPanel : UserControl
	{
		Font normalFont;
		
		public GradientHeaderPanel(int fontSize)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			normalFont = resourceService.LoadFont("SansSerif", fontSize, GraphicsUnit.World);
			
			ResizeRedraw  = true;
			Text = String.Empty;
			SetStyle(ControlStyles.UserPaint, true);
		}
		
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			Refresh();
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
//    		base.OnPaintBackground(pe);
			Graphics g = pe.Graphics;
//			g.FillRectangle(new SolidBrush(SystemColors.Control), pe.ClipRectangle);
			
			g.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(Width, Height), 
			                                        SystemColors.Window,
			                                        SystemColors.Control),
							new Rectangle(0, 0, Width, Height));
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
//    		base.OnPaint(pe);
			Graphics g = pe.Graphics;
			
			g.DrawString(Text, normalFont, new SolidBrush(SystemColors.WindowText),
			             10,
			             Height - normalFont.Height - 4,
			             StringFormat.GenericTypographic);
			g.DrawLine(new Pen(Color.Black), 10, Height - 1 - normalFont.Height / 8, Width - 10, Height - 1 - normalFont.Height / 8);
		}
	}
	
	/// <summary>
	/// TreeView options are used, when more options will be edited (for something like
	/// IDE Options + Plugin Options)
	/// </summary>
	public class TreeViewOptions : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel    backgroundPanel;
		private System.Windows.Forms.Button   okButton;
		private System.Windows.Forms.Button   cancelButton;
		private System.Windows.Forms.TreeView optionsTreeView;
		private System.Windows.Forms.Splitter leftRightSplitter;
		private System.Windows.Forms.Panel    optionBackgroundPanel;
		private GradientHeaderPanel           optionsPanelLabel;
		private System.Windows.Forms.Panel    optionControlPanel;
		
		ArrayList OptionPanels = new ArrayList();
		IProperties properties = null;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		public IProperties Properties {
			get {
				return properties;
			}
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				foreach (AbstractOptionPanel pane in OptionPanels) {
					pane.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		void AcceptEvent(object sender, EventArgs e)
		{
			foreach (AbstractOptionPanel pane in OptionPanels) {
				if (!pane.ReceiveDialogMessage(DialogMessage.OK)) {
					return;
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		void ResetImageIndex(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes) {
				if (node.Nodes.Count > 0) {
					ResetImageIndex(node.Nodes);
				} else {
					node.ImageIndex         = 2;
					node.SelectedImageIndex = 3;
				}
			}
		}
		
		bool b = true;
		
		void BeforeExpandNode(object sender, TreeViewCancelEventArgs e)
		{
			if (!b) {
				return;
			}
			b = false;
			optionsTreeView.BeginUpdate();
			// search first leaf node (leaf nodes have no childs)
			TreeNode node = e.Node.FirstNode;
			while (node.Nodes.Count > 0) {
				node = node.FirstNode;
			}
			optionsTreeView.CollapseAll();
			node.EnsureVisible();
			node.ImageIndex = 3;
			optionsTreeView.EndUpdate();
			SetOptionPanelTo(node);
			b = true;
		}
		
		void BeforeSelectNode(object sender, TreeViewCancelEventArgs e)
		{
			ResetImageIndex(optionsTreeView.Nodes);
			if (e.Node.Nodes.Count > 0) { // select folder
				if (e.Node.IsExpanded) {
					e.Node.Collapse();
				}  else
					e.Node.Expand();
			} 
		}
		void HandleClick(object sender, EventArgs e)
		{
			if (optionsTreeView.GetNodeAt(optionsTreeView.PointToClient(Control.MousePosition)) ==  optionsTreeView.SelectedNode) {
				if (optionsTreeView.SelectedNode.IsExpanded) {
					optionsTreeView.SelectedNode.Collapse();
				} else {
					optionsTreeView.SelectedNode.Expand();
				}
			}
		}
		
		void SetOptionPanelTo(TreeNode node)
		{
			IDialogPanelDescriptor descriptor = (IDialogPanelDescriptor)node.Tag ;
			if (descriptor.DialogPanel != null) {
				optionControlPanel.Controls.Clear();
				optionControlPanel.Controls.Add(descriptor.DialogPanel.Control);
				optionsPanelLabel.Text = descriptor.Label;
			}
		}
		
		void AddNodes(TreeNodeCollection nodes, ArrayList dialogPanelDescriptors)
		{
			nodes.Clear();
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				
				if (descriptor.DialogPanel != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.CustomizationObject = properties;
					descriptor.DialogPanel.Control.Dock = DockStyle.Fill;
					OptionPanels.Add(descriptor.DialogPanel);
				}
				
				TreeNode newNode = new TreeNode(descriptor.Label);
				newNode.Tag = descriptor;
				nodes.Add(newNode);
				if (descriptor.DialogPanelDescriptors != null) {
					AddNodes(newNode.Nodes, descriptor.DialogPanelDescriptors);
				}
			}
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			SetOptionPanelTo(optionsTreeView.SelectedNode);
		}
		
		void InitImageList()
		{
			ImageList imglist = new ImageList();
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(new Bitmap(1, 1));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.SelectionArrow"));
			
			optionsTreeView.ImageList = imglist;
		}
		
		
		void ShowOpenFolderIcon(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Nodes.Count > 0) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
			}
		}
		
		void ShowClosedFolderIcon(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Nodes.Count > 0) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
			}
		}
		
		public TreeViewOptions(IProperties properties, IAddInTreeNode node) 
		{
			this.properties = properties;
			
			this.Text = resourceService.GetString("Dialog.Options.TreeViewOptions.DialogName");
			
			this.InitializeComponent();
			
			okButton.Click += new EventHandler(AcceptEvent);
			
			this.FormBorderStyle = FormBorderStyle.Sizable;
			MaximizeBox  = MinimizeBox = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Icon = null;
			InitImageList();
			
			optionsTreeView.BeforeExpand   += new TreeViewCancelEventHandler(ShowOpenFolderIcon);
			optionsTreeView.BeforeCollapse += new TreeViewCancelEventHandler(ShowClosedFolderIcon);
			
			AddNodes(optionsTreeView.Nodes, node.BuildChildItems(this));
		}
		
		private void InitializeComponent() 
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			this.backgroundPanel = new System.Windows.Forms.Panel();
			this.leftRightSplitter = new System.Windows.Forms.Splitter();
			this.optionsTreeView = new TreeView();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.optionBackgroundPanel = new System.Windows.Forms.Panel();
			this.optionsPanelLabel = new GradientHeaderPanel(23);
			this.optionControlPanel = new System.Windows.Forms.Panel();
			this.backgroundPanel.SuspendLayout();
			this.optionBackgroundPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// Win32Form1
			// 
			this.AcceptButton = okButton;
			this.CancelButton = cancelButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(600, 493);
						
			// 
			// backgroundPanel
			// 
			this.backgroundPanel.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | 
			                                System.Windows.Forms.AnchorStyles.Left)| 
			                                System.Windows.Forms.AnchorStyles.Right);
			this.backgroundPanel.Controls.AddRange(new System.Windows.Forms.Control[] {this.optionBackgroundPanel,
//                        this.leftRightSplitter,
						this.optionsTreeView});
			this.backgroundPanel.Size = new System.Drawing.Size(592, 456 + 4);
			this.backgroundPanel.TabIndex = 0;
			
			// 
			// leftRightSplitter
			// 
			this.leftRightSplitter.Location = new System.Drawing.Point(121, 0);
			this.leftRightSplitter.Size = new System.Drawing.Size(0, 456);
			this.leftRightSplitter.TabIndex = 1;
			this.leftRightSplitter.TabStop = false;
			this.leftRightSplitter.BorderStyle = BorderStyle.FixedSingle;
			
			// 
			// optionsTreeView
			// 
			this.optionsTreeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.optionsTreeView.ImageIndex = -1;
			this.optionsTreeView.SelectedImageIndex = -1;
			this.optionsTreeView.Size = new System.Drawing.Size(150, 470);
			this.optionsTreeView.TabIndex = 0;
			this.optionsTreeView.HideSelection = false;
			
			this.optionsTreeView.Click        += new EventHandler(HandleClick);
			this.optionsTreeView.AfterSelect  += new TreeViewEventHandler(SelectNode);
			this.optionsTreeView.BeforeSelect += new TreeViewCancelEventHandler(BeforeSelectNode);
			this.optionsTreeView.BeforeExpand += new TreeViewCancelEventHandler(BeforeExpandNode);
			this.optionsTreeView.ShowLines = this.optionsTreeView.ShowPlusMinus = false;
//            optionsTreeView.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			optionsTreeView.BorderStyle  =  BorderStyle.None;
			
			
			//
			// okButton
			// 
			this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.okButton.Location = new System.Drawing.Point(428 + 8, 464);
			this.okButton.Size = new System.Drawing.Size(74, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = resourceService.GetString("Global.OKButtonText");
			okButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.cancelButton.Location = new System.Drawing.Point(428 + 88, 464);
			this.cancelButton.Size = new System.Drawing.Size(74, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Text = resourceService.GetString("Global.CancelButtonText");
			cancelButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// optionBackgroundPanel
			// 
			this.optionBackgroundPanel.Controls.AddRange(new System.Windows.Forms.Control[] {this.optionControlPanel, this.optionsPanelLabel});
			this.optionBackgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.optionBackgroundPanel.Location = new System.Drawing.Point(126, 0);
			this.optionBackgroundPanel.Size = new System.Drawing.Size(466, 456);
			this.optionBackgroundPanel.TabIndex = 2;
			// 
			// optionsPanelLabel
			// 
			this.optionsPanelLabel.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right);
			this.optionsPanelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.optionsPanelLabel.Location = new System.Drawing.Point(0, 0);
			this.optionsPanelLabel.Size = new System.Drawing.Size(460 + 8, 24 + 8);
			this.optionsPanelLabel.TabIndex = 2;
			
			// 
			// optionControlPanel
			// 
			this.optionControlPanel.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right);
//            this.optionControlPanel.BorderStyle = flat ? BorderStyle.FixedSingle : System.Windows.Forms.BorderStyle.Fixed3D;
			this.optionControlPanel.BackColor = System.Drawing.SystemColors.Control;
			this.optionControlPanel.Location = new System.Drawing.Point(0, 32);
			this.optionControlPanel.Size = new System.Drawing.Size(464 + 8, 424);
			this.optionControlPanel.TabIndex = 4;
			StartPosition  = FormStartPosition.CenterScreen;
			
			Label label1 = new Label();
			label1.Size        = new Size(Width - 4, flat ? 1 : 2);
			label1.BorderStyle = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			label1.Location    = new Point(0, cancelButton.Top - 4); 
			label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.cancelButton, 
			                       this.okButton, 
			                       this.backgroundPanel,
			                       label1});
			this.backgroundPanel.ResumeLayout(false);
			this.optionBackgroundPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.ClientSize = new System.Drawing.Size(560, 400);
		}
	}
}
