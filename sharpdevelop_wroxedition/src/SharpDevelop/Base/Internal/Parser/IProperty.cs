// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System.Reflection;

namespace SharpDevelop.Internal.Parser
{
	public interface IProperty : IDecoration
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
	}
}
