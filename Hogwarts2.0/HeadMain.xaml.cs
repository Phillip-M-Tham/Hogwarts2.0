using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class HeadMain : Page
    {
        private string _firstname = "";
        private string _lastname = "";
        private string UserHuid = "";
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        public HeadMain()
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
            Navbar.Opacity = .8;
            UpdatetopBar();
        }
        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            switch (item.Tag.ToString())
            {
                case "Home":
                    //Navbar.Opacity = .8;
                    ContentFrame.Navigate(typeof(HeadHome), UserHuid);
                    break;
                case "AccountPage":
                    //Navbar.Opacity = .8;
                    ContentFrame.Navigate(typeof(HeadAccount), UserHuid);
                    break;
                case "Departments":
                    //Navbar.Opacity = 1;
                    ContentFrame.Navigate(typeof(HeadDepartments), UserHuid);
                    break;
                case "AccountSettings":
                    //Navbar.Opacity = 1;
                    ContentFrame.Navigate(typeof(HeadAccountSettings), UserHuid);
                    break;
                case "Logout":
                    //Navbar.Opacity = .8;
                    Frame.Navigate(typeof(MainPage));
                    break;
            }
        }
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(HeadHome), UserHuid);
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
