// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Services;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels 
{
	public class CodeTemplatePane : AbstractOptionPanel
	{
		System.ComponentModel.Container components;
		System.Windows.Forms.Button editButton;
		TextAreaControl textAreaControl = new TextAreaControl();
		
		System.Windows.Forms.Button removeButton;
		System.Windows.Forms.Button addButton;
		System.Windows.Forms.ColumnHeader Description;
		System.Windows.Forms.ColumnHeader Template;
		
		System.Windows.Forms.ListView listView1;
		System.Windows.Forms.Panel    panel1;
		ArrayList templates;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				CodeTemplateLoader.Template = templates;
				CodeTemplateLoader.SaveTemplates();
			}
			return true;
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
				textAreaControl.Dispose();
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		
		int GetCurrentIndex()
		{
			if (listView1.SelectedItems.Count > 0) {
				return listView1.SelectedItems[0].Index;
			}
			return -1;
		}
		
		void RemoveEvent(object sender, System.EventArgs e)
		{
			int i = GetCurrentIndex();
			if (i != -1) {
				templates.RemoveAt(i);
				BuildListView();
			}
		}
		
		void AddEvent(object sender, System.EventArgs e)
		{
			CodeTemplate t = new CodeTemplate();
			EditTemplateDialog etd = new EditTemplateDialog(t);
			if (etd.ShowDialog() == DialogResult.OK) {
				templates.Add(t);
				listView1.SelectedItems.Clear();
				BuildListView();
				//		   TODO TODO TODO TODO
				//				listView1.SelectedItems.Add(listView1.Items[templates.Count-1]);
				listView1.Select();
			}
			etd.Dispose();
		}
		
		void IndexChange(object sender, System.EventArgs e)
		{
			int i = GetCurrentIndex();
			if (i != -1) {
				textAreaControl.Enabled = true;
				textAreaControl.Document.TextContent = ((CodeTemplate)templates[i]).Text;
			} else {
				textAreaControl.Enabled = false;
				textAreaControl.Document.TextContent = String.Empty;
			}
			textAreaControl.Refresh();
		}
		
		void TextChange(object sender, DocumentAggregatorEventArgs e)
		{
			int i = GetCurrentIndex();
			if (i != -1) {
				((CodeTemplate)templates[i]).Text = textAreaControl.Document.TextContent;
			}
		}
		
		void EditEvent(object sender, System.EventArgs e)
		{
			int i = GetCurrentIndex();
			if (i != -1) {
				CodeTemplate t = (CodeTemplate)templates[i];
				t = new CodeTemplate(t.Shortcut, t.Description, t.Text);
				EditTemplateDialog etd = new EditTemplateDialog(t);
				if (etd.ShowDialog() == DialogResult.OK) {
					templates[i] = t;
				}
				etd.Dispose();
				BuildListView();
			}
		}
		
		void BuildListView()
		{
			listView1.Items.Clear();
			foreach (CodeTemplate t in templates) {
				listView1.Items.Add(new ListViewItem(new string[] { t.Shortcut, t.Description }));
			}
		}
		
		public CodeTemplatePane()// : base(resourceService.GetString("Dialog.Options.CodeTemplatesText"))
		{
			InitializeComponents();
			
			textAreaControl.Dock     = DockStyle.Fill;
			textAreaControl.Document.Properties = new DefaultProperties();
			textAreaControl.Document.ReadOnly = false;
			textAreaControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();
			textAreaControl.ShowLineNumbers  = false;
			textAreaControl.ShowInvalidLines = false;
			textAreaControl.EnableFolding    = false;
			textAreaControl.ScrollLineHeight = 0;
			
			//			textAreaControl.Font = new Font("Courier New", 10);
			
			//			TextBox.TextAreaPainter.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy("C#");
			//			TextBox.TextAreaPainter.ContextMenu = null; // FIXME : Make better MenuAction execute structure.
			
			textAreaControl.Size = new System.Drawing.Size(328, 136);
			textAreaControl.TabIndex   = 1;
			textAreaControl.Location   = new System.Drawing.Point(8, 184);
			
			textAreaControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			panel1.Controls.Add(textAreaControl);
			
			//			AcceptButton = okButton;
			//			CancelButton = cancelButton;
			removeButton.Click += new System.EventHandler(RemoveEvent);
			addButton.Click += new System.EventHandler(AddEvent);
			editButton.Click += new System.EventHandler(EditEvent);
			listView1.Activation = ItemActivation.Standard;
			listView1.ItemActivate += new System.EventHandler(EditEvent);
			listView1.SelectedIndexChanged += new System.EventHandler(IndexChange);
			textAreaControl.Document.DocumentChanged += new DocumentAggregatorEventHandler(TextChange);
			BuildListView();
			IndexChange(null, null);
			//			StartPosition = FormStartPosition.CenterParent;
			//			MaximizeBox  = MinimizeBox = false;
		}
		
		// TODO : remove this hack
		// hack is 4 the bug in the code template window, if you press a shortcut key
		// the shortcut is activated, no matter if the alt key is pressed.
		// That's why I filter out the shortcuts from the buttons.
		string RemoveCh(string str)
		{
			string back = "";
			for (int i = 0; i < str.Length; ++i)
				if (str[i] != '&')
					back += str[i];
			return back;
		}
		
		void InitializeComponents()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			templates = (ArrayList)CodeTemplateLoader.Template.Clone();
			this.components = new System.ComponentModel.Container();
			this.removeButton = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.Description = new System.Windows.Forms.ColumnHeader();
//			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.editButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.Template = new System.Windows.Forms.ColumnHeader();
			
			removeButton.Location = new System.Drawing.Point(152 + 20, 152);
			removeButton.Size = new System.Drawing.Size(74, 24);
			removeButton.TabIndex = 7;
			removeButton.Text = RemoveCh(resourceService.GetString("Global.RemoveButtonText"));
			removeButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			removeButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			panel1.Size = new System.Drawing.Size(352, 328);
			panel1.TabIndex = 0;
			panel1.Text = "panel1";
			panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom| AnchorStyles.Right |AnchorStyles.Left;
			
			Description.Text = RemoveCh(resourceService.GetString("Dialog.Options.CodeTemplate.Description"));
			Description.Width = 175;
			Description.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			
			listView1.Text = "listView1";
			listView1.MultiSelect = false;
			listView1.Size = new System.Drawing.Size(328, 136);
			listView1.View = System.Windows.Forms.View.Details;
			listView1.ForeColor = System.Drawing.SystemColors.WindowText;
			listView1.TabIndex = 0;
			listView1.Location = new System.Drawing.Point(8, 8);
			listView1.FullRowSelect = true;
			listView1.GridLines   = true;
			listView1.HideSelection = false;
			listView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			listView1.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
//  		listView1.Sorting = SortOrder.Ascending;
			
			listView1.Columns.Add(Template);
			listView1.Columns.Add(Description);
				
			editButton.Location = new System.Drawing.Point(80 + 10, 152);
			editButton.Size = new System.Drawing.Size(74, 24);
			editButton.TabIndex = 3;
			editButton.Text = RemoveCh(resourceService.GetString("Global.ChangeButtonText"));
			editButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			editButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			addButton.Location = new System.Drawing.Point(8, 152);
			addButton.Size = new System.Drawing.Size(74, 24);
			addButton.TabIndex = 2;
			addButton.Text = RemoveCh(resourceService.GetString("Global.AddButtonText"));
			addButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			addButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
//			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
//			//@design this.TrayLargeIcon = true;
//			//@design this.TrayHeight = 0;
			this.ClientSize = new System.Drawing.Size(344, 325);
			
			Template.Text = RemoveCh(resourceService.GetString("Dialog.Options.CodeTemplate.Template"));
			Template.Width = 77;
			Template.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			
			this.Controls.Add(panel1);
			panel1.Controls.Add(editButton);
			panel1.Controls.Add(removeButton);
			panel1.Controls.Add(addButton);
			panel1.Controls.Add(listView1);
			
		}
	}
}
