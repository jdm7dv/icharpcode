// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
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

using ICSharpCode.Core.Properties;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.Gui;

#if !BuildAsStandalone
using ICSharpCode.SharpDevelop.Gui.Pads;
#endif

namespace ICSharpCode.TextEditor
{
	public class TextAreaClipboardHandler : IClipboardHandler
	{
		TextAreaControl textAreaControl;
		
		public TextAreaClipboardHandler(TextAreaControl textAreaControl)
		{
			this.textAreaControl = textAreaControl;
		}
		
		public void Cut(object sender, EventArgs e)
		{
			string str = textAreaControl.GetSelectedText();
			
#if !BuildAsStandalone
			// quick hack : put in clipboard ring as 'static' variant, please make a 'clean'
			// design. see the same hack in the 'copy' method.
			SideBarView.PutInClipboardRing(str);
#endif
			textAreaControl.RemoveSelectedText();
			
			// paste to clipboard
			Clipboard.SetDataObject(new DataObject(DataFormats.Text, str), true);
		}
		
		public void Copy(object sender, EventArgs e)
		{
			string text = textAreaControl.GetSelectedText();
			
			if (text.Length > 0) {
#if !BuildAsStandalone
				// quick hack : put in clipboard ring as 'static' variant, please make a 'clean'
				// design. see the same hack in the 'cut' method.
				SideBarView.PutInClipboardRing(text);
#endif			
				Clipboard.SetDataObject(new DataObject(DataFormats.Text, text), true);
			}
		}
		
		public void Paste(object sender, EventArgs e)
		{
			IDataObject data = Clipboard.GetDataObject();
			if (data.GetDataPresent(DataFormats.Text)) {
				string text = (string)data.GetData(DataFormats.Text);
				if (!text.Equals("")) {
					int redocounter = 0;
					if (textAreaControl.HasSomethingSelected) {
						Delete(sender, e);
						redocounter++;
					}
					int offset = textAreaControl.Document.Caret.Offset;
					textAreaControl.BeginUpdate();
					textAreaControl.Document.Insert(offset, text);
					redocounter++;
					textAreaControl.Document.Caret.Offset = offset +  text.Length;
					textAreaControl.EndUpdate();
					
					textAreaControl.UpdateToEnd(textAreaControl.Document.GetLineNumberForOffset(offset));
					
					if (redocounter > 0) {
						textAreaControl.Document.UndoStack.UndoLast(redocounter); // redo the whole operation
					}
				}
			}
		}
		
		public void Delete(object sender, EventArgs e)
		{
			new ICSharpCode.TextEditor.Actions.Delete().Execute(textAreaControl);
		}
		
		public void SelectAll(object sender, EventArgs e)
		{
			new ICSharpCode.TextEditor.Actions.SelectWholeDocument().Execute(textAreaControl);
		}
	}
}
