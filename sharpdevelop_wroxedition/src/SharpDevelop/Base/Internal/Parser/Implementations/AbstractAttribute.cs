// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public class AbstractAttributeSection : IAttributeSection
	{
		protected AttributeTarget     attributeTarget;
		protected AttributeCollection attributes = new AttributeCollection();
		
		public virtual AttributeTarget AttributeTarget {
			get {
				return attributeTarget;
			}
			set {
				attributeTarget = value;
			}
		}
		
		public virtual AttributeCollection Attributes {
			get {
				return attributes;
			}
		}
	}
	
	public abstract class AbstractAttribute : IAttribute
	{
		protected string name;
		protected ArrayList positionalArguments = new ArrayList();
		protected SortedList namedArguments = new SortedList();
		
		public virtual string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		public virtual ArrayList PositionalArguments { // [expression]
			get {
				return positionalArguments;
			}
			set {
				positionalArguments = value;
			}
		}
		public virtual SortedList NamedArguments { // string/expression
			get {
				return namedArguments;
			}
			set {
				namedArguments = value;
			}
		}
	}
}
