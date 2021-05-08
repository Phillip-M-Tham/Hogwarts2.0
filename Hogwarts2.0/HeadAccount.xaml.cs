using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HeadAccount : Page
    {
        private string _userHuid = "";
        private string _userinfo = "";
        private string _updateaboutme = "";
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        public HeadAccount()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            Setuserinfo(_userHuid);
            //we can add one for the picture here
        }

        private async void Setuserinfo(string myuserhuid)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT AboutInfo FROM Users WHERE HUID ={myuserhuid};";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    _userinfo += reader.GetValue(0).ToString();
                                }
                            }
                        }
                        sqlConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new MessageDialog(ex.Message);
                await errorMessage.ShowAsync();
            }
            if (_userinfo == "")
            {
                UserInfoBox.Text = "Please add an about me by using the Edit button";
            }
            else
            {
                UserInfoBox.Text = _userinfo;
                EditUserInfoInput.Text = _userinfo;
            }
        }

        private void EditInfo_Click(object sender, RoutedEventArgs e)
        {
            Editform.Visibility = Visibility.Visible;
            EditInfo.Visibility = Visibility.Collapsed;
            UserInfoBox.Visibility = Visibility.Collapsed;

        }
        private async void SubmitStuUpdate_Click(object sender, RoutedEventArgs e)
        {
            //we can check to see if its an empty string
            //CONVERT TO A FRIENDLY STRING
            _updateaboutme = EditUserInfoInput.Text;
            if (_updateaboutme != "")
            {
                _updateaboutme = ConverttoFriendly(_updateaboutme);
                try
                {
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"UPDATE Users SET AboutInfo = '{_updateaboutme}' WHERE HUID ={_userHuid};", sqlConn);
                            adapter.UpdateCommand = command;
                            adapter.UpdateCommand.ExecuteNonQuery();
                            sqlConn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = new MessageDialog(ex.Message);
                    await errorMessage.ShowAsync();
                }
                Frame.Navigate(typeof(CounselorAccount), _userHuid);
            }
            else
            {
                var NotValidMessage = new MessageDialog("About Info is Empty, please try again");
                await NotValidMessage.ShowAsync();
            }
        }

        private string ConverttoFriendly(string badstring)
        {
            var goodstring = new StringBuilder();
            foreach (var char1 in badstring)
            {
                goodstring.Append(char1);
                if (char1 == '\'')
                {
                    goodstring.Append('\'');
                }
            }
            return goodstring.ToString();
        }

        private void CancelUpdate_Click(object sender, RoutedEventArgs e)
        {
            Editform.Visibility = Visibility.Collapsed;
            EditInfo.Visibility = Visibility.Visible;
            UserInfoBox.Visibility = Visibility.Visible;
        }
    }
}
