// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class WindowCascade : AbstractMenuCommand
	{
		public override void Run()
		{
			((Form)WorkbenchSingleton.Workbench).LayoutMdi(MdiLayout.Cascade);
		}
	}
	
	public class WindowTileHorizontal : AbstractMenuCommand
	{
		public override void Run()
		{
			((Form)WorkbenchSingleton.Workbench).LayoutMdi(MdiLayout.TileHorizontal);
		}
	}
	
	public class WindowTileVertical : AbstractMenuCommand
	{
		public override void Run()
		{
			((Form)WorkbenchSingleton.Workbench).LayoutMdi(MdiLayout.TileVertical);
		}
	}
	
	public class WindowArrangeIcons : AbstractMenuCommand
	{
		public override void Run()
		{
			((Form)WorkbenchSingleton.Workbench).LayoutMdi(MdiLayout.ArrangeIcons);
		}
	}
	
	public class CloseAllWindows : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.CloseAllViews();
		}
	}
	
}
