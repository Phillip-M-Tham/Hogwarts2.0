using System;
using System.Data.SqlClient;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Connecting to Database
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private string usernameInput = "";
        private string passwordInput = "";
        private string house = "";
        private int HUID=0;
        MediaPlayer player;
        public MainPage()
        {
            InitializeComponent();
            player = new MediaPlayer();
            SetmySong(player);
            //SetDataConn();
        }

        private async void SetmySong(MediaPlayer player)
        {
            if (player.Source == null)
            {
                Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
                Windows.Storage.StorageFile file = await folder.GetFileAsync("testsong.wav");
                player.AutoPlay = true;
                player.Source = MediaSource.CreateFromStorageFile(file);
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            //connects to database
            usernameInput = Username.Text;
            passwordInput = Password.Password;
            bool validUser = false;
            string queryresult = " ";
            string queryresult2 = "";
            string ValidUserQuery = $"SELECT HUID FROM Users WHERE Username = '{usernameInput}' AND UserPassword ='{passwordInput}';";
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = ValidUserQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    queryresult += reader.GetValue(0).ToString();
                                }
                                if(queryresult==" ")
                                {
                                    validUser = false;
                                }
                                else
                                {
                                    validUser = true;
                                }
                            }
                        }
                        sqlConn.Close();
                    }
                    else
                    {
                        var validMessage = new MessageDialog("Weesnaw");//This will be the place to put the function to move to the correct dashboard.
                        await validMessage.ShowAsync();
                    }
                }
            }catch(Exception ex)
            {
                var errormes = new MessageDialog(ex.Message);
                await errormes.ShowAsync();
            }

            if (validUser == false)
            {
                var notValidMessage = new MessageDialog($"usernameInput and password is not valid"); //This is when the username is not valid
                await notValidMessage.ShowAsync();
            }
            else
            {
                string ManueverQuery = $"SELECT PositionType FROM Positions WHERE HUID = {queryresult};";
                try
                {
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            using (SqlCommand cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = ManueverQuery;
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        queryresult2 += reader.GetValue(0).ToString();
                                    }
                                }
                            }
                            if(queryresult2 == "S")
                            {
                                //IF position is a Student Find the house they are in and set it to house string
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT HouseName FROM Houses WHERE HUID = {queryresult};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            house += reader.GetValue(0).ToString();
                                        }
                                    }
                                }
                            }
                        }
                        sqlConn.Close();
                    }
                }
                catch(Exception ex)
                {
                    var errormes = new MessageDialog(ex.Message);
                    await errormes.ShowAsync();
                }
                HUID = Int32.Parse(queryresult);//HOPEFULLY THIS WORKS 
                //var validMessage = new MessageDialog("Username and password is correct HUID IS "+HUID);//This will be the place to put the function to move to the correct dashboard.
                //await validMessage.ShowAsync();
                //player.Source = null;
                player.Dispose();

                if (queryresult2 == "S")
                {
                    if (house == "Slytherin")
                    {
                        Frame.Navigate(typeof(SlytherinStu),HUID);
                    }else if(house == "Gryffindor")
                    {
                        Frame.Navigate(typeof(GryffindorStu), HUID);
                    }
                    else if(house == "Ravenclaw")
                    {
                        Frame.Navigate(typeof(RavenclawStu), HUID);
                    }
                    else if(house == "Hufflepuff")
                    {
                        Frame.Navigate(typeof(HufflepuffStu), HUID);
                    }
                    else
                    {
                        var HouseProbMessage = new MessageDialog("HOUSE NAME IS SPELLED WRONG SOMEWHERE");
                        await HouseProbMessage.ShowAsync();
                    }
                }
                else if (queryresult2 == "P")
                {
                    Frame.Navigate(typeof(ProfessorMain), HUID);
                }
                else if (queryresult2 == "C")
                {
                    Frame.Navigate(typeof(CounselorMain), HUID);
                }
                else if (queryresult2 == "H")
                {
                    Frame.Navigate(typeof(HeadMain), HUID);
                }
                else
                {
                    var RoleProbMessage = new MessageDialog("THIS PERSON DOESN'T HAVE THE RIGHT ROLE SOMEWHERE");
                    await RoleProbMessage.ShowAsync();
                }
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
                player.Dispose();
                Frame.Navigate(typeof(Register));
        }

        private void Sound_On_Click(object sender, RoutedEventArgs e)
        {
            Sound_On.Visibility = Visibility.Collapsed;
            Sound_Off.Visibility = Visibility.Visible;
            player.Pause();
        }

        private void Sound_Off_Click(object sender, RoutedEventArgs e)
        {
            Sound_Off.Visibility = Visibility.Collapsed;
            Sound_On.Visibility = Visibility.Visible;
            player.Play();
        }
    }
}
