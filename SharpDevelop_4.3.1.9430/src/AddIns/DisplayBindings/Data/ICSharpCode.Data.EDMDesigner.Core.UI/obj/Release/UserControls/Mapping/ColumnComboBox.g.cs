﻿#pragma checksum "..\..\..\..\UserControls\Mapping\ColumnComboBox.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CE61EAD511B3CCA44F5303ABFAFAF4F8A1655A61"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping;
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


namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping {
    
    
    /// <summary>
    /// ColumnComboBox
    /// </summary>
    public partial class ColumnComboBox : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\UserControls\Mapping\ColumnComboBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.ComboBoxEditableWhenFocused columnComboBox;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\UserControls\Mapping\ColumnComboBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Data.CollectionContainer columnNullValues;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\..\UserControls\Mapping\ColumnComboBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Data.CollectionContainer columns;
        
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
            System.Uri resourceLocater = new System.Uri("/ICSharpCode.Data.EDMDesigner.Core.UI;component/usercontrols/mapping/columncombob" +
                    "ox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControls\Mapping\ColumnComboBox.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            case 1:
            this.columnComboBox = ((ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.ComboBoxEditableWhenFocused)(target));
            return;
            case 2:
            this.columnNullValues = ((System.Windows.Data.CollectionContainer)(target));
            return;
            case 3:
            this.columns = ((System.Windows.Data.CollectionContainer)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

