// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.SharpDevelop.Gui
{
	public interface IClipboardHandler
	{
		void Cut(object sender, EventArgs e);
		void Copy(object sender, EventArgs e);
		void Paste(object sender, EventArgs e);
		void Delete(object sender, EventArgs e);
		void SelectAll(object sender, EventArgs e);
	}
}
