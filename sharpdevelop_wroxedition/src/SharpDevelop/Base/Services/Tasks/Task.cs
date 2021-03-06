// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;


namespace ICSharpCode.SharpDevelop.Services
{
	public enum TaskType {
		Error,
		Warning,
		Comment,
		SearchResult
	}
	
	public class Task 
	{
		string   description;
		string   fileName;
		TaskType type;
		IProject project;
		int      line;
		int      column;
		

		public override string ToString()
		{
			return String.Format("[Task:File={0}, Line={1}, Column={2}, Type={3}, Description={4}",
			                     fileName,
			                     line,
			                     column,
			                     type,
			                     description);
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public int Line {
			get {
				return line;
			}
		}
		
		public int Column {
			get {
				return column;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public TaskType TaskType {
			get {
				return type;
			}
		}
		
		public Task(string fileName, string description, int column, int line)
		{
			type = TaskType.SearchResult;
			this.fileName    = fileName;
			this.description = description.Trim();
			this.column      = column;
			this.line        = line;
		}
		
		public Task(IProject project, CompilerError error)
		{
			this.project = project;
			type        = error.IsWarning ? TaskType.Warning : TaskType.Error;
			column      = error.Column - 1;
			line        = error.Line - 1;
			description = error.ErrorText + "(" + error.ErrorNumber + ")";
			fileName    = error.FileName;
		}
		
		public void JumpToPosition()
		{
			if (fileName != null && fileName.Length > 0) {
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile(fileName);
				IViewContent content = fileService.GetOpenFile(fileName).ViewContent;
				if (content is IPositionable) {
					((IPositionable)content).JumpTo(new Point(Math.Max(0, column), Math.Max(0, line)));
					// Begin Svante Lidman (by mike : May be workaround for .NET bug ... ?)
					content.Control.Focus();
					// End Svante Lidman
				}
			}
			
//			CompilerResultListItem li = (CompilerResultListItem)OpenTaskView.FocusedItem;
//			
//			string filename   = li.FileName;
//			
//			if (filename == null || filename.Equals(""))
//				return;
//			
//			if (File.Exists(filename)) {
//				string directory  = Path.GetDirectoryName(filename);
//				if (directory[directory.Length - 1] != Path.DirectorySeparatorChar) {
//					directory += Path.DirectorySeparatorChar;
//				}
//				
//				ContentWindow window = OpenWindow(filename);
//			}
		}
	}
}
