using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class HStuHome : Page
    {
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        public HStuHome()
        {
            this.InitializeComponent();
            setuphomepage();
        }
        private void setuphomepage()
        {
            int headhouseid = 0;
            int CurrentSemesterID = 0;
            int CurrentPoints = 0;
            string LastName = "";
            string CurrentSemesterName = "";
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                sqlConn.Open();
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT ProfHUID FROM HouseHead WHERE HouseName = 'Hufflepuff';";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                headhouseid = (int)reader.GetValue(0);
                            }
                        }
                    }
                    if (headhouseid != 0)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT LastName FROM Users WHERE HUID = {headhouseid};";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    LastName = reader.GetValue(0).ToString();
                                }
                            }
                        }
                    }
                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT CurrentSemesterID,HufflepuffPoints FROM HousePoints;";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CurrentSemesterID = (int)reader.GetValue(0);
                                CurrentPoints = (int)reader.GetValue(1);
                            }
                        }
                    }
                    if (CurrentSemesterID != 0)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT Semester FROM Semesters WHERE SemesterID = {CurrentSemesterID};";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    CurrentSemesterName = reader.GetValue(0).ToString();
                                }
                            }
                        }
                    }
                    sqlConn.Close();
                }
            }
            if (headhouseid != 0)
            {//there is head of house
                HeadName.Text = $"Professor {LastName}";
            }
            else
            {//no one is assigned the postion currently
                HeadName.Text = "N/A";
            }
            if (CurrentSemesterID != 0)
            {//there is an assigned current semester
                SemesterName.Text = CurrentSemesterName;
                CurrentHousePoints.Text = "House Points: "+CurrentPoints.ToString();
            }
            else
            {//there is not an assigned current semester
                SemesterName.Text = "No Current Semester is Selected";
                CurrentHousePoints.Text = "House Points: 0";
            }
        }
    }
}
