// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.Core.AddIns.Conditions;

namespace ICSharpCode.Core.AddIns.Codons
{
	[CodonNameAttribute("DisplayBinding")]
	public class DisplayBindingCodon : AbstractCodon
	{
		[XmlMemberArrayAttribute("supportedformats")]
		string[] supportedFormats;
		
		IDisplayBinding displayBinding;
		
		public string[] SupportedFormats {
			get {
				return supportedFormats;
			}
			set {
				supportedFormats = value;
			}
		}
		
		public IDisplayBinding DisplayBinding {
			get {
				return displayBinding;
			}
		}

		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public override object BuildItem(object owner, ArrayList subItems, ConditionFailedAction action)
		{
//			if (subItems == null || subItems.Count > 0) {
//				throw new ApplicationException("Tried to buil a command with sub commands, please check the XML definition.");
//			}
			
			Debug.Assert(Class != null && Class.Length > 0);
			
			displayBinding = (IDisplayBinding)AddIn.CreateObject(Class);
			
			return this;
		}
	
	}
}