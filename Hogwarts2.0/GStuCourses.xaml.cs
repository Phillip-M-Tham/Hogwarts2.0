using System;
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
    public sealed partial class GStuCourses : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";

        public GStuCourses()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            setupforms();
        }

        private void setupforms()
        {
            int semestercount = 0;
            List<int> mysemesterids = new List<int>();
            List<string> semesternames = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//grabs the semesterids for the student enrolled courses
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT DISTINCT SemesterID FROM StudentEnrolledCourses WHERE StudentID = {_userHuid};";
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    mysemesterids.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                    }
                    if (mysemesterids.Count > 0)
                    {//convert semesterids to name of semester
                        foreach (var id in mysemesterids)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Semester FROM Semesters WHERE SemesterID = {id};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        semesternames.Add(reader.GetValue(0).ToString());
                                    }
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (semesternames.Count > 0)
            {
                foreach (var name in semesternames)
                {//populates the semester table with each semester student enrolled in
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    SemesterList.RowDefinitions.Add(newrow);

                    Button semester = new Button();
                    semester.FontSize = 36;
                    semester.Foreground = new SolidColorBrush(Colors.Black);
                    semester.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    semester.HorizontalAlignment = HorizontalAlignment.Center;
                    semester.VerticalAlignment = VerticalAlignment.Center;
                    semester.Content = name;
                    semester.Name = mysemesterids[semestercount].ToString();
                    semester.SetValue(Grid.RowProperty, semestercount);
                    semester.SetValue(Grid.ColumnProperty, 0);
                    SemesterList.Children.Add(semester);
                    semester.Click += GotoCourses;
                    semestercount++;
                }
            }
            else
            {//informs user they are not enrolled in any semester
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                SemesterList.RowDefinitions.Add(newrow);

                TextBlock txtblck = new TextBlock();
                txtblck.FontSize = 36;
                txtblck.Foreground = new SolidColorBrush(Colors.Black);
                txtblck.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                txtblck.HorizontalAlignment = HorizontalAlignment.Center;
                txtblck.VerticalAlignment = VerticalAlignment.Center;
                txtblck.Text = "You Are not currently enrolled in any semester";
                txtblck.SetValue(Grid.RowProperty, 0);
                txtblck.SetValue(Grid.ColumnProperty, 0);
                SemesterList.Children.Add(txtblck);
            }
        }

        private void GotoCourses(object sender, RoutedEventArgs e)
        {
            Button pressedbutton = sender as Button;
            int semesterID = Int32.Parse(pressedbutton.Name.ToString());
            List<int> courseids = new List<int>();
            List<string> coursenames = new List<string>();
            int coursecount = 0;
            Form2.Visibility = Visibility.Visible;
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//grabs the semesterids for the student enrolled courses
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT CourseID FROM StudentEnrolledCourses WHERE StudentID = {_userHuid} AND SemesterID = {semesterID};";
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    courseids.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                    }
                    if(courseids.Count > 0)
                    {
                        foreach(var courseid in courseids)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Title FROM Courses WHERE CourseID = {courseid};";
                                {
                                    using(SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            coursenames.Add(reader.GetValue(0).ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                sqlConn.Close();
                }
            }
            if(coursenames.Count > 0)
            {
                foreach(var name in coursenames)
                {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    CoursesList.RowDefinitions.Add(newrow);

                    Button course = new Button();
                    course.FontSize = 36;
                    course.Foreground = new SolidColorBrush(Colors.Black);
                    course.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    course.HorizontalAlignment = HorizontalAlignment.Center;
                    course.VerticalAlignment = VerticalAlignment.Center;
                    course.Content = name;
                    //course.Name = mysemesterids[semestercount].ToString();
                    course.SetValue(Grid.RowProperty, coursecount);
                    course.SetValue(Grid.ColumnProperty, 0);
                    CoursesList.Children.Add(course);
                    //semester.Click += GotoCourses;
                    coursecount++;
                }
            }
            else
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                CoursesList.RowDefinitions.Add(newrow);

                TextBlock txtblck = new TextBlock();
                txtblck.FontSize = 36;
                txtblck.Foreground = new SolidColorBrush(Colors.Black);
                txtblck.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                txtblck.HorizontalAlignment = HorizontalAlignment.Center;
                txtblck.VerticalAlignment = VerticalAlignment.Center;
                txtblck.Text = "You Are not currently enrolled in any courses for this semester";
                txtblck.SetValue(Grid.RowProperty, 0);
                txtblck.SetValue(Grid.ColumnProperty, 0);
                CoursesList.Children.Add(txtblck);
            }
        }

        private void Form2Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Collapsed;
        }
    }
}
