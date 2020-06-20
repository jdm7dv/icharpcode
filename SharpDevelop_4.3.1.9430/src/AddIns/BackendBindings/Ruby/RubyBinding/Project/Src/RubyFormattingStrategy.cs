﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.RubyBinding
{
	public class RubyFormattingStrategy : ScriptingFormattingStrategy
	{
		public override string LineComment {
			get { return "#"; }
		}
		
		protected override LineIndenter CreateLineIndenter(ITextEditor editor, IDocumentLine line)
		{
			return new RubyLineIndenter(editor, line);
		}
	}
}
