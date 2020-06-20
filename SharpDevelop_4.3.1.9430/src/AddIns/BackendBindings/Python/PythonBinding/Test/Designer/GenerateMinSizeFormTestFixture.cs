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
	/// Tests that a form's MinimumSize, AutoScrollMinSize and AutoScrollMargin properties are generated
	/// in the InitializeComponent method.
	/// </summary>
	[TestFixture]
	public class GenerateMinSizeFormTestFixture
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
				PropertyDescriptor descriptor = descriptors.Find("MinimumSize", false);
				descriptor.SetValue(form, new Size(100, 200));
				descriptor = descriptors.Find("AutoScrollMinSize", false);
				descriptor.SetValue(form, new Size(10, 20));
				descriptor = descriptors.Find("AutoScrollMargin", false);
				descriptor.SetValue(form, new Size(11, 22));
				descriptor = descriptors.Find("AutoScroll", false);
				descriptor.SetValue(form, false);
				
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
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
			string expectedCode = "    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.AutoScrollMargin = System.Drawing.Size(11, 22)\r\n" +
								"    self.AutoScrollMinSize = System.Drawing.Size(10, 20)\r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.MinimumSize = System.Drawing.Size(100, 200)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
	}
}
