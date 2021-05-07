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
        public HeadHousePoints()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
         //   setupSemesters(1);
        }

        private void SetSemester_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdatePoints_Click(object sender, RoutedEventArgs e)
        {
            Form2.Visibility = Visibility.Visible;
            Form1.Visibility = Visibility.Collapsed;
            setupform2();
        }

        private void setupform2()
        {
            int semesterID=0;
            int currGryffindorPoints=0;
            int currSlytherinPoints=0;
            int currHufflepuffPoints=0;
            int currRavenclawPoints=0;
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
                                semesterID = (int)reader.GetValue(0);
                                currGryffindorPoints = (int)reader.GetValue(1);
                                currSlytherinPoints = (int)reader.GetValue(2);
                                currHufflepuffPoints = (int)reader.GetValue(3);
                                currRavenclawPoints = (int)reader.GetValue(4);
                            }
                        }
                    }
                    if (semesterID != 0)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"Select Semester FROM Semesters WHERE SemesterID = {semesterID}";
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
            CurrentSemesterID = semesterID;
            if(semesterID != 0)
            {//there is a current selected semesterID
                Form2Title.Text = CurrentSemester;
                Gpoints.Text = currGryffindorPoints.ToString();
                Rpoints.Text = currRavenclawPoints.ToString();
                Hpoints.Text = currHufflepuffPoints.ToString();
                Spoints.Text = currSlytherinPoints.ToString();
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
            if(CurrentSemester != "")
            {//this is a valid semester to apply points

            }
            else
            {
                var NotValidSemester = new MessageDialog("There is no current semester assigned.");
                await NotValidSemester.ShowAsync();
            }
        }
    }
}
