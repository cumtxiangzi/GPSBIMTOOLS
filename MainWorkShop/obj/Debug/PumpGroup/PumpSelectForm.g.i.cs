﻿#pragma checksum "..\..\..\PumpGroup\PumpSelectForm.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4D47FC834B5BF128809E923EB769256F040BDD9C2126C3DFA77282EA18A61B06"
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
    /// PumpSelectForm
    /// </summary>
    public partial class PumpSelectForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 47 "..\..\..\PumpGroup\PumpSelectForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Flow;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\PumpGroup\PumpSelectForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Lift;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\PumpGroup\PumpSelectForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox PumpManufacture;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\PumpGroup\PumpSelectForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox PumpModel;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\PumpGroup\PumpSelectForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OkButton;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\PumpGroup\PumpSelectForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelButton;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\PumpGroup\PumpSelectForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid RoomSettingGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/MainWorkShop;component/pumpgroup/pumpselectform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\PumpGroup\PumpSelectForm.xaml"
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
            
            #line 8 "..\..\..\PumpGroup\PumpSelectForm.xaml"
            ((FFETOOLS.PumpSelectForm)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Flow = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.Lift = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.PumpManufacture = ((System.Windows.Controls.ComboBox)(target));
            
            #line 56 "..\..\..\PumpGroup\PumpSelectForm.xaml"
            this.PumpManufacture.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.PumpManufacture_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.PumpModel = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this.OkButton = ((System.Windows.Controls.Button)(target));
            
            #line 63 "..\..\..\PumpGroup\PumpSelectForm.xaml"
            this.OkButton.Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 74 "..\..\..\PumpGroup\PumpSelectForm.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.RoomSettingGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

