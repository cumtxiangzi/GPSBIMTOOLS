﻿#pragma checksum "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "57F5F057C86835F96BCFCC34196676FC60B2F2DA5F27CBC38076A3FE6BBC7CB6"
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
    /// ConstructionPlanForm
    /// </summary>
    public partial class ConstructionPlanForm : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 8 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FFETOOLS.ConstructionPlanForm @this;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView ConstructionPlanTreeView;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddButton;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RemoveButton;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView ConstructionDrawingTreeView;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OkButton;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
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
            System.Uri resourceLocater = new System.Uri("/DrawingTools;component/creatconstructionplan/constructionplan.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
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
            this.@this = ((FFETOOLS.ConstructionPlanForm)(target));
            
            #line 8 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
            this.@this.Loaded += new System.Windows.RoutedEventHandler(this.this_Loaded);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
            this.@this.KeyDown += new System.Windows.Input.KeyEventHandler(this.this_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ConstructionPlanTreeView = ((System.Windows.Controls.TreeView)(target));
            return;
            case 4:
            this.AddButton = ((System.Windows.Controls.Button)(target));
            
            #line 48 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
            this.AddButton.Click += new System.Windows.RoutedEventHandler(this.AddButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.RemoveButton = ((System.Windows.Controls.Button)(target));
            
            #line 59 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
            this.RemoveButton.Click += new System.Windows.RoutedEventHandler(this.RemoveButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.ConstructionDrawingTreeView = ((System.Windows.Controls.TreeView)(target));
            return;
            case 8:
            this.OkButton = ((System.Windows.Controls.Button)(target));
            
            #line 100 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
            this.OkButton.Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 111 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
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
            case 3:
            
            #line 30 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Click += new System.Windows.RoutedEventHandler(this.PlanCheckBox_Click);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 84 "..\..\..\CreatConstructionPlan\ConstructionPlan.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Click += new System.Windows.RoutedEventHandler(this.DrawingCheckBox_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

