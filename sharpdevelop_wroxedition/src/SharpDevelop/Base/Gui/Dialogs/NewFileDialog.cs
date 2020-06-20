// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	///  This class is for creating a new "empty" file
	/// </summary>
	public class NewFileDialog : Form, INewFileCreator
	{
		Container components = new Container();
		
		ToolTip    tooltip;
		
		RadioButton smalliconbutton = new RadioButton();
		RadioButton largeiconbutton = new RadioButton();
		
		Label label1            =  new Label();
		Label label2            =  new Label();
		Label label3            =  new Label();
		Label descriptionlabel  =  new Label();
		
		TreeView categorytree   = new TreeView();
		ListView templateview   = new ListView();
		
		Button helpbutton   = new Button();
		Button cancelbutton = new Button();
		Button openbutton   = new Button();
		
		ArrayList alltemplates = new ArrayList();
		ArrayList categories   = new ArrayList();
		Hashtable icons        = new Hashtable();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		public NewFileDialog()
		{
			try {
				InitializeComponent();
				InitializeTemplates();
				
				InitializeView();
				categorytree.Select();
				
				StartPosition = FormStartPosition.CenterParent;
				Icon = null;
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}
		
		void InitializeView()
		{
			ImageList smalllist = new ImageList();
			ImageList imglist = new ImageList();
			
			imglist.ImageSize    = new Size(32, 32);
			smalllist.ImageSize  = new Size(16, 16);
			
			smalllist.Images.Add(resourceService.GetBitmap("Icons.32x32.EmptyFileIcon"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.32x32.EmptyFileIcon"));
			
			int i = 0;
			Hashtable tmp = new Hashtable(icons);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (DictionaryEntry entry in icons) {
				Bitmap bitmap = fileUtilityService.GetBitmap(entry.Key.ToString());
				if (bitmap != null) {
					smalllist.Images.Add(bitmap);
					imglist.Images.Add(bitmap);
					tmp[entry.Key] = ++i;
				} else {
					Console.WriteLine("can't load bitmap " + entry.Key.ToString() + " using default");
				}
			}
			
			icons = tmp;
			foreach (TemplateItem item in alltemplates) {
				if (item.Template.Icon == null) {
					item.ImageIndex = 0;
				} else {
					item.ImageIndex = (int)icons[item.Template.Icon];
				}
			}
			
			templateview.LargeImageList = imglist;
			templateview.SmallImageList = smalllist;
			
			InsertCategories(null, categories);
			if (categories.Count > 0) {
				categorytree.SelectedNode = (TreeNode)categorytree.Nodes[0];
			}
			
		}
		
		void InsertCategories(TreeNode node, ArrayList catarray)
		{
			foreach (Category cat in catarray) {
				if (node == null) {
					categorytree.Nodes.Add(cat);
				} else {
					node.Nodes.Add(cat);
				}
				InsertCategories(cat, cat.Categories);
			}
		}
		
		// TODO : insert sub categories
		Category GetCategory(string categoryname)
		{
			foreach (Category category in categories) {
				if (category.Text == categoryname)
					return category;
			}
			Category newcategory = new Category(categoryname);
			categories.Add(newcategory);
			return newcategory;
		}
		
		void InitializeTemplates()
		{
			foreach (FileTemplate template in FileTemplate.FileTemplates) {
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null) {
					icons[titem.Template.Icon] = 0; // "create template icon"
				}
				Category cat = GetCategory(titem.Template.Category);
				cat.Templates.Add(titem); 
				if (cat.Selected == false && template.WizardPath == null) {
					cat.Selected = true;
					titem.Selected = true;
				}
				alltemplates.Add(titem);
			}
		}
		
		// tree view event handlers
		void CategoryChange(object sender, TreeViewEventArgs e)
		{
			templateview.Items.Clear();
			if (categorytree.SelectedNode != null) {
				foreach (TemplateItem item in ((Category)categorytree.SelectedNode).Templates) {
					templateview.Items.Add(item);
				}
			}
		}
		
		void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 1;
		}
		
		void OnBeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 0;
		}
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (templateview.SelectedItems.Count == 1) {
				descriptionlabel.Text = stringParserService.Parse(((TemplateItem)templateview.SelectedItems[0]).Template.Description);
				openbutton.Enabled = true;
			} else {
				descriptionlabel.Text = "";
				openbutton.Enabled = false;
			}
		}
		
		// button events
		
		void CheckedChange(object sender, EventArgs e)
		{
			templateview.View = smalliconbutton.Checked ? View.List : View.LargeIcon;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			return true;
		}
		
		public void SaveFile(string filename, string content, string languageName, bool showFile)
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			
			fileService.NewFile(filename, languageName, content);
			DialogResult = DialogResult.OK;
		}
		
		void OpenEvent(object sender, EventArgs e)
		{
			if (templateview.SelectedItems.Count == 1) {
				TemplateItem item = (TemplateItem)templateview.SelectedItems[0];
				
				if (item.Template.WizardPath != null) {
					IProperties customizer = new DefaultProperties();
					customizer.SetProperty("Template", item.Template);
					customizer.SetProperty("Creator",  this);
					WizardDialog wizard = new WizardDialog("File Wizard", customizer, item.Template.WizardPath);
					if (wizard.ShowDialog() == DialogResult.OK) {
						DialogResult = DialogResult.OK;
					}
				} else {
					foreach (FileDescriptionTemplate newfile in item.Template.Files) {
						SaveFile(newfile.Name, newfile.Content, item.Template.LanguageName, true);
					}
					DialogResult = DialogResult.OK;
				}
			}
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		internal class Category : TreeNode
		{
			ArrayList categories = new ArrayList();
			ArrayList templates  = new ArrayList();
			string name;
			public bool Selected = false;
			public Category(string name) : base(name)
			{
				this.name = name;
				ImageIndex = 1;
			}
			
			public string Name {
				get {
					return name;
				}
			}
			public ArrayList Categories {
				get {
					return categories;
				}
			}
			public ArrayList Templates {
				get {
					return templates;
				}
			}
		}
		
		/// <summary>
		///  Represents a new file template
		/// </summary>
		class TemplateItem : ListViewItem
		{
			FileTemplate template;
			
			public TemplateItem(FileTemplate template) : base(((StringParserService)ServiceManager.Services.GetService(typeof(StringParserService))).Parse(template.Name))
			{
				this.template = template;
				ImageIndex    = 0;
			}
			
			public FileTemplate Template {
				get {
					return template;
				}
			}
		}
		
		void InitializeComponent()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			tooltip = new ToolTip(components);
			tooltip.SetToolTip(largeiconbutton, resourceService.GetString("Global.LargeIconToolTip"));
			tooltip.SetToolTip(smalliconbutton, resourceService.GetString("Global.SmallIconToolTip"));
			tooltip.Active = true;
			
			descriptionlabel.Location    = new Point(8, 262);
			descriptionlabel.Text        = "";
			descriptionlabel.Size        = new Size(496, 18);
			descriptionlabel.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			descriptionlabel.TabIndex    = 9;
			
			label1.Location = new Point(8, 12);
			label1.Text     = resourceService.GetString("Dialog.NewFile.CategoryText");
			label1.Size     = new Size(152, 16);
			label1.TabIndex = 3;
			
			label2.Location = new Point(224, 12);
			label2.Text     = resourceService.GetString("Dialog.NewFile.TemplateText");
			label2.Size     = new Size(96, 16);
			label2.TabIndex = 5;
			
			// this is my "boguslabel" (for the single line in the dialog)
			label3.Location    = new Point(8, 290);
			label3.Size        = new Size(496, flat ? 1 :2);
			label3.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			label3.TabIndex    = 10;
			
			openbutton.Location     = new Point(253, 300);
			openbutton.Size         = new Size(75, 23);
			openbutton.TabIndex     = 0;
			openbutton.Text         = resourceService.GetString("Global.CreateButtonText");
			openbutton.Click       += new EventHandler(OpenEvent);
			openbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			cancelbutton.Location     = new Point(341, 300);
			cancelbutton.DialogResult = DialogResult.Cancel;
			cancelbutton.Size         = new Size(75, 23);
			cancelbutton.TabIndex     = 1;
			cancelbutton.Text         = resourceService.GetString("Global.CancelButtonText");
			cancelbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			helpbutton.Location = new Point(429, 300);
			helpbutton.Size     = new Size(75, 23);
			helpbutton.TabIndex = 2;
			helpbutton.Text     = resourceService.GetString("Global.HelpButtonText");
			helpbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			largeiconbutton.Location   = new Point(460,  6);
			largeiconbutton.Size       = new Size(22, 22);
			largeiconbutton.Appearance = Appearance.Button;
			largeiconbutton.TabIndex   = 11;
			largeiconbutton.Checked    = true;
			largeiconbutton.CheckedChanged += new EventHandler(CheckedChange);
			largeiconbutton.Image      = resourceService.GetBitmap("Icons.16x16.LargeIconsIcon");
			largeiconbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			smalliconbutton.Location   = new Point(482, 6);
			smalliconbutton.Size       = new Size(22, 22);
			smalliconbutton.Appearance = Appearance.Button;
			smalliconbutton.TabIndex   = 12;
			smalliconbutton.CheckedChanged += new EventHandler(CheckedChange);
			smalliconbutton.Image      = resourceService.GetBitmap("Icons.16x16.SmallIconsIcon");
			smalliconbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			categorytree.Location = new Point(8, 28);
			categorytree.Size     = new Size(215, 232);
			categorytree.HideSelection  = false;
			categorytree.TabIndex = 3;
			categorytree.Sorted = true;
			categorytree.AfterSelect    += new TreeViewEventHandler(CategoryChange);
			categorytree.BeforeSelect   += new TreeViewCancelEventHandler(OnBeforeExpand);
			categorytree.BeforeExpand   += new TreeViewCancelEventHandler(OnBeforeExpand);
			categorytree.BeforeCollapse += new TreeViewCancelEventHandler(OnBeforeCollapse);
			categorytree.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			ImageList imglist = new ImageList();
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			
			categorytree.ImageList = imglist;
			
			templateview.Location     = new Point(224, 28);
			templateview.Size         = new Size(280, 232);
			templateview.ForeColor    = SystemColors.WindowText;
			templateview.HideSelection  = false;
			templateview.TabIndex     = 5;
			templateview.MultiSelect  = false;
			templateview.SelectedIndexChanged += new EventHandler(SelectedIndexChange);
			templateview.DoubleClick          += new EventHandler(OpenEvent);
			templateview.Sorting = SortOrder.Ascending;
			templateview.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			this.Text              = resourceService.GetString("Dialog.NewFile.DialogName");
			this.MaximizeBox       = false;
			this.StartPosition     = FormStartPosition.CenterParent;
			this.AutoScaleBaseSize = new Size(5, 13);
			this.cancelbutton      = cancelbutton;
			this.FormBorderStyle   = FormBorderStyle.FixedDialog;
			this.ShowInTaskbar     = false;
			this.AcceptButton      = openbutton;
			this.CancelButton      = cancelbutton;
			this.MinimizeBox       = false;
			this.ClientSize        = new Size(514, 327);
			
			this.Controls.Add(label1);
			this.Controls.Add(label2);
			this.Controls.Add(label3);
			this.Controls.Add(descriptionlabel);
			
			this.Controls.Add(templateview);
			this.Controls.Add(categorytree);
			
			this.Controls.Add(smalliconbutton);
			this.Controls.Add(largeiconbutton);
			
			this.Controls.Add(openbutton);
			this.Controls.Add(cancelbutton);
			this.Controls.Add(helpbutton);
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
	}
}
