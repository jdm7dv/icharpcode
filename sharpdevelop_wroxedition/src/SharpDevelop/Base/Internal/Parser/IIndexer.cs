// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace SharpDevelop.Internal.Parser
{
	public interface IIndexer: IDecoration
	{
		string FullyQualifiedName {
			get;
		}
		
		string Name {
			get;
		}
		
		string Namespace {
			get;
		}
		
		IRegion Region {
			get;
		}
		
		IRegion GetterRegion {
			get;
		}
		
		IRegion SetterRegion {
			get;
		}
		
		IReturnType ReturnType {
			get;
		}
		
		ParameterCollection Parameters {
			get;
		}
	}
}
