// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public class Combine
	{
		string name        = null;
		string description = null;
		
		/// <summary>
		/// name of the project to startup in singlestartup mode.
		/// </summary>
		string startProject  = null;
		bool   singleStartup = true;
		
		ArrayList entries       = new ArrayList();
		
		CombineConfiguration activeConfiguration;
		
		Hashtable configurations            = new Hashtable();
		ArrayList combineExecuteDefinitions = new ArrayList();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public CombineConfiguration ActiveConfiguration {
			get {
				return activeConfiguration;
			}
			set {
				activeConfiguration = value;
			}
		}
		
		public Hashtable Configurations {
			get {
				return configurations;
			}
		}
		
		public ArrayList CombineExecuteDefinitions {
			get {
				return combineExecuteDefinitions;
			}
		}
		
		public string SingleStartProjectName {
			get {
				return startProject;
			}
			set {
				startProject = value;
				OnStartupPropertyChanged(null);
			}
		}
		
		public bool SingleStartupProject {
			get {
				return singleStartup;
			}
			set {
				singleStartup = value;
				OnStartupPropertyChanged(null);
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
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public ArrayList Entries {
			get {
				return entries;
			}
		}
		
		public bool NeedsBuilding {
			get {
				ArrayList projects = new ArrayList();
				GetAllProjects(projects, this);
				foreach (ProjectCombineEntry projectEntry in projects) {
					if (projectEntry.IsDirty) {
						return true;
					}
				}
				return false;
			}
		}
		
		public Combine()
		{
		}
		
		public Combine(string filename)
		{
			LoadCombine(filename);
		}
		
		public IProject LoadProject(string filename)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			ILanguageBinding binding = languageBindingService.GetBindingPerProjectFile(filename);
			if (binding == null) {
				MessageBox.Show(stringParserService.Parse(resourceService.GetString("Internal.Project.Combine.CantFindCodonError"), new string[,] {{"FILENAME", filename}}),
				                resourceService.GetString("Global.ErrorText"),
				                MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
			
			IProject project = binding.CreateProject(null);
			project.LoadProject(filename);
			return project;
		}
		
		public void LoadCombine(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			string path = Path.GetDirectoryName(filename);
			
			
			XmlElement root = doc.DocumentElement;
			
			name          = root.Attributes["name"].InnerText;
			description   = root.Attributes["description"].InnerText;
			
			startProject   = root["StartMode"].Attributes["startupentry"].InnerText;
			singleStartup  = Boolean.Parse(root["StartMode"].Attributes["single"].InnerText);
			
			XmlNodeList nodes = root["Entries"].ChildNodes;
			entries.Clear();
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (XmlElement el in nodes) {
				string abs_path = fileUtilityService.RelativeToAbsolutePath(path, el.Attributes["filename"].InnerText);
				AddEntry(abs_path);
			}
			
			nodes = root["StartMode"].ChildNodes;
			combineExecuteDefinitions.Clear();
			foreach (XmlElement el in nodes)  {
				if (el.Name == "Execute") {
					CombineExecuteDefinition ced = new CombineExecuteDefinition();
					ced.Entry = GetEntry(el.Attributes["entry"].InnerText);
					ced.Type = (EntryExecuteType)Enum.Parse(typeof(EntryExecuteType), el.Attributes["type"].InnerText);
					combineExecuteDefinitions.Add(ced);
				}
			}
			
			nodes = root["Configurations"].ChildNodes;
			configurations.Clear();
			foreach (XmlElement el in nodes) {
				CombineConfiguration cconf = new CombineConfiguration(el, this);
				configurations[cconf.Name] = cconf;
				
				// set the active configuration, either to the first (if the active attribute is not set)
				// or to the active configuration specified by the active attribute.
				if ((doc.DocumentElement["Configurations"].Attributes["active"] == null) || cconf.Name == doc.DocumentElement["Configurations"].Attributes["active"].InnerText) { // ok, I know that && has a higher priority than ||, but many programmers think that a bracket is easier to read ... one thing I don't find easy to read are long lines :)
					ActiveConfiguration = cconf;
				}
			}
		}
		
		public void SaveCombine(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<Combine fileversion=\"1.0\"/>");
			
			XmlAttribute combineNameAttribute = doc.CreateAttribute("name");
			combineNameAttribute.InnerText = name;
			doc.DocumentElement.Attributes.Append(combineNameAttribute);
			
			XmlAttribute combineDescriptionAttribute = doc.CreateAttribute("description");
			combineDescriptionAttribute.InnerText = description;
			doc.DocumentElement.Attributes.Append(combineDescriptionAttribute);
			
			string path = Path.GetDirectoryName(filename);
			
			XmlElement startupnode  = doc.CreateElement("StartMode");
			
			XmlAttribute single = doc.CreateAttribute("startupentry");
			single.InnerText  = startProject;
			startupnode.Attributes.Append(single);
			
			XmlAttribute activeconf = doc.CreateAttribute("single");
			activeconf.InnerText = singleStartup.ToString();
			startupnode.Attributes.Append(activeconf);
			
			foreach (CombineExecuteDefinition ced in combineExecuteDefinitions) {
				XmlElement el = doc.CreateElement("Execute");
				
				XmlAttribute a1 = doc.CreateAttribute("entry");
				CombineEntry centry = ced.Entry;
				if (centry == null || centry.Entry == null) {
					continue;
				}
				if (centry.Entry is IProject) {
					a1.InnerText  = ((IProject)centry.Entry).Name;
				} else {
					a1.InnerText  = ((Combine)centry.Entry).Name;
				}
				el.Attributes.Append(a1);
				
				XmlAttribute a2 = doc.CreateAttribute("type");
				a2.InnerText  = ced.Type.ToString();
				el.Attributes.Append(a2);
				
				startupnode.AppendChild(el);
			}
			
			doc.DocumentElement.AppendChild(startupnode);
						
			XmlElement projectsnode = doc.CreateElement("Entries");
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (CombineEntry entry in entries) {
				XmlElement el = doc.CreateElement("Entry");
	
				XmlAttribute entrynameattr = doc.CreateAttribute("filename");
				entrynameattr.InnerText = fileUtilityService.AbsoluteToRelativePath(path, entry.Filename);
				el.Attributes.Append(entrynameattr);
				
				projectsnode.AppendChild(el);
			}
			doc.DocumentElement.AppendChild(projectsnode);
			
			XmlElement confnode = doc.CreateElement("Configurations");
			
			if (ActiveConfiguration != null) {
				XmlAttribute activeconfattr = doc.CreateAttribute("active");
				activeconfattr.InnerText = ActiveConfiguration.Name;
				confnode.Attributes.Append(activeconfattr);
			}
			foreach (DictionaryEntry dentry in configurations) {
				confnode.AppendChild(((CombineConfiguration)dentry.Value).ToXmlElement(doc));
			}
			doc.DocumentElement.AppendChild(confnode);
			
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save),
			                                filename,
			                                resourceService.GetString("Internal.Project.Combine.CantSaveCombineErrorText"),
			                                FileErrorPolicy.ProvideAlternative);
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		}
		
		public void SaveCombineAs()
		{
			SaveFileDialog fdiag = new SaveFileDialog();
			fdiag.OverwritePrompt = true;
			fdiag.AddExtension    = true;
			
			// TODO : make filefilter method
			fdiag.Filter = "Combines|*.cmbx|All files|*";
			
			if (fdiag.ShowDialog() == DialogResult.OK) {
				string filename = fdiag.FileName;
				SaveCombine(filename);
				MessageBox.Show(filename, resourceService.GetString("Internal.Project.Combine.CombineSavedMessage"), MessageBoxButtons.OK);
			}
		}

		public object AddEntry(string filename)
		{
			if (Path.GetExtension(filename).ToUpper() == ".PRJX") {
				IProject project = LoadProject(filename);
				ProjectCombineEntry newEntry = new ProjectCombineEntry(project, filename);
				entries.Add(newEntry);
				combineExecuteDefinitions.Add(new CombineExecuteDefinition(newEntry, EntryExecuteType.None));
				if (startProject == null)
					startProject = project.Name;
				
				if (configurations.Count == 0) {
					foreach (IConfiguration pconf in project.Configurations) {
						CombineConfiguration cconf = new CombineConfiguration(pconf.Name, this);
						configurations[pconf.Name] = cconf;
						if (ActiveConfiguration == null)
							ActiveConfiguration = cconf;
					}
				}
				
				foreach (DictionaryEntry entry in configurations) {
					CombineConfiguration conf = (CombineConfiguration)entry.Value;
					conf.AddEntry(project);
				}				
				return project;
			} else {
				Combine combine = new Combine(filename);
				CombineCombineEntry newEntry = new CombineCombineEntry(combine, filename);
				entries.Add(newEntry);
				combineExecuteDefinitions.Add(new CombineExecuteDefinition(newEntry, EntryExecuteType.None));
				if (startProject == null)
					startProject = combine.Name;
				
				if (configurations.Count == 0) {
					foreach (DictionaryEntry dentry in combine.Configurations) {
						CombineConfiguration cconf = ((CombineConfiguration)dentry.Value);
						configurations[cconf.Name] = new CombineConfiguration(cconf.Name, this);
						if (ActiveConfiguration == null)
							ActiveConfiguration = cconf;
					}
				}
				
				foreach (DictionaryEntry entry in configurations) {
					CombineConfiguration conf = (CombineConfiguration)entry.Value;
					conf.AddEntry(combine);
				}
				return combine;
			}
		}
		
		public void SaveAllProjects()
		{
			foreach (CombineEntry entry in entries) {
				entry.Save();
			}
		}
		
		public int GetEntryNumber(string name)
		{
			for (int i = 0; i < entries.Count; ++i) {
				if (((CombineEntry)entries[i]).Name == name) {
					return i;		
				}
			}
			return -1;
		}
		
		public CombineEntry GetEntry(string name)
		{
			for (int i = 0; i < entries.Count; ++i) {
				if (((CombineEntry)entries[i]).Name == name) {
					return (CombineEntry)entries[i];
				}
			}
			return null;
		}
		
		void StartProject(int  nr) 
		{
			CombineEntry entry = (CombineEntry)entries[nr];
			entry.Execute();
		}
		
		void StartProject(string name) 
		{
			int entrynum = GetEntryNumber(name);
			if (entrynum == -1) {
				throw new NoStartupCombineDefinedException();
			}
			StartProject(entrynum);
		}

		public void Execute()
		{
			if (singleStartup) {
				StartProject(startProject);
			} else {
				foreach (CombineExecuteDefinition ced in combineExecuteDefinitions) {
					if (ced.Type == EntryExecuteType.Execute) {
						StartProject(Entries.IndexOf(ced.Entry));
					}
				}
			}
		}
		
		/// <remarks>
		/// Returns an ArrayList containing all ProjectEntries in this combine and 
		/// undercombines
		/// </remarks>
		public static ArrayList GetAllProjects(Combine combine)
		{
			ArrayList list = new ArrayList();
			GetAllProjects(list, combine);
			return list;
		}
		
		static void GetAllProjects(ArrayList list, Combine combine)
		{
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is ProjectCombineEntry) {
					list.Add((ProjectCombineEntry)entry);
				} else {
					GetAllProjects(list, ((CombineCombineEntry)entry).Combine);
				}
			}
		}
		
		public ProjectCombineEntry GetProjectEntryContaining(string fileName) 
		{
			ArrayList projects = new ArrayList();
			GetAllProjects(projects, this);
			foreach (ProjectCombineEntry projectEntry in projects) {
				if (projectEntry.Project.IsFileInProject(fileName)) {
					return projectEntry;
				}
			}
			return null;
		}
		
		ArrayList TopologicalSort(ArrayList allProjects)
		{
			ArrayList sortedEntries = new ArrayList(allProjects.Count);
			bool[]    inserted      = new bool[allProjects.Count];
			bool[]    triedToInsert = new bool[allProjects.Count];
			for (int i = 0; i < allProjects.Count; ++i) {
				inserted[i] = triedToInsert[i] = false;
			}
			for (int i = 0; i < allProjects.Count; ++i) {
				if (!inserted[i]) {
					Insert(i, allProjects, sortedEntries, inserted, triedToInsert);
				}
			}
			return sortedEntries;
		}
		
		void Insert(int index, ArrayList allProjects, ArrayList sortedEntries, bool[] inserted, bool[] triedToInsert)
		{
			if (triedToInsert[index]) {
				throw new CyclicBuildOrderException();
			}
			triedToInsert[index] = true;
			foreach (ProjectReference reference in ((ProjectCombineEntry)allProjects[index]).Project.ProjectReferences) {
				if (reference.ReferenceType == ReferenceType.Project) {
					int j = 0;
					for (; j < allProjects.Count; ++j) {
						if (reference.Reference == ((ProjectCombineEntry)allProjects[j]).Name) {
							if (!inserted[j]) {
								Insert(j, allProjects, sortedEntries, inserted, triedToInsert);
							}
							break;
						}
					}
				}
			}
			sortedEntries.Add(allProjects[index]);
			inserted[index] = true;
		}
		
		public void Build(bool doBuildAll)
		{
			ArrayList allProjects = GetAllProjects(this);
			try {
				allProjects = TopologicalSort(allProjects);
			} catch (CyclicBuildOrderException) {
				MessageBox.Show("Cyclic dependencies can not be build with this version.\nBut we are working on it.", "Cyclic dependencies error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			foreach (ProjectCombineEntry entry in allProjects) {
				entry.Build(doBuildAll);
				if (taskService.SomethingWentWrong) {
					break;
				}
			}
		}
		
		void OnStartupPropertyChanged(EventArgs e)
		{
			if (StartupPropertyChanged != null) {
				StartupPropertyChanged(this, e);
			}
		}
			
		
		public event EventHandler StartupPropertyChanged;
	}
}
