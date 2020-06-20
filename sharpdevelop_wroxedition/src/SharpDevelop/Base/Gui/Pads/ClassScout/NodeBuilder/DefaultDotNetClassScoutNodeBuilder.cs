// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Collections.Specialized;

using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Internal.Project;
using SharpDevelop.Internal.Parser;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class DefaultDotNetClassScoutNodeBuilder : IClassScoutNodeBuilder
	{
		int imageIndexOffset;
		IAmbience languageConversion;
		
		public DefaultDotNetClassScoutNodeBuilder()
		{
		}
		
		public bool CanBuildClassTree(IProject project)
		{
			return true;
		}
		
		void GetCurrentAmbience()
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));			
			AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
			
			languageConversion = ambienceService.CurrentAmbience;
			languageConversion.ConversionFlags = ConversionFlags.None;		
		}
		
		public TreeNode BuildClassTreeNode(IProject p, int imageIndexOffset)
		{
			GetCurrentAmbience();
			this.imageIndexOffset = imageIndexOffset;
			TreeNode prjNode = new AbstractClassScoutNode(p.Name);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			prjNode.SelectedImageIndex = prjNode.ImageIndex = imageIndexOffset + fileUtilityService.GetImageIndexForProjectType(p.ProjectType);
			
 			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.BuildAction == BuildAction.Compile) {
					int i = 0;
					IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
					if (parserService.GetParser(finfo.Name) == null) {
						continue;
					}
					while (parserService.GetParseInformation(finfo.Name) == null) {
						Thread.Sleep(100);	
						if (i++ > 5) {
							break;
						}
					}
					if (parserService.GetParseInformation(finfo.Name) == null) {
						continue;
					}
					IParseInformation parseInformation = parserService.GetParseInformation(finfo.Name);
					if (parseInformation != null) {
						ICompilationUnit unit = parseInformation.BestCompilationUnit as ICompilationUnit;
						if (unit != null) {
							foreach (IClass c in unit.Classes) {
								TreeNode node = GetPath(c.Namespace, prjNode.Nodes, true);
								if (node == null) {
									node = prjNode;
								}
								Insert(node.Nodes, finfo.Name, c);
							}
						}
					}
				}
			}
			return prjNode;
		}
		
		TreeNode GetNodeFromCollectionTreeByName(TreeNodeCollection collection, string name)
		{
			foreach (TreeNode node in collection) {
				if (node.Text == name) {
					return node;
				}
			}
			return null;
		}
		
		public TreeNode GetPath(string directory, TreeNodeCollection root, bool create)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
	
			string[] treepath   = directory.Split(new char[] { '.' });
			TreeNodeCollection curcollection = root;
			TreeNode           curnode       = null;
			
			foreach (string path in treepath) {
				
				if (path.Length == 0 || path[0] == '.') {
					continue;
				}
				
				TreeNode node = GetNodeFromCollectionTreeByName(curcollection, path);
				if (node == null) {
					if (create) {
						TreeNode newnode = new AbstractClassScoutNode(path);
						newnode.ImageIndex = newnode.SelectedImageIndex = classBrowserIconService.NamespaceIndex;
						
						curcollection.Add(newnode);
						curnode = newnode;
						curcollection = curnode.Nodes;
						continue;
					} else {
						return null;
					}
				}
				curnode = node;
				curcollection = curnode.Nodes;
			}
			return curnode;
		}
		
		void Insert(TreeNodeCollection nodes, string filename, IClass c)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			AbstractClassScoutNode cNode = new AbstractClassScoutNode(c.Name);
			cNode.SelectedImageIndex = cNode.ImageIndex = classBrowserIconService.GetIcon(c);
			cNode.ContextmenuAddinTreePath = "/SharpDevelop/Views/ClassScout/ContextMenu/ClassNode";
			
			cNode.Tag = new ClassScoutTag(c.Region.BeginLine, filename);
			
			nodes.Add(cNode);
			// don't insert delegate 'members'
			if (c.ClassType == ClassType.Delegate) {
				return;
			}
			
			TreeNode cbNode;
			
			foreach (IClass innerClass in c.InnerClasses) {
				if (innerClass.ClassType == ClassType.Delegate) {
					cbNode = new AbstractClassScoutNode(languageConversion.Convert(innerClass));
					cbNode.Tag = new ClassScoutTag(innerClass.Region.BeginLine, filename);
					cbNode.SelectedImageIndex = cbNode.ImageIndex = classBrowserIconService.GetIcon(innerClass);
					cNode.Nodes.Add(cbNode);
				} else {
					Insert(cNode.Nodes, filename, innerClass);
				}
			}
			
			foreach (IMethod method in c.Methods) {
				cbNode = new AbstractClassScoutNode(languageConversion.Convert(method));
				cbNode.Tag = new ClassScoutTag(method.Region.BeginLine, filename);
				cbNode.SelectedImageIndex = cbNode.ImageIndex = classBrowserIconService.GetIcon(method);
				cNode.Nodes.Add(cbNode);
			}
			
			foreach (IProperty property in c.Properties) {
				cbNode = new AbstractClassScoutNode(languageConversion.Convert(property));
				cbNode.Tag = new ClassScoutTag(property.Region.BeginLine, filename);
				cbNode.SelectedImageIndex = cbNode.ImageIndex = classBrowserIconService.GetIcon(property);
				cNode.Nodes.Add(cbNode);
			}
			
			foreach (IField field in c.Fields) {
				cbNode = new AbstractClassScoutNode(languageConversion.Convert(field));
				cbNode.Tag = new ClassScoutTag(field.Region.BeginLine, filename);
				
				cbNode.SelectedImageIndex = cbNode.ImageIndex = classBrowserIconService.GetIcon(field);
				cNode.Nodes.Add(cbNode);
			}
			
			foreach (IEvent e in c.Events) {
				cbNode = new AbstractClassScoutNode(languageConversion.Convert(e));
				cbNode.Tag = new ClassScoutTag(e.Region.BeginLine, filename);
				cbNode.SelectedImageIndex = cbNode.ImageIndex = classBrowserIconService.GetIcon(e);
				cNode.Nodes.Add(cbNode);
			}
		}
		
		public void UpdateClassTree(TreeNode projectNode, ParseInformationEventArgs e)
		{
//			TreeNode newNode = BuildClassTreeNode((IProject)projectNode.Tag, imageIndexOffset);
//			projectNode.Nodes.Clear();
//			foreach (TreeNode node in newNode.Nodes) {
//				projectNode.Nodes.Add(node);
//			}
		}
	}
}
