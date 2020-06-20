// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	class MenuCommandService : IMenuCommandService
	{
		IDesignerHost host;
		ArrayList     commands = new ArrayList();
		ArrayList     verbs    = new ArrayList();
		
		public DesignerVerbCollection Verbs {
			get {
				DesignerVerbCollection verbCollection = CreateDesignerVerbCollection();
				verbCollection.AddRange((DesignerVerb[])verbs.ToArray(typeof(DesignerVerb)));
				return verbCollection;
			}
		}
		
		public MenuCommandService(IDesignerHost host)
		{
			this.host = host;
		}
		
		public void AddCommand(MenuCommand command)
		{
			if (command != null && command.CommandID != null && !commands.Contains(command)) {
				this.commands.Add(command);
			}
		}
		
		public void AddVerb(DesignerVerb verb)
		{
			if (verb != null) {
				this.verbs.Add(verb);
			}
		}
		
		public void RemoveCommand(MenuCommand command)
		{
			if (command != null) {
				commands.Remove(command.CommandID);
			}
		}
		
		public void RemoveVerb(DesignerVerb verb)
		{
			if (verb != null) {
				verbs.Remove(verb);
			}
		}
		
		public bool GlobalInvoke(CommandID commandID)
		{
			MenuCommand menuCommand = FindCommand(commandID);
			if (menuCommand == null) {
				MessageBox.Show("Can't find command " + commandID, "Error");
				return false;
			}
			
			menuCommand.Invoke();
			return true;
		}
		
		public MenuCommand FindCommand(CommandID commandID)
		{
//			if (StringType.StrCmp(MenuUtilities.GetCommandNameFromCommandID(commandID), "", false) == 0 && StringType.StrCmp(commandID.ToString(), "74d21313-2aee-11d1-8bfb-00a0c90f26f7 : 12288", false) == 0) {
//				return MenuUtilities.gPropertyGridResetCommand;
//			}
			
			foreach (MenuCommand menuCommand in commands) {
				if (menuCommand.CommandID == commandID) {
					return menuCommand;
				}
			}
			
			foreach (DesignerVerb verb in Verbs) {
				if (verb.CommandID == commandID) {
					return verb;
				}
			}
			return null;
		}
		
		public void ShowContextMenu(CommandID menuID, int x, int y)
		{
			string contextMenuPath = "/SharpDevelop/FormsDesigner/ContextMenus/";
			
			if (menuID == MenuCommands.ComponentTrayMenu) {
				contextMenuPath += "ComponentTrayMenu";
			} else if (menuID == MenuCommands.ContainerMenu) {
				contextMenuPath += "ContainerMenu";
			} else if (menuID == MenuCommands.SelectionMenu) {
				contextMenuPath += "SelectionMenu";
			} else if (menuID == MenuCommands.TraySelectionMenu) {
				contextMenuPath += "TraySelectionMenu";
			} else {
				throw new Exception();
			}
			
			ICSharpCode.Core.AddIns.Codons.PopupMenuManager.ShowPopupMenu(this, contextMenuPath, x, y);
		}
		
		public DesignerVerbCollection CreateDesignerVerbCollection()
		{
			DesignerVerbCollection designerVerbCollection = new DesignerVerbCollection();
			
			bool isRootComponentSelected = false;
			int  designerVerbCollectionCount = 0;
			if (host != null) {
				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
				
				if (selectionService != null && selectionService.SelectionCount == 1) {
					object selectedComponent = selectionService.PrimarySelection;
					if (selectedComponent is Component && !(TypeDescriptor.GetAttributes(selectedComponent).Contains(InheritanceAttribute.InheritedReadOnly))) {
						IDesigner designer = host.GetDesigner((IComponent)selectedComponent);
						if (designer != null) {
							designerVerbCollection = designer.Verbs;
							if (designerVerbCollection != null) {
								designerVerbCollectionCount += designerVerbCollection.Count;
							}
						}
						if (selectedComponent == host.RootComponent) {
							isRootComponentSelected = true;
							designerVerbCollectionCount += verbs.Count;
						}
					}
				}
			}
			
			if (designerVerbCollectionCount > 0) {
				System.ComponentModel.Design.DesignerVerb[] verbArray = null;
				if (isRootComponentSelected && verbs != null) {
					verbArray = (DesignerVerb[])this.verbs.ToArray(typeof(DesignerVerb));
				}
				if (designerVerbCollection != null && designerVerbCollection.Count > 0) {
					int verbsCount = 0;
					if (isRootComponentSelected) {
						verbsCount = this.verbs.Count;
					}
					verbArray = new DesignerVerb[designerVerbCollection.Count];
					designerVerbCollection.CopyTo(verbArray, 0);
					if (isRootComponentSelected) {
						int lastVerb = this.verbs.Count - 1;
						while (lastVerb >= 0) {
							int curVerbIndex = designerVerbCollection.Count - 1;
							while (curVerbIndex >= 0) {
								if (verbArray[lastVerb].Text == designerVerbCollection[curVerbIndex].Text) {
									verbArray[lastVerb] = null;
									--designerVerbCollectionCount;
									break;
								}
								curVerbIndex--;
							}
							lastVerb--;
						}
						
						if (verbArray.Length != designerVerbCollectionCount) {
							System.ComponentModel.Design.DesignerVerb[] cleanVerbArray = new DesignerVerb[designerVerbCollectionCount];
							int i = 0;
							int j = 0;
							while (j <= verbArray.Length - 1) {
								if (verbArray[j] != null) {
									cleanVerbArray[i] = verbArray[j];
									++i;
								}
								j++;
							}
							verbArray = cleanVerbArray;
						}
					}
				}
				return new DesignerVerbCollection(verbArray);
			}
			return new DesignerVerbCollection();
		}
	}
}
