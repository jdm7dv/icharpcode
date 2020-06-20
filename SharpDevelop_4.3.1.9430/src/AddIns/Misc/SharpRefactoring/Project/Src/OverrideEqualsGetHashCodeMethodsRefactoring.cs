﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using SharpRefactoring.Gui;

namespace SharpRefactoring
{
	public class OverrideEqualsGetHashCodeMethodsRefactoring : ICompletionItemHandler
	{
		public void Insert(CompletionContext context, ICompletionItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			
			if (!(item is OverrideCompletionItem))
				throw new ArgumentException("item is not an OverrideCompletionItem");
			
			OverrideCompletionItem completionItem = item as OverrideCompletionItem;
			
			ITextEditor textEditor = context.Editor;
			
			IEditorUIService uiService = textEditor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			
			if (uiService == null)
				return;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
			
			if (parseInfo == null)
				return;
			
			CodeGenerator generator = parseInfo.CompilationUnit.Language.CodeGenerator;
			IClass current = parseInfo.CompilationUnit.GetInnermostClass(textEditor.Caret.Line, textEditor.Caret.Column);
			ClassFinder finder = new ClassFinder(current, textEditor.Caret.Line, textEditor.Caret.Column);
			
			if (current == null)
				return;
			
			using (textEditor.Document.OpenUndoGroup()) {
				ITextAnchor startAnchor = textEditor.Document.CreateAnchor(textEditor.Caret.Offset);
				startAnchor.MovementType = AnchorMovementType.BeforeInsertion;
				
				ITextAnchor endAnchor = textEditor.Document.CreateAnchor(textEditor.Caret.Offset);
				endAnchor.MovementType = AnchorMovementType.AfterInsertion;
				
				MethodDeclaration member = (MethodDeclaration)generator.GetOverridingMethod(completionItem.Member, finder);
				
				string indent = DocumentUtilitites.GetWhitespaceBefore(textEditor.Document, textEditor.Caret.Offset);
				string codeForBaseCall = generator.GenerateCode(member.Body.Children.OfType<AbstractNode>().First(), "");
				string code = generator.GenerateCode(member, indent);
				int marker = code.IndexOf(codeForBaseCall);
				
				textEditor.Document.Insert(startAnchor.Offset, code.Substring(0, marker).TrimStart());
				
				ITextAnchor insertionPos = textEditor.Document.CreateAnchor(endAnchor.Offset);
				insertionPos.MovementType = AnchorMovementType.BeforeInsertion;
				
				InsertionContext insertionContext = new InsertionContext(textEditor.GetService(typeof(TextArea)) as TextArea, startAnchor.Offset);
				
				AbstractInlineRefactorDialog dialog = new OverrideEqualsGetHashCodeMethodsDialog(insertionContext, textEditor, startAnchor, endAnchor, insertionPos, current, completionItem.Member as IMethod, codeForBaseCall.Trim());
				dialog.Element = uiService.CreateInlineUIElement(insertionPos, dialog);
				
				textEditor.Document.InsertNormalized(endAnchor.Offset, Environment.NewLine + code.Substring(marker + codeForBaseCall.Length));
				
				insertionContext.RegisterActiveElement(new InlineRefactorSnippetElement(cxt => null, ""), dialog);
				insertionContext.RaiseInsertionCompleted(EventArgs.Empty);
			}
		}
		
		public bool Handles(ICompletionItem item)
		{
			return item is OverrideCompletionItem && (item.Text == "GetHashCode()" || item.Text == "Equals(object obj)");
		}
	}
}
