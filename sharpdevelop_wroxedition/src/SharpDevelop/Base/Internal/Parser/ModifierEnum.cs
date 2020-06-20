// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;

namespace SharpDevelop.Internal.Parser
{
	[Flags]
	public enum ModifierEnum : uint {
		None       = 0,
		New        = (1 << 0),
		
		Public     = (1 << 1),
		Protected  = (1 << 2),
		Internal   = (1 << 3),
		Private    = (1 << 4),
		
		Abstract   = (1 << 5),
		Sealed     = (1 << 6),
		
		Static     = (1 << 7),
		Readonly   = (1 << 8),
		
		Virtual    = (1 << 9),
		Override   = (1 << 10),
		
		Extern     = (1 << 11),
		
		Unsafe     = (1 << 12),
		
		SpecialName = (1 << 13),
		
		ProtectedAndInternal = Internal | Protected,
		ProtectedOrInternal = (1 << 14),
		Literal             = (1 << 15),
		Const      = Static | Literal,
	}
}

