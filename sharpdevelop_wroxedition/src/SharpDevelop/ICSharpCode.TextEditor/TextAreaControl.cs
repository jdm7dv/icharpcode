// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Xml;
using System.Text;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class puts all textarea components together and makes a nice control
	/// out of it.
	/// </summary>
	public class TextAreaControl : UserControl, IEditActionHandler, IPositionable, IPrintable, IEditable
	{
		Ruler  ruler            = null;
		Panel  rowpanel         = new Panel();
		
		VScrollBar vscrollbar   = new VScrollBar();
		HScrollBar hscrollbar   = new HScrollBar();
		
		TextAreaMouseHandler mouseeventhandler = null;
		TextAreaPainter      textarea;
		Gutter       gutter = null;
		IClipboardHandler    clipboardhandler = null;
		
		Thread caretthread;
	
		int maxVisibleChar;
		int maxVisibleLine;
		
		bool caretchanged       = false;
		bool autoClearSelection = false;
		
		int oldCaretOffset = 0;
		int oldCaretLineNr = 0;
		
//		[System.Runtime.InteropServices.DllImport("winmm.dll")]
//		public static extern long mciSendString(string lpszCommand, 
//		                                       string lpszReturnString,
//		                                       long cchReturn,
//		                                       long hwndCallback);
//		static int nextTypeWriterChannel = -1;
		
		/// <summary>
		/// This hashtable contains all editor keys, where
		/// the key is the key combination and the value the
		/// action.
		/// </summary>
		protected Hashtable editactions = new Hashtable();
		
		ArrayList lastDrawnBookmarks;
		
		// it is possible to define an own dialog key processor
		public delegate bool DialogKeyProcessor(Keys keyData);
		public DialogKeyProcessor ProcessDialogKeyProcessor;
		
		string currentFileName = null;
		
//		static TextAreaControl()
//		{
//			foreach (string str in SharpDevelopMain.CommandLineArgs) {
//				if (str == "IWantATypeWriter") {
//					nextTypeWriterChannel = 0;
//					string dir = Application.StartupPath +
//								 Path.DirectorySeparatorChar + ".." +
//								 Path.DirectorySeparatorChar + "data" +
//								 Path.DirectorySeparatorChar + "resources" +
//								 Path.DirectorySeparatorChar + "sounds" +
//								 Path.DirectorySeparatorChar;
//					for (int i = 0; i < 10; ++i) {
//						mciSendString("open \"" + dir + "type.wav\" type waveaudio alias typeWriter" + i, null, 0, 0);
//						mciSendString("open \"" + dir + "return.wav\" type waveaudio alias returnSound" + i, null, 0, 0);
//						mciSendString("open \"" + dir + "back.wav\" type waveaudio alias backSound" + i, null, 0, 0);
//					}
//				}
//			}
//		}
		
		public string FileName {
			get {
				return currentFileName;
			}
			set {
				if (currentFileName != value) {
					currentFileName = value;
					OnFileNameChanged(EventArgs.Empty);
				}
			}
		}
		
		/// <value>
		/// Used for edit actions, if set false the selection won't
		/// be cleared if the caret moves.
		/// </value>
		public bool AutoClearSelection {
			get {
				return autoClearSelection;
			}
			set {
				autoClearSelection = value;
			}
		}	
		
		/// <value>
		/// The visual line number of the first visible line in the 
		/// text area.
		/// </value>
		public int FirstVisibleColumn {
			get {
				return vscrollbar.Value;
			}
			set {
				vscrollbar.Value = value;
			}
		}
		
		/// <value>
		/// The visual row number of the first visible row in the 
		/// text area.
		/// </value>
		public int FirstVisibleRow {
			get {
				return hscrollbar.Value;
			}
			set {
				hscrollbar.Value = value;
			}
		}
		
		/// <value>
		/// The maximal visible row on the screen
		/// </value>
		public int MaxVisibleChar {
			get {
				return maxVisibleChar;
			}
		}
		
		/// <value>
		/// The maximal visible line on the screen
		/// </value>
		public int MaxVisibleLine {
			get {
				return maxVisibleLine;
			}
		}
		
		public bool IsReadOnly {
			get {
				return Document.ReadOnly;
			}
			set {
				Document.ReadOnly = value;
			}
		}
		
		public TextAreaPainter TextAreaPainter {
			get {
				return textarea;
			}
		}
		
		public PrintDocument PrintDocument {
			get { 
				return textarea.PrintDocument;
			}
		}
		
		public IDocumentAggregator Document {
			get {
				return textarea.Document;
			}
		}
		
		public IClipboardHandler ClipboardHandler {
			get {
				return clipboardhandler;
			}
		}
		
		public UndoStack UndoStack {
			get {
				return Document.UndoStack;
			}
		}
		
		public IProperties Properties {
			get {
				return textarea.Properties;
			}
		}
		
		public bool HasSomethingSelected {
			get {
				return Document.SelectionCollection.Count > 0;
			}
		}
		
		public string TextContent {
			get {
				return Document.TextContent;
			}
			set {
				Document.TextContent = value;
			}
		}
				
		// C# properties for some of the Editor Properties
		// These allow someone to use the control and set some of the properties without knowing anything about SharpDevelop
		public bool ShowSpaces
		{
			get { 
				return Properties.GetProperty("ShowSpaces", false); 
			}
			set { 
				Properties.SetProperty("ShowSpaces", value); 
			}
		}

		public bool ShowTabs
		{
			get { 
				return Properties.GetProperty("ShowTabs", false); 
			}
			set { 
				Properties.SetProperty("ShowTabs", value); 
			}
		}

		public bool ShowEOLMarkers
		{
			get { 
				return Properties.GetProperty("ShowEOLMarkers", false); 
			}
			set { 
				Properties.SetProperty("ShowEOLMarkers", value); 
			}
		}

		public bool ShowHRuler
		{
			get { 
				return Properties.GetProperty("ShowHRuler", false); 
			}
			set { 
				Properties.SetProperty("ShowHRuler", value); 
			}
		}
		
		public bool ShowVRuler
		{
			get { 
				return Properties.GetProperty("ShowVRuler", false); 
			}
			set { 
				Properties.SetProperty("ShowVRuler", value); 
			}
		}

		public bool ShowLineNumbers
		{
			get { 
				return Properties.GetProperty("ShowLineNumbers", true); 
			}
			set { 
				Properties.SetProperty("ShowLineNumbers", value); 
			}
		}

		public bool ShowInvalidLines
		{
			get { 
				return Properties.GetProperty("ShowInvalidLines", true); 
			}
			set { 
				Properties.SetProperty("ShowInvalidLines", value); 
			}
		}
		
		public bool EnableFolding
		{
			get { 
				return Properties.GetProperty("EnableFolding", true); 
			}
			set { 
				Properties.SetProperty("EnableFolding", value); 
			}
		}
		
		public int TabIndent
		{
			get { 
				return Properties.GetProperty("TabIndent", 4); 
			}
			set { 
				Properties.SetProperty("TabIndent", value); 
			}
		}
		
		public void Undo()
		{
			this.UndoStack.Undo();
		}
		public void Redo()
		{
			this.UndoStack.Redo();
		}
		

		public TextAreaControl() : this(new DocumentAggregatorFactory().CreateDocument())
		{
		}
		
		public TextAreaControl(IDocumentAggregator document)
		{
			textarea = new TextAreaPainter(this, document);
			gutter = new Gutter(textarea);
			
			clipboardhandler  = new TextAreaClipboardHandler(this);
			mouseeventhandler = new TextAreaMouseHandler(this);

			Controls.Add(textarea);
			Controls.Add(rowpanel);
			Controls.Add(gutter);
			Controls.Add(vscrollbar);
			Controls.Add(hscrollbar);			
			
			vscrollbar.ValueChanged += new EventHandler(ScrollVScrollBar);
			hscrollbar.ValueChanged += new EventHandler(ScrollHScrollBar);
			
			Resize += new System.EventHandler(ResizePanelEvent);
			
			gutter.Location = new Point(0, 0);
			
			textarea.Location = new Point(gutter.Width, 0);
			textarea.Resize += new System.EventHandler(CalculateNewWindowBounds);
			
			Document.DocumentChanged += new DocumentAggregatorEventHandler(BufferChange);
			Document.DocumentChanged += new DocumentAggregatorEventHandler(DirtyChangedEvent);
			
			textarea.KeyPress += new KeyPressEventHandler(KeyPressed);
			
			document.Caret.OffsetChanged += new CaretEventHandler(NewCaretPos);
			
#if BuildAsStandalone
			 //initialize the document properly
			document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();
#endif
			ResizeRedraw  = false;
			
			ruler = new Ruler(textarea);
			rowpanel.Controls.Add(ruler);

			Document.BookmarkManager.BeforeChanged += new EventHandler(OnBeforeChangedBookMarks);
			Document.BookmarkManager.Changed += new EventHandler(OnChangedBookMarks);
			
			mouseeventhandler.Attach(this);
			caretthread = new Thread(new ThreadStart(CaretThreadMethod));
			caretthread.IsBackground  = true;
			caretthread.Start();
			BufferChange(null, null);
			OptionsChanged();
			GenerateDefaultActions();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				try {
					caretthread.Abort();
				} catch (Exception) {}
			}
			base.Dispose(disposing);
		}
		
		void GenerateDefaultActions()
		{
			editactions[Keys.Left] = new CaretLeft();
			editactions[Keys.Left | Keys.Shift] = new ShiftCaretLeft();
			editactions[Keys.Left | Keys.Control] = new WordLeft();
			editactions[Keys.Left | Keys.Control | Keys.Shift] = new ShiftWordLeft();
			editactions[Keys.Right] = new CaretRight();
			editactions[Keys.Right | Keys.Shift] = new ShiftCaretRight();
			editactions[Keys.Right | Keys.Control] = new WordRight();
			editactions[Keys.Right | Keys.Control | Keys.Shift] = new ShiftWordRight();
			editactions[Keys.Up] = new CaretUp();
			editactions[Keys.Up | Keys.Shift] = new ShiftCaretUp();
			editactions[Keys.Up | Keys.Control] = new ScrollLineUp();
			editactions[Keys.Down] = new CaretDown();
			editactions[Keys.Down | Keys.Shift] = new ShiftCaretDown();
			editactions[Keys.Down | Keys.Control] = new ScrollLineDown();
			
			editactions[Keys.Insert] = new ToggleEditMode();
			editactions[Keys.Insert | Keys.Control] = new Copy();
			editactions[Keys.Insert | Keys.Shift] = new Paste();
			editactions[Keys.Delete] = new Delete();
			editactions[Keys.Delete | Keys.Shift] = new Cut();
			editactions[Keys.Home] = new Home();
			editactions[Keys.Home | Keys.Shift] = new ShiftHome();
			editactions[Keys.Home | Keys.Control] = new MoveToStart();
			editactions[Keys.Home | Keys.Control | Keys.Shift] = new ShiftMoveToStart();
			editactions[Keys.End] = new End();
			editactions[Keys.End | Keys.Shift] = new ShiftEnd();
			editactions[Keys.End | Keys.Control] = new MoveToEnd();
			editactions[Keys.End | Keys.Control | Keys.Shift] = new ShiftMoveToEnd();
			editactions[Keys.PageUp] = new MovePageUp();
			editactions[Keys.PageUp | Keys.Shift] = new ShiftMovePageUp();
			editactions[Keys.PageDown] = new MovePageDown();			
			editactions[Keys.PageDown | Keys.Shift] = new ShiftMovePageDown();

			editactions[Keys.Return] = new Return();
			editactions[Keys.Tab] = new Tab();
			editactions[Keys.Tab | Keys.Shift] = new ShiftTab();
			editactions[Keys.Back] = new Backspace();
			editactions[Keys.Back | Keys.Shift] = new Backspace();

			editactions[Keys.X | Keys.Control] = new Cut();
			editactions[Keys.C | Keys.Control] = new Copy();
			editactions[Keys.V | Keys.Control] = new Paste();

			editactions[Keys.A | Keys.Control] = new SelectWholeDocument();
			editactions[Keys.Escape] = new ClearAllSelections();

			editactions[Keys.Divide | Keys.Control] = new CommentAction();
			editactions[Keys.OemQuestion | Keys.Control] = new CommentAction();
			
			editactions[Keys.OemPipe | Keys.Control] = new UncommentAction();
			
			editactions[Keys.Back | Keys.Alt]  = new Actions.Undo();
			editactions[Keys.Z | Keys.Control] = new Actions.Undo();
			editactions[Keys.Y | Keys.Control] = new Redo();
			
			editactions[Keys.Delete | Keys.Control] = new DeleteWord();
			editactions[Keys.Back | Keys.Control]   = new WordBackspace();
		}
		
		// supposedly this is the way to do it according to .NET docs, 
		// as opposed to setting the size in the constructor
		protected override Size DefaultSize {
			get {
				return new Size(100, 100);
			}
		}
				
		void OnBeforeChangedBookMarks(object sender, EventArgs e)
		{
			lastDrawnBookmarks = (ArrayList)Document.BookmarkManager.Marks.Clone();
		}
		
		void OnChangedBookMarks(object sender, EventArgs e)
		{
			foreach (int line in lastDrawnBookmarks)  {
				UpdateLines(line, line);
			}
			foreach (int line in Document.BookmarkManager.Marks) {
				UpdateLines(line, line);
			}
		}
		
		/// <returns>
		/// The correct with of the gutter panel
		/// </returns>
		int CalculateGutterWidth()
		{
			bool showLineNumbers = Properties.GetProperty("ShowLineNumbers", true);
			bool folding         = Properties.GetProperty("EnableFolding", true); 
			int  lineSize        = (folding ? 0 : 8) +(int)((Math.Max(3, (int)(Math.Log10(Document.TotalNumberOfLines) + 1))) * textarea.FontWidth);
			int  foldingSize     = 15;
			return (int)((folding ? foldingSize : 0) + (showLineNumbers ? lineSize : 0));
		}
		
		public void OptionsChanged()
		{
			textarea.CalculateFontSize();
			rowpanel.Visible = Properties.GetProperty("ShowHRuler", false);
			
			gutter.Width   = CalculateGutterWidth();
			
			ResizePanelEvent(null, null);
			CalculateNewWindowBounds(null, null);
			Invalidate(true);
		}
		
		delegate void InvalidatePosDelegate(int offset);
		delegate void UpdateDelegate();
		
		/// <summary>
		/// This method blinks the caret
		/// </summary>
		void CaretThreadMethod()
		{
			try {
				while (true) {
					bool visibilityChanged = false;
					
					if (textarea.IHaveTheFocus) {
						Document.Caret.Visible = !Document.Caret.Visible;
						visibilityChanged = true;
					} else if (Document.Caret.Visible) {
						Document.Caret.Visible = false;
						visibilityChanged = true;
					}
					
					if (visibilityChanged) {
						textarea.Invoke(new InvalidatePosDelegate(textarea.InvalidateOffset), new object[] {Document.Caret.Offset});
						textarea.Invoke(new UpdateDelegate(textarea.Update));
					}
					
					Thread.Sleep(400);
				}
			} catch (ThreadAbortException) {
			} catch (Exception e) {
				Console.WriteLine("Caret thread got unexpected exception :\n" + e.ToString());
			}
		}
		
		public string GetWordBeforeCaretToWs()
		{
			int offset = Document.Caret.Offset;
			LineSegment line = Document.GetLineSegmentForOffset(offset);
			
			while (offset > line.Offset && (Document.GetCharAt(offset - 1) == '.' || Document.GetCharAt(offset - 1) == '_' || Char.IsLetterOrDigit(Document.GetCharAt(offset - 1)))) {
				--offset;
			}
			
			return Document.GetText(offset, Document.Caret.Offset - offset);
		}
		
		public string GetWordBeforeCaret()
		{
			int start = TextUtilities.FindPrevWordStart(Document, Document.Caret.Offset);
			return Document.GetText(start, Document.Caret.Offset - start);
		}
		
		public int DeleteWordBeforeCaret()
		{
			int start = TextUtilities.FindPrevWordStart(Document, Document.Caret.Offset);
			Document.Remove(start, Document.Caret.Offset - start);
			return start;
		}
		
		public void InsertChar(char ch)
		{
			BeginUpdate();
			if ((DocumentSelectionMode)Properties.GetProperty("SelectionMode", DocumentSelectionMode.Normal) == DocumentSelectionMode.Normal &&
			    Document.SelectionCollection.Count > 0) {
				Document.Caret.Offset = Document.SelectionCollection[0].Offset;
			    RemoveSelectedText();
			}
			
			Document.Insert(Document.Caret.Offset, ch.ToString());
			EndUpdate();
			
			int lineNr = Document.GetLineNumberForOffset(Document.Caret.Offset);
			UpdateLineToEnd(lineNr, Document.Caret.Offset - Document.GetLineSegment(lineNr).Offset);
			++Document.Caret.Offset;
			
			// I prefer to set NOT the standard column, if you type something
//			++Document.Caret.DesiredColumn;
		}
		
		public void InsertString(string str)
		{
			BeginUpdate();
			if ((DocumentSelectionMode)Properties.GetProperty("SelectionMode", DocumentSelectionMode.Normal) == DocumentSelectionMode.Normal &&
			    Document.SelectionCollection.Count > 0) {
				Document.Caret.Offset = Document.SelectionCollection[0].Offset;
			    RemoveSelectedText();
			}
			
			Document.Insert(Document.Caret.Offset, str);
			EndUpdate();
			
			int lineNr = Document.GetLineNumberForOffset(Document.Caret.Offset);
			UpdateLineToEnd(lineNr, Document.Caret.Offset - Document.GetLineSegment(lineNr).Offset);
			Document.Caret.Offset += str.Length;
//			Document.Caret.DesiredColumn += str.Length;
		}
		
		public void ReplaceChar(char ch)
		{
			BeginUpdate();
			if ((DocumentSelectionMode)Properties.GetProperty("SelectionMode", DocumentSelectionMode.Normal) == DocumentSelectionMode.Normal &&
			    Document.SelectionCollection.Count > 0) {
				Document.Caret.Offset = Document.SelectionCollection[0].Offset;
			    RemoveSelectedText();
			}
			
			int lineNr   = Document.GetLineNumberForOffset(Document.Caret.Offset);
			LineSegment  line = Document.GetLineSegment(lineNr);
			if (Document.Caret.Offset < line.Offset + line.Length) {
				Document.Replace(Document.Caret.Offset, 1, ch.ToString());
			} else {
				Document.Insert(Document.Caret.Offset, ch.ToString());
			}
			EndUpdate();
			
			UpdateLineToEnd(lineNr, Document.Caret.Offset - Document.GetLineSegment(lineNr).Offset);
			++Document.Caret.Offset;
//			++Document.Caret.DesiredColumn;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			textarea.Focus();
		}
		
		/// <remarks>
		/// This method is called on each Keypress
		/// </remarks>
		/// <returns>
		/// True, if the key is handled by this method and should NOT be
		/// inserted in the textarea.
		/// </returns>
		protected virtual bool HandleKeyPress(char ch)
		{
			return false;
		}
		
		public void KeyPressed(object sender, KeyPressEventArgs e)
		{
			if (Document.ReadOnly) {
				return;
			}
			
			char ch = e.KeyChar;
			if (ch < ' ') {
				return;
			}
//			if (nextTypeWriterChannel >= 0) {
//				mciSendString("stop typeWriter" + nextTypeWriterChannel, null, 0, 0);
//				mciSendString("seek typeWriter" + nextTypeWriterChannel + " to start", null, 0, 0); 
//				mciSendString("play typeWriter" + nextTypeWriterChannel, null, 0, 0);
//				nextTypeWriterChannel = (nextTypeWriterChannel + 1) % 10;
//			}
			
			switch (ch) {
				default:       // INSERT char
					if (!HandleKeyPress(ch)) {
						switch (Document.Caret.CaretMode) {
							case CaretMode.InsertMode:
								InsertChar(ch);
								break;
							case CaretMode.OverwriteMode:
								ReplaceChar(ch);
								break;
							default:
								Debug.Assert(false, "Unknown caret mode " + Document.Caret.CaretMode);
								break;
						}
					}
					break;
			}
			
			int currentLineNr = Document.GetLineNumberForOffset(Document.Caret.Offset);
			int delta = Document.FormattingStrategy.FormatLine(currentLineNr, Document.Caret.Offset, ch);
			if (delta != 0) {
				UpdateLines(currentLineNr, currentLineNr);
			}
		}
		
		/// <remarks>
		/// This method executes a dialog key
		/// </remarks>
		public bool ExecuteDialogKey(Keys keyData)
		{
			Console.WriteLine(keyData);
			
//			if (nextTypeWriterChannel >= 0) {
//				if (keyData == Keys.Return) {
//					mciSendString("stop returnSound" + nextTypeWriterChannel, null, 0, 0);
//					mciSendString("seek returnSound" + nextTypeWriterChannel + " to start", null, 0, 0); 
//					mciSendString("play returnSound" + nextTypeWriterChannel, null, 0, 0);
//					nextTypeWriterChannel = (nextTypeWriterChannel + 1) % 10;
//				} 
//				
//				if (keyData == Keys.Back) {
//					mciSendString("stop backSound" + nextTypeWriterChannel, null, 0, 0);
//					mciSendString("seek backSound" + nextTypeWriterChannel + " to start", null, 0, 0); 
//					mciSendString("play backSound" + nextTypeWriterChannel, null, 0, 0);
//					nextTypeWriterChannel = (nextTypeWriterChannel + 1) % 10;
//				}
//			}
			
			// try, if a dialog key processor was set to use this
			if (ProcessDialogKeyProcessor != null && ProcessDialogKeyProcessor(keyData)) {
				return true;
			}
			
			// if not (or the process was 'silent', use the standard edit actions
			object action = editactions[keyData];
			autoClearSelection = true;
			if (action != null) {
				lock (Document) {
					((IEditAction)action).Execute(this);
					if (HasSomethingSelected && autoClearSelection && caretchanged) {
						if ((DocumentSelectionMode)Properties.GetProperty("SelectionMode", DocumentSelectionMode.Normal) == DocumentSelectionMode.Normal) {
							ClearSelection();
						}
					}
				}
				return true;
			} 
			return false;
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
		{
			return ExecuteDialogKey(keyData) || base.ProcessDialogKey(keyData);
		}
		
		void NewCaretPos(object sender, CaretEventArgs e)
		{
			caretchanged = true;
			if (doUpdate) {
				return;
			}
			int newLineNr = Document.GetLineNumberForOffset(Document.Caret.Offset);
			
			LineSegment newLine = Document.GetLineSegment(newLineNr);
			Point        newPos = Document.OffsetToView(Document.Caret.Offset);
			
			Document.Caret.Visible = true;

			switch ((LineViewerStyle)Properties.GetProperty("LineViewerStyle", LineViewerStyle.None)) {
				case LineViewerStyle.None:
					if (oldCaretOffset <= Document.TextLength) {
						textarea.InvalidateOffset(oldCaretOffset);
					}
					textarea.InvalidateOffset(Document.Caret.Offset);
					break;
				case LineViewerStyle.FullRow:
					if (newLineNr == oldCaretLineNr) {
						goto case LineViewerStyle.None;
					}
					InvalidateLines(0, oldCaretLineNr, oldCaretLineNr);
					InvalidateLines(0, newLineNr, newLineNr);
					break;
			}
			
			ScrollToCaret();
			oldCaretOffset = Document.Caret.Offset;
			oldCaretLineNr = newLineNr;
		}
		
		bool doUpdate = false;
		public bool IsUpdating  {
			get {
				return doUpdate;
			}
		}
		public void BeginUpdate()
		{
			doUpdate = true;
		}
		public void EndUpdate()
		{
			doUpdate = false;
			if (caretchanged) {
				NewCaretPos(null, null);
			}
		}
		
		public void BufferChange(object sender, DocumentAggregatorEventArgs e)
		{
			ResizeTextArea();
			
			// do buffer refresh (SPAN insertion etc.)
			// the document update flags have higher priority than
			// the doUpdate flag
			if (textarea.Document.UpdateDocumentRequested) {
				textarea.Document.UpdateDocumentRequested = textarea.Document.UpdateCaretLineRequested = false;
				textarea.Refresh();
			} else if (textarea.Document.UpdateCaretLineRequested){
				textarea.Document.UpdateCaretLineRequested = false;
				Point caretPos = Document.OffsetToView(Document.Caret.Offset);
				bool oldUpdate = doUpdate;
				doUpdate = false;
				UpdateLine(caretPos.Y);
				doUpdate = oldUpdate;
			}
		}
		
		void ResizeTextArea()
		{
			int newwidth = CalculateGutterWidth();
			
			if (newwidth != gutter.Width) {
				gutter.Width = newwidth;
				ResizePanelEvent(null, null);
			}
			CalculateNewWindowBounds(null, null);
		}
		
		void DirtyChangedEvent(object sender, DocumentAggregatorEventArgs e)
		{
			OnChanged(e);
		}	
		
		private void ScrollVScrollBar(object sender, EventArgs e) 
		{
			int vAbsPos  = (int)((vscrollbar.Value - vscrollbar.Minimum) * textarea.FontHeight);
			gutter.VirtualTop = vAbsPos;
			gutter.Invalidate();
			textarea.ScreenVirtualTop = vAbsPos;
			textarea.Invalidate();
		}
		
		private void ScrollHScrollBar(object sender, EventArgs e) 
		{
			int hAbsPos   = (int)((hscrollbar.Value - hscrollbar.Minimum) * textarea.FontWidth);
			textarea.ScreenVirtualLeft = hAbsPos;
			textarea.Invalidate();
		}
		
		public void ResizePanelEvent(object sender, EventArgs e)
		{
			ResizeTextArea();
			vscrollbar.Height = ClientSize.Height - SystemInformation.VerticalScrollBarWidth;
			
			vscrollbar.Width  = SystemInformation.VerticalScrollBarWidth;
			vscrollbar.Location = new Point (ClientSize.Width - SystemInformation.VerticalScrollBarWidth, 0);
			
			hscrollbar.Width  = ClientSize.Width  - SystemInformation.HorizontalScrollBarHeight;
			hscrollbar.Height = SystemInformation.HorizontalScrollBarHeight;
			hscrollbar.Location = new Point (0, ClientSize.Height - SystemInformation.HorizontalScrollBarHeight);
			
			rowpanel.Location = new Point(0, 0);
			rowpanel.Size = new Size(ClientSize.Width  - SystemInformation.VerticalScrollBarWidth, 
			                        (rowpanel.Visible ? SystemInformation.HorizontalScrollBarHeight : 0));
			
			gutter.Location = new Point(0, rowpanel.Visible ? rowpanel.Height : 0);
			gutter.Size = new Size(gutter.Width,
			                       ClientSize.Height - SystemInformation.HorizontalScrollBarHeight - (rowpanel.Visible ? rowpanel.Height : 0));

			textarea.Top  = rowpanel.Bottom;
			textarea.Left = gutter.Right;
			textarea.Size = new Size(ClientSize.Width  - SystemInformation.VerticalScrollBarWidth - gutter.Width ,
			                         ClientSize.Height - SystemInformation.HorizontalScrollBarHeight - (rowpanel.Visible ? rowpanel.Height : 0));
			ruler.Size = new Size(Math.Max(textarea.Width, textarea.Width) + gutter.Width, rowpanel.Height);
		}
		
		void CalculateNewWindowBounds(object sender, EventArgs e)
		{
			if (Height == 0 || Width == 0) {
				return;
			}
			maxVisibleChar = (int)((textarea.Width)  / textarea.FontWidth) - 3;
			maxVisibleLine = (int)((textarea.Height - (textarea.Height % (int)textarea.FontHeight)) / textarea.FontHeight);
			
			vscrollbar.Maximum = Document.TotalNumberOfLines + maxVisibleLine/2;
			hscrollbar.Maximum = 1000;
			vscrollbar.Minimum = 0;
			hscrollbar.Minimum = 0;
						
			vscrollbar.LargeChange = maxVisibleLine; // set the large change property to scroll a whole page
		}
		
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			int newvalue;
			
			int MAX_DELTA  = 120; // basically it's constant now, but could be changed later by MS
			int multiplier = Math.Abs(e.Delta) / MAX_DELTA;
			
			if (System.Windows.Forms.SystemInformation.MouseWheelScrollLines > 0) {
				newvalue = vscrollbar.Value - (Properties.GetProperty("MouseWheelScrollDown", true) ? 1 : -1) * Math.Sign(e.Delta) * System.Windows.Forms.SystemInformation.MouseWheelScrollLines * multiplier;
			} else {
				newvalue = vscrollbar.Value - (Properties.GetProperty("MouseWheelScrollDown", true) ? 1 : -1) * Math.Sign(e.Delta) * vscrollbar.LargeChange;
			}
			vscrollbar.Value = Math.Max(vscrollbar.Minimum, Math.Min(vscrollbar.Maximum, newvalue));
		}
		int scrollwidth = 2;
		
		public int ScrollLineHeight {
			get {
				return scrollwidth;
			}
			set {
				scrollwidth = value;
			}
		}
		public void ScrollToCaret()
		{
			int curCharMin  = hscrollbar.Value - hscrollbar.Minimum;
			int curCharMax  = curCharMin + maxVisibleChar;
			
			int pos         = Document.OffsetToView(Document.Caret.Offset).X;
			
			if (pos < curCharMin) {
				hscrollbar.Value = hscrollbar.Minimum + Math.Max(0, pos - scrollwidth);
			} else {
				if (pos > curCharMax) {
					hscrollbar.Value = Math.Min(hscrollbar.Maximum, 
						                        pos - maxVisibleChar + scrollwidth + hscrollbar.Minimum);
				}
			}
			ScrollTo(Document.GetLineNumberForOffset(Document.Caret.Offset));
		}
		
		public int GetMaxScroll()
		{
			return vscrollbar.Maximum;
		}
		
		public void ScrollTo(int line)
		{
			line = Document.GetLogicalLine(line);
			int curLineMin = vscrollbar.Value - vscrollbar.Minimum;
			
			if (line - scrollwidth < curLineMin) {
				vscrollbar.Value =  Math.Max(vscrollbar.Minimum, line - scrollwidth + vscrollbar.Minimum);
			} else {
				int curLineMax = curLineMin + maxVisibleLine;
				if (line + scrollwidth > curLineMax) {
					vscrollbar.Value = Math.Min(vscrollbar.Maximum, line - maxVisibleLine + scrollwidth + vscrollbar.Minimum);
				}
			}
		}
		
		public override void Refresh()
		{
			if (doUpdate) {
				return;
			}
			base.Refresh();
		}
		
		public void UpdateLine(int line)
		{
			UpdateLines(0, line, line);
		}
		
		public void UpdateLines(int lineBegin, int lineEnd)
		{
			UpdateLines(0, lineBegin, lineEnd);
		}
	
		public void UpdateToEnd(int lineBegin) 
		{
			int firstLine = vscrollbar.Value;
			lineBegin     = Math.Max(lineBegin, vscrollbar.Value);
			
			UpdateLines(0, lineBegin +  maxVisibleLine);
		}

		public void UpdateLineToEnd(int lineNr, int xStart)
		{
			UpdateLines(xStart, lineNr, lineNr);
		}
		
		
		public void UpdateLine(int line, int begin, int end)
		{
			UpdateLines(line, line);
		}
	
		public void UpdateLines(int xPos, int lineBegin, int lineEnd)
		{
			lineBegin = lineBegin;
			lineEnd   = lineEnd;
			InvalidateLines((int)(xPos * textarea.FontWidth), lineBegin, lineEnd);
		}
		
		public void InvalidateLines(int xPos, int lineBegin, int lineEnd)
		{
			if (doUpdate) {
				return;
			}
			int firstLine = vscrollbar.Value;
			lineBegin     = Math.Max(Document.GetLogicalLine(lineBegin), vscrollbar.Value);
			lineEnd       = Math.Min(lineEnd,   vscrollbar.Value + maxVisibleLine);
			int y         = Math.Max(    0, (int)(lineBegin  * textarea.FontHeight));
			int height    = Math.Min(textarea.Height, (int)((1 + lineEnd - lineBegin) * (textarea.FontHeight + 1)));
			int xScreen = xPos - textarea.ScreenVirtualLeft;
			textarea.Invalidate(new Rectangle(xScreen, y - 1 - textarea.ScreenVirtualTop, textarea.Width - xScreen, height + 3));
		}
		
		public void InvalidateLines(int from, int length, int lineBegin, int lineEnd)
		{
			if (doUpdate) {
				return;
			}
			int firstLine = vscrollbar.Value;
			lineBegin     = Math.Max(lineBegin, vscrollbar.Value);
			lineEnd       = Math.Min(lineEnd,   vscrollbar.Value + maxVisibleLine);
			int y         = Math.Max(    0, (int)(lineBegin  * textarea.FontHeight));
			int height    = Math.Min(textarea.Height, (int)((1 + lineEnd - lineBegin) * (textarea.FontHeight + 1)));
			textarea.Invalidate(new Rectangle((int)(textarea.FontWidth * from) - textarea.ScreenVirtualLeft, y - 1 - textarea.ScreenVirtualTop, (int)(textarea.FontWidth * length), height + 3));
		}
		
		public void SetSelection(ISelection selection)
		{
			autoClearSelection = false;
			ClearSelection();
	
			if (selection != null) {
				Document.SelectionCollection.Add(selection);
				UpdateLines(selection.StartLine, selection.EndLine);
			}
		}
		
		public void ClearSelection()
		{
			while (Document.SelectionCollection.Count > 0) {
				ISelection s = Document.SelectionCollection[Document.SelectionCollection.Count - 1];
				Document.SelectionCollection.RemoveAt(Document.SelectionCollection.Count - 1);
				UpdateLines(s.StartLine, s.EndLine);
			}
		}
		
		public string GetSelectedText()
		{
			StringBuilder builder = new StringBuilder();
			
			PriorityQueue queue = new PriorityQueue();
			foreach (ISelection s in Document.SelectionCollection) {
				queue.Insert(-s.Offset, s);
			}
			while (queue.Count > 0) {
				ISelection s = ((ISelection)queue.Remove());
				builder.Append(s.SelectedText);
			}
			return builder.ToString();
		}
		
		public bool IsSelected(int offset)
		{
			return GetSelectionAt(offset) != null;
		}
		
		public ISelection GetSelectionAt(int offset)
		{
			return GetSelectionBetween(offset, offset);
		}
		
		ISelection GetSelectionBetween(int offset, int offset2)
		{
			int min = Math.Min(offset, offset2);
			int max = Math.Max(offset, offset2);
			ISelection ti = new DefaultSelection(Document, min, max - min);
		
			foreach (ISelection s in Document.SelectionCollection) {
				if (SelectionsOverlap(ti, s)) {
					return s;
				}
			}
			return null;
		}
		
		public void ExtendSelection(int oldOffset, int newOffset)
		{
			autoClearSelection = false;
			ISelection s = GetSelectionBetween(oldOffset, newOffset);
			int min = Math.Min(oldOffset, newOffset);
			int max = Math.Max(oldOffset, newOffset);
			if (s != null) {
				int oldEndOffset = s.Offset + s.Length;
				if (oldOffset <= s.Offset) {
					if (newOffset < oldEndOffset) {
						s.Length = oldEndOffset - newOffset;
						s.Offset = newOffset;
					} else {
						s.Offset = oldEndOffset;
						s.Length = newOffset - oldEndOffset;
					}
				} else  {
					if (newOffset > s.Offset) {
						s.Length = newOffset - s.Offset;
					} else {
						s.Length = s.Offset - newOffset;
						s.Offset = newOffset;
					}
				}
				if (s.Length == 0) {
					Document.SelectionCollection.Remove(s);
				}
				UpdateLines(Document.GetLineNumberForOffset(min), Document.GetLineNumberForOffset(max));
			} else {
				if ((DocumentSelectionMode)Properties.GetProperty("SelectionMode", DocumentSelectionMode.Normal) == DocumentSelectionMode.Normal) {
					ClearSelection();
				}
				
				AddToSelection(new DefaultSelection(Document, min, max - min));
			}
		} 
		
		public void AddToSelection(ISelection selection)
		{
			autoClearSelection = false;
			if (selection != null) {
				InternalAddToSelection(selection);
			}
		}
		
		public void CheckCaretPos()
		{
			int newOffset = Math.Max(0, Math.Min(Document.Caret.Offset, Document.TextLength));
			if (newOffset != Document.Caret.Offset) {
				Document.Caret.Offset = newOffset;
			}
		}
		
		public void RemoveSelectedText()
		{
			ArrayList lines = new ArrayList();
			BeginUpdate();
			int offset = -1;
			bool oneLine = true;
			PriorityQueue queue = new PriorityQueue();
			foreach (ISelection s in Document.SelectionCollection) {
				queue.Insert(-s.Offset, s);
			}
			while (queue.Count > 0) {
				ISelection s = ((ISelection)queue.Remove());
				if (oneLine) {
					int lineBegin = s.StartLine;
					if (lineBegin != s.EndLine) {
						oneLine = false;
					} else {
						lines.Add(lineBegin);
					}
				}
				offset = Document.Caret.Offset = s.Offset;
				Document.Remove(s.Offset, s.Length);
			}
			ClearSelection();
			EndUpdate();
			if (offset != -1) {
				if (oneLine) {
					foreach (int i in lines) {
						UpdateLine(i);
					}
				} else {
					Refresh();
				}
			}
		}
		
		void InternalAddToSelection(ISelection selection)
		{
			if (selection.Length == 0) {
				return;
			}
			foreach (ISelection s in Document.SelectionCollection) {
				// try and merge existing selections one by
				// one with the new selection
				if (SelectionsOverlap(s, selection)) {
					int newOffset = Math.Min(selection.Offset, s.Offset);
					int newLength = Math.Max(selection.Offset + selection.Length, s.Offset + s.Length) - newOffset;
					
					selection.Offset = newOffset;
					selection.Length = newLength;
					Document.SelectionCollection.Remove(s);
					break;
				}
			}
			Document.SelectionCollection.Add(selection);
			
			UpdateLines(selection.StartLine, selection.EndLine);
		}
		
		bool SelectionsOverlap(ISelection s1, ISelection s2)
		{
			return (s1.Offset <= s2.Offset && s2.Offset <= s1.Offset + s1.Length)                         ||
			       (s1.Offset <= s2.Offset + s2.Length && s2.Offset + s2.Length <= s1.Offset + s1.Length) ||
			       (s1.Offset >= s2.Offset && s1.Offset + s1.Length <= s2.Offset + s2.Length);
		}
		
		public void LoadFile(string fileName)
		{
			Document.UndoStack.ClearAll();
			Document.SelectionCollection.Clear();
			Document.BookmarkManager.Clear();
			
			StreamReader stream = new StreamReader(fileName);
			
			Document.TextContent = stream.ReadToEnd();
			stream.Close();
			Refresh();
			
			this.FileName = fileName;
		}
		
		public void SaveFile(string fileName)
		{
#if !BuildAsStandalone
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (propertyService.GetProperty("SharpDevelop.CreateBackupCopy", false)) {
				CreateBackupCopy(fileName);
			}
#endif
			
			StreamWriter stream = File.CreateText(fileName);
			LineTerminatorStyle lineTerminatorStyle = (LineTerminatorStyle)propertyService.GetProperty("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);
			
			foreach (LineSegment line in Document.LineSegmentCollection) {
				stream.Write(Document.GetText(line.Offset, line.Length));
				stream.Write(GetLineTerminatorString(lineTerminatorStyle));
			}
			stream.Close();
			
			this.FileName = fileName;
		}
		
		
		/// <summary>
		/// gets the line terminator string.
		/// </summary>
		string GetLineTerminatorString(LineTerminatorStyle lineTerminatorStyle)
		{
			switch (lineTerminatorStyle) {
				case LineTerminatorStyle.Windows:
					return "\r\n";
				case LineTerminatorStyle.Macintosh:
					return "\r";
				case LineTerminatorStyle.Unix:
					return "\n";
			}
			Debug.Assert(false, "Unknown line terminator style: " + lineTerminatorStyle);
			return null;
		}
		
		void CreateBackupCopy(string fileName) 
		{
			try {
				if (File.Exists(fileName)) {
					string backupName = fileName + ".bak";
					if (File.Exists(backupName)) {
						File.Delete(backupName);
					}
					File.Copy(fileName, backupName);
				}
			} catch (Exception e) {
				MessageBox.Show("Could not create backup copy of " + fileName +".\nReason : " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
		
		public void JumpTo(Point pos)
		{
			LineSegment line = Document.GetLineSegment(Math.Min(Document.TotalNumberOfLines - 1, Math.Max(0, pos.Y)));
			Document.Caret.Offset = line.Offset + Math.Min(Math.Max(0, pos.X), line.Length);
			Document.SetDesiredColumn();
			ScrollToCaret();
			textarea.Focus();
		}
		
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			if (FileNameChanged != null) {
				FileNameChanged(this, e);
			}
		}
		
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		public event EventHandler FileNameChanged;
		public event EventHandler Changed;
	}
}
