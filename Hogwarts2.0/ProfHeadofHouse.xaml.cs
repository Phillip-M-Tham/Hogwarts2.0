using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfHeadofHouse : Page
    {
        private string UserHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private int StudentListRow=0;
        private string SelectedHouse = "";
        private bool FilterOn;
        private int SelectedStudentID;
        private int SelectedStudentYear;
        public ProfHeadofHouse()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UserHuid = e.Parameter.ToString();
            SetUpForms();
            ResetFilter();
            Setupchart();
        }
        private void Setupchart()
        {
            (lineChart.Series[0] as LineSeries).DependentRangeAxis = new LinearAxis
            {
                FontFamily = new FontFamily("/Assets/ReginaScript.ttf#Regina Script"),
                FontSize = 25,
                Foreground = new SolidColorBrush(Colors.Black),
                Minimum = 0.0,
                Maximum = 4.0,
                Interval = .5,
                Orientation = AxisOrientation.Y,
                ShowGridLines = true
            };
        }

        private void ResetFilter()
        {
            YearlevelInput.SelectedItem = "All Years";
            FilterbyReg.IsChecked = true;
            FilterbyAlph.IsChecked = false;
        }

        private void SetUpForms()
        {
            string HeadofHouse = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {//checks if the logged in Professor is a head of a house
                        cmd.CommandText = $"SELECT HouseName FROM HouseHead WHERE ProfHUID = {UserHuid};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HeadofHouse = (reader.GetValue(0).ToString());
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            Form1Title.Text = $"{HeadofHouse} Management";
            SelectedHouse = HeadofHouse;
        }

        private void EnableFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterForm.Visibility = Visibility.Visible;
            FilterOn = true;
        }

        private void FilterCancel_Click(object sender, RoutedEventArgs e)
        {
            FilterForm.Visibility = Visibility.Collapsed;
            FilterOn = false;
        }

        private void YearlevelInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int year;
            Int32.TryParse(YearlevelInput.SelectedValue.ToString(), out year);
            purgeStudentList();
            if (FilterbyReg.IsChecked == true)
            {
                setupStudentList("default", year);
            }
            else
            {
                setupStudentList("alph", year);
            }
        }

        private void SetFilter(object sender, RoutedEventArgs e)
        {
            int year;
            Int32.TryParse(YearlevelInput.SelectedValue.ToString(), out year);

            if ((sender as CheckBox).Name == "FilterbyAlph")
            {
                FilterbyReg.IsChecked = false;
                purgeStudentList();
                setupStudentList("alph", year);
            }
            else if ((sender as CheckBox).Name == "FilterbyReg")
            {
                FilterbyAlph.IsChecked = false;
                purgeStudentList();
                setupStudentList("default", year);
            }
        }

        private void setupStudentList(string mode, int year)
        {
            purgeStudentList();
            StudentListRow = 0;
            List<int> StudentIDs = new List<int>();
            List<int> AlphstudentIDs = new List<int>();
            List<int> FilterStudentIDs = new List<int>();
            List<string> StudentNames = new List<string>();
            List<int> StudentYears = new List<int>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT HUID FROM Houses WHERE HouseName = '{SelectedHouse}';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StudentIDs.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if (StudentIDs.Count > 0)
                    {//should only work with default and all
                        if (year == 0)
                        {
                            foreach (var ID in StudentIDs)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT FirstName,LastName FROM Users WHERE HUID = {ID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            StudentNames.Add(reader.GetValue(0).ToString() + " " + reader.GetValue(1).ToString());
                                        }
                                    }
                                }
                                if (mode == "default")
                                {//populate studentyears with default all names and ids
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {ID};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                StudentYears.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                            }
                            if (mode == "alph")
                            {//this is alphabetical
                                StudentNames.Sort();
                                foreach (var name in StudentNames)
                                {
                                    string[] firstlast = name.Split(" ");
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT HUID FROM Users WHERE FirstName = '{firstlast[0]}' AND LastName = '{firstlast[1]}';";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                AlphstudentIDs.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                                foreach (var id in AlphstudentIDs.ToList())
                                {//filters the obtained ids for non student ids
                                    if (!StudentIDs.Contains(id))
                                    {
                                        AlphstudentIDs.Remove(id);
                                    }
                                }
                                foreach (var id in AlphstudentIDs)
                                {
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {//populates studentyears with alphabetical obtained ids
                                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                StudentYears.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //StudentIDs.Clear();
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT HUID FROM Students WHERE StudentYear = {year};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        FilterStudentIDs.Add((int)reader.GetValue(0));
                                    }
                                }
                            }
                            //filter the results
                            foreach(var id in FilterStudentIDs.ToList())
                            {
                                if (!StudentIDs.Contains(id))
                                {
                                    FilterStudentIDs.Remove(id);
                                }
                            }
                            foreach (var ID in FilterStudentIDs)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT FirstName,LastName FROM Users WHERE HUID = {ID};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            StudentNames.Add(reader.GetValue(0).ToString() + " " + reader.GetValue(1).ToString());
                                        }
                                    }
                                }
                                if (mode == "default")
                                {//populate studentyears with default all names and ids
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {ID};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                StudentYears.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                            }
                            if (mode == "alph")
                            {//this is alphabetical
                                StudentNames.Sort();
                                foreach (var name in StudentNames)
                                {
                                    string[] firstlast = name.Split(" ");
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT HUID FROM Users WHERE FirstName = '{firstlast[0]}' AND LastName = '{firstlast[1]}';";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                AlphstudentIDs.Add((int)reader.GetValue(0));
                                            }
                                        }
                                    }
                                }
                                foreach (var id in AlphstudentIDs.ToList())
                                {//filters the obtained ids for non faculty ids
                                    if (!StudentIDs.Contains(id))
                                    {
                                        AlphstudentIDs.Remove(id);
                                    }
                                }
                                foreach (var id in AlphstudentIDs)
                                {
                                    using (SqlCommand cmd = sqlConn.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {id};";
                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                StudentYears.Add((int)reader.GetValue(0));
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
            if (StudentNames.Count > 0)
            {
                foreach (var name in StudentNames)
                {
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(50);
                    StudentList.RowDefinitions.Add(newrow);

                    Border bdName = new Border();
                    bdName.BorderThickness = new Thickness(2);
                    bdName.BorderBrush = new SolidColorBrush(Colors.Black);
                    bdName.SetValue(Grid.RowProperty, StudentListRow);
                    bdName.SetValue(Grid.ColumnProperty, 0);

                    Button StuName = new Button();
                    StuName.Foreground = new SolidColorBrush(Colors.Black);
                    if (mode == "default")
                    {
                        if (year == 0)
                        {
                            StuName.Name = StudentIDs[StudentListRow].ToString();//this is for default,all
                        }
                        else
                        {
                            StuName.Name = FilterStudentIDs[StudentListRow].ToString();
                        }
                    }
                    else if (mode == "alph")
                    {
                        StuName.Name = AlphstudentIDs[StudentListRow].ToString();//this is for alpha,all
                    }
                    StuName.FontSize = 36;
                    StuName.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    StuName.VerticalAlignment = VerticalAlignment.Center;
                    StuName.HorizontalAlignment = HorizontalAlignment.Center;
                    StuName.Content = name;
                    StuName.Click += ViewSelectedStudent;
                    StuName.SetValue(Grid.RowProperty, StudentListRow);
                    StuName.SetValue(Grid.ColumnProperty, 0);
                    bdName.Child = StuName;

                    Border bdRole = new Border();
                    bdRole.BorderThickness = new Thickness(2);
                    bdRole.BorderBrush = new SolidColorBrush(Colors.Black);
                    bdRole.SetValue(Grid.RowProperty, StudentListRow);
                    bdRole.SetValue(Grid.ColumnProperty, 1);

                    TextBlock StuYear = new TextBlock();
                    StuYear.Foreground = new SolidColorBrush(Colors.Black);
                    StuYear.FontSize = 36;
                    StuYear.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                    StuYear.VerticalAlignment = VerticalAlignment.Center;
                    StuYear.HorizontalAlignment = HorizontalAlignment.Center;
                    StuYear.SetValue(Grid.RowProperty, StudentListRow);
                    StuYear.SetValue(Grid.ColumnProperty, 1);
                    StuYear.Text = StudentYears[StudentListRow].ToString();
                    bdRole.Child = StuYear;

                    StudentList.Children.Add(bdRole);
                    StudentList.Children.Add(bdName);

                    StudentListRow++;
                }
            }
            else
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                StudentList.RowDefinitions.Add(newrow);

                TextBlock txtblk = new TextBlock();
                txtblk.Foreground = new SolidColorBrush(Colors.Black);
                txtblk.FontSize = 36;
                txtblk.HorizontalAlignment = HorizontalAlignment.Center;
                txtblk.VerticalAlignment = VerticalAlignment.Center;
                txtblk.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");
                txtblk.SetValue(Grid.RowProperty, 0);
                txtblk.SetValue(Grid.ColumnProperty, 0);
                txtblk.Text = "No Students exist";
            }
        }

        private void ViewSelectedStudent(object sender, RoutedEventArgs e)
        {
            Button selectedstudent = sender as Button;
            FilterForm.Visibility = Visibility.Collapsed;
            Form1.Visibility = Visibility.Collapsed;
            Form2.Visibility = Visibility.Visible;
            Form2Title.Text = selectedstudent.Content.ToString();
            SelectedStudentID = Int32.Parse(selectedstudent.Name);
            Form2ScrollViewer.ChangeView(null, 0, null, true);
            setupForm2();
            getChartData();
        }

        private async void setupForm2()
        {
            string aboutme = "";
            byte[] result = default(byte[]);
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT AboutInfo FROM Users WHERE HUID = {SelectedStudentID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                aboutme = reader.GetValue(0).ToString();
                            }
                        }
                    }
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT StudentYear FROM Students WHERE HUID = {SelectedStudentID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SelectedStudentYear = (int)reader.GetValue(0);
                            }
                        }
                    }
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT ProfilePic FROM ProfilePics WHERE HUID ={SelectedStudentID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result = (byte[])reader.GetValue(0);
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            CurrentYearlevel.SelectedValue = SelectedStudentYear;
            if (aboutme != "")
            {
                StudentBio.Text = aboutme;
            }
            else
            {
                StudentBio.Text = "No Biography Available";
            }
            if (result != null)
            {
                BitmapImage biSource = new BitmapImage();
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(result.AsBuffer());
                    stream.Seek(0);
                    await biSource.SetSourceAsync(stream);
                }
                SelectedStudentProfilePic.Source = biSource;
            }
        }

        public class Records
        {
            public string Semester { get; set; }
            public double GPA { get; set; }
            public Records(string Sem, double gpa)
            {
                Semester = Sem;
                GPA = gpa;
            }
        }
        private void getChartData()
        {
            List<int> semesterIDs = new List<int>();
            List<decimal> Grades = new List<decimal>();
            List<Records> validGPA = new List<Records>();
            string semestername = "";
            //get the distinct semesterids the student is enrolled in
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                { //puts info from database for semesters in a list
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT DISTINCT SemesterID FROM StudentEnrolledCourses WHERE StudentID = {SelectedStudentID};";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                semesterIDs.Add((int)reader.GetValue(0));
                            }
                        }
                    }
                    if (semesterIDs.Count > 0)
                    {
                        foreach (var id in semesterIDs)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT CurrentGrade FROM FinalGrade WHERE StudentHUID = {SelectedStudentID} AND SemesterID = {id};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Grades.Add((decimal)reader.GetValue(0));
                                    }
                                }
                            }
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT Semester FROM Semesters WHERE SemesterID = {id};";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        semestername = reader.GetValue(0).ToString();
                                    }
                                }
                            }
                            //we obtain all the grades, for the selected semester, we need to create a GPA and create a Record with it
                            Records myrecord = createRecord(Grades, semestername);
                            if (myrecord.Semester != "bad")
                            {//valid record to be added to list of valid records
                                validGPA.Add(myrecord);
                            }
                            Grades.Clear();
                        }
                    }
                    else
                    {
                        validGPA.Add(new Records("N/A", 0.0));
                    }
                    sqlConn.Close();
                }
            }
            (lineChart.Series[0] as LineSeries).ItemsSource = validGPA;

        }

        private Records createRecord(List<decimal> grades, string semestername)
        {
            double GPA;
            double totalpoints = 0.0;
            double gradepoint;
            int credit = 0;
            if (grades.Count > 0)
            {
                foreach (var item in grades)
                {
                    gradepoint = getGradePoint(item);
                    totalpoints += gradepoint;
                    credit += 3;
                }
                GPA = totalpoints / credit;
                Records myrecord = new Records(semestername, GPA);
                return myrecord;
            }
            else
            {
                Records records = new Records("bad", 0.0);
                return records;
            }
        }

        private double getGradePoint(decimal item)
        {
            double gradepoint;
            if (item >= (decimal)90.00)
            {//4.0
                gradepoint = 4.0;
            }
            else if (item >= (decimal)80.00 && item < (decimal)90.00)
            {//3.0
                gradepoint = 3.0;
            }
            else if (item >= (decimal)70.00 && item < (decimal)80.00)
            {//2.0
                gradepoint = 2.0;
            }
            else if (item >= (decimal)60.00 && item < (decimal)70.00)
            {//1.0
                gradepoint = 1.0;
            }
            else
            {
                gradepoint = 0.0;
            }
            gradepoint *= 3;
            return gradepoint;
        }


        private void purgeStudentList()
        {
            StudentList.RowDefinitions.Clear();
            StudentList.Children.Clear();
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

        private void Form2Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Collapsed;
            Form1.Visibility = Visibility.Visible;
            if (FilterOn == true)
            {
                FilterForm.Visibility = Visibility.Visible;
            }
            Form2ScrollViewer.ChangeView(null, 0, null, true);
            if (SelectedHouse == "Gryffindor")
            {
                SelectedStudentProfilePic.Source = new BitmapImage(new Uri(base.BaseUri, @"Assets/Gstudefaultpic.png"));
            }
            else if (SelectedHouse == "Hufflepuff")
            {
                SelectedStudentProfilePic.Source = new BitmapImage(new Uri(base.BaseUri, @"Assets/Hstudefaultpic.png"));
            }
            else if(SelectedHouse == "Ravenclaw")
            {
                SelectedStudentProfilePic.Source = new BitmapImage(new Uri(base.BaseUri, @"Assets/Rstudefaultpic.png"));
            }
            else
            {
                SelectedStudentProfilePic.Source = new BitmapImage(new Uri(base.BaseUri, @"Assets/Sstudefaultpic.png"));
            }
        }

        private void Form2UpdateYearLevel_click(object sender, RoutedEventArgs e)
        {
            Form2AUpdateYear.Visibility = Visibility.Visible;
        }

        private void Form2ExpellStudent_click(object sender, RoutedEventArgs e)
        {
            Form2ExpellStudentWarning.Visibility = Visibility.Visible;
        }

        private async void ExpellStudent_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    SqlCommand command = new SqlCommand($"DELETE FROM FinalGrade WHERE StudentHUID = {SelectedStudentID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM Grades WHERE StudentHUID = {SelectedStudentID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM ProfilePics WHERE HUID = {SelectedStudentID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM Houses WHERE HUID = {SelectedStudentID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM Positions WHERE HUID = {SelectedStudentID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM StudentEnrolledCourses WHERE StudentID = {SelectedStudentID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM Students WHERE HUID = {SelectedStudentID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                    command = new SqlCommand($"DELETE FROM Users WHERE HUID = {SelectedStudentID};", sqlConn);
                    adapter.DeleteCommand = command;
                    adapter.DeleteCommand.ExecuteNonQuery();

                }
                sqlConn.Close();
            }
            var NotValidDelete = new MessageDialog("Student Successfully Expelled");
            await NotValidDelete.ShowAsync();
            Form2.Visibility = Visibility.Collapsed;
            Form1.Visibility = Visibility.Visible;
            Form2ExpellStudentWarning.Visibility = Visibility.Collapsed;
            purgeStudentList();
            setupStudentList("default", 0);
        }

        private void CancelForm2B_Click(object sender, RoutedEventArgs e)
        {
            Form2ExpellStudentWarning.Visibility = Visibility.Collapsed;
        }

        private void Form2ACancel_Click(object sender, RoutedEventArgs e)
        {
            Form2AUpdateYear.Visibility = Visibility.Collapsed;
        }

        private async void Form2ASubmit_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    SqlCommand command = new SqlCommand($"UPDATE Students SET StudentYear = {CurrentYearlevel.SelectedValue} WHERE HUID ={SelectedStudentID};", sqlConn);
                    adapter.UpdateCommand = command;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                sqlConn.Close();
            }
            SelectedStudentYear = (int)CurrentYearlevel.SelectedValue;
            var NotValidMessage = new MessageDialog("Student Year Successfully Updated");
            await NotValidMessage.ShowAsync();
            Form2AUpdateYear.Visibility = Visibility.Collapsed;
        }
    }
}
