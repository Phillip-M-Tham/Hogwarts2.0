using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HeadFaculty : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private int FacultyApplicationsRow = 0;
        private int FacultyListRow = 0;
        private bool FilterOn = false;
        int SelectedFacultyHUID;
        string SelecetedFacultyRoleType;
        public HeadFaculty()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
        }

        private void Form1ViewFaculty_Click(object sender, RoutedEventArgs e)
        {
            Form3.Visibility = Visibility.Visible;
            Form1.Visibility = Visibility.Collapsed;
            if (Form3Filter.Visibility == Visibility.Collapsed)
            {
                Form3Filter.Visibility = Visibility.Visible;
            }
            ResetForm3Filter();
        }

        private void ResetForm3Filter()
        {
            RoleInput.SelectedItem = "All Roles";
            FilterbyReg.IsChecked = true;
            FilterbyAlph.IsChecked = false;
            setupForm3("default","All Roles");
        }

        private void setupForm3(string mode,string type)
        {
            PurgeFacultyList();
            FacultyListRow = 0;
            List<int> FacID = new List<int>();
            List<int> AlphFacIDs = new List<int>();
            List<string> FacNames = new List<string>();
            List<string> FacRoles = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT HUID FROM Faculty;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FacID.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if(FacID.Count > 0)
                    {//should only work with default and all
                        if (type == "All Roles")
                        {
                            foreach (var ID in FacID)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT FirstName,LastName FROM Users WHERE HUID = {ID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            FacNames.Add(reader.GetValue(0).ToString() + " " + reader.GetValue(1).ToString());
                                        }
                                    }
                                }
                                if (mode == "default")
                                {//populate facroles with default all names and ids
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT PositionName FROM Positions WHERE HUID = {ID};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                FacRoles.Add(reader.GetValue(0).ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            if(mode == "alph")
                            {//this is alphabetical
                                FacNames.Sort();
                                foreach (var name in FacNames)
                                {
                                    string[] firstlast = name.Split(" ");
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT HUID FROM Users WHERE FirstName = '{firstlast[0]}' AND LastName = '{firstlast[1]}';";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                AlphFacIDs.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                                foreach (var id in AlphFacIDs.ToList())
                                {//filters the obtained ids for non faculty ids
                                    if (!FacID.Contains(id))
                                    {
                                        AlphFacIDs.Remove(id);
                                    }
                                }
                                foreach (var id in AlphFacIDs)
                                {
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT PositionName FROM Positions WHERE HUID = {id};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                FacRoles.Add(reader.GetValue(0).ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            FacID.Clear();
                            string Role = "";
                            if(type == "Professor")
                            {
                                Role = "P";
                            }else if(type == "Counselor")
                            {
                                Role = "C";
                            }else if(type == "Headmaster")
                            {
                                Role = "H";
                            }
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT HUID FROM Faculty WHERE PositionType = '{Role}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        FacID.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                            foreach (var ID in FacID)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT FirstName,LastName FROM Users WHERE HUID = {ID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            FacNames.Add(reader.GetValue(0).ToString() + " " + reader.GetValue(1).ToString());
                                        }
                                    }
                                }
                                if (mode == "default")
                                {//populate facroles with default all names and ids
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT PositionName FROM Positions WHERE HUID = {ID};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                FacRoles.Add(reader.GetValue(0).ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            if (mode == "alph")
                            {//this is alphabetical
                                FacNames.Sort();
                                foreach (var name in FacNames)
                                {
                                    string[] firstlast = name.Split(" ");
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT HUID FROM Users WHERE FirstName = '{firstlast[0]}' AND LastName = '{firstlast[1]}';";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                AlphFacIDs.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                                foreach (var id in AlphFacIDs.ToList())
                                {//filters the obtained ids for non faculty ids
                                    if (!FacID.Contains(id))
                                    {
                                        AlphFacIDs.Remove(id);
                                    }
                                }
                                foreach (var id in AlphFacIDs)
                                {
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT PositionName FROM Positions WHERE HUID = {id};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                FacRoles.Add(reader.GetValue(0).ToString());
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
            if(FacNames.Count > 0)
            {
                foreach (var name in FacNames)
                {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    FacultyList.RowDefinitions.Add(newrow);

                    Border bdName = new Border();
                    bdName.BorderThickness = new Thickness(2);
                    bdName.BorderBrush = new SolidColorBrush(Colors.Black);
                    bdName.SetValue(Grid.RowProperty, FacultyListRow);
                    bdName.SetValue(Grid.ColumnProperty, 0);

                    Button FacName = new Button();
                    FacName.Foreground = new SolidColorBrush(Colors.Black);
                    if (mode == "default")
                    {
                        FacName.Name = FacID[FacultyListRow].ToString();//this is for default,all
                    }else if (mode == "alph")
                    {
                        FacName.Name = AlphFacIDs[FacultyListRow].ToString();//this is for alpha,all
                    }
                    FacName.FontSize = 36;
                    FacName.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    FacName.VerticalAlignment = VerticalAlignment.Center;
                    FacName.HorizontalAlignment = HorizontalAlignment.Center;
                    FacName.Content = name;
                    FacName.Click += ViewSelectedFac;
                    FacName.SetValue(Grid.RowProperty, FacultyListRow);
                    FacName.SetValue(Grid.ColumnProperty, 0);
                    bdName.Child = FacName;

                    Border bdRole = new Border();
                    bdRole.BorderThickness = new Thickness(2);
                    bdRole.BorderBrush = new SolidColorBrush(Colors.Black);
                    bdRole.SetValue(Grid.RowProperty, FacultyListRow);
                    bdRole.SetValue(Grid.ColumnProperty, 1);

                    TextBlock FacRole = new TextBlock();
                    FacRole.Foreground = new SolidColorBrush(Colors.Black);
                    FacRole.FontSize = 36;
                    FacRole.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    FacRole.VerticalAlignment = VerticalAlignment.Center;
                    FacRole.HorizontalAlignment = HorizontalAlignment.Center;
                    FacRole.SetValue(Grid.RowProperty, FacultyListRow);
                    FacRole.SetValue(Grid.ColumnProperty, 1);
                    FacRole.Text = FacRoles[FacultyListRow];
                    bdRole.Child = FacRole;

                    FacultyList.Children.Add(bdRole);
                    FacultyList.Children.Add(bdName);

                    FacultyListRow++;
                }
            }
            else
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                FacultyList.RowDefinitions.Add(newrow);

                TextBlock txtblk = new TextBlock();
                txtblk.Foreground = new SolidColorBrush(Colors.Black);
                txtblk.FontSize = 36;
                txtblk.HorizontalAlignment = HorizontalAlignment.Center;
                txtblk.VerticalAlignment = VerticalAlignment.Center;
                txtblk.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                txtblk.SetValue(Grid.RowProperty, 0);
                txtblk.SetValue(Grid.ColumnProperty, 0);
                txtblk.Text = "No faculty members exist";
            }
        }

        private void ViewSelectedFac(object sender, RoutedEventArgs e)
        {
            Button mybutton = sender as Button;
            Int32.TryParse(mybutton.Name, out SelectedFacultyHUID);
            Form5.Visibility = Visibility.Visible;
            Form3.Visibility = Visibility.Collapsed;
            Form4Filter.Visibility = Visibility.Collapsed;
            Form5Title.Text = mybutton.Content.ToString();
            SetupAboutMe();
        }

        private void SetupAboutMe()
        {
            string userinfo = "";
            //string PositionType="";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT AboutInfo FROM Users WHERE HUID = {SelectedFacultyHUID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userinfo = reader.GetValue(0).ToString();
                            }
                        }
                    }
                    using(SqlCommand cmd= sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT PositionType FROM Faculty WHERE HUID = {SelectedFacultyHUID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SelecetedFacultyRoleType = reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if(userinfo != "")
            {
                AboutMe.Text = userinfo;
            }
            else
            {
                AboutMe.Text = "No Biography Available";
            }
            if(SelecetedFacultyRoleType == "P")
            {
                SetHouseMaster.Visibility = Visibility.Visible;
            }
            else
            {
                SetHouseMaster.Visibility = Visibility.Collapsed;
            }
        }

        private void Form1ViewApplications_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Visible;
            Form1.Visibility = Visibility.Collapsed;
            setupForm2();
        }

        private void setupForm2()
        {
            FacultyApplicationsRow = 0;
            List<int> FacHUID = new List<int>();
            List<string> FacRoles = new List<string>();
            List<string> CandidateNames = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT FacHUID,FacRole FROM FacultyCandidate;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FacHUID.Add((int)reader.GetValue(0));
                                FacRoles.Add(reader.GetValue(1).ToString());
                            }
                        }
                    }
                    if(FacHUID.Count > 0)
                    {
                        foreach (var Id in FacHUID)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {//gets the name from each acquired ID
                                cmd.CommandText = $"SELECT FirstName,LastName FROM Users WHERE HUID = {Id};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        CandidateNames.Add(reader.GetValue(0) + " " + reader.GetValue(1));
                                    }
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if(CandidateNames.Count > 0)
            {
                foreach (var name in CandidateNames) {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    ApplicationList.RowDefinitions.Add(newrow);

                    RowDefinition checkrow = new RowDefinition();
                    checkrow.Height = new GridLength(50);
                    ApplicationListChecks.RowDefinitions.Add(checkrow);

                    CheckBox Chkbx = new CheckBox();
                    Chkbx.VerticalAlignment = VerticalAlignment.Center;
                    Chkbx.HorizontalAlignment = HorizontalAlignment.Center;
                    Chkbx.Name = FacHUID[FacultyApplicationsRow].ToString();
                    Chkbx.SetValue(Grid.RowProperty, FacultyApplicationsRow);
                    Chkbx.SetValue(Grid.ColumnProperty, 0);

                    Border bd2 = new Border();
                    bd2.BorderThickness = new Thickness(2);
                    bd2.BorderBrush = new SolidColorBrush(Colors.Black);
                    bd2.SetValue(Grid.RowProperty, FacultyApplicationsRow);
                    bd2.SetValue(Grid.ColumnProperty, 0);

                    TextBlock txtblock = new TextBlock();
                    txtblock.FontSize = 36;
                    txtblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    txtblock.Foreground = new SolidColorBrush(Colors.Black);
                    txtblock.VerticalAlignment = VerticalAlignment.Center;
                    txtblock.HorizontalAlignment = HorizontalAlignment.Center;
                    txtblock.Text = name + $" Requested Role : {FacRoles[FacultyApplicationsRow]}";
                    bd2.Child = txtblock;

                    ApplicationListChecks.Children.Add(Chkbx);
                    ApplicationList.Children.Add(bd2);

                    FacultyApplicationsRow++;
                }
            }
            else
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                ApplicationList.RowDefinitions.Add(newrow);

                TextBlock txtblock = new TextBlock();
                txtblock.FontSize = 36;
                txtblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                txtblock.Foreground = new SolidColorBrush(Colors.Black);
                txtblock.TextAlignment = TextAlignment.Center;
                txtblock.Text = "No Applications Currently Submitted";
                txtblock.SetValue(Grid.RowProperty, 0);
                txtblock.SetValue(Grid.ColumnProperty, 1);

                ApplicationList.Children.Add(txtblock);
            }
        }

        private void Form2Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Collapsed;
            Form1.Visibility = Visibility.Visible;
            purgeApplicationList();
        }

        private void purgeApplicationList()
        {
            ApplicationList.RowDefinitions.Clear();
            ApplicationList.Children.Clear();
            ApplicationListChecks.RowDefinitions.Clear();
            ApplicationListChecks.Children.Clear();
        }

        private void Form2Decline_Click(object sender, RoutedEventArgs e)
        {
            ProcessApplications("decline");
        }

        private void RemoveApplications(List<int> targetApplicationID, string mode)
        {
            string AcceptType="";
            string AcceptRole="";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    foreach (var target in targetApplicationID)
                    {
                        if (mode == "accept")
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT FacRole,FacType FROM FacultyCandidate WHERE FacHUID = {target};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        AcceptRole = reader.GetValue(0).ToString();
                                        AcceptType = reader.GetValue(1).ToString();
                                    }
                                }
                            }
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"INSERT INTO Positions VALUES ({target},'{AcceptRole}','{AcceptType}');", sqlConn);
                            adapter.InsertCommand = command;
                            adapter.InsertCommand.ExecuteNonQuery();

                            command = new SqlCommand($"INSERT INTO Faculty VALUES({target},'{AcceptType}');", sqlConn);
                            adapter.InsertCommand = command;
                            adapter.InsertCommand.ExecuteNonQuery();

                            command = new SqlCommand($"DELETE FROM FacultyCandidate WHERE FacHUID = {target};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                        }
                        else
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"DELETE FROM FacultyCandidate WHERE FacHUID = {target};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();

                            command = new SqlCommand($"DELETE FROM Users WHERE HUID = {target};", sqlConn);
                            adapter.DeleteCommand = command;
                            adapter.DeleteCommand.ExecuteNonQuery();
                        }
                        
                    }
                    sqlConn.Close();
                }
            }
        }

        private async void ProcessApplications(string mode)
        {
            List<int> targetApplicationID = new List<int>();
            if (ApplicationListChecks.Children.Count > 0)
            {
                foreach (CheckBox checkbx in ApplicationListChecks.Children)
                {
                    if (checkbx.IsChecked == true)
                    {
                        Int32.TryParse(checkbx.Name, out int targetID);
                        targetApplicationID.Add(targetID);
                    }
                }
                RemoveApplications(targetApplicationID,mode);
                if (mode == "decline")
                {
                    var DeclineValid = new MessageDialog("Selected applications successfully denied.");
                    await DeclineValid.ShowAsync();
                }
                else
                {
                    var DeclineValid = new MessageDialog("Selected applications successfully accepted.");
                    await DeclineValid.ShowAsync();
                }
                purgeApplicationList();
                setupForm2();
            }
            else
            {
                var Declineerror = new MessageDialog("No Applications exist.");
                await Declineerror.ShowAsync();
            }

        }

        private void Form2Accept_Click(object sender, RoutedEventArgs e)
        {
            ProcessApplications("accept");
        }

        private void Form3Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form3.Visibility = Visibility.Collapsed;
            Form1.Visibility = Visibility.Visible;
            Form4Filter.Visibility = Visibility.Collapsed;
            FilterOn = false;
            ResetForm3Filter();
            PurgeFacultyList();
        }

        private void PurgeFacultyList()
        {
            FacultyList.RowDefinitions.Clear();
            FacultyList.Children.Clear();
        }

        private void Form3Filter_Click(object sender, RoutedEventArgs e)
        {
            Form4Filter.Visibility = Visibility.Visible;
            Form3Filter.Visibility = Visibility.Collapsed;
            FilterOn = true;
        }

        private void Form4FilterCancel_Click(object sender, RoutedEventArgs e)
        {
            FilterOn = false;
            Form4Filter.Visibility = Visibility.Collapsed;
            Form3Filter.Visibility = Visibility.Visible;
        }

        private void RoleInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PurgeFacultyList();
            if (FilterbyReg.IsChecked == true)
            {
                setupForm3("default", RoleInput.SelectedValue.ToString());
            }
            else
            {
                setupForm3("alph", RoleInput.SelectedValue.ToString());
            }
        }

        private void SetFilter(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Name == "FilterbyAlph")
            {
                FilterbyReg.IsChecked = false;
                PurgeFacultyList();
                setupForm3("alph", RoleInput.SelectedValue.ToString());
            }
            else if ((sender as CheckBox).Name == "FilterbyReg")
            {
                FilterbyAlph.IsChecked = false;
                PurgeFacultyList();
                setupForm3("default", RoleInput.SelectedValue.ToString());
            }
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

        private void Form5Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form5.Visibility = Visibility.Collapsed;
            if(FilterOn == true)
            {
                Form4Filter.Visibility = Visibility.Visible;
            }
            else
            {
                Form3Filter.Visibility = Visibility.Visible;
            }
            SetHouseMaster.Visibility = Visibility.Collapsed;
            Form3.Visibility = Visibility.Visible;
            Form5B.Visibility = Visibility.Collapsed;
            ReliefMember.Visibility = Visibility.Visible;
            Form5Scrollviewer.ChangeView(null, 0, null, true);
            
        }

        private void CancelForm5B_Click(object sender, RoutedEventArgs e)
        {
            Form5B.Visibility = Visibility.Collapsed;
            ReliefMember.Visibility = Visibility.Visible;
            if (SelecetedFacultyRoleType == "P")
            {
                SetHouseMaster.Visibility = Visibility.Visible;
            }
        }

        private async void Terminatefaculty_Click(object sender, RoutedEventArgs e)
        {
            List<int> TargetCourseID = new List<int>();
            List<int> TargetAssignmentID = new List<int>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    if (SelecetedFacultyRoleType == "P")
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {//get all the courses this professor teaches
                            cmd.CommandText = $"SELECT CourseID FROM Courses WHERE ProfHUID = {SelectedFacultyHUID};";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    TargetCourseID.Add((int)reader.GetValue(0));
                                }
                            }
                        }
                        if(TargetCourseID.Count > 0)
                        {
                            foreach(var course in TargetCourseID)
                            {//get all the assingment ids for each course
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT AssignmentID FROM Assignments WHERE CourseID = {course}";
                                    using(SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read()) 
                                        {
                                            TargetAssignmentID.Add((int)reader.GetValue(0));
                                        }
                                    }
                                }
                            }
                            if (TargetAssignmentID.Count > 0)
                            {
                                foreach (var assignment in TargetAssignmentID)
                                {//delete all grades foreach selected assignment id
                                    SqlDataAdapter adapter2 = new SqlDataAdapter();
                                    SqlCommand command2 = new SqlCommand($"DELETE FROM Grades WHERE AssignmentID = {assignment};", sqlConn);
                                    adapter2.DeleteCommand = command2;
                                    adapter2.DeleteCommand.ExecuteNonQuery();
                                }
                            }
                            foreach(var courseid in TargetCourseID)
                            {//delete all finalgrades,time, location,daytype,reqmaterials,studentenrolledcourses,assignments,semestercourses, and courses for each courseid
                                SqlDataAdapter adapter3 = new SqlDataAdapter();
                                SqlCommand command3 = new SqlCommand($"DELETE FROM FinalGrade WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();

                                command3 = new SqlCommand($"DELETE FROM Times WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();

                                command3 = new SqlCommand($"DELETE FROM Locations WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();

                                command3 = new SqlCommand($"DELETE FROM DayTypes WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();

                                command3 = new SqlCommand($"DELETE FROM ReqMaterials WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();

                                command3 = new SqlCommand($"DELETE FROM StudentEnrolledCourses WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();

                                command3 = new SqlCommand($"DELETE FROM Assignments WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();

                                command3 = new SqlCommand($"DELETE FROM SemesterCourses WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();

                                command3 = new SqlCommand($"DELETE FROM Courses WHERE CourseID = {courseid};", sqlConn);
                                adapter3.DeleteCommand = command3;
                                adapter3.DeleteCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    //delete matching, user, faculty, position
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    SqlCommand command = new SqlCommand($"DELETE FROM Positions WHERE HUID = {SelectedFacultyHUID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM Faculty WHERE HUID = {SelectedFacultyHUID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM Users WHERE HUID = {SelectedFacultyHUID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    sqlConn.Close();
                }
            }
            var NotValidMessage = new MessageDialog("Faculty member has been successfully removed.");
            await NotValidMessage.ShowAsync();
            Form5.Visibility = Visibility.Collapsed;
            if (FilterOn == true)
            {
                Form4Filter.Visibility = Visibility.Visible;
            }
            else
            {
                Form3Filter.Visibility = Visibility.Visible;
            }
            SetHouseMaster.Visibility = Visibility.Collapsed;
            Form3.Visibility = Visibility.Visible;
            Form5B.Visibility = Visibility.Collapsed;
            ReliefMember.Visibility = Visibility.Visible;
            Form5Scrollviewer.ChangeView(null, 0, null, true);
            PurgeFacultyList();
            ResetForm3Filter();
        }

        private void ReliefMember_Click(object sender, RoutedEventArgs e)
        {
            Form5B.Visibility = Visibility.Visible;
            ReliefMember.Visibility = Visibility.Collapsed;
            if(SelecetedFacultyRoleType == "P")
            {
                SetHouseMaster.Visibility = Visibility.Collapsed;
            }
        }

        private void SetHouseMaster_Click(object sender, RoutedEventArgs e)
        {
            Form5A.Visibility = Visibility.Visible;
            ReliefMember.Visibility = Visibility.Collapsed;
            SetHouseMaster.Visibility = Visibility.Collapsed;
            SetupForm5A();
        }

        private void SetupForm5A()
        {
            string selectedHouse="";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {//get all the courses this professor teaches
                        cmd.CommandText = $"SELECT HouseName FROM HouseHead WHERE ProfHUID = {SelectedFacultyHUID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                selectedHouse=reader.GetValue(0).ToString();
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if(selectedHouse != "")
            {
                HouseInput.SelectedValue = selectedHouse;
            }
            else
            {
                HouseInput.SelectedItem = null;
            }
        }

        private void Form5ACancel_Click(object sender, RoutedEventArgs e)
        {
            Form5A.Visibility = Visibility.Collapsed;
            ReliefMember.Visibility = Visibility.Visible;
            SetHouseMaster.Visibility = Visibility.Visible;
        }

        private async void Form5AUpdateSubmit_Click(object sender, RoutedEventArgs e)
        {
            int currentProfHUID=0;
            if(HouseInput.SelectedItem != null)
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {//get all the courses this professor teaches
                            cmd.CommandText = $"SELECT ProfHUID FROM HouseHead WHERE HouseName = '{HouseInput.SelectedValue}';";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    currentProfHUID = (int)reader.GetValue(0);
                                }
                            }
                        }
                        if(currentProfHUID != 0)
                        {//update
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"UPDATE HouseHead SET ProfHUID = {SelectedFacultyHUID} WHERE HouseName ='{HouseInput.SelectedValue}';", sqlConn);
                            adapter.UpdateCommand = command;
                            adapter.UpdateCommand.ExecuteNonQuery();
                            var ValidUpdate = new MessageDialog($"Head of {HouseInput.SelectedValue} successfully updated.");
                            await ValidUpdate.ShowAsync();
                        }
                        else
                        {//insert
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"INSERT INTO HouseHead VALUES ('{HouseInput.SelectedValue}',{SelectedFacultyHUID});", sqlConn);
                            adapter.InsertCommand = command;
                            adapter.InsertCommand.ExecuteNonQuery();
                            var ValidInsert = new MessageDialog($"Head of {HouseInput.SelectedValue} successfully assigned.");
                            await ValidInsert.ShowAsync();
                        }
                        sqlConn.Close();
                    }
                }
                Form5A.Visibility = Visibility.Collapsed;
                ReliefMember.Visibility = Visibility.Visible;
                SetHouseMaster.Visibility = Visibility.Visible;
            }
            else
            {
                var NotValidMessage = new MessageDialog("Please select a house to assign this member to become the head of.");
                await NotValidMessage.ShowAsync();
            }
        }
    }
}
