﻿#pragma checksum "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9460845566BA2F4B260C90C4E6A616F0D0505FB3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ICSharpCode.Core.Presentation;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ICSharpCode.XamlBinding.PowerToys.Dialogs {
    
    
    /// <summary>
    /// EditGridColumnsAndRowsDialog
    /// </summary>
    public partial class EditGridColumnsAndRowsDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 44 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel buttonPanel;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnOK;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancel;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel dropPanel;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid columnWidthGrid;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid rowHeightGrid;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridDisplay;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ICSharpCode.XamlBinding;component/powertoys/dialogs/editgridcolumnsandrowsdialog" +
                    ".xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            this.buttonPanel = ((System.Windows.Controls.StackPanel)(target));
            
            #line 44 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
            this.buttonPanel.Drop += new System.Windows.DragEventHandler(this.ButtonPanelDrop);
            
            #line default
            #line hidden
            
            #line 44 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
            this.buttonPanel.DragOver += new System.Windows.DragEventHandler(this.ButtonPanelDragOver);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnOK = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
            this.btnOK.Click += new System.Windows.RoutedEventHandler(this.BtnOKClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 47 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
            this.btnCancel.Click += new System.Windows.RoutedEventHandler(this.BtnCancelClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.dropPanel = ((System.Windows.Controls.StackPanel)(target));
            
            #line 50 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
            this.dropPanel.Drop += new System.Windows.DragEventHandler(this.DropPanelDrop);
            
            #line default
            #line hidden
            
            #line 50 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
            this.dropPanel.DragOver += new System.Windows.DragEventHandler(this.DropPanelDragOver);
            
            #line default
            #line hidden
            return;
            case 6:
            this.columnWidthGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.rowHeightGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 8:
            this.gridDisplay = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 37 "..\..\..\..\PowerToys\Dialogs\EditGridColumnsAndRowsDialog.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnDeleteItemClick);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}
