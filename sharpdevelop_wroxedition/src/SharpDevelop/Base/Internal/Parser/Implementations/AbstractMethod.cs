// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;

namespace SharpDevelop.Internal.Parser
{
	
	[Serializable]
	public abstract class AbstractMethod : AbstractDecoration, IMethod
	{
		protected string           fullyQualifiedName;
		protected IRegion          region;
		
		protected IReturnType returnType;
		protected ParameterCollection parameters = new ParameterCollection();
		
		public virtual string FullyQualifiedName {
			get {
				return fullyQualifiedName;
			}
			set {
				fullyQualifiedName = value;
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
		
		public virtual IReturnType ReturnType {
			get {
				return returnType;
			}
			set {
				returnType = value;
			}
		}
		
		public virtual ParameterCollection Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value;
			}
		}
		
		public bool IsConstructor {
			get {
				return returnType == null || Name == "#ctor";
			}
		}
		
		public override string ToString()
		{
			return String.Format("[AbstractMethod: FullyQualifiedName={0}, ReturnType = {1}, IsConstructor={2}]",
			                     FullyQualifiedName,
			                     ReturnType,
			                     IsConstructor);
		}
	}
}
