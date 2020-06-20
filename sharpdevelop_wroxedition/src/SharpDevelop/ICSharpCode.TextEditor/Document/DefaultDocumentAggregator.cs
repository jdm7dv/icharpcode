// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.TextEditor.Undo;

namespace ICSharpCode.TextEditor.Document
{
	public enum LineViewerStyle {
		None,
		FullRow
	}
	
	public enum IndentStyle {
		None, 
		Auto, 
		Smart
	}
	
	public enum BracketHighlightingStyle {
		None,
		OnBracket, 
		AfterBracket
	}
	
	public enum DocumentSelectionMode {
		Normal,
		Additive
	}
	
	public class DefaultDocumentAggregator : IDocumentAggregator
	{	
		bool readOnly  = false;
		bool updateDocumentRequested = false;
		bool updateCaretLineRequested = false;
		
		UndoStack             undoStack            = null;
		ILineManager lineTrackingStrategy = null;
		IBookMarkManager      bookmarkManager      = null;
		ITextBufferStrategy   textBufferStrategy   = null;
		ICaret                caret                = null;
		IFormattingStrategy   formattingStrategy   = null;
		ITextModel            textModel            = null;
		IProperties           properties           = null;
		IFoldingStrategy      foldingStrategy      = null;
		
		SelectionCollection selectionCollection = new SelectionCollection();
		
		public IProperties Properties {
			get {
				return properties;
			}
			set {
				properties = value;
			}
		}
		
		public SelectionCollection SelectionCollection {
			get {
				return selectionCollection;
			}
		}
		
		public LineSegmentCollection LineSegmentCollection {
			get {
				return lineTrackingStrategy.LineSegmentCollection;
			}
		}
		
		public UndoStack UndoStack {
			get {
				return undoStack;
			}
			set {
				undoStack = value;
			}
		}
		
