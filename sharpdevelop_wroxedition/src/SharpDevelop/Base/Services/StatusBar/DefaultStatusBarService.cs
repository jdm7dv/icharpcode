// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DefaultStatusBarService : AbstractService, IStatusBarService
	{
		SdStatusBar statusBar = null;
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		public DefaultStatusBarService()
		{
			statusBar = new SdStatusBar(this);
		}
		
		public void Dispose()
		{
			if (statusBar != null) {
				statusBar.Dispose();
				statusBar = null;
			}
		}
		
		public Control Control {
			get {
				Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		
		public IProgressMonitor ProgressMonitor {
			get { 
				Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		
		public bool CancelEnabled {
			get {
				return statusBar != null && statusBar.CancelEnabled;
			}
			set {
				Debug.Assert(statusBar != null);
				statusBar.CancelEnabled = value;
			}
		}
		public void SetCaretPosition(int x, int y, int charOffset)
		{
			statusBar.CursorStatusBarPanel.Text = String.Format("ln {0,-10} col {1,-5} ch {2,-5}", y, x, charOffset);
		}
		
		public void SetInsertMode(bool insertMode)
		{
			statusBar.ModeStatusBarPanel.Text = insertMode ? "INS" : "OVR";
		}
		
		public void ShowErrorMessage(string message)
		{
			Debug.Assert(statusBar != null);
			statusBar.ShowErrorMessage(stringParserService.Parse(message));
		}
		
		public void SetMessage(string message)
		{
			Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(stringParserService.Parse(message));
		}
		
		public void SetMessage(Image image, string message)
		{
			Debug.Assert(statusBar != null);
			statusBar.SetMessage(image, stringParserService.Parse(message));
		}
		
		bool   wasError    = false;
		string lastMessage = "";
		public void RedrawStatusbar()
		{
			if (wasError) {
				ShowErrorMessage(lastMessage);
			} else {
				SetMessage(lastMessage);
			}
		}
		
		public void Update()
		{
			Debug.Assert(statusBar != null);
	/*		statusBar.Panels.Clear();
			statusBar.Controls.Clear();
			
			foreach (StatusBarContributionItem item in Items) {
				if (item.Control != null) {
					statusBar.Controls.Add(item.Control);
				} else if (item.Panel != null) {
					statusBar.Panels.Add(item.Panel);
				} else {
					throw new ApplicationException("StatusBarContributionItem " + item.ItemID + " has no Control or Panel defined.");
				}
			}*/
		}
	}
}
