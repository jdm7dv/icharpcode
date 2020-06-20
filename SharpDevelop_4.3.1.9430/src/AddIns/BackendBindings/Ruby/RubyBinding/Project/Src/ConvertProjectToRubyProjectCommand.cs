﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Converts a C# or VB.NET project to Ruby.
	/// </summary>
	public class ConvertProjectToRubyProjectCommand : LanguageConverter
	{
		public override string TargetLanguageName {
			get { return RubyProjectBinding.LanguageName; }
		}
		
		/// <summary>
		/// Creates an RubyProject.
		/// </summary>
		protected override IProject CreateProject(string targetProjectDirectory, IProject sourceProject)
		{
			RubyProject targetProject = (RubyProject)base.CreateProject(targetProjectDirectory, sourceProject);
			return targetProject;
		}
		
		/// <summary>
		/// Converts C# and VB.NET files to Ruby and saves the files.
		/// </summary>
		protected override void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			ParseInformation parseInfo = GetParseInfo(sourceItem.FileName);
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(sourceItem.Include, parseInfo);
			if (converter != null) {
				targetItem.Include = ChangeFileExtensionToRubyFileExtension(sourceItem.Include);

				string code = GetParseableFileContent(sourceItem.FileName);
				string rubyCode = converter.Convert(code);
			
				RubyProject rubyTargetProject = (RubyProject)targetItem.Project;
				if ((converter.EntryPointMethods.Count > 0) && !rubyTargetProject.HasMainFile) {
					rubyTargetProject.AddMainFile(targetItem.Include);					
					// Add code to call main method at the end of the file.
					rubyCode += "\r\n\r\n" + converter.GenerateMainMethodCall(converter.EntryPointMethods[0]);
				}
				SaveFile(targetItem.FileName, rubyCode, GetDefaultFileEncoding());
			} else {
				LanguageConverterConvertFile(sourceItem, targetItem);
			}
		}

		/// <summary>
		/// Adds the MainFile property since adding it in the CreateProject method would mean
		/// it gets removed via the base class CopyProperties method.
		/// </summary>
		protected override void CopyProperties(IProject sourceProject, IProject targetProject)
		{
			base.CopyProperties(sourceProject, targetProject);
		}
		
		/// <summary>
		/// Calls the LanguageConverter class method ConvertFile which copies the source file to the target
		/// file without any modifications.
		/// </summary>
		protected virtual void LanguageConverterConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			base.ConvertFile(sourceItem, targetItem);
		}
		
		/// <summary>
		/// Writes the specified file to disk.
		/// </summary>
		protected virtual void SaveFile(string fileName, string content, Encoding encoding)
		{
			File.WriteAllText(fileName, content, encoding);
		}
		
		/// <summary>
		/// Gets the content of the file from the parser service.
		/// </summary>
		protected virtual string GetParseableFileContent(string fileName)
		{
			return ParserService.GetParseableFileContent(fileName).Text;
		}
		
		/// <summary>
		/// Gets the project content for the specified project.
		/// </summary>
		protected virtual IProjectContent GetProjectContent(IProject project)
		{
			return ParserService.GetProjectContent(project);
		}
		
		protected virtual ParseInformation GetParseInfo(string fileName)
		{
			return ParserService.GetParseInformation(fileName);
		}
		
		protected virtual Encoding GetDefaultFileEncoding()
		{
			return FileService.DefaultFileEncoding.GetEncoding();
		}
		
		/// <summary>
		/// Changes the extension to ".rb"
		/// </summary>
		static string ChangeFileExtensionToRubyFileExtension(string fileName)
		{
			return Path.ChangeExtension(fileName, ".rb");	
		}
	}
}
