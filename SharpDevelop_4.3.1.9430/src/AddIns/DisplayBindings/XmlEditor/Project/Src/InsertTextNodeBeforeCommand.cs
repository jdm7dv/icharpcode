﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Inserts a new text node to before the selected node.
	/// </summary>
	public class InsertTextNodeBeforeCommand : AbstractMenuCommand
	{		
		public override void Run()
		{
			XmlTreeViewContainerControl view = Owner as XmlTreeViewContainerControl;
			if (view != null) {
				view.InsertTextNodeBefore();
			}
		}
	}
}
