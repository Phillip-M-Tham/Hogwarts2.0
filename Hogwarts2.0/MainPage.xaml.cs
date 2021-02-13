using System;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaPlayer player;
        public MainPage()
        {
            InitializeComponent();
            player = new MediaPlayer();
            SetmySong(player);
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

        string usernameInput = "";
        string passwordInput = "";
        string validUsername = "Mr. Krabs"; //WE NEED TO GET A TRUE RETURN STATEMENT FROM A SQL SELECT STATEMETN HERE SHOUDLD NOT BE PASSING THE USERNAME AND PASSWORD 
        string validPassword = "admin"; //


        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            usernameInput = Username.Text;
            passwordInput = Password.Password;

            // var testUserInput = new MessageDialog($"{usernameInput} is username and {passwordInput} is password"); //this is to test my user inputs
            // testUserInput.ShowAsync();//
            if (usernameInput != validUsername || passwordInput != validPassword)
            {
                var notValidMessage = new MessageDialog($"usernameInput and password is not valid"); //This is when the username is not valid
                await notValidMessage.ShowAsync();
            }
            else
            {
                var validMessage = new MessageDialog("Username and password is correct");//This will be the place to put the function to move to the correct dashboard.
                await validMessage.ShowAsync();
                player.Source = null;
                player.Dispose();
            }
        }
        
        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                player.Dispose();
                //Register registerPage = new Register();
                //Frame.Navigate(typeof(Register),claymore );
                //registerPage.InitializeComponent();
                Frame.Navigate(typeof(Register));
                //ContentFrame.Navigate
                //player.Source = null;
               
                // this.Content.n
                //this.Frame.Navigate(typeof(Register),null);
                //this.Content = registerPage;
                //Frame.NavigateToType(registerPage,MediaPlayer);
            }
            catch(Exception ex) 
            {
                var errormes = new MessageDialog(ex.Message);
                await errormes.ShowAsync();

            }
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
