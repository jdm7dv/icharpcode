// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractViewContent : IViewContent
	{
		IWorkbenchWindow workbenchWindow = null;

		string untitledName = "";
		string contentName  = null;
		
		bool   isDirty  = false;
		bool   isViewOnly = false;
		
		public virtual IWorkbenchWindow WorkbenchWindow {
			get {
				return workbenchWindow;
			}
			set {
				workbenchWindow = value;
			}
		}
		
		public virtual string UntitledName {
			get {
				return untitledName;
			}
			set {
				untitledName = value;
			}
		}
		
		public virtual string ContentName {
			get {
				return contentName;
			}
			set {
				contentName = value;
				OnContentNameChanged(null);
			}
		}
		
		public bool IsUntitled {
			get {
				return contentName == null;
			}
		}
		
		public virtual bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
				OnDirtyChanged(null);
			}
		}
		
		public virtual bool IsReadOnly {
			get {
				return false;
			}
		}		
		
		public virtual bool IsViewOnly {
			get {
				return isViewOnly;
			}
			set {
				isViewOnly = value;
			}
		}
		
		public abstract Control Control {
			get;
		}
		
		public virtual void RedrawContent()
		{
		}
		
		public virtual void Dispose()
		{
		}
		
		public virtual void SaveFile()
		{
			SaveFile(contentName);
		}
		
		public virtual void SaveFile(string fileName)
		{
			throw new System.NotImplementedException();
		}
		
		public abstract void LoadFile(string fileName);
				
		protected virtual void OnDirtyChanged(EventArgs e)
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, e);
			}
		}
				
		protected virtual void OnContentNameChanged(EventArgs e)
		{
			if (ContentNameChanged != null) {
				ContentNameChanged(this, e);
			}
		}
		
		public event EventHandler ContentNameChanged;
		public event EventHandler DirtyChanged;
	}
}
