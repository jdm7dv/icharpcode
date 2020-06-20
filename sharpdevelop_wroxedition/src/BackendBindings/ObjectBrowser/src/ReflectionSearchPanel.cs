// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Resources;
using System.Reflection.Emit;

using SharpDevelop.Gui.Edit;

namespace SharpDevelop.Gui.Edit.Reflection
{
	public class ReflectionSearchPanel : UserControl
	{
		System.Windows.Forms.Label     searchfor       = new System.Windows.Forms.Label();
		
		TextBox   searchstringbox = new TextBox();
		ListView  itemsfound      = new ListView();
		Button    button = new Button();
		ReflectionTree tree;
		ObjectBrowser.DisplayInformationWrapper _parent;
		
		public ReflectionSearchPanel(ReflectionTree tree)
		{
			Dock = DockStyle.Fill;
			
			this.tree = tree;
			
			searchfor.Text     = "Search for:";
			searchfor.Location = new Point(0, 4);
			searchfor.Width    = 100;
			searchfor.Height   = 12;
			searchfor.Anchor   = AnchorStyles.Top | AnchorStyles.Left;
			
			searchstringbox.Location = new Point(0, 12 + 5);
			searchstringbox.AcceptsReturn = true;
			searchstringbox.Width    = Width - 60;
			searchstringbox.Height   = 30;
			searchstringbox.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			
			itemsfound.Location = new Point(0, 10 + 30);
			itemsfound.Width    = Width;
			itemsfound.FullRowSelect = true;
			itemsfound.MultiSelect = false;
			itemsfound.Height   = Height - 70;
			itemsfound.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			itemsfound.View     = View.Details;
			itemsfound.Columns.Add("Name",      100, HorizontalAlignment.Left);
			itemsfound.Columns.Add("Type",      100, HorizontalAlignment.Left);
			itemsfound.Columns.Add("Namespace", 150, HorizontalAlignment.Left);
			itemsfound.Columns.Add("Assembly",  100, HorizontalAlignment.Left);
			itemsfound.DoubleClick += new EventHandler(SelectItem);
			itemsfound.SmallImageList = tree.ImageList;
			
			button.Size = new Size(52, 21);
			button.Text = "Search";
			button.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
			button.Location = new Point(Width - 55, 17);
			
			button.Click += new EventHandler(Showtypes);
			Controls.Add(button);
			Controls.Add(searchfor);
			Controls.Add(searchstringbox);
			Controls.Add(itemsfound);
			
		}
		
		public ObjectBrowser.DisplayInformationWrapper ParentDisplayInfo {
			get {
				return _parent;
			}
			set {
				_parent = value;
			}
		}
		
		void SelectItem(object sender, EventArgs e)
		{
			if (itemsfound.SelectedItems.Count != 1)
				return;
			
			if(itemsfound.SelectedItems[0] is TypeItem) {
				TypeItem item = (TypeItem)itemsfound.SelectedItems[0];
				tree.GoToType(item.type);
			} else if (itemsfound.SelectedItems[0] is MemberItem) {
				MemberItem member = (MemberItem)itemsfound.SelectedItems[0];
				tree.GoToMember(member.member, member.assembly);
			}
			ParentDisplayInfo.tctrl.SelectedTab = ParentDisplayInfo.tctrl.TabPages[0];
		}
		
		class TypeItem : ListViewItem {
			public Type type;
			public TypeItem(Type type) : base ( new string[] { type.Name, "Class", type.Namespace, Path.GetFileName(type.Assembly.CodeBase)})
			{
				this.type = type;
			}
		}

		
		class MemberItem : ListViewItem {
			public MemberInfo member;
			public Assembly assembly;

			public MemberItem(MemberInfo member, Assembly assembly) : base ( new string[] { member.DeclaringType.Name + "." + member.Name, GetType(member), "", Path.GetFileName(assembly.CodeBase)})
			{
				this.member = member;
				this.assembly = assembly;
			}
			
			private static string GetType(MemberInfo member) {
				if(member is MethodInfo) {
					return "Method";
				} else if(member is ConstructorInfo) {
					return "Constructor";
				} else if(member is FieldInfo) {
					return "Field";
				} else if(member is PropertyInfo) {
					return "Property";
				} else {
					return "unknown";
				}
			}
		}
		
		void Showtypes(object sender, EventArgs e)
		{
			itemsfound.Items.Clear();
			foreach (Assembly asm in tree.Assemblies) {
				Type[] types = asm.GetTypes();
				foreach (Type type in types) {
					if (type.Name.ToLower().IndexOf(searchstringbox.Text.ToLower()) >= 0) {
						itemsfound.Items.Add(new TypeItem(type));
					}

					MemberInfo[] members =type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
					foreach(MemberInfo member in members) {
						if(member.Name.ToLower().IndexOf(searchstringbox.Text.ToLower()) >= 0) {
							if(member is MethodInfo) {
								if (! ((MethodInfo)member).IsSpecialName) {
									itemsfound.Items.Add(new MemberItem(member, type.Assembly));
								}
							} else {
								itemsfound.Items.Add(new MemberItem(member, type.Assembly));
							}
						}
					}
				}
			}
		}
	}
}
