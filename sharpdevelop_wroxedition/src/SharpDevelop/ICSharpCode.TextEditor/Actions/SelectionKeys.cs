// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Windows.Forms;
using System;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions
{
	public class ShiftCaretRight : CaretRight
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	
	public class ShiftCaretLeft : CaretLeft
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftCaretUp : CaretUp
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftCaretDown : CaretDown
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftWordRight : WordRight
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftWordLeft : WordLeft
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftHome : Home
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftEnd : End
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftMoveToStart : MoveToStart
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftMoveToEnd : MoveToEnd
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftMovePageUp : MovePageUp
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class ShiftMovePageDown : MovePageDown
	{
		public override void Execute(IEditActionHandler services)
		{
			int oldCaretOffset = services.Document.Caret.Offset;
			base.Execute(services);
			services.ExtendSelection(oldCaretOffset, services.Document.Caret.Offset);
		}
	}
	
	public class SelectWholeDocument : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			services.AddToSelection(new DefaultSelection(services.Document, 0, services.Document.TextLength));
		}
	}
	
	public class ClearAllSelections : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			string text = services.Document.TextContent;
			services.ClearSelection();
		}
	}
}
