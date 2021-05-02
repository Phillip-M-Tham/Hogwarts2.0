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
    public sealed partial class HeadDepartments : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private int DepartmentRowCounter;
        public HeadDepartments()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            setupdepartments(1);
        }

        private void setupdepartments(int mode)
        {
            List<int> sorteddepids = new List<int>();
            DepartmentRowCounter = 0;
            List<string> departments = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Department FROM Departments;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                departments.Add(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    if (departments.Count > 0)
                    {
                        departments.Sort();
                        foreach (var dep in departments)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT DepartmentID FROM Departments WHERE Department = '{dep}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        sorteddepids.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (departments.Count > 0)
            {
                foreach (var dept in departments)
                {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    if (mode == 1)
                    {
                        DepartmentsTable1A.RowDefinitions.Add(newrow);

                        TextBlock textblock = new TextBlock();
                        textblock.SetValue(Grid.RowProperty, DepartmentRowCounter);
                        textblock.SetValue(Grid.ColumnProperty, 0);
                        textblock.HorizontalAlignment = HorizontalAlignment.Center;
                        textblock.VerticalAlignment = VerticalAlignment.Center;
                        textblock.Foreground = new SolidColorBrush(Colors.Black);
                        textblock.Text = dept;
                        textblock.FontSize = 36;
                        textblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");

                        DepartmentsTable1A.Children.Add(textblock);
                    }
                    else if (mode == 2)
                    {
                        RowDefinition newrow2 = new RowDefinition();
                        newrow2.Height = new GridLength(50);

                        DepartmentsTable2A.RowDefinitions.Add(newrow);
                        DepartmentsTable2Achecks.RowDefinitions.Add(newrow2);

                        CheckBox chk = new CheckBox();
                        chk.HorizontalAlignment = HorizontalAlignment.Center;
                        chk.VerticalAlignment = VerticalAlignment.Center;
                        chk.Name = sorteddepids[DepartmentRowCounter].ToString();
                        chk.SetValue(Grid.RowProperty, DepartmentRowCounter);
                        chk.SetValue(Grid.ColumnProperty, 0);
                        chk.IsChecked = false;

                        Border bd = new Border();
                        bd.BorderThickness = new Thickness(2);
                        bd.BorderBrush = new SolidColorBrush(Colors.Black);
                        bd.SetValue(Grid.RowProperty, DepartmentRowCounter);
                        bd.SetValue(Grid.ColumnProperty, 0);

                        TextBox txtbox = new TextBox();
                        txtbox.HorizontalAlignment = HorizontalAlignment.Center;
                        txtbox.VerticalAlignment = VerticalAlignment.Center;
                        txtbox.Foreground = new SolidColorBrush(Colors.Black);
                        txtbox.Width = 500;
                        txtbox.Text = dept;
                        txtbox.TextAlignment = TextAlignment.Center;
                        txtbox.FontSize = 36;
                        txtbox.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        bd.Child = txtbox;

                        DepartmentsTable2Achecks.Children.Add(chk);
                        DepartmentsTable2A.Children.Add(bd);
                    }
                    DepartmentRowCounter++;
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
                textblock.Text = "No Departments Available";
                textblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");

                if (mode == 1)
                {
                    DepartmentsTable1A.RowDefinitions.Add(newrow);
                    DepartmentsTable1A.Children.Add(textblock);
                }
            }
        }

        private void Edit1A(object sender, RoutedEventArgs e)
        {
            if (DepartmentsTable2A.Visibility == Visibility.Collapsed)
            {
                DepartmentsTable2Achecks.Visibility = Visibility.Visible;
                DepartmentsTable2A.Visibility = Visibility.Visible;
                EditAddAssignment.Visibility = Visibility.Visible;
                EditRemoveAssignment.Visibility = Visibility.Visible;
                EditSubmitAssignments.Visibility = Visibility.Visible;
                EditRemoveByCheck.Visibility = Visibility.Visible;
                EditCancel.Visibility = Visibility.Visible;
                Purgedepartmentlist(1);
                DepartmentsTable1A.Visibility = Visibility.Collapsed;
                setupdepartments(2);
            }
        }

        private void Purgedepartmentlist(int mode)
        {
            if (mode == 1)
            {
                DepartmentsTable1A.RowDefinitions.Clear();
                DepartmentsTable1A.Children.Clear();
            }
            else
            {
                DepartmentsTable2A.Children.Clear();
                DepartmentsTable2A.RowDefinitions.Clear();

                DepartmentsTable2Achecks.Children.Clear();
                DepartmentsTable2Achecks.RowDefinitions.Clear();
            }
        }

        private async void EditRemoveByCheck_Click(object sender, RoutedEventArgs e)
        {
            int targetid;
            List<CheckBox> targetlist = new List<CheckBox>();
            foreach (CheckBox box in DepartmentsTable2Achecks.Children)
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
                    Int32.TryParse(target.Name, out targetid);
                    DeleteDepartmentbyID(targetid);
                }
                var ValidRemoveassignment = new MessageDialog("Successfully Updated Departments");
                await ValidRemoveassignment.ShowAsync();
                Purgedepartmentlist(2);
                setupdepartments(2);
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("No Departments were selected.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private async void EditSubmitAssignments_Click(object sender, RoutedEventArgs e)
        {         
            bool validinput = true;
            int selectedDeptID = -1;
            bool needsDelete = false;
            List<string> InputDeptTitles = new List<string>();
            List<int> originalDeptIDs = new List<int>();
            List<int> newDeptIDs = new List<int>();
            foreach (var bd in DepartmentsTable2A.Children)
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
                        InputDeptTitles.Add(tbx.Text);
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
                            cmd.CommandText = $"SELECT departmentID FROM Departments;";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    originalDeptIDs.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                        foreach (var title in InputDeptTitles)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT DepartmentID FROM Departments WHERE Department = '{title}';";
                                {
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            selectedDeptID = (int)reader.GetValue(0);
                                        }
                                    }
                                }
                            }
                            if (selectedDeptID != -1)
                            {//update
                                needsDelete = true;
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand cmd2 = new SqlCommand($"UPDATE Departments SET Department = '{title}' WHERE DepartmentID = {selectedDeptID};", sqlConn);
                                adapter.UpdateCommand = cmd2;
                                adapter.UpdateCommand.ExecuteNonQuery();
                                newDeptIDs.Add(selectedDeptID);
                            }
                            else
                            {//insert
                                needsDelete = true;
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"INSERT INTO Departments VALUES ('{title}');", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get the assignment id of the new inserted assignment
                                    cmd.CommandText = $"SELECT DepartmentID FROM Departments WHERE Department = '{title}';";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            newDeptIDs.Add((int)reader.GetValue(0));
                                        }
                                    }
                                }
                            }
                            selectedDeptID = -1;
                        }
                        foreach (var id in originalDeptIDs)
                        {
                            if (newDeptIDs.Contains(id) == false)
                            {
                                needsDelete = true;
                                DeleteDepartmentbyID(id);
                            }
                        }
                        sqlConn.Close();
                    }
                }
                if (needsDelete == true)
                {
                    var ValidRemoveassignment = new MessageDialog("Successfully Updated Departments");
                    await ValidRemoveassignment.ShowAsync();
                    Purgedepartmentlist(2);
                    setupdepartments(2);
                }
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("Please do not leave any department titles empty.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private void DeleteDepartmentbyID(int id)
        {
            List<int> RemoveAssignmentID = new List<int>();
            List<int> RemoveCourseIDs = new List<int>();
            string RemoveDeptName = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {//acquired the dept name
                        cmd.CommandText = $"SELECT Department FROM Departments WHERE DepartmentID = {id};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RemoveDeptName = reader.GetValue(0).ToString();
                            }
                        }
                    }
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT CourseID FROM Courses WHERE Department ='{RemoveDeptName}';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RemoveCourseIDs.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if (RemoveCourseIDs.Count > 0)
                    {
                        foreach (var CID in RemoveCourseIDs)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE CourseID = {CID};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        RemoveAssignmentID.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                    }
                    if (RemoveAssignmentID.Count > 0)
                    {
                        foreach (var AssignmentID in RemoveAssignmentID)
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"DELETE FROM Grades WHERE AssignmentID = {AssignmentID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();
                        }
                    }
                    if (RemoveCourseIDs.Count > 0)
                    {
                        foreach (var CourseID in RemoveCourseIDs)
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"DELETE FROM Assignments WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            adapter = new SqlDataAdapter();
                            command = new SqlCommand($"DELETE FROM Times WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            adapter = new SqlDataAdapter();
                            command = new SqlCommand($"DELETE FROM ReqMaterials WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            adapter = new SqlDataAdapter();
                            command = new SqlCommand($"DELETE FROM DayTypes WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            adapter = new SqlDataAdapter();
                            command = new SqlCommand($"DELETE FROM Locations WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            adapter = new SqlDataAdapter();
                            command = new SqlCommand($"DELETE FROM FinalGrade WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            adapter = new SqlDataAdapter();
                            command = new SqlCommand($"DELETE FROM StudentEnrolledCourses WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            adapter = new SqlDataAdapter();
                            command = new SqlCommand($"DELETE FROM SemesterCourses WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            adapter = new SqlDataAdapter();
                            command = new SqlCommand($"DELETE FROM Courses WHERE CourseID = {CourseID};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();
                        }
                    }
                    SqlDataAdapter adapter2 = new SqlDataAdapter();
                    SqlCommand command2 = new SqlCommand($"DELETE FROM Departments WHERE Department = '{RemoveDeptName}';", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }

        private async void EditRemoveAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (DepartmentRowCounter != 0)
            {//get rid of the current row
                --DepartmentRowCounter;
                DepartmentsTable2A.Children.RemoveAt(DepartmentRowCounter);
                DepartmentsTable2A.RowDefinitions.RemoveAt(DepartmentRowCounter);

                DepartmentsTable2Achecks.Children.RemoveAt(DepartmentRowCounter);
                DepartmentsTable2Achecks.RowDefinitions.RemoveAt(DepartmentRowCounter);
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
            DepartmentsTable2A.RowDefinitions.Add(newrow);
            RowDefinition newcheckrow = new RowDefinition();
            newcheckrow.Height = new GridLength(50);
            DepartmentsTable2Achecks.RowDefinitions.Add(newcheckrow);

            Border bd = new Border();
            bd.BorderThickness = new Thickness(2);
            bd.BorderBrush = new SolidColorBrush(Colors.Black);
            bd.SetValue(Grid.RowProperty, DepartmentRowCounter);
            bd.SetValue(Grid.ColumnProperty, 0);

            TextBox txtbox = new TextBox();
            txtbox.FontSize = 36;
            txtbox.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
            txtbox.PlaceholderText = "Enter New Department Title";
            txtbox.Foreground = new SolidColorBrush(Colors.Black);
            txtbox.TextAlignment = TextAlignment.Center;
            bd.Child = txtbox;

            CheckBox checkbx = new CheckBox();
            checkbx.HorizontalAlignment = HorizontalAlignment.Center;
            checkbx.VerticalAlignment = VerticalAlignment.Center;
            checkbx.SetValue(Grid.RowProperty, DepartmentRowCounter);
            checkbx.SetValue(Grid.ColumnProperty, 0);

            DepartmentsTable2Achecks.Children.Add(checkbx);
            DepartmentsTable2A.Children.Add(bd);

            DepartmentRowCounter++;
        }

        private void EditCancel_Click(object sender, RoutedEventArgs e)
        {
            EditAddAssignment.Visibility = Visibility.Collapsed;
            EditRemoveAssignment.Visibility = Visibility.Collapsed;
            EditSubmitAssignments.Visibility = Visibility.Collapsed;
            EditRemoveByCheck.Visibility = Visibility.Collapsed;
            EditCancel.Visibility = Visibility.Collapsed;
            Purgedepartmentlist(2);
            DepartmentsTable2A.Visibility = Visibility.Collapsed;
            DepartmentsTable2Achecks.Visibility = Visibility.Collapsed;
            DepartmentsTable1A.Visibility = Visibility.Visible;
            setupdepartments(1);
        }
    }
}
