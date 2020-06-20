// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections;

namespace SharpDevelop.Internal.Parser {
	
	public class Variable : AbstractVariable
	{
		public Variable(string name, IRegion region)
		{
			this.name = name;
			this.region = region;
		}
		
	}
}

