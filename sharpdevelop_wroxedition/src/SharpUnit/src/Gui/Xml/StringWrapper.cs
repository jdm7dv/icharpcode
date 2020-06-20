//  StringWrapper.cs
//  Copyright (C) 2002 Mike Krueger
// 
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.GUI.Xml {
	
	/// <summary>
	/// This class wrapps a string to an object, the string is accessible
	/// through the 'Text' property. This class was written for setting the
	/// items in a combobox inside a xml definition.
	/// </summary>
	public class StringWrapper 
	{
		string text;
		
		/// <summary>
		/// Get/Set the string.
		/// </summary>
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		/// <summary>
		/// returns <code>Text</code>
		/// </summary>
		public override string ToString()
		{
			return text;
		}
	}
}
