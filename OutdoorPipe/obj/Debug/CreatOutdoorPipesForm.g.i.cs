﻿#pragma checksum "..\..\CreatOutdoorPipesForm.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A73013BD091C6B5C62B5B39F6A0794ACCE0F3AE2"
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
    public partial class CreatOutdoorPipesForm : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 8 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FFETOOLS.CreatOutdoorPipesForm @this;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ProfessionCmb;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PipeQuantityTxt;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\CreatOutdoorPipesForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid PipeSettingGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/OutdoorPipe;component/creatoutdoorpipesform.xaml", System.UriKind.Relative);
            
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
            this.@this = ((FFETOOLS.CreatOutdoorPipesForm)(target));
            
            #line 8 "..\..\CreatOutdoorPipesForm.xaml"
            this.@this.Loaded += new System.Windows.RoutedEventHandler(this.this_Loaded);
            
            #line default
            #line hidden
            
            #line 8 "..\..\CreatOutdoorPipesForm.xaml"
            this.@this.KeyDown += new System.Windows.Input.KeyEventHandler(this.this_KeyDown);
            
            #line default
            #line hidden
            
            #line 8 "..\..\CreatOutdoorPipesForm.xaml"
            this.@this.Closing += new System.ComponentModel.CancelEventHandler(this.this_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ProfessionCmb = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.PipeQuantityTxt = ((System.Windows.Controls.TextBox)(target));
            
            #line 47 "..\..\CreatOutdoorPipesForm.xaml"
            this.PipeQuantityTxt.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.PipeQuantityTxt_TextChanged);
            
            #line default
            #line hidden
            
            #line 47 "..\..\CreatOutdoorPipesForm.xaml"
            this.PipeQuantityTxt.LostFocus += new System.Windows.RoutedEventHandler(this.PipeQuantityTxt_LostFocus);
            
            #line default
            #line hidden
            
            #line 47 "..\..\CreatOutdoorPipesForm.xaml"
            this.PipeQuantityTxt.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.PipeQuantityTxt_PreviewMouseDown);
            
            #line default
            #line hidden
            
            #line 47 "..\..\CreatOutdoorPipesForm.xaml"
            this.PipeQuantityTxt.GotFocus += new System.Windows.RoutedEventHandler(this.PipeQuantityTxt_GotFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            this.PipeSettingGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 56 "..\..\CreatOutdoorPipesForm.xaml"
            this.PipeSettingGrid.CurrentCellChanged += new System.EventHandler<System.EventArgs>(this.PipeSettingGrid_CurrentCellChanged);
            
            #line default
            #line hidden
            
            #line 56 "..\..\CreatOutdoorPipesForm.xaml"
            this.PipeSettingGrid.PreparingCellForEdit += new System.EventHandler<System.Windows.Controls.DataGridPreparingCellForEditEventArgs>(this.PipeSettingGrid_PreparingCellForEdit);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 138 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OK_Button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 148 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Cancel_Button_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 158 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Caculate_Button_Click);
            
            #line default
            #line hidden
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
            case 5:
            
            #line 92 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.ComboBox)(target)).DropDownClosed += new System.EventHandler(this.PipeSystemDropDownClosed);
            
            #line default
            #line hidden
            
            #line 92 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.ComboBox)(target)).Loaded += new System.Windows.RoutedEventHandler(this.PipeSystemLoaded);
            
            #line default
            #line hidden
            break;
            case 6:
            
            #line 108 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.ComboBox)(target)).DropDownClosed += new System.EventHandler(this.PipeTypeDropDownClosed);
            
            #line default
            #line hidden
            
            #line 108 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.ComboBox)(target)).Loaded += new System.Windows.RoutedEventHandler(this.PipeTypeLoaded);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 124 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.ComboBox)(target)).DropDownClosed += new System.EventHandler(this.PipeSizeDropDownClosed);
            
            #line default
            #line hidden
            
            #line 124 "..\..\CreatOutdoorPipesForm.xaml"
            ((System.Windows.Controls.ComboBox)(target)).Loaded += new System.Windows.RoutedEventHandler(this.PipeSizeLoaded);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

