// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using SharpDevelop;
using SharpDevelop.Internal.Parser;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class CompletionWindow : Form
	{
		ICompletionDataProvider completionDataProvider;
		TextAreaControl         control;
		ListView                listView              = new MyListView();
		DeclarationViewWindow   declarationviewwindow = new DeclarationViewWindow();
		
		int    insertLength = 0;
		
		class MyListView : ListView
		{
			protected override bool ProcessDialogKey(Keys keyData)
			{
				if (keyData == Keys.Tab) {
					OnItemActivate(null);
				}
				return base.ProcessDialogKey(keyData);
			}
		}
		
		string GetTypedString()
		{
			return control.Document.GetText(control.Document.Caret.Offset - insertLength, insertLength);
		}
		
		void DeleteInsertion()
		{
			if (insertLength > 0) {
				control.Document.Remove(control.Document.Caret.Offset - insertLength, insertLength);
				control.UpdateLine(control.Document.GetLineNumberForOffset(control.Document.Caret.Offset), 0, 0);
			}
		}
				
		void ListKeypressEvent(object sender, KeyPressEventArgs ex)
		{
			switch (ex.KeyChar) {
				case (char)27: // Escape
					LostFocusListView(null, null);
					return;
				case '\b': //Backspace
					new ICSharpCode.TextEditor.Actions.Backspace().Execute(control);
					if (insertLength > 0) {
						--insertLength;
					} else {
						// no need to delete here (insertLength <= 0)
						LostFocusListView(null, null);
					}
					break;
				default:
					if (ex.KeyChar != '_' && !Char.IsLetterOrDigit(ex.KeyChar)) {
						if (listView.SelectedItems.Count > 0) {
							ActivateItem(null, null);
						} else {
							LostFocusListView(null, null);
						}
						control.KeyPressed(this, ex);
						return;
					} else {
						control.InsertChar(ex.KeyChar);
						++insertLength;
					}
					break;
			}
			
			// select the current typed word
			int lastSelected = -1;
			int capitalizationIndex = -1;
			
			string typedString = GetTypedString();
			for (int i = 0; i < listView.Items.Count; ++i) {
				
				if (listView.Items[i].Text.ToUpper().StartsWith(typedString.ToUpper())) {
					int currentCapitalizationIndex = 0;
					for (int j = 0; j < typedString.Length && j < listView.Items[i].Text.Length; ++j) {
						if (typedString[j] == listView.Items[i].Text[j]) {
							++currentCapitalizationIndex;
						}
					}
					
					if (currentCapitalizationIndex > capitalizationIndex) {
						lastSelected = i;
						capitalizationIndex = currentCapitalizationIndex;
					}
				}
			}
			
			listView.SelectedItems.Clear();
			if (lastSelected != -1) {
				listView.Items[lastSelected].Focused  = true;
				listView.Items[lastSelected].Selected = true;
				listView.EnsureVisible(lastSelected);
			}
			ex.Handled = true;
		}
		
		void InitializeControls()
		{
			Width    = 340;
			Height   = 210 - 85;
			
			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			TopMost         = true;
			ShowInTaskbar   = false;
			
			listView.Dock        = DockStyle.Fill;
			listView.View        = View.Details;
			listView.AutoArrange = true;
			listView.Alignment   = ListViewAlignment.Left;
			listView.HeaderStyle = ColumnHeaderStyle.None;
			listView.Sorting     = SortOrder.Ascending;
			listView.MultiSelect = false;
			listView.FullRowSelect  = true;
			listView.HideSelection  = false;
//			listView.Font           = ICSharpCode.TextEditor.Document.FontContainer.DefaultFont;
			listView.SmallImageList = completionDataProvider.ImageList;
			listView.KeyPress += new KeyPressEventHandler(ListKeypressEvent);
			
			listView.LostFocus            += new EventHandler(LostFocusListView);
			listView.ItemActivate         += new EventHandler(ActivateItem);
			listView.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
			this.Controls.Add(listView);			
		}
		
		/// <remarks>
		/// Shows the filled completion window, if it has no items it isn't shown.
		/// </remarks>
		public void ShowCompletionWindow(char firstChar)
		{
			FillList(true, firstChar);
			
			if (listView.Items.Count > 0) {
				Rectangle size = listView.Items[0].GetBounds(ItemBoundsPortion.Entire);
				
				ClientSize = new Size(size.Width + SystemInformation.VerticalScrollBarWidth + 4,
				                      size.Height * Math.Min(10, listView.Items.Count));
				
				declarationviewwindow.Show();
				Show();
				
				listView.Select();
				listView.Focus();
				
				listView.Items[0].Focused = listView.Items[0].Selected = true;				
				control.TextAreaPainter.IHaveTheFocus = true;
			} else {
				control.Focus();
			}
		}
		string fileName;
		
		/// <remarks>
		/// Creates a new Completion window and puts it location under the caret
		/// </remarks>
		public CompletionWindow(TextAreaControl control, string fileName, ICompletionDataProvider completionDataProvider)
		{
			this.fileName = fileName;
			this.completionDataProvider = completionDataProvider;
			this.control                = control;
			
			Point caretPos = control.Document.OffsetToView(control.Document.Caret.Offset);
			Point visualPos = new Point(control.TextAreaPainter.GetVirtualPos(control.Document.GetLineSegmentForOffset(control.Document.Caret.Offset), caretPos.X),
			                            (int)((1 + caretPos.Y) * control.TextAreaPainter.FontHeight) - control.TextAreaPainter.ScreenVirtualTop);
			                            
	 		Location = control.TextAreaPainter.PointToScreen(visualPos);
			InitializeControls();
		}
		
		/// <remarks>
		/// Creates a new Completion window at a given location
		/// </remarks>
		CompletionWindow(TextAreaControl control, Point location, ICompletionDataProvider completionDataProvider)
		{
			this.completionDataProvider = completionDataProvider;
			this.control                = control;
			Location = location;
			InitializeControls();
		}
		
		void SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count > 0) {
				ICompletionData data = (ICompletionData)listView.SelectedItems[0].Tag;				
				
				if (data.Description != null) {
					listView.EnsureVisible(listView.SelectedIndices[0]);
					Point pos = new Point(
						Bounds.Right,
						Bounds.Top + listView.GetItemRect(listView.SelectedIndices[0]).Y);
					declarationviewwindow.Location    = pos;
					declarationviewwindow.Description = data.Description;
				} else {
					declarationviewwindow.Size = new Size(0, 0);
				}
			}
		}
		
		void ActivateItem(object sender, EventArgs e)
		{
			control.TextAreaPainter.IHaveTheFocusLock = true;
			if (listView.SelectedItems.Count > 0) {
				ICompletionData data = (ICompletionData)listView.SelectedItems[0].Tag;
				DeleteInsertion();
				data.InsertAction(control);
				LostFocusListView(sender, e);
			}
			control.TextAreaPainter.IHaveTheFocusLock = false;
		}
		
		void LostFocusListView(object sender, EventArgs e)
		{
			control.Focus();
			declarationviewwindow.Close();
			Close();
		}
		
		void FillList(bool firstTime, char ch)
		{
			ICompletionData[] completionData = completionDataProvider.GenerateCompletionData(fileName, control.Document, ch);
			if (completionData == null || completionData.Length == 0) {
				return;
			}
			if (firstTime && completionData.Length > 0) {
				int columnHeaders = completionData[0].Text.Length;
				for (int i = 0; i < columnHeaders; ++i) {
					ColumnHeader header = new ColumnHeader();
					header.Width = -1;
					listView.Columns.Add(header);
				}
			}
			
			listView.BeginUpdate();
			foreach (ICompletionData data in completionData) {
				ListViewItem newItem = new ListViewItem(data.Text, data.ImageIndex);
				newItem.Tag = data;
				listView.Items.Add(newItem);
			}
			listView.EndUpdate();
		}
	}
}
