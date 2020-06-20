﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SmartTagTest
{
	internal class NemerleImplementsSmartTag : SmartTag
	{
		public NemerleImplementsSmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets) : base(SmartTagType.Factoid, actionSets)
		{
		}
	}
}
