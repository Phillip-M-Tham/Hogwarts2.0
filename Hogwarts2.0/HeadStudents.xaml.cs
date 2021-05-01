using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private int StudentRowCounter=0;
        private string SelectedHouse;
        public HeadStudents()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            setupform3filter();
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
            setupForm3("default",0);
        }

        private void Form2ASlytherin_Click(object sender, RoutedEventArgs e)
        {
            purgeForm3StudentList();
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            SelectedHouse = "Slytherin";
            setupForm3("default",0);
        }

        private void Form2AGryffindor_Click(object sender, RoutedEventArgs e)
        {
            purgeForm3StudentList();
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            SelectedHouse = "Gryffindor";
            setupForm3("default",0);
        }
        private void setupForm3(string filter, int year)
        {
            StudentRowCounter = 0;
            List<int> UnfilteredStudentIDs = new List<int>();
            List<string> StudentNames = new List<string>();
            List<int> UnfilteredYears = new List<int>();
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
                                UnfilteredStudentIDs.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if(UnfilteredStudentIDs.Count > 0)
                    {
                        foreach(var id in UnfilteredStudentIDs)
                        {
                            using(SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Firstname,Lastname FROM Users WHERE HUID = {id};";
                                using(SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        StudentNames.Add(reader.GetValue(0).ToString() +" "+ reader.GetValue(1).ToString());
                                    }
                                }
                            }
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        UnfilteredYears.Add((int)reader.GetValue(0));
                                    }
                                }
                            }

                        }
                    }
                    sqlConn.Close();
                }
            }
            if(StudentNames.Count > 0)
            {//post the name of each student as a clickable button
                Form3StudentTitle.Visibility = Visibility.Visible;
                Form3YearTitle.Visibility = Visibility.Visible;
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
                    mybutton.SetValue(NameProperty, UnfilteredStudentIDs[StudentRowCounter].ToString());
                    //mybutton.Click += SetSelectedCourse;
                    //put the button in the border
                    
                    TextBlock txtblck = new TextBlock();
                    txtblck.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    txtblck.FontSize = 36;
                    txtblck.Foreground = new SolidColorBrush(Colors.Black);
                    txtblck.HorizontalAlignment = HorizontalAlignment.Center;
                    txtblck.VerticalAlignment = VerticalAlignment.Center;
                    txtblck.Text = UnfilteredYears[StudentRowCounter].ToString();

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

        private void setupform3filter()
        {
            YearlevelInput.SelectedValue = "All years";
            FilterbyAlph.IsChecked = false;
            FilterbyReg.IsChecked = true;
        }

        private void Form3Cancel_Click(object sender, RoutedEventArgs e)
        {
            //need to purgestudent list
            Form3.Visibility = Visibility.Collapsed;
            Form2.Visibility = Visibility.Visible;
            Form3Filter.Visibility = Visibility.Collapsed;
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
    }
}
