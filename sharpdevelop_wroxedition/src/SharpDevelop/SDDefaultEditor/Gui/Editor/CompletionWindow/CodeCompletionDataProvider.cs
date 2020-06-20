// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

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
	public class CodeCompletionDataProvider : ICompletionDataProvider
	{
		static ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		static IParserService           parserService           = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
//		static AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		Hashtable insertedElements           = new Hashtable();
		Hashtable insertedPropertiesElements = new Hashtable();
		Hashtable insertedEventElements      = new Hashtable();
		
		public ImageList ImageList {
			get {
				return classBrowserIconService.ImageList;
			}
		}
		
		int caretLineNumber;
		int caretColumn;
		string fileName;
		
		ArrayList completionData = null;
			
		public ICompletionData[] GenerateCompletionData(string fileName, IDocumentAggregator document, char charTyped)
		{
			completionData = new ArrayList();
			this.fileName = fileName;
			
			// the parser works with 1 based coordinates
			caretLineNumber      = document.GetLineNumberForOffset(document.Caret.Offset) + 1;
			caretColumn          = document.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			string expression    = TextUtilities.GetExpressionBeforeOffset(document, document.Caret.Offset);
			ResolveResult results;
			
			if (expression.Length == 0) {
				return null;
			}
			Console.WriteLine("expression '{0}'", expression);
			if (charTyped == ' ') {
				if (expression == "using" || expression.EndsWith(" using") || expression.EndsWith("\tusing")|| expression.EndsWith("\nusing")|| expression.EndsWith("\rusing")) {
					string[] namespaces = parserService.GetNamespaceList("");
//					AddResolveResults(new ResolveResult(namespaces, ShowMembers.Public));
					AddResolveResults(new ResolveResult(namespaces));
//					IParseInformation info = parserService.GetParseInformation(fileName);
//					ICompilationUnit unit = info.BestCompilationUnit as ICompilationUnit;
//					if (unit != null) {
//						foreach (IUsing u in unit.Usings) {
//							if (u.Region.IsInside(caretLineNumber, caretColumn)) {
//								foreach (string usingStr in u.Usings) {
//									results = parserService.Resolve(usingStr, caretLineNumber, caretColumn, fileName);
//									AddResolveResults(results);
//								}
//								if (u.Aliases[""] != null) {
//									results = parserService.Resolve(u.Aliases[""].ToString(), caretLineNumber, caretColumn, fileName);
//									AddResolveResults(results);
//								}
//							}
//						}
//					}
				}
			} else {
				Console.WriteLine("expression : '{0}' ", expression);
				results    = parserService.Resolve(expression, 
				                                   caretLineNumber,
				                                   caretColumn,
				                                   fileName);
				AddResolveResults(results);
			}
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		void AddResolveResults(ResolveResult results)
		{
			if (results != null) {
				if (results.Namespaces != null && results.Namespaces.Count > 0) {
					foreach (string s in results.Namespaces) {
						completionData.Add(new CodeCompletionData(s, classBrowserIconService.NamespaceIndex));
					}
				}
				if (results.Members != null && results.Members.Count > 0) {
					foreach (Object o in results.Members) {
						if (o is IClass) {
							completionData.Add(new CodeCompletionData((IClass)o));
						} else if (o is IProperty) {
							IProperty property = (IProperty)o;
							if (insertedPropertiesElements[property.Name] == null) {
								completionData.Add(new CodeCompletionData(property));
								insertedPropertiesElements[property.Name] = property;
							}
						} else if (o is IMethod) {
							IMethod method = (IMethod)o;
							if (insertedElements[method.Name] == null && !method.IsConstructor) {
								completionData.Add(new CodeCompletionData(method));
								insertedElements[method.Name] = method;
							}
						} else if (o is IField) {
							completionData.Add(new CodeCompletionData((IField)o));
						} else if (o is IEvent) {
							IEvent e = (IEvent)o;
							if (insertedEventElements[e.Name] == null) {
								completionData.Add(new CodeCompletionData(e));
								insertedEventElements[e.Name] = e;
							}
						}
					}
				}
			}
		}
	}
}
