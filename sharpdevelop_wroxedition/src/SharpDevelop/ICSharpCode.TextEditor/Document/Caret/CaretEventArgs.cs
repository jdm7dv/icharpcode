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
	public delegate void CaretEventHandler(object sender, CaretEventArgs e);
	
	public class CaretEventArgs : EventArgs
	{
		ICaret    caret;
		IDocumentAggregator document;
		
		public IDocumentAggregator Document {
			get {
				return document;
			}
		}
		
		public ICaret Caret {
			get {
				return caret;
			}
		}
		
		public CaretEventArgs(IDocumentAggregator document, ICaret caret) : base()
		{
			this.document = document;
			this.caret    = caret;
		}
	}
}
