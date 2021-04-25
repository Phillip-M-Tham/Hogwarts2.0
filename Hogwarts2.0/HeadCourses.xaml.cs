using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
                    else if(mode ==2)
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
                textblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");

                if (mode == 1)
                {
                    CoursesTable1A.RowDefinitions.Add(newrow);
                    CoursesTable1A.Children.Add(textblock);
                }
                else if(mode == 2)
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
                    if(ProfHUIDs.Count > 0)
                    {//get names from huids
                        foreach(var id in ProfHUIDs)
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
            if(ProfHUIDs.Count > 0)
            {
                foreach (var name in Professors)
                {
                    Form2AProfessor.Items.Add(name);
                }
            }
            else
            {
                Form2AProfessor.Items.Add("No professors found");
                var ermessage = new MessageDialog("No Professors Have Been Hired");
                await ermessage.ShowAsync();
            }
            if (Departments.Count > 0)
            {
                foreach(var dept in Departments)
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

        private void EditRemoveCourse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditSubmitCourses_Click(object sender, RoutedEventArgs e)
        {

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
            if(mode == 1)
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

        private void EditRemoveByCheck_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Form2ACancel_Click(object sender, RoutedEventArgs e)
        {
            Form2A.Visibility = Visibility.Collapsed;
            Form1A.Visibility = Visibility.Visible;
            ValidDepartmentsInput.Items.Clear();
            Form2AProfessor.Items.Clear();
        }

        private async void SubmitCreateCourse_Click(object sender, RoutedEventArgs e)
        {
            int validCourseInput = 0;
            string errormessage = "";
            if(Form2AProfessor.SelectedValue != null)
            {
                if(Form2AProfessor.SelectedValue.ToString() == "No professors found")
                {
                    errormessage += "\nPlease Add Proffesors to Faculty";
                }
            }
            else
            {
                errormessage += "\nPlease Select A Professor";
                validCourseInput++;
            }
            
            
            
            if(validCourseInput == 0)
            {
                
            }
            else
            {
                var ermessage2 = new MessageDialog(errormessage);
                await ermessage2.ShowAsync();
            }
        }
    }
}