		public bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				readOnly = value;
			}
		}
		
		public bool UpdateDocumentRequested {
			get {
				return updateDocumentRequested;
			}
			set {
				updateDocumentRequested = value;
			}
		}
		
		public bool UpdateCaretLineRequested {
			get {
				return updateCaretLineRequested;
			}
			
			set {
				updateCaretLineRequested = value;
			}
		}
		

		public ICaret Caret {
			get {
				return caret;
			}
			set {
				caret = value;
			}
		}		
		
		public ILineManager LineManager {
			get {
				return lineTrackingStrategy;
			}
			set {
				lineTrackingStrategy = value;
			}
		}
		
		public ITextBufferStrategy TextBufferStrategy {
			get {
				return textBufferStrategy;
			}
			set {
				textBufferStrategy = value;
			}
		}
		
		public IFormattingStrategy FormattingStrategy {
			get {
				return formattingStrategy;
			}
			set {
				formattingStrategy = value;
			}
		}
		
		public IFoldingStrategy FoldingStrategy {
			get {
				return foldingStrategy;
			}
			set {
				foldingStrategy = value;
			}
		}
		
		public IHighlightingStrategy HighlightingStrategy {
			get {
				return lineTrackingStrategy.HighlightingStrategy;
			}
			set {
				lineTrackingStrategy.HighlightingStrategy = value;
			}
		}
		
		public ITextModel TextModel {
			get {
				return textModel;
			}
			set {
				textModel = value;
			}
		}
		
		public int TextLength {
			get {
				return textBufferStrategy.Length;
			}
		}
		
		public IBookMarkManager BookmarkManager {
			get {
				return bookmarkManager;
			}
			set {
				bookmarkManager = value;
			}
		}
		
		public string TextContent {
			get {
				return GetText(0, textBufferStrategy.Length);
			}
			set {
				Debug.Assert(Caret != null);
				Debug.Assert(SelectionCollection != null);
				Debug.Assert(textBufferStrategy != null);
				Debug.Assert(lineTrackingStrategy != null);
				
				OnDocumentAboutToBeChanged(new DocumentAggregatorEventArgs(this, 0, 0, value));
				textBufferStrategy.SetContent(value);
				lineTrackingStrategy.SetContent(value);
				caret.Offset = 0;
				SelectionCollection.Clear();
				OnDocumentChanged(new DocumentAggregatorEventArgs(this, 0, 0, value));				
			}
		}
		
		public void SetDesiredColumn()
		{
			LineSegment caretLine = lineTrackingStrategy.GetLineSegmentForOffset(Caret.Offset);
			Caret.DesiredColumn = TextModel.GetViewXPos(caretLine, Caret.Offset - caretLine.Offset);
		}
		
		public void SetCaretToDesiredColumn(LineSegment caretLine)
		{
			Caret.Offset = caretLine.Offset + TextModel.GetLogicalXPos(caretLine, Caret.DesiredColumn);
		}
		
		public void Insert(int offset, string text)
		{
			if (readOnly) {
				return;
			}
			OnDocumentAboutToBeChanged(new DocumentAggregatorEventArgs(this, offset, -1, text));
			DateTime time = DateTime.Now;
			textBufferStrategy.Insert(offset, text);
			
			time = DateTime.Now;
			lineTrackingStrategy.Insert(offset, text);
			
			time = DateTime.Now;
			if (Caret.Offset > offset) {
				Caret.Offset += text.Length;
			}
			
			foreach (ISelection selection in SelectionCollection) {
				if (selection.Offset > offset) {
					selection.Offset += text.Length;
				} else if (selection.Offset + selection.Length > offset) {
					selection.Length += text.Length;
				}
			}
			
			undoStack.Push(new UndoableInsert(this, offset, text));
			
			time = DateTime.Now;
			OnDocumentChanged(new DocumentAggregatorEventArgs(this, offset, -1, text));
		}
		
		public void Remove(int offset, int length)
		{
			if (readOnly) {
				return;
			}
			OnDocumentAboutToBeChanged(new DocumentAggregatorEventArgs(this, offset, length));
			undoStack.Push(new UndoableDelete(this, offset, GetText(offset, length)));
			
			textBufferStrategy.Remove(offset, length);
			lineTrackingStrategy.Remove(offset, length);
			
			if (Caret.Offset > offset) {
				Caret.Offset -= Math.Min(Caret.Offset - offset, length);
			}
			
			foreach (ISelection selection in selectionCollection) {
				if (selection.Offset > offset) {
					selection.Offset -= length;
				} else if (selection.Offset + selection.Length > offset) {
					selection.Length -= length;
				}
			}
			
			OnDocumentChanged(new DocumentAggregatorEventArgs(this, offset, length));
		}
		
		public void Replace(int offset, int length, string text)
		{
			if (readOnly) {
				return;
			}
			OnDocumentAboutToBeChanged(new DocumentAggregatorEventArgs(this, offset, length, text));
			undoStack.Push(new UndoableReplace(this, offset, GetText(offset, length), text));
			
			textBufferStrategy.Replace(offset, length, text);
			lineTrackingStrategy.Replace(offset, length, text);
			
			if (Caret.Offset > offset) {
				int caretDelta = Math.Min(Caret.Offset - offset, length);
				Caret.Offset = Caret.Offset - caretDelta + Math.Min(caretDelta, text.Length);
			}
			
			foreach (ISelection selection in selectionCollection) {
				if (selection.Offset > offset) {
					selection.Offset = selection.Offset - length + text.Length;
				} else if (selection.Offset + selection.Length > offset) {
					selection.Length = selection.Length - length + text.Length;
				}
			}
			
			OnDocumentChanged(new DocumentAggregatorEventArgs(this, offset, length, text));
		}
		
		public char GetCharAt(int offset)
		{
			return textBufferStrategy.GetCharAt(offset);
		}
		
		public string GetText(int offset, int length)
		{
			return textBufferStrategy.GetText(offset, length);
		}
		
		public int TotalNumberOfLines {
			get {
				return lineTrackingStrategy.TotalNumberOfLines;
			}
		}
		
		public int GetLineNumberForOffset(int offset)
		{
			return lineTrackingStrategy.GetLineNumberForOffset(offset);
		}
		
		public LineSegment GetLineSegmentForOffset(int offset)
		{
			return lineTrackingStrategy.GetLineSegmentForOffset(offset);
		}
		
		public LineSegment GetLineSegment(int line)
		{
			return lineTrackingStrategy.GetLineSegment(line);
		}
		
		public int GetLogicalLine(int lineNumber)
		{
			return lineTrackingStrategy.GetLogicalLine(lineNumber);
		}

		public int GetVisibleLine(int lineNumber)
		{
			return lineTrackingStrategy.GetVisibleLine(lineNumber);
		}
		
		public int GetNextVisibleLineAbove(int lineNumber, int lineCount)
		{
			return lineTrackingStrategy.GetNextVisibleLineAbove(lineNumber, lineCount);
		}
		
		public int GetNextVisibleLineBelow(int lineNumber, int lineCount)
		{
			return lineTrackingStrategy.GetNextVisibleLineBelow(lineNumber, lineCount);
		}
		
		public Point OffsetToView(int offset)
		{
			return textModel.OffsetToView(offset);
		}
		
		public int   ViewToOffset(Point p)
		{
			return textModel.ViewToOffset(p);
		}
		
		public int GetViewXPos(LineSegment line, int logicalXPos)
		{
			return textModel.GetViewXPos(line, logicalXPos);
			
		}
		
		public int GetLogicalXPos(LineSegment line, int viewXPos)
		{
			return textModel.GetLogicalXPos(line, viewXPos);
		}
		
		protected void OnDocumentAboutToBeChanged(DocumentAggregatorEventArgs e)
		{
			if (DocumentAboutToBeChanged != null) {
				DocumentAboutToBeChanged(this, e);
			}
		}
		
		protected void OnDocumentChanged(DocumentAggregatorEventArgs e)
		{
			if (DocumentChanged != null) {
				DocumentChanged(this, e);
			}
		}
		
		public event DocumentAggregatorEventHandler DocumentAboutToBeChanged;
		public event DocumentAggregatorEventHandler DocumentChanged;
	}
}
