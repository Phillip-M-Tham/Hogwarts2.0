using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Data.SqlClient;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RStuAccountSettings : Page
    {
        private string _newpassword = "";
        private string _password = "";
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";

        public RStuAccountSettings()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordForm.Visibility = Visibility.Visible;
            ChangePassword.Visibility = Visibility.Collapsed;
        }
        private async void UpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            _password = "";
            try
            {
                //See if the original password is valid
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT UserPassword FROM Users WHERE HUID ='{_userHuid}';";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    _password += reader.GetValue(0).ToString();
                                }
                            }
                        }
                        sqlConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var NotValidMessage = new MessageDialog(ex.Message);
                await NotValidMessage.ShowAsync();
            }
            if (_password == OldPasswordInput.Password.ToString())
            {
                if (NewPasswordInput.Text.ToString() == ConfirmNewPasswordInput.Text.ToString())
                {
                    if (NewPasswordInput.Text.ToString() != "" && ConfirmNewPasswordInput.Text.ToString() != "")
                    {
                        _newpassword = NewPasswordInput.Text.ToString();
                        try
                        {
                            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                            {
                                sqlConn.Open();
                                if (sqlConn.State == System.Data.ConnectionState.Open)
                                {
                                    SqlDataAdapter adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"UPDATE Users SET UserPassword = '{_newpassword}' WHERE HUID ='{_userHuid}';", sqlConn);
                                    adapter.UpdateCommand = command;
                                    adapter.UpdateCommand.ExecuteNonQuery();
                                }
                                sqlConn.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            var Error2 = new MessageDialog(ex.Message);
                            await Error2.ShowAsync();
                        }
                        Frame.Navigate(typeof(RStuAccountSettings), _userHuid);
                    }
                    else
                    {
                        var NotValidNewPass2 = new MessageDialog("New Password and Confirmation Password are empty");
                        await NotValidNewPass2.ShowAsync();
                    }
                }
                else
                {
                    var NotValidNewPass = new MessageDialog("New Password and Confirmation Password does not match");
                    await NotValidNewPass.ShowAsync();
                }
            }
            else
            {
                var NotValidPassword = new MessageDialog("Old Password does not match please try again");
                await NotValidPassword.ShowAsync();
            }
        }
        private void CancelUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword.Visibility = Visibility.Visible;
            ChangePasswordForm.Visibility = Visibility.Collapsed;
        }
    }
}
