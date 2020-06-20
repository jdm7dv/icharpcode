﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of the pad content
	/// </summary>
	internal sealed  class ReportExplorerPad : AbstractPadContent,INotifyPropertyChanged
	{
		private static int viewCount;
		private ExplorerTree explorerTree;
		private static ReportExplorerPad instance;
		private ReportModel reportModel;
		/// <summary>
		/// Creates a new ReportExplorer object
		/// </summary>
		
		
		public ReportExplorerPad():base()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			WorkbenchSingleton.Workbench.ViewClosed += ActiveViewClosed;
			this.explorerTree = new ExplorerTree();
			this.explorerTree.MouseDown += new MouseEventHandler(ReportExplorer_MouseDown);
			this.explorerTree.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportExplorerPad_PropertyChanged);
			instance = this;
		}

		
		void ReportExplorerPad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.NotifyReportView(e.PropertyName);
		}
		
		#region Setup
		
		
		
		public void AddContent (ReportModel reportModel)
		{
			
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			this.reportModel = reportModel;
			this.explorerTree.ReportModel = this.reportModel;
			ViewCount++;
		}
	
		#endregion
		
		
		void ActiveViewContentChanged(object source, EventArgs e)
		{
			ReportDesignerView vv = WorkbenchSingleton.Workbench.ActiveViewContent as ReportDesignerView;
			if (vv != null) {
				Console.WriteLine("Explorerpad:ActiveViewContentChanged {0}",vv.TitleName);
			}
		}
		
		private void ActiveViewClosed (object source, ViewContentEventArgs e)
		{
			if (e.Content is ReportDesignerView) {
				Console.WriteLine ("Designer closed");
				                   ViewCount --;
			}
		}
		
		
		#region Mouse
		
		private void ReportExplorer_MouseDown (object sender, MouseEventArgs e)
		{
			AbstractFieldsNode abstrNode =  this.explorerTree.GetNodeAt(e.X, e.Y) as AbstractFieldsNode;
			if (e.Button == MouseButtons.Right) {
				this.explorerTree.SelectedNode = abstrNode;
				if (abstrNode != null) {
					if (abstrNode.ContextMenuAddinTreePath.Length > 0) {
						ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,abstrNode.ContextMenuAddinTreePath);
						ctMen.Show (this.explorerTree, new Point (e.X,e.Y));
					}
				}
			}
		}
		
		#endregion
		
		
		#region publics for Commands
	
		// These public methods are all called from ExplorerCommands
		
		public void ClearNodes () 
		{
			this.explorerTree.ClearSection();
		}
		
		
		public void ToggleOrder ()
		{
			this.explorerTree.ToggleSortOrder();
		}
		
		
		public void RemoveSortNode()
		{
			this.explorerTree.RemoveSortNode();
		}
		
		
		public void RemoveGroupNode()
		{
			this.explorerTree.RemoveGroupNode();
		}
		
		public void RefreshParameters()
		{
			this.explorerTree.BuildTree();
			this.NotifyReportView("Parameters");
		}
		
		#endregion
		
		public static ReportExplorerPad Instance {
			get { return instance; }
		}
		
		
		public static int ViewCount {
			get { return viewCount; }
			set {
				viewCount = value;
				if (viewCount == 0)	{
					Console.WriteLine("Should find a way to close/hide a pad");
				}
			}
		}
		
		
		
		public ReportModel ReportModel 
		{get {return this.reportModel;}}
		

		#region IPropertyChanged
		
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		private  void NotifyReportView(string property)
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this,new System.ComponentModel.PropertyChangedEventArgs(property));                     
			}
		}
		
		#endregion
		
		#region AbstractPadContent
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override object Control 
		{
			get {
				return this.explorerTree;
			}
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
			this.explorerTree.Dispose();
		}
		
		#endregion
	}
}
