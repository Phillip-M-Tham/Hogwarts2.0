using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class HeadFaculty : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private int FacultyApplicationsRow = 0;
        public HeadFaculty()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
        }

        private void Form1ViewFaculty_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Form1ViewApplications_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Visible;
            Form1.Visibility = Visibility.Collapsed;
            setupForm2();
        }

        private void setupForm2()
        {
            List<int> FacHUID = new List<int>();
            List<string> FacRoles = new List<string>();
            List<string> CandidateNames = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT FacHUID,FacRole FROM FacultyCandidate;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FacHUID.Add((int)reader.GetValue(0));
                                FacRoles.Add(reader.GetValue(1).ToString());
                            }
                        }
                    }
                    if(FacHUID.Count > 0)
                    {
                        foreach (var Id in FacHUID)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {//gets the name from each acquired ID
                                cmd.CommandText = $"SELECT FirstName,LastName FROM Users WHERE HUID = {Id};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        CandidateNames.Add(reader.GetValue(0) + " " + reader.GetValue(1));
                                    }
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if(CandidateNames.Count > 0)
            {
                foreach (var name in CandidateNames) {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    ApplicationList.RowDefinitions.Add(newrow);

                    Border bd1 = new Border();
                    bd1.BorderThickness = new Thickness(2);
                    bd1.BorderBrush = new SolidColorBrush(Colors.Black);
                    bd1.SetValue(Grid.RowProperty, FacultyApplicationsRow);
                    bd1.SetValue(Grid.ColumnProperty, 0);

                    TextBlock txtblock = new TextBlock();
                    txtblock.FontSize = 36;
                    txtblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    txtblock.Foreground = new SolidColorBrush(Colors.Black);
                    txtblock.TextAlignment = TextAlignment.Center;
                    txtblock.Text = name + $"Role : {FacRoles[FacultyApplicationsRow]}";
                    //bd.Child = txtbox;


                    Border bd2 = new Border();
                    bd2.BorderThickness = new Thickness(2);
                    bd2.BorderBrush = new SolidColorBrush(Colors.Black);
                    bd2.SetValue(Grid.RowProperty, FacultyApplicationsRow);
                    bd2.SetValue(Grid.ColumnProperty, 1);



                    FacultyApplicationsRow++;
                }
            }
            else
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Collapsed;
            Form1.Visibility = Visibility.Visible;
        }
    }
}
