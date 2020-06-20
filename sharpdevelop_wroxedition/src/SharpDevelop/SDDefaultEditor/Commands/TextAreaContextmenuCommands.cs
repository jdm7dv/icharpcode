// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using Crownwood.Magic.Menus;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ShowBufferOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			
			TabbedOptions o = new TabbedOptions(textarea.Document.Properties, AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/ViewContent/DefaultTextEditor/OptionsDialog"));
			o.Width  = 450;
			o.Height = 425;
			o.FormBorderStyle = FormBorderStyle.FixedDialog;
			o.ShowDialog();
			o.Dispose();
			textarea.OptionsChanged();
		}
	}
	
	
	public class HighlightingTypeBuilder : ISubmenuBuilder
	{
		TextAreaControl control      = null;
		MenuCommand[]   menuCommands = null;
		
		public MenuCommand[] BuildSubmenu(object owner)
		{
			control = (TextAreaControl)owner;
//			IconMenuStyle iconMenuStyle = (IconMenuStyle)propertyService.GetProperty("IconMenuItem.IconMenuStyle", IconMenuStyle.VSNet);
			
			ArrayList menuItems = new ArrayList();
			
			foreach (DictionaryEntry entry in HighlightingManager.Manager.HighlightingDefinitions) {
				IHighlightingStrategy syntax = (IHighlightingStrategy)entry.Value;
				SdMenuCommand item = new SdMenuCommand(syntax.Name, new EventHandler(ChangeSyntax));
				item.Checked = control.Document.HighlightingStrategy == syntax;
				menuItems.Add(item);
			}
			menuCommands = (MenuCommand[])menuItems.ToArray(typeof(MenuCommand));
			return menuCommands;
		}
		
		void ChangeSyntax(object sender, EventArgs e)
		{
			if (control != null) {
				SdMenuCommand item = ((SdMenuCommand)sender);
				foreach (SdMenuCommand i in menuCommands) {
					i.Checked = false;
				}
				item.Checked = true;
				IHighlightingStrategy strat = HighlightingStrategyFactory.CreateHighlightingStrategy(item.Text);
				if (strat == null) {
					throw new Exception("Strategy can't be null");
				}
				control.Document.HighlightingStrategy = strat;
				control.Refresh();
			}
		}
	}	
}
