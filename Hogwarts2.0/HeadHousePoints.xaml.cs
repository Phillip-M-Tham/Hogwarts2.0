using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class HeadHousePoints : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private string CurrentSemester="";
        private int CurrentSemesterID=0;
        private int CurrGryffindorPoints;
        private int CurrRavenclawPoints;
        private int CurrHufflepuffPoints;
        private int CurrSlytherinPoints;
        public HeadHousePoints()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            getCurrentSemesterInfo();
         //   setupSemesters(1);
        }

        private void getCurrentSemesterInfo()
        {//sets up form1 and form2
            List<string> Semesters = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT * FROM HousePoints;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CurrentSemesterID = (int)reader.GetValue(0);
                                CurrGryffindorPoints = (int)reader.GetValue(1);
                                CurrSlytherinPoints= (int)reader.GetValue(2);
                                CurrHufflepuffPoints = (int)reader.GetValue(3);
                                CurrRavenclawPoints = (int)reader.GetValue(4);
                            }
                        }
                    }
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
                    if (CurrentSemesterID != 0)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"Select Semester FROM Semesters WHERE SemesterID = {CurrentSemesterID}";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    CurrentSemester = reader.GetValue(0).ToString();
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (Semesters.Count() == 0)
            {
                Form3SemesterInput.Items.Add("There Are No Semesters");
            }
            else
            {
                foreach (var semester in Semesters)
                {
                    Form3SemesterInput.Items.Add(semester);
                }
            }
            
        }
        private void SetSemester_Click(object sender, RoutedEventArgs e)
        {
            Form1.Visibility = Visibility.Collapsed;
            Form3.Visibility = Visibility.Visible;
            setupform3();
        }

        private void setupform3()
        {
            if (CurrentSemesterID != 0)
            {
                Form3SemesterInput.SelectedItem = CurrentSemester;
            }
            else
            {
                Form3SemesterInput.SelectedItem = null;
            }
        }

        private void UpdatePoints_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Visible;
            Form1.Visibility = Visibility.Collapsed;
            setupform2();
        }

        private void setupform2()
        {
            if(CurrentSemesterID != 0)
            {//there is a current selected semesterID
                Form2Title.Text = CurrentSemester;
                Gpoints.Text = CurrGryffindorPoints.ToString();
                Rpoints.Text = CurrRavenclawPoints.ToString();
                Hpoints.Text = CurrHufflepuffPoints.ToString();
                Spoints.Text = CurrSlytherinPoints.ToString();
            }
            else
            {
                Form2Title.Text = "No Current Semester Exists";
                Gpoints.Text = "N/A";
                Rpoints.Text = "N/A";
                Hpoints.Text = "N/A";
                Spoints.Text = "N/A";
            }
        }

        private void Form2Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Collapsed;
            Form1.Visibility = Visibility.Visible;
            ResetForm2();
        }

        private void ResetForm2()
        {
            HouseInput.SelectedItem = null;
            PointInput.Text = "";
        }

        private async void ApplyPoints_Click(object sender, RoutedEventArgs e)
        {
            string SelectedHouse = "";
            string warning = "";
            int validupdate = 0;
            int testresults;
            string notvalidmessage = "";
            string updateresults = "";
            bool isNumber;
            int points = 0;
            if(CurrentSemester != "")
            {//this is a valid semester to apply points
                if(HouseInput.SelectedItem == null)
                {
                    validupdate++;
                    notvalidmessage += "Please Select a House"; 
                }
                if(PointInput.Text != "")
                {//they typed something in the textbox
                    isNumber = Int32.TryParse(PointInput.Text, out points);
                    if(isNumber == false)
                    {
                        notvalidmessage += "\nPlease Input a valid number.";
                        validupdate++;
                    }
                    if(points == 0)
                    {
                        notvalidmessage += "\nPlease pick a value that is not 0.";
                        validupdate++;
                    }
                }
                else
                {
                    notvalidmessage += "\nPlease Provide how many points you want to apply to a house.";
                    validupdate++;
                }
                if (validupdate == 0)
                {//we can update
                    if (HouseInput.SelectedValue.ToString() == "Gryffindor")
                    {
                        testresults = CurrGryffindorPoints;
                        testresults += points;
                        SelectedHouse = "GryffindorPoints";
                    }
                    else if (HouseInput.SelectedValue.ToString() == "Ravenclaw")
                    {
                        testresults = CurrRavenclawPoints;
                        testresults += points;
                        SelectedHouse = "RavenclawPoints";
                    }
                    else if (HouseInput.SelectedValue.ToString() == "Hufflepuff")
                    {
                        testresults = CurrHufflepuffPoints;
                        testresults += points;
                        SelectedHouse = "HufflepuffPoints";
                    }
                    else
                    {
                        testresults = CurrSlytherinPoints;
                        testresults += points;
                        SelectedHouse = "SlytherinPoints";
                    }
                    if (testresults < 0)
                    {
                        warning = "You cannot have negative points.The house will be set to 0.";
                        testresults = 0;
                    }
                    if (testresults > 1000000)
                    {
                        warning = "You cannot exceed 1000000 points. The house will be set to 1000000";
                        testresults = 1000000;
                    }
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"UPDATE HousePoints SET {SelectedHouse} = {testresults};", sqlConn);
                                adapter.UpdateCommand = command;
                                adapter.UpdateCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    if (points > 0)
                    {//adding points
                        updateresults = $"{Math.Abs(points)} Points To {HouseInput.SelectedValue}";
                    }
                    else
                    {//subtracting points
                        updateresults = $"{Math.Abs(points)} Points From {HouseInput.SelectedValue}";
                    }
                    if (warning != "")
                    {
                        updateresults += "\n" + warning;
                    }
                    var NotValidSemester = new MessageDialog(updateresults);
                    await NotValidSemester.ShowAsync();
                    getCurrentSemesterInfo();
                    ResetForm2();
                    setupform2();
                }
                else
                {
                    var NotValidSemester = new MessageDialog(notvalidmessage);
                    await NotValidSemester.ShowAsync();
                }
            }
            else
            {
                var NotValidSemester = new MessageDialog("There is no current semester assigned.");
                await NotValidSemester.ShowAsync();
            }
        }

        private void Form3Cancel_Click(object sender, RoutedEventArgs e)
        {
            Form3.Visibility = Visibility.Collapsed;
            Form1.Visibility = Visibility.Visible;
            ResetForm3();
        }

        private void ResetForm3()
        {
            Form3SemesterInput.SelectedItem = null;
        }

        private async void Form3Submit_Click(object sender, RoutedEventArgs e)
        {
            int SelectedSemesterID = 0;
           
            if (Form3SemesterInput.SelectedItem != null)
            {
                if (Form3SemesterInput.SelectedValue.ToString() != "There Are No Semesters")
                {
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = $"SELECT SemesterID FROM Semesters Where Semester = '{Form3SemesterInput.SelectedValue}';";
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        SelectedSemesterID = (int)reader.GetValue(0);
                                    }
                                }
                            }
                            if (CurrentSemester == "")
                            {//insert
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"INSERT INTO HousePoints VALUES ({SelectedSemesterID},0,0,0,0);", sqlConn);
                                adapter.InsertCommand = command;
                                adapter.InsertCommand.ExecuteNonQuery();
                            }
                            else
                            {//update
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                SqlCommand command = new SqlCommand($"UPDATE HousePoints SET CurrentSemesterID = {SelectedSemesterID}, GryffindorPoints = 0, SlytherinPoints = 0, HufflepuffPoints = 0, RavenclawPoints = 0;", sqlConn);
                                adapter.UpdateCommand = command;
                                adapter.UpdateCommand.ExecuteNonQuery();
                            }
                            sqlConn.Close();
                        }
                    }
                    var ValidSemester = new MessageDialog("Successfully Set Current Semester.");
                    await ValidSemester.ShowAsync();
                    Frame.Navigate(typeof(HeadHousePoints),_userHuid);
                }
                else
                {//they tried to insert there are no semesters
                    var NotValidSemester = new MessageDialog("No semesters available, please create a semester.");
                    await NotValidSemester.ShowAsync();
                }
            }
            else
            {//they didnt choose
                var NotValidSemester = new MessageDialog("Please select a semester");
                await NotValidSemester.ShowAsync();
            }
            
        }
    }
}
