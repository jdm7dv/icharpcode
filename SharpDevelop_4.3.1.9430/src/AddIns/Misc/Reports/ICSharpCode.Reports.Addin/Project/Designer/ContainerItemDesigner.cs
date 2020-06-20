﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of RowItemDesigner.
	/// </summary>
	public class ContainerItemDesigner:ParentControlDesigner
	{
		
		private ISelectionService selectionService;
		private IComponentChangeService componentChangeService;
		
		public ContainerItemDesigner()
		{
		}
		
		public override void Initialize(IComponent component)
		{
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			GetService ();	
		}
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties)
		{
			DesignerHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
		}
		
		
		protected override void OnDragDrop(DragEventArgs de)
		{
			base.OnDragDrop(de);
			IToolboxService it = (IToolboxService)this.GetService(typeof(IToolboxService));
			it.SetSelectedToolboxItem(null);
		}
		
		
		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate( );
		}
		
	
		private void OnComponentRename(object sender,ComponentRenameEventArgs e) {
			if (e.Component == this.Component) {
				Control.Name = e.NewName;
				Control.Invalidate();
			}
		}
		
		private void GetService ()
		{
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
			
			componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (componentChangeService != null)
			{
				componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
		}
		
		
		#region Dispose
		protected override void Dispose(bool disposing)
		{
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			
			
			if (componentChangeService != null) {
				componentChangeService.ComponentRename -= OnComponentRename;
			}
			base.Dispose(disposing);
		}
		#endregion
	}
}
