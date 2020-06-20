// project created on 15.09.2002 at 18:30
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.StyleguideChecker 
{
	
	public class StyleGuideChecker : AbstractMenuCommand
	{
		TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				
		Regex[] formattingErrors = new Regex[] {
	// OK:
			new Regex(@"\A\t*[ ]"),
			new Regex(@"\Anamespace.*{"),
			
	// FAULTY:		
	//		new Regex(@"if\s*\(.*\)\s+[^{]"),
	//		new Regex(@"return\s+\(.*\)\s*;"),
	//		new Regex(@"for\s*\(.*\)\s+[^{]"),
	//		new Regex(@"foreach\s*\(.*\)\s+[^{]"),
	//		new Regex(@"while\s*\(.*\)\s+[^{]"),
	//		new Regex(@"switch\s*\(.*\)\s+[^{]"),
	//		new Regex(@"do\s*\(.*\)\s+[^{]")
		};
	
		
		string[] formattingErrorMessages = new string[] {
	// OK:
			"spaces in indent",
			"namespace with { in the same line",
	// FAULTY:
	//		"if without { in the same line",
	//		"return with brackets used",
	//		"for without { in the same line",
	//		"foreach without { in the same line",
	//		"while without { in the same line",
	//		"switch without { in the same line",
	//		"do without { in the same line",
		};
		public class Error {
			
			public int   column;
			public string description;
			public string line;
			
			public Error(string description, string line, int column)
			{
				this.description = description;
				this.line = line;
				this.column = column;
			}
		}
		
		ArrayList GetFormattingFaults(string text)
		{
			ArrayList error = new ArrayList();
			for (int i = 0; i < formattingErrors.Length; ++i) {
				Match match = formattingErrors[i].Match(text);
				while (match.Success) {
					error.Add(new Error(formattingErrorMessages[i],
					                    match.ToString(),
					                    match.Index));
					match = match.NextMatch();
				}
			}		
			return error;
		}
		
		void Check(string file)
		{
			StreamReader sr = File.OpenText(file);
			int lineNr = 0;
			while (true) {
				string curLine = sr.ReadLine();
				++lineNr;
				if (curLine == null) {
					break;
				}
				ArrayList errors = GetFormattingFaults(curLine);
				foreach (Error error in errors) {
					taskService.Tasks.Add(new Task(file, 
					                               error.description + " : " + error.line, 
					                               error.column, 
					                               lineNr));
				}
			}
			sr.Close();
		}
		
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			if (projectService.CurrentOpenCombine != null) {
				ArrayList projects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				taskService.Tasks.Clear();
				
				foreach (ProjectCombineEntry entry in projects) {
					foreach (ProjectFile file in entry.Project.ProjectFiles) {
						if (file.Subtype == Subtype.Code) {
							if (File.Exists(file.Name) && file.Name.EndsWith(".cs")) {
								Check(file.Name);
							}
						}
					}
				}
				
				taskService.NotifyTaskChange();
			}
		}
	}
}
