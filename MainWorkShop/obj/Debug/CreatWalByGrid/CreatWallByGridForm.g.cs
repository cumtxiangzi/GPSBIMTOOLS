﻿#pragma checksum "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "75ADC9CCD8CF3DCDBF2551E8C69B02F702C25ADAECB1CE2A189975E225202E2E"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using FFETOOLS;
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


namespace FFETOOLS {
    
    
    /// <summary>
    /// CreatWallByGridForm
    /// </summary>
    public partial class CreatWallByGridForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 36 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TopElevation;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox BottomElevation;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CreatRoof;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CreatGround;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CreatSlab;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OkButton;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelButton;
        
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
            System.Uri resourceLocater = new System.Uri("/MainWorkShop;component/creatwalbygrid/creatwallbygridform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
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
            
            #line 8 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
            ((FFETOOLS.CreatWallByGridForm)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
            ((FFETOOLS.CreatWallByGridForm)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TopElevation = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.BottomElevation = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.CreatRoof = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.CreatGround = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.CreatSlab = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 7:
            this.OkButton = ((System.Windows.Controls.Button)(target));
            
            #line 63 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
            this.OkButton.Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 74 "..\..\..\CreatWalByGrid\CreatWallByGridForm.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
