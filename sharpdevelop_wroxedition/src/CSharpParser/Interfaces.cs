// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections;

namespace SharpDevelop.Internal.Parser {
	
	public interface Scope
	{
		IRegion Region {
			get;
		}
	}
	
	public interface ClassContent : Scope
	{
		string FullyQualifiedName{
			get;
		}
		string Name{
			get;
		}
		Type Parent{
			get;
		}
	}
	
	public interface Member : ClassContent
	{
		IReturnType ReturnType{
			get;
		}
		bool InterfaceContent{
			get;
		}
		void Parse();
	}
	
	public interface Type : ClassContent
	{
		IUsing Usings{
			get;
		}
		ModifierEnum Modifiers {
			get;
		}
		CompilationUnit Unit{
			get;
		}
	}
	
	public interface MemberParent : Type
	{
		Property.GetSet Accessors();
	}
	
}

