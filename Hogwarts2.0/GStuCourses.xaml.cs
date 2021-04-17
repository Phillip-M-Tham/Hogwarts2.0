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
    public sealed partial class GStuCourses : Page
    {
        private string _userHuid;
        private int SelectedSemesterID;
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
            Populatetable(semesternames, mysemesterids,1);
        }

        private void Populatetable(List<string> thelist,List<int> ids ,int mode, List<decimal> grades = null)
        {
            Int32.TryParse(_userHuid,out int studentid);
            int rowcounter = 0;
            bool validgrade;
            if (thelist.Count > 0)
            {
                foreach (var name in thelist)
                {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    
                    Border buttonborder = new Border();
                    buttonborder.BorderBrush = new SolidColorBrush(Colors.Black);
                    buttonborder.BorderThickness = new Thickness(2);
                    buttonborder.SetValue(Grid.RowProperty, rowcounter);
                    buttonborder.SetValue(Grid.ColumnProperty, 0);
                    if (mode != 3)
                    {
                        Button thebutton = new Button();
                        thebutton.FontSize = 36;
                        thebutton.Foreground = new SolidColorBrush(Colors.Black);
                        thebutton.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        thebutton.HorizontalAlignment = HorizontalAlignment.Center;
                        thebutton.VerticalAlignment = VerticalAlignment.Center;
                        thebutton.Content = name;
                        thebutton.Name = ids[rowcounter].ToString();
                        buttonborder.Child = thebutton;
                        if (mode == 1)
                        {//populates the semester table with each semester student enrolled in
                            SemesterList.RowDefinitions.Add(newrow);
                            SemesterList.Children.Add(buttonborder);
                            thebutton.Click += GotoCourses;
                        }
                        else if (mode == 2)
                        {//populates the courses table from the selected semester student enrolled in
                            CoursesList.RowDefinitions.Add(newrow);
                            CoursesList.Children.Add(buttonborder);
                            thebutton.Click += GotoGrades;
                        }
                    }
                    else
                    {
                        GradesList.RowDefinitions.Add(newrow);
                        TextBlock assignment = new TextBlock();
                        assignment.FontSize = 36;
                        assignment.Foreground = new SolidColorBrush(Colors.Black);
                        assignment.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        assignment.HorizontalAlignment = HorizontalAlignment.Center;
                        assignment.VerticalAlignment = VerticalAlignment.Center;
                        assignment.Text = name;
                        assignment.Name = ids[rowcounter].ToString();

                        Border newborder = new Border();
                        newborder.BorderBrush = new SolidColorBrush(Colors.Black);
                        newborder.BorderThickness = new Thickness(2);
                        newborder.SetValue(Grid.RowProperty, rowcounter);
                        newborder.SetValue(Grid.ColumnProperty, 1);

                        TextBlock grade = new TextBlock();
                        grade.FontSize = 36;
                        grade.Foreground = new SolidColorBrush(Colors.Black);
                        grade.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        grade.HorizontalAlignment = HorizontalAlignment.Center;
                        grade.VerticalAlignment = VerticalAlignment.Center;
                        //validgrade check here
                        validgrade = IsValidGrade(studentid,ids[rowcounter]);
                        if (validgrade == true)
                        {
                            grade.Text = grades[rowcounter].ToString();
                        }
                        else
                        {
                            grade.Text = "";
                        }
                        newborder.Child = grade;
                        buttonborder.Child = assignment;
                        GradesList.Children.Add(buttonborder);
                        GradesList.Children.Add(newborder);
                    }
                    rowcounter++;
                }
            }
            else
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                
                TextBlock txtblck = new TextBlock();
                txtblck.FontSize = 36;
                txtblck.Foreground = new SolidColorBrush(Colors.Black);
                txtblck.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                txtblck.HorizontalAlignment = HorizontalAlignment.Center;
                txtblck.VerticalAlignment = VerticalAlignment.Center;
                txtblck.SetValue(Grid.RowProperty, 0);
                txtblck.SetValue(Grid.ColumnProperty, 0);
                
                if(mode == 1)
                {
                    SemesterList.RowDefinitions.Add(newrow);
                    txtblck.Text = "You Are not currently enrolled in any semester";
                    SemesterList.Children.Add(txtblck);
                }
                else if(mode == 2)
                {
                    CoursesList.RowDefinitions.Add(newrow);
                    txtblck.Text = "You Are not currently enrolled in any courses for this semester";
                    CoursesList.Children.Add(txtblck);
                }else if(mode==3)
                {
                    GradesList.RowDefinitions.Add(newrow);
                    txtblck.Text = "There are no assignments for this course currently";
                    GradesList.Children.Add(txtblck);
                }
            }
        }
        private bool IsValidGrade(int studentid, int assignmentid)
        {
            string validgrade = "";
            bool result;
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the all students enrolled in the course 
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Grade FROM Grades WHERE StudentHUID = {studentid} AND AssignmentID = {assignmentid};";                      
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                validgrade += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (validgrade == "")
            {
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
        private void GotoGrades(object sender, RoutedEventArgs e)
        {
            Form3.Visibility = Visibility.Visible;
            Button course = sender as Button;
            int courseID = Int32.Parse(course.Name.ToString());
            List<int> assignmentids = new List<int>();
            List<string> assignmentnames = new List<string>();
            List<decimal> grades = new List<decimal>();
            //need assignment ids
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//grabs the semesterids for the student enrolled courses
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT AssignmentID,AssignmentTitle FROM Assignments WHERE CourseID = {courseID} AND SemesterID = {SelectedSemesterID};";
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    assignmentids.Add((int)reader.GetValue(0));
                                    assignmentnames.Add(reader.GetValue(1).ToString());
                                }
                            }
                        }
                    }
                    if (assignmentids.Count > 0)
                    {
                        foreach (var id in assignmentids)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Grade From Grades WHERE StudentHUID = {_userHuid} AND AssignmentID = {id};";
                                using(SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while(reader.Read())
                                    {
                                        grades.Add((decimal)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                    }
                }
                sqlConn.Close();
            }
            Populatetable(assignmentnames,assignmentids,3,grades);
            SetupFinalGradeText(courseID);
        }
        private void SetupFinalGradeText(int courseID)
        {
            string validgrade = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the all students enrolled in the course 
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT CurrentGrade FROM FinalGrade WHERE StudentHUID = {_userHuid} AND CourseID = {courseID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                validgrade += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (validgrade == "")
            {
                FinalGrade.Text = "N/A";
            }
            else
            {
                FinalGrade.Text = validgrade;
            }
        }

        private void GotoCourses(object sender, RoutedEventArgs e)
        {
            Button pressedbutton = sender as Button;
            SelectedSemesterID = Int32.Parse(pressedbutton.Name.ToString());
            List<int> courseids = new List<int>();
            List<string> coursenames = new List<string>();
            Form2.Visibility = Visibility.Visible;
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//grabs the semesterids for the student enrolled courses
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT CourseID FROM StudentEnrolledCourses WHERE StudentID = {_userHuid} AND SemesterID = {SelectedSemesterID};";
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
            Populatetable(coursenames, courseids, 2);
        }

        private void Form2Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Collapsed;
            CoursesList.Children.Clear();
            CoursesList.RowDefinitions.Clear();
        }

        private void Form3Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form3.Visibility = Visibility.Collapsed;
            GradesList.Children.Clear();
            GradesList.RowDefinitions.Clear();
        }
    }
}
