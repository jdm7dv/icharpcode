﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateImageListResourceTestFixture
	{
		MockResourceWriter resourceWriter;
		MockComponentCreator componentCreator;
		string generatedRubyCode;
		Icon icon;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resourceWriter = new MockResourceWriter();
			componentCreator = new MockComponentCreator();
			componentCreator.SetResourceWriter(resourceWriter);
			
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				host.AddService(typeof(IResourceService), componentCreator);
				
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Add ImageList.
				icon = new Icon(typeof(GenerateFormResourceTestFixture), "App.ico");
				ImageList imageList = (ImageList)host.CreateComponent(typeof(ImageList), "imageList1");
				imageList.Images.Add("App.ico", icon);
				imageList.Images.Add("", icon);
				imageList.Images.Add("", icon);
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, "RootNamespace", 1);
				}
			}
		}
		
		[Test]
		public void TearDownFixture()
		{
			icon.Dispose();
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    @components = System::ComponentModel::Container.new()\r\n" +
								"    resources = System::Resources::ResourceManager.new(\"RootNamespace.MainForm\", System::Reflection::Assembly.GetEntryAssembly())\r\n" +
								"    @imageList1 = System::Windows::Forms::ImageList.new(@components)\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # imageList1\r\n" +
								"    # \r\n" +
								"    @imageList1.ImageStream = resources.GetObject(\"imageList1.ImageStream\")\r\n" +
								"    @imageList1.TransparentColor = System::Drawing::Color.Transparent\r\n" +
								"    @imageList1.Images.SetKeyName(0, \"App.ico\")\r\n" +
								"    @imageList1.Images.SetKeyName(1, \"\")\r\n" +
								"    @imageList1.Images.SetKeyName(2, \"\")\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}
		
		[Test]
		public void ImageStreamAddedToResourceWriter()
		{
			Assert.IsNotNull(resourceWriter.GetResource("imageList1.ImageStream"));
		}
	}
}
