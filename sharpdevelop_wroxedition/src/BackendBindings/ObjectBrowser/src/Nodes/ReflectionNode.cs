// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Gui.Edit;

namespace SharpDevelop.Gui.Edit.Reflection
{
	public enum ReflectionNodeType {
		Folder,
		Resource,
		Assembly,
		Library,
		Namespace,
		Type,
		Constructor,
		Method,
		Field,
		Property,
		SubTypes,
		SuperTypes,
		Reference,
		Event,
		Link,
		Module,
	}
	
	public class ReflectionNode : TreeNode
	{
		protected const int CLASSINDEX     = 14;
		protected const int STRUCTINDEX    = CLASSINDEX + 1 * 4;
		protected const int INTERFACEINDEX = CLASSINDEX + 2 * 4;
		protected const int ENUMINDEX      = CLASSINDEX + 3 * 4;
		protected const int METHODINDEX    = CLASSINDEX + 4 * 4;
		protected const int PROPERTYINDEX  = CLASSINDEX + 5 * 4;
		protected const int FIELDINDEX     = CLASSINDEX + 6 * 4;
		protected const int DELEGATEINDEX  = CLASSINDEX + 7 * 4;
		
		protected ReflectionNodeType type;
		protected string             name;
		protected object             attribute;
		
		protected bool  populated = false;
		
		public ReflectionNodeType Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public object Attribute {
			get {
				return attribute;
			}
		}
		
