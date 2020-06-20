﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the controls are initialized in the order they were put on the form. 
	/// The forms designer has them in reverse order in the Controls collection.
	/// </summary>
	[TestFixture]
	public class GeneratedControlOrderingTestFixture
	{
		string generatedRubyCode;
		
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
				
				Button button = (Button)host.CreateComponent(typeof(Button), "button1");
				button.Location = new Point(0, 0);
				button.Size = new Size(10, 10);
				button.Text = "button1";
				button.TabIndex = 0;
				button.UseCompatibleTextRendering = false;
				form.Controls.Add(button);

				RadioButton radioButton = (RadioButton)host.CreateComponent(typeof(RadioButton), "radioButton1");
				radioButton.Location = new Point(20, 0);
				radioButton.Size = new Size(10, 10);
				radioButton.Text = "radioButton1";
				radioButton.TabIndex = 1;
				radioButton.UseCompatibleTextRendering = false;
				form.Controls.Add(radioButton);
				
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
			string expectedCode = "    @button1 = System::Windows::Forms::Button.new()\r\n" +
								"    @radioButton1 = System::Windows::Forms::RadioButton.new()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # button1\r\n" +
								"    # \r\n" +
								"    @button1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @button1.Name = \"button1\"\r\n" +
								"    @button1.Size = System::Drawing::Size.new(10, 10)\r\n" +
								"    @button1.TabIndex = 0\r\n" +
								"    @button1.Text = \"button1\"\r\n" +				
								"    # \r\n" +
								"    # radioButton1\r\n" +
								"    # \r\n" +
								"    @radioButton1.Location = System::Drawing::Point.new(20, 0)\r\n" +
								"    @radioButton1.Name = \"radioButton1\"\r\n" +
								"    @radioButton1.Size = System::Drawing::Size.new(10, 10)\r\n" +
								"    @radioButton1.TabIndex = 1\r\n" +
								"    @radioButton1.Text = \"radioButton1\"\r\n" +				
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(284, 264)\r\n" +
								"    self.Controls.Add(@button1)\r\n" +
								"    self.Controls.Add(@radioButton1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}
	}
}
