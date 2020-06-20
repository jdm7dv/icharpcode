// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.FormDesigner.Util
{
	public sealed class WindowUtility
	{
		static IWin32Window mainWindow = null;
		
		public static IWin32Window MainWindow {
			get {
				return mainWindow;
			}
			
			set {
				if (mainWindow == null) {
					mainWindow = value;
				} else {
					throw new NotSupportedException("The MainWindow property  has already been set and may not be set more than once.");
				}
			}
		}
		
		WindowUtility()
		{
		}
	}
}
