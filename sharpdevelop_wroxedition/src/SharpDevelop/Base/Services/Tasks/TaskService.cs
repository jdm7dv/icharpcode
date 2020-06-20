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

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Services
{
	public class TaskService : AbstractService
	{
		ArrayList tasks  = new ArrayList();
		string    compilerOutput = "";
		
		public ArrayList Tasks {
			get {
				return tasks;
			}
		}
		
		int warnings = 0;
		int errors   = 0;
		int comments = 0;
		
		public int Warnings {
			get {
				return warnings;
			}
		}
		
		public int Errors {
			get {
				return errors;
			}
		}
		
		public int Comments {
			get {
				return comments;
			}
		}
		
		public bool SomethingWentWrong {
			get {
				return errors + warnings > 0;
			}
		}
		
		public string CompilerOutput {
			get {
				return compilerOutput;
			}
			set {
				compilerOutput = value;
				OnCompilerOutputChanged(null);
			}
		}
		
		protected virtual void OnCompilerOutputChanged(EventArgs e)
		{
			if (CompilerOutputChanged != null) {
				CompilerOutputChanged(this, e);
			}
		}
		
		protected virtual void OnTasksChanged(EventArgs e)
		{
			if (TasksChanged != null) {
				TasksChanged(this, e);
			}
		}
		
		public void NotifyTaskChange()
		{
			warnings = errors = comments = 0;
			foreach (Task task in tasks) {
				switch (task.TaskType) {
					case TaskType.Warning:
						++warnings;
						break;
					case TaskType.Error:
						++errors;
						break;
					default:
						++comments;
						break;
				}
			}
			OnTasksChanged(null);
		}

		public event EventHandler TasksChanged;
		public event EventHandler CompilerOutputChanged;
	}

}
