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
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfessorMain : Page
    {
        private string _firstname = "";
        private string _lastname = "";
        private string UserHuid = "";
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        public ProfessorMain()
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
            GreetingBar.Text = $"      Hello, Professor {_lastname}";
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UserHuid = e.Parameter.ToString();
            //Navbar.Opacity = .8;
            UpdatetopBar();
            checkforHead();
        }

        private void checkforHead()
        {
            string HeadofHouse="";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                { 
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {//checks if the logged in Professor is a head of a house
                        cmd.CommandText = $"SELECT HouseName FROM HouseHead WHERE ProfHUID = {UserHuid};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HeadofHouse=(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if(HeadofHouse != "")
            {//they are a head of a house
                Navbar.MenuItems.Insert(Navbar.MenuItems.Count-1,new NavigationViewItem
                {//add the head of house button to navbar
                    Content = $"Head of {HeadofHouse}",
                    Foreground = new SolidColorBrush(Colors.Black),
                    Icon = new SymbolIcon(Symbol.Admin),
                    FontFamily = new FontFamily("/Assets/ReginaScript.ttf#Regina Script"),
                    Tag = "HeadOfHouse",
                    FontSize = 36
                }) ;
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            switch (item.Tag.ToString())
            {
                case "Home":
                    //Navbar.Opacity = .8;
                    ContentFrame.Navigate(typeof(ProfHome), UserHuid);
                    break;
                case "AccountPage":
                    //Navbar.Opacity = .8;
                    ContentFrame.Navigate(typeof(ProfAccount), UserHuid);
                    break;
                case "MyCoursesPage":
                    //Navbar.Opacity = 1;
                    ContentFrame.Navigate(typeof(ProfCourses), UserHuid);
                    break;
                case "AccountSettings":
                    //Navbar.Opacity = 1;
                    ContentFrame.Navigate(typeof(ProfAccountSettings), UserHuid);
                    break;
                case "HeadOfHouse":
                    ContentFrame.Navigate(typeof(ProfHeadofHouse), UserHuid);
                    break;
                case "Logout":
                    //Navbar.Opacity = .8;
                    Frame.Navigate(typeof(MainPage));
                    break;
            }
        }
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(ProfHome), UserHuid);
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
