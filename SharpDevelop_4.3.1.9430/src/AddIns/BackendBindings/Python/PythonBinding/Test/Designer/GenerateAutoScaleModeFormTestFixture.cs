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
	/// A Form has a base ContainerControl class which has an AutoScaleMode property. This has the following attributes:
	/// 
	/// Browsable = false
	/// DesignerSerializationVisibility = Hidden
	/// EditorBrowsable = EditorBrowsableState.Advanced
	/// 
	/// However the forms root designer overrides these and shows it in the designer. 
	/// 
	/// This test fixture checks that the AutoScaleMode value will be generated in the form's code
	/// by the python forms designer.
	/// </summary>
	[TestFixture]
	public class GenerateAutoScaleModeFormTestFixture
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

				PropertyDescriptor autoScaleModeDescriptor = descriptors.Find("AutoScaleMode", false);
				autoScaleModeDescriptor.SetValue(form, AutoScaleMode.Font);
				
				PropertyDescriptor autoScaleDimensionsDescriptor = descriptors.Find("AutoScaleDimensions", false);
				autoScaleDimensionsDescriptor.SetValue(form, new SizeF(6F, 13F));

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
								"    self.AutoScaleDimensions = System.Drawing.SizeF(6, 13)\r\n" +
								"    self.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font\r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
	}
}
