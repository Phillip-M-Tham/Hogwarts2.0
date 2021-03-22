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
    public sealed partial class CounselorEnrollStudents : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";

        public CounselorEnrollStudents()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            SetUpSchedules();
            SetUpCalendar();
        }

        private void SetUpCalendar()
        {
            Queue<string> mytimes = GetAllTimes();
            int count = 2;
            //int daycount =0;
            //this should add the title of the calendar
            /*Border titleheader = new Border();
            titleheader.BorderThickness = new Thickness(2);
            titleheader.BorderBrush = new SolidColorBrush(Colors.Black);
            titleheader.SetValue(Grid.ColumnSpanProperty, 6);

            TextBlock title = new TextBlock();
            title.FontSize = 26;
            title.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
            title.Text = "Hello World";
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.Foreground = new SolidColorBrush(Colors.Black);
            title.Name = "TableTitle";

            titleheader.Child = title;
            StudentSemesterCalendar.Children.Add(titleheader);

            while(daycount != 6)
            {
                Border dayborder = new Border();
                dayborder.BorderThickness = new Thickness(2);
                dayborder.BorderBrush = new SolidColorBrush(Colors.Black);
                dayborder.SetValue(Grid.RowProperty, 1);
                dayborder.SetValue(Grid.ColumnProperty, count);
                if(daycount == 0)
                {

                }
                else if (daycount == 1)
                {
                    TextBlock dayblock = new TextBlock();
                    dayblock.HorizontalAlignment = HorizontalAlignment.Center;
                    dayblock.VerticalAlignment = VerticalAlignment.Center;
                    dayblock.Foreground = new SolidColorBrush(Colors.Black);
                    dayblock.FontSize = 40;
                    dayblock.Text = "Monday";
                }
                else if (daycount == 2)
                {

                }
                else if (daycount == 3)
                {

                }
                else if (daycount == 4)
                {

                }
                else if (daycount == 5)
                {

                }
            }*/

            while (count != 26)
            {//creates new row
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(75);
                StudentSemesterCalendar.RowDefinitions.Add(newrow);
                for (int count2 = 0; count2 < 6; count2++)
                {
                    //creates new border
                    Border newborder = new Border();
                    newborder.BorderThickness = new Thickness(2);
                    newborder.BorderBrush = new SolidColorBrush(Colors.Black);
                    newborder.SetValue(Grid.RowProperty, count);
                    newborder.SetValue(Grid.ColumnProperty, count2);
                    if (count2 == 0)
                    {//add the time textblock to this spot
                        TextBlock txtblock = new TextBlock();
                        txtblock.FontSize = 36;
                        txtblock.Text = mytimes.Dequeue().ToString();
                        txtblock.Foreground = new SolidColorBrush(Colors.Black);
                        txtblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        txtblock.HorizontalAlignment = HorizontalAlignment.Center;
                        txtblock.VerticalAlignment = VerticalAlignment.Center;
                        newborder.Child = txtblock;
                    }
                    StudentSemesterCalendar.Children.Add(newborder);
                }
                count++;
            }
        }

        private async void SetUpSchedules()
        {
            List<string> mysemesters = new List<string>();
            List<string> mycourses = new List<string>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    { //puts info from database for semesters in a list
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT Semester FROM Semesters;";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    mysemesters.Add(reader.GetValue(0).ToString());
                                }
                            }
                        }
                        sqlConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var ermessage = new MessageDialog(ex.Message);
                await ermessage.ShowAsync();
            }

            if (mysemesters.Count == 0)
            {
                FormA2ValidSemesters.Items.Add("There Are No Semesters");
            }
            else
            {
                foreach (var semester in mysemesters)
                {
                    FormA2ValidSemesters.Items.Add(semester);
                }
            }
            if (FormA2AssignedCourses.SelectedItem == null)
            {
                FormA2AssignedCourses.Items.Add("Please Pick A Semester");
            }
        }

        private void EnrollGryffindorCancel_Click(object sender, RoutedEventArgs e)
        {
            GryffindorOptions.Visibility = Visibility.Collapsed;
        }

        private void EnableGryffindor(object sender, RoutedEventArgs e)
        {
            PurgeStudentTable();
            GryffindorOptions.Visibility = Visibility.Visible;
            GryffindorEnroll.Visibility = Visibility.Collapsed;
            StudentEnrollSchedule.Visibility = Visibility.Collapsed;
            //reset student schedule here
            resetenrollStudentschedule();
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
            string housename = "";
            if (house == 1)
            {
                housename = "Gryffindor";
            }
            else if (house == 2)
            {
                housename = "Slytherin";
            }
            else if (house == 3)
            {
                housename = "Ravenclaw";
            }
            else if (house == 4)
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
                        sqlConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new MessageDialog(ex.Message);
                await errorMessage.ShowAsync();
            }

            if (studentnames.Count == 0)
            {
                var errorMessage = new MessageDialog("Failed to get the names");
                await errorMessage.ShowAsync();
            }
            if (yearlevel.Count == 0)
            {
                var errorMessage2 = new MessageDialog("Failed to get the years");
                await errorMessage2.ShowAsync();
            }

            if (yearlevel.Count > 0 && studentnames.Count > 0)
            {
                int rowposition = 0;

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
                    btn.SetValue(NameProperty, myids[rowposition].ToString());
                    btn.Click += EnableStudentSchedule;

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
                int rowposition2 = 0;
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
                int rowposition3 = 0;
                foreach (var id in myids)
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
            }
        }

        private void EnableStudentSchedule(object sender, RoutedEventArgs e)
        {
            //resetcalendar
            GryffindorEnroll.Visibility = Visibility.Collapsed;
            StudentEnrollSchedule.Visibility = Visibility.Visible;
            Button mybutton = (sender as Button);
            StudentScheduleTitle.Text = mybutton.Content.ToString() + "'s Schedule";
            //find out if the student is using time turner here
            //Setup students schedule from the database
            if (TimeTurnerEnabler.IsChecked == true)
            {

            }
            else
            {
                
            }

        }

        private Queue<string> GetAllTimes()
        {
            Queue<string> emptyqueue = new Queue<string>();
            emptyqueue.Enqueue("0000");
            emptyqueue.Enqueue("0100");
            emptyqueue.Enqueue("0200");
            emptyqueue.Enqueue("0300");
            emptyqueue.Enqueue("0400");
            emptyqueue.Enqueue("0500");
            emptyqueue.Enqueue("0600");
            emptyqueue.Enqueue("0700");
            emptyqueue.Enqueue("0800");
            emptyqueue.Enqueue("0900");
            emptyqueue.Enqueue("1000");
            emptyqueue.Enqueue("1100");
            emptyqueue.Enqueue("1200");
            emptyqueue.Enqueue("1300");
            emptyqueue.Enqueue("1400");
            emptyqueue.Enqueue("1500");
            emptyqueue.Enqueue("1600");
            emptyqueue.Enqueue("1700");
            emptyqueue.Enqueue("1800");
            emptyqueue.Enqueue("1900");
            emptyqueue.Enqueue("2000");
            emptyqueue.Enqueue("2100");
            emptyqueue.Enqueue("2200");
            emptyqueue.Enqueue("2300");
            return emptyqueue;
        }

        private void DisenrollGryffindor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GryffindorEnrollCancel_Click(object sender, RoutedEventArgs e)
        {
            GryffindorEnroll.Visibility = Visibility.Collapsed;
            GryffindorOptions.Visibility = Visibility.Visible;
            PurgeStudentTable();
        }

        private void PurgeStudentTable()
        {
            StudentTable.RowDefinitions.Clear();
            StudentTable.Children.Clear();
        }

        private void StudentEnrollScheduleCancel_Click(object sender, RoutedEventArgs e)
        {
            StudentEnrollSchedule.Visibility = Visibility.Collapsed;
            GryffindorEnroll.Visibility = Visibility.Visible;
            //resetschedulehere
            resetenrollStudentschedule();
            TableTitle.Text = "";
            StudentSemesterCalendar.Visibility = Visibility.Collapsed;
        }

        private void resetenrollStudentschedule()
        {
            if (FormA2ValidSemesters.SelectedItem != null)
            {
                FormA2ValidSemesters.SelectedItem = null;
            }
        }

        private void SetupCourses(object sender, SelectionChangedEventArgs e)
        {//populates courses from the selected semester and shows schedule for that semester
            int _semesterID;
            List<int> mycourseIDs = new List<int>();
            List<string> coursetitles = new List<string>();
            resetcourses();
            if (FormA2ValidSemesters.SelectedItem != null)
            {
                if (FormA2ValidSemesters.SelectedValue.ToString() != "There Are No Semesters")
                {
                    StudentSemesterCalendar.Visibility = Visibility.Visible;
                    TableTitle.Text = $"Semester {FormA2ValidSemesters.SelectedValue}";
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {//acquire the semesterid from the name of the semester
                            _semesterID = GetSemesterID();
                            if (_semesterID != 0)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT CourseID FROM SemesterCourses WHERE SemesterID ={_semesterID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Int32.TryParse(reader.GetValue(0).ToString(), out int _courseID);
                                            mycourseIDs.Add(_courseID);
                                        }
                                    }
                                }
                            }
                            if (mycourseIDs.Count > 0)
                            {
                                //acquire the courses name with course ID
                                foreach (var courseID in mycourseIDs)
                                {
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT Title FROM Courses WHERE CourseID = {courseID};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                coursetitles.Add(reader.GetValue(0).ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                FormA2AssignedCourses.Items.Add("Please Assign Some Courses");
                            }
                            sqlConn.Close();
                        }
                    }
                    if (coursetitles.Count > 0)
                    {
                        foreach (var title in coursetitles)
                        {
                            FormA2AssignedCourses.Items.Add(title);
                        }
                    }
                }
            }
            else
            {
                FormA2AssignedCourses.Items.Add("Please pick a Semester");
            }
        }

        private int GetSemesterID()
        {
            int _semesterID = 0;
            string _semesterid = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the semesterid from the name of the semester
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT SemesterID FROM Semesters WHERE Semester ='{FormA2ValidSemesters.SelectedValue}';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _semesterid += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    Int32.TryParse(_semesterid, out _semesterID);
                    sqlConn.Close();
                }
            }
            return _semesterID;
        }

        private void resetcourses()
        {
            if (FormA2AssignedCourses.SelectedItem != null)
            {
                FormA2AssignedCourses.SelectedItem = null;
            }
            FormA2AssignedCourses.Items.Clear();
        }

        private async void PreviewEnrollment(object sender, SelectionChangedEventArgs e)
        {
            purgePreview();
            string _courseid = "";
            int _courseID = 0;
            int _semesterID = 0;
            List<string> days = new List<string>();
            List<string> MTimes = new List<string>();
            List<string> TuTimes = new List<string>();
            List<string> WTimes = new List<string>();
            List<string> ThTimes = new List<string>();
            List<string> FTimes = new List<string>();
            if(FormA2AssignedCourses.SelectedItem != null)
            {
                if(FormA2AssignedCourses.SelectedValue.ToString() != "Please Assign Some Courses")
                {//get the courseid from the name of the course
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT CourseID FROM Courses WHERE Title ='{FormA2AssignedCourses.SelectedValue}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        _courseid += reader.GetValue(0).ToString();
                                    }
                                }
                            }
                            Int32.TryParse(_courseid, out _courseID);
                            _semesterID = GetSemesterID();
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            { //get the days from the courseid
                                cmd.CommandText = $"SELECT DaysName FROM DayTypes WHERE CourseID = {_courseID} AND SemesterID = {_semesterID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        days.Add(reader.GetValue(0).ToString());
                                    }
                                }
                            }
                            try
                            {
                                if (days.Count > 0)
                                {//get times from the days
                                    foreach (var day in days)
                                    {
                                        if (day == "Monday")
                                        {
                                            using (SqlCommand cmd = sqlConn.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_courseID} AND SemesterID = {_semesterID} AND DaysName = 'Monday';";
                                                using (SqlDataReader reader = cmd.ExecuteReader())
                                                {
                                                    while (reader.Read())
                                                    {
                                                        MTimes.Add(reader.GetValue(0).ToString());
                                                    }
                                                }
                                            }
                                        }
                                        else if (day == "Tuesday")
                                        {
                                            using (SqlCommand cmd = sqlConn.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_courseID} AND SemesterID = {_semesterID} AND DaysName = 'Tuesday';";
                                                using (SqlDataReader reader = cmd.ExecuteReader())
                                                {
                                                    while (reader.Read())
                                                    {
                                                        TuTimes.Add(reader.GetValue(0).ToString());
                                                    }
                                                }
                                            }
                                        }
                                        else if (day == "Wednesday")
                                        {
                                            using (SqlCommand cmd = sqlConn.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_courseID} AND SemesterID = {_semesterID} AND DaysName = 'Wednesday';";
                                                using (SqlDataReader reader = cmd.ExecuteReader())
                                                {
                                                    while (reader.Read())
                                                    {
                                                        WTimes.Add(reader.GetValue(0).ToString());
                                                    }
                                                }
                                            }
                                        }
                                        else if (day == "Thursday")
                                        {
                                            using (SqlCommand cmd = sqlConn.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_courseID} AND SemesterID = {_semesterID} AND DaysName = 'Thursday';";
                                                using (SqlDataReader reader = cmd.ExecuteReader())
                                                {
                                                    while (reader.Read())
                                                    {
                                                        ThTimes.Add(reader.GetValue(0).ToString());
                                                    }
                                                }
                                            }
                                        }
                                        else if (day == "Friday")
                                        {
                                            using (SqlCommand cmd = sqlConn.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_courseID} AND SemesterID = {_semesterID} AND DaysName = 'Friday';";
                                                using (SqlDataReader reader = cmd.ExecuteReader())
                                                {
                                                    while (reader.Read())
                                                    {
                                                        FTimes.Add(reader.GetValue(0).ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }catch(Exception ex)
                            {
                                var errorMessage = new MessageDialog(ex.Message);
                                await errorMessage.ShowAsync();
                            }
                            sqlConn.Close();
                        }
                    }
                    //used the times and days list to populate green boxes on calendar or red if spot is already taken
                    if(MTimes.Count > 0)
                    {
                        foreach(var time in MTimes)
                        {
                            if(time == "0000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview0m";
                                myblock.SetValue(Grid.RowProperty, 2);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview1m";
                                myblock.SetValue(Grid.RowProperty, 3);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview2m";
                                myblock.SetValue(Grid.RowProperty, 4);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview3m";
                                myblock.SetValue(Grid.RowProperty, 5);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview4m";
                                myblock.SetValue(Grid.RowProperty, 6);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview5m";
                                myblock.SetValue(Grid.RowProperty, 7);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview6m";
                                myblock.SetValue(Grid.RowProperty, 8);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview7m";
                                myblock.SetValue(Grid.RowProperty, 9);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview8m";
                                myblock.SetValue(Grid.RowProperty, 10);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview9m";
                                myblock.SetValue(Grid.RowProperty, 11);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview10m";
                                myblock.SetValue(Grid.RowProperty, 12);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview11m";
                                myblock.SetValue(Grid.RowProperty, 13);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }else if (time == "1200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview12m";
                                myblock.SetValue(Grid.RowProperty, 14);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview13m";
                                myblock.SetValue(Grid.RowProperty, 15);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview14m";
                                myblock.SetValue(Grid.RowProperty, 16);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview15m";
                                myblock.SetValue(Grid.RowProperty, 17);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview16m";
                                myblock.SetValue(Grid.RowProperty, 18);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview17m";
                                myblock.SetValue(Grid.RowProperty, 19);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview18m";
                                myblock.SetValue(Grid.RowProperty, 20);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview19m";
                                myblock.SetValue(Grid.RowProperty, 21);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview20m";
                                myblock.SetValue(Grid.RowProperty, 22);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview21m";
                                myblock.SetValue(Grid.RowProperty, 23);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview22m";
                                myblock.SetValue(Grid.RowProperty, 24);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview23m";
                                myblock.SetValue(Grid.RowProperty, 25);
                                myblock.SetValue(Grid.ColumnProperty, 1);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                        }
                    }
                    if(TuTimes.Count > 0)
                    {
                        foreach (var time in TuTimes)
                        {
                            if (time == "0000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview0tu";
                                myblock.SetValue(Grid.RowProperty, 2);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview1tu";
                                myblock.SetValue(Grid.RowProperty, 3);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview2tu";
                                myblock.SetValue(Grid.RowProperty, 4);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview3tu";
                                myblock.SetValue(Grid.RowProperty, 5);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview4tu";
                                myblock.SetValue(Grid.RowProperty, 6);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview5tu";
                                myblock.SetValue(Grid.RowProperty, 7);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview6t";
                                myblock.SetValue(Grid.RowProperty, 8);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview7tu";
                                myblock.SetValue(Grid.RowProperty, 9);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview8tu";
                                myblock.SetValue(Grid.RowProperty, 10);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview9tu";
                                myblock.SetValue(Grid.RowProperty, 11);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview10tu";
                                myblock.SetValue(Grid.RowProperty, 12);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview11tu";
                                myblock.SetValue(Grid.RowProperty, 13);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview12tu";
                                myblock.SetValue(Grid.RowProperty, 14);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview13tu";
                                myblock.SetValue(Grid.RowProperty, 15);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview14tu";
                                myblock.SetValue(Grid.RowProperty, 16);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview15tu";
                                myblock.SetValue(Grid.RowProperty, 17);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview16tu";
                                myblock.SetValue(Grid.RowProperty, 18);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview17tu";
                                myblock.SetValue(Grid.RowProperty, 19);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview18tu";
                                myblock.SetValue(Grid.RowProperty, 20);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview19tu";
                                myblock.SetValue(Grid.RowProperty, 21);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview20tu";
                                myblock.SetValue(Grid.RowProperty, 22);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview21tu";
                                myblock.SetValue(Grid.RowProperty, 23);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview22tu";
                                myblock.SetValue(Grid.RowProperty, 24);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview23tu";
                                myblock.SetValue(Grid.RowProperty, 25);
                                myblock.SetValue(Grid.ColumnProperty, 2);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                        }
                    }
                    if(WTimes.Count > 0)
                    {
                        foreach (var time in WTimes)
                        {
                            if (time == "0000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview0w";
                                myblock.SetValue(Grid.RowProperty, 2);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview1w";
                                myblock.SetValue(Grid.RowProperty, 3);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview2w";
                                myblock.SetValue(Grid.RowProperty, 4);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview3w";
                                myblock.SetValue(Grid.RowProperty, 5);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview4w";
                                myblock.SetValue(Grid.RowProperty, 6);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview5w";
                                myblock.SetValue(Grid.RowProperty, 7);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview6w";
                                myblock.SetValue(Grid.RowProperty, 8);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview7w";
                                myblock.SetValue(Grid.RowProperty, 9);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview8w";
                                myblock.SetValue(Grid.RowProperty, 10);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview9w";
                                myblock.SetValue(Grid.RowProperty, 11);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview10w";
                                myblock.SetValue(Grid.RowProperty, 12);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview11w";
                                myblock.SetValue(Grid.RowProperty, 13);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview12w";
                                myblock.SetValue(Grid.RowProperty, 14);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview13w";
                                myblock.SetValue(Grid.RowProperty, 15);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview14w";
                                myblock.SetValue(Grid.RowProperty, 16);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview15w";
                                myblock.SetValue(Grid.RowProperty, 17);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview16w";
                                myblock.SetValue(Grid.RowProperty, 18);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview17w";
                                myblock.SetValue(Grid.RowProperty, 19);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview18w";
                                myblock.SetValue(Grid.RowProperty, 20);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview19w";
                                myblock.SetValue(Grid.RowProperty, 21);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview20w";
                                myblock.SetValue(Grid.RowProperty, 22);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview21w";
                                myblock.SetValue(Grid.RowProperty, 23);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview22w";
                                myblock.SetValue(Grid.RowProperty, 24);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview23w";
                                myblock.SetValue(Grid.RowProperty, 25);
                                myblock.SetValue(Grid.ColumnProperty, 3);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                        }
                    }
                    if(ThTimes.Count > 0)
                    {
                        foreach (var time in ThTimes)
                        {
                            if (time == "0000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 2);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 3);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 4);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 5);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 6);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 7);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 8);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 9);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 10);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 11);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 12);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 13);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 14);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 15);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 16);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 17);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 18);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 19);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 20);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 21);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 22);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 23);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 24);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 25);
                                myblock.SetValue(Grid.ColumnProperty, 4);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                        }
                    }
                    if(FTimes.Count > 0)
                    {
                        foreach (var time in FTimes)
                        {
                            if (time == "0000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 2);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 3);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 4);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 5);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 6);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 7);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 8);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 9);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 10);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "0900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 11);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 12);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 13);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 14);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 15);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1400")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 16);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1500")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 17);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1600")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 18);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1700")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 19);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1800")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 20);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "1900")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 21);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2000")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 22);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2100")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 23);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2200")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 24);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                            else if (time == "2300")
                            {
                                Border myblock = new Border();
                                myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                myblock.Name = "preview";
                                myblock.SetValue(Grid.RowProperty, 25);
                                myblock.SetValue(Grid.ColumnProperty, 5);
                                StudentSemesterCalendar.Children.Add(myblock);
                            }
                        }
                    }
                }
            }
        }

        private void purgePreview()
        {
            foreach (Border item in StudentSemesterCalendar.Children)
            {
                foreach (Border item2 in StudentSemesterCalendar.Children)
                {
                    if (item2.Name != "")
                    {
                        StudentSemesterCalendar.Children.Remove(item2);
                    }
                }
                if(item.Name != "")
                {
                    StudentSemesterCalendar.Children.Remove(item);
                }
            }
        }
    }
}
