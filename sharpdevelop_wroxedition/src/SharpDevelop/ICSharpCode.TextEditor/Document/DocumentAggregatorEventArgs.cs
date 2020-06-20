// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.TextEditor.Document
{
	public delegate void DocumentAggregatorEventHandler(object sender, DocumentAggregatorEventArgs e);
	
	public class DocumentAggregatorEventArgs : EventArgs
	{
		IDocumentAggregator document;
		int       offset;
		int       length;
		string    text;
		
		/// <returns>
		/// always a valid Document which is related to the Event.
		/// </returns>
		public IDocumentAggregator Document {
			get {
				return document;
			}
		}
		
		/// <returns>
		/// -1 if no offset was specified for this event
		/// </returns>
		public int Offset {
			get {
				return offset;
			}
		}
		
		/// <returns>
		/// null if no text was specified for this event
		/// </returns>
		public string Text {
			get {
				return text;
			}
		}
		
		/// <returns>
		/// -1 if no length was specified for this event
		/// </returns>
		public int Length {
			get {
				return length;
			}
		}
		
		public DocumentAggregatorEventArgs(IDocumentAggregator document) : this(document, -1, -1, null)
		{
		}
		
		public DocumentAggregatorEventArgs(IDocumentAggregator document, int offset) : this(document, offset, -1, null)
		{
		}
		
		public DocumentAggregatorEventArgs(IDocumentAggregator document, int offset, int length) : this(document, offset, length, null)
		{
		}
		
		public DocumentAggregatorEventArgs(IDocumentAggregator document, int offset, int length, string text)
		{
			this.document = document;
			this.offset   = offset;
			this.length   = length;
			this.text     = text;
		}
	}
}
