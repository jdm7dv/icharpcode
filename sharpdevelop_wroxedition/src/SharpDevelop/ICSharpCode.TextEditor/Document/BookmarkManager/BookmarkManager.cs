// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using ICSharpCode.Core.Properties;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This class handles the bookmarks for a buffer
	/// </summary>
	public class BookmarkManager : IBookMarkManager
	{
		ArrayList bookmark = new ArrayList();
		
		public event EventHandler BeforeChanged;
		public event EventHandler Changed;
		
		public ArrayList Marks {
			get {
				return bookmark;
			}
		}
			
		public BookmarkManager(ILineManager lineTracker)
		{
			lineTracker.LineCountChanged += new LineManagerEventHandler(MoveIndices);
		}
		
		void OnChanged() 
		{
			if (Changed != null) {
				Changed(this, null);
			}
		}
		void OnBeforeChanged() 
		{
			if (BeforeChanged != null) {
				BeforeChanged(this, null);
			}
		}
			
		/// <summary>
		/// If a mark exists at position num it will be removed, if not it will be inserted
		/// </summary>
		public void ToggleMarkAt(int lineNr)
		{
			OnBeforeChanged();
			for (int i = 0; i < bookmark.Count; ++i) {
				if ((int)bookmark[i] == lineNr) {
					bookmark.RemoveAt(i);
					OnChanged();
					return;
				}
			}
			bookmark.Add(lineNr);
			OnChanged();
		}
		
		/// <returns>
		/// true, if a mark at mark exists, otherwise false
		/// </returns>
		public bool IsMarked(int lineNr)
		{
			for (int i = 0; i < bookmark.Count; ++i) {
				if ((int)bookmark[i] == lineNr) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// This method moves all indices from index upward count lines
		/// (useful for deletion/insertion of text)
		/// </summary>
		void MoveIndices(object sender,LineManagerEventArgs e)
		{
			bool changed = false;
			OnBeforeChanged();
			for (int i = 0; i < bookmark.Count; ++i) {
				int mark = (int)bookmark[i];
				if (e.LinesMoved < 0 && mark == e.LineStart) {
					bookmark.RemoveAt(i);
					--i;
					changed = true;
				} else if (mark > e.LineStart + 1 || (e.LinesMoved < 0 && mark > e.LineStart))  {
					changed = true;
					bookmark[i] = mark + e.LinesMoved;
				}
			}
			
			if (changed) {
				OnChanged();
			}
		}
		
		public IXmlConvertable CreateMemento()
		{
			return new BookmarkManagerMemento((ArrayList)bookmark.Clone());
		}
		
		public void SetMemento(IXmlConvertable memento)
		{
			bookmark = ((BookmarkManagerMemento)memento).Bookmarks;
		}
		
		
		/// <summary>
		/// Clears all bookmark
		/// </summary>
		public void Clear()
		{
			OnBeforeChanged();
			bookmark.Clear();
			OnChanged();
		}
		
		/// <returns>
		/// returns the lowest mark, if no marks exists it returns -1
		/// </returns>
		public int FirstMark {
			get {
				if (bookmark.Count < 1) {
					return -1;
				}
				int first = (int)bookmark[0];
				for (int i = 1; i < bookmark.Count; ++i) {
					first = Math.Min(first, (int)bookmark[i]);
				}
				return first;
			}
		}
		
		/// <returns>
		/// returns the highest mark, if no marks exists it returns -1
		/// </returns>
		public int LastMark {
			get {
				if (bookmark.Count < 1) {
					return -1;
				}
				int last = (int)bookmark[0];
				for (int i = 1; i < bookmark.Count; ++i) {
					last = Math.Max(last, (int)bookmark[i]);
				}
				return last;
			}
		}
		
		/// <returns>
		/// returns the next mark > cur, if it not exists it returns FirstMark()
		/// </returns>
		public int GetNextMark(int curLineNr)
		{
			int next = -1;
			for (int i = 0; i < bookmark.Count; ++i) {
				int j = (int)bookmark[i];
				if (j > curLineNr) {
					next = next == -1 ? j : Math.Min(next, j);
				}
			}
			return next == -1 ? FirstMark : next;
		}
		
		/// <returns>
		/// returns the next mark lower than cur, if it not exists it returns LastMark()
		/// </returns>
		public int GetPrevMark(int curLineNr)
		{
			int prev = -1;
			for (int i = 0; i < bookmark.Count; ++i) {
				int j = (int)bookmark[i];
				if (j < curLineNr) {
					prev = prev == -1 ? j : Math.Max(prev, j);
				}
			}
			return prev == -1 ? LastMark : prev;
		}
	}
}
