// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Document
{
	public enum DocumentIteratorType {
		None,
		CurrentDocument,
		AllOpenFiles,
		WholeCombine,
		Directory // only used for search in files
	}
	
	public enum SearchStrategyType {
		None,
		Normal,
		RegEx,
		Wildcard
	}
	
	public class SearchReplaceManager
	{
		public static ReplaceDialog ReplaceDialog     = null;
				
		static IFind find                  = new DefaultFind();
		static SearchOptions searchOptions = new SearchOptions("SharpDevelop.SearchAndReplace.SearchAndReplaceProperties");

		
		public static SearchOptions SearchOptions {
			get {
				return searchOptions;
			}
		}
		
		static SearchReplaceManager()
		{
			find.TextIteratorBuilder = new ForwardTextIteratorBuilder();
			searchOptions.SearchStrategyTypeChanged   += new EventHandler(InitializeSearchStrategy);
			searchOptions.DocumentIteratorTypeChanged += new EventHandler(InitializeDocumentIterator);
			InitializeDocumentIterator(null, null);
			InitializeSearchStrategy(null, null);
		}	
		
		static void InitializeSearchStrategy(object sender, EventArgs e)
		{
			find.SearchStrategy = SearchReplaceUtilities.CreateSearchStrategy(SearchOptions.SearchStrategyType);
		}
		
		static void InitializeDocumentIterator(object sender, EventArgs e)
		{
			find.DocumentIterator = SearchReplaceUtilities.CreateDocumentIterator(SearchOptions.DocumentIteratorType);
		}
		
		// TODO: Transform Replace Pattern
		public static void Replace()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				TextAreaControl textarea = ((ITextAreaControlProvider)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).TextAreaControl;
				string text = textarea.GetSelectedText();
				if (text == SearchOptions.SearchPattern) {
					int offset = textarea.Document.SelectionCollection[0].Offset;
					
					textarea.BeginUpdate();
					textarea.RemoveSelectedText();
					textarea.Document.Insert(offset, SearchOptions.ReplacePattern);
					textarea.Document.Caret.Offset = offset +  SearchOptions.ReplacePattern.Length;
					textarea.EndUpdate();
				}
			}
			FindNext();
		}
		
		public static void MarkAll()
		{
			TextAreaControl textArea = null;
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				textArea = (TextAreaControl)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.Control;
				textArea.ClearSelection();
			}
			find.Reset();
			find.SearchStrategy.CompilePattern(searchOptions);
			while (true) {
				ISearchResult result = SearchReplaceManager.find.FindNext(searchOptions);
				
				if (result == null) {
					MessageBox.Show((Form)WorkbenchSingleton.Workbench, "Mark all done", "Finished");
					find.Reset();
					return;
				} else {
					textArea = OpenTextArea(result.FileName); 
					
					textArea.Document.Caret.Offset = result.Offset;
					int lineNr = textArea.Document.GetLineNumberForOffset(result.Offset);
					
					if (!textArea.Document.BookmarkManager.IsMarked(lineNr)) {
						textArea.Document.BookmarkManager.ToggleMarkAt(lineNr);
					}
				}
			}
		}
		
		public static void ReplaceAll()
		{
			TextAreaControl textArea = null;
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				textArea = ((ITextAreaControlProvider)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).TextAreaControl;
				textArea.ClearSelection();
			}
			find.Reset();
			find.SearchStrategy.CompilePattern(searchOptions);
			
			while (true) {
				ISearchResult result = SearchReplaceManager.find.FindNext(SearchReplaceManager.searchOptions);
				
				if (result == null) {
					MessageBox.Show((Form)WorkbenchSingleton.Workbench, "Replace all done", "Finished");
					find.Reset();
					return;
				} else {
					textArea = OpenTextArea(result.FileName); 
					
					textArea.BeginUpdate();
					textArea.Document.SelectionCollection.Clear();
					
					string transformedPattern = result.TransformReplacePattern(SearchOptions.ReplacePattern);
					find.Replace(result.Offset,
					             result.Length, 
					             transformedPattern);
					textArea.EndUpdate();
					textArea.Invalidate(true);
				}
			}
		}
		
		static ISearchResult lastResult = null;
		public static void FindNext()
		{
			if (find == null || 
			    searchOptions.SearchPattern == null || 
			    searchOptions.SearchPattern.Length == 0) {
				return;
			}
			
			find.SearchStrategy.CompilePattern(searchOptions);
			ISearchResult result = find.FindNext(searchOptions);
				
			if (result == null) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				MessageBox.Show((Form)WorkbenchSingleton.Workbench,
				                resourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound"),
				                "Not Found", 
				                MessageBoxButtons.OK, 
				                MessageBoxIcon.Information);
				find.Reset();
			} else {
				TextAreaControl textArea = OpenTextArea(result.FileName);
					
				if (lastResult != null  && lastResult.FileName == result.FileName && 
				    textArea.Document.Caret.Offset != lastResult.Offset + lastResult.Length) {
					find.Reset();
				}
				int startPos = Math.Min(textArea.Document.TextLength, Math.Max(0, result.Offset));
				int endPos   = Math.Min(textArea.Document.TextLength, startPos + result.Length);
				textArea.Document.Caret.Offset = endPos;
				textArea.SetSelection(new DefaultSelection(textArea.Document, startPos, endPos - startPos));
			}
			
			lastResult = result;
		}
		
		static TextAreaControl OpenTextArea(string fileName) 
		{
			if (fileName != null) {
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile(fileName);
			}
			
			return ((ITextAreaControlProvider)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).TextAreaControl;
		}
	}	
}
