// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

#if BuildAsStandalone
namespace ICSharpCode.SharpDevelop.Internal.Undo
{
	/// <summary>
	/// This Interface describes a the basic Undo/Redo operation
	/// all Undo Operations must implement this interface.
	/// </summary>
	public interface IUndoableOperation
	{
		void Undo();
		void Redo();
	}
	
	/// <summary>
	/// This class stacks the last x operations from the undostack and makes
	/// one undo/redo operation from it.
	/// </summary>
	public class UndoQueue : IUndoableOperation
	{
		ArrayList undolist = new ArrayList();
		
		public UndoQueue(UndoStack stack, int numops)
		{
			if (stack == null)  {
				throw new ArgumentNullException("stack");
			}
			
			Debug.Assert(numops > 0 , "ICSharpCode.SharpDevelop.Internal.Undo.UndoQueue : numops should be > 0");
			
			for (int i = 0; i < numops; ++i) {
				if (stack._UndoStack.Count > 0) {
					undolist.Add(stack._UndoStack.Pop());
				}
			}
		}
		
		public void Undo()
		{
			for (int i = 0; i < undolist.Count; ++i) {
				((IUndoableOperation)undolist[i]).Undo();
			}
		}
		
		public void Redo()
		{
			for (int i = undolist.Count - 1 ; i >= 0 ; --i) {
				((IUndoableOperation)undolist[i]).Redo();
			}
		}
	}
	
	/// <summary>
	/// This class implements an undo stack
	/// </summary>
	public class UndoStack
	{
		Stack undostack = new Stack();
		Stack redostack = new Stack();
		
		public event EventHandler ActionUndone;
		public event EventHandler ActionRedone;
		
		public bool AcceptChanges = true;
		
		/// <summary>
		/// This property is EXCLUSIVELY for the UndoQueue class, don't USE it
		/// </summary>
		internal Stack _UndoStack {
			get {
				return undostack;
			}
		}
		
		public bool CanUndo {
			get {
				return undostack.Count > 0;
			}
		}
		
		public bool CanRedo {
			get {
				return redostack.Count > 0;
			}
		}
		
		/// <summary>
		/// You call this method to pool the last x operations from the undo stack
		/// to make 1 operation from it.
		/// </summary>
		public void UndoLast(int x)
		{
			undostack.Push(new UndoQueue(this, x));
		}
		
		/// <summary>
		/// Call this method to undo the last operation on the stack
		/// </summary>
		public void Undo()
		{
			if (undostack.Count > 0) {
				IUndoableOperation uedit = (IUndoableOperation)undostack.Pop();
				redostack.Push(uedit);
				uedit.Undo();
				OnActionUndone();
			}
		}
		
		/// <summary>
		/// Call this method to redo the last undone operation
		/// </summary>
		public void Redo()
		{
			if (redostack.Count > 0) {
				IUndoableOperation uedit = (IUndoableOperation)redostack.Pop();
				undostack.Push(uedit);
				uedit.Redo();
				OnActionRedone();
			}
		}
		
		/// <summary>
		/// Call this method to push an UndoableOperation on the undostack, the redostack
		/// will be cleared, if you use this method.
		/// </summary>
		public void Push(IUndoableOperation operation) 
		{
			if (operation == null) {
				throw new ArgumentNullException("UndoStack.Push(UndoableOperation operation) : operation can't be null");
			}
			
			if (AcceptChanges) {
				undostack.Push(operation);
				ClearRedoStack();
			}
		}
		
		/// <summary>
		/// Call this method, if you want to clear the redo stack
		/// </summary>
		public void ClearRedoStack()
		{
			redostack.Clear();
		}
		
		public void ClearAll()
		{
			undostack.Clear();
			redostack.Clear();
		}
		
		protected void OnActionUndone()
		{
			if (ActionUndone != null) {
				ActionUndone(null, null);
			}
		}
		
		protected void OnActionRedone()
		{
			if (ActionRedone != null) {
				ActionRedone(null, null);
			}
		}
	}	
	
}
#endif