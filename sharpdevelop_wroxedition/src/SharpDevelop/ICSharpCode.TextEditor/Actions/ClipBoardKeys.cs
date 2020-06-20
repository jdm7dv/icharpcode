// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Windows.Forms;
using System;

using ICSharpCode.Core.Properties;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions 
{
	public class Cut : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			services.ClipboardHandler.Cut(null, null);
		}
	}
	
	public class Copy : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			services.AutoClearSelection = false;
			services.ClipboardHandler.Copy(null, null);
	    }
	}

	public class Paste : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			services.ClipboardHandler.Paste(null, null);
	    }
	}
}
