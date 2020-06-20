// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;

using ResEdit;
using ICSharpCode.Core.AddIns.Codons;

namespace ResourceEditor
{
	public class ResourceEditorDisplayBinding : IDisplayBinding
	{
		// IDisplayBinding interface
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName) == ".resources" || Path.GetExtension(fileName) == ".resx";
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return language == "ResourceFiles";
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			ResourceEditWrapper di2 = new ResourceEditWrapper();
			di2.LoadFile(fileName);
			return di2;
		}
		
		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return new ResourceEditWrapper();;
		}
	}
	
	/// <summary>
	/// This class describes the main functionalaty of a language codon
	/// </summary>
	public class ResourceEditWrapper : AbstractViewContent
	{
		ResourceEdit resedit = new ResourceEdit();
		
		public override Control Control {
			get {
				return resedit;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		void SetDirty(object sender, EventArgs e)
		{
			IsDirty = true;
		}
		
		public ResourceEditWrapper()
		{
			resedit.Changed += new EventHandler(SetDirty);
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
			resedit.Dispose();
		}
		
		public override void SaveFile()
		{
			SaveFile(ContentName);
		}
				
		public override void LoadFile(string filename)
		{
			resedit.LoadFile(filename);
			ContentName = filename;
			IsDirty = false;
		}
		
		public override void SaveFile(string filename)
		{
			resedit.SaveFile(filename);
			ContentName = filename;
			IsDirty = false;
		}
	}
}
