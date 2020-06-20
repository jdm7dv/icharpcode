// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.Core.Services;

namespace ICSharpCode.TextEditor.Document
{
	public class HighlightingManager
	{
		static readonly string syntaxHighlightingPath = Application.StartupPath +
		                                                Path.DirectorySeparatorChar + ".." +
		                                                Path.DirectorySeparatorChar + "data" +
		                                                Path.DirectorySeparatorChar + "modes";
		
		static HighlightingManager highlightingManager;
		Hashtable highlightingDefinitions;
		
		public HighlightingManager()
		{
			highlightingDefinitions = new Hashtable();
			
			LoadDefinitions();
		}
		
		static HighlightingManager()
		{
			highlightingManager = new HighlightingManager();
			highlightingManager.ResolveReferences();
		}
		
		IHighlightingStrategy CreateDefaultHighlightingStrategy()
		{
			DefaultHighlightingStrategy defaultHighlightingStrategy = new DefaultHighlightingStrategy();
			defaultHighlightingStrategy.Extensions = new string[] {};
			defaultHighlightingStrategy.Rules.Add(new HighlightRuleSet());
			return defaultHighlightingStrategy;
		}
		
		// leave this method here and don't use the file utility service because this assembly must not depend
		// on the core assembly
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
		void LoadDefinitions()
		{
			if (!Directory.Exists(syntaxHighlightingPath)) {
				highlightingDefinitions["Default"] = CreateDefaultHighlightingStrategy();
				return;
			}
			
			StringCollection files = new StringCollection();
			SearchDirectory(syntaxHighlightingPath, "*.xshd", files, true);
	
			foreach(string aFile in files) {
				try {
					DefaultHighlightingStrategy highlighter = HighlightingDefinitionParser.Parse(aFile);
					if (highlighter != null) {
						highlightingDefinitions[highlighter.Name] = highlighter;
					}
				} catch (Exception e) {
					throw new ApplicationException("can't load xml syntax definition file " + aFile + "\n" + e.ToString());
				}
			}
		}
		
		void ResolveReferences()
		{
			IDictionaryEnumerator iterator = highlightingDefinitions.GetEnumerator();
			while (iterator.MoveNext()) {
				((DefaultHighlightingStrategy)iterator.Value).ResolveReferences();
			}
		}
		
		public static HighlightingManager Manager
		{
			get {
				return highlightingManager;		
			}
		}
		
		public Hashtable HighlightingDefinitions
		{
			get {
				return highlightingDefinitions;
			}
		}
		
		public IHighlightingStrategy FindHighlighter(string name)
		{
			return (IHighlightingStrategy)highlightingDefinitions[name];
		}
		
		public IHighlightingStrategy FindHighlighterForFile(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToUpper();
			foreach (DictionaryEntry entry in highlightingDefinitions) {
				foreach (string ext in ((IHighlightingStrategy)entry.Value).Extensions) {
					if (ext.ToUpper() == extension) {
						return (IHighlightingStrategy)entry.Value;
					}
				}
			}
			return (IHighlightingStrategy)highlightingDefinitions["Default"];
		}
	}
}
