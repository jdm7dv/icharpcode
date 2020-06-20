// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// An interface representing a portion of the current selection.
	/// </summary>
	public interface ISelection
	{
		/// <summary>
		/// Returns the start offset of this selection.
		/// </summary>
		int Offset {
			get;
			set;
		}
		
		/// <summary>
		/// Returns the length of this selection.
		/// </summary>
		int Length {
			get;
			set;
		}
		
		/// <summary>
		/// Returns the starting line number of this selection.
		/// </summary>
		int StartLine {
			get;
		}
		
		/// <summary>
		/// Returns the ending line number of this selection.
		/// </summary>
		int EndLine {
			get;
		}
		
		/// <summary>
		/// Returns true, if the selection is rectangular
		/// </summary>
		bool IsRectangularSelection {
			get;
		}
		
		/// <summary>
		/// Returns the text which is selected by this selection.
		/// </summary>
		string SelectedText {
			get;
		}
	}
}
