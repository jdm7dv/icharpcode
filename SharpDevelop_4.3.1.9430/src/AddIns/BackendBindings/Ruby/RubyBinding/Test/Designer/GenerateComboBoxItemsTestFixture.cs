﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateComboBoxItemsFormTestFixture
	{
		string generatedRubyCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Add combo box.
				ComboBox comboBox = (ComboBox)host.CreateComponent(typeof(ComboBox), "comboBox1");
				comboBox.TabIndex = 0;
				comboBox.Location = new Point(0, 0);
				comboBox.Size = new System.Drawing.Size(121, 21);
				comboBox.Items.Add("aaa");
				comboBox.Items.Add("bbb");
				comboBox.Items.Add("ccc");
				
				form.Controls.Add(comboBox);
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, 1);
				}	
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    @comboBox1 = System::Windows::Forms::ComboBox.new()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # comboBox1\r\n" +
								"    # \r\n" +
								"    @comboBox1.Items.AddRange(System::Array[System::Object].new(\r\n" +
								"        [\"aaa\",\r\n" +
								"        \"bbb\",\r\n" +
								"        \"ccc\"]))\r\n" +
								"    @comboBox1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @comboBox1.Name = \"comboBox1\"\r\n" +
								"    @comboBox1.Size = System::Drawing::Size.new(121, 21)\r\n" +
								"    @comboBox1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"    self.Controls.Add(@comboBox1)\r\n" +
 								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}		
	}
}
