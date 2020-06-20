// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace CSharpBinding
{
	public class ChooseRuntimePanel : AbstractOptionPanel
	{
		CSharpCompilerParameters config = null;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (((RadioButton)ControlDictionary["msnetRadioButton"]).Checked) {
					config.NetRuntime =  NetRuntime.MsNet;
				} else if (((RadioButton)ControlDictionary["monoRadioButton"]).Checked) {
					config.NetRuntime =  NetRuntime.Mono;
				} else {
					config.NetRuntime =  NetRuntime.MonoInterpreter;
				}
			 	config.CsharpCompiler = ((RadioButton)ControlDictionary["cscRadioButton"]).Checked ? CsharpCompiler.Csc : CsharpCompiler.Mcs;
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.config = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			((RadioButton)ControlDictionary["msnetRadioButton"]).Checked = config.NetRuntime == NetRuntime.MsNet;
			((RadioButton)ControlDictionary["monoRadioButton"]).Checked  = config.NetRuntime == NetRuntime.Mono;
			((RadioButton)ControlDictionary["mintRadioButton"]).Checked  = config.NetRuntime == NetRuntime.MonoInterpreter;
			
			((RadioButton)ControlDictionary["cscRadioButton"]).Checked = config.CsharpCompiler == CsharpCompiler.Csc;
			((RadioButton)ControlDictionary["mcsRadioButton"]).Checked = config.CsharpCompiler == CsharpCompiler.Mcs;
		}
		
		public ChooseRuntimePanel() : base(Application.StartupPath + @"\..\data\resources\panels\ChooseRuntimePanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
	}
}
