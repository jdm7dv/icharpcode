// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.TextEditor.Document;
using SharpDevelop.Internal.Parser;


namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class IndexerInsightDataProvider : IInsightDataProvider
	{
		ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		IParserService parserService = (IParserService)ServiceManager.Services.GetService(typeof(IParserService));
		AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		
		string              fileName = null;
		IDocumentAggregator document = null;
		IndexerCollection   methods  = new IndexerCollection();
		
		public int InsightDataCount {
			get {
				return methods.Count;
			}
		}
		
		public string GetInsightData(int number)
		{
			IIndexer method = methods[number];
			IAmbience conv = ambienceService.CurrentAmbience;
			conv.ConversionFlags = ConversionFlags.StandardConversionFlags;
			return conv.Convert(method) + 
			       "\n" + 
			       method.Documentation;
		}
		
		int initialOffset;
		public void SetupDataProvider(string fileName, IDocumentAggregator document)
		{
			this.fileName = fileName;
			this.document = document;
			initialOffset = document.Caret.Offset;
			
			string word         = TextUtilities.GetExpressionBeforeOffset(document, document.Caret.Offset);
			string methodObject = word;
			
			// the parser works with 1 based coordinates
			int caretLineNumber      = document.GetLineNumberForOffset(document.Caret.Offset) + 1;
			int caretColumn          = document.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			ResolveResult results = parserService.Resolve(methodObject,
			                                              caretLineNumber,
			                                              caretColumn,
			                                              fileName);
			if (results != null && results.Type != null) {
				foreach (IClass c in results.Type.ClassInheritanceTree) {
					foreach (IIndexer indexer in c.Indexer) {
						methods.Add(indexer);
					}
				}
//				foreach (object o in results.ResolveContents) {
//					if (o is IClass) {
//						foreach (IClass c in ((IClass)o).ClassInheritanceTree) {
//							foreach (IIndexer indexer in c.Indexer) {
//								methods.Add(indexer);
//							}
//						}
//					}
//				}
			}
		}
		
		public bool CaretOffsetChanged()
		{
			bool closeDataProvider = document.Caret.Offset <= initialOffset;
			
			if (!closeDataProvider) {
				bool insideChar   = false;
				bool insideString = false;
				for (int offset = initialOffset; offset < Math.Min(document.Caret.Offset, document.TextLength); ++offset) {
					char ch = document.GetCharAt(offset);
					switch (ch) {
						case '\'':
							insideChar = !insideChar;
							break;
						case '"':
							insideString = !insideString;
							break;
						case ']':
						case '}':
						case '{':
						case ';':
							if (!(insideChar || insideString)) {
								return true;
							}
							break;
					}
				}
			}
			
			return closeDataProvider;
		}
		
		public bool CharTyped()
		{
			int offset = document.Caret.Offset - 1;
			if (offset >= 0) {
				return document.GetCharAt(offset) == ']';
			}
			return false;
		}
	}
}
