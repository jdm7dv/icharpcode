// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;


using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class TextEditorDisplayBinding : IDisplayBinding
	{
		public virtual bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public virtual bool CanCreateContentForLanguage(string language)
		{
			return true;
		}
		
		public virtual IViewContent CreateContentForFile(string fileName)
		{
			TextEditorDisplayBindingWrapper b2 = new TextEditorDisplayBindingWrapper();
			
			b2.textAreaControl.Dock = DockStyle.Fill;
			b2.LoadFile(fileName);
			b2.textAreaControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);
			b2.textAreaControl.InitializeFormatter();
			return b2;
		}
		
		public virtual IViewContent CreateContentForLanguage(string language, string content)
		{
			TextEditorDisplayBindingWrapper b2 = new TextEditorDisplayBindingWrapper();
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			b2.textAreaControl.Document.TextContent = stringParserService.Parse(content);
			b2.textAreaControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(language);
			b2.textAreaControl.InitializeFormatter();
			return b2;
		}		
	}
	
	public class TextEditorDisplayBindingWrapper : AbstractViewContent, IMementoCapable, IPrintable, IEditable, IPositionable, ITextAreaControlProvider
	{
		public SharpDevelopTextAreaControl textAreaControl = new SharpDevelopTextAreaControl();

		public TextAreaControl TextAreaControl {
			get {
				return textAreaControl;
			}
		}
		
		/*
		// KSL Start, New lines
		FileSystemWatcher watcher;
		bool wasChangedExternally = false;
		// KSL End 
		*/
		
		public string TextContent {
			get {
				return textAreaControl.Document.TextContent;
			}
			set {
				textAreaControl.Document.TextContent = value;
			}
		}
		
		public PrintDocument PrintDocument {
			get { 
				return textAreaControl.PrintDocument;
			}
		}
		
		public IClipboardHandler ClipboardHandler {
			get {
				return textAreaControl.ClipboardHandler;
			}
		}
		
		public override Control Control {
			get {
				return textAreaControl;
			}
		}
		
		public void Undo()
		{
			 textAreaControl.Undo();
		}
		public void Redo()
		{
			 textAreaControl.Redo();
		}
		
		public TextEditorDisplayBindingWrapper()
		{
			textAreaControl.Changed += new EventHandler(TextAreaChangedEvent);
			textAreaControl.Document.Caret.OffsetChanged    += new CaretEventHandler(CaretChanged);
			textAreaControl.Document.Caret.CaretModeChanged += new CaretEventHandler(CaretModeChanged);
			
			textAreaControl.TextAreaPainter.Enter += new EventHandler(CaretUpdate);
			/*
			// KSL Start, New lines
			textAreaControl.FileNameChanged += new EventHandler(FileNameChangedEvent);
			textAreaControl.GotFocus += new EventHandler(GotFocusEvent);
			// KSL End 
			*/
			
		}
		// KSL Start, new event handlers

		/*
		void SetWatcher()
		{
			if (this.watcher == null) {
				this.watcher = new FileSystemWatcher();
				this.watcher.Changed += new FileSystemEventHandler(this.OnFileChangedEvent);
			}
			else {
				this.watcher.EnableRaisingEvents = false;
			}
			this.watcher.Path = Path.GetDirectoryName(textAreaControl.FileName);
			this.watcher.Filter = Path.GetFileName(textAreaControl.FileName);
			this.watcher.NotifyFilter = NotifyFilters.LastWrite;
			this.watcher.EnableRaisingEvents = true;
		} */
		
		/*
		void FileNameChangedEvent(object sender, EventArgs e)
		{
			if (textAreaControl.FileName != null) {
				SetWatcher();
			} else {
				this.watcher = null;
			}
		}

		void GotFocusEvent(object sender, EventArgs e)
		{
			lock (this) {
				if (wasChangedExternally) {
					wasChangedExternally = false;
					if (MessageBox.Show("The file " + textAreaControl.FileName + " has been changed externally to SharpDevelop.\nDo you want to reload it?",
					                    "SharpDevelop",
					                    MessageBoxButtons.YesNo,
					                    MessageBoxIcon.Question) == DialogResult.Yes) {
					                    	LoadFile(textAreaControl.FileName);
					}
				}
			}
		}
		
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			lock (this) {
				wasChangedExternally = true;
			}
		}

		// KSL End
		*/
		void TextAreaChangedEvent(object sender, EventArgs e)
		{
			IsDirty = true;
		}
		
		public override void RedrawContent()
		{
			textAreaControl.OptionsChanged();
		}
		
		public override void Dispose()
		{
			textAreaControl.Dispose();
		}
		
		public override bool IsReadOnly {
			get {
				return textAreaControl.IsReadOnly;
			}
		}
		
		public override void SaveFile(string fileName)
		{
			/*
			// KSL, Start new line
			if (watcher != null) {
				this.watcher.EnableRaisingEvents = false;
			}
			// KSL End
			*/
			
			textAreaControl.SaveFile(fileName);
			ContentName = fileName;
			IsDirty     = false;
			
			/*
			// KSL, Start new lines
			if (this.watcher != null) {
				this.watcher.EnableRaisingEvents = true;
			}
			// KSL End
			*/
		}
		
		public override void LoadFile(string fileName)
		{
			textAreaControl.IsReadOnly = (File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
			    
			textAreaControl.LoadFile(fileName);
			ContentName = fileName;
			IsDirty     = false;
		}
		
		public IXmlConvertable CreateMemento()
		{
			DefaultProperties properties = new DefaultProperties();
			properties.SetProperty("Bookmarks",   textAreaControl.Document.BookmarkManager.CreateMemento());
			properties.SetProperty("CaretOffset", textAreaControl.Document.Caret.Offset);
			properties.SetProperty("VisibleLine",   textAreaControl.FirstVisibleColumn);
			properties.SetProperty("VisibleColumn", textAreaControl.FirstVisibleRow);
//			properties.SetProperty("Properties",    textAreaControl.Properties);
			properties.SetProperty("HighlightingLanguage", textAreaControl.Document.HighlightingStrategy.Name);
			return properties;
		}
		
		public void SetMemento(IXmlConvertable memento)
		{
			IProperties properties = (IProperties)memento;
			BookmarkManagerMemento bookmarkMemento = (BookmarkManagerMemento)properties.GetProperty("Bookmarks", textAreaControl.Document.BookmarkManager.CreateMemento());
			bookmarkMemento.CheckMemento(textAreaControl.Document);
			textAreaControl.Document.BookmarkManager.SetMemento(bookmarkMemento);
			textAreaControl.Document.Caret.Offset = Math.Min(textAreaControl.Document.TextLength, Math.Max(0, properties.GetProperty("CaretOffset", textAreaControl.Document.Caret.Offset)));
			textAreaControl.Document.SetDesiredColumn();
			textAreaControl.FirstVisibleColumn    = Math.Min(textAreaControl.Document.TotalNumberOfLines, Math.Max(0, properties.GetProperty("VisibleLine", textAreaControl.FirstVisibleColumn)));
			textAreaControl.FirstVisibleRow       = Math.Max(0, properties.GetProperty("VisibleColumn", textAreaControl.FirstVisibleRow));
//			textAreaControl.Document.Properties   = (IProperties)properties.GetProperty("Properties",    textAreaControl.Properties);
			IHighlightingStrategy highlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(properties.GetProperty("HighlightingLanguage", textAreaControl.Document.HighlightingStrategy.Name));
			
			if (highlightingStrategy != null) {
				textAreaControl.Document.HighlightingStrategy = highlightingStrategy;
			}
			
			// insane check for cursor position, may be required for document reload.
			int lineNr = textAreaControl.Document.GetLineNumberForOffset(textAreaControl.Document.Caret.Offset);
			LineSegment lineSegment = textAreaControl.Document.GetLineSegment(lineNr);
			textAreaControl.Document.Caret.Offset = Math.Min(lineSegment.Offset + lineSegment.Length, textAreaControl.Document.Caret.Offset);
			
			textAreaControl.OptionsChanged();
			textAreaControl.Refresh();
		}
		
		void CaretUpdate(object sender, EventArgs e)
		{
			CaretChanged(null, null);
			CaretModeChanged(null, null);
		}
		
		void CaretChanged(object sender, CaretEventArgs e)
		{
			Point    pos       = textAreaControl.Document.OffsetToView(textAreaControl.Document.Caret.Offset);
			LineSegment line   = textAreaControl.Document.GetLineSegment(pos.Y);
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetCaretPosition(pos.X + 1, pos.Y + 1, textAreaControl.Document.Caret.Offset - line.Offset + 1);
		}
		
		void CaretModeChanged(object sender, CaretEventArgs e)
		{
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetInsertMode(textAreaControl.Document.Caret.CaretMode == CaretMode.InsertMode);
		}
				
		public override string ContentName {
			set {
				if (Path.GetExtension(ContentName) != Path.GetExtension(value)) {
					if (textAreaControl.Document.HighlightingStrategy != null) {
						textAreaControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(value);
						textAreaControl.Refresh();
					}
				}
				base.ContentName = value;
			}
		}
				
		public void JumpTo(Point pos)
		{
			textAreaControl.JumpTo(pos);
		}
	}
}
