/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project.Samples.NestedProject
{
	/// <summary>
	/// Represent the methods for creating projects within the solution.
	/// </summary>
	[GuidAttribute(GuidStrings.GuidNestedProjectFactory)]
	public class NestedProjectFactory : ProjectFactory
	{
		#region Constructors
		/// <summary>
		/// Explicit default constructor.
		/// </summary>
		/// <param name="package">Value of the project package for initialize internal package field.</param>
		public NestedProjectFactory(NestedProjectPackage package)
			: base(package)
		{
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Creates a new project by cloning an existing template project.
		/// </summary>
		/// <returns></returns>
		protected override ProjectNode CreateProject()
		{
			NestedProjectNode project = new NestedProjectNode();
			project.SetSite((IOleServiceProvider)((IServiceProvider)this.Package).GetService(typeof(IOleServiceProvider)));
			return project;
		}
		#endregion Methods
	}
}
