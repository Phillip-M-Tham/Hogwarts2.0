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
    public sealed partial class CounselorMain : Page
    {
        private string _firstname = "";
        private string _lastname = "";
        private string UserHuid = "";
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        public CounselorMain()
        {
            this.InitializeComponent();
        }
        private async void UpdatetopBar()
        {
            //This updates the top bar greeting
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT Firstname, Lastname FROM Users WHERE HUID ={UserHuid};";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    _firstname += reader.GetValue(0).ToString();
                                    _lastname += reader.GetValue(1).ToString();
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
            GreetingBar.Text = $"      Hello, {_firstname} {_lastname}";
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UserHuid = e.Parameter.ToString();
            //Navbar.Opacity = .8;
            UpdatetopBar();
        }
        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            switch (item.Tag.ToString())
            {
                case "Home":
                    Navbar.Opacity = .9;
                    ContentFrame.Navigate(typeof(CounselorHome), UserHuid);
                    break;
                case "AccountPage":
                    Navbar.Opacity = .9;
                    ContentFrame.Navigate(typeof(CounselorAccount), UserHuid);
                    break;
                case "EnrollStudents":
                    Navbar.Opacity = .9;
                    ContentFrame.Navigate(typeof(CounselorEnrollStudents), UserHuid);
                    break;
                case "AccountSettings":
                    Navbar.Opacity = .9;
                    ContentFrame.Navigate(typeof(CounselorAccountSettings), UserHuid);
                    break;
                case "Logout":
                    //Navbar.Opacity = .8;
                    Frame.Navigate(typeof(MainPage));
                    break;
            }
        }
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            Navbar.Opacity = .9;
            ContentFrame.Navigate(typeof(CounselorHome), UserHuid);
        }

        private void ChangePaneBackgroundsize(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
            test.Width = 44;
        }

        private void ChangePaneBackToNormal(NavigationView sender, object args)
        {
            test.Width = 313;
        }
    }
}
