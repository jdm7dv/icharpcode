// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions
{
	/// <summary>
	/// To define a new key for the textarea, you must write a class which
	/// implements this interface.
	/// </summary>
	public interface IEditAction
	{
		Keys[] Keys {
			get;
			set;
		}
		
		/// <summary>
		/// When the key which is defined per XML is pressed, this method will be launched.
		/// </summary>
		void Execute(IEditActionHandler services);
	}
	
	/// <summary>
	/// To define a new key for the textarea, you must write a class which
	/// implements this interface.
	/// </summary>
	public abstract class AbstractEditAction : IEditAction
	{
		Keys[] keys = null;
		
		public Keys[] Keys {
			get {
				return keys;
			}
			set {
				keys = value;
			}
		}
		
		public abstract void Execute(IEditActionHandler services);
	}		
}
