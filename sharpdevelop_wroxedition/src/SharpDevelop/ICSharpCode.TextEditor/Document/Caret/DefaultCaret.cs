// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using ICSharpCode.TextEditor.Document;
using System.Text;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This class represents the text caret.
	/// </summary>
	public class DefaultCaret : ICaret
	{
		int offset        = 0;
		int desiredColumn = 0;
		
		CaretMode caretMode = CaretMode.InsertMode;
		bool      visible   = true;
		
		IDocumentAggregator document;
		
		public int Offset {
			get {
				return offset;
			}
			set {
				if (offset != value) {
					offset = value;
					OnOffsetChanged(new CaretEventArgs(document, this));
				}
			}
		}
		
		public int DesiredColumn {
			get {
				return desiredColumn;
			}
			set {
				desiredColumn = value;
			}
		}
		
		public CaretMode CaretMode {
			get {
				return caretMode;
			}
			set {
				caretMode = value;
				OnCaretModeChanged(new CaretEventArgs(document, this));
			}
		}
		
		public bool Visible {
			get {
				return visible;
			}
			set {
				visible = value;
			}
		}
		
		protected void OnOffsetChanged(CaretEventArgs e)
		{
			if (OffsetChanged != null) {
				OffsetChanged(this, e);
			}
		}
		
		protected void OnCaretModeChanged(CaretEventArgs e)
		{
			if (CaretModeChanged != null) {
				CaretModeChanged(this, e);
			}
		}
		
		public DefaultCaret(IDocumentAggregator document)
		{
			this.document = document;
		}
		
		public event CaretEventHandler OffsetChanged;
		public event CaretEventHandler CaretModeChanged;
		
		public override string ToString()
		{
			return "[Caret: Offset = " + Offset + ", DesiredColumn = " + DesiredColumn + ", InsertMode = " + CaretMode +", Visible = " + Visible + "]";
		}
	}
}
