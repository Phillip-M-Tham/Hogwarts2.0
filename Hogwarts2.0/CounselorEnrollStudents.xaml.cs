using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CounselorEnrollStudents : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        int rowposition = 0;
        int rowposition2 = 0;
        int rowposition3 = 0;

        public CounselorEnrollStudents()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
        }
        private void EnrollGryffindorCancel_Click(object sender, RoutedEventArgs e)
        {
            GryffindorOptions.Visibility = Visibility.Collapsed;
        }

        private void EnableGryffindor(object sender, RoutedEventArgs e)
        {
            GryffindorOptions.Visibility = Visibility.Visible;
        }

        private void EnrollGryffindor_Click(object sender, RoutedEventArgs e)
        {
            GryffindorEnroll.Visibility = Visibility.Visible;
            GryffindorOptions.Visibility = Visibility.Collapsed;
            PopulateTableAll(1);
        }

        private async void PopulateTableAll(int house)
        {
            //Grab all students from gryffindor
            string housename ="";
            if(house == 1)
            {
                housename = "Gryffindor";
            }else if(house == 2)
            {
                housename = "Slytherin";
            }else if(house == 3)
            {
                housename = "Ravenclaw"; 
            }else if(house == 4)
            {
                housename = "Hufflepuff";
            }
            else
            {

            }
            List<string> studentnames = new List<string>();
            List<int> yearlevel = new List<int>();
            List<int> myids = new List<int>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {//get all student ids in Gryffindor
                            cmd.CommandText = $"SELECT HUID FROM Houses WHERE HouseName ='{housename}';";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    myids.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                        if (myids.Count > 0)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {//get all names from the acquired ids
                                foreach (var id in myids)
                                {
                                    cmd.CommandText = $"SELECT FirstName, LastName FROM Users WHERE HUID = {id};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            studentnames.Add(reader.GetValue(0).ToString() + " " + reader.GetValue(1).ToString());
                                        }
                                    }
                                }

                            }
                            using(SqlCommand cmd = sqlConn.CreateCommand())
                            {//get all year levels from acquired ids
                                foreach(var id in myids)
                                {
                                    cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id};";
                                    using(SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            yearlevel.Add((int)reader.GetValue(0));
                                        }
                                    }
                                }
                            }
                        }
                        sqlConn.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                var errorMessage = new MessageDialog(ex.Message);
                await errorMessage.ShowAsync();
            }
            
            if (studentnames.Count == 0)
            {
                var errorMessage = new MessageDialog("Failed to get the names");
                await errorMessage.ShowAsync();
            }
            if(yearlevel.Count == 0)
            {
                var errorMessage2 = new MessageDialog("Failed to get the years");
                await errorMessage2.ShowAsync();
            }
            
            if(yearlevel.Count > 0 && studentnames.Count > 0)
            {
                
                foreach (var student in studentnames)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(75.00);
                    StudentTable.RowDefinitions.Add(row);

                    Button btn = new Button();
                    btn.FontSize = 36;
                    btn.Content = student.ToString();
                    btn.Foreground = new SolidColorBrush(Colors.Black);
                    btn.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.VerticalAlignment = VerticalAlignment.Center;

                    Border myborder = new Border();
                    myborder.BorderThickness = new Thickness(2);
                    myborder.BorderBrush = new SolidColorBrush(Colors.Black);
                    myborder.SetValue(Grid.RowProperty, rowposition);
                    myborder.SetValue(Grid.ColumnProperty, 0);
                    myborder.Child = btn;

                    StudentTable.Children.Add(myborder);
                    rowposition++;
                }
                //ColumnDefinition col2 = new ColumnDefinition();
                //col2.Width = new GridLength(100.00);
               // StudentTable.ColumnDefinitions.Add(col2);
                //int rowposition2 = 0;
                foreach (var year in yearlevel)
                {
                    /*RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(75.00);
                    StudentTable.RowDefinitions.Add(row);*/

                    TextBlock txtblock = new TextBlock();
                    txtblock.FontSize = 36;
                    txtblock.Text = year.ToString();
                    txtblock.Foreground = new SolidColorBrush(Colors.Black);
                    txtblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    txtblock.HorizontalAlignment = HorizontalAlignment.Center;
                    txtblock.VerticalAlignment = VerticalAlignment.Center;

                    Border myborder = new Border();
                    myborder.BorderThickness = new Thickness(2);
                    myborder.BorderBrush = new SolidColorBrush(Colors.Black);
                    myborder.SetValue(Grid.RowProperty, rowposition2);
                    myborder.SetValue(Grid.ColumnProperty, 1);
                    myborder.Child = txtblock;

                    StudentTable.Children.Add(myborder);
                    rowposition2++;
                }
                //ColumnDefinition col3 = new ColumnDefinition();
                //col3.Width = new GridLength(100.00);
                //StudentTable.ColumnDefinitions.Add(col3);
                //int rowposition3 = 0;
                foreach(var id in myids)
                {
                    /*RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(75.00);
                    StudentTable.RowDefinitions.Add(row);*/

                    TextBlock txtblock = new TextBlock();
                    txtblock.FontSize = 36;
                    txtblock.Text = id.ToString();
                    txtblock.Foreground = new SolidColorBrush(Colors.Black);
                    txtblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    txtblock.HorizontalAlignment = HorizontalAlignment.Center;
                    txtblock.VerticalAlignment = VerticalAlignment.Center;

                    Border myborder = new Border();
                    myborder.BorderThickness = new Thickness(2);
                    myborder.BorderBrush = new SolidColorBrush(Colors.Black);
                    myborder.SetValue(Grid.RowProperty, rowposition3);
                    myborder.SetValue(Grid.ColumnProperty, 2);
                    myborder.Child = txtblock;

                    StudentTable.Children.Add(myborder);
                    rowposition3++;
                }


                var test = new MessageDialog(rowposition.ToString());
                await test.ShowAsync();
            }
        }
        
        private void DisenrollGryffindor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GryffindorEnrollCancel_Click(object sender, RoutedEventArgs e)
        {
            GryffindorEnroll.Visibility = Visibility.Collapsed;
            GryffindorOptions.Visibility = Visibility.Visible;
            //depopulate the table 
        }

        private void GryffindorEnrollSubmit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
