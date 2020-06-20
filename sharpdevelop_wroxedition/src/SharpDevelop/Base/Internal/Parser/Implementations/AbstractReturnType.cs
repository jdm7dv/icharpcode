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
	public abstract class AbstractReturnType : IReturnType
	{
		protected string fullyQualifiedName;
		protected int    pointerNestingLevel;
		protected int[]  arrayDimensions;
		
		public virtual string FullyQualifiedName {
			get {
				return fullyQualifiedName;
			}
		}
		
		public virtual string Name {
			get {
				string[] name = fullyQualifiedName.Split(new char[] {'.'});
				return name[name.Length - 1];
			}
		}
		
		public virtual string Namespace {
			get {
				int index = fullyQualifiedName.LastIndexOf('.');
				return index < 0 ? String.Empty : fullyQualifiedName.Substring(0, index);
			}
		}
		
		public virtual int PointerNestingLevel {
			get {
				return pointerNestingLevel;
			}
		}
		
		public int ArrayCount {
			get {
				return ArrayDimensions.Length;
			}
		}
		
		public virtual int[] ArrayDimensions {
			get {
				return arrayDimensions;
			}
		}
		
	}
}
