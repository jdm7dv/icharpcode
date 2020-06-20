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
	public abstract class AbstractIndexer : AbstractDecoration, IIndexer
	{
		protected string              fullyQualifiedName;
		protected IRegion             region;
		protected IRegion             getterRegion;
		protected IRegion             setterRegion;
		protected IReturnType         returnType;
		protected ParameterCollection parameters = new ParameterCollection();
		
		public virtual string FullyQualifiedName {
			get {
				return fullyQualifiedName;
			}
		}
		
		public virtual string Name {
			get {
				string[] name = fullyQualifiedName.Split(new char[] {'.'});
				return name[name.Length -1];
			}
		}
		
		public virtual string Namespace {
			get {
				int index = fullyQualifiedName.LastIndexOf('.');
				return index < 0 ? String.Empty : fullyQualifiedName.Substring(0, index);
			}
		}
		
		public virtual IRegion Region {
			get {
				return region;
			}
		}
		
		public IRegion GetterRegion {
			get {
				return getterRegion;
			}
		}
		
		public IRegion SetterRegion {
			get {
				return setterRegion;
			}
		}
		
		public virtual IReturnType ReturnType {
			get {
				return returnType;
			}
		}
		
		public virtual ParameterCollection Parameters {
			get {
				return parameters;
			}
		}
	}
}
