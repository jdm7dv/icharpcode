﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class NUnitConsoleApplication
	{
		public NUnitConsoleApplication(SelectedTests selectedTests, UnitTestingOptions options)
		{
			Initialize(selectedTests);
			InitializeOptions(options);
		}
		
		public NUnitConsoleApplication(SelectedTests selectedTests)
		{
			Initialize(selectedTests);
		}
		
		void Initialize(SelectedTests selectedTests)
		{
			this.selectedTests = selectedTests;
			this.project = selectedTests.Project;
			Assemblies.Add(project.OutputAssemblyFullPath);
			if (selectedTests.NamespaceFilter != null) {
				NamespaceFilter = selectedTests.NamespaceFilter;
			}
			if (selectedTests.Class != null) {
				Fixture = selectedTests.Class.DotNetName;
				if (selectedTests.Member != null) {
					Test = selectedTests.Member.Name;
				}
			}
		}
		
		void InitializeOptions(UnitTestingOptions options)
		{
			NoThread = options.NoThread;
			NoLogo = options.NoLogo;
			NoDots = options.NoDots;
			Labels = options.Labels;
			ShadowCopy = !options.NoShadow;
			NoXmlOutputFile = !options.CreateXmlOutputFile;
			
			if (options.CreateXmlOutputFile) {
				GenerateXmlOutputFileName();
			}
		}
		
		void GenerateXmlOutputFileName()
		{
			string directory = Path.GetDirectoryName(project.OutputAssemblyFullPath);
			string fileName = project.AssemblyName + "-TestResult.xml";
			XmlOutputFile = Path.Combine(directory, fileName);
		}
		
		/// <summary>
		/// returns full/path/to/Tools/NUnit
		/// </summary>
		string WorkingDirectory {
			get { return Path.Combine(FileUtility.ApplicationRootPath, @"bin\Tools\NUnit"); }
		}
				
		/// <summary>
		/// returns full/path/to/Tools/NUnit/nunit-console.exe or nunit-console-x86.exe if the
		/// project platform target is x86.
		/// </summary>
		public string FileName {
			get {
				string exe = "nunit-console";
				if (ProjectUsesDotnet20Runtime(project)) {
					exe += "-dotnet2";
				}
				// As SharpDevelop can't debug 64-bit applications yet, use
				// 32-bit NUnit even for AnyCPU test projects.
				if (IsPlatformTarget32BitOrAnyCPU(project)) {
					exe += "-x86";
				}
				exe += ".exe";
				return Path.Combine(WorkingDirectory, exe);
			}
		}
		
		public readonly List<string> Assemblies = new List<string>();
		
		/// <summary>
		/// Use shadow copy assemblies. Default = true.
		/// </summary>
		public bool ShadowCopy = true;
		
		/// <summary>
		/// Disables the use of a separate thread to run tests on separate thread. Default = false;
		/// </summary>
		public bool NoThread = false;
		
		/// <summary>
		/// Use /nologo directive.
		/// </summary>
		public bool NoLogo = false;
		
		/// <summary>
		/// Use /labels directive.
		/// </summary>
		public bool Labels = false;
		
		/// <summary>
		/// Use /nodots directive.
		/// </summary>
		public bool NoDots = false;
		
		/// <summary>
		/// File to write xml output to. Default = null.
		/// </summary>
		public string XmlOutputFile;
		
		/// <summary>
		/// Use /noxml.
		/// </summary>
		public bool NoXmlOutputFile = true;
		
		/// <summary>
		/// Fixture to test. Null = test all fixtures.
		/// </summary>
		public string Fixture;
		
		/// <summary>
		/// Test to run. Null = run all tests. Only valid together with the Fixture property.
		/// </summary>
		public string Test;
		
		/// <summary>
		/// File to write test results to.
		/// </summary>
		public string Results;
		
		/// <summary>
		/// The namespace that tests need to be a part of if they are to 
		/// be run.
		/// </summary>
		public string NamespaceFilter;
		
		IProject project;
		SelectedTests selectedTests;
		
		public SelectedTests SelectedTests {
			get { return selectedTests; }
		}
		
		public IProject Project {
			get { return project; }
		}
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = FileName;
			startInfo.Arguments = GetArguments();
			startInfo.WorkingDirectory = WorkingDirectory;
			return startInfo;
		}
		
		/// <summary>
		/// Gets the full command line to run the unit test application.
		/// This is the combination of the UnitTestApplication and
		/// the command line arguments.
		/// </summary>
		public string GetCommandLine()
		{
			return String.Format("\"{0}\" {1}", FileName, GetArguments());
		}
		
		/// <summary>
		/// Gets the arguments to use on the command line to run NUnit.
		/// </summary>
		public string GetArguments()
		{
			StringBuilder b = new StringBuilder();
			foreach (string assembly in Assemblies) {
				if (b.Length > 0)
					b.Append(' ');
				b.Append('"');
				b.Append(assembly);
				b.Append('"');
			}
			if (!ShadowCopy)
				b.Append(" /noshadow");
			if (NoThread)
				b.Append(" /nothread");
			if (NoLogo)
				b.Append(" /nologo");
			if (Labels) 
				b.Append(" /labels");
			if (NoDots) 
				b.Append(" /nodots");
			if (NoXmlOutputFile) {
				b.Append(" /noxml");
			} else if (XmlOutputFile != null) {
				b.Append(" /xml=\"");
				b.Append(XmlOutputFile);
				b.Append('"');
			}
			if (Results != null) {
				b.Append(" /results=\"");
				b.Append(Results);
				b.Append('"');
			}
			string run = null;
			if (NamespaceFilter != null) {
				run = NamespaceFilter;
			} else if (Fixture != null) {
				if (Test != null) {
					run = Fixture + "." + Test;
				} else {
					run = Fixture;
				}
			} else if (Test != null) {
				run = Test;
			}
			if (run != null) {
				b.Append(" /run=\"");
				b.Append(run);
				b.Append('"');
			}
			return b.ToString();
		}
		
		/// <summary>
		/// Checks that the project's PlatformTarget is x32 for the active configuration.
		/// </summary>
		bool IsPlatformTarget32BitOrAnyCPU(IProject project)
		{
			MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
			if (msbuildProject != null) {
				string platformTarget = msbuildProject.GetEvaluatedProperty("PlatformTarget");
				return String.Equals(platformTarget, "x86", StringComparison.OrdinalIgnoreCase)
					|| String.Equals(platformTarget, "AnyCPU", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		
		bool ProjectUsesDotnet20Runtime(IProject project)
		{
			var p = project as ICSharpCode.SharpDevelop.Project.Converter.IUpgradableProject;
			if (p != null && p.CurrentTargetFramework != null) {
				return p.CurrentTargetFramework.SupportedRuntimeVersion == "v2.0.50727";
			}
			return false;
		}
	}
}
