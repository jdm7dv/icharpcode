// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.TextEditor.Document
{
	public delegate void LineManagerEventHandler(object sender,LineManagerEventArgs e);
	
	public class LineManagerEventArgs : EventArgs
	{
		IDocumentAggregator document;
		int       start;
		int       moved;
		
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
		public int LineStart {
			get {
				return start;
			}
		}
		
		/// <returns>
		/// -1 if no length was specified for this event
		/// </returns>
		public int LinesMoved {
			get {
				return moved;
			}
		}
		
		public LineManagerEventArgs(IDocumentAggregator document, int lineStart, int linesMoved)
		{
			this.document = document;
			this.start    = lineStart;
			this.moved    = linesMoved;
		}
	}
}
