// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.XmlForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ScrollBox : UserControl
	{
		Image  image;
		string text;
		Timer  timer;
		int    scroll = -220;
		
		public int ScrollY {
			get {
				return scroll;
			}
			set {
				scroll = value;
			}
		}
		
		public Image Image {
			get {
				return image;
			}
			set {
				image = value;
			}
		}
		
		public string ScrollText {
			get {
				return text;
			}
			set {
				text =  value;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				timer.Stop();
			}
			base.Dispose(disposing);
		}
		
		public ScrollBox()
		{
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			Font = resourceService.LoadFont("Tahoma", 10);
			
			text = "\"The most successful method of programming is to begin a program as simply as possible, test it, and then add to the program until it performs the required job.\"\n    -- PDP8 handbook, Pg 9-64";
			timer = new Timer();
			timer.Interval = 80;
			timer.Tick += new EventHandler(ScrollDown);
			timer.Start();
		}
		
		void ScrollDown(object sender, EventArgs e)
		{
			++scroll;
			Refresh();
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			if (image != null) {
				pe.Graphics.DrawImage(image, 0, 0, Width, Height);
			}
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;
			g.DrawString(text, Font, new SolidBrush(Color.Black), new Rectangle(Width / 2, 0 - scroll, Width / 2, Height));
			SizeF size = g.MeasureString(text, Font);
			if (scroll > (int)(size.Height + Height)) {
				scroll = -(int)size.Height - Height;
			}
		}
	}
	
	public class CommonAboutDialog : XmlForm
	{
		public ScrollBox ScrollBox {
			get {
				return (ScrollBox)ControlDictionary["aboutPictureScrollBox"];
			}
		}
		
		public CommonAboutDialog() : base(Application.StartupPath + @"\..\data\resources\dialogs\CommonAboutDialog.xfrm")
		{
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
		}
	}
}
