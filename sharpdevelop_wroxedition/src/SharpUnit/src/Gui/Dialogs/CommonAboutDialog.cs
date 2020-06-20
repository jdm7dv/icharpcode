/*
 * CommonAboutDialog.cs
 * Copyright (C) 2002 Mike Krueger, icsharpcode
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice, 
 *   this list of conditions and the following disclaimer. 
 * 
 * - Redistributions in binary form must reproduce the above copyright notice, 
 *   this list of conditions and the following disclaimer in the documentation 
 *   and/or other materials provided with the distribution. 
 * 
 * - Neither the name of icsharpcode nor the names of its contributors may 
 *   be used to endorse or promote products derived from this software without specific 
 *   prior written permission. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
 * SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
 * OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

using ICSharpCode.GUI.Xml;

namespace ICSharpCode.SharpUnit.Gui {
	
	public class ScrollBox : UserControl
	{
		Image  image;
		string text;
    	Timer  timer;
		int    scroll = -220;
		
		public int ScrollY {
			get {
				return scroll;
			}
			set {
				scroll = value;
			}
		}
		
		public Image Image {
			get {
				return image;
			}
			set {
				image = value;
			}
		}
		
		public string ScrollText {
			get {
				return text;
			}
			set {
				text =  value;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				timer.Stop();
			}
			base.Dispose(disposing);
		}
		
		public ScrollBox()
		{
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			Font = new Font("Tahoma", 10);
			
			text = "\"The most successful method of programming is to begin a program as simply as possible, test it, and then add to the program until it performs the required job.\"\n    -- PDP8 handbook, Pg 9-64";
			timer = new Timer();
			timer.Interval = 80;
			timer.Tick += new EventHandler(ScrollDown);
			timer.Start();
		}
		
		void ScrollDown(object sender, EventArgs e)
		{
			++scroll;
			Refresh();
		}
		
    	protected override void OnPaintBackground(PaintEventArgs pe)
    	{
    		if (image != null) {
    			pe.Graphics.DrawImage(image, 0, 0, Width, Height);
    		}
    	}
    	
    	protected override void OnPaint(PaintEventArgs pe)
    	{
    		Graphics g = pe.Graphics;
    		g.DrawString(text, Font, new SolidBrush(Color.Black), new Rectangle(Width / 2, 0 - scroll, Width / 2, Height));
    		SizeF size = g.MeasureString(text, Font);
    		if (scroll > (int)(size.Height + Height)) {
    			scroll = -(int)size.Height - Height;
    		}
    	}
	}
	
	public class CommonAboutDialog : XmlForm
	{
		public ScrollBox ScrollBox {
			get {
				return (ScrollBox)ControlDictionary["aboutPictureScrollBox"];
			}
		}
		
		public CommonAboutDialog() : base(Application.StartupPath + @"\..\data\SharpUnit\dialogs\CommonAboutDialog.xml")
		{
		}
	}
	
	public class AboutSharpUnitTabPage : UserControl
	{
		Label      buildLabel   = new Label();
		TextBox    buildTextBox = new TextBox();
		
		Label      versionLabel   = new Label();
		TextBox    versionTextBox = new TextBox();
		
		Label      sponsorLabel   = new Label();
		
		
		public AboutSharpUnitTabPage()
		{
			Version v = Assembly.GetExecutingAssembly().GetName().Version;
			versionTextBox.Text = v.Major + "." + v.Minor;
			buildTextBox.Text   = v.Revision + "." + v.Build;
			
			versionLabel.Location = new System.Drawing.Point(8, 8);
			versionLabel.Text = "Version";
			versionLabel.Size = new System.Drawing.Size(64, 16);
			versionLabel.TabIndex = 1;
			Controls.Add(versionLabel);
			
			versionTextBox.Location = new System.Drawing.Point(64 + 8 + 4, 8);
			versionTextBox.ReadOnly = true;
			versionTextBox.TabIndex = 4;
			versionTextBox.Size = new System.Drawing.Size(48, 20);
			Controls.Add(versionTextBox);
			
			buildLabel.Location = new System.Drawing.Point(64 + 12 + 48 + 4, 8);
			buildLabel.Text = "Build";
			buildLabel.Size = new System.Drawing.Size(48, 16);
			buildLabel.TabIndex = 2;
			Controls.Add(buildLabel);
			
			buildTextBox.Location = new System.Drawing.Point(64 + 12 + 48 + 4 + 48 + 4, 8);
			buildTextBox.ReadOnly = true;
			buildTextBox.TabIndex = 3;
			buildTextBox.Size = new System.Drawing.Size(72, 20);
			Controls.Add(buildTextBox);
			
			sponsorLabel.Location = new System.Drawing.Point(8, 34);
			sponsorLabel.Text = "Programmed 2002 by Mike Krueger.\nReleased under the terms of the BSD license.\n\n" + 
				                "sponsored by AlphaSierraPapa\n" +
			                    "                   www.AlphaSierraPapa.com";
			sponsorLabel.Size = new System.Drawing.Size(362, 74);
			sponsorLabel.TabIndex = 8;
			Controls.Add(sponsorLabel);
			Dock = DockStyle.Fill;
		}
	}	
}
