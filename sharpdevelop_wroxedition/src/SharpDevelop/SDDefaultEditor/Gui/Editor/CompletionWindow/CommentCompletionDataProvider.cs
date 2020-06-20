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

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using SharpDevelop.Internal.Parser;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class CommentCompletionDataProvider : ICompletionDataProvider
	{
		static ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		static IParserService           parserService           = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
		
		ICompilationUnit cu;
		
		int caretLineNumber;
		int caretColumn;
		
		string[][] commentTags = new string[][] {
			new string[] {"c", "marks text as code"},
			new string[] {"code", "marks text as code"},
			new string[] {"example", "A description of the code example\n(must have a <code> tag inside)"},
			new string[] {"exception cref=\"\"", "description to an exception thrown"},
			new string[] {"list type=\"\"", "A list"},
			new string[] {"listheader", "The header from the list"},
			new string[] {"item", "A list item"},
			new string[] {"term", "A term in a list"},
			new string[] {"description", "A description to a term in a list"},
			new string[] {"param name=\"\"", "A description for a parameter"},
			new string[] {"paramref name\"\"", "A reference to a parameter"},
			new string[] {"permission cref=\"\"", ""},
			new string[] {"remarks", "Gives description for a member"},
			new string[] {"returns", "Gives description for a return value"},
			new string[] {"see cref=\"\"", "A reference to a member"},
			new string[] {"seealso cref=\"\"", "A reference to a member in the seealso section"},
			new string[] {"summary", "A summary of the object"},
			new string[] {"value", "A description of a property"}
		};
		
		public ImageList ImageList {
			get {
				return classBrowserIconService.ImageList;
			}
		}
		
		/// <remarks>
		/// Returns true, if the given coordinates (row, column) are in the region.
		/// </remarks>
		bool IsBetween(int row, int column, IRegion region)
		{
			return row >= region.BeginLine && (row <= region.EndLine || region.EndLine == -1);
		}
		
		/// <remarks>
		/// Returns the class in which the carret currently is, returns null
		/// if the carret is outside the class boundaries.
		/// </remarks>
		IClass GetCurrentClass(IDocumentAggregator document, string fileName)
		{
			if (cu != null) {
				foreach (IClass c in cu.Classes) {
					if (IsBetween(caretLineNumber, caretColumn, c.Region)) {
						return c;
					}
				}
			}
			return null;
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, IDocumentAggregator document, char charTyped)
		{
			caretLineNumber = document.GetLineNumberForOffset(document.Caret.Offset) + 1;
			caretColumn     = document.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			IParseInformation parseInformation = parserService.GetParseInformation(fileName);
			
			if (parseInformation == null) {
				return null;
			}
			
			cu = parseInformation.MostRecentCompilationUnit as ICompilationUnit;
			if (cu == null) {
				return null;
			}
			
			IClass callingClass = GetCurrentClass(document, fileName);
			bool inComment = false;
			foreach (Comment comment in cu.DokuComments) {
				if (IsBetween(caretLineNumber, caretColumn, comment.Region)) {
					inComment = true;
					break;
				}
			}
			
			ArrayList completionData = new ArrayList();
			
			if (inComment) {
				foreach (string[] tag in commentTags) {
					completionData.Add(new CommentCompletionData(tag[0], tag[1]));
				}
			}
			
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		class CommentCompletionData : ICompletionData
		{
			string text;
			string description;
			
			public int ImageIndex {
				get {
					return classBrowserIconService.MethodIndex;
				}
			}
			
			public string[] Text {
				get {
					return new string[] { text };
				}
			}
			
			public string Description {
				get {
					return description;
				}
			}
			
			public void InsertAction(TextAreaControl control)
			{
				((SharpDevelopTextAreaControl)control).InsertString(text);
			}
			
			public CommentCompletionData(string text, string description) 
			{
				this.text        = text;
				this.description = description;
			}
		}
	}
}
