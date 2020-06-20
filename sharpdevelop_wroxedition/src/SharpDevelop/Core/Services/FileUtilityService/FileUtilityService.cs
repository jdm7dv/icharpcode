// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.Core.Services
{
	public enum DriveType {
		Unknown     = 0,
		NoRoot      = 1,
		Removeable  = 2,
		Fixed       = 3,
		Remote      = 4,
		Cdrom       = 5,
		Ramdisk     = 6
	}
	
	public enum FileErrorPolicy {
		Inform,
		ProvideAlternative
	}
	
	public enum FileOperationResult {
		OK,
		Failed,
		SavedAlternatively
	}
	
	public delegate void FileOperationDelegate();
	
	public delegate void NamedFileOperationDelegate(string fileName);
	
	/// <summary>
	/// A utility class related to file utilities.
	/// </summary>
	public class FileUtilityService : AbstractService
	{
		ImageList imagelist            = new ImageList();
		Hashtable extensionHashtable   = new Hashtable();
		Hashtable projectFileHashtable = new Hashtable();
		Hashtable customIcons          = new Hashtable();
		
		readonly static char[] separators = {Path.DirectorySeparatorChar, Path.VolumeSeparatorChar};
		
		public ImageList ImageList {
			get {
				return imagelist;
			}
		}
		
		int initalsize = 0;
		int offset     = 0;
		
		public FileUtilityService()
		{
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
			InitializeIcons(AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Icons"));
		}
		
		public override void UnloadService()
		{
			base.UnloadService();
			imagelist.Dispose();
		}
		
		public Bitmap GetBitmap(string name)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			
			if (customIcons[name] != null) {
				return (Bitmap)customIcons[name];
			}
			
			if (resourceService.GetBitmap(name) != null) {
				return resourceService.GetBitmap(name);
			}
			
			return resourceService.GetBitmap("Icons.16x16.MiscFiles");
		}
		
		public Image GetImageForProjectType(string projectType)
		{
			int index = GetImageIndexForProjectType(projectType);
			if (index >= 0) {
				return imagelist.Images[index];
			}
			return null;
		}
		
		public int GetImageIndexForProjectType(string projectType)
		{
			if (projectFileHashtable[projectType] != null) {
				return (int)projectFileHashtable[projectType];
			}
			return (int)extensionHashtable[".PRJX"];
		}
		
		public Image GetImageForFile(string fileName)
		{
			int index = GetImageIndexForFile(fileName);
			if (index >= 0) {
				return imagelist.Images[index];
			}
			return null;
		}
		
		public int GetImageIndexForFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToUpper();
			if (extensionHashtable[extension] != null) {
				return (int)extensionHashtable[extension];
			}
			return initalsize;
		}
		
		void InitializeIcons(IAddInTreeNode treeNode)
		{
			imagelist = new ImageList();
			
			initalsize  = imagelist.Images.Count;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			
			imagelist.Images.Add(resourceService.GetBitmap("Icons.16x16.MiscFiles"));
			
			extensionHashtable[".PRJX"] = imagelist.Images.Count;
			imagelist.Images.Add(resourceService.GetBitmap("Icons.16x16.SolutionIcon"));
			
			extensionHashtable[".CMBX"] = imagelist.Images.Count;
			imagelist.Images.Add(resourceService.GetBitmap("Icons.16x16.CombineIcon"));
			
			offset = imagelist.Images.Count;
			
			IconCodon[] icons = (IconCodon[])treeNode.BuildChildItems(null).ToArray(typeof(IconCodon));
			foreach (IconCodon iconCodon in icons) {
				Bitmap iconBitmap;
				
				if (iconCodon.Location != null) {
					iconBitmap = new Bitmap(iconCodon.Location);
					customIcons[iconCodon.ID] = iconBitmap;
				} else {
					iconBitmap = GetBitmap(iconCodon.ID);
				}
				imagelist.Images.Add(iconBitmap);
				
				if (iconCodon.Extensions != null) {
					foreach (string ext in iconCodon.Extensions) {
						extensionHashtable[ext.ToUpper()] = imagelist.Images.Count - 1;
					}
				}
				
				if (iconCodon.Language != null) {
					projectFileHashtable[iconCodon.Language] = imagelist.Images.Count - 1;
				}
				++offset;
			}
		}
		
		class NativeMethods {
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern int GetVolumeInformation(string volumePath,
			                                              StringBuilder volumeNameBuffer,
			                                              int volNameBuffSize,
			                                              ref int volumeSerNr,
			                                              ref int maxComponentLength,
			                                              ref int fileSystemFlags,
			                                              StringBuilder fileSystemNameBuffer,
			                                              int fileSysBuffSize);
			
			[DllImport("kernel32.dll")]
			public static extern DriveType GetDriveType(string driveName);
		}
		
		public string VolumeLabel(string volumePath)
		{
			try {
				StringBuilder volumeName  = new StringBuilder(128);
				int dummyInt = 0;
				NativeMethods.GetVolumeInformation(volumePath,
				                                   volumeName,
				                                   128,
				                                   ref dummyInt,
				                                   ref dummyInt,
				                                   ref dummyInt,
				                                   null,
				                                   0);
				return volumeName.ToString();
			} catch (Exception) {
				return String.Empty;
			}
		}
		
		public DriveType GetDriveType(string driveName)
		{
			return NativeMethods.GetDriveType(driveName);
		}
		
		public StringCollection SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			StringCollection collection = new StringCollection();
			SearchDirectory(directory, filemask, collection, searchSubdirectories);
			return collection;
		}
		
		public StringCollection SearchDirectory(string directory, string filemask)
		{
			return SearchDirectory(directory, filemask, true);
		}
		
		/// <summary>
		/// Finds all files which are valid to the mask <code>filemask</code> in the path
		/// <code>directory</code> and all subdirectories (if searchSubdirectories
		/// is true. The found files are added to the StringCollection 
		/// <code>collection</code>.
		/// </summary>
		void SearchDirectory(string directory, string filemask, StringCollection collection, bool searchSubdirectories)
		{
			try {
				string[] file = Directory.GetFiles(directory, filemask);
				foreach (string f in file) {
					collection.Add(f);
				}
				
				if (searchSubdirectories) {
					string[] dir = Directory.GetDirectories(directory);
					foreach (string d in dir) {
						SearchDirectory(d, filemask, collection, searchSubdirectories);
					}
				}
			} catch (Exception e) {
				MessageBox.Show("Can't access directory " + directory + " reason:\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		
		/// <summary>
		/// Converts a given absolute path and a given base path to a path that leads
		/// from the base path to the absoulte path. (as a relative path)
		/// </summary>
		public string AbsoluteToRelativePath(string baseDirectoryPath, string absPath)
		{
			string[] bPath = baseDirectoryPath.Split(separators);
			string[] aPath = absPath.Split(separators);
			int indx = 0;
			for(; indx < Math.Min(bPath.Length, aPath.Length); ++indx){
				if(!bPath[indx].Equals(aPath[indx]))
					break;
			}
			
			if (indx == 0) {
				return absPath;
			}
			
			string erg = "";
			
			if(indx == bPath.Length) {
				erg += "." + Path.DirectorySeparatorChar;
			} else {
				for (int i = indx; i < bPath.Length; ++i) {
					erg += ".." + Path.DirectorySeparatorChar;
				}
			}
			erg += String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length-indx);
			
			return erg;
		}
		
		/// <summary>
		/// Converts a given relative path and a given base path to a path that leads
		/// to the relative path absoulte.
		/// </summary>
		public string RelativeToAbsolutePath(string baseDirectoryPath, string relPath)
		{
			if (separators[0] != separators[1] && relPath.IndexOf(separators[1]) != -1) {
				return relPath;
			}
			string[] bPath = baseDirectoryPath.Split(separators[0]);
			string[] rPath = relPath.Split(separators[0]);
			int indx = 0;
			
			for (; indx < rPath.Length; ++indx) {
				if (!rPath[indx].Equals("..")) {
					break;
				}
			}
			
			if (indx == 0) {
				return baseDirectoryPath + separators[0] + String.Join(Path.DirectorySeparatorChar.ToString(), rPath, 1, rPath.Length-1);
			}
			
			string erg = String.Join(Path.DirectorySeparatorChar.ToString(), bPath, 0, bPath.Length-indx);
			
			erg += separators[0] + String.Join(Path.DirectorySeparatorChar.ToString(), rPath, indx, rPath.Length-indx);
			
			return erg;
		}
		
		/// <summary>
		/// This method checks the file fileName if it is valid.
		/// </summary>
		public bool IsValidFileName(string fileName)
		{
			// Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
			//        I can't find a .NET property or method for determining this variable.
			if (fileName == null || fileName.Length == 0 || fileName.Length >= 260) {
				return false;
			}
			
			// platform independend : check for invalid path chars
			foreach (char invalidChar in Path.InvalidPathChars) {
				if (fileName.IndexOf(invalidChar) >= 0) {
					return false;
				}
			}
			
			// platform dependend : Check for invalid file names (DOS)
			// this routine checks for follwing bad file names :
				// CON, PRN, AUX, NUL, COM1-9 and LPT1-9
				string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (nameWithoutExtension != null) {
				nameWithoutExtension = nameWithoutExtension.ToUpper();
			}
			if (nameWithoutExtension == "CON" ||
			    nameWithoutExtension == "PRN" ||
			    nameWithoutExtension == "AUX" ||
			    nameWithoutExtension == "NUL") {
			    	return false;
			    }
			    
			    char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';
			
			return !((nameWithoutExtension.StartsWith("COM") ||
			          nameWithoutExtension.StartsWith("LPT")) &&
			          Char.IsDigit(ch));
		}
		
		public bool TestFileExists(string filename)
		{
			if (!File.Exists(filename)) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				
				MessageBox.Show(stringParserService.Parse(resourceService.GetString("Fileutility.CantFindFileError"), new string[,] { {"FILE",  filename} }),
				                resourceService.GetString("Global.WarningText"),
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Warning);
				return false;
			}
			return true;
		}
		
		public bool IsDirectory(string filename)
		{
			FileAttributes attr = File.GetAttributes(filename);
			return (attr & FileAttributes.Directory) != 0;
		}
		
		/// <summary>
		/// Returns directoryName + "\\" (Win32) when directoryname doesn't end with
		/// "\\"
		/// </summary>
		public string GetDirectoryNameWithSeparator(string directoryName)
		{
			if (directoryName.EndsWith(Path.DirectorySeparatorChar.ToString())) {
				return directoryName;
			}
			return directoryName + Path.DirectorySeparatorChar;
		}
		
		// Observe SAVE functions
		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
			try {
				saveFile();
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while saving", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while saving", e, false)) {
							switch (chooseDialog.ShowDialog()) {
								case DialogResult.OK: // choose location (never happens here)
								break;
								case DialogResult.Retry:
									return ObservedSave(saveFile, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			return ObservedSave(saveFile,
			                    fileName,
			                    resourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
			                    policy);
		}
		
		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName)
		{
			return ObservedSave(saveFile, fileName, FileErrorPolicy.Inform);
		}
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
			try {
				saveFileAs(fileName);
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while saving", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						restartlabel:
							using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while saving", e, true)) {
								switch (chooseDialog.ShowDialog()) {
									case DialogResult.OK:
										using (SaveFileDialog fdiag = new SaveFileDialog()) {
											fdiag.OverwritePrompt = true;
											fdiag.AddExtension    = true;
											fdiag.CheckFileExists = false;
											fdiag.CheckPathExists = true;
											fdiag.Title           = "Choose alternate file name";
											fdiag.FileName        = fileName;
											if (fdiag.ShowDialog() == DialogResult.OK) {
												return ObservedSave(saveFileAs, fdiag.FileName, message, policy);
											} else {
												goto restartlabel;
											}
										}
										case DialogResult.Retry:
											return ObservedSave(saveFileAs, fileName, message, policy);
									case DialogResult.Ignore:
										return FileOperationResult.Failed;
								}
							}
							break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			return ObservedSave(saveFileAs,
			                    fileName,
			                    resourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
			                    policy);
		}
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedSave(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
		
		// Observe LOAD functions
		public FileOperationResult ObservedLoad(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
			try {
				saveFile();
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while loading", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while loading", e, false)) {
							switch (chooseDialog.ShowDialog()) {
								case DialogResult.OK: // choose location (never happens here)
								break;
								case DialogResult.Retry:
									return ObservedLoad(saveFile, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedLoad(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			return ObservedLoad(saveFile,
			                    fileName,
			                    resourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
			                    policy);
		}
		
		public FileOperationResult ObservedLoad(FileOperationDelegate saveFile, string fileName)
		{
			return ObservedSave(saveFile, fileName, FileErrorPolicy.Inform);
		}
		
		class LoadWrapper
		{
			NamedFileOperationDelegate saveFileAs;
			string fileName;
			
			public LoadWrapper(NamedFileOperationDelegate saveFileAs, string fileName)
			{
				this.saveFileAs = saveFileAs;
				this.fileName   = fileName;
			}
			public void Invoke()
			{
				saveFileAs(fileName);
			}
		}
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			return ObservedLoad(new FileOperationDelegate(new LoadWrapper(saveFileAs, fileName).Invoke), fileName, message, policy);
		}
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			return ObservedLoad(saveFileAs,
			                    fileName,
			                    resourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
			                    policy);
		}
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedLoad(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
	}
}
