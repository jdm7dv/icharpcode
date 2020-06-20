﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public interface ITextEditorWithParseInformationFoldingFactory
	{
		ITextEditorWithParseInformationFolding CreateTextEditor(ITextEditor textEditor);
	}
}
