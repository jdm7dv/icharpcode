// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;

using Crownwood.Magic.Menus;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SdMenuCommand : MenuCommand
	{
//		string description = String.Empty;
//		
//		public string Description {
//			get {
//				return description;
//			}
//			set {
//				description = value;
//			}
//		}
		
		public SdMenuCommand(string label) : base(label)
		{
		}
		public SdMenuCommand(string label, EventHandler handler) : base(label, handler)
		{
		}
	}
}
