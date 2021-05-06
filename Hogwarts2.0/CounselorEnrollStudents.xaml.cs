using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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
        private bool TimeTurnerSafety = false;
        private int SelectedStudentHUID;
        private bool HasNormalCourses;
        private int TTMondayCount = 2;
        private int TTTuesdayCount = 2;
        private int TTWednesdayCount = 2;
        private int TTThursdayCount = 2;
        private int TTFridayCount = 2;
        private int TTTotoalRowCount;
        private string SelectedHouse;
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
                        courseblock.Name = $"{count2}{count}";//sets name of txtblock to the position in the table
                        courseblock.FontSize = 36;
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
                FormStudentDisenrollValidSemesters.Items.Add("There Are No Semesters");
            }
            else
            {
                foreach (var semester in mysemesters)
                {
                    FormA2ValidSemesters.Items.Add(semester);
                    FormStudentDisenrollValidSemesters.Items.Add(semester);
                }
            }
            if (FormA2AssignedCourses.SelectedItem == null)
            {
                FormA2AssignedCourses.Items.Add("Please Pick A Semester");
            }
            if (FormStudentDisenrollAssignedCourses.SelectedItem == null)
            {
                FormStudentDisenrollAssignedCourses.Items.Add("Please Pick A Semester");
            }
        }
        private void EnrollHouseCancel_Click(object sender, RoutedEventArgs e)
        {
            HouseOptions.Visibility = Visibility.Collapsed;
        }
        private void EnableHouseOptions(object sender, RoutedEventArgs e)
        {
            SelectedHouse = "";
            PurgeStudentTable();
            if ((sender as Button).Name == "Gryffindor")
            {
                HouseOptionsTitle.Text = "Gryffindor Managment";
                SelectedHouse = "Gryffindor";
            }
            else if ((sender as Button).Name == "Slytherin")
            {
                HouseOptionsTitle.Text = "Slytherin Management";
                SelectedHouse = "Slytherin";
            }
            else if ((sender as Button).Name == "Ravenclaw")
            {
                HouseOptionsTitle.Text = "Ravenclaw Management";
                SelectedHouse = "Ravenclaw";
            }
            else if ((sender as Button).Name == "Hufflepuff")
            {
                HouseOptionsTitle.Text = "Hufflepuff Management";
                SelectedHouse = "Hufflepuff";
            }
            HouseOptions.Visibility = Visibility.Visible;
            HouseEnroll.Visibility = Visibility.Collapsed;
            HouseDisenroll.Visibility = Visibility.Collapsed;
            StudentEnrollSchedule.Visibility = Visibility.Collapsed;
            StudentDisenrollCourse.Visibility = Visibility.Collapsed;
            //reset student schedule here
            if (ScrollveiwerNormal.Visibility == Visibility.Visible)
            {
                ScrollveiwerNormal.Visibility = Visibility.Collapsed;
            }
            if (ScrollVeiwerTimeTurner.Visibility == Visibility.Visible)
            {
                ScrollVeiwerTimeTurner.Visibility = Visibility.Collapsed;
            }
            resetTimeTurner();
            resetenrollStudentschedule();
            resetfilter();
            TableTitle.Text = "";
        }

        private void resetfilter()
        {
            HouseFilterEnrollOptions.Visibility = Visibility.Collapsed;
            FilterbyAlph.IsChecked = default;
            FilterbyReg.IsChecked = default;
            YearlevelInput.SelectedValue = "All years";

            HouseFilterDisnerollOptions.Visibility = Visibility.Collapsed;
            DisenrollFilterbyReg.IsChecked = default;
            DisenrollFilterbyAlph.IsChecked = default;
            DisenrollYearlevelInput.SelectedValue = "All years";
        }

        private void EnrollSelectedHouse_Click(object sender, RoutedEventArgs e)
        {
            HouseEnroll.Visibility = Visibility.Visible;
            HouseOptions.Visibility = Visibility.Collapsed;
            YearlevelInput.SelectedValue = "All years";
            FilterbyReg.IsChecked = true;
        }
        private async void PopulateTableAll(int house, string mode, int yearfilter, string modetype)
        {
            List<int> UnfilteredyearIDs = new List<int>();
            List<int> sortedIDs = new List<int>();
            List<string> studentnames = new List<string>();
            List<int> yearlevel = new List<int>();
            List<int> myids = new List<int>();
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
            else
            {
                housename = "Hufflepuff";
            }
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {//get all student ids in selected house
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
                            if (yearfilter == 0)
                            {//do this if the year filter is default
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
                                if (mode == "default")
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
                                    studentnames.Sort();
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {//get all names from the acquired ids
                                        foreach (var name in studentnames)
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
                            {// do this if the user selects a year filter
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all ids from the selected year
                                    cmd.CommandText = $"SELECT HUID FROM Students WHERE StudentYear = {yearfilter};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            UnfilteredyearIDs.Add((int)reader.GetValue(0));
                                        }
                                    }
                                }
                                foreach (var unfilteredid in UnfilteredyearIDs.ToList())
                                {//filter the yearIDS to the studentIDs in the selected house
                                    if (myids.Contains(unfilteredid) == false)
                                    {
                                        UnfilteredyearIDs.Remove(unfilteredid);
                                    }
                                }
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get all names from the acquired ids
                                    foreach (var id in UnfilteredyearIDs)
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
                                if (FilterbyReg.IsChecked == true)
                                {//do this if they choose to filter by HUID
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {//get all year levels from acquired ids
                                        foreach (var id in UnfilteredyearIDs)
                                        {//ensures we only grab the student years from the selected year filter
                                            cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id} AND StudentYear = {yearfilter};";
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
                                    studentnames.Sort();
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {//get all names from the acquired ids
                                        foreach (var name in studentnames)
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
                                        foreach (var id in UnfilteredyearIDs)
                                        {//double tap by ensuring we select by specified student year
                                            cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id} AND StudentYear = {yearfilter};";
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
                    if (modetype == "enroll")
                    {
                        StudentTable.RowDefinitions.Add(row);
                    }
                    else if (modetype == "disenroll")
                    {
                        StudentTableA3.RowDefinitions.Add(row);
                    }
                    Button btn = new Button();
                    btn.FontSize = 36;
                    btn.Content = student.ToString();
                    btn.Foreground = new SolidColorBrush(Colors.Black);
                    btn.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    if (mode == "default" && yearfilter == 0)//sets up the name of the button with the matching ID based on the filter applied
                    {
                        btn.SetValue(NameProperty, myids[rowposition].ToString());
                    }
                    else if (mode == "alph" && yearfilter == 0)
                    {
                        btn.SetValue(NameProperty, sortedIDs[rowposition].ToString());
                    }
                    else if (mode == "default" && yearfilter != 0)
                    {
                        btn.SetValue(NameProperty, UnfilteredyearIDs[rowposition].ToString());
                    }
                    else if (mode == "alph" && yearfilter != 0)
                    {
                        btn.SetValue(NameProperty, sortedIDs[rowposition].ToString());
                    }
                    if (modetype == "enroll")
                    {//sets button for enroll
                        btn.Click += EnableStudentSchedule;
                    }
                    else if (modetype == "disenroll")
                    {
                        btn.Click += EnableDisenrollCourse;
                    }
                    Border myborder = new Border();
                    myborder.BorderThickness = new Thickness(2);
                    myborder.BorderBrush = new SolidColorBrush(Colors.Black);
                    myborder.SetValue(Grid.RowProperty, rowposition);
                    myborder.SetValue(Grid.ColumnProperty, 0);
                    myborder.Child = btn;
                    if (modetype == "enroll")
                    {//adds border button to enroll table 
                        StudentTable.Children.Add(myborder);
                    }
                    else if (modetype == "disenroll")
                    {
                        StudentTableA3.Children.Add(myborder);
                    }
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
                    if (modetype == "enroll")
                    {
                        StudentTable.Children.Add(myborder);
                    }
                    else if (modetype == "disenroll")
                    {
                        StudentTableA3.Children.Add(myborder);
                    }
                    rowposition2++;
                }
            }
        }

        private void EnableDisenrollCourse(object sender, RoutedEventArgs e)
        {
            HouseDisenroll.Visibility = Visibility.Collapsed;
            HouseFilterDisnerollOptions.Visibility = Visibility.Collapsed;
            StudentDisenrollCourse.Visibility = Visibility.Visible;
            Button mybutton = sender as Button;
            Int32.TryParse(mybutton.Name, out SelectedStudentHUID);
            StudentDisenrollTitle.Text = mybutton.Content.ToString();
        }

        private void EnableStudentSchedule(object sender, RoutedEventArgs e)
        {
            HouseEnroll.Visibility = Visibility.Collapsed;
            HouseFilterEnrollOptions.Visibility = Visibility.Collapsed;
            StudentEnrollSchedule.Visibility = Visibility.Visible;
            Button mybutton = sender as Button;
            Int32.TryParse(mybutton.Name, out SelectedStudentHUID);
            StudentScheduleTitle.Text = mybutton.Content.ToString() + "'s Schedule";
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
        private void DisenrollSelectedHouse_Click(object sender, RoutedEventArgs e)
        {
            HouseDisenroll.Visibility = Visibility.Visible;
            HouseOptions.Visibility = Visibility.Collapsed;
            DisenrollYearlevelInput.SelectedValue = "All years";
            DisenrollFilterbyReg.IsChecked = true;
        }
        private void SelectedHouseEnrollCancel_Click(object sender, RoutedEventArgs e)
        {
            HouseEnroll.Visibility = Visibility.Collapsed;
            HouseOptions.Visibility = Visibility.Visible;
            PurgeStudentTable();
            resetfilter();
        }
        private void PurgeStudentTable()
        {
            StudentTable.RowDefinitions.Clear();
            StudentTable.Children.Clear();
        }
        private void StudentEnrollScheduleCancel_Click(object sender, RoutedEventArgs e)
        {
            StudentEnrollSchedule.Visibility = Visibility.Collapsed;
            HouseEnroll.Visibility = Visibility.Visible;
            if (ScrollveiwerNormal.Visibility == Visibility.Visible)
            {
                ScrollveiwerNormal.Visibility = Visibility.Collapsed;
            }
            if (ScrollVeiwerTimeTurner.Visibility == Visibility.Visible)
            {
                ScrollVeiwerTimeTurner.Visibility = Visibility.Collapsed;
            }
            //resetschedulehere
            resetTimeTurner();
            resetenrollStudentschedule();
            TableTitle.Text = "";
        }

        private void resetTimeTurner()
        {
            TurnOnTimeTurner.Visibility = Visibility.Collapsed;
            TurnOffTimeTurner.Visibility = Visibility.Collapsed;
            TimeTurnerEnablerWarning.Visibility = Visibility.Collapsed;
            TimeTurnerEnabler.IsChecked = null;
        }

        private void resetenrollStudentschedule()
        {
            if (FormA2ValidSemesters.SelectedItem != null)
            {
                FormA2ValidSemesters.SelectedItem = null;
            }
            if (FormStudentDisenrollValidSemesters.SelectedItem != null)
            {
                FormStudentDisenrollValidSemesters.SelectedItem = null;
            }
            if(FormA2AssignedCourses.SelectedItem != null)
            {
                FormA2AssignedCourses.SelectedItem = null;
            }
            if(FormStudentDisenrollAssignedCourses.SelectedItem != null)
            {
                FormStudentDisenrollAssignedCourses.SelectedItem = null;
            }
        }
        private async void SetupCourses(object sender, SelectionChangedEventArgs e)
        {//populates courses from the selected semester and shows schedule for that semester
            List<int> CurrentlyEnrolledCourseIDs = new List<int>();//for updating prior enrolled courses
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
                    ScrollveiwerNormal.Visibility = Visibility.Visible;
                    TableTitle.Text = $"Semester {FormA2ValidSemesters.SelectedValue}";
                    TableTitleTT.Text = $"Semester {FormA2ValidSemesters.SelectedValue}";
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
                                    if (TimeTableflag[0] == false)
                                    {//sets ups courses only if the time turner is 
                                        TimeTurnerSafety = false;
                                        TimeTurnerEnabler.IsChecked = false;
                                        TimeTurnerSafety = true;
                                        HasNormalCourses = true;
                                    }
                                    else
                                    {//sets up the courses if the time turner is on
                                        TimeTurnerSafety = false;
                                        TimeTurnerEnabler.IsChecked = true;
                                        TimeTurnerSafety = true;
                                        HasNormalCourses = false;
                                    }
                                    foreach (var item in CurrentlyEnrolledCourseIDs)
                                    {//grabs all of the courses 
                                        using (SqlCommand cmd = sqlConn.CreateCommand())
                                        {//gets the course names and types for each courseid 
                                            cmd.CommandText = $"SELECT Title, CourseType FROM Courses WHERE CourseID = {item};";
                                            using (SqlDataReader reader = cmd.ExecuteReader())
                                            {
                                                while (reader.Read())
                                                {
                                                    Enrolledresults += (reader.GetValue(0).ToString()) + "+?";
                                                    Enrolledresults += (reader.GetValue(1).ToString()) + "+?";
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
                                                    Enrolledresults += "|" + reader.GetValue(0).ToString() + " ";
                                                    Enrolledresults += reader.GetValue(1).ToString();
                                                }
                                            }
                                        }
                                        TotalEnrolledCourseTimes.Add(Enrolledresults);
                                        Enrolledresults = "";
                                    }
                                }
                            }
                            catch (Exception ex)
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
                    if (CurrentlyEnrolledCourseIDs.Count > 0)
                    {//if there are more than 0 courese enrolled turn on time turner safety here
                        foreach (var course in TotalEnrolledCourseTimes)
                        {//we have to parse this message
                            Parsecourseinfo(course);
                        }
                    }
                    else
                    {
                        TimeTurnerSafety = false;
                    }
                }
            }
            else
            {
                FormA2AssignedCourses.Items.Add("Please pick a Semester");
            }
        }

        private void Parsecourseinfo(string courseinfo)
        {
            string[] ParseCourseInfo = courseinfo.Split("+?");
            string coursetitle = ParseCourseInfo[0];
            string coursetype = ParseCourseInfo[1];
            string therest = ParseCourseInfo[2];
            SetupEnrolledCourses(coursetitle, coursetype, therest);
        }
        private void SetupEnrolledCourses(string coursetitle, string coursetype, string therest)
        {
            string[] coordinates = therest.Split("|");
            if (HasNormalCourses == true)
            {//populates when schedule is normal
                foreach (var item in coordinates)
                {
                    if (item != "")
                    {
                        string[] rowcolumn = item.Split(" ");
                        string column = rowcolumn[0];
                        string row = rowcolumn[1];
                        column = ConvertDayTOnumber(column);
                        row = ConvertTimeToNumber(row);
                        string name = column + row;
                        foreach (var spot in StudentSemesterCalendar.Children)
                        {
                            if (spot.GetType() == typeof(Border))
                            {
                                TextBlock tb = (spot as Border).Child as TextBlock;
                                if (tb.Name == name)
                                {
                                    tb.FontSize = 20;
                                    tb.Text = coursetype + "\n" + coursetitle;
                                    tb.TextWrapping = TextWrapping.Wrap;
                                }
                            }
                        }
                    }
                }
            }
            else
            {//populates when schedule is time turner
                List<int> daycounter = new List<int>();
                int MCount = 0;
                int TCount = 0;
                int WCount = 0;
                int ThCount = 0;
                int FCount = 0;
                foreach (var item in coordinates)
                {
                    if (item != "")
                    {//gets a count of all the days
                        string[] TTrowcolum = item.Split(" ");
                        if (TTrowcolum[0] == "Monday")
                        {
                            MCount++;
                        }
                        else if (TTrowcolum[0] == "Tuesday")
                        {
                            TCount++;
                        }
                        else if (TTrowcolum[0] == "Wednesday")
                        {
                            WCount++;
                        }
                        else if (TTrowcolum[0] == "Thursday")
                        {
                            ThCount++;
                        }
                        else if (TTrowcolum[0] == "Friday")
                        {
                            FCount++;
                        }
                    }
                }
                //adds total number of days to the count for the one course passed to us
                daycounter.Add(MCount);
                daycounter.Add(TCount);
                daycounter.Add(WCount);
                daycounter.Add(ThCount);
                daycounter.Add(FCount);
                TTTotoalRowCount += daycounter.Max();
                for (int newrows = 0; newrows < daycounter.Max(); newrows++)
                {//adds new rows to match max total rows
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(75.00);
                    StudentSemesterCalendarTT.RowDefinitions.Add(row);
                }
                foreach (var item in coordinates)
                {//I WISH I COULD FIND A BETTER WAY TO DO THIS
                    if (item != "")
                    {
                        string[] TTrowcolum = item.Split(" ");
                        Border hopethisworks = new Border();
                        hopethisworks.BorderThickness = new Thickness(2);
                        hopethisworks.BorderBrush = new SolidColorBrush(Colors.Black);
                        hopethisworks.Name = "deletemelater";
                        TextBlock txtblock = new TextBlock();
                        txtblock.Name = "deletemelater2";
                        txtblock.FontSize = 20;
                        txtblock.Foreground = new SolidColorBrush(Colors.Black);
                        txtblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        txtblock.HorizontalAlignment = HorizontalAlignment.Center;
                        txtblock.VerticalAlignment = VerticalAlignment.Center;
                        txtblock.TextWrapping = TextWrapping.Wrap;
                        if (TTrowcolum[0] == "Monday")
                        {
                            txtblock.Text = coursetype + "\n" + coursetitle + "\n" + TTrowcolum[1];
                            hopethisworks.SetValue(Grid.RowProperty, TTMondayCount);
                            hopethisworks.SetValue(Grid.ColumnProperty, 0);
                            TTMondayCount++;
                        }
                        else if (TTrowcolum[0] == "Tuesday")
                        {
                            txtblock.Text = coursetype + "\n" + coursetitle + "\n" + TTrowcolum[1];
                            hopethisworks.SetValue(Grid.RowProperty, TTTuesdayCount);
                            hopethisworks.SetValue(Grid.ColumnProperty, 1);
                            TTTuesdayCount++;
                        }
                        else if (TTrowcolum[0] == "Wednesday")
                        {
                            txtblock.Text = coursetype + "\n" + coursetitle + "\n" + TTrowcolum[1];
                            hopethisworks.SetValue(Grid.RowProperty, TTWednesdayCount);
                            hopethisworks.SetValue(Grid.ColumnProperty, 2);
                            TTWednesdayCount++;
                        }
                        else if (TTrowcolum[0] == "Thursday")
                        {
                            txtblock.Text = coursetype + "\n" + coursetitle + "\n" + TTrowcolum[1];
                            hopethisworks.SetValue(Grid.RowProperty, TTThursdayCount);
                            hopethisworks.SetValue(Grid.ColumnProperty, 3);
                            TTThursdayCount++;
                        }
                        else if (TTrowcolum[0] == "Friday")
                        {
                            txtblock.Text = coursetype + "\n" + coursetitle + "\n" + TTrowcolum[1];
                            hopethisworks.SetValue(Grid.RowProperty, TTFridayCount);
                            hopethisworks.SetValue(Grid.ColumnProperty, 4);
                            TTFridayCount++;
                        }
                        hopethisworks.Child = txtblock;
                        StudentSemesterCalendarTT.Children.Add(hopethisworks);
                    }
                }
                //necessary clean up
                daycounter.Clear();
            }
        }

        private string ConvertTimeToNumber(string row)
        {
            if (row == "0000")
            {
                row = "2";
            }
            else if (row == "0100")
            {
                row = "3";
            }
            else if (row == "0200")
            {
                row = "4";
            }
            else if (row == "0300")
            {
                row = "5";
            }
            else if (row == "0400")
            {
                row = "6";
            }
            else if (row == "0500")
            {
                row = "7";
            }
            else if (row == "0600")
            {
                row = "8";
            }
            else if (row == "0700")
            {
                row = "9";
            }
            else if (row == "0800")
            {
                row = "10";
            }
            else if (row == "0900")
            {
                row = "11";
            }
            else if (row == "1000")
            {
                row = "12";
            }
            else if (row == "1100")
            {
                row = "13";
            }
            else if (row == "1200")
            {
                row = "14";
            }
            else if (row == "1300")
            {
                row = "15";
            }
            else if (row == "1400")
            {
                row = "16";
            }
            else if (row == "1500")
            {
                row = "17";
            }
            else if (row == "1600")
            {
                row = "18";
            }
            else if (row == "1700")
            {
                row = "19";
            }
            else if (row == "1800")
            {
                row = "20";
            }
            else if (row == "1900")
            {
                row = "21";
            }
            else if (row == "2000")
            {
                row = "22";
            }
            else if (row == "2100")
            {
                row = "23";
            }
            else if (row == "2200")
            {
                row = "24";
            }
            else
            {
                row = "25";
            }
            return row;
        }
        private string ConvertDayTOnumber(string column)
        {
            if (column == "Monday")
            {
                column = "1";
            }
            else if (column == "Tuesday")
            {
                column = "2";
            }
            else if (column == "Wednesday")
            {
                column = "3";
            }
            else if (column == "Thursday")
            {
                column = "4";
            }
            else
            {
                column = "5";
            }
            return column;
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
            if (ScrollVeiwerTimeTurner.Visibility == Visibility.Visible)
            {
                ScrollVeiwerTimeTurner.Visibility = Visibility.Collapsed;
            }
            if (ScrollveiwerNormal.Visibility == Visibility.Visible)
            {
                ScrollveiwerNormal.Visibility = Visibility.Collapsed;
            }
            if (TimeTurnerEnabler.IsChecked != null)
            {
                TimeTurnerEnabler.IsChecked = null;
            }
            if (FormA2AssignedCourses.SelectedItem != null)
            {
                FormA2AssignedCourses.SelectedItem = null;
            }
            FormA2AssignedCourses.Items.Clear();
            foreach (var spot in StudentSemesterCalendar.Children)
            {
                if (spot.GetType() == typeof(Border))
                {
                    TextBlock tb = (spot as Border).Child as TextBlock;
                    if (tb != null)
                    {
                        if (Regex.IsMatch(tb.Name, @"^[0-9]+$") == true)
                        {
                            if (tb.Text != "")
                            {
                                tb.Text = "";
                            }
                        }
                    }
                }
            }
            TTMondayCount = 2;
            TTTuesdayCount = 2;
            TTWednesdayCount = 2;
            TTThursdayCount = 2;
            TTFridayCount = 2;
            TTTotoalRowCount = 0;
            foreach (Border spot in StudentSemesterCalendarTT.Children)
            {
                foreach (Border spot2 in StudentSemesterCalendarTT.Children)
                {
                    if (spot2.Name == "deletemelater")
                    {
                        StudentSemesterCalendarTT.Children.Remove(spot2.Child);
                        StudentSemesterCalendarTT.Children.Remove(spot2);
                    }
                }
            }
            //removes row definitions
            int num = StudentSemesterCalendarTT.RowDefinitions.Count();
            while (num != 2)
            {
                --num;
                StudentSemesterCalendarTT.RowDefinitions.RemoveAt(num);
            }
        }
        private void PreviewEnrollment(object sender, SelectionChangedEventArgs e)
        {
            ValidEnrollment = true;
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
                    sqlConn.Close();
                }
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
            if (ScrollveiwerNormal.Visibility == Visibility.Visible)
            {
                Border myblock = new Border();
                string txtboxname = "";
                myblock.Name = name;
                myblock.SetValue(Grid.RowProperty, row);
                myblock.SetValue(Grid.ColumnProperty, column);
                txtboxname = column.ToString() + row.ToString();
                foreach (var spot in StudentSemesterCalendar.Children)
                {
                    if (spot.GetType() == typeof(Border))
                    {
                        TextBlock tb = (spot as Border).Child as TextBlock;
                        if (tb != null)
                        {
                            if (tb.Name == txtboxname)
                            {
                                if (tb.Text != "")
                                {
                                    myblock.Background = new SolidColorBrush(Colors.Red);
                                    ValidEnrollment = false;
                                    break;
                                }
                                else
                                {
                                    myblock.Background = new SolidColorBrush(Colors.LightGreen);
                                    break;
                                }
                            }
                        }
                    }
                }
                StudentSemesterCalendar.Children.Add(myblock);
            }
            else
            {
                purgePreview();
            }
        }
        private void purgePreview()
        {
            foreach (Border item in StudentSemesterCalendar.Children)
            {
                foreach (Border item2 in StudentSemesterCalendar.Children)
                {//i dont understand why this works
                    if (item2.Name == "preview")
                    {
                        StudentSemesterCalendar.Children.Remove(item2);
                    }
                }
                if (item.Name == "preview")
                {
                    StudentSemesterCalendar.Children.Remove(item);
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
            List<int> courseidchecker = new List<int>();
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
            if (ValidEnrollment == false)
            {
                notvalidenrollmessage += "This spot is already taken\n";
                notvalidenroll++;
            }
            if (notvalidenroll == 0)
            {
                if (ValidEnrollment == true)
                {//insert the selected day,time,course, and semester to StudentEnrolledCourses
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {//convert HUID to StudentID
                            if (TimeTurnerEnabler.IsChecked == true)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT SECID FROM StudentEnrolledCourses WHERE StudentID = {SelectedStudentHUID} AND SemesterID = {SemID} AND CourseID = {CrsID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            courseidchecker.Add((int)reader.GetValue(0));
                                        }
                                    }
                                }
                            }
                            else
                            {//ensure that this is 0 to actually insert if TT is off
                                courseidchecker.Clear();
                            }
                            if (courseidchecker.Count > 0)
                            {//inform user that they already are inserted in the course
                                var BadTTCourseEnroll = new MessageDialog("Student is already enrolled in course.");
                                await BadTTCourseEnroll.ShowAsync();
                            }
                            else
                            {//if they are not already enrolled they can insert into the course
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"INSERT INTO StudentEnrolledCourses VALUES ({SemID},{CrsID},{SelectedStudentHUID},{timeturner});", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                            }
                            sqlConn.Close();
                        }
                    }
                    if (courseidchecker.Count == 0)
                    {//safety check to move to next page after inserting the course
                        var CourseEnrolled = new MessageDialog("Successfully enrolled student in course");
                        await CourseEnrolled.ShowAsync();
                        SetupCourses(null, null);
                    }
                }
            }
            else
            {
                var NotValidMessage = new MessageDialog(notvalidenrollmessage);
                await NotValidMessage.ShowAsync();
            }
        }
        private async void TryToEnableTimeTurner(object sender, RoutedEventArgs e)
        {
            bool iscourses;
            int semesterid;
            List<int> enrolledcourses = new List<int>();
            if (FormA2ValidSemesters.SelectedValue != null)
            {//can only set the TimeTurner if they select a semester
                if (FormA2ValidSemesters.SelectedItem.ToString() != "There Are No Semesters")
                {//can only set the TimeTurner if the semester selected is a valid semester
                    semesterid = GetSemesterID();
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {//first we need to see if this person is already enrolled in any courses for the selected semester
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT CourseID FROM StudentEnrolledCourses WHERE SemesterID = {semesterid} AND StudentID = {SelectedStudentHUID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        enrolledcourses.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                            sqlConn.Close();
                        }
                    }
                    if (enrolledcourses.Count > 0)
                    {
                        iscourses = true;
                    }
                    else
                    {
                        iscourses = false;
                    }
                    if (HasNormalCourses == true && TimeTurnerSafety == true)
                    {
                        TimeTurnerEnabler.IsChecked = null;
                        if (iscourses == true)
                        {//they are currenetly enrolled in normal classes we need to prompt user if they wish to disenroll user from all currently enrolled courses 
                            TimeTurnerSafety = false;
                            TimeTurnerEnabler.IsChecked = false;
                            TimeTurnerSafety = true;
                            TimeTurnerEnablerWarning.Visibility = Visibility.Visible;
                            TurnOnTimeTurner.Visibility = Visibility.Visible;
                        }
                    }
                    else if (HasNormalCourses == false && TimeTurnerSafety == true)
                    {
                        TimeTurnerEnabler.IsChecked = null;
                        if (iscourses == true)
                        {//they are currenetly enrolled in TimeTurner classes we need to prompt user if they wish to disenroll user from all currently enrolled courses 
                            TimeTurnerSafety = false;
                            TimeTurnerEnabler.IsChecked = true;
                            TimeTurnerSafety = true;
                            TimeTurnerEnablerWarning.Visibility = Visibility.Visible;
                            TurnOffTimeTurner.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {//if not enrolled
                        if (TimeTurnerEnabler.IsChecked == true)
                        {//setup tt calendar
                            ScrollveiwerNormal.Visibility = Visibility.Collapsed;
                            ScrollVeiwerTimeTurner.Visibility = Visibility.Visible;
                        }
                        else
                        {//set up the normal calendar
                            ScrollVeiwerTimeTurner.Visibility = Visibility.Collapsed;
                            ScrollveiwerNormal.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            else
            {
                var NotValidMessage = new MessageDialog("Please Select A Semester To Enable Time Turner");
                await NotValidMessage.ShowAsync();
                if (TimeTurnerEnabler.IsChecked == true)
                {
                    TimeTurnerEnabler.IsChecked = null;
                }
                else
                {
                    TimeTurnerEnabler.IsChecked = null;
                }
            }
        }
        private void CancelTimeTurner_Click(object sender, RoutedEventArgs e)
        {
            TimeTurnerEnablerWarning.Visibility = Visibility.Collapsed;
            if (TurnOnTimeTurner.Visibility == Visibility.Visible)
            {
                TurnOnTimeTurner.Visibility = Visibility.Collapsed;
            }
            else
            {
                TurnOffTimeTurner.Visibility = Visibility.Collapsed;
            }
            if (HasNormalCourses == true)
            {
                TimeTurnerEnabler.IsChecked = false;
            }
            else
            {
                TimeTurnerEnabler.IsChecked = true;
            }
        }
        private void DisEnrollStudent_Click(object sender, RoutedEventArgs e)
        {
            if (TurnOnTimeTurner.Visibility == Visibility.Visible)
            {
                TimeTurnerSafety = false;
                TimeTurnerEnabler.IsChecked = true;
            }
            else
            {
                TimeTurnerSafety = false;
                TimeTurnerEnabler.IsChecked = false;
            }
            TimeTurnerEnablerWarning.Visibility = Visibility.Collapsed;
            DisenrollAllClassesFromSelectedSemester();
        }

        private async void DisenrollAllClassesFromSelectedSemester()
        {
            List<int> assignmentids = new List<int>();
            List<int> courseids = new List<int>();
            int semesterid = GetSemesterID();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//convert HUID to StudentID
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {//get all courseids the student is enrolled in for the semester
                        cmd.CommandText = $"SELECT CourseID FROM StudentEnrolledCourses WHERE SemesterID = {semesterid} AND StudentID = {SelectedStudentHUID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                courseids.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if (courseids.Count > 0)
                    {//if there are any courseids get all assignment ids from each course
                        foreach (var id in courseids)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE SemesterID = {semesterid} AND CourseID = {id};";
                                using(SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while(reader.Read())
                                    {
                                        assignmentids.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                    }
                    if(assignmentids.Count > 0)
                    {//delete each grade by assignment id
                        foreach(var assignmentid in assignmentids)
                        {
                            SqlDataAdapter adapter2 = new SqlDataAdapter();
                            SqlCommand command2 = new SqlCommand($"DELETE FROM Grades WHERE AssignmentID = {assignmentid} AND StudentHUID = {SelectedStudentHUID};", sqlConn);
                            adapter2.DeleteCommand = command2;
                            adapter2.DeleteCommand.ExecuteNonQuery();
                        }
                    }
                    if(courseids.Count > 0)
                    {//delete each final grade by course id
                        foreach(var courseid in courseids)
                        {
                            SqlDataAdapter adapter2 = new SqlDataAdapter();
                            SqlCommand command2 = new SqlCommand($"DELETE FROM FinalGrade WHERE CourseID = {courseid} AND StudentHUID = {SelectedStudentHUID};", sqlConn);
                            adapter2.DeleteCommand = command2;
                            adapter2.DeleteCommand.ExecuteNonQuery();
                        }
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    SqlCommand command = new SqlCommand($"DELETE FROM StudentEnrolledCourses WHERE SemesterID = {semesterid} AND StudentID = {SelectedStudentHUID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
            var NotValidMessage = new MessageDialog("All courses for this semester has been disenrolled");
            await NotValidMessage.ShowAsync();
        }

        private void FilterHouseEnroll_Click(object sender, RoutedEventArgs e)
        {
            HouseFilterEnrollOptions.Visibility = Visibility.Visible;
        }

        private void CancelFilter_Click(object sender, RoutedEventArgs e)
        {
            HouseFilterEnrollOptions.Visibility = Visibility.Collapsed;
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

        private void SetFilter(object sender, RoutedEventArgs e)
        {//Set all filters to false
            int year;
            Int32.TryParse(YearlevelInput.SelectedValue.ToString(), out year);

            if ((sender as CheckBox).Name == "FilterbyAlph")
            {
                FilterbyReg.IsChecked = false;
                PurgeStudentTable();
                if (SelectedHouse == "Gryffindor")
                {
                    PopulateTableAll(1, "alph", year, "enroll");
                }
                else if (SelectedHouse == "Slytherin")
                {
                    PopulateTableAll(2, "alph", year, "enroll");
                }
                else if (SelectedHouse == "Ravenclaw")
                {
                    PopulateTableAll(3, "alph", year, "enroll");
                }
                else if (SelectedHouse == "Hufflepuff")
                {
                    PopulateTableAll(4, "alph", year, "enroll");
                }
            }
            else if ((sender as CheckBox).Name == "FilterbyReg")
            {
                FilterbyAlph.IsChecked = false;
                PurgeStudentTable();
                if (SelectedHouse == "Gryffindor")
                {
                    PopulateTableAll(1, "default", year, "enroll");
                }
                else if (SelectedHouse == "Slytherin")
                {
                    PopulateTableAll(2, "default", year, "enroll");
                }
                else if (SelectedHouse == "Ravenclaw")
                {
                    PopulateTableAll(3, "default", year, "enroll");
                }
                else if (SelectedHouse == "Hufflepuff")
                {
                    PopulateTableAll(4, "default", year, "enroll");
                }
            }
        }

        private void YearlevelInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int year;
            Int32.TryParse(YearlevelInput.SelectedValue.ToString(), out year);
            PurgeStudentTable();
            if (FilterbyReg.IsChecked == true)
            {
                if (SelectedHouse == "Gryffindor")
                {
                    PopulateTableAll(1, "default", year, "enroll");
                }
                else if (SelectedHouse == "Slytherin")
                {
                    PopulateTableAll(2, "default", year, "enroll");
                }
                else if (SelectedHouse == "Ravenclaw")
                {
                    PopulateTableAll(3, "default", year, "enroll");
                }
                else if (SelectedHouse == "Hufflepuff")
                {
                    PopulateTableAll(4, "default", year, "enroll");
                }
            }
            else
            {
                if (SelectedHouse == "Gryffindor")
                {
                    PopulateTableAll(1, "alph", year, "enroll");
                }
                else if (SelectedHouse == "Slytherin")
                {
                    PopulateTableAll(2, "alph", year, "enroll");
                }
                else if (SelectedHouse == "Ravenclaw")
                {
                    PopulateTableAll(3, "alph", year, "enroll");
                }
                else if (SelectedHouse == "Hufflepuff")
                {
                    PopulateTableAll(4, "alph", year, "enroll");
                }
            }
        }

        private void SelectedHouseDisenrollCancel_Click(object sender, RoutedEventArgs e)
        {
            HouseDisenroll.Visibility = Visibility.Collapsed;
            HouseOptions.Visibility = Visibility.Visible;
            DisenrollPurgeStudentTable();
            resetfilter();
        }

        private void DisenrollPurgeStudentTable()
        {
            StudentTableA3.RowDefinitions.Clear();
            StudentTableA3.Children.Clear();
        }

        private void DisenrollCancelFilter_Click(object sender, RoutedEventArgs e)
        {
            HouseFilterDisnerollOptions.Visibility = Visibility.Collapsed;
        }

        private void FilterSelectedHouseDisenroll_Click(object sender, RoutedEventArgs e)
        {
            HouseFilterDisnerollOptions.Visibility = Visibility.Visible;
        }

        private void DisenrollSetFilter(object sender, RoutedEventArgs e)
        {
            int year;
            Int32.TryParse(DisenrollYearlevelInput.SelectedValue.ToString(), out year);

            if ((sender as CheckBox).Name == "DisenrollFilterbyAlph")
            {
                DisenrollFilterbyReg.IsChecked = false;
                DisenrollPurgeStudentTable();
                if (SelectedHouse == "Gryffindor")
                {
                    PopulateTableAll(1, "alph", year, "disenroll");
                }
                else if (SelectedHouse == "Slytherin")
                {
                    PopulateTableAll(2, "alph", year, "disenroll");
                }
                else if (SelectedHouse == "Ravenclaw")
                {
                    PopulateTableAll(3, "alph", year, "disenroll");
                }
                else if (SelectedHouse == "Hufflepuff")
                {
                    PopulateTableAll(4, "alph", year, "disenroll");
                }
            }
            else if ((sender as CheckBox).Name == "DisenrollFilterbyReg")
            {
                DisenrollFilterbyAlph.IsChecked = false;
                DisenrollPurgeStudentTable();
                if (SelectedHouse == "Gryffindor")
                {
                    PopulateTableAll(1, "default", year, "disenroll");
                }
                else if (SelectedHouse == "Slytherin")
                {
                    PopulateTableAll(2, "default", year, "disenroll");
                }
                else if (SelectedHouse == "Ravenclaw")
                {
                    PopulateTableAll(3, "default", year, "disenroll");
                }
                else if (SelectedHouse == "Hufflepuff")
                {
                    PopulateTableAll(4, "default", year, "disenroll");
                }
            }
        }
        private async void DisenrollFilterAttemptUncheck(object sender, RoutedEventArgs e)
        {
            if (DisenrollFilterbyAlph.IsChecked == false && DisenrollFilterbyReg.IsChecked == false)
            {
                (sender as CheckBox).IsChecked = true;
                var NotValidMessage = new MessageDialog("Please select a filter that is not currently in use.");
                await NotValidMessage.ShowAsync();
            }
        }

        private void DisenrollYearlevelInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int year;
            Int32.TryParse(DisenrollYearlevelInput.SelectedValue.ToString(), out year);
            DisenrollPurgeStudentTable();
            if (FilterbyReg.IsChecked == true)
            {
                if (SelectedHouse == "Gryffindor")
                {
                    PopulateTableAll(1, "default", year, "disenroll");
                }
                else if (SelectedHouse == "Slytherin")
                {
                    PopulateTableAll(2, "default", year, "disenroll");
                }
                else if (SelectedHouse == "Ravenclaw")
                {
                    PopulateTableAll(3, "default", year, "disenroll");
                }
                else if (SelectedHouse == "Hufflepuff")
                {
                    PopulateTableAll(4, "default", year, "disenroll");
                }
            }
            else
            {
                if (SelectedHouse == "Gryffindor")
                {
                    PopulateTableAll(1, "alph", year, "disenroll");
                }
                else if (SelectedHouse == "Slytherin")
                {
                    PopulateTableAll(2, "alph", year, "disenroll");
                }
                else if (SelectedHouse == "Ravenclaw")
                {
                    PopulateTableAll(3, "alph", year, "disenroll");
                }
                else if (SelectedHouse == "Hufflepuff")
                {
                    PopulateTableAll(4, "alph", year, "disenroll");
                }
            }
        }

        private void StudentDisenrollScheduleCancel_Click(object sender, RoutedEventArgs e)
        {
            StudentDisenrollCourse.Visibility = Visibility.Collapsed;
            HouseDisenroll.Visibility = Visibility.Visible;
            FormStudentDisenrollValidSemesters.SelectedItem = null;
            FormStudentDisenrollAssignedCourses.SelectedItem = null;
        }

        private void SetUpDisenrollCourses(object sender, SelectionChangedEventArgs e)
        {
            int _semesterID;
            List<int> mycourseIDs = new List<int>();
            List<string> coursetitles = new List<string>();
            if (FormStudentDisenrollAssignedCourses.SelectedItem != null)
            {
                FormStudentDisenrollAssignedCourses.SelectedItem = null;
            }
            FormStudentDisenrollAssignedCourses.Items.Clear();
            if (FormStudentDisenrollValidSemesters.SelectedItem != null)
            {
                if (FormStudentDisenrollValidSemesters.SelectedValue.ToString() != "There Are No Semesters")
                { 
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {//updates the combo box for the courses for the selected semester
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {//acquire the semesterid from the name of the semester
                            _semesterID = GetDisenrollSemesterID();
                            if (_semesterID != 0)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT CourseID FROM StudentEnrolledCourses WHERE SemesterID = {_semesterID} AND StudentID = {SelectedStudentHUID};";
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
                            {//acquire the courses name with course ID
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
                                FormStudentDisenrollAssignedCourses.Items.Add("Please Assign Some Courses");
                            }
                            sqlConn.Close();
                        }
                    }
                    if (coursetitles.Count > 0)
                    {
                        foreach (var title in coursetitles)
                        {
                            FormStudentDisenrollAssignedCourses.Items.Add(title);
                        }
                    }
                }
            }
            else
            {
                FormStudentDisenrollAssignedCourses.Items.Add("Please pick a Semester");
            }
        }

        private int GetDisenrollSemesterID()
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
                        cmd.CommandText = $"SELECT SemesterID FROM Semesters WHERE Semester ='{FormStudentDisenrollValidSemesters.SelectedValue}';";
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

        private async void StudentDisenrollStudent_Click(object sender, RoutedEventArgs e)
        {
            int semesterid = 0;
            int notvalidDisenroll=0;
            string notvalidDisenrollMessage = "";
            int validcourseid = 0;
            List<int> assignmentids = new List<int>();
            if(FormStudentDisenrollValidSemesters.SelectedItem == null)
            {
                notvalidDisenroll++;
                notvalidDisenrollMessage += "Please Select A semester\n";
            }else if(FormStudentDisenrollValidSemesters.SelectedValue.ToString() == "There Are No Semesters")
            {
                notvalidDisenroll++;
                notvalidDisenrollMessage += "Please Add a semester\n";
            }
            if(FormStudentDisenrollAssignedCourses.SelectedItem == null)
            {
                notvalidDisenroll++;
                notvalidDisenrollMessage += "Please Select a course\n";
            }else if(FormStudentDisenrollAssignedCourses.SelectedValue.ToString() == "Please Assign Some Courses")
            {
                notvalidDisenroll++;
                notvalidDisenrollMessage += "Please Assign Some Courses\n";
            }
            if(notvalidDisenroll == 0)
            {
                semesterid = GetDisenrollSemesterID();
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {//acquire the courseid of the selected course to disenroll
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {//acquire the semesterid from the name of the semester
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT CourseID FROM Courses WHERE Title = '{FormStudentDisenrollAssignedCourses.SelectedValue}';";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    validcourseid = (int)reader.GetValue(0);
                                }
                            }
                        }
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {//get all the assignment ids
                            cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE SemesterID = {semesterid} AND CourseID = {validcourseid};";
                            using(SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    assignmentids.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                        if(assignmentids.Count > 0)
                        {
                            foreach(var id in assignmentids)
                            {
                                SqlDataAdapter adapter2 = new SqlDataAdapter();
                                SqlCommand command2 = new SqlCommand($"DELETE FROM Grades WHERE AssignmentID = {id} AND StudentHUID = {SelectedStudentHUID};", sqlConn);
                                adapter2.DeleteCommand = command2;
                                adapter2.DeleteCommand.ExecuteNonQuery();
                            }
                        }
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand($"DELETE FROM FinalGrade WHERE StudentHUID = {SelectedStudentHUID} AND CourseID = {validcourseid};", sqlConn);
                        adapter.DeleteCommand = command;
                        adapter.DeleteCommand.ExecuteNonQuery();
                        adapter = new SqlDataAdapter();
                        command = new SqlCommand($"DELETE FROM StudentEnrolledCourses WHERE SemesterID = {semesterid} AND StudentID = {SelectedStudentHUID} AND CourseID = {validcourseid};", sqlConn);
                        adapter.DeleteCommand = command;
                        adapter.DeleteCommand.ExecuteNonQuery();
                        sqlConn.Close();
                    }
                }
                var DisenrollMessage = new MessageDialog("Successfully disenrolled student from course.");
                await DisenrollMessage.ShowAsync();
                Frame.Navigate(typeof(CounselorEnrollStudents), _userHuid);
            }
            else
            {
                var NotValidDisenroll = new MessageDialog(notvalidDisenrollMessage);
                await NotValidDisenroll.ShowAsync();
            }
        }
    }
}