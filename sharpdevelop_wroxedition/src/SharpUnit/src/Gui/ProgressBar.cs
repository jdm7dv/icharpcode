/*
 * Progressbar.cs
 * Copyright (C) 2002 Mike Krueger, icsharpcode
 * Original file was included in NUnit Progressbar.cs Copyright 2000 by Philip Craig
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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;

namespace ICSharpCode.SharpUnit.Gui {
	
	public class TestEnvironmentProgressBar : UserControl
	{
		const int LeftRightBorder = 10;
		
		int myValue = 0;
		int min = 0;
		int max = 100;
		
		Color startColor = Color.Lime;
		Color endColor   = Color.Red;
		
		Brush backgroundDim  = null;
		Brush baseBackground = null;
		
		bool changeBrush  = true;
		bool errorOccured = false;
		
		public TestEnvironmentProgressBar() 
		{
			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}
		
		public int Minimum {
			get { 
				return min; 
			}
			set { 
				min = value; 
			}
		}
		
		public int Maximum {
			get { 
				return max; 
			}
			set {
				max = value;
				Refresh();
			}
		}
		
		public Color StatusColor {
			get {
				return errorOccured ? Color.Red : Color.Lime;
			}
		}
		
		public bool Error {
			get { 
				return errorOccured; 
			}
			set {
				errorOccured = value;
				changeBrush = true;
				Refresh();
			}
		}
		
		public int Value {
			get { 
				return myValue; 
			}
			set {
				myValue = value;
				Refresh();
			}
		}
		
		public void Step()
		{
			++Value;
		}
		
		protected override void OnPaint(PaintEventArgs e) 
		{
			base.OnPaint(e);
			
			if (changeBrush) {
				changeBrush = false;
				baseBackground = new SolidBrush(StatusColor);
				backgroundDim  = new SolidBrush(BackColor);
			}
			
			Rectangle r = Rectangle.Inflate(ClientRectangle, -2, -2);
			Rectangle toDim = r;
			float percentValue = ((float)myValue / ((float)Maximum - (float)Minimum));
			int nonDimLength   = (int)(percentValue * (float)toDim.Width);
			toDim.X += nonDimLength;
			toDim.Width -= nonDimLength;
			
			ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle);
			e.Graphics.FillRectangle(baseBackground, r);
			e.Graphics.FillRectangle(backgroundDim, toDim);
			e.Graphics.Flush();
		}
	}
}
