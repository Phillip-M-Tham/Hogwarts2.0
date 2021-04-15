using System;
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
    public sealed partial class ProfCourses : Page
    {
        private string UserHuid = "";
        private string _coursetitle = "";
        private string _coursetype = "";
        private string _department = "";
        private int _yearlevel = 0;
        private string _courseinfo = "";
        private int _notvalidform1 = 0;
        private int _notvalidform2A = 0;
        private string _errorform1 = "";
        private string _errorform2A = "";
        private string form1title = "";
        private string form2semester = "";
        private string form2course = "";
        private string form2validsemesterID = "";
        private string form2validcourseID = "";
        private string form2validsemestercourse = "";
        private int mycourseID;
        private int mysemesterID;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private int EditSelectedSemesterID;
        private int EditSelectedCourseID;
        private int EditRowCounter;
        private int SelectedAssignmentID;
        private int Form4CSelectedStudentHUID;
        public ProfCourses()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UserHuid = e.Parameter.ToString();
            SetUpForms();
        }
        private async void SetUpForms()
        {
            //Set ups form 1
            List<string> mydepartments = new List<string>();
            //set ups form 2A semesters
            List<string> mysemesters = new List<string>();
            //set ups form 2A courses
            List<string> mycourses = new List<string>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {   //puts info from database for departments in a list
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT Department FROM Departments;";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    mydepartments.Add(reader.GetValue(0).ToString());
                                }
                            }
                        }
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {//puts info from database for semesters in a list
                            cmd.CommandText = $"SELECT Semester FROM Semesters;";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    mysemesters.Add(reader.GetValue(0).ToString());
                                }
                            }
                        }
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT Title FROM Courses WHERE ProfHUID = '{UserHuid}';";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    mycourses.Add(reader.GetValue(0).ToString());
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
            //populates the departments drop down combo box form 1
            if (mydepartments.Count() == 0)
            {
                ValidDepartmentsInput.Items.Add("There are no Departments");
            }
            else
            {
                foreach (var deps in mydepartments)
                {
                    ValidDepartmentsInput.Items.Add(deps);
                }
            }
            //populates the semester drop down combo box form 2A and 2B and 1C
            if (mysemesters.Count() == 0)
            {
                Form2ValidSemesters.Items.Add("There Are No Semesters");
                AssignedSemesters.Items.Add("There Are No Semesters");
                Form1CValidSemester.Items.Add("There Are No Semesters");
            }
            else
            {
                foreach (var semester in mysemesters)
                {
                    Form2ValidSemesters.Items.Add(semester);
                    AssignedSemesters.Items.Add(semester);
                    Form1CValidSemester.Items.Add(semester);
                }
            }
            //populates the course drop down combo box form 2A
            if (mycourses.Count() == 0)
            {
                Form2Mycourses.Items.Add("Please create some courses");
            }
            else
            {
                foreach (var course in mycourses)
                {
                    Form2Mycourses.Items.Add(course);
                }
            }
        }
        private void CreateCourse_Click(object sender, RoutedEventArgs e)
        {
            AddCourseForm.Visibility = Visibility.Visible;
            AssignCourseForm.Visibility = Visibility.Collapsed;
        }
        private void AssignCourse_Click(object sender, RoutedEventArgs e)
        {
            AddCourseForm.Visibility = Visibility.Collapsed;
            AssignCourseForm.Visibility = Visibility.Visible;
        }
        private void GradeCourse_Click(object sender, RoutedEventArgs e)
        {
            Form1C.Visibility = Visibility.Visible;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AddCourseForm.Visibility = Visibility.Collapsed;
            AssignCourseForm.Visibility = Visibility.Collapsed;
            form2A.Visibility = Visibility.Collapsed;
            ResetForm1();
        }
        private void ResetForm1()
        {
            CourseInsertTitleInput.Text = "";
            CourseInsertTypeInput.Text = "";
            if (ValidDepartmentsInput.SelectedItem != null)
            {
                ValidDepartmentsInput.SelectedItem = null;
            }
            if (YearlevelInput.SelectedItem != null)
            {
                YearlevelInput.SelectedItem = null;
            }
            CourseInsertInfoInput.Text = "";
        }
        private async void SubmitCourse_Click(object sender, RoutedEventArgs e)
        {
            _notvalidform1 = 0;
            _errorform1 = "";
            _coursetitle = CourseInsertTitleInput.Text;
            _coursetype = CourseInsertTypeInput.Text;
            _department = "";
            _courseinfo = CourseInsertInfoInput.Text;
            //checks course title
            if (_coursetitle == "")
            {
                _notvalidform1++;
                _errorform1 += "Please provide a course title \n";
            }
            else
            {
                try
                {
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Title FROM Courses WHERE Title ='{_coursetitle}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        form1title += reader.GetValue(0).ToString();
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
                if (form1title != "")
                {
                    _notvalidform1++;
                    _errorform1 += "Course title already exists please pick a different title \n";
                }
            }
            //checks course type
            if (_coursetype == "")
            {
                _notvalidform1++;
                _errorform1 += "Please provide a course type \n";
            }
            //checks for department
            if (ValidDepartmentsInput.SelectedValue == null)
            {
                _notvalidform1++;
                _department = "";
                _errorform1 += "Please provide a department \n";
            }
            else
            {
                _department = ValidDepartmentsInput.SelectedValue.ToString();
            }
            //checks for level year
            if (YearlevelInput.SelectedValue == null)
            {
                _notvalidform1++;
                _yearlevel = 0;
                _errorform1 += "Please provide a year level \n";
            }
            else
            {
                Int32.TryParse(YearlevelInput.SelectedValue.ToString(), out _yearlevel);
            }

            if (_notvalidform1 != 0)
            {
                var ermessage = new MessageDialog(_errorform1);
                await ermessage.ShowAsync();
            }
            else
            {
                try
                {//Insert course into database
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"INSERT INTO Courses VALUES ({UserHuid},'{_coursetitle}','{_coursetype}','{_department}',{_yearlevel},'{_courseinfo}');", sqlConn);
                            adapter.InsertCommand = command;
                            adapter.InsertCommand.ExecuteNonQuery();
                            var coursecreated = new MessageDialog($"Course {_coursetitle} successfully created");
                            await coursecreated.ShowAsync();
                            sqlConn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var ermessage = new MessageDialog(ex.Message);
                    await ermessage.ShowAsync();
                }
                Frame.Navigate(typeof(ProfCourses), UserHuid);
            }
        }
        private void Cancelform2_Click(object sender, RoutedEventArgs e)
        {
            AddCourseForm.Visibility = Visibility.Collapsed;
            AssignCourseForm.Visibility = Visibility.Collapsed;
            form2A.Visibility = Visibility.Collapsed;
        }
        private async void Submitform2A_Click(object sender, RoutedEventArgs e)
        {
            //_totaltimes = 0;
            //resets notvalid counter and error message string
            _notvalidform2A = 0;
            _errorform2A = "";
            form2validsemestercourse = "";
            form2validsemesterID = "";
            form2validcourseID = "";
            mysemesterID = 0;
            mycourseID = 0;
            //Get list of each day for times
            //conduct a lot of error checking
            List<string> ValidMondayTimes;
            List<string> ValidTuesdayTimes;
            List<string> ValidWednesdayTimes;
            List<string> ValidThursdayTimes;
            List<string> ValidFridayTimes;
            ValidMondayTimes = GetValidMTimes();
            ValidTuesdayTimes = GetValidTTimes();
            ValidWednesdayTimes = GetValidWTimes();
            ValidThursdayTimes = GetValidThTimes();
            ValidFridayTimes = GetValidFTimes();
            //check to see if anything is blank
            if (Form2ValidSemesters.SelectedItem == null)
            {
                _notvalidform2A++;
                _errorform2A += "Please choose a semester \n";
            }
            else if (Form2ValidSemesters.SelectedItem.ToString() == "There Are No Semesters")
            {
                _notvalidform2A++;
                _errorform2A += "Please contact executive to add semesters \n";
            }
            else
            {
                form2semester = Form2ValidSemesters.SelectedItem.ToString();
            }

            if (Form2Mycourses.SelectedItem == null)
            {
                _notvalidform2A++;
                _errorform2A += "Please choose a Course \n";
            }
            else if (Form2Mycourses.SelectedItem.ToString() == "Please create some courses")
            {
                _notvalidform2A++;
                _errorform2A += "Please create courses to add \n";
            }
            else
            {
                form2course = Form2Mycourses.SelectedItem.ToString();
            }

            if (Form2MoInput.IsChecked == false && Form2TuInput.IsChecked == false && Form2WeInput.IsChecked == false && Form2ThInput.IsChecked == false && Form2FrInput.IsChecked == false)
            {
                _notvalidform2A++;
                _errorform2A += "Please assign days to teach your course \n";
            }
            else
            {
                if (ValidMondayTimes.Count() == 0 && Form2MoInput.IsChecked == true)
                {
                    _notvalidform2A++;
                    _errorform2A += "Please pick times for Monday \n";
                }
                if (ValidTuesdayTimes.Count() == 0 && Form2TuInput.IsChecked == true)
                {
                    _notvalidform2A++;
                    _errorform2A += "Please pick times for Tuesday \n";
                }
                if (ValidWednesdayTimes.Count() == 0 && Form2WeInput.IsChecked == true)
                {
                    _notvalidform2A++;
                    _errorform2A += "Please pick times for Wednesday \n";
                }
                if (ValidThursdayTimes.Count() == 0 && Form2ThInput.IsChecked == true)
                {
                    _notvalidform2A++;
                    _errorform2A += "Please pick times for Thursday \n";
                }
                if (ValidFridayTimes.Count() == 0 && Form2FrInput.IsChecked == true)
                {
                    _notvalidform2A++;
                    _errorform2A += "Please pick times for Friday \n";
                }
            }
            if (Form2LocationInput.Text == "")
            {
                _notvalidform2A++;
                _errorform2A += "Please provide a location for your course \n";
            }
            if (_notvalidform2A > 0)
            {
                var test = new MessageDialog(_errorform2A);
                await test.ShowAsync();
            }
            else
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {//First add course to semcourses
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {//retrive the semesterID from semesters
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT SemesterID FROM Semesters WHERE Semester ='{form2semester}';";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    form2validsemesterID += reader.GetValue(0).ToString();
                                }
                            }
                        }
                        //retrieve courseID from courses
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT CourseID FROM Courses WHERE Title = '{form2course}';";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    form2validcourseID += reader.GetValue(0).ToString();
                                }
                            }
                        }
                        if (form2validcourseID == "")
                        {
                            var nocourse = new MessageDialog("No Course was found");
                            await nocourse.ShowAsync();
                        }
                        else
                        {
                            Int32.TryParse(form2validcourseID, out mycourseID);
                        }

                        if (form2validsemesterID == "")
                        {
                            var nosemester = new MessageDialog("No Semester was found");
                            await nosemester.ShowAsync();
                        }
                        else
                        {
                            Int32.TryParse(form2validsemesterID, out mysemesterID);
                        }

                        if (form2validcourseID != "" && form2validsemesterID != "")
                        {//check to see if semesterid and course id already exists
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT SemesterID,CourseID FROM SemesterCourses WHERE SemesterID = {mysemesterID} AND CourseID = {mycourseID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        form2validsemestercourse += reader.GetValue(0).ToString();
                                    }
                                }
                            }
                            if (form2validsemestercourse != "")
                            {
                                var nosemestercourse = new MessageDialog("Course is already assigned");
                                await nosemestercourse.ShowAsync();
                            }
                            else
                            {
                                //continue insert
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"INSERT INTO SemesterCourses VALUES ({mysemesterID},{mycourseID});", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                                //insert days for this semester course
                                if (ValidMondayTimes.Count() > 0)
                                {//add monday to daytypes
                                    adapter = new SqlDataAdapter();
                                    command = new SqlCommand($"INSERT INTO DayTypes VALUES ({mysemesterID},{mycourseID},\'Monday\');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                    //insert monday times
                                    foreach (var time in ValidMondayTimes)
                                    {
                                        adapter = new SqlDataAdapter();
                                        command = new SqlCommand($"INSERT INTO Times VALUES ({mysemesterID},{mycourseID},\'Monday\','{time}');", sqlConn);
                                        adapter.InsertCommand = command;
                                        adapter.InsertCommand.ExecuteNonQuery();
                                    }
                                }
                                if (ValidTuesdayTimes.Count() > 0)
                                {//add tuesday to daytypes
                                    adapter = new SqlDataAdapter();
                                    command = new SqlCommand($"INSERT INTO DayTypes VALUES ({mysemesterID},{mycourseID},\'Tuesday\');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                    //insert tuesday times
                                    foreach (var time in ValidTuesdayTimes)
                                    {
                                        adapter = new SqlDataAdapter();
                                        command = new SqlCommand($"INSERT INTO Times VALUES ({mysemesterID},{mycourseID},\'Tuesday\','{time}');", sqlConn);
                                        adapter.InsertCommand = command;
                                        adapter.InsertCommand.ExecuteNonQuery();
                                    }
                                }
                                if (ValidWednesdayTimes.Count() > 0)
                                {
                                    adapter = new SqlDataAdapter();
                                    command = new SqlCommand($"INSERT INTO DayTypes VALUES ({mysemesterID},{mycourseID},\'Wednesday\');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                    foreach (var time in ValidWednesdayTimes)
                                    {
                                        adapter = new SqlDataAdapter();
                                        command = new SqlCommand($"INSERT INTO Times VALUES ({mysemesterID},{mycourseID},\'Wednesday\','{time}');", sqlConn);
                                        adapter.InsertCommand = command;
                                        adapter.InsertCommand.ExecuteNonQuery();
                                    }
                                }
                                if (ValidThursdayTimes.Count() > 0)
                                {
                                    adapter = new SqlDataAdapter();
                                    command = new SqlCommand($"INSERT INTO DayTypes VALUES ({mysemesterID},{mycourseID},\'Thursday\');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                    foreach (var time in ValidThursdayTimes)
                                    {
                                        adapter = new SqlDataAdapter();
                                        command = new SqlCommand($"INSERT INTO Times VALUES ({mysemesterID},{mycourseID},\'Thursday\','{time}');", sqlConn);
                                        adapter.InsertCommand = command;
                                        adapter.InsertCommand.ExecuteNonQuery();
                                    }
                                }
                                if (ValidFridayTimes.Count() > 0)
                                {
                                    adapter = new SqlDataAdapter();
                                    command = new SqlCommand($"INSERT INTO DayTypes VALUES ({mysemesterID},{mycourseID},\'Friday\');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                    foreach (var time in ValidFridayTimes)
                                    {
                                        adapter = new SqlDataAdapter();
                                        command = new SqlCommand($"INSERT INTO Times VALUES ({mysemesterID},{mycourseID},\'Friday\','{time}');", sqlConn);
                                        adapter.InsertCommand = command;
                                        adapter.InsertCommand.ExecuteNonQuery();
                                    }
                                }
                                //Insert Location
                                adapter = new SqlDataAdapter();
                                command = new SqlCommand($"INSERT INTO Locations VALUES ({mysemesterID},{mycourseID},'{Form2LocationInput.Text}');", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                                //Insert RequiredMaterial
                                adapter = new SqlDataAdapter();
                                command = new SqlCommand($"INSERT INTO ReqMaterials VALUES ({mysemesterID},{mycourseID},'{Form2RequiredMaterialsInput.Text}');", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                                var semester = new MessageDialog($"Course {form2course} was successfully assigned to {form2semester}");
                                await semester.ShowAsync();
                                sqlConn.Close();
                                Frame.Navigate(typeof(ProfCourses), UserHuid);
                            }
                        }
                        sqlConn.Close();
                    }
                }
            }
        }
        private List<string> GetValidFTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in FridayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }
        private List<string> GetValidThTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in ThursdayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private List<string> GetValidWTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in WednesdayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private List<string> GetValidTTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in TuesdayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private List<string> GetValidMTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in MondayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private void AssignCourseForm2_Click(object sender, RoutedEventArgs e)
        {
            form2A.Visibility = Visibility.Visible;
            AssignCourseForm2.Visibility = Visibility.Collapsed;
            UpdateCourseForm2.Visibility = Visibility.Collapsed;
            Cancelform2.Visibility = Visibility.Collapsed;
            Form2B.Visibility = Visibility.Collapsed;
        }
        private void UpdateCourseForm2_Click(object sender, RoutedEventArgs e)
        {
            Form2B.Visibility = Visibility.Visible;
            AssignCourseForm.Visibility = Visibility.Collapsed;
        }
        private void Weesnaw(object sender, SelectionChangedEventArgs e)
        {//Sets the visibility of the times if you check the respective day
            if (form2A.Visibility == Visibility.Visible)
            {
                if (MondayTimes.Visibility == Visibility.Visible)
                {
                    MondayTimes.SelectedValue = "Monday";
                }
                if (TuesdayTimes.Visibility == Visibility.Visible)
                {
                    TuesdayTimes.SelectedValue = "Tuesday";
                }
                if (WednesdayTimes.Visibility == Visibility.Visible)
                {
                    WednesdayTimes.SelectedValue = "Wednesday";
                }
                if (ThursdayTimes.Visibility == Visibility.Visible)
                {
                    ThursdayTimes.SelectedValue = "Thursday";
                }
                if (FridayTimes.Visibility == Visibility.Visible)
                {
                    FridayTimes.SelectedValue = "Friday";
                }
            }
            else if (Form2B.Visibility == Visibility.Visible)
            {
                if (Form2BMondayTimes.Visibility == Visibility.Visible)
                {
                    Form2BMondayTimes.SelectedValue = "Monday";
                }
                if (Form2BTuesdayTimes.Visibility == Visibility.Visible)
                {
                    Form2BTuesdayTimes.SelectedValue = "Tuesday";
                }
                if (Form2BWednesdayTimes.Visibility == Visibility.Visible)
                {
                    Form2BWednesdayTimes.SelectedValue = "Wednesday";
                }
                if (Form2BThursdayTimes.Visibility == Visibility.Visible)
                {
                    Form2BThursdayTimes.SelectedValue = "Thursday";
                }
                if (Form2BFridayTimes.Visibility == Visibility.Visible)
                {
                    Form2BFridayTimes.SelectedValue = "Friday";
                }
            }
        }
        private void TurnOnMondayTimes(object sender, RoutedEventArgs e)
        {
            MondayTimes.Visibility = Visibility.Visible;
        }
        private void TurnOffMondayTimes(object sender, RoutedEventArgs e)
        {
            MondayTimes.Visibility = Visibility.Collapsed;
            ResetMondayTimes();
        }
        private void ResetMondayTimes()
        {//if you uncheck Monday all checks for time is reset
            foreach (CheckBox item in MondayTimes.Items)
            {
                item.IsChecked = false;
            }
            foreach (CheckBox item in Form2BMondayTimes.Items)
            {
                item.IsChecked = false;
            }
        }
        private void TurnOnTuesdayTimes(object sender, RoutedEventArgs e)
        {
            TuesdayTimes.Visibility = Visibility.Visible;
        }
        private void TurnOffTuesdayTimes(object sender, RoutedEventArgs e)
        {
            TuesdayTimes.Visibility = Visibility.Collapsed;
            ResetTuesdayTimes();
        }
        private void ResetTuesdayTimes()
        {//if you uncheck Tuesday all checks for time is reset
            foreach (CheckBox item in TuesdayTimes.Items)
            {
                item.IsChecked = false;
            }
            foreach (CheckBox item in Form2BTuesdayTimes.Items)
            {
                item.IsChecked = false;
            }
        }
        private void TurnOnWednesdayTimes(object sender, RoutedEventArgs e)
        {
            WednesdayTimes.Visibility = Visibility.Visible;
        }
        private void TurnOffWednesdayTimes(object sender, RoutedEventArgs e)
        {
            WednesdayTimes.Visibility = Visibility.Collapsed;
            ResetWednesdayTimes();
        }
        private void ResetWednesdayTimes()
        {//if you uncheck Wednesday all checks for time is reset
            foreach (CheckBox item in WednesdayTimes.Items)
            {
                item.IsChecked = false;
            }
            foreach (CheckBox item in Form2BWednesdayTimes.Items)
            {
                item.IsChecked = false;
            }
        }
        private void TurnOnThursdayTimes(object sender, RoutedEventArgs e)
        {
            ThursdayTimes.Visibility = Visibility.Visible;
        }
        private void TurnOffThursdayTimes(object sender, RoutedEventArgs e)
        {
            ThursdayTimes.Visibility = Visibility.Collapsed;
            ResetThursdayTimes();
        }
        private void ResetThursdayTimes()
        {//if you uncheck Thursday all checks for time is reset
            foreach (CheckBox item in ThursdayTimes.Items)
            {
                item.IsChecked = false;
            }
            foreach (CheckBox item in Form2BThursdayTimes.Items)
            {
                item.IsChecked = false;
            }

        }
        private void TurnOnFridayTimes(object sender, RoutedEventArgs e)
        {
            FridayTimes.Visibility = Visibility.Visible;
        }
        private void TurnOffFridayTimes(object sender, RoutedEventArgs e)
        {
            FridayTimes.Visibility = Visibility.Collapsed;
            ResetFridayTimes();
        }
        private void ResetFridayTimes()
        {//if you uncheck Friday all checks for time is reset
            foreach (CheckBox item in FridayTimes.Items)
            {
                item.IsChecked = false;
            }
            foreach (CheckBox item in Form2BFridayTimes.Items)
            {
                item.IsChecked = false;
            }
        }
        private void Cancelform2A_Click(object sender, RoutedEventArgs e)
        {
            form2A.Visibility = Visibility.Collapsed;
            AssignCourseForm2.Visibility = Visibility.Visible;
            UpdateCourseForm2.Visibility = Visibility.Visible;
            Cancelform2.Visibility = Visibility.Visible;
            ResetForm2A();
        }
        private void ResetForm2A()
        {
            if (Form2ValidSemesters != null)
            {
                Form2ValidSemesters.SelectedItem = null;
            }
            if (Form2Mycourses != null)
            {
                Form2Mycourses.SelectedItem = null;
            }
            if (Form2MoInput.IsChecked == true)
            {
                Form2MoInput.IsChecked = false;
            }
            if (Form2TuInput.IsChecked == true)
            {
                Form2TuInput.IsChecked = false;
            }
            if (Form2WeInput.IsChecked == true)
            {
                Form2WeInput.IsChecked = false;
            }
            if (Form2ThInput.IsChecked == true)
            {
                Form2ThInput.IsChecked = false;
            }
            if (Form2FrInput.IsChecked == true)
            {
                Form2FrInput.IsChecked = false;
            }
            ResetMondayTimes();
            ResetTuesdayTimes();
            ResetWednesdayTimes();
            ResetThursdayTimes();
            ResetFridayTimes();
            Form2LocationInput.Text = "";
            Form2RequiredMaterialsInput.Text = "";
        }
        private async void Submitform2B_Click(object sender, RoutedEventArgs e)
        { //update your already assigned courses
            bool MondayExist = false;
            bool TuesdayExist = false;
            bool WednesdayExist = false;
            bool ThursdayExist = false;
            bool FridayExist = false;
            int _notvalidform2B = 0;
            string _errorform2B = "";
            List<string> Form2BValidMondayTimes;
            List<string> Form2BValidTuesdayTimes;
            List<string> Form2BValidWednesdayTimes;
            List<string> Form2BValidThursdayTimes;
            List<string> Form2BValidFridayTimes;
            Form2BValidMondayTimes = Form2BGetValidMTimes();
            Form2BValidTuesdayTimes = Form2BGetValidTTimes();
            Form2BValidWednesdayTimes = Form2BGetValidWTimes();
            Form2BValidThursdayTimes = Form2BGetValidThTimes();
            Form2BValidFridayTimes = Form2BGetValidFTimes();
            int SemesterID = GetSemesterID();
            int CourseID = GetcourseID();
            //Check to see if anything is blank
            if (SemesterID == 0)
            {
                _notvalidform2B++;
                _errorform2B += "Please select a Semester \n";
            }
            if (CourseID == 0 && Form2BCourseLabel.Visibility == Visibility.Visible)
            {
                _notvalidform2B++;
                _errorform2B += "Please select a course \n";
            }

            if (Form2BMoInput.IsChecked == false && Form2BTuInput.IsChecked == false && Form2BWeInput.IsChecked == false && Form2BThInput.IsChecked == false && Form2BFrInput.IsChecked == false && Form2BDaysLabel.Visibility == Visibility.Visible)
            {
                _notvalidform2B++;
                _errorform2B += "Please assign days to teach your course \n";
            }
            else
            {
                if (Form2BValidMondayTimes.Count() == 0 && Form2BMoInput.IsChecked == true)
                {
                    _notvalidform2B++;
                    _errorform2B += "Please pick times for Monday \n";
                }
                if (Form2BValidTuesdayTimes.Count() == 0 && Form2BTuInput.IsChecked == true)
                {
                    _notvalidform2B++;
                    _errorform2B += "Please pick times for Tuesday \n";
                }
                if (Form2BValidWednesdayTimes.Count() == 0 && Form2BWeInput.IsChecked == true)
                {
                    _notvalidform2B++;
                    _errorform2B += "Please pick times for Wednesday \n";
                }
                if (Form2BValidThursdayTimes.Count() == 0 && Form2BThInput.IsChecked == true)
                {
                    _notvalidform2B++;
                    _errorform2B += "Please pick times for Thursday \n";
                }
                if (Form2BValidFridayTimes.Count() == 0 && Form2BFrInput.IsChecked == true)
                {
                    _notvalidform2B++;
                    _errorform2B += "Please pick times for Friday \n";
                }
            }
            if (ReallyAnnoyingScrollBarForm2B.Visibility == Visibility.Visible)
            {
                if (Form2BLocationInput.Text == "")
                {
                    _notvalidform2B++;
                    _errorform2B += "Please provide a Location";
                }
            }

            if (_notvalidform2B != 0)
            {
                var ermessage = new MessageDialog(_errorform2B);
                await ermessage.ShowAsync();
            }
            else
            {
                try
                {//update the course: days, times, location, course info and req materials
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            MondayExist = CheckMonday(SemesterID, CourseID);
                            TuesdayExist = CheckTuesday(SemesterID, CourseID);
                            WednesdayExist = CheckWednesday(SemesterID, CourseID);
                            ThursdayExist = CheckThursday(SemesterID, CourseID);
                            FridayExist = CheckFriday(SemesterID, CourseID);

                            if (Form2BValidMondayTimes.Count > 0)
                            {//this doesnt check if it already exist
                                if (MondayExist == false)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO DayTypes VALUES ({SemesterID},{CourseID},'Monday');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                                //update the times take care of times purge all original monday times
                                purgeMondayTimes(SemesterID, CourseID);
                                foreach (var time in Form2BValidMondayTimes)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO Times VALUES ({SemesterID},{CourseID},'Monday','{time}');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {//remove monday from the daytypes table
                                if (MondayExist == true)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"DELETE FROM DayTypes WHERE SemesterID = {SemesterID} AND CourseID = {CourseID} AND DaysName = 'Monday';", sqlConn);
                                    adapter.DeleteCommand = command;
                                    adapter.DeleteCommand.ExecuteNonQuery();
                                }
                                //take care of times
                                purgeMondayTimes(SemesterID, CourseID);
                            }

                            if (Form2BValidTuesdayTimes.Count > 0)
                            {
                                if (TuesdayExist == false)
                                {//add it if it doesnt already exist
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO DayTypes VALUES ({SemesterID},{CourseID},'Tuesday');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                                purgeTuesdayTimes(SemesterID, CourseID);
                                foreach (var time in Form2BValidTuesdayTimes)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO Times VALUES ({SemesterID},{CourseID},'Tuesday','{time}');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                if (TuesdayExist == true)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"DELETE FROM DayTypes WHERE SemesterID = {SemesterID} AND CourseID = {CourseID} AND DaysName = 'Tuesday';", sqlConn);
                                    adapter.DeleteCommand = command;
                                    adapter.DeleteCommand.ExecuteNonQuery();
                                }
                                purgeTuesdayTimes(SemesterID, CourseID);
                            }

                            if (Form2BValidWednesdayTimes.Count > 0)
                            {
                                if (WednesdayExist == false)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO DayTypes VALUES ({SemesterID},{CourseID},'Wednesday');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                                purgeWednesdayTimes(SemesterID, CourseID);
                                foreach (var time in Form2BValidWednesdayTimes)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO Times VALUES ({SemesterID},{CourseID},'Wednesday','{time}');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                if (WednesdayExist == true)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"DELETE FROM DayTypes WHERE SemesterID = {SemesterID} AND CourseID = {CourseID} AND DaysName = 'Wednesday';", sqlConn);
                                    adapter.DeleteCommand = command;
                                    adapter.DeleteCommand.ExecuteNonQuery();
                                }
                                purgeWednesdayTimes(SemesterID, CourseID);
                            }

                            if (Form2BValidThursdayTimes.Count > 0)
                            {
                                if (ThursdayExist == false)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO DayTypes VALUES ({SemesterID},{CourseID},'Thursday');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                                purgeThursdayTimes(SemesterID, CourseID);
                                foreach (var time in Form2BValidThursdayTimes)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO Times VALUES ({SemesterID},{CourseID},'Thursday','{time}');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                if (ThursdayExist == true)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"DELETE FROM DayTypes WHERE SemesterID = {SemesterID} AND CourseID = {CourseID} AND DaysName = 'Thursday';", sqlConn);
                                    adapter.DeleteCommand = command;
                                    adapter.DeleteCommand.ExecuteNonQuery();
                                }
                                purgeThursdayTimes(SemesterID, CourseID);
                            }

                            if (Form2BValidFridayTimes.Count > 0)
                            {
                                if (FridayExist == false)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO DayTypes VALUES ({SemesterID},{CourseID},'Friday');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                                purgeFridayTimes(SemesterID, CourseID);
                                foreach (var time in Form2BValidFridayTimes)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"INSERT INTO Times VALUES ({SemesterID},{CourseID},'Friday','{time}');", sqlConn);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                if (FridayExist == true)
                                {
                                    adapter = new SqlDataAdapter();
                                    SqlCommand command = new SqlCommand($"DELETE FROM DayTypes WHERE SemesterID = {SemesterID} AND CourseID = {CourseID} AND DaysName = 'Friday';", sqlConn);
                                    adapter.DeleteCommand = command;
                                    adapter.DeleteCommand.ExecuteNonQuery();
                                }
                                purgeFridayTimes(SemesterID, CourseID);
                            }
                            //update location
                            adapter = new SqlDataAdapter();
                            SqlCommand cmd2 = new SqlCommand($"UPDATE Locations SET Locations = '{Form2BLocationInput.Text}' WHERE SemesterID = {SemesterID} AND CourseID = {CourseID};", sqlConn);
                            adapter.UpdateCommand = cmd2;
                            adapter.UpdateCommand.ExecuteNonQuery();
                            //Update course info
                            adapter = new SqlDataAdapter();
                            cmd2 = new SqlCommand($"UPDATE Courses SET CourseInfo = '{Form2BCourseInfoInput.Text}' WHERE CourseID = {CourseID};", sqlConn);
                            adapter.UpdateCommand = cmd2;
                            adapter.UpdateCommand.ExecuteNonQuery();
                            //Update reqMaterials
                            adapter = new SqlDataAdapter();
                            cmd2 = new SqlCommand($"UPDATE ReqMaterials SET Materials = '{Form2BReqMaterialInput.Text}' WHERE SemesterID = {SemesterID} AND CourseID = {CourseID};", sqlConn);
                            adapter.UpdateCommand = cmd2;
                            adapter.UpdateCommand.ExecuteNonQuery();
                            sqlConn.Close();
                            var semester = new MessageDialog($"Course {AssignedCourses.SelectedValue.ToString()} was successfully updated for {AssignedSemesters.SelectedValue.ToString()}");
                            await semester.ShowAsync();
                            Frame.Navigate(typeof(ProfCourses), UserHuid);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = new MessageDialog(ex.Message);
                    await message.ShowAsync();
                }
            }
        }

        private void purgeFridayTimes(int semesterid, int courseid)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand($"DELETE FROM Times WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Friday';", sqlConn);
                        adapter.DeleteCommand = command;
                        adapter.DeleteCommand.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                }
            }
        }

        private bool CheckFriday(int semesterid, int courseid)
        {
            bool doesexist = false;
            string result = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT DaysName FROM DayTypes WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Friday';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (result == "Friday")
            {
                doesexist = true;
            }
            return doesexist;
        }

        private void purgeThursdayTimes(int semesterid, int courseid)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand($"DELETE FROM Times WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Thursday';", sqlConn);
                        adapter.DeleteCommand = command;
                        adapter.DeleteCommand.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                }
            }
        }

        private bool CheckThursday(int semesterid, int courseid)
        {
            bool doesexist = false;
            string result = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT DaysName FROM DayTypes WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Thursday';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (result == "Thursday")
            {
                doesexist = true;
            }
            return doesexist;
        }

        private void purgeWednesdayTimes(int semesterid, int courseid)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand($"DELETE FROM Times WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Wednesday';", sqlConn);
                        adapter.DeleteCommand = command;
                        adapter.DeleteCommand.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                }
            }
        }

        private bool CheckWednesday(int semesterid, int courseid)
        {
            bool doesexist = false;
            string result = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT DaysName FROM DayTypes WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Wednesday';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (result == "Wednesday")
            {
                doesexist = true;
            }
            return doesexist;
        }

        private void purgeTuesdayTimes(int semesterid, int courseid)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand($"DELETE FROM Times WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Tuesday';", sqlConn);
                        adapter.DeleteCommand = command;
                        adapter.DeleteCommand.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                }
            }
        }

        private bool CheckTuesday(int semesterid, int courseid)
        {
            bool doesexist = false;
            string result = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT DaysName FROM DayTypes WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Tuesday';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (result == "Tuesday")
            {
                doesexist = true;
            }
            return doesexist;
        }

        private void purgeMondayTimes(int semesterid, int courseid)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand($"DELETE FROM Times WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Monday';", sqlConn);
                        adapter.DeleteCommand = command;
                        adapter.DeleteCommand.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                }
            }
        }

        private bool CheckMonday(int semesterid, int courseid)
        {
            bool doesexist = false;
            string result = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT DaysName FROM DayTypes WHERE SemesterID = {semesterid} AND CourseID = {courseid} AND DaysName = 'Monday';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (result == "Monday")
            {
                doesexist = true;
            }
            return doesexist;
        }

        private List<string> Form2BGetValidFTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in Form2BFridayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private List<string> Form2BGetValidThTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in Form2BThursdayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private List<string> Form2BGetValidWTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in Form2BWednesdayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private List<string> Form2BGetValidTTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in Form2BTuesdayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private List<string> Form2BGetValidMTimes()
        {
            List<string> mylist = new List<string>();
            foreach (CheckBox item in Form2BMondayTimes.Items)
            {
                if (item.IsChecked == true)
                {
                    mylist.Add(item.Content.ToString());
                }
            }
            return mylist;
        }

        private void Cancelform2B_Click(object sender, RoutedEventArgs e)
        {
            Form2B.Visibility = Visibility.Collapsed;
            AssignCourseForm.Visibility = Visibility.Visible;
            ResetForm2B();
        }
        private void ResetForm2B()
        {
            if (AssignedSemesters.SelectedItem != null)
            {
                AssignedSemesters.SelectedItem = null;//changing selected item resets courses
            }
            Form2BCourseLabel.Visibility = Visibility.Collapsed;
            AssignedCourses.Visibility = Visibility.Collapsed;
            Form2BDaysLabel.Visibility = Visibility.Collapsed;
            Form2BDayInputs.Visibility = Visibility.Collapsed;
            ResetDays();
            Form2BTimeLabel.Visibility = Visibility.Collapsed;
            ResetMondayTimes();
            ResetTuesdayTimes();
            ResetWednesdayTimes();
            ResetThursdayTimes();
            ResetFridayTimes();
            //set visiblity to false for the scrollviewer here
            Form2BLocationInput.Text = "";
            Form2BCourseInfoInput.Text = "";
            Form2BReqMaterialInput.Text = "";
            ReallyAnnoyingScrollBarForm2B.Visibility = Visibility.Collapsed;
            ReallyAnnoyingScrollBarForm2B.ChangeView(0, 0, null);
        }
        private void SetupCourses(object sender, SelectionChangedEventArgs e)
        {
            ResetCourses();
            if (Form2BDaysLabel.Visibility == Visibility.Visible)
            {
                Form2BDaysLabel.Visibility = Visibility.Collapsed;
            }
            if (Form2BDayInputs.Visibility == Visibility.Visible)
            {
                Form2BDayInputs.Visibility = Visibility.Collapsed;
                ResetDays();
            }
            if (Form2BTimeLabel.Visibility == Visibility.Visible)
            {
                Form2BTimeLabel.Visibility = Visibility.Collapsed;
            }
            if (ReallyAnnoyingScrollBarForm2B.Visibility == Visibility.Visible)
            {
                ReallyAnnoyingScrollBarForm2B.Visibility = Visibility.Collapsed;
            }
            //reset the location
            Form2BLocationInput.Text = "";
            //reset the coures info
            Form2BCourseInfoInput.Text = "";
            //reset req materials 
            Form2BReqMaterialInput.Text = "";
            if (AssignedSemesters.SelectedItem != null)
            {
                int _semesterID;
                List<int> mycourseIDs = new List<int>();
                List<string> coursetitles = new List<string>();
                Form2BCourseLabel.Visibility = Visibility.Visible;
                AssignedCourses.Visibility = Visibility.Visible;
                //Reveal courses based on item and make courses visible
                if (AssignedSemesters.SelectedValue.ToString() != "There Are No Semesters")
                {//populate the courses combo box
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {//acquire the semesterid from the name of the semester
                            _semesterID = GetSemesterID();
                            if (_semesterID != 0)
                            {//acquire courseIDS from semcourses that have the semesterID
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
                                AssignedCourses.Items.Add("Please Assign Some Courses");
                            }
                            sqlConn.Close();
                        }
                    }
                    if (coursetitles.Count > 0)
                    {
                        foreach (var title in coursetitles)
                        {
                            AssignedCourses.Items.Add(title);
                        }
                    }
                }
            }
        }
        private void ResetDays()
        {
            if (Form2BMoInput.IsChecked == true)
            {
                Form2BMoInput.IsChecked = false;
            }
            if (Form2BTuInput.IsChecked == true)
            {
                Form2BTuInput.IsChecked = false;
            }
            if (Form2BWeInput.IsChecked == true)
            {
                Form2BWeInput.IsChecked = false;
            }
            if (Form2BThInput.IsChecked == true)
            {
                Form2BThInput.IsChecked = false;
            }
            if (Form2BFrInput.IsChecked == true)
            {
                Form2BFrInput.IsChecked = false;
            }
        }
        private void ResetCourses()
        {
            if (AssignedCourses.SelectedItem != null)
            {
                AssignedCourses.SelectedItem = null;
            }
            AssignedCourses.Items.Clear();
        }
        private void SetupCourseInfo(object sender, SelectionChangedEventArgs e)
        {
            int courseID;
            int semesterID;
            string location = "";
            string CourseInfo = "";
            string materials = "";
            List<string> Days = new List<string>();
            if (AssignedCourses.SelectedItem != null)
            {
                ResetDays();
                if (AssignedCourses.SelectedItem.ToString() != "Please Assign Some Courses")
                {
                    //set the days label visible
                    Form2BDaysLabel.Visibility = Visibility.Visible;
                    Form2BDayInputs.Visibility = Visibility.Visible;
                    ReallyAnnoyingScrollBarForm2B.Visibility = Visibility.Visible;
                    //find the courseid and semesterid
                    courseID = GetcourseID();
                    semesterID = GetSemesterID();
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {//acquire the semesterid from the name of the semester
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT DaysName FROM DayTypes WHERE SemesterID = {semesterID} AND CourseID = {courseID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Days.Add(reader.GetValue(0).ToString());
                                    }
                                }
                            }//acquire Location
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Locations FROM Locations WHERE SemesterID = {semesterID} AND CourseID = {courseID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        location += reader.GetValue(0).ToString();
                                    }
                                }
                            }//acquire course info
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT CourseInfo FROM Courses WHERE CourseID = {courseID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        CourseInfo += reader.GetValue(0).ToString();
                                    }
                                }
                            }//acquire required materials
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Materials FROM ReqMaterials WHERE CourseID = {courseID} AND SemesterID = {semesterID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        materials += reader.GetValue(0).ToString();
                                    }
                                }
                            }
                            sqlConn.Close();
                        }
                    }
                    if (Days.Count > 0)
                    {
                        //check the box if the name is in the list
                        Form2BTimeLabel.Visibility = Visibility.Visible;
                        foreach (var day in Days)
                        {
                            if (day == "Monday")
                            {
                                Form2BMoInput.IsChecked = true;
                            }
                            else if (day == "Tuesday")
                            {
                                Form2BTuInput.IsChecked = true;
                            }
                            else if (day == "Wednesday")
                            {
                                Form2BWeInput.IsChecked = true;
                            }
                            else if (day == "Thursday")
                            {
                                Form2BThInput.IsChecked = true;
                            }
                            else if (day == "Friday")
                            {
                                Form2BFrInput.IsChecked = true;
                            }
                        }
                    }
                    else
                    {
                        //no days were found 
                    }
                    //Sets up location
                    if (location == "")
                    {
                        //this should be possible
                    }
                    else
                    {
                        Form2BLocationInput.Text = location;
                    }
                    //sets up course info
                    if (CourseInfo == "")
                    {
                        Form2BCourseInfoInput.Text = "";
                    }
                    else
                    {
                        Form2BCourseInfoInput.Text = CourseInfo;
                    }
                    //sets up reqmaterials
                    if (materials == "")
                    {
                        Form2BReqMaterialInput.Text = "";
                    }
                    else
                    {
                        Form2BReqMaterialInput.Text = materials;
                    }
                }
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
                        //if()
                        cmd.CommandText = $"SELECT SemesterID FROM Semesters WHERE Semester ='{AssignedSemesters.SelectedValue}';";
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
        private int GetcourseID()
        {
            int _courseID = 0;
            string _courseid = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the semesterid from the name of the semester
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT courseID FROM Courses WHERE Title ='{AssignedCourses.SelectedValue}';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _courseid += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    Int32.TryParse(_courseid, out _courseID);
                    sqlConn.Close();
                }
            }
            return _courseID;
        }
        private void Form2BTurnOnMondayTimes(object sender, RoutedEventArgs e)
        {
            Form2BMondayTimes.Visibility = Visibility.Visible;
            SetUpAssignedMondayTimes();
        }
        private void SetUpAssignedMondayTimes()
        {
            int semesterID;
            int courseID;
            courseID = GetcourseID();
            semesterID = GetSemesterID();
            List<string> MondayTimes = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the semesterid from the name of the semester
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Times FROM Times WHERE SemesterID = {semesterID} AND CourseID = {courseID} AND DaysName = \'Monday\';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MondayTimes.Add(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (MondayTimes.Count > 0)
            {
                foreach (var time in MondayTimes)
                {
                    if (time == "0000")
                    {
                        Form2BMondayTimeInput0.IsChecked = true;
                    }
                    else if (time == "0100")
                    {
                        Form2BMondayTimeInput1.IsChecked = true;
                    }
                    else if (time == "0200")
                    {
                        Form2BMondayTimeInput2.IsChecked = true;
                    }
                    else if (time == "0300")
                    {
                        Form2BMondayTimeInput3.IsChecked = true;
                    }
                    else if (time == "0400")
                    {
                        Form2BMondayTimeInput4.IsChecked = true;
                    }
                    else if (time == "0500")
                    {
                        Form2BMondayTimeInput5.IsChecked = true;
                    }
                    else if (time == "0600")
                    {
                        Form2BMondayTimeInput6.IsChecked = true;
                    }
                    else if (time == "0700")
                    {
                        Form2BMondayTimeInput7.IsChecked = true;
                    }
                    else if (time == "0800")
                    {
                        Form2BMondayTimeInput8.IsChecked = true;
                    }
                    else if (time == "0900")
                    {
                        Form2BMondayTimeInput9.IsChecked = true;
                    }
                    else if (time == "1000")
                    {
                        Form2BMondayTimeInput10.IsChecked = true;
                    }
                    else if (time == "1100")
                    {
                        Form2BMondayTimeInput11.IsChecked = true;
                    }
                    else if (time == "1200")
                    {
                        Form2BMondayTimeInput12.IsChecked = true;
                    }
                    else if (time == "1300")
                    {
                        Form2BMondayTimeInput13.IsChecked = true;
                    }
                    else if (time == "1400")
                    {
                        Form2BMondayTimeInput14.IsChecked = true;
                    }
                    else if (time == "1500")
                    {
                        Form2BMondayTimeInput15.IsChecked = true;
                    }
                    else if (time == "1600")
                    {
                        Form2BMondayTimeInput16.IsChecked = true;
                    }
                    else if (time == "1700")
                    {
                        Form2BMondayTimeInput17.IsChecked = true;
                    }
                    else if (time == "1800")
                    {
                        Form2BMondayTimeInput18.IsChecked = true;
                    }
                    else if (time == "1900")
                    {
                        Form2BMondayTimeInput19.IsChecked = true;
                    }
                    else if (time == "2000")
                    {
                        Form2BMondayTimeInput20.IsChecked = true;
                    }
                    else if (time == "2100")
                    {
                        Form2BMondayTimeInput21.IsChecked = true;
                    }
                    else if (time == "2200")
                    {
                        Form2BMondayTimeInput22.IsChecked = true;
                    }
                    else if (time == "2300")
                    {
                        Form2BMondayTimeInput23.IsChecked = true;
                    }
                }
            }
        }
        private void Form2BTurnOffMondayTimes(object sender, RoutedEventArgs e)
        {
            Form2BMondayTimes.Visibility = Visibility.Collapsed;
            ResetMondayTimes();
        }
        private void Form2BTurnOnTuesdayTimes(object sender, RoutedEventArgs e)
        {
            Form2BTuesdayTimes.Visibility = Visibility.Visible;
            SetUpAssignedTuesdayTimes();
        }
        private void SetUpAssignedTuesdayTimes()
        {
            int semesterID;
            int courseID;
            courseID = GetcourseID();
            semesterID = GetSemesterID();
            List<string> TuesdayTimes = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the semesterid from the name of the semester
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Times FROM Times WHERE SemesterID = {semesterID} AND CourseID = {courseID} AND DaysName = \'Tuesday\';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TuesdayTimes.Add(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (TuesdayTimes.Count > 0)
            {
                foreach (var time in TuesdayTimes)
                {
                    if (time == "0000")
                    {
                        Form2BTuesdayTimeInput0.IsChecked = true;
                    }
                    else if (time == "0100")
                    {
                        Form2BTuesdayTimeInput1.IsChecked = true;
                    }
                    else if (time == "0200")
                    {
                        Form2BTuesdayTimeInput2.IsChecked = true;
                    }
                    else if (time == "0300")
                    {
                        Form2BTuesdayTimeInput3.IsChecked = true;
                    }
                    else if (time == "0400")
                    {
                        Form2BTuesdayTimeInput4.IsChecked = true;
                    }
                    else if (time == "0500")
                    {
                        Form2BTuesdayTimeInput5.IsChecked = true;
                    }
                    else if (time == "0600")
                    {
                        Form2BTuesdayTimeInput6.IsChecked = true;
                    }
                    else if (time == "0700")
                    {
                        Form2BTuesdayTimeInput7.IsChecked = true;
                    }
                    else if (time == "0800")
                    {
                        Form2BTuesdayTimeInput8.IsChecked = true;
                    }
                    else if (time == "0900")
                    {
                        Form2BTuesdayTimeInput9.IsChecked = true;
                    }
                    else if (time == "1000")
                    {
                        Form2BTuesdayTimeInput10.IsChecked = true;
                    }
                    else if (time == "1100")
                    {
                        Form2BTuesdayTimeInput11.IsChecked = true;
                    }
                    else if (time == "1200")
                    {
                        Form2BTuesdayTimeInput12.IsChecked = true;
                    }
                    else if (time == "1300")
                    {
                        Form2BTuesdayTimeInput13.IsChecked = true;
                    }
                    else if (time == "1400")
                    {
                        Form2BTuesdayTimeInput14.IsChecked = true;
                    }
                    else if (time == "1500")
                    {
                        Form2BTuesdayTimeInput15.IsChecked = true;
                    }
                    else if (time == "1600")
                    {
                        Form2BTuesdayTimeInput16.IsChecked = true;
                    }
                    else if (time == "1700")
                    {
                        Form2BTuesdayTimeInput17.IsChecked = true;
                    }
                    else if (time == "1800")
                    {
                        Form2BTuesdayTimeInput18.IsChecked = true;
                    }
                    else if (time == "1900")
                    {
                        Form2BTuesdayTimeInput19.IsChecked = true;
                    }
                    else if (time == "2000")
                    {
                        Form2BTuesdayTimeInput20.IsChecked = true;
                    }
                    else if (time == "2100")
                    {
                        Form2BTuesdayTimeInput21.IsChecked = true;
                    }
                    else if (time == "2200")
                    {
                        Form2BTuesdayTimeInput22.IsChecked = true;
                    }
                    else if (time == "2300")
                    {
                        Form2BTuesdayTimeInput23.IsChecked = true;
                    }
                }
            }
        }
        private void Form2BTurnOffTuesdayTimes(object sender, RoutedEventArgs e)
        {
            Form2BTuesdayTimes.Visibility = Visibility.Collapsed;
            ResetTuesdayTimes();
        }
        private void Form2BTurnOnWednesdayTimes(object sender, RoutedEventArgs e)
        {
            Form2BWednesdayTimes.Visibility = Visibility.Visible;
            SetUpAssignedWednesdayTimes();
        }
        private void SetUpAssignedWednesdayTimes()
        {
            int semesterID;
            int courseID;
            courseID = GetcourseID();
            semesterID = GetSemesterID();
            List<string> WednesdayTimes = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the semesterid from the name of the semester
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Times FROM Times WHERE SemesterID = {semesterID} AND CourseID = {courseID} AND DaysName = \'Wednesday\';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WednesdayTimes.Add(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (WednesdayTimes.Count > 0)
            {
                foreach (var time in WednesdayTimes)
                {
                    if (time == "0000")
                    {
                        Form2BWednesdayTimeInput0.IsChecked = true;
                    }
                    else if (time == "0100")
                    {
                        Form2BWednesdayTimeInput1.IsChecked = true;
                    }
                    else if (time == "0200")
                    {
                        Form2BWednesdayTimeInput2.IsChecked = true;
                    }
                    else if (time == "0300")
                    {
                        Form2BWednesdayTimeInput3.IsChecked = true;
                    }
                    else if (time == "0400")
                    {
                        Form2BWednesdayTimeInput4.IsChecked = true;
                    }
                    else if (time == "0500")
                    {
                        Form2BWednesdayTimeInput5.IsChecked = true;
                    }
                    else if (time == "0600")
                    {
                        Form2BWednesdayTimeInput6.IsChecked = true;
                    }
                    else if (time == "0700")
                    {
                        Form2BWednesdayTimeInput7.IsChecked = true;
                    }
                    else if (time == "0800")
                    {
                        Form2BWednesdayTimeInput8.IsChecked = true;
                    }
                    else if (time == "0900")
                    {
                        Form2BWednesdayTimeInput9.IsChecked = true;
                    }
                    else if (time == "1000")
                    {
                        Form2BWednesdayTimeInput10.IsChecked = true;
                    }
                    else if (time == "1100")
                    {
                        Form2BWednesdayTimeInput11.IsChecked = true;
                    }
                    else if (time == "1200")
                    {
                        Form2BWednesdayTimeInput12.IsChecked = true;
                    }
                    else if (time == "1300")
                    {
                        Form2BWednesdayTimeInput13.IsChecked = true;
                    }
                    else if (time == "1400")
                    {
                        Form2BWednesdayTimeInput14.IsChecked = true;
                    }
                    else if (time == "1500")
                    {
                        Form2BWednesdayTimeInput15.IsChecked = true;
                    }
                    else if (time == "1600")
                    {
                        Form2BWednesdayTimeInput16.IsChecked = true;
                    }
                    else if (time == "1700")
                    {
                        Form2BWednesdayTimeInput17.IsChecked = true;
                    }
                    else if (time == "1800")
                    {
                        Form2BWednesdayTimeInput18.IsChecked = true;
                    }
                    else if (time == "1900")
                    {
                        Form2BWednesdayTimeInput19.IsChecked = true;
                    }
                    else if (time == "2000")
                    {
                        Form2BWednesdayTimeInput20.IsChecked = true;
                    }
                    else if (time == "2100")
                    {
                        Form2BWednesdayTimeInput21.IsChecked = true;
                    }
                    else if (time == "2200")
                    {
                        Form2BWednesdayTimeInput22.IsChecked = true;
                    }
                    else if (time == "2300")
                    {
                        Form2BWednesdayTimeInput23.IsChecked = true;
                    }
                }
            }
        }
        private void Form2BTurnOffWednesdayTimes(object sender, RoutedEventArgs e)
        {
            Form2BWednesdayTimes.Visibility = Visibility.Collapsed;
            ResetWednesdayTimes();
        }
        private void Form2BTurnOnThursdayTimes(object sender, RoutedEventArgs e)
        {
            Form2BThursdayTimes.Visibility = Visibility.Visible;
            SetUpAssignedThursdayTimes();
        }
        private void SetUpAssignedThursdayTimes()
        {
            int semesterID;
            int courseID;
            courseID = GetcourseID();
            semesterID = GetSemesterID();
            List<string> ThursdayTimes = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the semesterid from the name of the semester
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Times FROM Times WHERE SemesterID = {semesterID} AND CourseID = {courseID} AND DaysName = \'Thursday\';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ThursdayTimes.Add(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (ThursdayTimes.Count > 0)
            {
                foreach (var time in ThursdayTimes)
                {
                    if (time == "0000")
                    {
                        Form2BThursdayTimeInput0.IsChecked = true;
                    }
                    else if (time == "0100")
                    {
                        Form2BThursdayTimeInput1.IsChecked = true;
                    }
                    else if (time == "0200")
                    {
                        Form2BThursdayTimeInput2.IsChecked = true;
                    }
                    else if (time == "0300")
                    {
                        Form2BThursdayTimeInput3.IsChecked = true;
                    }
                    else if (time == "0400")
                    {
                        Form2BThursdayTimeInput4.IsChecked = true;
                    }
                    else if (time == "0500")
                    {
                        Form2BThursdayTimeInput5.IsChecked = true;
                    }
                    else if (time == "0600")
                    {
                        Form2BThursdayTimeInput6.IsChecked = true;
                    }
                    else if (time == "0700")
                    {
                        Form2BThursdayTimeInput7.IsChecked = true;
                    }
                    else if (time == "0800")
                    {
                        Form2BThursdayTimeInput8.IsChecked = true;
                    }
                    else if (time == "0900")
                    {
                        Form2BThursdayTimeInput9.IsChecked = true;
                    }
                    else if (time == "1000")
                    {
                        Form2BThursdayTimeInput10.IsChecked = true;
                    }
                    else if (time == "1100")
                    {
                        Form2BThursdayTimeInput11.IsChecked = true;
                    }
                    else if (time == "1200")
                    {
                        Form2BThursdayTimeInput12.IsChecked = true;
                    }
                    else if (time == "1300")
                    {
                        Form2BThursdayTimeInput13.IsChecked = true;
                    }
                    else if (time == "1400")
                    {
                        Form2BThursdayTimeInput14.IsChecked = true;
                    }
                    else if (time == "1500")
                    {
                        Form2BThursdayTimeInput15.IsChecked = true;
                    }
                    else if (time == "1600")
                    {
                        Form2BThursdayTimeInput16.IsChecked = true;
                    }
                    else if (time == "1700")
                    {
                        Form2BThursdayTimeInput17.IsChecked = true;
                    }
                    else if (time == "1800")
                    {
                        Form2BThursdayTimeInput18.IsChecked = true;
                    }
                    else if (time == "1900")
                    {
                        Form2BThursdayTimeInput19.IsChecked = true;
                    }
                    else if (time == "2000")
                    {
                        Form2BThursdayTimeInput20.IsChecked = true;
                    }
                    else if (time == "2100")
                    {
                        Form2BThursdayTimeInput21.IsChecked = true;
                    }
                    else if (time == "2200")
                    {
                        Form2BThursdayTimeInput22.IsChecked = true;
                    }
                    else if (time == "2300")
                    {
                        Form2BThursdayTimeInput23.IsChecked = true;
                    }
                }
            }
        }
        private void Form2BTurnOffThursdayTimes(object sender, RoutedEventArgs e)
        {
            Form2BThursdayTimes.Visibility = Visibility.Collapsed;
            ResetThursdayTimes();
        }
        private void Form2BTurnOnFridayTimes(object sender, RoutedEventArgs e)
        {
            Form2BFridayTimes.Visibility = Visibility.Visible;
            SetUpAssignedFridayTimes();

        }
        private void SetUpAssignedFridayTimes()
        {
            int semesterID;
            int courseID;
            courseID = GetcourseID();
            semesterID = GetSemesterID();
            List<string> FridayTimes = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the semesterid from the name of the semester
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Times FROM Times WHERE SemesterID = {semesterID} AND CourseID = {courseID} AND DaysName = \'Friday\';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FridayTimes.Add(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (FridayTimes.Count > 0)
            {
                foreach (var time in FridayTimes)
                {
                    if (time == "0000")
                    {
                        Form2BFridayTimeInput0.IsChecked = true;
                    }
                    else if (time == "0100")
                    {
                        Form2BFridayTimeInput1.IsChecked = true;
                    }
                    else if (time == "0200")
                    {
                        Form2BFridayTimeInput2.IsChecked = true;
                    }
                    else if (time == "0300")
                    {
                        Form2BFridayTimeInput3.IsChecked = true;
                    }
                    else if (time == "0400")
                    {
                        Form2BFridayTimeInput4.IsChecked = true;
                    }
                    else if (time == "0500")
                    {
                        Form2BFridayTimeInput5.IsChecked = true;
                    }
                    else if (time == "0600")
                    {
                        Form2BFridayTimeInput6.IsChecked = true;
                    }
                    else if (time == "0700")
                    {
                        Form2BFridayTimeInput7.IsChecked = true;
                    }
                    else if (time == "0800")
                    {
                        Form2BFridayTimeInput8.IsChecked = true;
                    }
                    else if (time == "0900")
                    {
                        Form2BFridayTimeInput9.IsChecked = true;
                    }
                    else if (time == "1000")
                    {
                        Form2BFridayTimeInput10.IsChecked = true;
                    }
                    else if (time == "1100")
                    {
                        Form2BFridayTimeInput11.IsChecked = true;
                    }
                    else if (time == "1200")
                    {
                        Form2BFridayTimeInput12.IsChecked = true;
                    }
                    else if (time == "1300")
                    {
                        Form2BFridayTimeInput13.IsChecked = true;
                    }
                    else if (time == "1400")
                    {
                        Form2BFridayTimeInput14.IsChecked = true;
                    }
                    else if (time == "1500")
                    {
                        Form2BFridayTimeInput15.IsChecked = true;
                    }
                    else if (time == "1600")
                    {
                        Form2BFridayTimeInput16.IsChecked = true;
                    }
                    else if (time == "1700")
                    {
                        Form2BFridayTimeInput17.IsChecked = true;
                    }
                    else if (time == "1800")
                    {
                        Form2BFridayTimeInput18.IsChecked = true;
                    }
                    else if (time == "1900")
                    {
                        Form2BFridayTimeInput19.IsChecked = true;
                    }
                    else if (time == "2000")
                    {
                        Form2BFridayTimeInput20.IsChecked = true;
                    }
                    else if (time == "2100")
                    {
                        Form2BFridayTimeInput21.IsChecked = true;
                    }
                    else if (time == "2200")
                    {
                        Form2BFridayTimeInput22.IsChecked = true;
                    }
                    else if (time == "2300")
                    {
                        Form2BFridayTimeInput23.IsChecked = true;
                    }
                }
            }
        }
        private void Form2BTurnOffFridayTimes(object sender, RoutedEventArgs e)
        {
            Form2BFridayTimes.Visibility = Visibility.Collapsed;
            ResetFridayTimes();
        }

        private void Form1CCancel_Click(object sender, RoutedEventArgs e)
        {
            Form1C.Visibility = Visibility.Collapsed;
            Form1CValidSemester.SelectedItem = default;
            purgecoursetable();
        }

        private void purgecoursetable()
        {
            CourseTable.RowDefinitions.Clear();
            CourseTable.Children.Clear();
            Form1CTableName.Visibility = Visibility.Collapsed;
            CourseTable.Visibility = Visibility.Collapsed;
        }

        private async void SetupCoursesTable(object sender, SelectionChangedEventArgs e)
        {
            purgecoursetable();
            List<int> unfilteredcourseids = new List<int>();
            List<int> profcourseids = new List<int>();
            string semesterid = "";
            ComboBox cb = sender as ComboBox;
            List<string> nameofCourses = new List<string>();
            if (cb.SelectedItem != null)
            {
                if (cb.SelectedValue.ToString() != "There Are No Semesters")
                {//obtain all of the courseids from the selected semester
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {//acquire the all the courses the professor has created
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT CourseID FROM Courses WHERE ProfHUID = {Int32.Parse(UserHuid)};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        profcourseids.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {//acquire the semesterid from the name of the semester
                                cmd.CommandText = $"SELECT SemesterID FROM Semesters WHERE Semester = '{Form1CValidSemester.SelectedValue}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        semesterid += reader.GetValue(0).ToString();
                                    }
                                }
                            }
                            Int32.TryParse(semesterid, out int SemesterID);
                            EditSelectedSemesterID = SemesterID;
                            if (SemesterID != 0)
                            {//obtained a real semesterid;acquire the unfiltered courseids with the semester
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT CourseID FROM SemesterCourses WHERE SemesterID = {SemesterID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            unfilteredcourseids.Add((int)reader.GetValue(0));
                                        }
                                    }
                                }
                                if (unfilteredcourseids.Count > 0)
                                {//filter courses against all courses the prof has created

                                    foreach (var unfilter in unfilteredcourseids.ToList())
                                    {
                                        if (profcourseids.Contains(unfilter) == false)
                                        {
                                            unfilteredcourseids.Remove(unfilter);
                                        }
                                    }
                                    foreach (var validID in unfilteredcourseids)
                                    {//for each valid id run this loop
                                        using (SqlCommand cmd = sqlConn.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT Title FROM Courses WHERE CourseID = {validID};";
                                            using (SqlDataReader reader = cmd.ExecuteReader())
                                            {
                                                while (reader.Read())
                                                {
                                                    nameofCourses.Add(reader.GetValue(0).ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var courseerror = new MessageDialog("No courses have been assigned to the selected semester");
                                    await courseerror.ShowAsync();
                                }
                            }
                            else
                            {
                                var semestererror = new MessageDialog("No Semesters have been created");
                                await semestererror.ShowAsync();
                            }
                            sqlConn.Close();
                        }
                    }
                    if (nameofCourses.Count > 0)
                    {//populate table with the name of the courses
                        Form1CTableName.Visibility = Visibility.Visible;
                        CourseTable.Visibility = Visibility.Visible;
                        int rowcounter = 0;
                        foreach (var course in nameofCourses)
                        {
                            //first create a row definition
                            RowDefinition myrow = new RowDefinition();
                            myrow.Height = new GridLength(50);
                            CourseTable.RowDefinitions.Add(myrow);
                            //create a border
                            Border myborder = new Border();
                            myborder.BorderThickness = new Thickness(2);
                            myborder.BorderBrush = new SolidColorBrush(Colors.Black);
                            myborder.SetValue(Grid.RowProperty, rowcounter);
                            myborder.SetValue(Grid.ColumnProperty, 0);
                            //create a button
                            Button mybutton = new Button();
                            mybutton.FontSize = 36;
                            mybutton.Content = course.ToString();
                            mybutton.Foreground = new SolidColorBrush(Colors.Black);
                            mybutton.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                            mybutton.HorizontalAlignment = HorizontalAlignment.Center;
                            mybutton.VerticalAlignment = VerticalAlignment.Center;
                            mybutton.SetValue(NameProperty, unfilteredcourseids[rowcounter].ToString());
                            mybutton.Click += SetSelectedCourse;
                            //put the button in the border
                            myborder.Child = mybutton;
                            //add border to table
                            CourseTable.Children.Add(myborder);
                            rowcounter++;
                        }
                    }
                    else
                    {//the professor does not have any courses for the selected semester
                        purgecoursetable();
                        var profnosemestercourses = new MessageDialog("Please Create Some Courses for the selected semester.");
                        await profnosemestercourses.ShowAsync();
                    }
                }
            }
        }

        private void SetSelectedCourse(object sender, RoutedEventArgs e)
        {
            Form1C.Visibility = Visibility.Collapsed;
            Form2C.Visibility = Visibility.Visible;
            Button pressedbutton = sender as Button;
            Int32.TryParse(pressedbutton.Name, out EditSelectedCourseID);
            Form2CTitle.Text = pressedbutton.Content.ToString();
            purgeAssignmentsTable();
            SetupAssignmentsTable("Form2C", -1);
        }

        private void Form2CCancel_Click(object sender, RoutedEventArgs e)
        {
            Form2C.Visibility = Visibility.Collapsed;
            Form1C.Visibility = Visibility.Visible;
            EditAddAssignment.Visibility = Visibility.Collapsed;
            EditRemoveAssignment.Visibility = Visibility.Collapsed;
            EditSubmitAssignments.Visibility = Visibility.Collapsed;
            EditCancel.Visibility = Visibility.Collapsed;
            EditRemoveByCheck.Visibility = Visibility.Collapsed;
            purgeAssignmentsTable();
        }

        private void purgeAssignmentsTable()
        {
            AssignmentListTable.Children.Clear();
            AssignmentListTable.RowDefinitions.Clear();

            AssingmentListTableRemove.Children.Clear();
            AssingmentListTableRemove.RowDefinitions.Clear();
        }
        private void SetupAssignmentsTable(string mode, int studentID)
        {
            int form4cindex = 0;
            bool GradeExist;
            List<string> Assignments = new List<string>();
            List<int> AssignmentID = new List<int>();
            List<decimal> Form4CGrades = new List<decimal>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the all the courses the professor has created
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT AssignmentTitle,AssignmentID FROM Assignments WHERE CourseID = {EditSelectedCourseID} AND SemesterID = {EditSelectedSemesterID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Assignments.Add(reader.GetValue(0).ToString());
                                AssignmentID.Add((int)reader.GetValue(1));
                            }
                        }
                    }
                    if (mode == "Form4C" && studentID != -1)
                    {//get all the grades for the student only applicable through form4C
                        if (AssignmentID.Count > 0)
                        {//if there are assignmentIDs, gets the grade for the selected student for each assignment ID
                            foreach (var assignmentid in AssignmentID)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT Grade FROM Grades WHERE AssignmentID = {assignmentid} AND StudentHUID = {studentID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Form4CGrades.Add((decimal)reader.GetValue(0));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (Assignments.Count > 0)
            {//populate the table
                EditRowCounter = 0;
                if (EditAddAssignment.Visibility == Visibility.Collapsed)
                {
                    foreach (var item in Assignments)
                    {
                        RowDefinition myrow = new RowDefinition();
                        myrow.Height = new GridLength(50);

                        Border myborder = new Border();
                        myborder.BorderThickness = new Thickness(2);
                        myborder.BorderBrush = new SolidColorBrush(Colors.Black);
                        myborder.SetValue(Grid.RowProperty, EditRowCounter);
                        myborder.SetValue(Grid.ColumnProperty, 0);

                        if (mode == "Form2C")
                        {
                            Button mybutton = new Button();
                            mybutton.FontSize = 36;
                            mybutton.Foreground = new SolidColorBrush(Colors.Black);
                            mybutton.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                            mybutton.HorizontalAlignment = HorizontalAlignment.Center;
                            mybutton.VerticalAlignment = VerticalAlignment.Center;
                            mybutton.Content = item;
                            mybutton.Name = AssignmentID[EditRowCounter].ToString();//assigns the id as a string to the name of the button
                            mybutton.Click += SetupGrades;

                            AssignmentListTable.RowDefinitions.Add(myrow);
                            myborder.Child = mybutton;
                            AssignmentListTable.Children.Add(myborder);

                        }
                        else if (mode == "Form4C")
                        {
                            TextBlock txtblox = new TextBlock();
                            txtblox.FontSize = 36;
                            txtblox.Foreground = new SolidColorBrush(Colors.Black);
                            txtblox.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                            txtblox.HorizontalAlignment = HorizontalAlignment.Center;
                            txtblox.VerticalAlignment = VerticalAlignment.Center;
                            txtblox.Text = item;
                            //txtblox.Name = AssignmentID[EditRowCounter].ToString();//assigns the id as a string to the name of the button

                            Form4CCoursesTable.RowDefinitions.Add(myrow);
                            myborder.Child = txtblox;
                            Form4CCoursesTable.Children.Add(myborder);

                            Border bd2 = new Border();
                            bd2.BorderThickness = new Thickness(2);
                            bd2.BorderBrush = new SolidColorBrush(Colors.Black);
                            bd2.SetValue(Grid.RowProperty, EditRowCounter);
                            bd2.SetValue(Grid.ColumnProperty, 1);

                            TextBox txtbox = new TextBox();
                            //txtbox.Foreground = new SolidColorBrush(Colors.Black);TURNS GRADE FOR FORM4C BLACK
                            txtbox.FontSize = 36;
                            txtbox.Name = AssignmentID[EditRowCounter].ToString();
                            txtbox.FontFamily = new FontFamily("/Assets/ReginaScript.ttf#Regina Script");

                            //need to find out if there is a grade for the assignment id
                            GradeExist = getstudentgrade(studentID, AssignmentID[EditRowCounter]);
                            if (GradeExist == true)
                            {
                                txtbox.Text = Form4CGrades[form4cindex].ToString();
                                form4cindex++;
                            }
                            else
                            {
                                txtbox.Text = "";
                            }
                            bd2.Child = txtbox;
                            Form4CCoursesTable.Children.Add(bd2);
                        }
                        EditRowCounter++;
                    }
                }
                else
                {
                    foreach (var item in Assignments)
                    {//should only work with form2c

                        RowDefinition myrow = new RowDefinition();
                        myrow.Height = new GridLength(50);

                        RowDefinition newcheckrow = new RowDefinition();
                        newcheckrow.Height = new GridLength(50);
                        if (mode == "Form2C")
                        {
                            AssignmentListTable.RowDefinitions.Add(myrow);
                            AssingmentListTableRemove.RowDefinitions.Add(newcheckrow);

                            TextBox txtbox = new TextBox();
                            txtbox.FontSize = 36;
                            txtbox.Name = EditRowCounter.ToString();
                            txtbox.FontFamily = new FontFamily("/Assets/ReginaScript.ttf#Regina Script");
                            txtbox.Text = item;
                            txtbox.SetValue(Grid.RowProperty, EditRowCounter);
                            txtbox.SetValue(Grid.ColumnProperty, 0);

                            CheckBox checkbx = new CheckBox();
                            checkbx.Name = EditRowCounter.ToString();
                            checkbx.SetValue(Grid.RowProperty, EditRowCounter);
                            checkbx.SetValue(Grid.ColumnProperty, 0);

                            AssingmentListTableRemove.Children.Add(checkbx);

                            AssignmentListTable.Children.Add(txtbox);
                        }
                        EditRowCounter++;
                    }
                }
            }
            else
            {
                if (EditAddAssignment.Visibility == Visibility.Collapsed)
                {
                    RowDefinition myrow = new RowDefinition();
                    myrow.Height = new GridLength(50);

                    Border myborder = new Border();
                    myborder.BorderThickness = new Thickness(2);
                    myborder.BorderBrush = new SolidColorBrush(Colors.Black);
                    myborder.SetValue(Grid.RowProperty, 0);
                    myborder.SetValue(Grid.ColumnProperty, 0);

                    TextBlock mytxtblock = new TextBlock();
                    mytxtblock.FontSize = 36;
                    mytxtblock.Foreground = new SolidColorBrush(Colors.Black);
                    mytxtblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    mytxtblock.HorizontalAlignment = HorizontalAlignment.Center;
                    mytxtblock.VerticalAlignment = VerticalAlignment.Center;
                    mytxtblock.Text = "Please Add some assignments";
                    myborder.Child = mytxtblock;
                    if (mode == "Form2C")
                    {
                        AssignmentListTable.RowDefinitions.Add(myrow);
                        AssignmentListTable.Children.Add(myborder);
                    }
                    else if (mode == "Form4C")
                    {
                        Form4CCoursesTable.RowDefinitions.Add(myrow);
                        Form4CCoursesTable.Children.Add(myborder);
                    }
                    EditRowCounter = 0;
                }
            }
        }

        private void SetupGrades(object sender, RoutedEventArgs e)
        {
            Form3C.Visibility = Visibility.Visible;
            Button thebutton = sender as Button;
            Form3CTitle.Text = thebutton.Content.ToString();
            Int32.TryParse(thebutton.Name, out SelectedAssignmentID);
            SetupgradesTable(SelectedAssignmentID);
        }

        private async void SetupgradesTable(int assignmentid)
        {
            bool studenthasgrade;
            int count = 0;
            int count2 = 0;
            List<decimal> Grades = new List<decimal>();
            List<int> studentids = new List<int>();
            List<int> allids = new List<int>();
            List<string> studentnames = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the all students enrolled in the course 
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT StudentID FROM StudentEnrolledCourses WHERE CourseID = {EditSelectedCourseID} AND SemesterID = {EditSelectedSemesterID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentids.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (studentids.Count > 0)
            {//get the names from the acquired ids
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {//acquire the all students enrolled in the course 
                        foreach (var id in studentids)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
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
                        studentnames.Sort();
                        foreach (var name in studentnames)
                        {
                            string[] fnamelname = name.Split(' ');
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT HUID FROM Users WHERE FirstName = '{fnamelname[0]}' AND LastName = '{fnamelname[1]}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        allids.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                        //successfully got all ids with the list of first names
                        foreach (var id in allids.ToList())
                        {
                            if (studentids.Contains(id) == false)
                            {
                                allids.Remove(id);
                            }
                        }
                        foreach (var id in allids)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Grade FROM Grades WHERE AssignmentID = {assignmentid} AND StudentHUID = {id};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Grades.Add((decimal)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                        sqlConn.Close();
                    }
                }
                Form3CTableTitle1.Visibility = Visibility.Visible;
                Form3CTableTitle2.Visibility = Visibility.Visible;
                Form3CSubmit.Visibility = Visibility.Visible;
                foreach (var name in studentnames)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength(50);
                    StudentNameTable.RowDefinitions.Add(rd);
                    //border and button for student name

                    Border bd = new Border();
                    bd.BorderThickness = new Thickness(2);
                    bd.BorderBrush = new SolidColorBrush(Colors.Black);
                    bd.SetValue(Grid.RowProperty, count);
                    bd.SetValue(Grid.ColumnProperty, 0);

                    Button btn = new Button();
                    btn.FontSize = 36;
                    btn.Foreground = new SolidColorBrush(Colors.Black);
                    btn.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    btn.Content = name;
                    btn.Name = allids[count].ToString();//assigns student id to button name
                    btn.Click += SetupSelectedStudentGrades;

                    bd.Child = btn;
                    StudentNameTable.Children.Add(bd);
                    //border and textbox for grade
                    Border bd2 = new Border();
                    bd2.BorderThickness = new Thickness(2);
                    bd2.BorderBrush = new SolidColorBrush(Colors.Black);
                    bd2.SetValue(Grid.RowProperty, count);
                    bd2.SetValue(Grid.ColumnProperty, 1);

                    TextBox txtbox = new TextBox();
                    txtbox.FontSize = 36;
                    //txtbox.Foreground = new SolidColorBrush(Colors.Black); THIS MAKES FORM3C GRADE BLACK
                    txtbox.FontFamily = new FontFamily("/Assets/ReginaScript.ttf#Regina Script");
                    txtbox.Name = allids[count].ToString();
                    studenthasgrade = getstudentgrade(allids[count], -1);

                    if (studenthasgrade == true)
                    {//updates if they do have a grade and use a separate index on number of grades
                        txtbox.Text = Grades[count2].ToString();
                        count2++;
                    }
                    else
                    {
                        txtbox.Text = "";
                    }

                    bd2.Child = txtbox;

                    StudentNameTable.Children.Add(bd2);

                    count++;
                }
            }
            else
            {
                var profnosemestercourses = new MessageDialog("There are no students currently enrolled in this course.");
                await profnosemestercourses.ShowAsync();
            }
        }

        private void SetupSelectedStudentGrades(object sender, RoutedEventArgs e)
        {//we might be able to use this for All students in course page
            Button thebutton = sender as Button;
            int studentID = Int32.Parse(thebutton.Name);
            Form4C.Visibility = Visibility.Visible;
            Form4CTitle.Text = thebutton.Content.ToString();
            Form4CSelectedStudentHUID = studentID;
            SetupAssignmentsTable("Form4C", studentID);
            SetupFinalGradeText(studentID);
        }

        private async void SetupFinalGradeText(int studentID)
        {
            string validgrade = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the all students enrolled in the course 
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT CurrentGrade FROM FinalGrade WHERE StudentHUID = {studentID} AND CourseID = {EditSelectedCourseID};";
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
            if(validgrade == "")
            {
                Form4CStudentGrade.Text = "N/A";
                var Removeassignmenterror = new MessageDialog("Please give this student a grade for a minimal one assignment.");
                await Removeassignmenterror.ShowAsync();
            }
            else
            {
                Form4CStudentGrade.Text = validgrade;
            }
        }

        private bool getstudentgrade(int studentid, int assignmentid)
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
                        if (assignmentid != -1)
                        {
                            cmd.CommandText = $"SELECT Grade FROM Grades WHERE StudentHUID = {studentid} AND AssignmentID = {assignmentid};";
                        }
                        else
                        {
                            cmd.CommandText = $"SELECT Grade FROM Grades WHERE StudentHUID = {studentid} AND AssignmentID = {SelectedAssignmentID};";
                        }
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

        private void EditCancel_Click(object sender, RoutedEventArgs e)
        {
            EditAddAssignment.Visibility = Visibility.Collapsed;
            EditRemoveAssignment.Visibility = Visibility.Collapsed;
            EditSubmitAssignments.Visibility = Visibility.Collapsed;
            EditCancel.Visibility = Visibility.Collapsed;
            EditRemoveByCheck.Visibility = Visibility.Collapsed;
            purgeAssignmentsTable();
            SetupAssignmentsTable("Form2C", -1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditAddAssignment.Visibility = Visibility.Visible;
            EditRemoveAssignment.Visibility = Visibility.Visible;
            EditSubmitAssignments.Visibility = Visibility.Visible;
            EditCancel.Visibility = Visibility.Visible;
            EditRemoveByCheck.Visibility = Visibility.Visible;
            purgeAssignmentsTable();
            SetupAssignmentsTable("Form2C", -1);
        }

        private async void EditRemoveAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (EditRowCounter != 0)
            {//get rid of the current row
                --EditRowCounter;
                AssignmentListTable.Children.RemoveAt(EditRowCounter);
                AssignmentListTable.RowDefinitions.RemoveAt(EditRowCounter);

                AssingmentListTableRemove.Children.RemoveAt(EditRowCounter);
                AssingmentListTableRemove.RowDefinitions.RemoveAt(EditRowCounter);
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("No assignments exist.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private void EditAddAssignment_Click(object sender, RoutedEventArgs e)
        {
            RowDefinition newrow = new RowDefinition();
            newrow.Height = new GridLength(50);
            AssignmentListTable.RowDefinitions.Add(newrow);
            RowDefinition newcheckrow = new RowDefinition();
            newcheckrow.Height = new GridLength(50);
            AssingmentListTableRemove.RowDefinitions.Add(newcheckrow);

            TextBox txtbox = new TextBox();
            txtbox.FontSize = 36;
            txtbox.Name = EditRowCounter.ToString();
            txtbox.FontFamily = new FontFamily("/Assets/ReginaScript.ttf#Regina Script");
            txtbox.PlaceholderText = "Enter Assignment title";
            txtbox.SetValue(Grid.RowProperty, EditRowCounter);
            txtbox.SetValue(Grid.ColumnProperty, 0);

            CheckBox checkbx = new CheckBox();
            checkbx.Name = EditRowCounter.ToString();
            checkbx.SetValue(Grid.RowProperty, EditRowCounter);
            checkbx.SetValue(Grid.ColumnProperty, 0);
            AssingmentListTableRemove.Children.Add(checkbx);

            AssignmentListTable.Children.Add(txtbox);

            EditRowCounter++;
        }

        private async void EditSubmitAssignments_Click(object sender, RoutedEventArgs e)
        {
            string validassignment = "";
            List<int> assignmentids = new List<int>();
            List<int> newassignmentids = new List<int>();
            bool validinput = true;
            foreach (TextBox assignment in AssignmentListTable.Children)
            {
                if (assignment.Text == "")
                {
                    validinput = false;
                }
            }
            if (validinput == true)
            {//remove the all assignments from the course
             //add all the assignments created
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE SemesterID = {EditSelectedSemesterID} AND CourseID = {EditSelectedCourseID};";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    assignmentids.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                        foreach (TextBox txtbox in AssignmentListTable.Children)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE SemesterID = {EditSelectedSemesterID} AND CourseID = {EditSelectedCourseID} AND AssignmentTitle = '{txtbox.Text}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        validassignment += reader.GetValue(0);
                                    }
                                }
                            }
                            if (validassignment != "")
                            {//update
                                int assignmentid = Int32.Parse(validassignment);
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand cmd2 = new SqlCommand($"UPDATE Assignments SET AssignmentTitle = '{txtbox.Text}' WHERE AssignmentID = {assignmentid};", sqlConn);
                                adapter.UpdateCommand = cmd2;
                                adapter.UpdateCommand.ExecuteNonQuery();
                                newassignmentids.Add(assignmentid);//adds the assignment id of the updated assignment
                            }
                            else
                            {//insert 
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"INSERT INTO Assignments VALUES ({EditSelectedSemesterID},{EditSelectedCourseID},'{txtbox.Text}');", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get the assignment id of the new inserted assignment
                                    cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE SemesterID = {EditSelectedSemesterID} AND CourseID = {EditSelectedCourseID} AND AssignmentTitle = '{txtbox.Text}';";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            newassignmentids.Add((int)reader.GetValue(0));
                                        }
                                    }
                                }
                            }
                            validassignment = "";
                        }
                        foreach (var id in assignmentids)
                        {
                            if (newassignmentids.Contains(id) == false)
                            {//if original id does not exist in the list of new ids DELETE the assignment
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"DELETE FROM Grades WHERE AssignmentID = {id};", sqlConn);
                                adapter.DeleteCommand = command;
                                adapter.DeleteCommand.ExecuteNonQuery();

                                adapter = new SqlDataAdapter();
                                command = new SqlCommand($"DELETE FROM Assignments WHERE AssignmentID = {id};", sqlConn);
                                adapter.DeleteCommand = command;
                                adapter.DeleteCommand.ExecuteNonQuery();
                            }
                        }
                        sqlConn.Close();
                    }
                }
                purgeAssignmentsTable();
                EditAddAssignment.Visibility = Visibility.Collapsed;
                EditRemoveAssignment.Visibility = Visibility.Collapsed;
                EditSubmitAssignments.Visibility = Visibility.Collapsed;
                EditCancel.Visibility = Visibility.Collapsed;
                EditRemoveByCheck.Visibility = Visibility.Collapsed;
                SetupAssignmentsTable("Form2C", -1);
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("Please do not leave any assignment title empty.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private async void EditRemoveByCheck_Click(object sender, RoutedEventArgs e)
        {
            List<int> targetlist = new List<int>();
            int count = 0;
            foreach (CheckBox box in AssingmentListTableRemove.Children)
            {//count how many checkboxes are checked and add the index to a list
                if (box.IsChecked == true)
                {
                    targetlist.Add(count);
                }
                count++;
            }
            if (targetlist.Count > 0)
            {//if the list of indexes is bigger than 0
                foreach (int item in targetlist)
                {//remove the item by index and decrement the edit row counter and row definitions
                    --EditRowCounter;
                    AssignmentListTable.RowDefinitions.RemoveAt(EditRowCounter);
                    AssingmentListTableRemove.RowDefinitions.RemoveAt(EditRowCounter);

                    foreach (TextBox txt in AssignmentListTable.Children)
                    {
                        if (txt.Name == $"{item}")
                        {
                            AssignmentListTable.Children.Remove(txt);
                        }
                    }
                    foreach (CheckBox chkbox in AssingmentListTableRemove.Children)
                    {
                        if (chkbox.Name == $"{item}")
                        {
                            AssingmentListTableRemove.Children.Remove(chkbox);
                        }
                    }
                }
                EditRowCounter = 0;
                int counter2 = 0;
                foreach (TextBox test in AssignmentListTable.Children)
                {//update the name and position of each textbox
                    test.Name = EditRowCounter.ToString();
                    test.SetValue(Grid.RowProperty, EditRowCounter);
                    EditRowCounter++;
                }
                foreach (CheckBox box in AssingmentListTableRemove.Children)
                {//update the name and position of each box
                    box.Name = counter2.ToString();
                    box.IsChecked = false;
                    box.SetValue(Grid.RowProperty, counter2);
                    counter2++;
                }
            }
            else
            {//no boxes were checked
                var Removeassignmenterror = new MessageDialog("No assignments were selected.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private void Form3CCancel_Click(object sender, RoutedEventArgs e)
        {
            purgeStudentNameTable();
        }

        private void purgeStudentNameTable()
        {
            Form3C.Visibility = Visibility.Collapsed;
            Form3CTableTitle1.Visibility = Visibility.Collapsed;
            Form3CTableTitle2.Visibility = Visibility.Collapsed;
            Form3CSubmit.Visibility = Visibility.Collapsed;
            StudentNameTable.Children.Clear();
            StudentNameTable.RowDefinitions.Clear();
        }

        private async void Form3CSubmit_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            List<double> candidateGrades = new List<double>();
            List<int> ids = new List<int>();
            string validupdate = "";
            foreach (var item in StudentNameTable.Children)
            {
                Border tb = item as Border;
                TextBox txtbox = tb.Child as TextBox;
                if (txtbox != null)
                {
                    if (txtbox.Text != "")
                    {
                        double.TryParse(txtbox.Text, out double inputgrade);
                        candidateGrades.Add(inputgrade);
                        Int32.TryParse(txtbox.Name, out int studentid);
                        ids.Add(studentid);
                    }
                }
            }
            if (candidateGrades.Count > 0)
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {//acquire the all students enrolled in the course 
                        foreach (var id in ids)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Grade FROM Grades WHERE AssignmentID = {SelectedAssignmentID} AND StudentHUID = {id}";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        validupdate += reader.GetValue(0).ToString();
                                    }
                                }
                            }
                            if (validupdate == "")
                            {//we can insert
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"INSERT INTO Grades VALUES ({SelectedAssignmentID},{id},{candidateGrades[index]});", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                            }
                            else
                            {//we have to update
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand cmd2 = new SqlCommand($"UPDATE Grades SET Grade = {candidateGrades[index]} WHERE AssignmentID = {SelectedAssignmentID} AND StudentHUID = {id};", sqlConn);
                                adapter.UpdateCommand = cmd2;
                                adapter.UpdateCommand.ExecuteNonQuery();
                            }
                            updateFinalGrade(candidateGrades[index], id);
                            index++;
                            validupdate = "";
                        }
                        sqlConn.Close();
                    }
                }
                var GradeInsert = new MessageDialog("Grades Successfully Inserted");
                await GradeInsert.ShowAsync();
                purgeStudentNameTable();
            }
            else
            {
                var NoGrades = new MessageDialog("Please provide at least one grade");
                await NoGrades.ShowAsync();
            }
        }

        private void Form4CCancel_Click(object sender, RoutedEventArgs e)
        {
            Form4C.Visibility = Visibility.Collapsed;
            Form4CCoursesTable.Children.Clear();
            Form4CCoursesTable.RowDefinitions.Clear();
        }

        private void Form4CSubmit_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string grade = "";
            List<double> candidateGrades = new List<double>();
            List<int> assignmentids = new List<int>();
            foreach (var item in Form4CCoursesTable.Children)
            {
                Border tb = item as Border;
                TextBox txtbox = tb.Child as TextBox;
                if (txtbox != null)
                {
                    if (txtbox.Text != "")
                    {
                        double.TryParse(txtbox.Text, out double inputgrade);
                        candidateGrades.Add(inputgrade);
                        Int32.TryParse(txtbox.Name, out int assignmentid);
                        assignmentids.Add(assignmentid);
                    }
                }
            }
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the all students enrolled in the course 
                    foreach (var id in assignmentids)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT Grade FROM Grades WHERE AssignmentID = {id} AND StudentHUID = {Form4CSelectedStudentHUID}";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    grade += reader.GetValue(0).ToString();
                                }
                            }
                        }
                        if (grade == "")
                        {//insert the grade
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"INSERT INTO Grades VALUES ({id},{Form4CSelectedStudentHUID},{candidateGrades[index]});", sqlConn);
                            adapter.InsertCommand = command;
                            adapter.InsertCommand.ExecuteNonQuery();
                        }
                        else
                        {//update the grade
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand cmd2 = new SqlCommand($"UPDATE Grades SET Grade = {candidateGrades[index]} WHERE AssignmentID = {id} AND StudentHUID = {Form4CSelectedStudentHUID};", sqlConn);
                            adapter.UpdateCommand = cmd2;
                            adapter.UpdateCommand.ExecuteNonQuery();
                        }
                        updateFinalGrade(candidateGrades[index], -1);
                        index++;
                        grade = "";
                    }
                    sqlConn.Close();
                }
            }
            Form4CCoursesTable.Children.Clear();
            Form4CCoursesTable.RowDefinitions.Clear();
            SetupAssignmentsTable("Form4C", Form4CSelectedStudentHUID);
            SetupFinalGradeText(Form4CSelectedStudentHUID);
        }

        private void updateFinalGrade(double insertedgrade, int studentHUID)
        {
            decimal currpoints = 0;
            decimal ttlpoints = 0;
            string grade = "";
            List<int> assignmentids = new List<int>();
            List<decimal> grades = new List<decimal>();
            if (studentHUID == -1)
            {
                studentHUID = Form4CSelectedStudentHUID;
            }

            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the all students enrolled in the course 
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT CurrentGrade FROM FinalGrade WHERE CourseID = {EditSelectedCourseID} AND StudentHUID = {studentHUID}";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                grade += reader.GetValue(0).ToString();
                            }
                        }
                    }
                    if (grade == "")
                    {//insert
                        double validgrade = (insertedgrade / 100) * 100;
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand($"INSERT INTO FinalGrade VALUES ({studentHUID},{EditSelectedCourseID},{insertedgrade},{100},{0},{validgrade});", sqlConn);
                        adapter.InsertCommand = command;
                        adapter.InsertCommand.ExecuteNonQuery();
                    }
                    else
                    {//update
                        //first we find out all the assignments with the courseid
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE CourseID = {EditSelectedCourseID} AND SemesterID = {EditSelectedSemesterID}";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    assignmentids.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                        //find all the grades with the list of assignment ids for the selected student
                        foreach (var assignmentid in assignmentids)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Grade FROM Grades WHERE StudentHUID = {studentHUID} AND AssignmentID = {assignmentid}";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        grades.Add((decimal)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                        foreach (var g in grades)
                        {//totals all the grade points
                            currpoints += g;
                        }
                        ttlpoints = grades.Count * 100;
                        double validgrade = (double)currpoints / (double)ttlpoints * 100;
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand cmd2 = new SqlCommand($"UPDATE FinalGrade SET CurrentPoints = {currpoints}, TotalPoints = {ttlpoints}, CurvePoints = {0}, CurrentGrade = {validgrade}  WHERE CourseID = {EditSelectedCourseID} AND StudentHUID = {studentHUID};", sqlConn);
                        adapter.UpdateCommand = cmd2;
                        adapter.UpdateCommand.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                }
            }
        }

        private void Form2CAllStudents_Click(object sender, RoutedEventArgs e)
        {//I have access to 2Csemesterid and 2c courseid
            Form5C.Visibility = Visibility.Visible;
            List<int> studentids = new List<int>();
            List<string> firstlastname = new List<string>();
            List<int> sortedstudentids = new List<int>();
            int rowcounter = 0;
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {//Populates the form5c student roster
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {//acquire the all students enrolled in the course 
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT StudentID FROM StudentEnrolledCourses WHERE CourseID = {EditSelectedCourseID} AND SemesterID = {EditSelectedSemesterID}";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentids.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if(studentids.Count > 0)
                    {//populate the table
                        foreach(var id in studentids)
                        {//get name of student 
                            using(SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT FirstName, LastName FROM Users WHERE HUID = {id};";
                                using(SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        firstlastname.Add(reader.GetValue(0)+" "+reader.GetValue(1));
                                    }
                                }
                            }
                        }
                        firstlastname.Sort();//sorts the names alphabetically
                        foreach(var name in firstlastname)
                        {
                            string[] lfname = name.Split(" ");
                            using(SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"Select HUID FROM Users WHERE FirstName = '{lfname[0]}' AND LastName = '{lfname[1]}';";
                                using(SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while(reader.Read())
                                    {
                                        sortedstudentids.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                        foreach (var id in sortedstudentids.ToList())
                        {//filter the ids
                            if (studentids.Contains(id) == false)
                            {
                                sortedstudentids.Remove(id);
                            }
                        }
                        foreach(var student in firstlastname)
                        {
                            RowDefinition rd = new RowDefinition();
                            rd.Height = new GridLength(50);
                            Form5CStudentRoster.RowDefinitions.Add(rd);

                            Border myborder = new Border();
                            myborder.BorderThickness = new Thickness(2);
                            myborder.BorderBrush = new SolidColorBrush(Colors.Black);
                            myborder.SetValue(Grid.RowProperty, rowcounter);
                            myborder.SetValue(Grid.ColumnProperty, 0);

                            Button btn = new Button();
                            btn.FontSize = 36;
                            btn.Foreground = new SolidColorBrush(Colors.Black);
                            btn.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                            btn.HorizontalAlignment = HorizontalAlignment.Center;
                            btn.VerticalAlignment = VerticalAlignment.Center;
                            btn.Content = student;
                            btn.Name = sortedstudentids[rowcounter].ToString();//assigns student id to button name
                            btn.Click += SetupSelectedStudentGrades;

                            myborder.Child = btn;
                            Form5CStudentRoster.Children.Add(myborder);
                            rowcounter++;
                        }
                    }
                    else
                    {//inform the user that no students are enrolled in the course
                        RowDefinition rd = new RowDefinition();
                        rd.Height = new GridLength(50);
                        Form5CStudentRoster.RowDefinitions.Add(rd);

                        TextBlock txtblock = new TextBlock();
                        txtblock.FontSize = 50;
                        txtblock.Foreground = new SolidColorBrush(Colors.Black);
                        txtblock.Text = "No Students are currently enrolled in this course.";
                        txtblock.FontFamily = new FontFamily("/Assets/ReginaScript.ttf#Regina Script");
                        txtblock.SetValue(Grid.RowProperty, 0);
                        txtblock.SetValue(Grid.ColumnProperty, 0);
                        Form5CStudentRoster.Children.Add(txtblock);
                    }
                    sqlConn.Close();
                }
            }
        }

        private void Form5CCancel_Click(object sender, RoutedEventArgs e)
        {
            Form5C.Visibility = Visibility.Collapsed;
            Form5CStudentRoster.RowDefinitions.Clear();
            Form5CStudentRoster.Children.Clear();
        }
    }
}