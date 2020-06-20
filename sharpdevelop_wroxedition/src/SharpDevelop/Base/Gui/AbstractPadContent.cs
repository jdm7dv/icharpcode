// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractPadContent : IPadContent
	{
		string title;
		Bitmap icon;
		
		public abstract Control Control {
			get;
		}
		
		public virtual string Title {
			get {
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				return stringParserService.Parse(title);
			}
		}
		
		public virtual Bitmap Icon {
			get {
				return icon;
			}
		}
		
		public AbstractPadContent(string title) : this(title, null)
		{
		}
		
		public AbstractPadContent(string title, string iconResoureName)
		{
			this.title = title;
			if (iconResoureName != null) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				this.icon  = resourceService.GetBitmap(iconResoureName);
			}
		}
		
		public virtual void RedrawContent()
		{
			
		}
		
		public virtual void Dispose()
		{
			if (icon != null) {
				icon.Dispose();
			}
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		
		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}
		
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;
	}
}
