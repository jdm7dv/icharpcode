// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This class handles the bookmarks for a buffer
	/// </summary>
	public interface IBookMarkManager : IMementoCapable
	{
		ArrayList Marks {
			get;
		}
		
		int FirstMark {
			get;
		}
		
		int LastMark {
			get;
		}
		
		void ToggleMarkAt(int lineNr);
		bool IsMarked(int lineNr);
		
		void Clear();
		
		int GetNextMark(int curLineNr);
		int GetPrevMark(int curLineNr);
		
		event EventHandler BeforeChanged;
		event EventHandler Changed;
	}
}
