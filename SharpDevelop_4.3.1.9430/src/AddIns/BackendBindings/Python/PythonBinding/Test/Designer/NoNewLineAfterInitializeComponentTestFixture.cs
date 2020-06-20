﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using AvalonEdit = ICSharpCode.AvalonEdit;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the code can be generated if there is no new line after the InitializeComponent method.
	/// </summary>
	[TestFixture]
	public class NoNewLineAfterInitializeComponentMethodTestFixture
	{
		TextDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			AvalonEdit.TextEditor textEditor = new AvalonEdit.TextEditor();
			document = textEditor.Document;
			textEditor.Text = GetTextEditorCode();

			PythonParser parser = new PythonParser();
			ICompilationUnit compilationUnit = parser.Parse(new DefaultProjectContent(), @"test.py", document.Text);

			using (DesignSurface designSurface = new DesignSurface(typeof(UserControl))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				UserControl userControl = (UserControl)host.RootComponent;			
				userControl.ClientSize = new Size(489, 389);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(userControl);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(userControl, "userControl1");
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					AvalonEditDocumentAdapter adapter = new AvalonEditDocumentAdapter(document, null);
					MockTextEditorOptions options = new MockTextEditorOptions();
					PythonDesignerGenerator generator = new PythonDesignerGenerator(options);
					generator.Merge(host, adapter, compilationUnit, serializationManager);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = 
				"from System.Windows.Forms import UserControl\r\n" +
				"\r\n" +
				"class MyUserControl(UserControl):\r\n" +
				"\tdef __init__(self):\r\n" +
				"\t\tself.InitializeComponent()\r\n" +
				"\t\r\n" +
				"\tdef InitializeComponent(self):\r\n" +
				"\t\tself.SuspendLayout()\r\n" +
				"\t\t# \r\n" +
				"\t\t# userControl1\r\n" +
				"\t\t# \r\n" + 
				"\t\tself.Name = \"userControl1\"\r\n" +
				"\t\tself.Size = System.Drawing.Size(489, 389)\r\n" +
				"\t\tself.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, document.Text);
		}
		
		/// <summary>
		/// No new line after the pass statement for InitializeComponent method.
		/// </summary>
		string GetTextEditorCode()
		{
			return "from System.Windows.Forms import UserControl\r\n" +
					"\r\n" +
					"class MyUserControl(UserControl):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
					 "\tdef InitializeComponent(self):\r\n" +
					"\t\tpass"; 						
		}
	}
}
