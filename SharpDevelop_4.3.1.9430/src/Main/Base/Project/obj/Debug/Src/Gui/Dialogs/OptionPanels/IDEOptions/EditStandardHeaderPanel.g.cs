﻿#pragma checksum "..\..\..\..\..\..\..\Src\Gui\Dialogs\OptionPanels\IDEOptions\EditStandardHeaderPanel.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "FCC54941DB48564801497180A3D802AF193A8533"
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
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace ICSharpCode.SharpDevelop.Gui.OptionPanels {
    
    
    /// <summary>
    /// EditStandardHeaderPanel
    /// </summary>
    public partial class EditStandardHeaderPanel : ICSharpCode.SharpDevelop.Gui.OptionPanel, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\..\..\..\..\..\Src\Gui\Dialogs\OptionPanels\IDEOptions\EditStandardHeaderPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox headerChooser;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\..\..\..\Src\Gui\Dialogs\OptionPanels\IDEOptions\EditStandardHeaderPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox headerTextBox;
        
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
            System.Uri resourceLocater = new System.Uri("/ICSharpCode.SharpDevelop;component/src/gui/dialogs/optionpanels/ideoptions/edits" +
                    "tandardheaderpanel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\..\Src\Gui\Dialogs\OptionPanels\IDEOptions\EditStandardHeaderPanel.xaml"
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
            this.headerChooser = ((System.Windows.Controls.ComboBox)(target));
            
            #line 13 "..\..\..\..\..\..\..\Src\Gui\Dialogs\OptionPanels\IDEOptions\EditStandardHeaderPanel.xaml"
            this.headerChooser.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.HeaderChooser_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.headerTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 21 "..\..\..\..\..\..\..\Src\Gui\Dialogs\OptionPanels\IDEOptions\EditStandardHeaderPanel.xaml"
            this.headerTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.HeaderTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

