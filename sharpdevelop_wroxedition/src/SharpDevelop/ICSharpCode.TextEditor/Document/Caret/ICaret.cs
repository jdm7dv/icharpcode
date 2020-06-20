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
	/// <summary>
	/// In this enumeration are all caret modes listed.
	/// </summary>
	public enum CaretMode {
		InsertMode,
		OverwriteMode
	}
	
	/// <summary>
	/// The basic interface for the caret.
	/// </summary>
	public interface ICaret
	{
		/// <summary>
		/// The current offset where the caret points to.
		/// </summary>
		int Offset {
			get;
			set;
		}
		
		/// <summary>
		/// The 'prefered' column in which the caret moves, when it is moved
		/// up/down.
		/// </summary>
		int DesiredColumn {
			get;
			set;
		}
		
		/// <summary>
		/// The current caret mode.
		/// </summary>
		CaretMode CaretMode {
			get;
			set;
		}
		
		/// <summary>
		/// Sets the caret visibility.
		/// </summary>
		bool Visible {
			get;
			set;
		}
		
		/// <summary>
		/// Is called each time the caret is moved.
		/// </summary>
		event CaretEventHandler OffsetChanged;
		
		/// <summary>
		/// Is called each time the CaretMode has changed.
		/// </summary>
		event CaretEventHandler CaretModeChanged;
	}
}
