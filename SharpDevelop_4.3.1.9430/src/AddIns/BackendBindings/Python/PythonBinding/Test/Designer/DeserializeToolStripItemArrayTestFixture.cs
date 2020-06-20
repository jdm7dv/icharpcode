﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class DeserializeToolStripItemArrayTestFixture : DeserializeAssignmentTestFixtureBase
	{		
		ToolStripMenuItem fileMenuItem;
		ToolStripMenuItem editMenuItem;
		
		public override string GetPythonCode()
		{
			fileMenuItem = (ToolStripMenuItem)componentCreator.CreateComponent(typeof(ToolStripMenuItem), "fileToolStripMenuItem");
			editMenuItem = (ToolStripMenuItem)componentCreator.CreateComponent(typeof(ToolStripMenuItem), "editToolStripMenuItem");
			
			componentCreator.Add(fileMenuItem, "fileToolStripMenuItem");
			componentCreator.Add(editMenuItem, "editToolStripMenuItem");
			
			return "self.Items = System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
				"    [self._fileToolStripMenuItem,\r\n" + 
				"    self._editToolStripMenuItem])";
		}
		
		[Test]
		public void DeserializedObjectIsExpectedCustomColor()
		{
			ToolStripItem[] expectedArray = new ToolStripItem[] {fileMenuItem, editMenuItem};
			Assert.AreEqual(expectedArray, deserializedObject);
		}
		
		[Test]
		public void StringTypeResolved()
		{
			Assert.AreEqual("System.Windows.Forms.ToolStripItem", componentCreator.LastTypeNameResolved);
		}
	}
}
