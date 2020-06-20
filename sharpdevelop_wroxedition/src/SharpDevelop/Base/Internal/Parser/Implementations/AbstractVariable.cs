// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public abstract class AbstractVariable : IVariable
	{
		protected string name;
		protected IRegion region;
		protected IReturnType returnType;
		
		public virtual string Name {
			get {
				return name;
			}
		}
		
		public virtual IRegion Region {
			get {
				return region;
			}
		}
		
		public virtual IReturnType ReturnType {
			get {
				return returnType;
			}
			set {
				returnType = value;
			}
		}
	}
}
