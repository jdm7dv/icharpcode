// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// Default implementation of the <see cref="ICSharpCode.TextEditor.Document.ISelection"/> interface.
	/// </summary>
	public class DefaultSelection : ISelection, ISegment
	{
		int       offset   = -1;
		int       length   = -1;
		IDocumentAggregator document = null;
		bool      isRectangularSelection = false;
		
		public int Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		
		public int Length {
			get {
				return length;
			}
			set {
				length = value;
			}
		}
		
		public int StartLine {
			get {
				return document.GetLineNumberForOffset(offset);
			}
		}
		
		public int EndLine {
			get {
				return document.GetLineNumberForOffset(offset + Length);
			}
		}
		
		public bool IsEmpty {
			get {
				return offset < 0 || length <= 0;
			}
		}
		
		// TODO : make this unused property used.
		public bool IsRectangularSelection {
			get {
				return isRectangularSelection;
			}
			set {
				isRectangularSelection = value;
			}
		}
		
		public string SelectedText {
			get {
				if (document != null) {
					return document.GetText(offset, length);
				}
				return null;
			}
		}
		
		public DefaultSelection(IDocumentAggregator document, int offset, int length)
		{
			this.document = document;
			this.offset   = offset;
			this.length   = length;
		}
		
		public override string ToString()
		{
			return "[Selection : Offset = " + Offset + ", Length = " + Length + "]";
		}
		
	}
}
