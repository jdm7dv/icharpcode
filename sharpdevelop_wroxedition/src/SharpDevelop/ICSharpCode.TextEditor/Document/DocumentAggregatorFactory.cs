// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.IO;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This interface represents a container which holds a text sequence and
	/// all necessary information about it. It is used as the base for a text editor.
	/// </summary>
	public class DocumentAggregatorFactory
	{
		public IDocumentAggregator CreateDocument()
		{
			DefaultDocumentAggregator doc = new DefaultDocumentAggregator();
#if !BuildAsStandalone
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (propertyService == null) {
				doc.Properties           = new DefaultProperties();	
			} else {
				doc.Properties           = ((IProperties)propertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			}
#else
			doc.Properties           = new DefaultProperties();	
#endif
			doc.UndoStack             = new UndoStack();
//			doc.TextBufferStrategy    = new StringTextBufferStrategy();
			doc.TextBufferStrategy    = new GapTextBufferStrategy();
			doc.Caret                 = new DefaultCaret(doc);
			doc.FormattingStrategy    = new DefaultFormattingStrategy();
			doc.FoldingStrategy       = new IndentFoldingStrategy();
			doc.FormattingStrategy.Document = doc;
			
			doc.LineManager = new DefaultLineManager(doc, null);
			doc.BookmarkManager      = new BookmarkManager(doc.LineManager);
			doc.TextModel            = new DefaultTextModel(doc);
			return doc;
		}
		
		public IDocumentAggregator CreateFromFile(string fileName)
		{
			IDocumentAggregator document = CreateDocument();
			StreamReader stream = File.OpenText(fileName);
			document.TextContent = stream.ReadToEnd();
			stream.Close();
			return document;
		}
	}
}
