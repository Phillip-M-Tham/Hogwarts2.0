using System.Collections.Generic;
using System.Data.SqlClient;
using Windows.UI;
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
        private int StudentRowCounter;
        public HeadStudents()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
        }
  
        private void Form2AHufflepuff_Click(object sender, RoutedEventArgs e)
        {//setup the filter page based on the house click
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            setupForm3("Hufflepuff");
        }
        private void Form2ARavenclaw_Click(object sender, RoutedEventArgs e)
        {
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            setupForm3("Ravenclaw");
        }

        private void Form2ASlytherin_Click(object sender, RoutedEventArgs e)
        {
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            setupForm3("Slytherin");
        }

        private void Form2AGryffindor_Click(object sender, RoutedEventArgs e)
        {
            Form3.Visibility = Visibility.Visible;
            Form2.Visibility = Visibility.Collapsed;
            setupForm3("Gryffindor");
        }
        private void setupForm3(string house)
        {
            StudentRowCounter = 0;
            List<int> UnfilteredStudentIDs = new List<int>();
            List<string> StudentNames = new List<string>();
            if (house == "Hufflepuff")
            {
                Form3title.Text = "Hufflepuff Students";
            }
            else if (house == "Ravenclaw")
            {
                Form3title.Text = "Ravenclaw Students";
            }
            else if (house == "Slytherin")
            {
                Form3title.Text = "Slytherin Students";
            }
            else
            {
                Form3title.Text = "Gryffindor Students";
            }

            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT HUID FROM Houses WHERE HouseName = '{house}';";
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
                        }
                    }
                    sqlConn.Close();
                }
            }
            if(StudentNames.Count > 0)
            {//post the name of each student as a clickable button
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
                    bd.Child = mybutton;
                    Form3StudentList.Children.Add(bd);

                    StudentRowCounter++;
                }
            }
            else
            {//post that no students can be found
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

        private void Form3Cancel_Click(object sender, RoutedEventArgs e)
        {
            //need to purgestudent list
            Form3.Visibility = Visibility.Collapsed;
            Form2.Visibility = Visibility.Visible;
            purgeForm3StudentList();
        }

        private void purgeForm3StudentList()
        {
            Form3StudentList.RowDefinitions.Clear();
            Form3StudentList.Children.Clear();
        }

        private void Form3Filter_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
