// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Resources;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Undo;
using System.Drawing.Printing;

namespace ResEdit
{
	/// <summary>
	/// This class allows viewing and editing of windows resource files
	/// both in XML as in normal format.
	/// </summary>
	public class ResourceEdit : ListView
	{
		ColumnHeader name     = new ColumnHeader();
		ColumnHeader type     = new ColumnHeader();
		ColumnHeader content  = new ColumnHeader();
		
		Hashtable resources = new Hashtable();
		
		ResourceClipboardHandler clipboardhandler = null;
		UndoStack                undostack        = null;
		bool                     writeprotected   = false;
		public event EventHandler         Changed;
		
		
		public bool      WriteProtected {
			get {
				return writeprotected;
			}
			set {
				writeprotected = value;
			}
		}
		
		public Hashtable Resources {
			get {
				return resources;
			}
		}
		
		public UndoStack UndoStack {
			get {
				return undostack;
			}
		}
		public IClipboardHandler ClipboardHandler {
			get {
				return clipboardhandler;
			}
		}
		
		public PrintDocument PrintDocument {
			get { // TODO
				return null; 
			}
		}
		
		public ResourceEdit()
		{
			clipboardhandler = new ResourceClipboardHandler(this);
			undostack        = new UndoStack();
			
			name.Text     = "Name";
			type.Text     = "Type";
			content.Text  = "Content";
			name.Width    = 100;
			type.Width    = 200;
			content.Width = 400;
			Columns.Add(name);
			Columns.Add(type);
			Columns.Add(content);
			
			FullRowSelect = true;
			AutoArrange   = true;
			Alignment     = ListViewAlignment.Left;
			View          = View.Details;
			HeaderStyle   = ColumnHeaderStyle.Nonclickable;
//			GridLines     = true;
			Activation    = ItemActivation.TwoClick;
			Sorting       = SortOrder.Ascending;
			Dock          = DockStyle.Fill;
			
			ItemActivate += new EventHandler(ClickItem);
			
			ContextMenu = new ContextMenu(new MenuItem[] {
											new MenuItem("&New string entry", new EventHandler(NewEvt)),
											new MenuItem("-"),
											new MenuItem("Add &files",        new EventHandler(AddEvt)),
											new MenuItem("Save &As...", new EventHandler(SaveAsEvt)),
											new MenuItem("-"),
											new MenuItem("&Rename",        new EventHandler(RenameEvt), Shortcut.F2),
											new MenuItem("-"),
											new MenuItem("Cu&t",    new EventHandler(clipboardhandler.Cut)),
											new MenuItem("&Copy",   new EventHandler(clipboardhandler.Copy)),
											new MenuItem("&Paste",  new EventHandler(clipboardhandler.Paste)),
											new MenuItem("&Delete", new EventHandler(clipboardhandler.Delete)),
											new MenuItem("-"),
											new MenuItem("Select &All", new EventHandler(clipboardhandler.SelectAll))
			});
		}
		
		public void LoadFile(string filename)
		{
			Stream s      = File.OpenRead(filename);
			switch (Path.GetExtension(filename).ToUpper()) {
				case ".RESX":
					ResXResourceReader rx = new ResXResourceReader(s);
//                  This don't work, because of a bug in the current CSharp implementation, I think
//                  try it again next release, must use ugly version.
//					foreach (DictionaryEntry entry in rx) {
//						if (!resources.ContainsKey(entry.Key))
//							resources.Add(entry.Key, entry.Value);
//					}
					
					// ugly version (from Framework Reference)
					IDictionaryEnumerator n = rx.GetEnumerator();
					while (n.MoveNext()) 
						if (!resources.ContainsKey(n.Key))
							resources.Add(n.Key, n.Value);
					
					rx.Close();
				break;
				case ".RESOURCES":
					ResourceReader rr = new ResourceReader(s);
					foreach (DictionaryEntry entry in rr) {
						if (!resources.ContainsKey(entry.Key))
							resources.Add(entry.Key, entry.Value);
					}
					rr.Close();
				break;
			}
			s.Close();
			InitializeListView();
		}
		
		public void SaveFile(string filename)
		{
			Debug.Assert(!writeprotected, "ICSharpCode.SharpDevelop.Gui.Edit.Resource.ResourceEdit.SaveFile(string filename) : trying to save a write protected file");
			switch (Path.GetExtension(filename).ToUpper()) {
				case ".RESX":		// write XML resource
					ResXResourceWriter rxw    = new ResXResourceWriter(filename);
					foreach (DictionaryEntry entry in resources) 
						if (entry.Value != null)
							rxw.AddResource(entry.Key.ToString(), entry.Value);
					rxw.Generate();
					rxw.Close();
				break;
				
				default:			// write default resource
					ResourceWriter rw = new ResourceWriter(filename);
					foreach (DictionaryEntry entry in resources) {
						rw.AddResource(entry.Key.ToString(), entry.Value);
					}
					rw.Generate();
					rw.Close();
				break;
			}
		}
		
		
		public void NewEvt(object sender, EventArgs e)
		{
			EditEntry ed = new EditEntry();
			if (ed.ShowDialog() == DialogResult.OK) {
				resources.Add(ed.Entry.Key, ed.Entry.Value);
				InitializeListView();
			}
			ed.Dispose();
			OnChanged();
		}
		
