﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using IronRuby.Compiler.Ast;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	class PublicPanelBaseForm : Form
	{
		public Panel panel1 = new Panel();
		Button button1 = new Button();
		
		public PublicPanelBaseForm()
		{
			button1.Name = "button1";

			panel1.Name = "panel1";
			panel1.Location = new Point(5, 10);
			panel1.Size = new Size(200, 100);
			panel1.Controls.Add(button1);

			Controls.Add(panel1);
		}
	}
	
	class PublicPanelDerivedForm : PublicPanelBaseForm 
	{
	}
	
	[TestFixture]
	public class LoadInheritedPublicPanelFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				base.ComponentCreator.AddType("RubyBinding.Tests.Designer.PublicPanelDerivedForm", typeof(PublicPanelDerivedForm));
				
				return
					"class MainForm < RubyBinding::Tests::Designer::PublicPanelDerivedForm\r\n" +
					"    def InitializeComponent()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # panel1\r\n" +
					"        # \r\n" +
					"        self.panel1.Location = System::Drawing::Point.new(10, 15)\r\n" +
					"        self.panel1.Size = System::Drawing::Size.new(108, 120)\r\n" +
					"        # \r\n" +
					"        # form1\r\n" +
					"        # \r\n" +
					"        self.Location = System::Drawing::Point.new(10, 20)\r\n" +
					"        self.Name = \"form1\"\r\n" +
					"        self.Controls.Add(@textBox1)\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		PublicPanelDerivedForm DerivedForm { 
			get { return Form as PublicPanelDerivedForm; }
		}
				
		[Test]
		public void PanelLocation()
		{
			Assert.AreEqual(new Point(10, 15), DerivedForm.panel1.Location);
		}

		[Test]
		public void PanelSize()
		{
			Assert.AreEqual(new Size(108, 120), DerivedForm.panel1.Size);
		}
		
		[Test]
		public void GetPublicPanelObject()
		{
			Assert.AreEqual(DerivedForm.panel1, RubyControlFieldExpression.GetInheritedObject("panel1", DerivedForm));
		}		
	}
}
