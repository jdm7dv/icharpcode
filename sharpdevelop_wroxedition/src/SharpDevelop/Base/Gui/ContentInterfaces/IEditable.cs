// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.SharpDevelop.Gui
{
	public interface IEditable
	{
		IClipboardHandler ClipboardHandler {
			get;
		}
		
		void Undo();
		void Redo();
		
		string TextContent {
			get;
			set;
		}
	}
}
