﻿#pragma checksum "..\..\..\Src\XmlSchemasPanel.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7D7E11D983AC2B16EB7EC38EF0DF64EA46F1600C"
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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.XmlEditor;
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


namespace ICSharpCode.XmlEditor {
    
    
    /// <summary>
    /// XmlSchemasPanel
    /// </summary>
    public partial class XmlSchemasPanel : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\Src\XmlSchemasPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button addSchemaButton;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Src\XmlSchemasPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button removeSchemaButton;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Src\XmlSchemasPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox schemaListBox;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Src\XmlSchemasPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox fileExtensionComboBox;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\Src\XmlSchemasPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button changeSchemaButton;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Src\XmlSchemasPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox schemaNamespaceTextBox;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Src\XmlSchemasPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox namespacePrefixTextBox;
        
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
            System.Uri resourceLocater = new System.Uri("/XmlEditor;component/src/xmlschemaspanel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Src\XmlSchemasPanel.xaml"
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
            case 1:
            this.addSchemaButton = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\..\Src\XmlSchemasPanel.xaml"
            this.addSchemaButton.Click += new System.Windows.RoutedEventHandler(this.AddSchemaButtonClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.removeSchemaButton = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\..\Src\XmlSchemasPanel.xaml"
            this.removeSchemaButton.Click += new System.Windows.RoutedEventHandler(this.RemoveSchemaButtonClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.schemaListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 22 "..\..\..\Src\XmlSchemasPanel.xaml"
            this.schemaListBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.SchemaListBoxSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.fileExtensionComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 39 "..\..\..\Src\XmlSchemasPanel.xaml"
            this.fileExtensionComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.FileExtensionComboBoxSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.changeSchemaButton = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\..\Src\XmlSchemasPanel.xaml"
            this.changeSchemaButton.Click += new System.Windows.RoutedEventHandler(this.ChangeSchemaButtonClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.schemaNamespaceTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.namespacePrefixTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 44 "..\..\..\Src\XmlSchemasPanel.xaml"
            this.namespacePrefixTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.NamespacePrefixTextBoxTextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

