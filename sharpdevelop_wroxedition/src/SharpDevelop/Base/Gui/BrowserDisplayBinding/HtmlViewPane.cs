// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Undo;
using System.Drawing.Printing;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserPane : AbstractViewContent
	{
		protected HtmlViewPane htmlViewPane;
		
		public override Control Control {
			get {
				return htmlViewPane;
			}
		}
		
		public override bool IsDirty {
			get {
				return false;
			}
			set {
			}
		}
		
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
		
		protected BrowserPane(bool showNavigation)
		{
			htmlViewPane = new HtmlViewPane(showNavigation);
			htmlViewPane.AxWebBrowser.TitleChange += new DWebBrowserEvents2_TitleChangeEventHandler(TitleChange);
		}
		
		public BrowserPane() : this(true)
		{
		}
		
		public override void Dispose()
		{
			htmlViewPane.Dispose();
		}
		
		public override void LoadFile(string url)
		{
			htmlViewPane.Navigate(url);
		}
		
		public override void SaveFile(string url)
		{
			LoadFile(url);
		}
		
		void TitleChange(object sender, DWebBrowserEvents2_TitleChangeEvent e)
		{
			ContentName = e.text;
		}
	}
	
	public class HtmlViewPane : UserControl
	{
		AxWebBrowser axWebBrowser = null;
		
		ToolBar toolBar    = new ToolBar();
		TextBox urlTextBox = new TextBox();
		
		bool   isHandleCreated  = false;
		string lastUrl     = null;
		
		public AxWebBrowser AxWebBrowser {
			get {
				return axWebBrowser;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				axWebBrowser.Dispose();
			}
		}
		
		public HtmlViewPane(bool showNavigation)
		{
			Dock = DockStyle.Fill;
			Size = new Size(500, 500);
			
			if (showNavigation) {
				
//				urlTextBox.Location  = new Point(0, 26);
//				urlTextBox.Size      = new Size(Width, 24);
				urlTextBox.KeyPress += new KeyPressEventHandler(KeyPressEvent);
				
				Controls.Add(urlTextBox);
				urlTextBox.Dock = DockStyle.Top;

				for (int i = 0; i < 4; ++i) {
					ToolBarButton toolBarButton = new ToolBarButton();
					toolBarButton.ImageIndex    = i;
					toolBar.Buttons.Add(toolBarButton);
				}
				
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				toolBar.ImageList = new ImageList();
				toolBar.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.BrowserBefore"));
				toolBar.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.BrowserAfter"));
				toolBar.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.BrowserCancel"));
				toolBar.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.BrowserRefresh"));
				
				toolBar.Appearance = ToolBarAppearance.Flat;
				toolBar.Dock = DockStyle.Top;
				toolBar.ButtonClick += new ToolBarButtonClickEventHandler(ToolBarClick);
				
				Controls.Add(toolBar);
			} 
			
			axWebBrowser = new AxWebBrowser();
			axWebBrowser.BeginInit();
			if (showNavigation) {
				int height = 48;
				axWebBrowser.Location = new Point(0, height);
				axWebBrowser.Size     = new Size(Width, Height - height); 
				axWebBrowser.Anchor   = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
			} else {
				axWebBrowser.Dock = DockStyle.Fill;
			}
			axWebBrowser.HandleCreated += new EventHandler(this.CreatedWebBrowserHandle);
			axWebBrowser.TitleChange   += new DWebBrowserEvents2_TitleChangeEventHandler(TitleChange);
			
			Controls.Add(axWebBrowser);
			axWebBrowser.EndInit();
		}
		
		void TitleChange(object sender, DWebBrowserEvents2_TitleChangeEvent e)
		{
			urlTextBox.Text = axWebBrowser.LocationURL;
		}
		
		void KeyPressEvent(object sender, KeyPressEventArgs ex)
		{
			if (ex.KeyChar == '\r') {
				Navigate(urlTextBox.Text);
			}
		}
		
		void ToolBarClick(object sender, ToolBarButtonClickEventArgs e)
		{
			try {
				switch(toolBar.Buttons.IndexOf(e.Button)) {
					case 0:
						axWebBrowser.GoBack();
						break;
					case 1:
						axWebBrowser.GoForward();
						break;
					case 2:
						axWebBrowser.Stop();
						break;
					case 3:
						axWebBrowser.CtlRefresh();
						break;
				}
			} catch (Exception) {
			}
		}
		
		public void CreatedWebBrowserHandle(object sender, EventArgs evArgs) 
		{
			isHandleCreated = true;
			if (lastUrl != null) {
				Navigate(lastUrl);
			}
		}
		
		public void Navigate(string name)
		{
			if (!isHandleCreated) {
				lastUrl = name;
				return;
			}
			urlTextBox.Text = name;
			object arg1 = 0; 
			object arg2 = ""; 
			object arg3 = ""; 
			object arg4 = "";
			try {
				axWebBrowser.Navigate(name, ref arg1, ref arg2, ref arg3, ref arg4);
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}
	}
}
