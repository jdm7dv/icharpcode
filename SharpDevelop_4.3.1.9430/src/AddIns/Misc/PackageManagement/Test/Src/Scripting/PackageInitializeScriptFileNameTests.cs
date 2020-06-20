﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageInitializeScriptFileNameTests
	{
		PackageInitializeScriptFileName scriptFileName;
		FakeFileSystem fakeFileSystem;

		void CreateFileSystem()
		{
			fakeFileSystem = new FakeFileSystem();
		}
		
		void CreateFileName(string installPath)
		{
			scriptFileName = new PackageInitializeScriptFileName(installPath);
		}
		
		void CreateFakeFileSystem()
		{
			fakeFileSystem = new FakeFileSystem();
		}
		
		void CreateFileNameWithFakeFileSystem()
		{
			scriptFileName = new PackageInitializeScriptFileName(fakeFileSystem);
		}
		
		[Test]
		public void ToString_InstallDirectoryPassed_ReturnsFullPathToInitScript()
		{
			CreateFileName(@"d:\projects\MyProject\packages\Test");
			
			string actualFileName = scriptFileName.ToString();
			string expectedFileName = @"d:\projects\MyProject\packages\Test\tools\init.ps1";
			
			Assert.AreEqual(expectedFileName, actualFileName);
		}
		
		[Test]
		public void PackageInstallDirectory_InstallDirectoryPassed_ReturnsFullPathToInitScript()
		{
			CreateFileName(@"d:\projects\MyProject\packages\Test");
			
			string actualDirectory = scriptFileName.PackageInstallDirectory;
			string expectedDirectory = @"d:\projects\MyProject\packages\Test";
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
		
		[Test]
		public void ScriptDirectoryExists_FileSystemHasScriptDirectory_ReturnsTrue()
		{
			CreateFakeFileSystem();
			fakeFileSystem.DirectoryExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			bool exists = scriptFileName.ScriptDirectoryExists();
			
			Assert.IsTrue(exists);
		}
		
		[Test]
		public void ScriptDirectoryExists_FileSystemDoesNotHaveScriptDirectory_ReturnsFalse()
		{
			CreateFakeFileSystem();
			fakeFileSystem.DirectoryExistsReturnValue = false;
			CreateFileNameWithFakeFileSystem();
			
			bool exists = scriptFileName.ScriptDirectoryExists();
			
			Assert.IsFalse(exists);
		}
		
		[Test]
		public void ScriptDirectoryExists_FileSystemHasScriptDirectory_ToolsDirectoryCheckedForExistence()
		{
			CreateFakeFileSystem();
			fakeFileSystem.DirectoryExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			scriptFileName.ScriptDirectoryExists();
			
			Assert.AreEqual("tools", fakeFileSystem.PathPassedToDirectoryExists);
		}
		
		[Test]
		public void FileExists_FileSystemHasScriptFile_ReturnsTrue()
		{
			CreateFakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			bool exists = scriptFileName.FileExists();
			
			Assert.IsTrue(exists);	
		}
		
		[Test]
		public void FileExists_FileSystemDoesNotHaveScriptFile_ReturnsFalse()
		{
			CreateFakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = false;
			CreateFileNameWithFakeFileSystem();
			
			bool exists = scriptFileName.FileExists();
			
			Assert.IsFalse(exists);	
		}
		
		[Test]
		public void FileExists_FileSystemHasScriptFile_ToolsScriptFileCheckedForExistence()
		{
			CreateFakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			CreateFileNameWithFakeFileSystem();
			
			scriptFileName.FileExists();
			
			Assert.AreEqual(@"tools\init.ps1", fakeFileSystem.PathPassedToFileExists);
		}
		
		[Test]
		public void GetScriptDirectory_InstallDirectoryPassed_ReturnsInitScriptDirectory()
		{
			CreateFileName(@"d:\projects\MyProject\packages\Test");
			
			string actualDirectory = scriptFileName.GetScriptDirectory();
			string expectedDirectory = @"d:\projects\MyProject\packages\Test\tools";
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
	}
}
