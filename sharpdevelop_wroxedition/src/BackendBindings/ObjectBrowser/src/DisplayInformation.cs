// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using SharpDevelop.Gui.Edit.Reflection;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using MagicControls = Crownwood.Magic.Controls;

namespace ObjectBrowser
{
	public class DisplayInformation : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName) == ".dll" || Path.GetExtension(fileName) == ".exe";
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return false;
		}
		
		
		public IViewContent CreateContentForFile(string fileName)
		{
			DisplayInformationWrapper wrapper = new DisplayInformationWrapper();
			wrapper.LoadFile(fileName);
			return wrapper;
		}
		
		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return null;
		}
	}
	
	public class DisplayInformationWrapper : IViewContent
	{
		string filename = "";
		public MagicControls.TabControl tctrl;
		
		Control control = null;
		ReflectionTree tree = null;
		public Control Control {
			get {
				return control;
			}
		}
		
		string untitledName = "";
		public string UntitledName {
			get {
				return untitledName;
			}
			set {
				untitledName = value;
			}
		}
		
		public string ContentName {
			get {
				return filename;
			}
			set {
				filename = value;
				OnContentNameChanged(null);
			}
		}
		
		public bool IsUntitled {
			get {
				return ContentName == null;
			}
		}
		
		public bool IsDirty {
			get {
				return false;
			}
			set {
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		public bool IsViewOnly {
			get {
				return true;
			}
		}
	
		
		IWorkbenchWindow workbenchWindow;
		
		public IWorkbenchWindow WorkbenchWindow {
			get {
				return workbenchWindow;
			}
			set {
				workbenchWindow = value;
				workbenchWindow.Title = filename;
			}
		}
		
		public void RedrawContent()
		{
		}
		
		public void Dispose()
		{
		}
		
		public void SaveFile()
		{
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName) == ".dll" || Path.GetExtension(fileName) == ".exe";
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return false;
		}
		
		
		public IViewContent CreateContentForFile(string fileName)
		{
			LoadFile(fileName);
			return this;
		}
		public void Undo()
		{
		}
		public void Redo()
		{
		}

		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return null;
		}
		
		public void LoadFile(string filename)
		{
			tree.LoadFile(filename);
			this.filename = filename;
			OnContentNameChanged(null);
		}
		
		public void SaveFile(string filename)
		{
		}
		
		public void OnContentNameChanged(EventArgs e) 
		{
			if (ContentNameChanged != null) {
				ContentNameChanged(this, e);
			}
		}
		public void OnDirtyChanged(EventArgs e) 
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, e);
			}
		}
		public event EventHandler ContentNameChanged;
		public event EventHandler DirtyChanged;
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		/// <summary>
		/// Gets the content pane for the language (you can create own views),
		/// this function can return null, in this case a sharptextarea will
		/// be viewed.
		/// </summary>
		public DisplayInformationWrapper()
		{
			Panel panel = new Panel();
			panel.Dock = DockStyle.Fill;
			
			tctrl = new MagicControls.TabControl();
			tctrl.Dock       = DockStyle.Fill;
			tctrl.Dock       = DockStyle.Left;
			tctrl.Width      = 350;
			
			MagicControls.TabPage treeviewpage = new MagicControls.TabPage("Tree");
			treeviewpage.Icon = resourceService.GetIcon("Icons.16x16.Class");
			ReflectionTree reflectiontree = new ReflectionTree();
			this.tree = reflectiontree;
			treeviewpage.Controls.Add(reflectiontree);
			tctrl.TabPages.Add(treeviewpage);
			
			MagicControls.TabPage indexviewpage = new MagicControls.TabPage("Search");
			indexviewpage.Icon = resourceService.GetIcon("Icons.16x16.FindIcon");
			ReflectionSearchPanel SearchPanel = new ReflectionSearchPanel(reflectiontree);
			SearchPanel.ParentDisplayInfo = this;
			indexviewpage.Controls.Add(SearchPanel);
			tctrl.TabPages.Add(indexviewpage);
			
			Splitter vsplitter = new Splitter();
			vsplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

			vsplitter.Location = new System.Drawing.Point(0, 200);
			vsplitter.TabIndex = 5;
			vsplitter.TabStop = false;
			vsplitter.Size = new System.Drawing.Size(3, 273);
			vsplitter.Dock = DockStyle.Left;
			
		
			Splitter hsplitter = new Splitter();
			hsplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			hsplitter.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			hsplitter.Location = new System.Drawing.Point(0, 200);
			hsplitter.TabIndex = 5;
			hsplitter.TabStop = false;
			hsplitter.Size = new System.Drawing.Size(3, 273);
			hsplitter.Dock = DockStyle.Top;
			
			MagicControls.TabControl tctrl2 = new MagicControls.TabControl();
			tctrl2.Dock       = DockStyle.Fill;
			
			
			MagicControls.TabPage ildasmviewpage = new MagicControls.TabPage("Dissassembler");
			ildasmviewpage.Icon = resourceService.GetIcon("Icons.16x16.ILDasm");
			ildasmviewpage.Controls.Add(new ReflectionILDasmView(reflectiontree));
			tctrl2.TabPages.Add(ildasmviewpage);
			
			MagicControls.TabPage sourceviewpage = new MagicControls.TabPage("Source");
			sourceviewpage.Icon = resourceService.GetIcon("Icons.16x16.TextFileIcon");
			sourceviewpage.Controls.Add(new ReflectionSourceView(reflectiontree));
			tctrl2.TabPages.Add(sourceviewpage);

			MagicControls.TabPage xmlviewpage = new MagicControls.TabPage("XML");
			xmlviewpage.Icon = resourceService.GetIcon("Icons.16x16.XMLFileIcon");
			xmlviewpage.Controls.Add(new ReflectionXmlView(reflectiontree));
			tctrl2.TabPages.Add(xmlviewpage);
			

			panel.Controls.Add(hsplitter);
			panel.Controls.Add(tctrl2);
			panel.Controls.Add(new ReflectionInfoView(reflectiontree));
			
			panel.Controls.Add(vsplitter);
			panel.Controls.Add(tctrl);
			
			this.control = panel;
		}
	}
}
