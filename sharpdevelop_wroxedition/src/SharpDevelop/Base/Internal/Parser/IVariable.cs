// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace SharpDevelop.Internal.Parser
{
	public interface IVariable
	{
		string Name {
			get;
		}
		
		IRegion Region {
			get;
		}
		
		IReturnType ReturnType {
			get;
		}
	}
}
