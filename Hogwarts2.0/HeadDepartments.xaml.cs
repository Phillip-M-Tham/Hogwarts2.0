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
    public sealed partial class HeadDepartments : Page
    {
        private string _userHuid;
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";

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
            int depeartmentcounter = 0;
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
                                using(SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while(reader.Read())
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
                        textblock.SetValue(Grid.RowProperty, depeartmentcounter);
                        textblock.SetValue(Grid.ColumnProperty, 0);
                        textblock.HorizontalAlignment = HorizontalAlignment.Center;
                        textblock.VerticalAlignment = VerticalAlignment.Center;
                        textblock.Foreground = new SolidColorBrush(Colors.Black);
                        textblock.Text = dept;
                        textblock.FontSize = 36;
                        textblock.FontFamily = new FontFamily("/Assets/HARRYP__.TTF#Harry P");

                        DepartmentsTable1A.Children.Add(textblock);
                    }
                    else if(mode == 2)
                    {
                        RowDefinition newrow2 = new RowDefinition();
                        newrow2.Height = new GridLength(50);

                        DepartmentsTable2A.RowDefinitions.Add(newrow);
                        DepartmentsTable2Achecks.RowDefinitions.Add(newrow2);

                        CheckBox chk = new CheckBox();
                        chk.HorizontalAlignment = HorizontalAlignment.Center;
                        chk.VerticalAlignment = VerticalAlignment.Center;
                        chk.Name = sorteddepids[depeartmentcounter].ToString();
                        chk.SetValue(Grid.RowProperty, depeartmentcounter);
                        chk.SetValue(Grid.ColumnProperty, 0);
                        chk.IsChecked = false;

                        Border bd = new Border();
                        bd.BorderThickness = new Thickness(2);
                        bd.BorderBrush = new SolidColorBrush(Colors.Black);
                        bd.SetValue(Grid.RowProperty,depeartmentcounter);
                        bd.SetValue(Grid.ColumnProperty, 1);

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
                    depeartmentcounter++;
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
                    DepartmentsTable1A.RowDefinitions.Add(newrow);
                    DepartmentsTable1A.Children.Add(textblock);
                } else if (mode == 2)
                {
                    DepartmentsTable2A.RowDefinitions.Add(newrow);
                    DepartmentsTable2A.Children.Add(textblock);
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
                setupdepartments(2);
            }
        }

        private void Purgedepartmentlist(int mode)
        {
            if (mode == 1)
            {
                DepartmentsTable1A.RowDefinitions.Clear();
                DepartmentsTable1A.Children.Clear();
                DepartmentsTable1A.Visibility = Visibility.Collapsed;
            }
            else
            {
                DepartmentsTable2A.Children.Clear();
                DepartmentsTable2A.RowDefinitions.Clear();
                DepartmentsTable2A.Visibility = Visibility.Collapsed;

                DepartmentsTable2Achecks.Children.Clear();
                DepartmentsTable2Achecks.RowDefinitions.Clear();
                DepartmentsTable2Achecks.Visibility = Visibility.Collapsed;

            }
        }

        private void EditRemoveByCheck_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditSubmitAssignments_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditRemoveAssignment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditAddAssignment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditCancel_Click(object sender, RoutedEventArgs e)
        {
            EditAddAssignment.Visibility = Visibility.Collapsed;
            EditRemoveAssignment.Visibility = Visibility.Collapsed;
            EditSubmitAssignments.Visibility = Visibility.Collapsed;
            EditRemoveByCheck.Visibility = Visibility.Collapsed;
            EditCancel.Visibility = Visibility.Collapsed;
            Purgedepartmentlist(2);
            DepartmentsTable1A.Visibility = Visibility.Visible;
            setupdepartments(1);
        }
    }
}
