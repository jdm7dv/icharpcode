// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.XmlForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ReplaceInFilesDialog : XmlForm
	{
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		bool replaceMode;
		
		public ReplaceInFilesDialog(bool replaceMode)
		{
			this.replaceMode = replaceMode;
			if (replaceMode) {
				this.SetupFromXml(Application.StartupPath + @"\..\data\resources\dialogs\ReplaceInFilesDialog.xfrm");
				ControlDictionary["replacePatternComboBox"].Text = SearchReplaceInFilesManager.SearchOptions.ReplacePattern;
				ControlDictionary["replaceHelpButton"].Enabled = false;
			} else {
				this.SetupFromXml(Application.StartupPath + @"\..\data\resources\dialogs\FindInFilesDialog.xfrm");
			}
			
			ControlDictionary["findHelpButton"].Enabled = false;
			ControlDictionary["searchPatternComboBox"].Text = SearchReplaceInFilesManager.SearchOptions.SearchPattern;
			
			AcceptButton = (Button)ControlDictionary["findButton"];
			CancelButton = (Button)ControlDictionary["closeButton"];
			
			((ComboBox)ControlDictionary["specialSearchStrategyComboBox"]).Items.Add("Wildcards");
			((ComboBox)ControlDictionary["specialSearchStrategyComboBox"]).Items.Add(resourceService.GetString("Dialog.NewProject.SearchReplace.SearchStrategy.RegexSearch"));
			int index = 0;
			switch (SearchReplaceManager.SearchOptions.SearchStrategyType) {
				case SearchStrategyType.Normal:
				case SearchStrategyType.Wildcard:
					break;
				case SearchStrategyType.RegEx:
					index = 1;
					break;
			}
 			((ComboBox)ControlDictionary["specialSearchStrategyComboBox"]).SelectedIndex = index;
			
			((ComboBox)ControlDictionary["searchLocationComboBox"]).Items.Add("Directories");
			((ComboBox)ControlDictionary["searchLocationComboBox"]).Items.Add(resourceService.GetString("Global.Location.allopenfiles"));
			((ComboBox)ControlDictionary["searchLocationComboBox"]).Items.Add(resourceService.GetString("Global.Location.wholeproject"));
						
			index = 0;
			switch (SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType) {
				case DocumentIteratorType.AllOpenFiles:
					index = 1;
					break;
				case DocumentIteratorType.WholeCombine:
					index = 2;
					break;
			}
			
			((ComboBox)ControlDictionary["searchLocationComboBox"]).SelectedIndex = index;
			((ComboBox)ControlDictionary["searchLocationComboBox"]).SelectedIndexChanged += new EventHandler(SearchLocationCheckBoxChangedEvent);
			
			((CheckBox)ControlDictionary["useSpecialSearchStrategyCheckBox"]).CheckedChanged += new EventHandler(SpecialSearchStrategyCheckBoxChangedEvent);
			
			ControlDictionary["directoryTextBox"].Text = SearchReplaceInFilesManager.SearchOptions.SearchDirectory;
			ControlDictionary["fileMaskTextBox"].Text = SearchReplaceInFilesManager.SearchOptions.FileMask;
			((CheckBox)ControlDictionary["includeSubdirectoriesCheckBox"]).Checked = SearchReplaceInFilesManager.SearchOptions.SearchSubdirectories;
			
			ControlDictionary["browseButton"].Click += new EventHandler(BrowseDirectoryEvent);
			
			ControlDictionary["findButton"].Click += new EventHandler(FindEvent);
			
			if (replaceMode) {
				ControlDictionary["replaceAllButton"].Click += new EventHandler(ReplaceEvent);
			}
			
			
			SearchLocationCheckBoxChangedEvent(null, null);
			SpecialSearchStrategyCheckBoxChangedEvent(null, null);
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
			xmlLoader.ObjectCreator        = new SharpDevelopObjectCreator();
		}
		
		void FindEvent(object sender, EventArgs e)
		{
			if (SetupSearchReplaceInFilesManager()) {
				SearchReplaceInFilesManager.FindAll();
			}
		}
		
		void ReplaceEvent(object sender, EventArgs e)
		{
			if (SetupSearchReplaceInFilesManager()) {
				SearchReplaceInFilesManager.ReplaceAll();
			}
		}
		
		void BrowseDirectoryEvent(object sender, EventArgs e)
		{
			FolderDialog fd = new FolderDialog();
			if (fd.DisplayDialog("choose the search location") == DialogResult.OK) {
				ControlDictionary["directoryTextBox"].Text = fd.Path;
			}
		}
		
		void SearchLocationCheckBoxChangedEvent(object sender, EventArgs e)
		{
			bool enableDirectorySearch = ((ComboBox)ControlDictionary["searchLocationComboBox"]).SelectedIndex == 0;
			ControlDictionary["fileMaskTextBox"].Enabled = enableDirectorySearch;
			ControlDictionary["directoryTextBox"].Enabled = enableDirectorySearch;
			ControlDictionary["browseButton"].Enabled = enableDirectorySearch;
			ControlDictionary["includeSubdirectoriesCheckBox"].Enabled = enableDirectorySearch;
		}
		
		void SpecialSearchStrategyCheckBoxChangedEvent(object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)ControlDictionary["useSpecialSearchStrategyCheckBox"];
			if (cb != null) {
				ControlDictionary["specialSearchStrategyComboBox"].Enabled = cb.Checked;
			}
		}
		
		bool SetupSearchReplaceInFilesManager()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			string directoryName = ControlDictionary["directoryTextBox"].Text;
			string fileMask      = ControlDictionary["fileMaskTextBox"].Text;
			if (fileMask == null || fileMask.Length == 0) {
				fileMask = "*";
			}
			
			if (SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType == DocumentIteratorType.Directory) {
				
				if (!fileUtilityService.IsValidFileName(directoryName)) {
					MessageBox.Show("Invalid directory name " + directoryName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				
				if (!Directory.Exists(directoryName)) {
					MessageBox.Show("Directory " + directoryName + " does not exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				
				if (!fileUtilityService.IsValidFileName(fileMask) || fileMask.IndexOf('\\') >= 0) {
					MessageBox.Show("Invalid file mask " + fileMask, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
			}
			if (fileMask == null || fileMask.Length == 0) {
				SearchReplaceInFilesManager.SearchOptions.FileMask = "*";
			} else {
				SearchReplaceInFilesManager.SearchOptions.FileMask        = fileMask;
			}
			SearchReplaceInFilesManager.SearchOptions.SearchDirectory = directoryName;
			SearchReplaceInFilesManager.SearchOptions.SearchSubdirectories = ((CheckBox)ControlDictionary["includeSubdirectoriesCheckBox"]).Checked;
			
			SearchReplaceInFilesManager.SearchOptions.SearchPattern  = ControlDictionary["searchPatternComboBox"].Text;
			if (replaceMode) {
				SearchReplaceInFilesManager.SearchOptions.ReplacePattern = ControlDictionary["replacePatternComboBox"].Text;
			}
			
			SearchReplaceInFilesManager.SearchOptions.IgnoreCase          = !((CheckBox)ControlDictionary["ignoreCaseCheckBox"]).Checked;
			SearchReplaceInFilesManager.SearchOptions.SearchWholeWordOnly = ((CheckBox)ControlDictionary["searchWholeWordOnlyCheckBox"]).Checked;
			
			if (((CheckBox)ControlDictionary["useSpecialSearchStrategyCheckBox"]).Checked) {
				switch (((ComboBox)ControlDictionary["specialSearchStrategyComboBox"]).SelectedIndex) {
					case 0:
						SearchReplaceInFilesManager.SearchOptions.SearchStrategyType = SearchStrategyType.Wildcard;
						break;
					case 1:
						SearchReplaceInFilesManager.SearchOptions.SearchStrategyType = SearchStrategyType.RegEx;
						break;
				}
			} else {
				SearchReplaceInFilesManager.SearchOptions.SearchStrategyType = SearchStrategyType.Normal;
			}
			
			switch (((ComboBox)ControlDictionary["searchLocationComboBox"]).SelectedIndex) {
				case 0:
					SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.Directory;
					break;
				case 1:
					SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.AllOpenFiles;
					break;
				case 2:
					SearchReplaceInFilesManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.WholeCombine;
					break;
			}
			return true;
		}

	}
}
