// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.TextEditor.Actions
{
	/// <summary>
	/// This is the interface for all EditActions to the outside world :)
	/// </summary>
	public interface IEditActionHandler
	{		
		IClipboardHandler ClipboardHandler {
			get;
		}
		
		IDocumentAggregator Document {
			get;
		}
		
		int MaxVisibleLine {
			get;
		}
		
		void InsertChar(char ch);
		void ReplaceChar(char ch);
		
		// update stuff
		void BeginUpdate();
		void EndUpdate();
		
		void Refresh();
		void UpdateToEnd(int lineBegin);
		void UpdateLines(int lineBegin, int lineEnd);
		void UpdateLineToEnd(int lineNr, int xStart);
		
		// caret stuff
		void ScrollToCaret();
		
		// scroll stuff
		void ScrollTo(int line);
		
		int FirstVisibleColumn {
			get;
			set;
		}
		
		int FirstVisibleRow {
			get;
			set;
		}
		
		// selection stuff
		bool HasSomethingSelected {
			get;
		}
		bool AutoClearSelection {
			get;
			set;
		}
		
		void SetSelection(ISelection selection);
		void ExtendSelection(int oldOffset, int newOffset);
		void AddToSelection(ISelection selection);
		void RemoveSelectedText();
		void ClearSelection();
	}
}
