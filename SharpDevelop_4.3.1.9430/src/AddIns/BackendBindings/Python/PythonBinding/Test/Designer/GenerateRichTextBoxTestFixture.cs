﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that multiline text is correctly generated for the RichTextBox.
	/// </summary>
	[TestFixture]
	public class GenerateRichTextBoxTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;			
				form.ClientSize = new Size(284, 264);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				RichTextBox textBox = (RichTextBox)host.CreateComponent(typeof(RichTextBox), "richTextBox1");
				textBox.Size = new Size(110, 20);
				textBox.TabIndex = 1;
				textBox.Location = new Point(10, 10);
				textBox.Text = "abc\r\n" +
					"def\r\n" +
					"ghi\r\n";
				
				form.Controls.Add(textBox);
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {					
					PythonCodeDomSerializer serializer = new PythonCodeDomSerializer("    ");
					generatedPythonCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    self._richTextBox1 = System.Windows.Forms.RichTextBox()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # richTextBox1\r\n" +
								"    # \r\n" +
								"    self._richTextBox1.Location = System.Drawing.Point(10, 10)\r\n" +
								"    self._richTextBox1.Name = \"richTextBox1\"\r\n" +
								"    self._richTextBox1.Size = System.Drawing.Size(110, 20)\r\n" +
								"    self._richTextBox1.TabIndex = 1\r\n" +
								"    self._richTextBox1.Text = \"\"\"abc\n" +
								"def\n" +
								"ghi\n" +
								"\"\"\"\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Controls.Add(self._richTextBox1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}		
	}
}