		public void AddEvt(object sender, EventArgs e)
		{
			if (writeprotected)
				return;
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.AddExtension   = true;
			fdiag.Filter         = "All files (*.*)|*.*";
			fdiag.Multiselect    = true;
			fdiag.CheckFileExists = true;
			
			if (fdiag.ShowDialog() == DialogResult.OK) {
				foreach (string filename in fdiag.FileNames) {
					string resname = Path.ChangeExtension(Path.GetFileName(filename), null);
					if ((resname != "") && (!resources.ContainsKey(resname))) {
						object tmp = LoadResource(filename);
						if (tmp != null) 
							resources.Add(resname, tmp);
					}
				}
				InitializeListView();
			}
			fdiag.Dispose();
			OnChanged();
		}
		
		/// <summary>
		/// Jeff Huntsman: added small utility SaveAs
		/// SaveAsEvt saves the current selected resource 
		/// depending on its type.
		/// </summary>
		/// <returns>void</returns>
		public void SaveAsEvt(object sender, EventArgs e)
		{
			if (SelectedItems.Count != 1) 
				return;
			
			string key = SelectedItems[0].Text;
			if (!resources.ContainsKey(key)) 
				return;
			
			object val = resources[key];
			
			SaveFileDialog sdialog 	= new SaveFileDialog();
			sdialog.AddExtension 	= true;			
			sdialog.FileName 		= key;
			if(val is Bitmap) {
				sdialog.Filter 		= "Bitmap files (*.bmp)|*.bmp";
				sdialog.DefaultExt 	= ".bmp";
			}
			else if(val is Icon) {
				sdialog.Filter 		= "Icon files (*.ico)|*.ico";
				sdialog.DefaultExt 	= ".ico";
			}
			else if(val is Cursor) {
				sdialog.Filter 		= "Cursor files (*.cur)|*.cur";
				sdialog.DefaultExt 	= ".cur";
			}
			else
				return;
			
			if(sdialog.ShowDialog() == DialogResult.OK) {			
				try {
					Image img = (Image)val;
					img.Save(sdialog.FileName);
				} catch(Exception ex) {
					MessageBox.Show(ex.Message, "Can't save resource to " + sdialog.FileName, MessageBoxButtons.OK); 
				}							
			}
		}
		
		
		void OnChanged()
		{
			if (Changed != null) {
				Changed(this, null);
			}
		}
						
		object LoadResource(string name)
		{
			
			switch (Path.GetExtension(name).ToUpper()) {
				case ".CUR":
					return new Cursor(name);
				case ".ICO":
					return new Icon(name);
				default:
					// try to read a bitmap
					try { 
						return new Bitmap(name); 
					} catch {}
					
					// try to read a serialized object
					try {
						Stream r = File.Open(name, FileMode.Open);
						try {
							BinaryFormatter c = new BinaryFormatter();
							object o = c.Deserialize(r);
							r.Close();
							return o;
						} catch { r.Close(); }
					} catch { }
					
					// try to read a byte array :)
					try {
						FileStream s = new FileStream(name, FileMode.Open);
						BinaryReader r = new BinaryReader(s);
						Byte[] d = new Byte[(int) s.Length];
						d = r.ReadBytes((int) s.Length);
						s.Close();
						return d;
					} catch (Exception e) { 
						MessageBox.Show(e.Message, "Can't load resource", MessageBoxButtons.OK); 
					}
				break;
			}
			return null;
		}
		
		
		public void RenameEvt(object sender, EventArgs e)
		{
			if (writeprotected || SelectedItems.Count != 1) 
				return;
			
			string key = SelectedItems[0].Text;
			if (!resources.ContainsKey(key)) 
				return;
			
			object val = resources[key];
			RenameEntry re = new RenameEntry(key);
			if (re.ShowDialog() == DialogResult.OK) {
				resources.Remove(key);
				resources.Add(re.Value, val);
				InitializeListView();
			}
			re.Dispose();
			OnChanged();
		}
		
		public void InitializeListView()
		{
			BeginUpdate();
			Items.Clear();
			foreach (DictionaryEntry entry in resources) {
				string tmp = (entry.Value.GetType() == typeof(String)) ? entry.Value.ToString() : "";
				Items.Add(new ListViewItem(new String[] {entry.Key.ToString(), entry.Value.GetType().FullName, tmp}));
			}
			EndUpdate();
		}
		
		protected void ClickItem(object sender, EventArgs e)
		{
			if (SelectedItems.Count != 1) 
				return;
			
			string key = SelectedItems[0].Text;
			if (!resources.ContainsKey(key)) 
				return;
			
			object val = resources[key];
			
			if (val is Icon) {
				BitmapView bv = new BitmapView("[" + key + "]", ((Icon)val).ToBitmap());
				bv.Show();
			} else
			if (val is Bitmap) {
				BitmapView bv = new BitmapView("[" + key + "]", (Bitmap)val);
				bv.Show();
			} else
			if (val is Cursor) {
				Cursor c = (Cursor)val;
				Bitmap a = new Bitmap(c.Size.Width, c.Size.Height);
				Graphics g = Graphics.FromImage(a);
				g.FillRectangle(new SolidBrush(Color.DarkCyan), 0, 0, a.Width, a.Height);
				c.Draw(g, new Rectangle(0, 0, a.Width, a.Height));
				BitmapView bv = new BitmapView(Text + " [" + key + "]", a);
				bv.Show();
			} else 
			if (val is string) {
				EditEntry ed = new EditEntry(key, val);
				if (writeprotected) {
					ed.Protect();
				}
				if (ed.ShowDialog() == DialogResult.OK) {
					resources.Remove(key);
					resources.Add(ed.Entry.Key, ed.Entry.Value);
					OnChanged();
					InitializeListView();
				}
				ed.Dispose();
			} else {
				MessageBox.Show("Unknown Entry, can't view/edit file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
	}
}

