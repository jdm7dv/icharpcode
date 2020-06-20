// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Resources;
using System.Reflection.Emit;

using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Properties;

namespace SharpDevelop.Gui.Edit.Reflection
{
	public class ReflectionTree : TreeView
	{
		ArrayList  assemblies = new ArrayList();

		public event EventHandler Changed;
		
		public ArrayList Assemblies {
			get {
				return assemblies;
			}
		}
		
		public PrintDocument PrintDocument {
			get {
				return null;
			}
		}
		
		public bool WriteProtected {
			get {
				return false;
			}
			set {
			}
		}
		
		public void LoadFile(string filename) 
		{
			try {
				AddAssembly(Assembly.LoadFrom(filename));
			} catch (Exception e) {
				MessageBox.Show("can't load " + filename+ " invalid format.\n(you can only view .NET assemblies)\n " + e.ToString());
			}
		}
		
		public void SaveFile(string filename) 
		{
		}
		
		public void AddAssembly(Assembly assembly)
		{
			assemblies.Add(assembly);
			TreeNode node = new ReflectionFolderNode(Path.GetFileNameWithoutExtension(assembly.CodeBase), assembly, ReflectionNodeType.Assembly, 0, 1);
			Nodes.Add(node);
			PopulateTreeView();
			Thread t = new Thread(new ThreadStart(PopulateTreeView));
			
			t.IsBackground = true;
			t.Start();
		}

		public void PopulateTreeView(ReflectionNode parentnode)
		{
			foreach (ReflectionNode node in parentnode.Nodes) {
				if (!node.Populated) {
					node.Populate();
				}
				PopulateTreeView(node);
			}
		}
		
		public void PopulateTreeView()
		{
			foreach (ReflectionNode node in Nodes) {
				if (!node.Populated)
					node.Populate();
				PopulateTreeView(node);
			}
		}
		
		internal static AmbienceReflectionDecorator languageConversion;
		
		public ReflectionTree()
		{
			if (Changed != null) {} // only to prevent these pesky compiler warning :) M.K.
			
			Dock = DockStyle.Fill;
			
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
			
			languageConversion = ambienceService.CurrentAmbience;
			languageConversion.ConversionFlags = languageConversion.ConversionFlags & ~(ConversionFlags.UseFullyQualifiedNames | ConversionFlags.ShowModifiers | ConversionFlags.ShowParameterNames);
			
			this.ImageList = classBrowserIconService.ImageList;
			
			LabelEdit     = false;
			HotTracking   = false;
			AllowDrop     = true;
			HideSelection = false;
		}

		public void GoToMember(MemberInfo member, Assembly MemberAssembly) 
		{
			foreach (ReflectionNode node in Nodes) {
				
				Assembly assembly = (Assembly)node.Attribute;
				string pathname = member.DeclaringType.Name;
				string paramtext = "";
				
				if (member is MethodInfo) {
					paramtext = languageConversion.Convert(member as MethodInfo);
				} else if (member is PropertyInfo) {
					paramtext = " : " + languageConversion.Convert(member as PropertyInfo);
				} else if (member is FieldInfo) {
					paramtext = " : " + languageConversion.Convert(member as FieldInfo);
				}
				
				if (assembly.FullName == MemberAssembly.FullName) {
					ReflectionNode curnode = (ReflectionNode)node.GetNodeFromCollection(node.Nodes, Path.GetFileName(assembly.CodeBase));
					if (member.DeclaringType.Namespace != null) {
						curnode = (ReflectionNode)curnode.GetNodeFromCollection(curnode.Nodes, member.DeclaringType.Namespace);
					}
					
					TreeNode path = curnode;
					TreeNode foundnode = node.GetNodeFromCollection(path.Nodes, pathname);
					
					ReflectionNode classnode = (ReflectionNode)foundnode;
					
					TreeNode membernode = classnode.GetNodeFromCollection(classnode.Nodes, member.Name + paramtext);
					membernode.EnsureVisible();
					SelectedNode = membernode;
				}
			}
		}
		
		public void GoToType(Type type)
		{
			foreach (ReflectionNode node in Nodes) {
				Assembly assembly = (Assembly)node.Attribute;
				if (type.Assembly.FullName == assembly.FullName) {
					
					ReflectionNode curnode = (ReflectionNode)node.GetNodeFromCollection(node.Nodes, Path.GetFileName(assembly.CodeBase));
					// what was that ? M.P.
					//curnode = (ReflectionNode)curnode.GetNodeFromCollection(curnode.Nodes, Path.GetFileName(assembly.CodeBase));
	
					string   typename  = type.ToString();
					int      lastindex = typename.LastIndexOf('.');
					TreeNode path      = curnode;
					string   nodename  = typename;
					if (lastindex != -1) {
						string pathname = typename.Substring(0, lastindex);
						TreeNode tnode = node.GetNodeFromCollection(node.Nodes, pathname);
						if (tnode == null) {
							return; // TODO : returns, if the tree isn't up to date.
						} else
							path = tnode;
						nodename = typename.Substring(lastindex + 1);
					}
					
					TreeNode foundnode = node.GetNodeFromCollection(path.Nodes, nodename);
					foundnode.EnsureVisible();
					SelectedNode = foundnode;
					return;
				}
			}
			AddAssembly(type.Assembly);
			GoToType(type);
		}
		
		protected override void OnDoubleClick(EventArgs e)
		{
			ReflectionNode rn = (ReflectionNode)SelectedNode;
			if (rn == null)
				return;
			switch (rn.Type) {
				
				case ReflectionNodeType.Link: // clicked on link, jump to link.
					if (rn.Attribute is Type) {
						GoToType((Type)rn.Attribute);
					}
					break;
				
				case ReflectionNodeType.Reference: // clicked on assembly reference, open assembly
					// check if the assembly is open
					AssemblyName name = (AssemblyName)rn.Attribute;
					foreach (ReflectionNode node in Nodes) {
						if (node.Type == ReflectionNodeType.Assembly) {
							if (name.FullName == ((Assembly)node.Attribute).FullName) // if yes, return
								return;
						}
					}
					AddAssembly(Assembly.Load(name));
					break;
			}
		}
		
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			base.OnBeforeCollapse(e);
			((ReflectionNode)e.Node).OnCollapse();
		}
		
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			base.OnBeforeExpand(e);
			
			// populate node
			ReflectionNode rn = (ReflectionNode)e.Node;
			if (!rn.Populated)
				rn.Populate();
			
			((ReflectionNode)e.Node).OnExpand();
		}
	}
}
