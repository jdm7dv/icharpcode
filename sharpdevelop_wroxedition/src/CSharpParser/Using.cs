// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace SharpDevelop.Internal.Parser {

	public class Using : AbstractUsing
	{
		public Using(IRegion region) 
		{
			this.region = region;
		}
	}
}
