// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DefaultParserService : AbstractService, IParserService
	{
		Hashtable classes    = new Hashtable();
		Hashtable parsings   = new Hashtable();
		Hashtable namespaces = new Hashtable();

		/// <remarks>
		/// The keys are the assemblies loaded. This hash table ensures that no
		/// assembly is loaded twice. I know that strong naming might be better but
		/// the use case isn't there. No one references 2 differnt files if he references
		/// the same assembly.
		/// </remarks>
		Hashtable loadedAssemblies = new Hashtable();
		
		ClassProxyCollection classProxies = new ClassProxyCollection();
		
		IParser[] parser;
		
		readonly static string[] assemblyList = {
			"Microsoft.VisualBasic",
			"Microsoft.JScript",
			"mscorlib",
			"System.Data",
			"System.Design",
			"System.DirectoryServices",
			"System.Drawing.Design",
			"System.Drawing",
			"System.EnterpriseServices",
			"System.Management",
			"System.Messaging",
			"System.Runtime.Remoting",
			"System.Runtime.Serialization.Formatters.Soap",

			"System.Security",
			"System.ServiceProcess",
			"System.Web.Services",
			"System.Web",
			"System.Windows.Forms",
			"System",
			"System.XML"
		};
		
		/// <remarks>
		/// The initialize method writes the location of the code completion proxy
		/// file to this string.
		/// </remarks>
		string codeCompletionProxyFile;
		string codeCompletionMainFile;
		
		class ClasstableEntry 
		{
			IClass           myClass;
			ICompilationUnit myCompilationUnit;
			string           myFileName;
			
			public IClass Class {
				get {
					return myClass;
				}
			}
			
			public ICompilationUnit CompilationUnit {
				get {
					return myCompilationUnit;
				}
			}
			
			public string FileName {
				get {
					return myFileName;
				}
			}
			
			public ClasstableEntry(string fileName, ICompilationUnit compilationUnit, IClass c)
			{
				this.myCompilationUnit = compilationUnit;
				this.myFileName        = fileName;
				this.myClass           = c;
			}
		}
		
		void RunWizard()
		{
			IProperties customizer = new DefaultProperties();
			
			if (SharpDevelopMain.SplashScreen != null) {
				SharpDevelopMain.SplashScreen.Close();
			}
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
			customizer.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
			                       propertyService.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty));
			
			using (WizardDialog wizard = new WizardDialog("Initialize Code Completion Database", customizer, "/SharpDevelop/CompletionDatabaseWizard")) {
				wizard.ControlBox = false;
				wizard.ShowInTaskbar = true;
				if (wizard.ShowDialog() == DialogResult.OK) {
					propertyService.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
					                            customizer.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty));
				}
			}
		}
		
		public void GenerateCodeCompletionDatabaseFast(string createPath, IProgressMonitor progressMonitor)
		{
			SetCodeCompletionFileLocation(createPath);
			
			// write all classes and proxies to the disc
			BinaryWriter classWriter = new BinaryWriter(new BufferedStream(new FileStream(codeCompletionMainFile, FileMode.Create, FileAccess.Write, FileShare.None)));
			BinaryWriter proxyWriter = new BinaryWriter(new BufferedStream(new FileStream(codeCompletionProxyFile, FileMode.Create, FileAccess.Write, FileShare.None)));
			if (progressMonitor != null) {
				progressMonitor.BeginTask("generate code completion database", assemblyList.Length);
			}
		
			// convert all assemblies
			for (int i = 0; i < assemblyList.Length; ++i) {
				try {
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					string path = fileUtilityService.GetDirectoryNameWithSeparator(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
					
					AssemblyInformation frameworkAssemblyInformation = new AssemblyInformation();
					frameworkAssemblyInformation.LoadAssembly(String.Concat(path, assemblyList[i], ".dll"));
					
					// create all class proxies
					foreach (IClass newClass in frameworkAssemblyInformation.Classes) {
						ClassProxy newProxy = new ClassProxy(newClass);
						classProxies.Add(newProxy);
						AddClassToNamespaceList(newProxy);
						
						PersistantClass pc = new PersistantClass(classProxies, newClass);
						newProxy.Offset = (uint)classWriter.BaseStream.Position;
						newProxy.WriteTo(proxyWriter);
						pc.WriteTo(classWriter);
					}
					if (progressMonitor != null) {
						progressMonitor.Worked(i);
					}
				} catch (Exception e) {
//					Console.WriteLine(e.ToString());
				}
				System.GC.Collect();
			}

			classWriter.Close();
			proxyWriter.Close();
			if (progressMonitor != null) {
				progressMonitor.Done();
			}
		}
		
		public void GenerateEfficientCodeCompletionDatabase(string createPath, IProgressMonitor progressMonitor)
		{
			SetCodeCompletionFileLocation(createPath);
			
			AssemblyInformation frameworkAssemblyInformation = new AssemblyInformation();
			if (progressMonitor != null) {
				progressMonitor.BeginTask("generate code completion database", assemblyList.Length * 3);
			}
						
			// convert all assemblies
			for (int i = 0; i < assemblyList.Length; ++i) {
				try {
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					string path = fileUtilityService.GetDirectoryNameWithSeparator(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
					
					frameworkAssemblyInformation.LoadAssembly(String.Concat(path, assemblyList[i], ".dll"));
					if (progressMonitor != null) {
						progressMonitor.Worked(i);
					}
				} catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
				System.GC.Collect();
			}
			
			// create all class proxies
			for (int i = 0; i < frameworkAssemblyInformation.Classes.Count; ++i) {
				ClassProxy newProxy = new ClassProxy(frameworkAssemblyInformation.Classes[i]);
				classProxies.Add(newProxy);
				AddClassToNamespaceList(newProxy);
				
				if (progressMonitor != null) {
					progressMonitor.Worked(assemblyList.Length + (i * assemblyList.Length) / frameworkAssemblyInformation.Classes.Count);
				}
			}
			
			// write all classes and proxies to the disc
			BinaryWriter classWriter = new BinaryWriter(new BufferedStream(new FileStream(codeCompletionMainFile, FileMode.Create, FileAccess.Write, FileShare.None)));
			BinaryWriter proxyWriter = new BinaryWriter(new BufferedStream(new FileStream(codeCompletionProxyFile, FileMode.Create, FileAccess.Write, FileShare.None)));
			
			for (int i  = 0; i < frameworkAssemblyInformation.Classes.Count; ++i) {
				IClass newClass = frameworkAssemblyInformation.Classes[i];
				PersistantClass pc = new PersistantClass(classProxies, newClass);
				ClassProxy proxy = classProxies[i];
				proxy.Offset = (uint)classWriter.BaseStream.Position;
				proxy.WriteTo(proxyWriter);
				pc.WriteTo(classWriter);
				if (progressMonitor != null) {
					progressMonitor.Worked(2 * assemblyList.Length + (i * assemblyList.Length) / frameworkAssemblyInformation.Classes.Count);
				}
			}
			
			classWriter.Close();
			proxyWriter.Close();
			if (progressMonitor != null) {
				progressMonitor.Done();
			}
		}
		
		void SetCodeCompletionFileLocation(string path)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string codeCompletionTemp = fileUtilityService.GetDirectoryNameWithSeparator(path);
			
			codeCompletionProxyFile = codeCompletionTemp + "CodeCompletionProxyData.bin";
			codeCompletionMainFile  = codeCompletionTemp + "CodeCompletionMainData.bin";
		}
		
		void SetDefaultCompletionFileLocation()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			SetCodeCompletionFileLocation(propertyService.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty).ToString());
		}
		
		void LoadProxyDataFile()
		{
			if (!File.Exists(codeCompletionProxyFile)) {
				return;
			}
			BinaryReader reader = new BinaryReader(new BufferedStream(new FileStream(codeCompletionProxyFile, FileMode.Open, FileAccess.Read, FileShare.Read)));
			while (true) {
				try {
					ClassProxy newProxy = new ClassProxy(reader);
					classProxies.Add(newProxy);
					AddClassToNamespaceList(newProxy);
				} catch (Exception) {
					break;
				}
			}
			reader.Close();
		}
		
		public override void InitializeService()
		{
			parser = (IParser[])(AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Parser").BuildChildItems(this)).ToArray(typeof(IParser));
			
			SetDefaultCompletionFileLocation();
			
			BinaryFormatter formatter = new BinaryFormatter();
			
			if (!File.Exists(codeCompletionProxyFile)) {
				RunWizard();
				SetDefaultCompletionFileLocation();
			} 
			LoadProxyDataFile();
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.CombineOpened += new CombineEventHandler(OpenCombine);
		}
		
		public void AddReferenceToCompletionLookup(IProject project, ProjectReference reference)
		{
			if (reference.ReferenceType != ReferenceType.Project) {
				string fileName = reference.GetReferencedFileName(project);
				if (fileName == null || fileName.Length == 0 || fileName.IndexOf("System") > 0) {
					return;
				}
				// HACK : Don't load references for non C# projects
				if (project.ProjectType != "C#") {
					return;
				}
				if (File.Exists(fileName)) {
					LoadAssemblyParseInformations(fileName);
				}
			}
		}
		
		void LoadAssemblyParseInformations(string assemblyFileName)
		{
			if (loadedAssemblies[assemblyFileName] != null) {
				return;
			}
			loadedAssemblies[assemblyFileName] = true;
			
			AssemblyInformation assemblyInformation = new AssemblyInformation();
			assemblyInformation.LoadAssembly(assemblyFileName);
			foreach (IClass newClass in assemblyInformation.Classes) {
				AddClassToNamespaceList(newClass);
				classes[newClass.FullyQualifiedName] = new ClasstableEntry(null, null, newClass);
			}
		}
		
		public void OpenCombine(object sender, CombineEventArgs e)
		{
			ArrayList projects =  Combine.GetAllProjects(e.Combine);
			foreach (ProjectCombineEntry entry in projects) {
				foreach (ProjectReference r in entry.Project.ProjectReferences) {
					AddReferenceToCompletionLookup(entry.Project, r);
				}
			}
		}
		
		public void StartParserThread()
		{
			Thread t = new Thread(new ThreadStart(ParserUpdateThread));
			t.IsBackground  = true;	
			t.Priority  = ThreadPriority.Lowest;
			t.Start();			
		}
		
		void ParserUpdateThread()
		{
			while (true) {
//				Thread.Sleep(1000);
				Thread.Sleep(0);
				try {
					if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent != null) {
						IEditable editable = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent as IEditable;
						if (editable != null) {
							string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;
							if (!(fileName == null || fileName.Length == 0)) {
//								Thread.Sleep(300);
								Thread.Sleep(0);
								lock (parsings) {
									ParseFile(fileName, editable.TextContent);
								}
							}
						}
					}
				} catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
				Thread.Sleep(500);
			}
		}
		
		Hashtable AddClassToNamespaceList(IClass addClass)
		{
			string nSpace = addClass.Namespace;
			if (nSpace == null) {
				nSpace = String.Empty;
			}
			string[] path = nSpace.Split('.');
			Hashtable cur = namespaces;
			
			for (int i = 0; i < path.Length; ++i) {
				if (cur[path[i]] == null) {
					cur[path[i]] = new Hashtable();
				} else {
					if (!(cur[path[i]] is Hashtable)) {
						return null;
					}
				}
				cur = (Hashtable)cur[path[i]];
			}
			
			cur[addClass.Name] = new ClassProxy(addClass);
			
			return cur;
		}
		
		public ArrayList GetNamespaceContents(string subNameSpace)
		{
			ArrayList namespaceList = new ArrayList();
			if (subNameSpace == null) {
				return namespaceList;
			}
			string[] path = subNameSpace.Split('.');
			Hashtable cur = namespaces;
			
			for (int i = 0; i < path.Length; ++i) {
				if (!(cur[path[i]] is Hashtable)) {
					return namespaceList;
				}
				cur = (Hashtable)cur[path[i]];
			}
			
			foreach (DictionaryEntry entry in cur)  {
				if (entry.Value is Hashtable) {
					namespaceList.Add(entry.Key);
				} else {
					namespaceList.Add(entry.Value);
				}
			}
			
			return namespaceList;
		}
		
		public bool NamespaceExists(string name)
		{
			if (name == null) {
				return false;
			}
			string[] path = name.Split('.');
			Hashtable cur = namespaces;
			
			for (int i = 0; i < path.Length; ++i) {
				if (!(cur[path[i]] is Hashtable)) {
					return false;
				}
				cur = (Hashtable)cur[path[i]];
			}
			return true;
		}
		
		public string[]  GetNamespaceList(string subNameSpace)
		{
			Debug.Assert(subNameSpace != null);
			
			string[] path = subNameSpace.Split('.');
			Hashtable cur = namespaces;
			
			if (subNameSpace.Length > 0) {
				for (int i = 0; i < path.Length; ++i) {
					if (!(cur[path[i]] is Hashtable)) {
						return null;
					}
					cur = (Hashtable)cur[path[i]];
				}
			}
			
			ArrayList namespaceList = new ArrayList();
			foreach (DictionaryEntry entry in cur) {
				if (entry.Value is Hashtable && entry.Key.ToString().Length > 0) {
					namespaceList.Add(entry.Key);
				}
			}
			
			return (string[])namespaceList.ToArray(typeof(string));
		}
		
		public IParseInformation ParseFile(string fileName, string fileContent)
		{
			IParser parser = GetParser(fileName);
			
			if (parser == null) {
				return null;
			}
			
			parser.LexerTags = new string[] { "HACK", "TODO", "UNDONE", "FIXME" };
			
			ICompilationUnitBase parserOutput = parser.Parse(fileName, fileContent);
			
			ParseInformation parseInformation = parsings[fileName] as ParseInformation;
			
			if (parseInformation == null) {
				parseInformation = new ParseInformation();
			}
			
			if (parserOutput.ErrorsDuringCompile) {
				parseInformation.DirtyCompilationUnit = parserOutput;
			} else {
				parseInformation.ValidCompilationUnit = parserOutput;
				parseInformation.DirtyCompilationUnit = null;
			}
			
			parsings[fileName] = parseInformation;
			
			// TODO : move this into the compilation unit layer
			if (!parserOutput.ErrorsDuringCompile && parseInformation.BestCompilationUnit is ICompilationUnit) {
				ICompilationUnit cu = (ICompilationUnit)parseInformation.BestCompilationUnit;
				foreach (IClass c in cu.Classes) {
					AddClassToNamespaceList(c);
					classes[c.FullyQualifiedName] = new ClasstableEntry(fileName, cu, c);
				}
			}
			
			OnParseInformationChanged(new ParseInformationEventArgs(fileName, parseInformation));
			return parseInformation;
		}
		
		void RemoveClasses(ICompilationUnit cu)
		{
			if (cu != null) {
				foreach (IClass c in cu.Classes) {
					classes.Remove(c.FullyQualifiedName);
				}
			}
		}
		
		public IParseInformation ParseFile(string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser == null) {
				return null;
			}
			
			parser.LexerTags = new string[] { "HACK", "TODO", "UNDONE", "FIXME" };
			
			ICompilationUnitBase parserOutput = parser.Parse(fileName);
			ParseInformation parseInformation = parsings[fileName] as ParseInformation;
			
			if (parseInformation == null) {
				parseInformation = new ParseInformation();
			}
			
			if (parserOutput.ErrorsDuringCompile) {
				parseInformation.DirtyCompilationUnit = parserOutput;
			} else {
				parseInformation.ValidCompilationUnit = parserOutput;
				parseInformation.DirtyCompilationUnit = null;
			}
			
			parsings[fileName] = parseInformation;
			
			// TODO : move this into the compilation unit layer
			if (!parserOutput.ErrorsDuringCompile && parseInformation.BestCompilationUnit is ICompilationUnit) {
				ICompilationUnit cu = (ICompilationUnit)parseInformation.BestCompilationUnit;
				foreach (IClass c in cu.Classes) {
					AddClassToNamespaceList(c);
					classes[c.FullyQualifiedName] = new ClasstableEntry(fileName, cu, c);
				}
			}
			
			OnParseInformationChanged(new ParseInformationEventArgs(fileName, parseInformation));
			return parseInformation;
		}
		
		public IParseInformation GetParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return null;
			}
			object cu = parsings[fileName];
			if (cu == null) {
				return ParseFile(fileName);
			}			
			return (IParseInformation)cu;
		}
		
		public IParser GetParser(string fileName)
		{
			if (Path.GetExtension(fileName) == ".cs") {
				return parser[0];
			}
			return null;
		}
		
		public IClass GetClass(string typeName)
		{
			ClasstableEntry entry = classes[typeName] as ClasstableEntry;
			if (entry != null) {
				return entry.Class;
			}
			
			// try to load the class from our data file
			int idx = classProxies.IndexOf(typeName);
			if (idx > 0) {
				BinaryReader reader = new BinaryReader(new BufferedStream(new FileStream(codeCompletionMainFile, FileMode.Open, FileAccess.Read, FileShare.Read)));
				reader.BaseStream.Seek(classProxies[idx].Offset, SeekOrigin.Begin);
				IClass c = new PersistantClass(reader, classProxies);
				reader.Close();
				classes[typeName] = new ClasstableEntry(null, null, c);
				return c;
			}
			
			return null;
		}
		////////////////////////////////////
		
		public ResolveResult Resolve(string expression, int caretLineNumber, int caretColumn, string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.Resolve(this, expression, caretLineNumber, caretColumn, fileName);
			}			
			return null;
		}
		
		protected void OnParseInformationChanged(ParseInformationEventArgs e) 
		{
			if (ParseInformationChanged != null) {
				ParseInformationChanged(this, e);
			}
		}
		
		public event ParseInformationEventHandler ParseInformationChanged;
	}
}