		public bool Populated {
			get {
				return populated;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public ReflectionNode(string name, object attribute, ReflectionNodeType type) : base(name)
		{
			this.attribute = attribute;
			this.type = type;
			this.name = name;
			if (type == ReflectionNodeType.Namespace ||
				type == ReflectionNodeType.Assembly  ||
				type == ReflectionNodeType.Library   ||
				type == ReflectionNodeType.Type) {
					Nodes.Add(new TreeNode(""));
			}
			if (type == ReflectionNodeType.Field) {
				Text = ReflectionTree.languageConversion.Convert((FieldInfo)attribute);
			} else
			if (type == ReflectionNodeType.Property) {
				Text = ReflectionTree.languageConversion.Convert((PropertyInfo)attribute);
			}
			SetIcon();
		}
		
	
		protected virtual void SetIcon()
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			
			switch (type) {
				case ReflectionNodeType.Link:
					break;
				case ReflectionNodeType.Resource:
					ImageIndex  = SelectedImageIndex = 11;
					break;
				
				case ReflectionNodeType.Reference:
					ImageIndex  = SelectedImageIndex = 8;
					break;
				
				// TODO : MODULE ICON
				case ReflectionNodeType.Module:
					ImageIndex  = SelectedImageIndex = 46;
					break;
				
				case ReflectionNodeType.SubTypes:
					ImageIndex  = SelectedImageIndex = 4;
					break;
					
				case ReflectionNodeType.SuperTypes:
					ImageIndex  = SelectedImageIndex = 5;
					break;
				
				case ReflectionNodeType.Event:
					EventInfo eventinfo = (EventInfo)attribute;
					ImageIndex  = SelectedImageIndex = classBrowserIconService.GetIcon(eventinfo);
					MethodInfo add    = eventinfo.GetAddMethod();
					MethodInfo raise  = eventinfo.GetRaiseMethod();
					MethodInfo remove = eventinfo.GetRemoveMethod();
					
					if (add != null)
						Nodes.Add(new ReflectionMethodNode(add));
					if (raise != null)
						Nodes.Add(new ReflectionMethodNode(raise));
					if (remove != null)
						Nodes.Add(new ReflectionMethodNode(remove));
					break;
				
				
				case ReflectionNodeType.Property:
					PropertyInfo propertyinfo = (PropertyInfo)attribute;
					ImageIndex  = SelectedImageIndex = classBrowserIconService.GetIcon(propertyinfo);
					
					if (propertyinfo.CanRead) {
						Nodes.Add(new ReflectionMethodNode(propertyinfo.GetGetMethod()));
					}
					if (propertyinfo.CanWrite) {
						Nodes.Add(new ReflectionMethodNode(propertyinfo.GetSetMethod()));
					}
					break;
				
				case ReflectionNodeType.Field:
					FieldInfo fieldinfo = (FieldInfo)attribute;
					ImageIndex = SelectedImageIndex = classBrowserIconService.GetIcon(fieldinfo);
					break;
				default:
					throw new Exception("ReflectionFolderNode.SetIcon : unknown ReflectionNodeType " + type.ToString());
			}
		}
		
		public virtual void Populate()
		{
			switch (type) {
				case ReflectionNodeType.Assembly:
					PopulateAssembly((Assembly)attribute, this);
					break;
				case ReflectionNodeType.Library:
					PopulateLibrary((Assembly)attribute, this);
					break;
			}
			populated = true;
		}
		
		public TreeNode GetNodeFromCollection(TreeNodeCollection collection, string title)
		{
			foreach (TreeNode node in collection)
				if (node.Text == title) {
					return node;
				}
			return null;
		}
		
		void PopulateLibrary(Assembly assembly, TreeNode parentnode)
		{
			parentnode.Nodes.Clear();
			string[] manifestresourcenames = assembly.GetManifestResourceNames();
			Type[]   types                 = assembly.GetTypes();
			foreach (Type type in types) {
				if(type.ToString().IndexOf("PrivateImplementationDetails") == -1) {
					string   typename  = type.ToString();
					int      lastindex = typename.LastIndexOf('.');
					TreeNode path      = parentnode;
					string   nodename  = typename;
					
					if (lastindex != -1) {
						string pathname = typename.Substring(0, lastindex);
						TreeNode node = GetNodeFromCollection(parentnode.Nodes, pathname);
						if (node == null) {
							TreeNode newnode = new ReflectionFolderNode(pathname, null, ReflectionNodeType.Namespace, 3, 3);
							newnode.Nodes.Clear();
							parentnode.Nodes.Add(newnode);
							path = newnode;
						} else
							path = node;
						nodename = typename.Substring(lastindex + 1);
					}
					path.Nodes.Add(new ReflectionTypeNode(nodename, type));
				}
			}
		}
		
		void PopulateAssembly(Assembly assembly, TreeNode parentnode)
		{
			parentnode.Nodes.Clear();
			
			TreeNode node = new ReflectionFolderNode(Path.GetFileName(assembly.CodeBase), assembly, ReflectionNodeType.Library, 2, 2);
			parentnode.Nodes.Add(node);
			
			ReflectionFolderNode resourcefolder = new ReflectionFolderNode("Resources", assembly, ReflectionNodeType.Folder, 6, 7);
			string[] resources = assembly.GetManifestResourceNames();
			foreach (string resource in resources) {
				resourcefolder.Nodes.Add(new ReflectionNode(resource, null, ReflectionNodeType.Resource));
			}
			parentnode.Nodes.Add(resourcefolder);
			
			ReflectionFolderNode referencefolder = new ReflectionFolderNode("References", assembly, ReflectionNodeType.Folder, 9, 10);
			AssemblyName[] references = assembly.GetReferencedAssemblies();
			foreach (AssemblyName name in references) {
				referencefolder.Nodes.Add(new ReflectionNode(name.Name, name, ReflectionNodeType.Reference));
			}
			parentnode.Nodes.Add(referencefolder);
			
			ReflectionFolderNode modulefolder = new ReflectionFolderNode("Modules", assembly, ReflectionNodeType.Folder, 9, 10);
			parentnode.Nodes.Add(modulefolder);
			Module[] modules = assembly.GetModules(true);
			foreach(Module module in modules) {
				modulefolder.Nodes.Add(new ReflectionNode(module.Name, null, ReflectionNodeType.Module));
			}
		}
		
		public virtual void OnExpand()
		{
		}
		public virtual void OnCollapse()
		{
		}
	}
}
