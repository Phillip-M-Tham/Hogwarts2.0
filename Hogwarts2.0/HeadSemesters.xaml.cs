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
    public sealed partial class HeadSemesters : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private int SemesterRowCounter = 0;
        public HeadSemesters()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            setupSemesters(1);
        }

        private void Edit1A(object sender, RoutedEventArgs e)
        {
            if (SemestersTable2A.Visibility == Visibility.Collapsed)
            {
                SemestersTable2Achecks.Visibility = Visibility.Visible;
                SemestersTable2A.Visibility = Visibility.Visible;
                EditAddSemester.Visibility = Visibility.Visible;
                EditRemoveSemester.Visibility = Visibility.Visible;
                EditSubmitSemesters.Visibility = Visibility.Visible;
                EditRemoveByCheck.Visibility = Visibility.Visible;
                EditCancel.Visibility = Visibility.Visible;
                PurgeSemesterlist(1);
                SemestersTable1A.Visibility = Visibility.Collapsed;
                setupSemesters(2);
            }
        }

        private void setupSemesters(int mode)
        {
            List<int> SemIDs = new List<int>();
            SemesterRowCounter = 0;
            List<string> Semesters = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT Semester FROM Semesters;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Semesters.Add(reader.GetValue(0).ToString());
                            }
                        }
                    }
                    if (Semesters.Count > 0)
                    {
                        //Semesters.Sort();
                        foreach (var sem in Semesters)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT SemesterID FROM Semesters WHERE Semester = '{sem}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        SemIDs.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (Semesters.Count > 0)
            {
                foreach (var sem in Semesters)
                {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    if (mode == 1)
                    {
                        SemestersTable1A.RowDefinitions.Add(newrow);

                        TextBlock textblock = new TextBlock();
                        textblock.SetValue(Grid.RowProperty, SemesterRowCounter);
                        textblock.SetValue(Grid.ColumnProperty, 0);
                        textblock.HorizontalAlignment = HorizontalAlignment.Center;
                        textblock.VerticalAlignment = VerticalAlignment.Center;
                        textblock.Foreground = new SolidColorBrush(Colors.Black);
                        textblock.Text = sem;
                        textblock.FontSize = 36;
                        textblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");

                        SemestersTable1A.Children.Add(textblock);
                    }
                    else if (mode == 2)
                    {
                        RowDefinition newrow2 = new RowDefinition();
                        newrow2.Height = new GridLength(50);

                        SemestersTable2A.RowDefinitions.Add(newrow);
                        SemestersTable2Achecks.RowDefinitions.Add(newrow2);

                        CheckBox chk = new CheckBox();
                        chk.HorizontalAlignment = HorizontalAlignment.Center;
                        chk.VerticalAlignment = VerticalAlignment.Center;
                        chk.Name = SemIDs[SemesterRowCounter].ToString();
                        chk.SetValue(Grid.RowProperty, SemesterRowCounter);
                        chk.SetValue(Grid.ColumnProperty, 0);
                        chk.IsChecked = false;

                        Border bd = new Border();
                        bd.BorderThickness = new Thickness(2);
                        bd.BorderBrush = new SolidColorBrush(Colors.Black);
                        bd.SetValue(Grid.RowProperty, SemesterRowCounter);
                        bd.SetValue(Grid.ColumnProperty, 0);

                        TextBox txtbox = new TextBox();
                        txtbox.HorizontalAlignment = HorizontalAlignment.Center;
                        txtbox.VerticalAlignment = VerticalAlignment.Center;
                        txtbox.Foreground = new SolidColorBrush(Colors.Black);
                        txtbox.Width = 500;
                        txtbox.Text = sem;
                        txtbox.TextAlignment = TextAlignment.Center;
                        txtbox.FontSize = 36;
                        txtbox.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                        bd.Child = txtbox;

                        SemestersTable2Achecks.Children.Add(chk);
                        SemestersTable2A.Children.Add(bd);
                    }
                    SemesterRowCounter++;
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
                textblock.Text = "No Semesters Available";
                textblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");

                if (mode == 1)
                {
                    SemestersTable1A.RowDefinitions.Add(newrow);
                    SemestersTable1A.Children.Add(textblock);
                }
            }
        }

        private void PurgeSemesterlist(int mode)
        {
            if (mode == 1)
            {
                SemestersTable1A.RowDefinitions.Clear();
                SemestersTable1A.Children.Clear();
            }
            else
            {
                SemestersTable2A.RowDefinitions.Clear();
                SemestersTable2A.Children.Clear();
                SemestersTable2Achecks.RowDefinitions.Clear();
                SemestersTable2Achecks.Children.Clear();
            }
        }

        private void EditAddSemester_Click(object sender, RoutedEventArgs e)
        {
            RowDefinition newrow = new RowDefinition();
            newrow.Height = new GridLength(50);
            SemestersTable2A.RowDefinitions.Add(newrow);
            RowDefinition newcheckrow = new RowDefinition();
            newcheckrow.Height = new GridLength(50);
            SemestersTable2Achecks.RowDefinitions.Add(newcheckrow);

            Border bd = new Border();
            bd.BorderThickness = new Thickness(2);
            bd.BorderBrush = new SolidColorBrush(Colors.Black);
            bd.SetValue(Grid.RowProperty, SemesterRowCounter);
            bd.SetValue(Grid.ColumnProperty, 0);

            TextBox txtbox = new TextBox();
            txtbox.FontSize = 36;
            txtbox.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
            txtbox.PlaceholderText = "Enter New Semester Title";
            txtbox.Foreground = new SolidColorBrush(Colors.Black);
            txtbox.TextAlignment = TextAlignment.Center;
            bd.Child = txtbox;

            CheckBox checkbx = new CheckBox();
            checkbx.HorizontalAlignment = HorizontalAlignment.Center;
            checkbx.VerticalAlignment = VerticalAlignment.Center;
            checkbx.SetValue(Grid.RowProperty, SemesterRowCounter);
            checkbx.SetValue(Grid.ColumnProperty, 0);

            SemestersTable2Achecks.Children.Add(checkbx);
            SemestersTable2A.Children.Add(bd);

            SemesterRowCounter++;
        }

        private async void EditRemoveSemester_Click(object sender, RoutedEventArgs e)
        {
            if (SemesterRowCounter != 0)
            {//get rid of the current row
                --SemesterRowCounter;
                SemestersTable2A.Children.RemoveAt(SemesterRowCounter);
                SemestersTable2A.RowDefinitions.RemoveAt(SemesterRowCounter);

                SemestersTable2Achecks.Children.RemoveAt(SemesterRowCounter);
                SemestersTable2Achecks.RowDefinitions.RemoveAt(SemesterRowCounter);
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("No Semesters exist.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private async void EditSubmitSemesters_Click(object sender, RoutedEventArgs e)
        {
            bool validinput = true;
            int selectedSemID = -1;
            bool needsDelete=false;
            List<string> InputSemTitles = new List<string>();
            List<int> originalSemIDs = new List<int>();
            List<int> newSemIDs = new List<int>();
            foreach (var bd in SemestersTable2A.Children)
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
                        InputSemTitles.Add(tbx.Text);
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
                            cmd.CommandText = $"SELECT SemesterID FROM Semesters;";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    originalSemIDs.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                        foreach (var title in InputSemTitles)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT SemesterID FROM Semesters WHERE Semester = '{title}';";
                                {
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            selectedSemID = (int)reader.GetValue(0);
                                        }
                                    }
                                }
                            }
                            if (selectedSemID != -1)
                            {//update
                                needsDelete = true;
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand cmd2 = new SqlCommand($"UPDATE Semesters SET Semester = '{title}' WHERE SemesterID = {selectedSemID};", sqlConn);
                                adapter.UpdateCommand = cmd2;
                                adapter.UpdateCommand.ExecuteNonQuery();
                                newSemIDs.Add(selectedSemID);
                            }
                            else
                            {//insert
                                needsDelete = true;
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"INSERT INTO Semesters VALUES ('{title}');", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {//get the assignment id of the new inserted assignment
                                    cmd.CommandText = $"SELECT DepartmentID FROM Departments WHERE Department = '{title}';";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            newSemIDs.Add((int)reader.GetValue(0));
                                        }
                                    }
                                }
                            }
                            selectedSemID = -1;
                        }
                        foreach (var id in originalSemIDs)
                        {
                            if (newSemIDs.Contains(id) == false)
                            {
                                needsDelete = true;
                                DeleteSemesterbyID(id);
                            }
                        }
                        sqlConn.Close();
                    }
                }
                if (needsDelete == true)
                {
                    var ValidRemoveassignment = new MessageDialog("Successfully Updated Semesters");
                    await ValidRemoveassignment.ShowAsync();
                    PurgeSemesterlist(2);
                    setupSemesters(2);
                }
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("Please do not leave any Semester titles empty.");
                await Removeassignmenterror.ShowAsync();
            }
        }

        private void DeleteSemesterbyID(int id)
        {
            List<int> RemoveAssignmentID = new List<int>();
            List<int> RemoveCourseIDs = new List<int>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT CourseID FROM SemesterCourses WHERE SemesterID = {id};";
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
                                cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE CourseID = {CID} AND SemesterID = {id};";
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
                    
                    SqlDataAdapter adapter2 = new SqlDataAdapter();
                    SqlCommand command2 = new SqlCommand($"DELETE FROM Locations WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM HousePoints WHERE CurrentSemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM Assignments WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM FinalGrade WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM ReqMaterials WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM StudentEnrolledCourses WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM SemesterCourses WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM DayTypes WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM Times WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();

                    command2 = new SqlCommand($"DELETE FROM Semesters WHERE SemesterID = {id};", sqlConn);
                    adapter2.DeleteCommand = command2;
                    adapter2.DeleteCommand.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
        private void EditCancel_Click(object sender, RoutedEventArgs e)
        {
            EditAddSemester.Visibility = Visibility.Collapsed;
            EditRemoveSemester.Visibility = Visibility.Collapsed;
            EditSubmitSemesters.Visibility = Visibility.Collapsed;
            EditRemoveByCheck.Visibility = Visibility.Collapsed;
            EditCancel.Visibility = Visibility.Collapsed;
            PurgeSemesterlist(2);
            SemestersTable2A.Visibility = Visibility.Collapsed;
            SemestersTable2Achecks.Visibility = Visibility.Collapsed;
            SemestersTable1A.Visibility = Visibility.Visible;
            setupSemesters(1);
        }

        private async void EditRemoveByCheck_Click(object sender, RoutedEventArgs e)
        {
            int targetid;
            List<CheckBox> targetlist = new List<CheckBox>();
            foreach (CheckBox box in SemestersTable2Achecks.Children)
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
                    DeleteSemesterbyID(targetid);
                }
                var ValidRemoveassignment = new MessageDialog("Successfully Updated Semesters");
                await ValidRemoveassignment.ShowAsync();
                PurgeSemesterlist(2);
                setupSemesters(2);
            }
            else
            {
                var Removeassignmenterror = new MessageDialog("No Departments were selected.");
                await Removeassignmenterror.ShowAsync();
            }
        }
    }
}
