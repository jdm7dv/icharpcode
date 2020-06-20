// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Specialized;
using System.IO;

namespace SharpDevelop.Internal.Parser {
	
	class MainClass
	{
		public static void SearchDirectory(string directory, string filemask, StringCollection collection)
		{
			string[] file = Directory.GetFiles(directory, filemask);
			foreach (string f in file) {
				collection.Add(f);
			}
			
			string[] dir = Directory.GetDirectories(directory);
			foreach (string d in dir) {
				SearchDirectory(d, filemask, collection);
			}
		}
		
		public static void Main(string[] args)
		{
			StringCollection fileCollection = new StringCollection();
			SearchDirectory(@"C:\Programme\SharpDevelop\src\SharpDevelop", "*.cs", fileCollection);
			
			foreach (string filename in fileCollection) {
								
				Parser parse = new Parser();
				CompilationUnit unit = (CompilationUnit)parse.Parse(filename);
				
				foreach (IClass c in unit.Classes) {
					Console.WriteLine(c.Name + " von " + c.Region.BeginLine + " bis " + c.Region.EndLine);
				}
			}
		}
	}
}
