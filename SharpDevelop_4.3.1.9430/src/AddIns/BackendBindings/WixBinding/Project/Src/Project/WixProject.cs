﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public enum WixOutputType {
		[Description("${res:ICSharpCode.WixBinding.ProjectOptions.OutputType.Installer} (.msi)")]
		Package,
		[Description("${res:ICSharpCode.WixBinding.ProjectOptions.OutputType.MergeModule} (.msm)")]
		Module,
		[Description("${res:ICSharpCode.WixBinding.ProjectOptions.OutputType.WixLibrary} (.wixlib)")]
		Library
	}
	
	public class WixProject : CompilableProject, IWixPropertyValueProvider
	{
		public const string DefaultTargetsFile = "$(WixTargetsPath)";
		public const string FileNameExtension = ".wixproj";
		
		delegate bool IsFileNameMatch(string fileName);
		
		public WixProject(ProjectLoadInformation info)
			: base(info)
		{
		}
		
		public WixProject(ProjectCreateInformation info)
			: base(info)
		{
			SetProperty("OutputType", "Package");
			AddWixTargetsPathProperties();
			AddImport(DefaultTargetsFile, null);
		}
		
		void AddWixTargetsPathProperties()
		{
			string condition = " '$(WixTargetsPath)' == '' AND '$(MSBuildExtensionsPath32)' != '' ";
			AddConditionalProperty("WixTargetsPath", @"$(MSBuildExtensionsPath32)\Microsoft\WiX\v3.x\Wix.targets", condition);
			
			AddGuardedProperty("WixTargetsPath", @"$(MSBuildExtensionsPath)\Microsoft\WiX\v3.x\Wix.targets");
		}
		
		public override string Language {
			get { return WixProjectBinding.LanguageName; }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return LanguageProperties.None; }
		}
		
		/// <summary>
		/// Returns the filename extension based on the project's output type.
		/// </summary>
		public static string GetInstallerExtension(string outputType)
		{
			outputType = outputType.ToLowerInvariant();
			switch (outputType) {
				case "package":
					return ".msi";
				case "module":
					return ".msm";
				case "library":
					return ".wixlib";
				default:
					return ".msi";
			}
		}
		
		/// <summary>
		/// Gets the full path to the installer file that will be generated by
		/// the Wix compiler and linker.
		/// </summary>
		public string GetInstallerFullPath()
		{
			string outputPath = GetEvaluatedPropertyOrEmptyString("OutputPath");
			string outputType = GetEvaluatedPropertyOrEmptyString("OutputType");
			string outputName = GetEvaluatedPropertyOrEmptyString("OutputName");
			string fileName = String.Concat(outputName, GetInstallerExtension(outputType));
			return Path.Combine(Directory, outputPath, fileName);
		}
		
		string GetEvaluatedPropertyOrEmptyString(string propertyName)
		{
			return GetEvaluatedProperty(propertyName) ?? String.Empty;
		}
		
		public override string OutputAssemblyFullPath {
			get { return GetInstallerFullPath(); }
		}
		
		/// <summary>
		/// Adds a set of Wix libraries (.wixlib) to the project.
		/// </summary>
		public void AddWixLibraries(string[] fileNames)
		{
			foreach (string fileName in fileNames) {
				AddWixLibrary(fileName);
			}
		}
		
		public void AddWixLibrary(string fileName)
		{
			WixLibraryProjectItem projectItem = new WixLibraryProjectItem(this);
			projectItem.FileName = fileName;
			ProjectService.AddProjectItem(this, projectItem);
		}
		
		public void AddWixExtensions(string[] fileNames)
		{
			foreach (string fileName in fileNames) {
				AddWixExtension(fileName);
			}
		}
		
		public void AddWixExtension(string fileName)
		{
			WixExtensionProjectItem projectItem = new WixExtensionProjectItem(this);
			projectItem.FileName = fileName;
			ProjectService.AddProjectItem(this, projectItem);
		}
		
		/// <summary>
		/// Returns the file project items that are Wix documents based on
		/// their filename.
		/// </summary>
		public ReadOnlyCollection<FileProjectItem> WixFiles {
			get { return GetMatchingFiles(WixFileName.IsWixFileName); }
		}
		
		/// <summary>
		/// Returns the file project items that are Wix source files (.wxs).
		/// </summary>
		public ReadOnlyCollection<FileProjectItem> WixSourceFiles {
			get { return GetMatchingFiles(WixFileName.IsWixSourceFileName); }
		}
		
		/// <summary>
		/// Returns the compiler extension project items.
		/// </summary>
		public ReadOnlyCollection<WixExtensionProjectItem> WixExtensions {
			get { return GetExtensions(); }
		}

		/// <summary>
		/// Gets a preprocessor variable value with the given name.
		/// </summary>
		/// <remarks>
		/// TODO: This can be configuration specific.
		/// </remarks>
		public string GetPreprocessorVariableValue(string name)
		{
			string constants = GetEvaluatedProperty("DefineConstants");
			NameValuePairCollection nameValuePairs = new NameValuePairCollection(constants);
			return WixPropertyParser.Parse(nameValuePairs.GetValue(name), this);
		}
		
		/// <summary>
		/// Gets the MSBuild Property value for the given name.
		/// </summary>
		string IWixPropertyValueProvider.GetValue(string name)
		{
			string propertyValue;
			if (MSBuildEngine.MSBuildProperties.TryGetValue(name, out propertyValue)) {
				return propertyValue;
			}
			return null;
		}
		
		/// <summary>
		/// AssemblyName must be implemented correctly - used when renaming projects.
		/// </summary>
		public override string AssemblyName {
			get { return GetEvaluatedProperty("OutputName"); }
			set { SetProperty("OutputName", value); }
		}
		
		/// <summary>
		/// Returns a collection of FileProjectItems that match using the
		/// IsFileNameMatch delegate.
		/// </summary>
		ReadOnlyCollection<FileProjectItem> GetMatchingFiles(IsFileNameMatch match)
		{
			List<FileProjectItem> items = new List<FileProjectItem>();
			foreach (ProjectItem projectItem in Items) {
				FileProjectItem fileProjectItem = projectItem as FileProjectItem;
				if (fileProjectItem != null) {
					if (match(fileProjectItem.FileName)) {
						items.Add(fileProjectItem);
					}
				}
			}
			return new ReadOnlyCollection<FileProjectItem>(items);
		}
		
		/// <summary>
		/// Returns a collection of compiler extension items that match the specified
		/// type.
		/// </summary>
		ReadOnlyCollection<WixExtensionProjectItem> GetExtensions()
		{
			List<WixExtensionProjectItem> items = new List<WixExtensionProjectItem>();
			foreach (ProjectItem projectItem in Items) {
				WixExtensionProjectItem item = projectItem as WixExtensionProjectItem;
				if (item != null) {
					items.Add(item);
				}
			}
			return new ReadOnlyCollection<WixExtensionProjectItem>(items);
		}
		
		protected override ProjectBehavior CreateDefaultBehavior()
		{
			return new WixStartBehavior(this, base.CreateDefaultBehavior());
		}
	}
	
	public class WixStartBehavior : ProjectBehavior
	{
		public WixStartBehavior(WixProject project, ProjectBehavior next = null)
			: base(project, next)
		{
		}
		
		new WixProject Project {
			get {
				return (WixProject)base.Project;
			}
		}
		
		public string StartArguments {
			get {
				return Project.GetEvaluatedProperty("StartArguments") ?? "";
			}
			set {
				Project.SetProperty("StartArguments", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		public string StartWorkingDirectory {
			get {
				return Project.GetEvaluatedProperty("StartWorkingDirectory") ?? "";
			}
			set {
				Project.SetProperty("StartWorkingDirectory", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		public StartAction StartAction {
			get {
				try {
					return (StartAction)Enum.Parse(typeof(StartAction), Project.GetEvaluatedProperty("StartAction") ?? "Project");
				} catch (ArgumentException) {
					return StartAction.Project;
				}
			}
			set {
				Project.SetProperty("StartAction", value.ToString());
			}
		}
		
		public override void Start(bool withDebugging)
		{
			base.Start(false); // debugging not supported
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			switch (StartAction) {
				case StartAction.Project:
					return DotNetStartBehavior.CreateStartInfo(Project.GetInstallerFullPath(), Project.Directory, StartWorkingDirectory, StartArguments);
				default:
					return base.CreateStartInfo();
			}
		}
		
		/// <summary>
		/// Adds the ability to creates Wix Library and Wix Object project items.
		/// </summary>
		public override ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			switch (item.ItemType.ItemName) {
				case WixItemType.LibraryName:
					return new WixLibraryProjectItem(Project, item);
				case WixItemType.ExtensionName:
					return new WixExtensionProjectItem(Project, item);
				default:
					return base.CreateProjectItem(item);
			}
		}
		
		/// <summary>
		/// Checks whether the specified file can be compiled by the
		/// Wix project.
		/// </summary>
		/// <returns>
		/// <c>Compile</c> if the file is a WiX source file (.wxs)
		/// or a WiX include file (.wxi), otherwise the default implementation
		/// in MSBuildBasedProject is called.</returns>
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (WixFileName.IsWixFileName(fileName)) {
				return ItemType.Compile;
			}
			return base.GetDefaultItemType(fileName);
		}
	}
}
