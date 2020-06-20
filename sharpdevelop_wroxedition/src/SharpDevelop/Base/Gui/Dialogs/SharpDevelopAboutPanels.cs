// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class AboutSharpDevelopTabPage : UserControl
	{
		Label      buildLabel   = new Label();
		TextBox    buildTextBox = new TextBox();
		
		Label      versionLabel   = new Label();
		TextBox    versionTextBox = new TextBox();
		
		Label      sponsorLabel   = new Label();
		
		
		public AboutSharpDevelopTabPage()
		{
			Version v = Assembly.GetExecutingAssembly().GetName().Version;
			versionTextBox.Text = v.Major + "." + v.Minor;
			buildTextBox.Text   = v.Revision + "." + v.Build;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			versionLabel.Location = new System.Drawing.Point(8, 8);
			versionLabel.Text = resourceService.GetString("Dialog.About.label1Text");
			versionLabel.Size = new System.Drawing.Size(64, 16);
			versionLabel.TabIndex = 1;
			Controls.Add(versionLabel);
			
			versionTextBox.Location = new System.Drawing.Point(64 + 8 + 4, 8);
			versionTextBox.ReadOnly = true;
			versionTextBox.TabIndex = 4;
			versionTextBox.Size = new System.Drawing.Size(48, 20);
			Controls.Add(versionTextBox);
			
			buildLabel.Location = new System.Drawing.Point(64 + 12 + 48 + 4, 8);
			buildLabel.Text = resourceService.GetString("Dialog.About.label2Text");
			buildLabel.Size = new System.Drawing.Size(48, 16);
			buildLabel.TabIndex = 2;
			Controls.Add(buildLabel);
			
			buildTextBox.Location = new System.Drawing.Point(64 + 12 + 48 + 4 + 48 + 4, 8);
			buildTextBox.ReadOnly = true;
			buildTextBox.TabIndex = 3;
			buildTextBox.Size = new System.Drawing.Size(72, 20);
			Controls.Add(buildTextBox);
			
			sponsorLabel.Location = new System.Drawing.Point(8, 34);
			sponsorLabel.Text = "Released under the GNU General Public license.\n\n" + 
				                "Sponsored by AlphaSierraPapa\n" +
			                    "                   http://www.AlphaSierraPapa.com";
			sponsorLabel.Size = new System.Drawing.Size(362, 74);
			sponsorLabel.TabIndex = 8;
			Controls.Add(sponsorLabel);
			Dock = DockStyle.Fill;
		}
	}
	
	public class AuthorAboutTabPage : RichTextBox
	{
		public AuthorAboutTabPage()
		{
			LoadFile(Application.StartupPath + 
			         Path.DirectorySeparatorChar + ".." + 
			         Path.DirectorySeparatorChar + "doc" +
			         Path.DirectorySeparatorChar + "AUTHORS.txt", RichTextBoxStreamType.PlainText);
			ReadOnly = true;
			Dock = DockStyle.Fill;
		}
	}
	
	public class ChangeLogTabPage : RichTextBox
	{
		public ChangeLogTabPage()
		{
			LoadFile(Application.StartupPath + 
			         Path.DirectorySeparatorChar + ".." +
			         Path.DirectorySeparatorChar + "doc" + 
			         Path.DirectorySeparatorChar + "ChangeLog.txt", RichTextBoxStreamType.PlainText);
			ReadOnly = true;
			Dock = DockStyle.Fill;
		}
	}
}
