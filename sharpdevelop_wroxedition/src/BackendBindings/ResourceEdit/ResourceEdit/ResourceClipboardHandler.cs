// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
	
namespace ResEdit
{
	class ResourceClipboardHandler : IClipboardHandler
	{
		ResourceEdit resourceedit;
		
		public ResourceClipboardHandler(ResourceEdit resourceedit)
		{
			this.resourceedit = resourceedit;
		}
		
		public void Cut(object sender, EventArgs e)
		{
			if (resourceedit.WriteProtected || resourceedit.SelectedItems.Count < 1) 
				return;
			
			Hashtable tmphash = new Hashtable();
			foreach (ListViewItem item in resourceedit.SelectedItems) {
				tmphash.Add(item.Text, resourceedit.Resources[item.Text]);
				resourceedit.Resources.Remove(item.Text);
			}
			resourceedit.InitializeListView();
			Clipboard.SetDataObject(tmphash);
		}
		
		public void Copy(object sender, EventArgs e)
		{
			if (resourceedit.SelectedItems.Count < 1) 
				return;
			
			Hashtable tmphash = new Hashtable();
			foreach (ListViewItem item in resourceedit.SelectedItems) 
				tmphash.Add(item.Text, ((ICloneable)resourceedit.Resources[item.Text]).Clone()); // copy a clone to clipboard
			
			Clipboard.SetDataObject(tmphash);
		}
		
		public void Paste(object sender, EventArgs e)
		{
			if (resourceedit.WriteProtected)
				return;
			IDataObject dob = Clipboard.GetDataObject();
			if (dob.GetDataPresent(typeof(Hashtable).FullName)) {
				Hashtable tmphash = (Hashtable)dob.GetData(typeof(Hashtable));
				foreach (DictionaryEntry entry in tmphash) {
					if (!resourceedit.Resources.ContainsKey(entry.Key)) {
						resourceedit.Resources.Add(entry.Key, ((ICloneable)entry.Value).Clone()); // paste a clone in resources
					}
				}
				resourceedit.InitializeListView();
			}
		}
		
		public void Delete(object sender, EventArgs e)
		{
			if (resourceedit.WriteProtected)
				return;
			foreach (ListViewItem item in resourceedit.SelectedItems)
				if (item.Text != null) {
					resourceedit.Resources.Remove(item.Text);
				}
			resourceedit.InitializeListView();
		}
		
		public void SelectAll(object sender, EventArgs e)
		{/* TODO TODO
			foreach (ListViewItem i in Items)
				SelectedItems.Add(i);*/
		}
	}
}
