﻿#pragma checksum "C:\Users\phill\MSSA2021\CSharp\Hogwarts2.0\HeadAccountSettings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D07E4DFF38B5978AC69F3A344A506AA3"
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
    partial class HeadAccountSettings : 
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
            case 2: // HeadAccountSettings.xaml line 12
                {
                    this.Profbackground3 = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 3: // HeadAccountSettings.xaml line 15
                {
                    this.Formbackground = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 4: // HeadAccountSettings.xaml line 16
                {
                    this.AccountSettingsFormTitle = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 5: // HeadAccountSettings.xaml line 17
                {
                    this.ChangePassword = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.ChangePassword).Click += this.ChangePassword_Click;
                }
                break;
            case 6: // HeadAccountSettings.xaml line 18
                {
                    this.ChangePasswordForm = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 7: // HeadAccountSettings.xaml line 41
                {
                    this.UpdatePassword = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.UpdatePassword).Click += this.UpdatePassword_Click;
                }
                break;
            case 8: // HeadAccountSettings.xaml line 38
                {
                    this.CancelUpdatePassword = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.CancelUpdatePassword).Click += this.CancelUpdatePassword_Click;
                }
                break;
            case 9: // HeadAccountSettings.xaml line 35
                {
                    this.ConfirmNewPasswordInput = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 10: // HeadAccountSettings.xaml line 32
                {
                    this.NewPasswordInput = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 11: // HeadAccountSettings.xaml line 29
                {
                    this.OldPasswordInput = (global::Windows.UI.Xaml.Controls.PasswordBox)(target);
                }
                break;
            case 12: // HeadAccountSettings.xaml line 26
                {
                    this.ConfirmNewPassword = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 13: // HeadAccountSettings.xaml line 23
                {
                    this.NewPassword = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 14: // HeadAccountSettings.xaml line 20
                {
                    this.OldPassword = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
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

