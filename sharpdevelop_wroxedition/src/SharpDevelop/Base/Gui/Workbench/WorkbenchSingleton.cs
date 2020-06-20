// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core.Properties;


using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.ErrorDialogs;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class WorkbenchSingleton : DefaultWorkbench
	{
		const string workbenchMemento        = "SharpDevelop.Workbench.WorkbenchMemento";
		const string uiIconStyle             = "IconMenuItem.IconMenuStyle";
		const string workbenchLayoutProperty = "SharpDevelop.Workbench.WorkbenchLayout";
		const string uiLanguageProperty      = "CoreProperties.UILanguage";
		const string layoutManagerPath       = "/SharpDevelop/Workbench/LayoutManager";
		
		static IWorkbench workbench    = null;
		static string currentLayout          = String.Empty;
		
		public static IWorkbench Workbench {
			get {
				if (workbench == null) {
					CreateWorkspace();
				}
				return workbench;
			}
		}
		
		static void SetWbLayout()
		{
			string layoutManager = propertyService.GetProperty(workbenchLayoutProperty, "MDI");
			if (layoutManager != currentLayout) {
				currentLayout = layoutManager;
				workbench.WorkbenchLayout = (IWorkbenchLayout)(ICSharpCode.Core.AddIns.AddInTreeSingleton.AddInTree.GetTreeNode(layoutManagerPath)).BuildChildItem(layoutManager, null);
			}
		}
		
		/// <remarks>
		/// This method handles the redraw all event for specific changed IDE properties
		/// </remarks>
		static void TrackPropertyChanges(object sender, ICSharpCode.Core.Properties.PropertyEventArgs e)
		{
			if (e.OldValue != e.NewValue) {
				switch (e.Key) {
					case workbenchLayoutProperty:
						SetWbLayout();
						break;
					
					case "ICSharpCode.SharpDevelop.Gui.VisualStyle":
					case "CoreProperties.UILanguage":
						workbench.RedrawAllComponents();
						break;
				}
			}
		}
		
		static void CreateWorkspace()
		{
			DefaultWorkbench w = new DefaultWorkbench();
			workbench = w;
//				TextTemplate.LoadTextTemplates();
//				CodeTemplateLoader.LoadTemplates();
//				ProjectTemplate.LoadProjectTemplates();
//				FileTemplate.LoadFileTemplates();
//				LanguageDefinition.LoadLanguageDefinitions();
				
//				EditActionLoader.LoadEditActions();
//				DefaultHighlightingStrategy.LoadSyntaxDefinitions();
//				FontContainer.LoadFonts();
//				CodonManager.LoadCodons();
//				FileUtility.LoadImageList(); // ProjectManager MUST load the language information before the fileutility can load
//				ClassScoutIcons.InitIcons();								
			
			w.InitializeWorkspace();
			w.SetMemento((IXmlConvertable)propertyService.GetProperty(workbenchMemento, new WorkbenchMemento()));
			w.UpdateViews(null, null);
			SetWbLayout();
			propertyService.PropertyChanged += new PropertyEventHandler(TrackPropertyChanges);
			workbench.RedrawAllComponents();
		}
		
		public static void SetTestMode()
		{
			workbench = new TestWorkbench();
			workbench.WorkbenchLayout = new TestLayoutManager();
		}
		
	}
}
