﻿#pragma checksum "..\..\CreatOutdoorPipesForm.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "67DA8E48A3DB81CD994676E5EE640F8DCD049B19"
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
    /// CreatOutdoorPipesForm
    /// </summary>
    public partial class CreatOutdoorPipesForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox DrawingTypeCombo;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DrawingNumber;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox MajorCombo;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton CH_Button;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton EN_Button;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton CH_EN_Button;
        
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
            System.Uri resourceLocater = new System.Uri("/BatchTools;component/creatoutdoorpipesform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CreatOutdoorPipesForm.xaml"
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
            
            #line 8 "..\..\CreatOutdoorPipesForm.xaml"
            ((FFETOOLS.CreatOutdoorPipesForm)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.DrawingTypeCombo = ((System.Windows.Controls.ComboBox)(target));
            
            #line 10 "..\..\CreatOutdoorPipesForm.xaml"
            this.DrawingTypeCombo.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DrawingTypeCombo_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.DrawingNumber = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            
            #line 14 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MajorCombo = ((System.Windows.Controls.ComboBox)(target));
            
            #line 16 "..\..\CreatOutdoorPipesForm.xaml"
            this.MajorCombo.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.MajorCombo_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.CH_Button = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 7:
            this.EN_Button = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 8:
            this.CH_EN_Button = ((System.Windows.Controls.RadioButton)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

