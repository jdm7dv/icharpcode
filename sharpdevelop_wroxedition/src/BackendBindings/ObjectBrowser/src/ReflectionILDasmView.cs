// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;

namespace SharpDevelop.Gui.Edit.Reflection
{
	public class ReflectionILDasmView : UserControl
	{
		RichTextBox tb = new RichTextBox();
		ReflectionTree tree;
		
		public ReflectionILDasmView(ReflectionTree tree)
		{
			this.tree = tree;
			
			Dock = DockStyle.Fill;
			tb.Font = new System.Drawing.Font("Courier New", 10);
			tb.Dock = DockStyle.Fill;
			tb.ScrollBars = RichTextBoxScrollBars.Both;
			
			tb.WordWrap  = false;
			tb.ReadOnly  = true;
			Controls.Add(tb);
			
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			ReflectionNode node = (ReflectionNode)e.Node;
			
			Assembly assembly = null;
			string item = " /item=";
			
			if (node.Attribute is Assembly) {
				assembly = (Assembly)node.Attribute;
			} else if (node.Attribute is Type) {
				Type type = (Type)node.Attribute;
				item += type.FullName;
				assembly = type.Module.Assembly;
			} else if (node.Attribute is MethodBase) {
				MethodBase method = (MethodBase) node.Attribute;
				item += method.DeclaringType.FullName + "::" + method.Name;
				assembly = method.DeclaringType.Module.Assembly;
			} else {
				tb.Text = "<NO ILDASM VIEW AVAIBLE>";
				return;
			}
			tb.Text = GetILDASMOutput(assembly, item).Replace("\n", "\r\n");
		}
		
		private string GetILDASMOutput(Assembly assembly, string item)
		{
			try {
				string args = '"' + assembly.Location + '"' + item + " /NOBAR /TEXT";
				ProcessStartInfo psi = new ProcessStartInfo("ildasm.exe", args);
				
				psi.RedirectStandardError  = true;
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardInput  = true;
				psi.UseShellExecute        = false;
				psi.CreateNoWindow         = true;
				
				Process process = Process.Start(psi);
				string output   = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				
				int cutpos = output.IndexOf(".namespace");
				
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				cutpos = output.IndexOf(".class");
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				cutpos = output.IndexOf(".method");
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				return output;
			} catch (Exception) {
				return "ildasm.exe is not installed\n(works only with the SDK)";
			}
		}
	}
}
