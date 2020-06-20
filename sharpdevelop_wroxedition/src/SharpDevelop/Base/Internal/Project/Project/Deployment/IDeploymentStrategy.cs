// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public interface IDeploymentStrategy
	{
		void DeployProject(IProject project);
	}
	
}
