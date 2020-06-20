// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
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
using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;
using CSharpBinding;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public class CSharpDesignerDisplayBindingWrapper : FormDesignerDisplayBindingBase, ITextAreaControlProvider
	{
		TabControl   tabControl;
		TabPage      designerPage;
		IViewContent csharpTextEditor;
		bool         failedDesignerInitialize;
		
		public TextAreaControl TextAreaControl {
			get {
				return (TextAreaControl)csharpTextEditor.Control;
			}
		}
		
		public override IClipboardHandler ClipboardHandler {
			get {
				if (tabControl.SelectedTab.Controls[0] is IEditable) {
					return ((IEditable)tabControl.SelectedTab.Controls[0]).ClipboardHandler;
				}
				return base.ClipboardHandler;
			}
		}
		
		public override string TextContent {
			get {
				return ((IEditable)csharpTextEditor).TextContent;
			}
			set {
				((IEditable)csharpTextEditor).TextContent = value;
			}
		}

		
		public override Control Control {
			get {
				return tabControl;
			}
		}
		
		IClass c;
		IMethod initializeComponents;
		string fileName;
		
		public CSharpDesignerDisplayBindingWrapper(string fileName, IClass c, IMethod initializeComponents)
		{
			this.fileName = fileName;
			this.c = c;
			this.initializeComponents = initializeComponents;
			
			TextEditorDisplayBinding tdb = new TextEditorDisplayBinding();
			csharpTextEditor = tdb.CreateContentForFile(fileName);
			InitializeComponents();
	 	}
		
		public CSharpDesignerDisplayBindingWrapper(string fileName, string content, IClass c, IMethod initializeComponents)
		{
			this.fileName = fileName;
			this.c = c;
			this.initializeComponents = initializeComponents;
			
			TextEditorDisplayBinding tdb = new TextEditorDisplayBinding();
			csharpTextEditor = tdb.CreateContentForLanguage("C#", content);
			InitializeComponents();
		}
		
		public override void Undo()
		{
			if (isFormDesignerVisible) {
				base.Undo();
			} else {
				((IEditable)csharpTextEditor).Undo();
			}
		}
		
		public override void Redo()
		{
			if (isFormDesignerVisible) {
				base.Redo();
			} else {
				((IEditable)csharpTextEditor).Redo();
			}
		}
		
		void InitializeComponents()
		{
			tabControl = new TabControl();
			tabControl.SelectedIndexChanged += new EventHandler(TabIndexChanged);
			tabControl.Alignment = TabAlignment.Bottom;
			
			TabPage sourcePage = new TabPage("Source");
			csharpTextEditor.Control.Dock = DockStyle.Fill;
			sourcePage.Controls.Add(csharpTextEditor.Control);
			tabControl.TabPages.Add(sourcePage);
			
			string formCode = GenerateClassString(TextAreaControl);
			string sourceFile = Path.GetTempFileName();
			string exeFile    = Path.GetTempFileName();
			StreamWriter sw = File.CreateText(sourceFile);
			sw.Write(formCode);
			sw.Close();
			
			Hashtable options = new Hashtable();
			options["target"] = "library";
			CSharpBindingCompilerManager compilerManager = new CSharpBindingCompilerManager();
			CSharpCompilerParameters param = new CSharpCompilerParameters();
			param.OutputAssembly = exeFile;
			param.CompileTarget  = CompileTarget.Library;
		
			ICompilerResult result = compilerManager.CompileFile(sourceFile, param);
			File.Delete(sourceFile);
			
			failedDesignerInitialize = true;
			StringWriter writer = new StringWriter();
			if (result.CompilerResults.Errors.Count > 0) {
				MessageBox.Show("Round trip failed due erros in input file: " + result.CompilerResults.Errors[0]);
			} else {
				Control ctrl = null;
				try {
					Assembly asm = Assembly.LoadFrom(exeFile);
					ctrl = (Control)asm.CreateInstance("Unknown");
				} catch (Exception) {}
				
				if (ctrl == null) {
					MessageBox.Show("Error creating the type for the round trip");
				} else {
					XmlElement el = new XmlFormGenerator().GetElementFor(new XmlDocument(), ctrl);
					if (el == null) {
						MessageBox.Show("Error while storing to xml");
					} else {
						try {
							XmlDocument doc = new XmlDocument();
							
							doc.LoadXml("<" + ctrl.GetType().BaseType.FullName + "/>");
							
							foreach (XmlNode node in el.ChildNodes) {
								if (node.Name == "Visible" || node.Name == "Location") {
									continue;
								}
								doc.DocumentElement.AppendChild(doc.ImportNode(node, true));
							}
							doc.Save(writer);
							failedDesignerInitialize = false;
						} catch (Exception e) {
							MessageBox.Show("Failed for unexpected reason :\n" + e.ToString());
						}
					}
				} 
			}
			if (failedDesignerInitialize) {
				writer.WriteLine("<System.Windows.Forms.Form/>");
			}
			
			InitializeFrom(fileName, writer.ToString());
			designerPage = new TabPage("Design");
			designerPage.Controls.Add(designPanel);
			tabControl.TabPages.Add(designerPage);
			
			if (failedDesignerInitialize) {
				sourcePage = new TabPage("Generated");
				TextEditorDisplayBinding tdb = new TextEditorDisplayBinding();
				IViewContent newTextEditor = tdb.CreateContentForLanguage("C#", formCode);
				newTextEditor.Control.Dock = DockStyle.Fill;
				((SharpDevelopTextAreaControl)newTextEditor.Control).Document.ReadOnly = true;
				sourcePage.Controls.Add(newTextEditor.Control);
				tabControl.TabPages.Add(sourcePage);
			}
			csharpTextEditor.DirtyChanged += new EventHandler(TextAreaIsDirty);
		}
		
		void TextAreaIsDirty(object sender, EventArgs e)
		{
			base.IsDirty = csharpTextEditor.IsDirty;
		}
		
		string GenerateClassString(TextAreaControl textArea)
		{
			Reparse(fileName, textArea.Document.TextContent);
			
			StringBuilder builder = new StringBuilder();
			// generate usings
			foreach (IUsing u in c.CompilationUnit.Usings) {
				foreach (string usingString in u.Usings) {
					if (usingString.StartsWith("System")) {
						builder.Append("using " + usingString + ";\n");
					}
				}
			}
			
			builder.Append("class Unknown : ");
			builder.Append(ExctractBaseClass(c));
			builder.Append(" {\n");
			ArrayList fields = GetUsedFields(textArea.Document, c, initializeComponents);
			foreach (IField field in fields) {
				LineSegment fieldLine = textArea.Document.GetLineSegment(field.Region.BeginLine - 1);
				builder.Append(textArea.Document.GetText(fieldLine.Offset, fieldLine.Length));
				builder.Append("\n");
			}
			
			builder.Append("\tpublic Unknown() {\n\t\t");
			builder.Append(initializeComponents.Name);
			builder.Append("();\n");
			builder.Append("\t}\n");
			string initializeComponentsString = GetInitializeComponentsString(textArea.Document, initializeComponents);
			builder.Append(initializeComponentsString);
			/*
			// get events
			Hashtable eventTable = new Hashtable();
			Regex eventHandler = new Regex(@"new\s+(?<EventName>\S*)EventHandler\s*\(\s*(\w*\.)?(?<HandlerMethod>\w*)\s*\)\s*;", RegexOptions.Compiled);
			Match match = eventHandler.Match(initializeComponentsString);
			while (match.Success) {
				eventTable[match.Result("${HandlerMethod}")] = match.Result("${EventName}");
				match = match.NextMatch();
			}
			
			// write event methods
			foreach (DictionaryEntry entry in eventTable) {
				builder.Append("\n\tvoid " + entry.Key.ToString() + "(object sender, " + entry.Value.ToString() + "EventArgs e)\n");
				builder.Append("\t{}\n");
			}*/
			
			builder.Append("}");
			return builder.ToString();
		}
		
		string GetInitializeComponentsString(IDocumentAggregator doc, IMethod initializeComponents)
		{
			LineSegment beginLine = doc.GetLineSegment(initializeComponents.Region.BeginLine - 1);
			LineSegment endLine   = doc.GetLineSegment(initializeComponents.Region.EndLine - 1);
			
			int startOffset = beginLine.Offset + initializeComponents.Region.BeginColumn - 1;
			int endOffset   = endLine.Offset   + initializeComponents.Region.EndColumn - 1;
			
			string initializeComponentsString = doc.GetText(startOffset, endOffset - startOffset);
			int idx = initializeComponentsString.LastIndexOf('}');
			if (idx > 0) {
				initializeComponentsString = initializeComponentsString.Substring(0, idx + 1);
			}
			return initializeComponentsString;
		}
		
		string ExctractBaseClass(IClass c)
		{
			foreach (string baseType in c.BaseTypes) {
				if (baseType == "System.Windows.Forms.Form" ||
				    baseType == "Form" ||
				    baseType == "System.Windows.Forms.UserControl" ||
				    baseType == "UserControl") {
					
					return baseType;
				}
			}
			return String.Empty;
		}
		
		ArrayList GetUsedFields(IDocumentAggregator doc, IClass c, IMethod initializeComponents)
		{
			string InitializeComponentsString = GetInitializeComponentsString(doc, initializeComponents);
			
			ArrayList fields = new ArrayList();
			foreach (IField field in c.Fields) {
				if (field.IsPrivate) {
					if (InitializeComponentsString.IndexOf(field.Name) >= 0) {
						fields.Add(field);
					}
				}
			}
			return fields;
		}
		
		void DeleteFormFields()
		{
			TextAreaControl textArea = TextAreaControl;
			ArrayList fields = GetUsedFields(textArea.Document, c, initializeComponents);
			for (int i = fields.Count - 1; i >= 0; --i) {
				IField field = (IField)fields[i];
				LineSegment fieldLine = textArea.Document.GetLineSegment(field.Region.BeginLine - 1);
				textArea.Document.Remove(fieldLine.Offset, fieldLine.TotalLength);
			}
		}
		
		void MergeFormChanges()
		{
			IParserService parserService    = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			// generate file and get initialize components string
			string currentForm = GetDataAs("C#");
			IParseInformation generatedInfo = parserService.ParseFile(fileName == null ? "a.cs" : fileName, currentForm);
			ICompilationUnit cu = (ICompilationUnit)generatedInfo.BestCompilationUnit;
			IClass generatedClass = cu.Classes[0];
			IMethod generatedInitializeComponents = GetInitializeComponents(cu.Classes[0]);
			IDocumentAggregator newDoc = new DocumentAggregatorFactory().CreateDocument();
			newDoc.TextContent = currentForm;
			string newInitializeComponents = GetInitializeComponentsString(newDoc, generatedInitializeComponents);
			
			TextAreaControl textArea = TextAreaControl;
			Reparse(fileName, textArea.Document.TextContent);
			DeleteFormFields();
			
			// replace the old initialize components method with the new one
			Reparse(fileName, textArea.Document.TextContent);
			LineSegment beginLine = textArea.Document.GetLineSegment(initializeComponents.Region.BeginLine - 1);
			int startOffset = beginLine.Offset + initializeComponents.Region.BeginColumn - 1;
			textArea.Document.Replace(startOffset, GetInitializeComponentsString(textArea.Document, initializeComponents).Length, newInitializeComponents);
			Reparse(fileName, textArea.Document.TextContent);
			
			// insert new fields
			int lineNr = c.Region.BeginLine - 1;
			while (true) {
				if (lineNr >= textArea.Document.TotalNumberOfLines - 2) {
					break;
				}
				LineSegment curLine = textArea.Document.GetLineSegment(lineNr);
				if (textArea.Document.GetText(curLine.Offset, curLine.Length).Trim().EndsWith("{")) {
					break;
				}
				++lineNr;
			}
			beginLine = textArea.Document.GetLineSegment(lineNr + 1);
			int insertOffset = beginLine.Offset;
			foreach (IField field in generatedClass.Fields) {
				LineSegment fieldLine = newDoc.GetLineSegment(field.Region.BeginLine - 1);
				textArea.Document.Insert(insertOffset,newDoc.GetText(fieldLine.Offset, fieldLine.TotalLength));
			}
		}
		
		void Reparse(string fileName, string content)
		{
			if (fileName == null) {
				fileName = "a.cs";
			}
			// get new initialize components
			IParserService parserService    = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			IParseInformation info = parserService.ParseFile(fileName, content);
			ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (BaseClassIsFormOrControl(c)) {
					initializeComponents = GetInitializeComponents(c);
					if (initializeComponents != null) {
						this.c = c;
						break;
					}
				}
			}
		}
		bool BaseClassIsFormOrControl(IClass c)
		{
			foreach (string baseType in c.BaseTypes) {
				if (baseType == "System.Windows.Forms.Form" ||
					baseType == "Form" ||
					baseType == "System.Windows.Forms.UserControl" ||
					baseType == "UserControl") {
						return true;
					}
			}
			return false;
		}
		
		IMethod GetInitializeComponents(IClass c)
		{
			foreach (IMethod method in c.Methods) {
				if ((method.Name == "InitializeComponents" ||
				    method.Name == "InitializeComponent") && method.Parameters.Count == 0) {
					return method;
				}
			}
			return null;
		}
		
		void TabIndexChanged(object sender, EventArgs e)
		{
			switch (tabControl.SelectedIndex) {
				case 0:
					if (!failedDesignerInitialize) {
						bool dirty = IsDirty;
						MergeFormChanges();
						IsDirty = dirty;
						TextAreaControl.Refresh();
					}
					break;
			}
			isFormDesignerVisible = tabControl.SelectedIndex == 1;
		}
		public override void ShowSourceCode()
		{
			tabControl.SelectedIndex = 0;
		}

		public override void SaveFile(string fileName)
		{
			ContentName = fileName;
			if (Path.GetExtension(fileName) == ".xfrm") {
				base.SaveFile(fileName);
				return;
			}
			if (tabControl.SelectedIndex == 0 || failedDesignerInitialize) {
				csharpTextEditor.SaveFile(fileName);
			} else {
				MergeFormChanges();
				csharpTextEditor.SaveFile(fileName);
			}
		}
	}
	
	public class CSharpDesignerDisplayBinding : IDisplayBinding
	{
		IMethod GetInitializeComponents(IClass c)
		{
			foreach (IMethod method in c.Methods) {
				if ((method.Name == "InitializeComponents" ||
				     method.Name == "InitializeComponent") && method.Parameters.Count == 0) {
					return method;
				}
			}
			return null;
		}
		bool BaseClassIsFormOrControl(IClass c)
		{
			foreach (string baseType in c.BaseTypes) {
				if (baseType == "System.Windows.Forms.Form" ||
				    baseType == "Form" ||
				    baseType == "System.Windows.Forms.UserControl" ||
				    baseType == "UserControl") {
				    	return true;
				    }
				    
			}
			return false;
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			if (Path.GetExtension(fileName) == ".cs") {
				IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
				IParseInformation info = parserService.ParseFile(fileName);
				ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
				foreach (IClass c in cu.Classes) {
					if (BaseClassIsFormOrControl(c)) {
						IMethod method = GetInitializeComponents(c);
						if (method == null) {
							continue;
						}
						return true;
					}
				}
			}
			return false;
		}
		
		public bool CanCreateContentForLanguage(string languageName)
		{
			return languageName == "CSharpForm";
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			IParseInformation info = parserService.ParseFile(fileName);
			ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (BaseClassIsFormOrControl(c)) {
					IMethod method = GetInitializeComponents(c);
					if (method == null) {
						continue;
					}
					return new CSharpDesignerDisplayBindingWrapper(fileName, c, method);
				}
			}
			return null;
		}
		
		public IViewContent CreateContentForLanguage(string languageName, string content)
		{
			IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			Console.WriteLine(content);
			IParseInformation info = parserService.ParseFile(@"C:\a.cs", content);
			
			ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (BaseClassIsFormOrControl(c)) {
					IMethod method = GetInitializeComponents(c);
					if (method == null) {
						continue;
					}
					return new CSharpDesignerDisplayBindingWrapper(null, content, c, method);
				}
			}
			return null;
		}
	}
}
