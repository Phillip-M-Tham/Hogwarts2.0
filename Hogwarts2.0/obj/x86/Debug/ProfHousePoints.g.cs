﻿#pragma checksum "C:\Users\phill\MSSA2021\CSharp\Hogwarts2.0\ProfHousePoints.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D9B3DEB06584170F28FC02C049221103"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hogwarts2._0
{
    partial class ProfHousePoints : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // ProfHousePoints.xaml line 13
                {
                    this.Form1 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3: // ProfHousePoints.xaml line 14
                {
                    this.Form1Background = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 4: // ProfHousePoints.xaml line 54
                {
                    this.ApplyPoints = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.ApplyPoints).Click += this.ApplyPoints_Click;
                }
                break;
            case 5: // ProfHousePoints.xaml line 51
                {
                    this.PointInput = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 6: // ProfHousePoints.xaml line 43
                {
                    this.HouseInput = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 7: // ProfHousePoints.xaml line 40
                {
                    this.Spoints = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 8: // ProfHousePoints.xaml line 37
                {
                    this.Hpoints = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 9: // ProfHousePoints.xaml line 34
                {
                    this.Rpoints = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10: // ProfHousePoints.xaml line 31
                {
                    this.Gpoints = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11: // ProfHousePoints.xaml line 16
                {
                    this.Form1Title = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}
