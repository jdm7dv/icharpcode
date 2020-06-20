// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Threading;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class CreationFinishedPanel : AbstractWizardPanel
	{
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
	    	return true;
		}
		
		public CreationFinishedPanel() : base(Application.StartupPath + @"\..\data\resources\panels\CompletionDatabaseWizard\CreationFinishedPanel.xfrm")
		{
			EnableFinish      = true;
			EnableNext        = false;
			EnablePrevious    = false;
			EnableCancel      = false;
			IsLastPanel       = true;
		}
	}
}
