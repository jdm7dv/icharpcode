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
	public abstract class AbstractDecoration : IDecoration
	{
		protected ModifierEnum               modifiers     = ModifierEnum.None;
		protected AttributeSectionCollection attributes    = new AttributeSectionCollection();
		protected string                     documentation = String.Empty;
		
		public virtual ModifierEnum Modifiers {
			get {
				return modifiers;
			}
		}
		
		public virtual AttributeSectionCollection Attributes {
			get {
				return attributes;
			}
		}
		
		public string Documentation {
			get {
				return documentation;
			}
		}
		
		public bool IsAbstract {
			get {
				return (modifiers & ModifierEnum.Abstract) == ModifierEnum.Abstract;
			}
		}
		
		public bool IsSealed {
			get {
				return (modifiers & ModifierEnum.Sealed) == ModifierEnum.Sealed;
			}
		}
				
		public bool IsStatic {
			get {
				return (modifiers & ModifierEnum.Static) == ModifierEnum.Static;
			}
		}
		
		public bool IsVirtual {
			get {
				return (modifiers & ModifierEnum.Virtual) == ModifierEnum.Virtual;
			}
		}
		
		public bool IsPublic {
			get {
				return (modifiers & ModifierEnum.Public) == ModifierEnum.Public;
			}
		}
		
		public bool IsProtected {
			get {
				return (modifiers & ModifierEnum.Protected) == ModifierEnum.Protected;
			}
		}
		
		public bool IsPrivate {
			get {
				return (modifiers & ModifierEnum.Private) == ModifierEnum.Private;
			}
		}
		
		public bool IsInternal {
			get {
				return (modifiers & ModifierEnum.Internal) == ModifierEnum.Internal;
			}
		}
		
		public bool IsProtectedAndInternal {
			get {
				return (modifiers & (ModifierEnum.Internal | ModifierEnum.Protected)) == (ModifierEnum.Internal | ModifierEnum.Protected);
			}
		}
		
		public bool IsProtectedOrInternal {
			get {
				return (modifiers & ModifierEnum.ProtectedOrInternal) == ModifierEnum.ProtectedOrInternal;
			}
		}
		
		public bool IsLiteral {
			get {
				return (modifiers & ModifierEnum.Literal) == ModifierEnum.Literal;
			}
		}
		
		public bool IsReadonly {
			get {
				return (modifiers & ModifierEnum.Readonly) == ModifierEnum.Readonly;
			}
		}

		public bool IsOverride {
			get {
				return (modifiers & ModifierEnum.Override) == ModifierEnum.Override;
			}
		}
		
		public bool IsNew {
			get {
				return (modifiers & ModifierEnum.New) == ModifierEnum.New;
			}
		}
		
	}
}
