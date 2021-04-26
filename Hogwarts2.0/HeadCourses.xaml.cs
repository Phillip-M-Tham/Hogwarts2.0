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
    public sealed partial class HeadCourses : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private int CourseRowCounter = 0;

        public HeadCourses()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            setupCourses(1);
        }

        private void setupCourses(int mode)
        {
            List<int> CourseIDs = new List<int>();
            CourseRowCounter = 0;
            List<string> Courses = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Title,CourseID FROM Courses;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Courses.Add(reader.GetValue(0).ToString());
                                CourseIDs.Add((int)reader.GetValue(1));
                            }
                        }
                    }
                }
                sqlConn.Close();
            }
            if (Courses.Count > 0)
            {
                foreach (var course in Courses)
                {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    if (mode == 1)
                    {
                        CoursesTable1A.RowDefinitions.Add(newrow);

                        TextBlock textblock = new TextBlock();
                        textblock.SetValue(Grid.RowProperty, CourseRowCounter);
                        textblock.SetValue(Grid.ColumnProperty, 0);
                        textblock.HorizontalAlignment = HorizontalAlignment.Center;
                        textblock.VerticalAlignment = VerticalAlignment.Center;
                        textblock.Foreground = new SolidColorBrush(Colors.Black);
                        textblock.Text = course;
                        textblock.FontSize = 36;
                        textblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");

                        CoursesTable1A.Children.Add(textblock);
                    }
                    else if (mode == 2)
                    {
                        RowDefinition newrow2 = new RowDefinition();
                        newrow2.Height = new GridLength(50);

                        CoursesTable2A.RowDefinitions.Add(newrow);
                        CoursesTable2Achecks.RowDefinitions.Add(newrow2);

                        CheckBox chk = new CheckBox();
                        chk.HorizontalAlignment = HorizontalAlignment.Center;
                        chk.VerticalAlignment = VerticalAlignment.Center;
                        chk.Name = CourseIDs[CourseRowCounter].ToString();
                        chk.SetValue(Grid.RowProperty, CourseRowCounter);
                        chk.SetValue(Grid.ColumnProperty, 0);
                        chk.IsChecked = false;

                        Border bd = new Border();
                        bd.BorderThickness = new Thickness(2);
                        bd.BorderBrush = new SolidColorBrush(Colors.Black);
                        bd.SetValue(Grid.RowProperty, CourseRowCounter);
                        bd.SetValue(Grid.ColumnProperty, 0);

                        TextBox txtbox = new TextBox();
                        txtbox.HorizontalAlignment = HorizontalAlignment.Center;
                        txtbox.VerticalAlignment = VerticalAlignment.Center;
                        txtbox.Foreground = new SolidColorBrush(Colors.Black);
                        txtbox.Width = 500;
                        txtbox.Text = course;
                        txtbox.TextAlignment = TextAlignment.Center;
                        txtbox.FontSize = 36;
                        txtbox.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        bd.Child = txtbox;

                        CoursesTable2Achecks.Children.Add(chk);
                        CoursesTable2A.Children.Add(bd);
                    }
                    CourseRowCounter++;
                }
            }
            else
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);

                TextBlock textblock = new TextBlock();
                textblock.SetValue(Grid.RowProperty, 0);
                textblock.SetValue(Grid.ColumnProperty, 0);
                textblock.HorizontalAlignment = HorizontalAlignment.Center;
                textblock.VerticalAlignment = VerticalAlignment.Center;
                textblock.Foreground = new SolidColorBrush(Colors.Black);
                textblock.FontSize = 36;
                textblock.Text = "No Courses Available";
                textblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");

                if (mode == 1)
                {
                    CoursesTable1A.RowDefinitions.Add(newrow);
                    CoursesTable1A.Children.Add(textblock);
                }
                else if (mode == 2)
                {
                    CoursesTable2A.RowDefinitions.Add(newrow);
                    CoursesTable2A.Children.Add(textblock);
                }
            }
        }

        private void Edit1A(object sender, RoutedEventArgs e)
        {
            if (CoursesTable2A.Visibility == Visibility.Collapsed)
            {
                CoursesTable2Achecks.Visibility = Visibility.Visible;
                CoursesTable2A.Visibility = Visibility.Visible;
                EditAddCourse.Visibility = Visibility.Visible;
                EditRemoveCourse.Visibility = Visibility.Visible;
                EditSubmitCourses.Visibility = Visibility.Visible;
                EditRemoveByCheck.Visibility = Visibility.Visible;
                EditCancel.Visibility = Visibility.Visible;
                PurgeCourselist(1);
                CoursesTable1A.Visibility = Visibility.Collapsed;
                setupCourses(2);
            }
        }

        private void EditAddCourse_Click(object sender, RoutedEventArgs e)
        {
            Form2A.Visibility = Visibility.Visible;
            Form1A.Visibility = Visibility.Collapsed;
            SetupForm2A();
        }

        private async void SetupForm2A()
        {//populate profs and deps
            int count = 0;
            List<int> ProfHUIDs = new List<int>();
            List<string> Professors = new List<string>();
            List<string> Departments = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT HUID FROM Faculty WHERE PositionType = 'P';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProfHUIDs.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if (ProfHUIDs.Count > 0)
                    {//get names from huids
                        foreach (var id in ProfHUIDs)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Lastname FROM Users WHERE HUID = {id};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Professors.Add("Professor " + reader.GetValue(0));
                                    }
                                }
                            }
                        }
                    }
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Department FROM Departments";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Departments.Add(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (ProfHUIDs.Count > 0)
            {
                foreach (var name in Professors)
                {
                    TextBlock prof = new TextBlock();
                    prof.Name = ProfHUIDs[count].ToString();
                    prof.Text = name;
                    prof.Foreground = new SolidColorBrush(Colors.White);
                    Form2AProfessor.Items.Add(prof);
                    count++;
                }
            }
            else
            {
                Form2AProfessor.Items.Add("No Professors Found");
                var ermessage = new MessageDialog("No Professors Have Been Hired");
                await ermessage.ShowAsync();
            }
            if (Departments.Count > 0)
            {
                foreach (var dept in Departments)
                {
                    ValidDepartmentsInput.Items.Add(dept);
                }
            }
            else
            {
                ValidDepartmentsInput.Items.Add("No Departments Found");
                var ermessage2 = new MessageDialog("No Departments Have Been Created");
                await ermessage2.ShowAsync();
            }
        }

        private async void EditRemoveCourse_Click(object sender, RoutedEventArgs e)
        {
            if (CourseRowCounter != 0)
            {//get rid of the current row
                --CourseRowCounter;
                CoursesTable2A.Children.RemoveAt(CourseRowCounter);
                CoursesTable2A.RowDefinitions.RemoveAt(CourseRowCounter);

                CoursesTable2Achecks.Children.RemoveAt(CourseRowCounter);
                CoursesTable2Achecks.RowDefinitions.RemoveAt(CourseRowCounter);
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("No Courses exist.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private async void EditSubmitCourses_Click(object sender, RoutedEventArgs e)
        {
            bool validinput = true;
            List<string> InputCourseTitles = new List<string>();
            List<int> OriginalCourseIDs = new List<int>();
            int selectedCrsID = -1;
            bool needsDelete = false;
            List<int> NewCourseIDs = new List<int>();
            foreach (var bd in CoursesTable2A.Children)
            {
                Border test = bd as Border;
                if (test != null)
                {
                    TextBox tbx = test.Child as TextBox;
                    if (tbx.Text == "")
                    {
                        validinput = false;
                    }
                    else
                    {
                        InputCourseTitles.Add(tbx.Text);
                    }
                }
            }
            if (validinput == true)
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT CourseID FROM Courses;";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    OriginalCourseIDs.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                        foreach (var title in InputCourseTitles)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT CourseID FROM Courses WHERE Title = '{title}';";
                                {
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            selectedCrsID = (int)reader.GetValue(0);
                                        }
                                    }
                                }
                            }
                            if (selectedCrsID != -1)
                            {//update
                                needsDelete = true;
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand cmd2 = new SqlCommand($"UPDATE Courses SET Title = '{title}' WHERE CourseID = {selectedCrsID};", sqlConn);
                                adapter.UpdateCommand = cmd2;
                                adapter.UpdateCommand.ExecuteNonQuery();
                                NewCourseIDs.Add(selectedCrsID);
                            }
                            selectedCrsID = -1;
                        }
                        foreach (var id in OriginalCourseIDs)
                        {
                            if (NewCourseIDs.Contains(id) == false)
                            {
                                needsDelete = true;
                                DeleteCoursebyID(id);
                            }
                        }
                        sqlConn.Close();
                    }
                }
                if (needsDelete == true)
                {
                    var ValidRemoveassignment = new MessageDialog("Successfully Updated Semesters");
                    await ValidRemoveassignment.ShowAsync();
                    PurgeCourselist(2);
                    setupCourses(2);
                }
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("Please do not leave any Course titles empty.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private void DeleteCoursebyID(int id)
        {
            List<int> RemoveAssingmentIDs = new List<int>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE CourseID = {id};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RemoveAssingmentIDs.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if (RemoveAssingmentIDs.Count > 0)
                    {
                        foreach (var AssignmentID in RemoveAssingmentIDs)
                        {//DELETES THE GRADES ASSOCIATED TO COURSEID 
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"DELETE FROM Grades WHERE AssignmentID = {AssignmentID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();
                        }
                    }
                    SqlDataAdapter adapter2 = new SqlDataAdapter();
                    SqlCommand command2 = new SqlCommand($"DELETE FROM Times WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    adapter2 = new SqlDataAdapter();
                    command2 = new SqlCommand($"DELETE FROM Locations WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    adapter2 = new SqlDataAdapter();
                    command2 = new SqlCommand($"DELETE FROM ReqMaterials WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    adapter2 = new SqlDataAdapter();
                    command2 = new SqlCommand($"DELETE FROM DayTypes WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    adapter2 = new SqlDataAdapter();
                    command2 = new SqlCommand($"DELETE FROM Assignments WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    adapter2 = new SqlDataAdapter();
                    command2 = new SqlCommand($"DELETE FROM FinalGrade WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    adapter2 = new SqlDataAdapter();
                    command2 = new SqlCommand($"DELETE FROM SemesterCourses WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    adapter2 = new SqlDataAdapter();
                    command2 = new SqlCommand($"DELETE FROM StudentEnrolledCourses WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    adapter2 = new SqlDataAdapter();
                    command2 = new SqlCommand($"DELETE FROM Courses WHERE CourseID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    sqlConn.Close();
                }
            }
        }

        private void EditCancel_Click(object sender, RoutedEventArgs e)
        {
            EditAddCourse.Visibility = Visibility.Collapsed;
            EditRemoveCourse.Visibility = Visibility.Collapsed;
            EditSubmitCourses.Visibility = Visibility.Collapsed;
            EditRemoveByCheck.Visibility = Visibility.Collapsed;
            EditCancel.Visibility = Visibility.Collapsed;
            PurgeCourselist(2);
            CoursesTable2A.Visibility = Visibility.Collapsed;
            CoursesTable2Achecks.Visibility = Visibility.Collapsed;
            CoursesTable1A.Visibility = Visibility.Visible;
            setupCourses(1);
        }

        private void PurgeCourselist(int mode)
        {
            if (mode == 1)
            {
                CoursesTable1A.RowDefinitions.Clear();
                CoursesTable1A.Children.Clear();
            }
            else
            {
                CoursesTable2A.RowDefinitions.Clear();
                CoursesTable2A.Children.Clear();
                CoursesTable2Achecks.RowDefinitions.Clear();
                CoursesTable2Achecks.Children.Clear();
            }
        }

        private async void EditRemoveByCheck_Click(object sender, RoutedEventArgs e)
        {
            List<CheckBox> targetlist = new List<CheckBox>();
            foreach (CheckBox box in CoursesTable2Achecks.Children)
            {//count how many checkboxes are checked and add the index to a list
                if (box.IsChecked == true)
                {
                    targetlist.Add(box);
                }
            }
            if (targetlist.Count > 0)
            {
                foreach (CheckBox target in targetlist)
                {
                    Int32.TryParse(target.Name, out int targetid);
                    DeleteCoursebyID(targetid);
                }
                var ValidRemoveassignment = new MessageDialog("Successfully Updated Courses");
                await ValidRemoveassignment.ShowAsync();
                PurgeCourselist(2);
                setupCourses(2);
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("No Courses were selected.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private void Form2ACancel_Click(object sender, RoutedEventArgs e)
        {
            Form2A.Visibility = Visibility.Collapsed;
            Form1A.Visibility = Visibility.Visible;
            ResetForm2A();
            PurgeCourselist(2);
            setupCourses(2);
        }

        private void ResetForm2A()
        {
            Form2AProfessor.Items.Clear();
            CourseInsertTitleInput.Text = "";
            CourseInsertTypeInput.Text = "";
            ValidDepartmentsInput.Items.Clear();
            YearlevelInput.SelectedItem = default;
            CourseInsertInfoInput.Text = "";
        }

        private async void SubmitCreateCourse_Click(object sender, RoutedEventArgs e)
        {
            int validCourseInput = 0;
            string errormessage = "";
            bool isvalidTitle;
            if (Form2AProfessor.SelectedValue != null)
            {
                if (Form2AProfessor.SelectedValue.ToString() == "No Professors Found")
                {
                    errormessage += "\nPlease Add Proffesors to Faculty";
                }
            }
            else
            {
                errormessage += "\nPlease Select A Professor";
                validCourseInput++;
            }
            if (CourseInsertTitleInput.Text != "")
            {
                isvalidTitle = CheckCourseTitle(CourseInsertTitleInput.Text);
                if (isvalidTitle == false)
                {
                    errormessage += "\nCourse Title Already Taken. Please Provide A Different Course Title";
                    validCourseInput++;
                }
            }
            else
            {
                errormessage += "\nPlease Provide A Course Title";
                validCourseInput++;
            }
            if (CourseInsertTypeInput.Text == "")
            {
                errormessage += "\nPlease Provide A Course Type";
                validCourseInput++;
            }
            if (ValidDepartmentsInput.SelectedItem != null)
            {
                if (ValidDepartmentsInput.SelectedValue.ToString() == "No Departments Found")
                {
                    errormessage += "\nPlease Create Some Departments";
                }
            }
            else
            {
                errormessage += "\nPlease Select A Department";
                validCourseInput++;
            }
            if (YearlevelInput.SelectedItem == null)
            {
                errormessage += "\nPlease Select A Year Level";
                validCourseInput++;
            }


            if (validCourseInput == 0)
            {
                TextBlock test = Form2AProfessor.SelectedItem as TextBlock;
                int.TryParse(test.Name, out int ProfHuid);
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlCommand command = new SqlCommand($"INSERT INTO Courses VALUES ({ProfHuid},'{CourseInsertTitleInput.Text}','{CourseInsertTypeInput.Text}','{ValidDepartmentsInput.SelectedValue}',{YearlevelInput.SelectedValue},'{CourseInsertInfoInput.Text}');", sqlConn);
                        adapter.InsertCommand = command;
                        adapter.InsertCommand.ExecuteNonQuery();
                        var coursecreated = new MessageDialog($"Course {CourseInsertTitleInput.Text} successfully created");
                        await coursecreated.ShowAsync();
                        sqlConn.Close();
                    }
                }
                ResetForm2A();
            }
            else
            {
                var ermessage2 = new MessageDialog(errormessage);
                await ermessage2.ShowAsync();
            }

        }

        private bool CheckCourseTitle(string coursename)
        {
            bool validname = true;
            string result = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Title FROM Courses WHERE Title = '{coursename}';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result = reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (result != "")
            {
                validname = false;
            }
            return validname;
        }
    }
}