// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;


using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.AddIns.Codons;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public abstract class FormDesignerDisplayBindingBase : AbstractViewContent, IEditable, IClipboardHandler
	{
		protected DesignPanel   designPanel = null;
		protected IDesignerHost host;
		protected bool          isFormDesignerVisible;
		AxSideTab               tab = null;
		UndoRedoManager         undoRedoManager = null;
		
		AmbientProperties ambientProperties = new AmbientProperties();
		
		public bool IsFormDesignerVisible {
			get {
				return isFormDesignerVisible;
			}
		}
		
		// IEditable
		public virtual IClipboardHandler ClipboardHandler {
			get {
				return this;
			}
		}
		
		public virtual string TextContent {
			get {
				return null;
			}
			set {
			}
		}
		
		public IDesignerHost DesignerHost {
			get {
				return host;
			}
		}
		
		// AbstractViewContent members
		public override IWorkbenchWindow WorkbenchWindow {
			set {
				base.WorkbenchWindow = value;
				WorkbenchWindow.WindowSelected   += new EventHandler(SelectMe);
				WorkbenchWindow.WindowDeselected += new EventHandler(DeSelectMe);
			}
		}
		
		void SelectMe(object sender, EventArgs e)
		{
			if (!SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
				SharpDevelopSideBar.SideBar.Tabs.Add(tab);
			}
			SharpDevelopSideBar.SideBar.ActiveTab = tab;
		}
		
		void DeSelectMe(object sender, EventArgs e)
		{
			SharpDevelopSideBar.SideBar.Tabs.Remove(tab);
		}
		
		public override bool IsReadOnly {
			get {
				return false; // FIXME
			}
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
			designPanel.Dispose();
		}
		
		protected string GetDataAs(string what)
		{
			XmlElement el = new XmlFormGenerator().GetElementFor(new XmlDocument(), host);
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<" + el.Name + " version=\"1.0\"/>");
			
			foreach (XmlNode node in el.ChildNodes) {
				doc.DocumentElement.AppendChild(doc.ImportNode(node, true));
			}
			
			StringWriter writer = new StringWriter();
			switch (what) {
				case "XML":
					doc.Save(writer);
					break;
				case "C#":
					new CodeDOMGenerator(host, new Microsoft.CSharp.CSharpCodeProvider()).ConvertContentDefinition(doc, writer);
					break;
				case "VB.NET":
					new CodeDOMGenerator(host, new Microsoft.VisualBasic.VBCodeProvider()).ConvertContentDefinition(doc, writer);
					break;
			}
			return writer.ToString();
		}
		
		public override void SaveFile(string fileName)
		{
			ContentName   = fileName;
			XmlElement el = new XmlFormGenerator().GetElementFor(new XmlDocument(), host);
			
			foreach (XmlNode node in el.ChildNodes) {
				Console.WriteLine("append " + node.Name);
			}
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<" + el.Name + " version=\"1.0\"/>");
			
			foreach (XmlNode node in el.ChildNodes) {
				Console.WriteLine("append " + node.Name);
				doc.DocumentElement.AppendChild(doc.ImportNode(node, true));
			}
			switch (Path.GetExtension(fileName)) {
				case ".cs":
					new CodeDOMGenerator(host, new Microsoft.CSharp.CSharpCodeProvider()).ConvertContentDefinition(doc, fileName);
					break;
				case ".vb":
					new CodeDOMGenerator(host, new Microsoft.VisualBasic.VBCodeProvider()).ConvertContentDefinition(doc, fileName);
					break;
				default:
					doc.Save(fileName);
					break;
			}
			IsDirty = false;
		}
		/*
		// todo : complete CodeDOM writing
		void SaveCSharp(string fileName, XmlDocument doc)
		{
			
			
			
			foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
				if (node.Attributes["value"] != null) {
				}
			}
			
			co.Members.Add(cm);
			
			
			StreamWriter writer = File.CreateText(fileName);
			cg.GenerateCodeFromNamespace(cnamespace, writer, null);
			writer.Close();
		}*/
		
		public override void LoadFile(string fileName)
		{
			
		}
		
		// IDisplayBinding interface
		public void InitializeFrom(string fileName, string xmlContent)
		{
			host   = new DefaultDesignerHost();
			
			host.AddService(typeof(System.ComponentModel.Design.IComponentChangeService), new ComponentChangeService());
			host.AddService(typeof(System.Windows.Forms.Design.IUIService),                    new UIService());
			host.AddService(typeof(System.ComponentModel.Design.IDesignerOptionService),       new ICSharpCode.SharpDevelop.FormDesigner.Services.DesignerOptionService());
			host.AddService(typeof(System.ComponentModel.Design.ITypeDescriptorFilterService), new TypeDescriptorFilterService());
			
			host.AddService(typeof(System.Drawing.Design.IToolboxService),                new ToolboxService());
			host.AddService(typeof(System.Drawing.Design.IPropertyValueUIService),        new PropertyValueUIService());
			host.AddService(typeof(System.ComponentModel.Design.IExtenderListService),    new ExtenderListService());
			
			host.AddService(typeof(System.ComponentModel.Design.IDesignerHost),        host);
			host.AddService(typeof(System.ComponentModel.IContainer),                  host.Container);
			host.AddService(typeof(System.ComponentModel.Design.IDictionaryService),   new DictionaryService());
			host.AddService(typeof(System.ComponentModel.Design.IEventBindingService), new EventBindingServiceImpl());
			host.AddService(typeof(System.ComponentModel.Design.ISelectionService),    new SelectionService(host));
			host.AddService(typeof(System.ComponentModel.Design.IMenuCommandService),  new MenuCommandService(host));
			
			host.AddService(typeof(AmbientProperties),                                                ambientProperties);
			host.AddService(typeof(System.ComponentModel.Design.Serialization.INameCreationService),  new NameCreationService(host));
			host.AddService(typeof(System.ComponentModel.Design.IDesignerEventService), new DesignerEventService());
			host.AddService(typeof(System.ComponentModel.Design.Serialization.IDesignerSerializationService), new DesignerSerializationService(host));
			
			InitializeExtendersForProject(host);
			host.Activate();
			
			undoRedoManager = new UndoRedoManager(host);
			
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			//			selectionService.SelectionChanged += new EventHandler(SelectionChanged);
			
			tab = new AxSideTab(SharpDevelopSideBar.SideBar, "Forms");
			tab.CanSaved = false;
			
			ArrayList toolboxItems = BuildToolboxFromAssembly(Assembly.GetAssembly(typeof(System.Windows.Forms.Control)));
			ToolboxService toolboxService = (ToolboxService)host.GetService(typeof(IToolboxService));
			{
				ToolboxItem toolboxItem = new ToolboxItem();
				AxSideTabItem tabItem = SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem("Pointer", toolboxItem);
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				tabItem.Icon = new Bitmap(fileUtilityService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
				tabItem.Tag  = toolboxItem;
				tab.Items.Add(tabItem);
			}
			
			foreach (ToolboxItem toolboxItem in toolboxItems) {
				AxSideTabItem tabItem = SharpDevelopSideBar.SideBar.SideTabItemFactory.CreateSideTabItem(toolboxItem.DisplayName, toolboxItem);
				tabItem.Icon = toolboxItem.Bitmap;
				tabItem.Tag  = toolboxItem;
				tab.Items.Add(tabItem);
				toolboxService.AddToolboxItem(toolboxItem);
			}
			
			SelectedToolUsedHandler(this, EventArgs.Empty);
			toolboxService.SelectedItemUsed   += new EventHandler(SelectedToolUsedHandler);
			
			tab.ChoosedItemChanged += new EventHandler(SelectedTabItemChanged);
			SharpDevelopSideBar.SideBar.Tabs.Add(tab);
			
			host.TransactionClosed += new DesignerTransactionCloseEventHandler(TransactionFinished);
			
			XmlFormReader xmlReader = new XmlFormReader(host);
			
			xmlReader.SetUpDesignerHost(xmlContent);
			designPanel = new DesignPanel(host);
			designPanel.Location = new Point(0, 0);
			designPanel.Dock     = DockStyle.Fill;
			ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad.SetDesignerHost((DefaultDesignerHost)host);
			
			ContentName = fileName;
			
			SelectMe(this, EventArgs.Empty);
		}
		
		public virtual void Undo()
		{
			if (undoRedoManager != null) {
				undoRedoManager.Undo();
			}
		}
		
		public virtual void Redo()
		{
			if (undoRedoManager != null) {
				undoRedoManager.Redo();
			}
		}
		
		public void Cut(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Cut);
		}
		
		public void Copy(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Copy);
		}
		
		public void Paste(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Paste);
		}
		
		public void Delete(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Delete);
		}
		
		public void SelectAll(object sender, EventArgs e)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)host.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.SelectAll);
		}
		
		void TransactionFinished(object sender, DesignerTransactionCloseEventArgs e)
		{
			IsDirty = true;
		}
		
		void SelectedToolUsedHandler(object sender, EventArgs e)
		{
			tab.ChoosedItem = tab.Items[0];
			SharpDevelopSideBar.SideBar.Refresh();
		}
		
		void SelectedTabItemChanged(object sender, EventArgs e)
		{
			AxSideTabItem item = tab.ChoosedItem;
			ToolboxService toolboxService = (ToolboxService)host.GetService(typeof(IToolboxService));
			
			if (item == null) {
				toolboxService.SetSelectedToolboxItem(null);
			} else {
				//				if (designPanel.ComponentTray.CanCreateComponentFromTool(item.Tag as ToolboxItem)) {
					//					designPanel.ComponentTray.CreateComponentFromTool(item.Tag as ToolboxItem);
				//				} else {
					toolboxService.SetSelectedToolboxItem(item.Tag as ToolboxItem);
				//				}
			}
		}
		
		
		// END IDisplayBinding interface
		
		// Service initialization
		ArrayList BuildToolboxFromAssembly(Assembly a)
		{
			ArrayList toolBoxItems = new ArrayList();
			
			Hashtable images = new Hashtable();
			ImageList il = new ImageList();
			// try to load res icon
			string[] imgNames = a.GetManifestResourceNames();
			
			foreach (string im in imgNames) {
				try {
					Bitmap b = new Bitmap(Image.FromStream(a.GetManifestResourceStream(im)));
					b.MakeTransparent();
					images[im] = il.Images.Count;
					il.Images.Add(b);
				} catch {}
			}
			
			Module[] ms = a.GetModules(false);
			foreach (Module m in ms) {
				Type[] ts = m.GetTypes();
				foreach (Type t in ts) {
					if (t.IsDefined(typeof(ToolboxItemFilterAttribute), true)) {
						object[] flt = t.GetCustomAttributes(typeof(ToolboxItemFilterAttribute), true);
						
						// here we should enumerate all attributes ...
						ToolboxItemFilterAttribute f = (ToolboxItemFilterAttribute)flt[0];
						// get only this components which requires 'Windows Forms' toolbox filter
						if (f.FilterType == ToolboxItemFilterType.Allow &&
						    f.FilterString == "System.Windows.Forms") {
						    	if (images[t.Namespace + "." + t.Name + ".bmp"] != null) {
						    		ToolboxItem item = new ToolboxItem(t);
						    		// filter out UserControl manually
						    		if (item.DisplayName != "UserControl") {
						    			toolBoxItems.Add(item);
						    		}
						    	}
						    }
					}
				}
			}
			
			toolBoxItems.Sort(new ToolboxItemSorter());
			return toolBoxItems;
		}
		
		class ToolboxItemSorter : IComparer
		{
			string[] sortOderString = {
				"Label",
				"LinkLabel",
				"Button",
				"MainMenu",
				"ListBox",
				"TextBox",
				"RadioButton",
				"CheckBox",
				"GroupBox",
				"PictureBox",
				"Panel",
				"DataGrid",
				"CheckedListBox",
				"ComboBox",
				"ListView",
				"TreeView",
				"TabControl",
				"DateTimePicker",
				"MonthCalendar",
				"HScrollBar",
				"VScrollBar",
				"Timer",
				"Splitter",
				"PropertyGrid",
				"DomainUpDown",
				"NumericUpDown",
				"TrackBar",
				"ProgressBar",
				"RichTextBox",
				"ImageList",
				"HelpProvider",
				"ToolTip",
				"ContextMenu",
				"ToolBar",
				"StatusBar",
				"NotifyIcon",
				"OpenFileDialog",
				"SaveFileDialog",
				"FontDialog",
				"ColorDialog",
				"PrintDialog",
				"PrintPreviewDialog",
				"PrintPreviewControl",
				"ErrorProvider",
				"PrintDocument",
				"PageSetupDialog",
				"CrystalReportViewer"
			};
			
			int GetIndex(string name)
			{
				for (int i = 0; i < sortOderString.Length; ++i) {
					if (sortOderString[i] == name) {
						return i;
					}
				}
				return -1;
			}
			
			public int Compare(object a, object b)
			{
				ToolboxItem itema = a as ToolboxItem;
				ToolboxItem itemb = b as ToolboxItem;
				int idxa = GetIndex(itema.DisplayName);
				int idxb = GetIndex(itemb.DisplayName);
				if (idxa >= 0 && idxb >= 0) {
					return idxa - idxb;
				}
				if (idxa >= 0 && idxb < 0) {
					return -1;
				}
				if (idxa < 0 && idxb >= 0) {
					return 1;
				}
				return itema.DisplayName.CompareTo(itemb.DisplayName);
			}
		}
		
		void InitializeExtendersForProject(IDesignerHost host)
		{
			ExtenderListService elsi = (ExtenderListService)host.GetService(typeof(IExtenderListService));
			
			elsi.ExtenderProviders.Add(new NameExtender());
		}
		
		/// <summary>
		/// This is used for letting the View Code command work.
		/// </summary>
		public virtual void ShowSourceCode()
		{
			
		}
		
	}
}
