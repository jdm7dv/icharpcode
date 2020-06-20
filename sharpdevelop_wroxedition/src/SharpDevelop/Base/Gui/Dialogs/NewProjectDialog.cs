// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	/// This class displays a new project dialog and sets up and creates a a new project,
	/// the project types are described in an XML options file
	/// </summary>
	public class NewProjectDialog : Form
	{
		Container components = new System.ComponentModel.Container();
		
		TextBox  solutionnametextbox = new TextBox();
		TextBox  nametextbox         = new TextBox();
		ComboBox locationcombobox    = new ComboBox();
		
		Label projectTypeLabel = new Label();
		Label templateLabel = new Label();
		Label nameLabel = new Label();
		Label locationLabel = new Label();
		Label newSolutionLabel = new Label();
		Label separatorLabel = new Label();
		
		Label descriptionlabel = new Label();
		Label createdinlabel   = new Label();
		
		Button browsebutton = new Button();
		Button helpbutton   = new Button();
		Button cancelbutton = new Button();
		Button okbutton     = new Button();
		
		CheckBox checkBox1 = new CheckBox();
		CheckBox checkBox2 = new CheckBox();
		
		RadioButton smalliconbutton = new RadioButton();
		RadioButton largeiconbutton = new RadioButton();
		
		ListView templateview    = new ListView();
		TreeView projecttypetree = new TreeView();
		
		ArrayList alltemplates = new ArrayList();
		ArrayList categories   = new ArrayList();
		Hashtable icons        = new Hashtable();
		
		ToolTip    tooltip;
		
		public string NewProjectLocation;
		public string NewCombineLocation;
		
		ResourceService     resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		FileUtilityService  fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		PropertyService     propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public NewProjectDialog()
		{
			InitializeComponent();
			
			InitializeTemplates();
			
			InitializeView();
			
			projecttypetree.Select();
			
			locationcombobox.Text = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", fileUtilityService.GetDirectoryNameWithSeparator(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)) + "SharpDevelop Projects").ToString();
			
			StartPosition = FormStartPosition.CenterParent;
			Icon = null;
		}
		
		void InitializeView()
		{
			ImageList smalllist = new ImageList();
			ImageList imglist = new ImageList();
			
			imglist.ImageSize    = new Size(32, 32);
			smalllist.ImageSize  = new Size(16, 16);
			
			smalllist.Images.Add(resourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			// load the icons and set their index from the image list in the hashtable
			int i = 0;
			Hashtable tmp = new Hashtable(icons);
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
			
			// set the correct imageindex for all templates
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
			if (categories.Count > 0)
				projecttypetree.SelectedNode = (TreeNode)projecttypetree.Nodes[0];
		}
		
		void InsertCategories(TreeNode node, ArrayList catarray)
		{
			foreach (Category cat in catarray) {
				if (node == null) {
					projecttypetree.Nodes.Add(cat);
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
			foreach (ProjectTemplate template in ProjectTemplate.ProjectTemplates) {
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null)
					icons[titem.Template.Icon] = 0; // "create template icon"
				Category cat = GetCategory(titem.Template.Category);
				cat.Templates.Add(titem);
				if (cat.Templates.Count == 1)
					titem.Selected = true;
				alltemplates.Add(titem);
			}
		}
		
		void CategoryChange(object sender, TreeViewEventArgs e)
		{
			templateview.Items.Clear();
			if (projecttypetree.SelectedNode != null) {
				foreach (TemplateItem item in ((Category)projecttypetree.SelectedNode).Templates) {
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
		
		void CheckedChange(object sender, EventArgs e)
		{
			solutionnametextbox.ReadOnly = !checkBox1.Checked;
			
			if (solutionnametextbox.ReadOnly) { // unchecked created own directory for solution
				NameTextChanged(null, null);    // set the value of the solutionnametextbox to nametextbox
			}
		}
		
		void NameTextChanged(object sender, EventArgs e)
		{
			if (!checkBox1.Checked)
				solutionnametextbox.Text = nametextbox.Text;
		}
		
		string ProjectSolution {
			get {
				string name = "";
				if (checkBox1.Checked) {
					name += Path.DirectorySeparatorChar + solutionnametextbox.Text;
				}
				return ProjectLocation + name;
			}
		}
		
		string ProjectLocation {
			get {
				string location = locationcombobox.Text.TrimEnd('\\', '/', Path.DirectorySeparatorChar);
				string name     = nametextbox.Text;
				return location + (checkBox2.Checked ? Path.DirectorySeparatorChar + name : "");
			}
		}
		
		// TODO : Format the text
		void PathChanged(object sender, EventArgs e)
		{
			createdinlabel.Text = resourceService.GetString("Dialog.NewProject.ProjectAtDescription")+ " " + ProjectSolution;
		}
		
		void IconSizeChange(object sender, EventArgs e)
		{
			templateview.View = smalliconbutton.Checked ? View.List : View.LargeIcon;
		}
		
		Combine cmb = new Combine();
		public Combine Combine {
			get {
				return cmb;
			}
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			return true;
		}
		
		public void SaveFile(IProject project, string filename, string content, bool showFile)
		{
			project.ProjectFiles.Add(new ProjectFile(filename));
			
			StreamWriter sr = File.CreateText(filename);
			sr.Write(stringParserService.Parse(content, new string[,] { {"PROJECT", nametextbox.Text}, {"FILE", Path.GetFileName(filename)}}));
			sr.Close();
			
			if (showFile) {
				string longfilename = fileUtilityService.GetDirectoryNameWithSeparator(ProjectSolution) + stringParserService.Parse(filename, new string[,] { {"PROJECT", nametextbox.Text}});
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile(longfilename);
			}
		}
		
		void OpenEvent(object sender, EventArgs e)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			if (!fileUtilityService.IsValidFileName(solutionnametextbox.Text) ||
			    !fileUtilityService.IsValidFileName(nametextbox.Text)||
			    !fileUtilityService.IsValidFileName(locationcombobox.Text)) {
				MessageBox.Show("Illegal project name.\nOnly use letter, digits, space, '.' or '_'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			propertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", locationcombobox.Text);
			propertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.AutoCreateProjectSubdir", checkBox2.Checked);
			if (templateview.SelectedItems.Count == 1) {
				if (!locationcombobox.Text.Equals("") && !solutionnametextbox.Text.Equals("")) {
					
					TemplateItem item = (TemplateItem)templateview.SelectedItems[0];
					
					System.IO.Directory.CreateDirectory(ProjectSolution);
					
					
					cmb.Name = nametextbox.Text;
					
					if (!(item.Template.LanguageName == null || item.Template.LanguageName.Length == 0))  {
						LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
				
						ILanguageBinding languageinfo = languageBindingService.GetBindingPerLanguageName(item.Template.LanguageName);
						
						if (languageinfo == null) {
							MessageBox.Show("Can't create project with type :" + item.Template.LanguageName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}
						
						ProjectCreateInformation cinfo = new ProjectCreateInformation();
						
						cinfo.Solution        = ProjectSolution;
						cinfo.Location        = ProjectLocation;
						cinfo.Description     = stringParserService.Parse(item.Template.Description);
						cinfo.Name            = nametextbox.Text;
						cinfo.ProjectTemplate = item.Template;
						
						IProject project = languageinfo.CreateProject(cinfo);
						
						// insert references which are defined in the project template to the new project
						if (item.Template.ProjectOptions["References"] != null) {
							XmlNodeList nodes = item.Template.ProjectOptions["References"].ChildNodes;
							foreach (XmlElement node in nodes) {
								ProjectReference refinfo = new ProjectReference();
								
								refinfo.ReferenceType = (ReferenceType)Enum.Parse(typeof(ReferenceType), node.Attributes["type"].InnerXml);
								refinfo.Reference     = node.Attributes["refto"].InnerXml;
								
								project.ProjectReferences.Add(refinfo);
							}
						}
						
						foreach (FileDescriptionTemplate file in item.Template.ProjectFiles) {
							string filename = fileUtilityService.GetDirectoryNameWithSeparator(ProjectSolution) + stringParserService.Parse(file.Name, new string[,] { {"PROJECT", nametextbox.Text}});
							
							project.ProjectFiles.Add(new ProjectFile(filename));
							
							if (File.Exists(filename)) {
								DialogResult result = MessageBox.Show("File " + filename + " already exists, do you want to overwrite\nthe existing file ?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
								switch(result) {
									case DialogResult.Yes:
										break;
									case DialogResult.No:
										continue;
								}
							}
							try {
								StreamWriter sr = File.CreateText(filename);
								sr.Write(stringParserService.Parse(file.Content, new string[,] { {"PROJECT", nametextbox.Text}, {"FILE", filename}}));
								sr.Close();
							} catch (Exception ex) {
								MessageBox.Show("File " + filename + " could not be written. \nReason :" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
							}
						}
						
						NewProjectLocation = fileUtilityService.GetDirectoryNameWithSeparator(ProjectSolution) + nametextbox.Text + ".prjx";
						if (File.Exists(NewProjectLocation)) {
							DialogResult result = MessageBox.Show("Project file " + NewProjectLocation + " already exists, do you want to overwrite\nthe existing file ?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
							switch(result) {
								case DialogResult.Yes:
									project.SaveProject(NewProjectLocation);
									break;
								case DialogResult.No:
									break;
							}
						} else {
							project.SaveProject(NewProjectLocation);
						}
												
						cmb.AddEntry(fileUtilityService.GetDirectoryNameWithSeparator(ProjectSolution) + nametextbox.Text + ".prjx");
						
						foreach (FileDescriptionTemplate file in item.Template.OpenFiles) {
							string longfilename = fileUtilityService.GetDirectoryNameWithSeparator(ProjectSolution) + stringParserService.Parse(file.Name, new string[,] { {"PROJECT", nametextbox.Text}});
							IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
							fileService.OpenFile(longfilename);
						}
						
					}
					
					if (item.Template.WizardPath != null) {
						IProperties customizer = new DefaultProperties();
						customizer.SetProperty("Template", item.Template);
						customizer.SetProperty("Creator",  this);
						WizardDialog wizard = new WizardDialog("Project Wizard", customizer, item.Template.WizardPath);
						if (wizard.ShowDialog() == DialogResult.OK) {
							DialogResult = DialogResult.OK;
						}
					}
					
					NewCombineLocation = fileUtilityService.GetDirectoryNameWithSeparator(ProjectLocation) + nametextbox.Text + ".cmbx";
					
					if (File.Exists(NewCombineLocation)) {
						DialogResult result = MessageBox.Show("Combine file " + NewCombineLocation + " already exists, do you want to overwrite\nthe existing file ?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
						switch(result) {
							case DialogResult.Yes:
								cmb.SaveCombine(NewCombineLocation);
								break;
							case DialogResult.No:
								break;
						}
					} else {
						cmb.SaveCombine(NewCombineLocation);
					}
					
					DialogResult = DialogResult.OK;
				} else {
					MessageBox.Show(resourceService.GetString("Dialog.NewProject.EmptyProjectFieldWarning"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}
		
		void BrowseDirectories(object sender, EventArgs e)
		{
			// Changes Shankar
			FolderDialog fd = new FolderDialog();
			if(fd.DisplayDialog() == DialogResult.OK)
				locationcombobox.Text = fd.Path;
			// End
		}
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (templateview.SelectedItems.Count == 1) {
				descriptionlabel.Text = stringParserService.Parse(((TemplateItem)templateview.SelectedItems[0]).Template.Description);
				okbutton.Enabled = true;
			} else {
				descriptionlabel.Text = "";
				okbutton.Enabled = false;
			}
		}
		
		private void InitializeComponent()
		{
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			tooltip = new ToolTip(components);
			tooltip.SetToolTip(largeiconbutton, resourceService.GetString("Global.LargeIconToolTip"));
			tooltip.SetToolTip(smalliconbutton, resourceService.GetString("Global.SmallIconToolTip"));
			tooltip.Active = true;
			
			solutionnametextbox.Location = new Point(112, 320);
			solutionnametextbox.TabIndex = 16;
			solutionnametextbox.Size     = new Size(192, 20);
			solutionnametextbox.ReadOnly = true;
			solutionnametextbox.TextChanged += new EventHandler(PathChanged);
			solutionnametextbox.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			nametextbox.Location    = new Point(112, 272);
			nametextbox.TabIndex    = 7;
			nametextbox.Size        = new Size(392, 20);
			nametextbox.TextChanged += new EventHandler(NameTextChanged);
			nametextbox.TextChanged += new EventHandler(PathChanged);
			nametextbox.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			locationcombobox.Location = new Point(112, 296);
			locationcombobox.Size = new Size(312 + 72 - 32, 21);
			locationcombobox.TabIndex = 8;
			locationcombobox.TextChanged += new EventHandler(PathChanged);
			
			projectTypeLabel.Location = new Point(8, 12);
			projectTypeLabel.Text = resourceService.GetString("Dialog.NewProject.ProjectTypeLabelText");
			projectTypeLabel.Size = new Size(136, 16);
			projectTypeLabel.TabIndex = 0;
			
			templateLabel.Location = new Point(224, 12);
			templateLabel.Text = resourceService.GetString("Dialog.NewProject.TemplateLabelText");
			templateLabel.Size = new Size(168, 16);
			templateLabel.TabIndex = 1;
			
			nameLabel.Location = new Point(4, 272);
			nameLabel.Text = resourceService.GetString("Dialog.NewProject.NameLabelText");
			nameLabel.Size = new Size(108, 24);
			nameLabel.TabIndex = 4;
			
			locationLabel.Location = new Point(4, 296);
			locationLabel.Text = resourceService.GetString("Dialog.NewProject.LocationLabelText");
			locationLabel.Size = new Size(108, 24);
			locationLabel.TabIndex = 8;
			
			newSolutionLabel.Location = new Point(4, 320);
			newSolutionLabel.Text = resourceService.GetString("Dialog.NewProject.NewSolutionLabelText");
			newSolutionLabel.Size = new Size(108, 24);
			newSolutionLabel.TabIndex = 15;
			
			separatorLabel.Location = new Point(8, 370 + 20);
			separatorLabel.Size = new Size(496, flat ? 1 : 2);
			separatorLabel.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			separatorLabel.TabIndex = 18;
			
			const int MODIFICATOR = 10;
			createdinlabel.Location = new Point(8, 372 - MODIFICATOR);
			createdinlabel.Size = new Size(496, 16 + MODIFICATOR);
			createdinlabel.TabIndex = 19;
			
			descriptionlabel.Location = new Point(8, 248);
			descriptionlabel.Size = new Size(496, 16);
			descriptionlabel.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			descriptionlabel.TabIndex = 6;
			
			largeiconbutton.Location   = new Point(460,  6);
			largeiconbutton.Size       = new Size(22, 22);
			largeiconbutton.Appearance = Appearance.Button;
			largeiconbutton.TabIndex   = 2;
			largeiconbutton.Checked    = true;
			largeiconbutton.CheckedChanged += new EventHandler(IconSizeChange);
			largeiconbutton.Image      = resourceService.GetBitmap("Icons.16x16.LargeIconsIcon");
			largeiconbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			smalliconbutton.Location   = new Point(482, 6);
			smalliconbutton.Size       = new Size(22, 22);
			smalliconbutton.Appearance = Appearance.Button;
			smalliconbutton.TabIndex   = 3;
			smalliconbutton.CheckedChanged += new EventHandler(IconSizeChange);
			smalliconbutton.Image      = resourceService.GetBitmap("Icons.16x16.SmallIconsIcon");
			smalliconbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			okbutton.Location    = new Point(264, 376 + 20);
			okbutton.Size         = new Size(72, 24);
			okbutton.TabIndex     = 9;
			okbutton.Text         = resourceService.GetString("Global.OKButtonText");
			okbutton.Click += new EventHandler(OpenEvent);
			okbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			cancelbutton.Location = new Point(344, 376 + 20);
			cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelbutton.Size = new Size(72, 24);
			cancelbutton.TabIndex = 10;
			cancelbutton.Text = resourceService.GetString("Global.CancelButtonText");
			cancelbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			helpbutton.Location = new Point(424, 376 + 20);
			helpbutton.Size = new Size(72, 24);
			helpbutton.TabIndex = 11;
			helpbutton.Text = resourceService.GetString("Global.HelpButtonText");
			helpbutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			browsebutton.Location = new Point(432+ 72 - 32, 296);
			browsebutton.Size = new Size(32, 24);
			browsebutton.TabIndex = 12;
			browsebutton.Text = "..."; // resourceService.GetString("Global.BrowseButtonText");
			browsebutton.Click += new EventHandler(BrowseDirectories);
			browsebutton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			checkBox1.Location = new Point(312, 320);
			checkBox1.Text = resourceService.GetString("Dialog.NewProject.checkBox1Text");
			checkBox1.Size = new Size(200, 24);
			checkBox1.TabIndex = 17;
			checkBox1.CheckedChanged += new EventHandler(CheckedChange);
			checkBox1.CheckedChanged += new EventHandler(PathChanged);
			checkBox1.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			checkBox2.Location = new Point(312, 340);
			checkBox2.Text = resourceService.GetString("Dialog.NewProject.autoCreateSubDirCheckBox");
			checkBox2.Size = new Size(300, 24);
			checkBox2.TabIndex = 18;
			checkBox2.Checked = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.AutoCreateProjectSubdir", true);
			checkBox2.CheckedChanged += new EventHandler(PathChanged);
			checkBox2.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			templateview.Location = new Point(224, 28);
			templateview.Size = new Size(280, 220);
			templateview.HideSelection  = false;
			templateview.MultiSelect  = false;
			templateview.ForeColor = System.Drawing.SystemColors.WindowText;
			templateview.TabIndex = 1;
			templateview.DoubleClick += new EventHandler(OpenEvent);
			templateview.SelectedIndexChanged += new EventHandler(SelectedIndexChange);
			
			projecttypetree.Location = new Point(8, 28);
			projecttypetree.Size     = new Size(215, 220);
			projecttypetree.HideSelection  = false;
			projecttypetree.TabIndex = 0;
			projecttypetree.Sorted = true;
			projecttypetree.AfterSelect    += new TreeViewEventHandler(CategoryChange);
			projecttypetree.BeforeSelect   += new TreeViewCancelEventHandler(OnBeforeExpand);
			projecttypetree.BeforeExpand   += new TreeViewCancelEventHandler(OnBeforeExpand);
			projecttypetree.BeforeCollapse += new TreeViewCancelEventHandler(OnBeforeCollapse);
			
			ImageList imglist = new ImageList();
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			
			projecttypetree.ImageList = imglist;
			
			this.Text = resourceService.GetString("Dialog.NewProject.DialogName");
			this.MaximizeBox = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.AutoScaleBaseSize = new Size(5, 13);
			this.CancelButton = cancelbutton;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.ShowInTaskbar = false;
			this.AcceptButton = okbutton;
			
			this.MinimizeBox = false;
			this.ClientSize = new Size(514, 407 + 20);
			
			this.Controls.Add(createdinlabel);
			this.Controls.Add(separatorLabel);
			this.Controls.Add(checkBox1);
			this.Controls.Add(checkBox2);
			this.Controls.Add(solutionnametextbox);
			this.Controls.Add(newSolutionLabel);
			this.Controls.Add(locationLabel);
			this.Controls.Add(nameLabel);
			this.Controls.Add(browsebutton);
			this.Controls.Add(helpbutton);
			this.Controls.Add(cancelbutton);
			this.Controls.Add(okbutton);
			this.Controls.Add(locationcombobox);
			this.Controls.Add(nametextbox);
			this.Controls.Add(descriptionlabel);
			this.Controls.Add(templateLabel);
			this.Controls.Add(projectTypeLabel);
			this.Controls.Add(smalliconbutton);
			this.Controls.Add(largeiconbutton);
			this.Controls.Add(templateview);
			this.Controls.Add(projecttypetree);
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
		
		/// <summary>
		///  Represents a category
		/// </summary>
		internal class Category : TreeNode
		{
			ArrayList categories = new ArrayList();
			ArrayList templates  = new ArrayList();
			string name;
			
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
		/// Holds a new file template
		/// </summary>
		internal class TemplateItem : ListViewItem
		{
			ProjectTemplate template;
			
			public TemplateItem(ProjectTemplate template) : base(((StringParserService)ServiceManager.Services.GetService(typeof(StringParserService))).Parse(template.Name))
			{
				this.template = template;
				ImageIndex = 0;
			}
			
			public ProjectTemplate Template {
				get {
					return template;
				}
			}
		}
	}
}
