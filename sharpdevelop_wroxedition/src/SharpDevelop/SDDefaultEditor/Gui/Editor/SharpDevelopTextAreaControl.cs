// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Actions;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Services;

using Crownwood.Magic.Menus;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class SharpDevelopTextAreaControl : TextAreaControl
	{
		readonly static string contextMenuPath       = "/SharpDevelop/ViewContent/DefaultTextEditor/ContextMenu";
		readonly static string editActionsPath       = "/AddIns/DefaultTextEditor/EditActions";
		readonly static string formatingStrategyPath = "/AddIns/DefaultTextEditor/Formater";
		
		BracketHighlighter bracketHighlighter;
		ErrorDrawer        errorDrawer;
		
		public BracketHighlighter BracketHighlighter {
			get {
				return bracketHighlighter;
			}
		}
		
		void PaintBracketHighlight(Graphics g, int lineNr, RectangleF rect, PointF pos, int virtualLeft, int virtualTop)
		{
			try {
				if (bracketHighlighter.Highlight != null && bracketHighlighter.Highlight.Offset >= 0 && bracketHighlighter.Highlight.Offset < Document.TextLength) {
					Point p1 = Document.OffsetToView(bracketHighlighter.Highlight.Offset);
					int yPos = p1.Y;
					if (p1.Y == lineNr) {
						LineSegment line = Document.GetLineSegment(p1.Y);
						
						Rectangle r = new Rectangle((int)(TextAreaPainter.GetScreenPos(g, line, Document.GetLogicalXPos(line, p1.X))  - virtualLeft + 1), 
													(int)(yPos * TextAreaPainter.FontHeight) + 1 - TextAreaPainter.ScreenVirtualTop, 
													(int)TextAreaPainter.FontWidth - 2,
													(int)(1 * TextAreaPainter.FontHeight) - 1);
						HighlightColor vRulerColor = Document.HighlightingStrategy.GetColorFor("VRulerColor");
						g.DrawRectangle(new Pen(new SolidBrush(vRulerColor.Color)), r);
					}
				}
			} catch(Exception) {}
		}
						
		public SharpDevelopTextAreaControl()
		{
			GenerateEditActions();
			TextAreaDragDropHandler dragDropHandler = new TextAreaDragDropHandler();
			dragDropHandler.Attach(this);
			
			bracketHighlighter = new BracketHighlighter(TextAreaPainter);
			TextAreaPainter.MouseUp += new MouseEventHandler(ShowContextMenu);
			TextAreaPainter.LinePainter += new LinePainter(PaintBracketHighlight);
			
			errorDrawer = new ErrorDrawer(this);
		}
		
		void ShowContextMenu(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) {
				MenuCommand[] contextMenu = (MenuCommand[])(AddInTreeSingleton.AddInTree.GetTreeNode(contextMenuPath).BuildChildItems(this)).ToArray(typeof(MenuCommand));
				if (contextMenu.Length > 0) {
					PopupMenu popup = new PopupMenu();
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					popup.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
					popup.MenuCommands.AddRange(contextMenu);
					popup.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
				}
			}
		}
	
		void GenerateEditActions()
		{
			try {
				IEditAction[] actions = (IEditAction[])(AddInTreeSingleton.AddInTree.GetTreeNode(editActionsPath).BuildChildItems(this)).ToArray(typeof(IEditAction));
				
				foreach (IEditAction action in actions) {
					foreach (Keys key in action.Keys) {
						editactions[key] = action;
					}
				}
			} catch (TreePathNotFoundException) {
				Console.WriteLine(editActionsPath + " doesn't exists in the AddInTree");
			}
		}
		
		InsightWindow insightWindow = null;
		protected override bool HandleKeyPress(char ch)
		{
			CompletionWindow completionWindow;
			IParserService  parserService           = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
//			parserService.ParseFile(FileName, Document.TextContent);
			
			string fileName = FileName;
			
			switch (ch) {
				case ' ':
					if (Properties.GetProperty("AutoInsertTemplates", true)) {
						string word = GetWordBeforeCaret();
						if (word != null) {
							foreach (CodeTemplate template in CodeTemplateLoader.Template) {
								if (template.Shortcut == word) {
									InsertTemplate(template);
									return true;
								}
							}
						}
					}
					goto case '.';
				case '<':
					completionWindow = new CompletionWindow(this, fileName,new CommentCompletionDataProvider());
					completionWindow.ShowCompletionWindow('<');
					return false;
				case '(':
					if (insightWindow == null || insightWindow.IsDisposed) {
						insightWindow = new InsightWindow(this, fileName);
					}
					
					insightWindow.AddInsightDataProvider(new MethodInsightDataProvider());
					insightWindow.ShowInsightWindow();
					return false;
				case '[':
					if (insightWindow == null || insightWindow.IsDisposed) {
						insightWindow = new InsightWindow(this, fileName);
					}
					
					insightWindow.AddInsightDataProvider(new IndexerInsightDataProvider());
					insightWindow.ShowInsightWindow();
					return false;
				case '.':
					TextAreaPainter.IHaveTheFocusLock = true;
					completionWindow = new CompletionWindow(this, fileName, new CodeCompletionDataProvider());
					completionWindow.ShowCompletionWindow(ch);
					TextAreaPainter.IHaveTheFocusLock = false;
					return false;
			}
			return false;
		}
		
		/// <remarks>
		/// This method inserts a code template at the current caret position
		/// </remarks>
		public void InsertTemplate(CodeTemplate template)
		{
			int newCaretOffset   = Document.Caret.Offset;
			string word = GetWordBeforeCaret().Trim();
			if (word.Length > 0) {
				newCaretOffset = DeleteWordBeforeCaret();
			}
			int finalCaretOffset = newCaretOffset;
			int firstLine        = Document.GetLineNumberForOffset(newCaretOffset);
			
			// save old properties, these properties cause strange effects, when not
			// be turned off (like insert curly braces or other formatting stuff)
			bool save1    = Properties.GetProperty("AutoInsertCurlyBracket", true);
			object save2  = Properties.GetProperty("IndentStyle");
			Properties.SetProperty("IndentStyle", IndentStyle.Auto);
			Properties.SetProperty("AutoInsertCurlyBracket", false);
			
			BeginUpdate();
			for (int i =0; i < template.Text.Length; ++i) {
				switch (template.Text[i]) {
					case '|':
						finalCaretOffset = newCaretOffset;
						break;
					case '\r':
						break;
					case '\t':
						ProcessDialogKey(Keys.Tab);
						break;
					case '\n':
						Document.Caret.Offset = newCaretOffset;
						ProcessDialogKey(Keys.Return);
						newCaretOffset = Document.Caret.Offset;
						break;
					default:
						Document.Insert(newCaretOffset, template.Text[i].ToString());
						++newCaretOffset;
						break;
				}
			}
			EndUpdate();
			UpdateLines(firstLine, Document.GetLineNumberForOffset(newCaretOffset));			
			Document.Caret.Offset = finalCaretOffset;
			
			// restore old property settings
			Properties.SetProperty("IndentStyle", save2);
			Properties.SetProperty("AutoInsertCurlyBracket", save1);
		}
		
		public void InitializeFormatter()
		{
			try {
				IFormattingStrategy[] formater = (IFormattingStrategy[])(AddInTreeSingleton.AddInTree.GetTreeNode(formatingStrategyPath).BuildChildItems(this)).ToArray(typeof(IFormattingStrategy));
				if (formater != null && formater.Length > 0) {
					formater[0].Document = Document;
					Document.FormattingStrategy = formater[0];
				}
			} catch (TreePathNotFoundException) {
				Console.WriteLine(formatingStrategyPath + " doesn't exists in the AddInTree");
			}
		}
	}
}
