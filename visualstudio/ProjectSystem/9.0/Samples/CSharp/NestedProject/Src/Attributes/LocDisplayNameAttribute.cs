/// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Project.Samples.NestedProject
{
	/// <summary>
	/// Specifies the display name for a property, event, 
	/// or public void method which takes no arguments.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	internal sealed class LocDisplayNameAttribute : DisplayNameAttribute
	{
		#region Fields
		private string name;
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="name">Attribute display name.</param>
		public LocDisplayNameAttribute(string name)
		{
			this.name = name;
		}
		#endregion Constructors

		#region Properties
		/// <summary>
		/// Gets attribute display name.
		/// </summary>
		public override string DisplayName
		{
			get
			{
				string result = Resources.GetString(this.name);

				if(result == null)
				{
					// Debug.Assert(false, "String resource '" + this.name + "' is missing");
					result = this.name;
				}
				return result;
			}
		}
		#endregion Properties
	}
}