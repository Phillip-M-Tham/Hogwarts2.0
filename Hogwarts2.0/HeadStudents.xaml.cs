﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HeadStudents : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private int StudentRowCounter = 0;
        private string SelectedHouse;
        private int SelectedStudentHUID;
        public HeadStudents()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            resetform3filter();
        }

        private void Form2AHufflepuff_Click(object sender, RoutedEventArgs e)
        {//setup the filter page based on the house click
            purgeForm3StudentList();
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            SelectedHouse = "Hufflepuff";
            setupForm3("default", 0);
        }
        private void Form2ARavenclaw_Click(object sender, RoutedEventArgs e)
        {
            purgeForm3StudentList();
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            SelectedHouse = "Ravenclaw";
            setupForm3("default", 0);
        }

        private void Form2ASlytherin_Click(object sender, RoutedEventArgs e)
        {
            purgeForm3StudentList();
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            SelectedHouse = "Slytherin";
            setupForm3("default", 0);
        }

        private void Form2AGryffindor_Click(object sender, RoutedEventArgs e)
        {
            purgeForm3StudentList();
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            SelectedHouse = "Gryffindor";
            setupForm3("default", 0);
        }
        private void setupForm3(string filter, int year)
        {
            StudentRowCounter = 0;
            List<int> UnfilteredyearIDs = new List<int>();
            List<int> myids = new List<int>();
            List<string> StudentNames = new List<string>();
            List<int> yearlevel = new List<int>();
            List<int> sortedIDs = new List<int>();
          
            if (SelectedHouse == "Hufflepuff")
            {
                Form3title.Text = "Hufflepuff";
            }
            else if (SelectedHouse == "Ravenclaw")
            {
                Form3title.Text = "Ravenclaw";
            }
            else if (SelectedHouse == "Slytherin")
            {
                Form3title.Text = "Slytherin";
            }
            else
            {
                Form3title.Text = "Gryffindor";
            }
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT HUID FROM Houses WHERE HouseName = '{SelectedHouse}';";
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
                        if (year == 0)
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
                                            StudentNames.Add(reader.GetValue(0).ToString() + " " + reader.GetValue(1).ToString());
                                        }
                                    }
                                }
                            }
                            if (filter == "default")
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all year levels from acquired ids
                                    foreach (var id in myids)
                                    {
                                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                yearlevel.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {//alphabettically sorted
                                StudentNames.Sort();
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all names from the acquired ids
                                    foreach (var name in StudentNames)
                                    {//sort the names alphabetically and get all users HUID with matching names
                                        string[] firstlast = name.Split(" ");
                                        cmd.CommandText = $"SELECT HUID FROM Users WHERE FirstName = '{firstlast[0]}' AND LastName = '{firstlast[1]}';";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                sortedIDs.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                    foreach (var unfilteredID in sortedIDs.ToList())
                                    {//filter ids against students table incase we got a HUID thats not a student
                                        if (myids.Contains(unfilteredID) == false)
                                        {
                                            sortedIDs.Remove(unfilteredID);
                                        }
                                    }
                                }
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all year levels from acquired ids
                                    foreach (var id in sortedIDs)
                                    {
                                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                yearlevel.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                           using (SqlCommand cmd = sqlConn.CreateCommand())
                            {//get all ids from the selected year
                                cmd.CommandText = $"SELECT HUID FROM Students WHERE StudentYear = {year};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        UnfilteredyearIDs.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                            foreach (var unfilteredID in UnfilteredyearIDs.ToList())
                            {//filter the yearIDS to the studentIDs in the selected house
                                if (myids.Contains(unfilteredID) == false)
                                {
                                    UnfilteredyearIDs.Remove(unfilteredID);
                                }
                            }

                            foreach (var id in UnfilteredyearIDs)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all names from the acquired ids
                                    cmd.CommandText = $"SELECT FirstName, LastName FROM Users WHERE HUID = {id};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            StudentNames.Add(reader.GetValue(0).ToString() + " " + reader.GetValue(1).ToString());
                                        }
                                    }
                                }
                            }
                            if (FilterbyReg.IsChecked == true)
                            {//do this if they choose to filter by HUID
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all year levels from acquired ids
                                    foreach (var id in UnfilteredyearIDs)
                                    {//ensures we only grab the student years from the selected year filter
                                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id} AND StudentYear = {year};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                yearlevel.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {//alphabetically is checked
                                StudentNames.Sort();
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all names from the acquired ids
                                    foreach (var name in StudentNames)
                                    {//sort the names alphabetically and get all users HUID with matching names
                                        string[] firstlast = name.Split(" ");
                                        cmd.CommandText = $"SELECT HUID FROM Users WHERE FirstName = '{firstlast[0]}' AND LastName = '{firstlast[1]}';";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                sortedIDs.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                    foreach (var unfilteredID in sortedIDs.ToList())
                                    {//filter ids against yearids incase we got a HUID thats not a student of the selected year
                                        if (UnfilteredyearIDs.Contains(unfilteredID) == false)
                                        {
                                            sortedIDs.Remove(unfilteredID);
                                        }
                                    }
                                }
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all year levels from acquired ids
                                    foreach (var id in sortedIDs)
                                    {//double tap by ensuring we select by specified student year
                                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id} AND StudentYear = {year};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                yearlevel.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    sqlConn.Close();
                }
            }
            
            if (StudentNames.Count > 0)
            {//post the name of each student as a clickable button
                Form3StudentTitle.Visibility = Visibility.Visible;
                Form3YearTitle.Visibility = Visibility.Visible;
                StudentRowCounter = 0;
                foreach (var name in StudentNames)
                {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    Form3StudentList.RowDefinitions.Add(newrow);

                    Border bd = new Border();
                    bd.BorderThickness = new Thickness(2);
                    bd.BorderBrush = new SolidColorBrush(Colors.Black);
                    bd.SetValue(Grid.RowProperty, StudentRowCounter);
                    bd.SetValue(Grid.ColumnProperty, 0);

                    Border bdyear = new Border();
                    bdyear.BorderThickness = new Thickness(2);
                    bdyear.BorderBrush = new SolidColorBrush(Colors.Black);
                    bdyear.SetValue(Grid.RowProperty, StudentRowCounter);
                    bdyear.SetValue(Grid.ColumnProperty, 1);

                    Button mybutton = new Button();
                    mybutton.FontSize = 36;
                    mybutton.Content = name.ToString();
                    mybutton.Foreground = new SolidColorBrush(Colors.Black);
                    mybutton.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    mybutton.HorizontalAlignment = HorizontalAlignment.Center;
                    mybutton.VerticalAlignment = VerticalAlignment.Center;
                    if (filter == "default" && year == 0)
                    {
                        mybutton.SetValue(NameProperty, myids[StudentRowCounter].ToString());
                    }else if(filter =="alph" && year == 0)
                    {
                        mybutton.SetValue(NameProperty, sortedIDs[StudentRowCounter].ToString());
                    }else if(filter =="default" && year !=0) 
                    {
                        mybutton.SetValue(NameProperty, UnfilteredyearIDs[StudentRowCounter].ToString());
                    }else if(filter =="alph" && year != 0)
                    {
                        mybutton.SetValue(NameProperty, sortedIDs[StudentRowCounter].ToString());
                    }
                    mybutton.Click += SetSelectedStudent;

                    TextBlock txtblck = new TextBlock();
                    txtblck.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    txtblck.FontSize = 36;
                    txtblck.Foreground = new SolidColorBrush(Colors.Black);
                    txtblck.HorizontalAlignment = HorizontalAlignment.Center;
                    txtblck.VerticalAlignment = VerticalAlignment.Center;
                    txtblck.Text = yearlevel[StudentRowCounter].ToString();
                    bdyear.Child = txtblck;
                    bd.Child = mybutton;
                    Form3StudentList.Children.Add(bd);
                    Form3StudentList.Children.Add(bdyear);

                    StudentRowCounter++;
                }
            }
            else
            {//post that no students can be found
                Form3YearTitle.Visibility = Visibility.Collapsed;
                Form3StudentTitle.Visibility = Visibility.Collapsed;
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                Form3StudentList.RowDefinitions.Add(newrow);

                TextBlock txtblck = new TextBlock();
                txtblck.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                txtblck.FontSize = 36;
                txtblck.Foreground = new SolidColorBrush(Colors.Black);
                txtblck.HorizontalAlignment = HorizontalAlignment.Center;
                txtblck.VerticalAlignment = VerticalAlignment.Center;
                txtblck.Text = "No Students Found";
                txtblck.SetValue(Grid.RowProperty, 0);
                txtblck.SetValue(Grid.ColumnProperty, 0);
            }
        }

        private async void SetSelectedStudent(object sender, RoutedEventArgs e)
        {
            Form3Filter.Visibility = Visibility.Collapsed;
            Form4.Visibility = Visibility.Visible;
            Form3.Visibility = Visibility.Collapsed;
            Button SelectedStudent = sender as Button;
            Int32.TryParse(SelectedStudent.Name, out SelectedStudentHUID);
            Form4Title.Text = SelectedStudent.Content.ToString();
            var NotValidMessage = new MessageDialog(SelectedStudentHUID.ToString());
            await NotValidMessage.ShowAsync();
        }

        private void resetform3filter()
        {
            YearlevelInput.SelectedValue = "All years";
            FilterbyReg.IsChecked = true;
            FilterbyAlph.IsChecked = false;
        }

        private void Form3Cancel_Click(object sender, RoutedEventArgs e)
        {
            //need to purgestudent list
            Form3.Visibility = Visibility.Collapsed;
            Form2.Visibility = Visibility.Visible;
            Form3Filter.Visibility = Visibility.Collapsed;
            resetform3filter();
            purgeForm3StudentList();
        }

        private void purgeForm3StudentList()
        {
            Form3StudentList.RowDefinitions.Clear();
            Form3StudentList.Children.Clear();
        }

        private void Form3Filter_click(object sender, RoutedEventArgs e)
        {
            Form3Filter.Visibility = Visibility.Visible;
        }

        private void Form3FilterCancel_Click(object sender, RoutedEventArgs e)
        {
            Form3Filter.Visibility = Visibility.Collapsed;
        }

        private void SetFilter(object sender, RoutedEventArgs e)
        {
            int year;
            Int32.TryParse(YearlevelInput.SelectedValue.ToString(), out year);

            if ((sender as CheckBox).Name == "FilterbyAlph")
            {
                FilterbyReg.IsChecked = false;
                purgeForm3StudentList();
                setupForm3("alph", year);
            }
            else if ((sender as CheckBox).Name == "FilterbyReg")
            {
                FilterbyAlph.IsChecked = false;
                purgeForm3StudentList();
                setupForm3("default", year);
            }
        }
        private async void FilterAttemptUncheck(object sender, RoutedEventArgs e)
        {
            if (FilterbyAlph.IsChecked == false && FilterbyReg.IsChecked == false)
            {
                (sender as CheckBox).IsChecked = true;
                var NotValidMessage = new MessageDialog("Please select a filter that is not currently in use.");
                await NotValidMessage.ShowAsync();
            }
        }

        private void YearlevelInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int year;
            Int32.TryParse(YearlevelInput.SelectedValue.ToString(), out year);
            purgeForm3StudentList();
            if (FilterbyReg.IsChecked == true)
            {
                setupForm3("default", year);
            }
            else
            {
                setupForm3("alph", year);
            }
        }
        private void Form4Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form4.Visibility = Visibility.Collapsed;
            Form3.Visibility = Visibility.Visible;
        }
    }
}
