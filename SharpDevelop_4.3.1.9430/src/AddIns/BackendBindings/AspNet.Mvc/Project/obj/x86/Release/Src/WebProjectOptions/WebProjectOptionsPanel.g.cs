﻿#pragma checksum "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2ED20733492E4CD89A656803FEA42DB106ADBA08"
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
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Widgets;
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


namespace ICSharpCode.AspNet.Mvc {
    
    
    /// <summary>
    /// WebProjectOptionsPanel
    /// </summary>
    public partial class WebProjectOptionsPanel : ICSharpCode.SharpDevelop.Gui.OptionPanels.ProjectOptionPanel, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton UseIISExpress;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid IISExpressGroup;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PortTextBox;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton UseLocalIIS;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LocalIISGroup;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ProjectUrl;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock StatusLabel;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreateVirtualDirectoryButton;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ClearWebServerButton;
        
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
            System.Uri resourceLocater = new System.Uri("/AspNet.Mvc;component/src/webprojectoptions/webprojectoptionspanel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
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
            this.UseIISExpress = ((System.Windows.Controls.RadioButton)(target));
            
            #line 18 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
            this.UseIISExpress.Click += new System.Windows.RoutedEventHandler(this.UseIISExpress_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.IISExpressGroup = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.PortTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 30 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
            this.PortTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.PortTextBox_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 31 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
            this.PortTextBox.KeyUp += new System.Windows.Input.KeyEventHandler(this.PortTextBox_KeyUp);
            
            #line default
            #line hidden
            return;
            case 4:
            this.UseLocalIIS = ((System.Windows.Controls.RadioButton)(target));
            
            #line 40 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
            this.UseLocalIIS.Click += new System.Windows.RoutedEventHandler(this.UseLocalIIS_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.LocalIISGroup = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.ProjectUrl = ((System.Windows.Controls.TextBox)(target));
            
            #line 59 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
            this.ProjectUrl.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ProjectUrl_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.StatusLabel = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.CreateVirtualDirectoryButton = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
            this.CreateVirtualDirectoryButton.Click += new System.Windows.RoutedEventHandler(this.CreateVirtualDirectory_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.ClearWebServerButton = ((System.Windows.Controls.Button)(target));
            
            #line 90 "..\..\..\..\..\Src\WebProjectOptions\WebProjectOptionsPanel.xaml"
            this.ClearWebServerButton.Click += new System.Windows.RoutedEventHandler(this.ClearWebServerButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

