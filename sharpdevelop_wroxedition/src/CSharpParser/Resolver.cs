// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;

using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser {
	
	public class Resolver
	{
		IParserService parserService;
		ICompilationUnit cu;
		IClass callingClass;
		
		bool showStatic = false;
		
		int caretLine;
		int caretColumn;
		
		string type;
		string expression;
		
		public ResolveResult Resolve(IParserService parserService, string expression, int caretLineNumber, int caretColumn, string fileName)
		{
//			Console.WriteLine("start resolving!!!!");
			this.parserService = parserService;
			this.caretLine     = caretLineNumber;
			this.caretColumn   = caretColumn;
			IParseInformation parseInformation   = parserService.GetParseInformation(fileName);
			
			if (parseInformation == null || expression == null) {
				return null;
			}
			expression = expression.TrimStart(null);
			if (expression == "") {
				return null;
			}
			
			int i;
			this.expression = expression;
			// expression[expression.Length - 1] != '.'
			// the period that causes this Resove() is not part from expression
			if (expression[expression.Length - 1] == '.') {
				return null;
			}
			for (i = expression.Length - 1; i >= 0; --i) {
				if (!(Char.IsLetterOrDigit(expression[i]) || expression[i] == '_' || expression[i] == '.')) {
					break;
				}
			}
			// no Identifier before the period
			if (i == expression.Length - 1) {
				return null;
			}
			
			type = expression.Substring(i + 1);
			
			ResolveResult res = null;
			cu = parseInformation.DirtyCompilationUnit as ICompilationUnit;
			if (cu != null) {
				callingClass = GetInnermostClass();
				res = Resolve();
				if (res != null && (res.Members != null && res.Members.Count > 0 || res.Namespaces != null && res.Namespaces.Count > 0)) {
					return res;
				}
			}
			cu = parseInformation.ValidCompilationUnit as ICompilationUnit;
			if (cu != null) {
				callingClass = GetInnermostClass();
				res = Resolve();
			}
			return res;
		}
		
		ResolveResult Resolve()
		{
			System.Diagnostics.Debug.Assert(expression != null);
			System.Diagnostics.Debug.Assert(expression != "");
			
			if (expression.StartsWith("using ")) {
//				Console.WriteLine("in Using Statement");
				string[] namespaces = parserService.GetNamespaceList(type);
				if (namespaces == null || namespaces.Length <= 0) {
					return null;
				}
				return new ResolveResult(namespaces);
			}
			
			IClass c = StepByStepLookup(type, callingClass);
			if (c == null) {
//				Console.WriteLine("no class found");
				// Neither static nor dynamic class found, try if type is namespace
				ArrayList nsContents = parserService.GetNamespaceContents(SearchNamespace(type, cu));
				if (nsContents != null) {
					// Put the classes in another ArrayList (classes) 
					// and remove them from nsContents so that 
					// there are only the namespaces left
					ArrayList classes    = new ArrayList();
					for (int j = 0; j < nsContents.Count; ++j) {
						if (nsContents[j] is IClass) {
							if (!((IClass)nsContents[j]).IsProtected || ((IClass)nsContents[j]).IsProtectedOrInternal) {
								classes.Add(nsContents[j]);
							}
							nsContents.RemoveAt(j);
							--j;
						}
					}
					return new ResolveResult((string[])nsContents.ToArray(typeof(string)), classes);
				}
				return null;
			}
			Console.WriteLine("class found " + c.Name + ", static = " + showStatic);
			IClass returnClass = SearchType(c.FullyQualifiedName, cu);
			if (returnClass == null) {
				returnClass = c;
			}
			
			return new ResolveResult(returnClass, ListMembers(new ArrayList(), returnClass));
		}
		
		ArrayList ListMembers(ArrayList members, IClass curType)
		{
			if (showStatic) {
				foreach (IClass c in curType.InnerClasses) {
					if (IsAccessible(curType, c)) {
						members.Add(c);
					}
				}
			}
			foreach (IProperty p in curType.Properties) {
				if (MustBeShowen(curType, p)) {
					members.Add(p);
				}
			}
			foreach (IMethod m in curType.Methods) {
				if (MustBeShowen(curType, m)) {
					members.Add(m);
				}
			}
			foreach (IEvent e in curType.Events) {
				if (MustBeShowen(curType, e)) {
					members.Add(e);
				}
			}
			foreach (IField f in curType.Fields) {
				if (MustBeShowen(curType, f)) {
					members.Add(f);
				}
			}
			if (curType.ClassType == ClassType.Class && !showStatic) {
				IClass baseClass = BaseClass(curType);
				if (baseClass != null) {
					ListMembers(members, baseClass);
				}
			} else if (curType.ClassType == ClassType.Interface && !showStatic) {
				foreach (string s in curType.BaseTypes) {
					IClass baseClass = SearchType(s, curType.CompilationUnit);
					if (baseClass != null && baseClass.ClassType == ClassType.Interface) {
						ListMembers(members, baseClass);
					}
				}
			}
			return members;
		}
		
		IClass BaseClass(IClass curClass)
		{
			foreach (string s in curClass.BaseTypes) {
				IClass baseClass = SearchType(s, curClass.CompilationUnit);
				if (baseClass != null && baseClass.ClassType == ClassType.Class) {
					return baseClass;
				}
			}
			return null;
		}
		
		IClass StepByStepLookup(string name, IClass curClass)
		{
			if (curClass == null) {
				return null;
			}
			showStatic = false;
//			Console.WriteLine("step by step started");
			string[] typename = name.Split('.');
			string current = SearchNamespace(typename[0], curClass.CompilationUnit);
			int i = 1;
			IClass curType = null;
			if (current != null) {
//				Console.WriteLine("namespace found : " + current);
				for ( ; i < typename.Length; ++i) {
					current += "." + typename[i];
					if (!parserService.NamespaceExists(current)) {
						break;
					}
				}
				++i;
				curType = SearchType(current, cu);
				showStatic = true;
//				if (curType != null) {
//					Console.WriteLine("Class found : " + current);
//				} else {
//					Console.WriteLine("Class not found : " + current);
//				}
			} else if (typename[0] == "this") {
//				Console.WriteLine("this found");
				if (InStatic()) {
					return null;
				}
				curType = curClass;
			} else if (typename[0] == "base") {
				if (InStatic()) {
					return null;
				}
				foreach (string baseClass in curClass.BaseTypes) {
					IClass c = SearchType(baseClass, curClass.CompilationUnit);
					if (c != null && c.ClassType == ClassType.Class) {
						curType = c;
						break;
					}
				}
			} else {
				current = typename[0];
				curType = DynamicLookup(current);
				if (curType == null) {
					curType = SearchType(current, curClass.CompilationUnit);
					showStatic = true;
				}
			}
			if (curType == null) {
//				Console.WriteLine("No Type found!");
				return null;
			}
//			Console.WriteLine("Type found " + curType.Name);
			for ( ; i < typename.Length; ++i) {
				curType = SearchMember(curType, typename[i]);
				if (curType == null) {
//					Console.WriteLine("member not found : " + typename[i]);
					return null;
				} else {
//					Console.WriteLine("member found : " + typename[i]);
				}
			}
			return curType;			
		}
		
		bool InStatic()
		{
			IProperty property = GetProperty();
			if (property != null) {
				return property.IsStatic;
			}
			IMethod method = GetMethod();
			if (method != null) {
				return method.IsStatic;
			}
			return false;
		}
		
		bool IsAccessible(IClass c, IDecoration member)
		{
			if ((member.Modifiers & ModifierEnum.Public) == ModifierEnum.Public) {
				return true;
			}
			if ((member.Modifiers & ModifierEnum.Protected) == ModifierEnum.Protected && IsClassInInheritanceTree(c, callingClass)) {
				return true;
			}
			return c.FullyQualifiedName == callingClass.FullyQualifiedName;
		}
		
		bool MustBeShowen(IClass c, IDecoration member)
		{
			if ((!showStatic &&  ((member.Modifiers & ModifierEnum.Static) == ModifierEnum.Static)) ||
			    ( showStatic && !((member.Modifiers & ModifierEnum.Static) == ModifierEnum.Static))) {
				return false;
			}
			return IsAccessible(c, member);
		}
		
		IClass SearchMember(IClass curType, string memberName)
		{
			if (showStatic) {
				foreach (IClass c in curType.InnerClasses) {
					if (c.Name == memberName && IsAccessible(curType, c)) {
						return c;
					}
				}
			}
//			Console.WriteLine("#Properties " + curType.Properties.Count);
			foreach (IProperty p in curType.Properties) {
//				Console.WriteLine("checke Property " + p.Name);
//				Console.WriteLine("member name " + memberName);
				if (p.Name == memberName && MustBeShowen(curType, p)) {
//					Console.WriteLine("Property found " + p.Name);
					showStatic = false;
					if (p.ReturnType.ArrayDimensions != null && p.ReturnType.ArrayDimensions.Length > 0) {
						return parserService.GetClass("System.Array");
					}
					return SearchType(p.ReturnType.FullyQualifiedName, curType.CompilationUnit);
				}
			}
			foreach (IField f in curType.Fields) {
//				Console.WriteLine("checke Feld " + f.Name);
//				Console.WriteLine("member name " + memberName);
				if (f.Name == memberName && MustBeShowen(curType, f)) {
//					Console.WriteLine("Field found " + f.Name);
					showStatic = false;
					
					// HACK: Enums may have no return type :/ (boese andrea)
					if (f.ReturnType == null) {
						return parserService.GetClass("System.Enum");
					}
					
					if (f.ReturnType.ArrayDimensions != null && f.ReturnType.ArrayDimensions.Length > 0) {
						return parserService.GetClass("System.Array");
					}
					return SearchType(f.ReturnType.FullyQualifiedName, curType.CompilationUnit);
				}
			}
//			foreach (IIndexer i in curType.Indexer) {
//				if (i.Name == memberName && MustBeShowen(curType, i)) {
//					showStatic = false;
//					if (i.ReturnType.ArrayDimensions != null && i.ReturnType.ArrayDimensions.Length > 0) {
//						return parserService.GetClass("System.Array");
//					}
//					return SearchType(i.ReturnType.FullyQualifiedName, curType.CompilationUnit);
//				}
//			}
//			foreach (IMethod m in curType.Methods) {
//				if (m.Name == memberName && MustBeShowen(curType, m)) {
//					showStatic = false;
//					if (m.ReturnType.ArrayDimensions != null && m.ReturnType.ArrayDimensions.Length > 0) {
//						return parserService.GetClass("System.Array");
//					}
//					return SearchType(m.ReturnType.FullyQualifiedName, curType.CompilationUnit);
//				}
//			}
//			foreach (IEvent e in curType.Events && MustBeShowen(curType, e)) {
//				if (e.Name == memberName) {
//					showStatic = false;
//					if (e.ReturnType.ArrayDimensions != null && e.ReturnType.ArrayDimensions.Length > 0) {
//						return parserService.GetClass("System.Array");
//					}
//					return SearchType(e.ReturnType.FullyQualifiedName, curType.CompilationUnit);
//				}
//			}
			foreach (string baseType in curType.BaseTypes) {
				IClass c = SearchType(baseType, curType.CompilationUnit);
				if (c != null) {
					IClass erg = SearchMember(c, memberName);
					if (erg != null) {
						return erg;
					}
				}
			}
			return null;
		}
		
		/// <remarks>
		/// does the dynamic lookup for the typeName
		/// </remarks>
		IClass DynamicLookup(string typeName)
		{
//			Console.WriteLine("starting dynamic lookup");
//			Console.WriteLine("name == " + typeName);
			
			// try if it exists a variable named typeName
			foreach (Variable v in cu.LookUpTable) {
//				Console.WriteLine("Variable: " + v.Name + " Region = " + v.Region);
				if (v.Name == typeName && v.Region.IsInside(caretLine, caretColumn)) {
//					Console.WriteLine("variable found");
					if (v.ReturnType.ArrayDimensions != null && v.ReturnType.ArrayDimensions.Length > 0) {
						return parserService.GetClass("System.Array");
					}
					return SearchType(v.ReturnType.FullyQualifiedName, callingClass.CompilationUnit);
				}
			}
			
			if (callingClass == null) {
				return null;
			}
			
			// try if typeName is a method parameter
			IClass found = SearchMethodParameter(typeName);
			if (found != null) {
				return found;
			}
			
			// check if typeName == value in set method of a property
			if (typeName == "value") {
				found = SearchProperty();
				if (found != null) {
					return found;
				}
			}
			
			// try if there exists a nonstatic member named typeName
			showStatic = false;
			found = SearchMember(callingClass, typeName);
			if (found != null) {
				return found;
			}
			
			// try if there exists a static member named typeName
			showStatic = true;
			found = SearchMember(callingClass, typeName);
			if (found != null) {
				return found;
			}
			
			// try if there exists a static member in outer classes named typeName
			ClassCollection classes = GetOuterClasses();
			foreach (IClass c in GetOuterClasses()) {
				found = SearchMember(c, typeName);
				if (found != null) {
					return found;
				}
			}
			
			return null;
		}
		
		IProperty GetProperty()
		{
			foreach (IProperty property in callingClass.Properties) {
				if (property.Region != null && property.Region.IsInside(caretLine, caretColumn)) {
					return property;
				}
			}
			return null;
		}
		
		IMethod GetMethod()
		{
			foreach (IMethod method in callingClass.Methods) {
				if (method.Region != null && method.Region.IsInside(caretLine, caretColumn)) {
					return method;
				}
			}
			return null;
		}
		
		IClass SearchProperty()
		{
			IProperty property = GetProperty();
			if (property == null) {
				return null;
			}
			if (property.SetterRegion != null && property.SetterRegion.IsInside(caretLine, caretColumn)) {
				if (property.ReturnType.ArrayDimensions != null && property.ReturnType.ArrayDimensions.Length > 0) {
					return parserService.GetClass("System.Array");
				}
				return SearchType(property.ReturnType.FullyQualifiedName, cu);
			}
			return null;
		}
		
		IClass SearchMethodParameter(string parameter)
		{
			IMethod method = GetMethod();
			if (method == null) {
				return null;
			}
			foreach (Parameter p in method.Parameters) {
				if (p.Name == parameter) {
					if (p.ReturnType.ArrayDimensions != null && p.ReturnType.ArrayDimensions.Length > 0) {
						return parserService.GetClass("System.Array");
					}
					return SearchType(p.ReturnType.FullyQualifiedName, cu);
				}
			}
			return null;
		}
		
		/// <remarks>
		/// use the usings to find the correct name of a namespace
		/// </remarks>
		string SearchNamespace(string name, ICompilationUnit unit)
		{
			if (parserService.NamespaceExists(name)) {
				return name;
			}
			if (unit == null) {
//				Console.WriteLine("done, resultless");
				return null;
			}
			foreach (IUsing u in unit.Usings) {
				if (u.Region.IsInside(caretLine, caretColumn)) {
					string nameSpace = u.SearchNamespace(name);
					if (nameSpace != null) {
						return nameSpace;
					}
				}
			}
//			Console.WriteLine("done, resultless");
			return null;
		}
		
		/// <remarks>
		/// use the usings to find a class
		/// </remarks>
		IClass SearchType(string name, ICompilationUnit unit)
		{
			IClass c;
			c = parserService.GetClass(name);
			if (c != null) {
				return c;
			}
			if (unit == null) {
				return null;
			}
			foreach (IUsing u in unit.Usings) {
				if (u.Region.IsInside(caretLine, caretColumn)) {
					c = u.SearchType(name);
					if (c != null) {
						return c;
					}
				}
			}
			return null;
		}
		
		/// <remarks>
		/// Returns true, if class possibleBaseClass is in the inheritance tree from c
		/// </remarks>
		bool IsClassInInheritanceTree(IClass possibleBaseClass, IClass c)
		{
			if (possibleBaseClass == null || c == null) {
				return false;
			}
			if (possibleBaseClass.FullyQualifiedName == c.FullyQualifiedName) {
				return true;
			}
			foreach (string baseClass in c.BaseTypes) {
				if (IsClassInInheritanceTree(possibleBaseClass, SearchType(baseClass, cu))) {
					return true;
				}
			}
			return false;
		}
		
		/// <remarks>
		/// Returns the innerst class in which the carret currently is, returns null
		/// if the carret is outside any class boundaries.
		/// </remarks>
		IClass GetInnermostClass()
		{
			if (cu != null) {
				foreach (IClass c in cu.Classes) {
					if (c.Region.IsInside(caretLine, caretColumn)) {
						return GetInnermostClass(c);
					}
				}
			}
			return null;
		}
		
		IClass GetInnermostClass(IClass curClass)
		{
			if (curClass == null) {
				return null;
			}
			if (curClass.InnerClasses == null) {
				return curClass;
			}
			foreach (IClass c in curClass.InnerClasses) {
				if (c.Region.IsInside(caretLine, caretColumn)) {
					return GetInnermostClass(c);
				}
			}
			return curClass;
		}
		
		/// <remarks>
		/// Returns all (nestet) classes in which the carret currently is exept
		/// the innermost class, returns an empty collection if the carret is in 
		/// no class or only in the innermost class.
		/// the most outer class is the last in the collection.
		/// </remarks>
		ClassCollection GetOuterClasses()
		{
			ClassCollection classes = new ClassCollection();
			if (cu != null) {
				foreach (IClass c in cu.Classes) {
					if (c.Region.IsInside(caretLine, caretColumn)) {
						if (c != GetInnermostClass()) {
							GetOuterClasses(classes, c);
							classes.Add(c);
						}
						break;
					}
				}
			}
			return classes;
		}
		
		void GetOuterClasses(ClassCollection classes, IClass curClass)
		{
			if (curClass != null) {
				foreach (IClass c in curClass.InnerClasses) {
					if (c.Region.IsInside(caretLine, caretColumn)) {
						if (c != GetInnermostClass()) {
							GetOuterClasses(classes, c);
							classes.Add(c);
						}
						break;
					}
				}
			}
		}
	}
}
