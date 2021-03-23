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
        private bool ValidEnrollment = true;
        List<string> days = new List<string>();
        List<string> MTimes = new List<string>();
        List<string> TuTimes = new List<string>();
        List<string> WTimes = new List<string>();
        List<string> ThTimes = new List<string>();
        List<string> FTimes = new List<string>();
        private int SelectedStudentHUID;
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
                    else
                    {//adds an empty course value block to every other spot
                        TextBlock courseblock = new TextBlock();
                        courseblock.FontSize = 36;
                        courseblock.Name = "CalendarDayBlockValue";
                        courseblock.Text = "";
                        courseblock.Foreground = new SolidColorBrush(Colors.Black);
                        courseblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        courseblock.HorizontalAlignment = HorizontalAlignment.Center;
                        courseblock.VerticalAlignment = VerticalAlignment.Center;
                        newborder.Child = courseblock;
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
            resetenrollStudentschedule();
            TableTitle.Text = "";
            StudentSemesterCalendar.Visibility = Visibility.Collapsed;
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
                int rowposition2 = 0;
                foreach (var year in yearlevel)
                {
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
                int rowposition3 = 0;
                foreach (var id in myids)
                {
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
            Int32.TryParse(mybutton.Name, out SelectedStudentHUID);
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

        private async void SetupCourses(object sender, SelectionChangedEventArgs e)
        {//populates courses from the selected semester and shows schedule for that semester
            List<int> CurrentlyEnrolledCourseIDs = new List<int>();//for updateing prior enrolled courses
            List<bool> TimeTableflag = new List<bool>();//for updating prior enrolled courses
            List<string> TotalEnrolledCourseTimes = new List<string>();
            string Enrolledresults = "";

            int _semesterID;
            List<int> mycourseIDs = new List<int>();//for course combo box
            List<string> coursetitles = new List<string>();//for course combo box
            resetcourses();
            if (FormA2ValidSemesters.SelectedItem != null)
            {
                if (FormA2ValidSemesters.SelectedValue.ToString() != "There Are No Semesters")
                {
                    StudentSemesterCalendar.Visibility = Visibility.Visible;
                    TableTitle.Text = $"Semester {FormA2ValidSemesters.SelectedValue}";
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {//updates the combo box for the courses for the selected semester
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
                            //Updates the students calendar for the selected semester
                            //find out the courses the student is currently enrolled in
                            try
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all courseids for selected semester for this particular student
                                    cmd.CommandText = $"SELECT CourseID,TimeTurnerFlag FROM StudentEnrolledCourses WHERE StudentID = {SelectedStudentHUID} AND SemesterID = {_semesterID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            CurrentlyEnrolledCourseIDs.Add((int)reader.GetValue(0));
                                            TimeTableflag.Add((bool)reader.GetValue(1));
                                        }
                                    }
                                }
                                if (CurrentlyEnrolledCourseIDs.Count > 0 && TimeTableflag.Count > 0)
                                {//found courses the student already enrolled in
                                    foreach (var item in CurrentlyEnrolledCourseIDs)
                                    {
                                        using (SqlCommand cmd = sqlConn.CreateCommand())
                                        {//gets the course names and types for each courseid 
                                            cmd.CommandText = $"SELECT Title, CourseType FROM Courses WHERE CourseID = {item};";
                                            using (SqlDataReader reader = cmd.ExecuteReader())
                                            {
                                                while (reader.Read())
                                                {
                                                    Enrolledresults += (reader.GetValue(0).ToString())+" ";
                                                    Enrolledresults += (reader.GetValue(1).ToString())+"\n";
                                                }
                                            }
                                        }
                                        using (SqlCommand cmd = sqlConn.CreateCommand())
                                        {//gets the times and day for each courseid
                                            cmd.CommandText = $"SELECT DaysName,Times FROM Times WHERE SemesterID ={_semesterID} AND CourseID = {item};";
                                            using (SqlDataReader reader = cmd.ExecuteReader())
                                            {
                                                while (reader.Read())
                                                {
                                                    Enrolledresults += reader.GetValue(0).ToString()+" ";
                                                    Enrolledresults += reader.GetValue(1).ToString()+"\n";
                                                }
                                            }
                                        }
                                        TotalEnrolledCourseTimes.Add(Enrolledresults);
                                        Enrolledresults = "";
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                var errorMessage = new MessageDialog(ex.Message);
                                await errorMessage.ShowAsync();
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
                    if(CurrentlyEnrolledCourseIDs.Count > 0)
                    {
                        foreach (var course in TotalEnrolledCourseTimes)
                        {//we have to parse this message
                            var Message = new MessageDialog(course);
                            await Message.ShowAsync();
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

        private void PreviewEnrollment(object sender, SelectionChangedEventArgs e)
        {
            resetListEnrollments();
            purgePreview();
            int _CID;
            int _SID;
            if (FormA2AssignedCourses.SelectedItem != null)
            {
                if (FormA2AssignedCourses.SelectedValue.ToString() != "Please Assign Some Courses")
                {//get the courseid from the name of the course
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            _CID = getCourseID();
                            _SID = GetSemesterID();
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            { //get the days from the courseid
                                cmd.CommandText = $"SELECT DaysName FROM DayTypes WHERE CourseID = {_CID} AND SemesterID = {_SID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        days.Add(reader.GetValue(0).ToString());
                                    }
                                }
                            }
                            if (days.Count > 0)
                            {//get times from the days might be able to make this into a function
                                foreach (var day in days)
                                {
                                    if (day == "Monday")
                                    {
                                        using (SqlCommand cmd = sqlConn.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_CID} AND SemesterID = {_SID} AND DaysName = 'Monday';";
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
                                            cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_CID} AND SemesterID = {_SID} AND DaysName = 'Tuesday';";
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
                                            cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_CID} AND SemesterID = {_SID} AND DaysName = 'Wednesday';";
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
                                            cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_CID} AND SemesterID = {_SID} AND DaysName = 'Thursday';";
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
                                            cmd.CommandText = $"SELECT Times FROM Times WHERE CourseID = {_CID} AND SemesterID = {_SID} AND DaysName = 'Friday';";
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
                            sqlConn.Close();
                        }
                    }
                    //used the times and days list to populate green boxes on calendar or red if spot is already taken
                    if (MTimes.Count > 0)
                    {
                        foreach (var time in MTimes)
                        {
                            if (time == "0000")
                            {
                                CreatePreviewBlock(2, 1, "preview");
                            }
                            else if (time == "0100")
                            {
                                CreatePreviewBlock(3, 1, "preview");
                            }
                            else if (time == "0200")
                            {
                                CreatePreviewBlock(4, 1, "preview");
                            }
                            else if (time == "0300")
                            {
                                CreatePreviewBlock(5, 1, "preview");
                            }
                            else if (time == "0400")
                            {
                                CreatePreviewBlock(6, 1, "preview");
                            }
                            else if (time == "0500")
                            {
                                CreatePreviewBlock(7, 1, "preview");
                            }
                            else if (time == "0600")
                            {
                                CreatePreviewBlock(8, 1, "preview");
                            }
                            else if (time == "0700")
                            {
                                CreatePreviewBlock(9, 1, "preview");
                            }
                            else if (time == "0800")
                            {
                                CreatePreviewBlock(10, 1, "preview");
                            }
                            else if (time == "0900")
                            {
                                CreatePreviewBlock(11, 1, "preview");
                            }
                            else if (time == "1000")
                            {
                                CreatePreviewBlock(12, 1, "preview");
                            }
                            else if (time == "1100")
                            {
                                CreatePreviewBlock(13, 1, "preview");
                            }
                            else if (time == "1200")
                            {
                                CreatePreviewBlock(14, 1, "preview");
                            }
                            else if (time == "1300")
                            {
                                CreatePreviewBlock(15, 1, "preview");
                            }
                            else if (time == "1400")
                            {
                                CreatePreviewBlock(16, 1, "preview");
                            }
                            else if (time == "1500")
                            {
                                CreatePreviewBlock(17, 1, "preview");
                            }
                            else if (time == "1600")
                            {
                                CreatePreviewBlock(18, 1, "preview");
                            }
                            else if (time == "1700")
                            {
                                CreatePreviewBlock(19, 1, "preview");
                            }
                            else if (time == "1800")
                            {
                                CreatePreviewBlock(20, 1, "preview");
                            }
                            else if (time == "1900")
                            {
                                CreatePreviewBlock(21, 1, "preview");
                            }
                            else if (time == "2000")
                            {
                                CreatePreviewBlock(22, 1, "preview");
                            }
                            else if (time == "2100")
                            {
                                CreatePreviewBlock(23, 1, "preview");
                            }
                            else if (time == "2200")
                            {
                                CreatePreviewBlock(24, 1, "preview");
                            }
                            else if (time == "2300")
                            {
                                CreatePreviewBlock(25, 1, "preview");
                            }
                        }
                    }
                    if (TuTimes.Count > 0)
                    {
                        foreach (var time in TuTimes)
                        {
                            if (time == "0000")
                            {
                                CreatePreviewBlock(2, 2, "preview");
                            }
                            else if (time == "0100")
                            {
                                CreatePreviewBlock(3, 2, "preview");
                            }
                            else if (time == "0200")
                            {
                                CreatePreviewBlock(4, 2, "preview");
                            }
                            else if (time == "0300")
                            {
                                CreatePreviewBlock(5, 2, "preview");
                            }
                            else if (time == "0400")
                            {
                                CreatePreviewBlock(6, 2, "preview");
                            }
                            else if (time == "0500")
                            {
                                CreatePreviewBlock(7, 2, "preview");
                            }
                            else if (time == "0600")
                            {
                                CreatePreviewBlock(8, 2, "preview");
                            }
                            else if (time == "0700")
                            {
                                CreatePreviewBlock(9, 2, "preview");
                            }
                            else if (time == "0800")
                            {
                                CreatePreviewBlock(10, 2, "preview");
                            }
                            else if (time == "0900")
                            {
                                CreatePreviewBlock(11, 2, "preview");
                            }
                            else if (time == "1000")
                            {
                                CreatePreviewBlock(12, 2, "preview");
                            }
                            else if (time == "1100")
                            {
                                CreatePreviewBlock(13, 2, "preview");
                            }
                            else if (time == "1200")
                            {
                                CreatePreviewBlock(14, 2, "preview");
                            }
                            else if (time == "1300")
                            {
                                CreatePreviewBlock(15, 2, "preview");
                            }
                            else if (time == "1400")
                            {
                                CreatePreviewBlock(16, 2, "preview");
                            }
                            else if (time == "1500")
                            {
                                CreatePreviewBlock(17, 2, "preview");
                            }
                            else if (time == "1600")
                            {
                                CreatePreviewBlock(18, 2, "preview");
                            }
                            else if (time == "1700")
                            {
                                CreatePreviewBlock(19, 2, "preview");
                            }
                            else if (time == "1800")
                            {
                                CreatePreviewBlock(20, 2, "preview");
                            }
                            else if (time == "1900")
                            {
                                CreatePreviewBlock(21, 2, "preview");
                            }
                            else if (time == "2000")
                            {
                                CreatePreviewBlock(22, 2, "preview");
                            }
                            else if (time == "2100")
                            {
                                CreatePreviewBlock(23, 2, "preview");
                            }
                            else if (time == "2200")
                            {
                                CreatePreviewBlock(24, 2, "preview");
                            }
                            else if (time == "2300")
                            {
                                CreatePreviewBlock(25, 2, "preview");
                            }
                        }
                    }
                    if (WTimes.Count > 0)
                    {
                        foreach (var time in WTimes)
                        {
                            if (time == "0000")
                            {
                                CreatePreviewBlock(2, 3, "preview");
                            }
                            else if (time == "0100")
                            {
                                CreatePreviewBlock(3, 3, "preview");
                            }
                            else if (time == "0200")
                            {
                                CreatePreviewBlock(4, 3, "preview");
                            }
                            else if (time == "0300")
                            {
                                CreatePreviewBlock(5, 3, "preview");
                            }
                            else if (time == "0400")
                            {
                                CreatePreviewBlock(6, 3, "preview");
                            }
                            else if (time == "0500")
                            {
                                CreatePreviewBlock(7, 3, "preview");
                            }
                            else if (time == "0600")
                            {
                                CreatePreviewBlock(8, 3, "preview");
                            }
                            else if (time == "0700")
                            {
                                CreatePreviewBlock(9, 3, "preview");
                            }
                            else if (time == "0800")
                            {
                                CreatePreviewBlock(10, 3, "preview");
                            }
                            else if (time == "0900")
                            {
                                CreatePreviewBlock(11, 3, "preview");
                            }
                            else if (time == "1000")
                            {
                                CreatePreviewBlock(12, 3, "preview");
                            }
                            else if (time == "1100")
                            {
                                CreatePreviewBlock(13, 3, "preview");
                            }
                            else if (time == "1200")
                            {
                                CreatePreviewBlock(14, 3, "preview");
                            }
                            else if (time == "1300")
                            {
                                CreatePreviewBlock(15, 3, "preview");
                            }
                            else if (time == "1400")
                            {
                                CreatePreviewBlock(16, 3, "preview");
                            }
                            else if (time == "1500")
                            {
                                CreatePreviewBlock(17, 3, "preview");
                            }
                            else if (time == "1600")
                            {
                                CreatePreviewBlock(18, 3, "preview");
                            }
                            else if (time == "1700")
                            {
                                CreatePreviewBlock(19, 3, "preview");
                            }
                            else if (time == "1800")
                            {
                                CreatePreviewBlock(20, 3, "preview");
                            }
                            else if (time == "1900")
                            {
                                CreatePreviewBlock(21, 3, "preview");
                            }
                            else if (time == "2000")
                            {
                                CreatePreviewBlock(22, 3, "preview");
                            }
                            else if (time == "2100")
                            {
                                CreatePreviewBlock(23, 3, "preview");
                            }
                            else if (time == "2200")
                            {
                                CreatePreviewBlock(24, 3, "preview");
                            }
                            else if (time == "2300")
                            {
                                CreatePreviewBlock(25, 3, "preview");
                            }
                        }
                    }
                    if (ThTimes.Count > 0)
                    {
                        foreach (var time in ThTimes)
                        {
                            if (time == "0000")
                            {
                                CreatePreviewBlock(2, 4, "preview");
                            }
                            else if (time == "0100")
                            {
                                CreatePreviewBlock(3, 4, "preview");
                            }
                            else if (time == "0200")
                            {
                                CreatePreviewBlock(4, 4, "preview");
                            }
                            else if (time == "0300")
                            {
                                CreatePreviewBlock(5, 4, "preview");
                            }
                            else if (time == "0400")
                            {
                                CreatePreviewBlock(6, 4, "preview");
                            }
                            else if (time == "0500")
                            {
                                CreatePreviewBlock(7, 4, "preview");
                            }
                            else if (time == "0600")
                            {
                                CreatePreviewBlock(8, 4, "preview");
                            }
                            else if (time == "0700")
                            {
                                CreatePreviewBlock(9, 4, "preview");
                            }
                            else if (time == "0800")
                            {
                                CreatePreviewBlock(10, 4, "preview");
                            }
                            else if (time == "0900")
                            {
                                CreatePreviewBlock(11, 4, "preview");
                            }
                            else if (time == "1000")
                            {
                                CreatePreviewBlock(12, 4, "preview");
                            }
                            else if (time == "1100")
                            {
                                CreatePreviewBlock(13, 4, "preview");
                            }
                            else if (time == "1200")
                            {
                                CreatePreviewBlock(14, 4, "preview");
                            }
                            else if (time == "1300")
                            {
                                CreatePreviewBlock(15, 4, "preview");
                            }
                            else if (time == "1400")
                            {
                                CreatePreviewBlock(16, 4, "preview");
                            }
                            else if (time == "1500")
                            {
                                CreatePreviewBlock(17, 4, "preview");
                            }
                            else if (time == "1600")
                            {
                                CreatePreviewBlock(18, 4, "preview");
                            }
                            else if (time == "1700")
                            {
                                CreatePreviewBlock(19, 4, "preview");
                            }
                            else if (time == "1800")
                            {
                                CreatePreviewBlock(20, 4, "preview");
                            }
                            else if (time == "1900")
                            {
                                CreatePreviewBlock(21, 4, "preview");
                            }
                            else if (time == "2000")
                            {
                                CreatePreviewBlock(22, 4, "preview");
                            }
                            else if (time == "2100")
                            {
                                CreatePreviewBlock(23, 4, "preview");
                            }
                            else if (time == "2200")
                            {
                                CreatePreviewBlock(24, 4, "preview");
                            }
                            else if (time == "2300")
                            {
                                CreatePreviewBlock(25, 4, "preview");
                            }
                        }
                    }
                    if (FTimes.Count > 0)
                    {
                        foreach (var time in FTimes)
                        {
                            if (time == "0000")
                            {
                                CreatePreviewBlock(2, 5, "preview");
                            }
                            else if (time == "0100")
                            {
                                CreatePreviewBlock(3, 5, "preview");
                            }
                            else if (time == "0200")
                            {
                                CreatePreviewBlock(4, 5, "preview");
                            }
                            else if (time == "0300")
                            {
                                CreatePreviewBlock(5, 5, "preview");
                            }
                            else if (time == "0400")
                            {
                                CreatePreviewBlock(6, 5, "preview");
                            }
                            else if (time == "0500")
                            {
                                CreatePreviewBlock(7, 5, "preview");
                            }
                            else if (time == "0600")
                            {
                                CreatePreviewBlock(8, 5, "preview");
                            }
                            else if (time == "0700")
                            {
                                CreatePreviewBlock(9, 5, "preview");
                            }
                            else if (time == "0800")
                            {
                                CreatePreviewBlock(10, 5, "preview");
                            }
                            else if (time == "0900")
                            {
                                CreatePreviewBlock(11, 5, "preview");
                            }
                            else if (time == "1000")
                            {
                                CreatePreviewBlock(12, 5, "preview");
                            }
                            else if (time == "1100")
                            {
                                CreatePreviewBlock(13, 5, "preview");
                            }
                            else if (time == "1200")
                            {
                                CreatePreviewBlock(14, 5, "preview");
                            }
                            else if (time == "1300")
                            {
                                CreatePreviewBlock(15, 5, "preview");
                            }
                            else if (time == "1400")
                            {
                                CreatePreviewBlock(16, 5, "preview");
                            }
                            else if (time == "1500")
                            {
                                CreatePreviewBlock(17, 5, "preview");
                            }
                            else if (time == "1600")
                            {
                                CreatePreviewBlock(18, 5, "preview");
                            }
                            else if (time == "1700")
                            {
                                CreatePreviewBlock(19, 5, "preview");
                            }
                            else if (time == "1800")
                            {
                                CreatePreviewBlock(20, 5, "preview");
                            }
                            else if (time == "1900")
                            {
                                CreatePreviewBlock(21, 5, "preview");
                            }
                            else if (time == "2000")
                            {
                                CreatePreviewBlock(22, 5, "preview");
                            }
                            else if (time == "2100")
                            {
                                CreatePreviewBlock(23, 5, "preview");
                            }
                            else if (time == "2200")
                            {
                                CreatePreviewBlock(24, 5, "preview");
                            }
                            else if (time == "2300")
                            {
                                CreatePreviewBlock(25, 5, "preview");
                            }
                        }
                    }
                }
            }
        }

        private int getCourseID()
        {
            string _courseid = "";
            int _courseID;
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
                }
                sqlConn.Close();
            }
            Int32.TryParse(_courseid, out _courseID);
            return _courseID;
        }

        private void resetListEnrollments()
        {
            if (days.Count > 0)
            {
                days.Clear();
            }
            if (MTimes.Count > 0)
            {
                MTimes.Clear();
            }
            if (TuTimes.Count > 0)
            {
                TuTimes.Clear();
            }
            if (WTimes.Count > 0)
            {
                WTimes.Clear();
            }
            if (ThTimes.Count > 0)
            {
                ThTimes.Clear();
            }
            if (FTimes.Count > 0)
            {
                FTimes.Clear();
            }
        }

        private void CreatePreviewBlock(int row, int column, string name)
        {
            Border myblock = new Border();
            myblock.Background = new SolidColorBrush(Colors.LightGreen);
            myblock.Name = name;
            myblock.SetValue(Grid.RowProperty, row);
            myblock.SetValue(Grid.ColumnProperty, column);
            /* foreach (TextBlock text in StudentSemesterCalendar.Children)
             {

             }*/
            StudentSemesterCalendar.Children.Add(myblock);
        }

        private void purgePreview()
        {
            foreach (Border item in StudentSemesterCalendar.Children)
            {
                foreach (Border item2 in StudentSemesterCalendar.Children)
                {
                    if (item2.Name == "preview")
                    {
                        StudentSemesterCalendar.Children.Remove(item2);
                    }
                }
            }
        }

        private async void StudentEnrollStudent_Click(object sender, RoutedEventArgs e)
        {//checks the input is valid
            int notvalidenroll = 0;
            string notvalidenrollmessage = "";
            int timeturner;
            int SemID = GetSemesterID();
            int CrsID = getCourseID();
            if (FormA2ValidSemesters.SelectedItem == null)
            {
                notvalidenrollmessage += "Please Select a Semester\n";
                notvalidenroll++;
            }
            else if (FormA2ValidSemesters.SelectedValue.ToString() == "There Are No Semesters")
            {
                notvalidenrollmessage += "Please add semesters \n";
                notvalidenroll++;
            }
            if (FormA2AssignedCourses.SelectedItem == null)
            {
                notvalidenrollmessage += "Please Select a Course\n";
                notvalidenroll++;
            }
            else if (FormA2AssignedCourses.SelectedValue.ToString() == "Please Assign Some Courses")
            {//this might be a duplicate
                notvalidenrollmessage += "Please Assign Some Courses\n";
                notvalidenroll++;
            }
            if (TimeTurnerEnabler.IsChecked == true)
            {
                timeturner = 1;
            }
            else
            {
                timeturner = 0;
            }

            if (notvalidenroll == 0)
            {
                if (ValidEnrollment == true)
                {//insert the selected day,time,course, and semester to StudentEnrolledCourses
                    try
                    {
                        using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                        {
                            sqlConn.Open();
                            if (sqlConn.State == System.Data.ConnectionState.Open)
                            {//convert HUID to StudentID
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"INSERT INTO StudentEnrolledCourses VALUES ({SemID},{CrsID},{SelectedStudentHUID},{timeturner});", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();

                                sqlConn.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = new MessageDialog(ex.Message);
                        await error.ShowAsync();
                    }

                    //tell the user the insert was success and move them to reload the page
                }
                else
                {

                }
            }
            else
            {
                var NotValidMessage = new MessageDialog(notvalidenrollmessage);
                await NotValidMessage.ShowAsync();
            }
        }
    }
}
