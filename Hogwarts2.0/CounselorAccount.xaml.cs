using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using Windows.UI.Popups;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CounselorAccount : Page
    {
        private string _userHuid = "";
        private string _userinfo = "";
        private string _updateaboutme = "";
        const string ConnectionString = "SERVER = DESKTOP-R3J82OF\\SQLEXPRESS2019; DATABASE= Hogwarts2.0; USER ID=Cohort7; PASSWORD=tuesday313";
        private byte[] reallyAnnoyingByteArray;
        public CounselorAccount()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
            Setuserinfo(_userHuid);
            //we can add one for the picture here
        }

        private async void Setuserinfo(string myuserhuid)
        {
            byte[] profilepic = default(byte[]);
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT AboutInfo FROM Users WHERE HUID ={myuserhuid};";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    _userinfo += reader.GetValue(0).ToString();
                                }
                            }
                        }
                        using (SqlCommand cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT ProfilePic FROM ProfilePics WHERE HUID = {_userHuid}";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    profilepic = (byte[])reader.GetValue(0);
                                }
                            }
                        }
                        sqlConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new MessageDialog(ex.Message);
                await errorMessage.ShowAsync();
            }
            if (_userinfo == "")
            {
                UserInfoBox.Text = "Please add an about me by using the Edit button";
            }
            else
            {
                UserInfoBox.Text = _userinfo;
                EditUserInfoInput.Text = _userinfo;
            }
            if (profilepic != null)
            {//they have a profile pic
                BitmapImage biSource = new BitmapImage();
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(profilepic.AsBuffer());
                    stream.Seek(0);
                    await biSource.SetSourceAsync(stream);
                }
                CounselorProfilePic.Source = biSource;
            }
        }

        private void EditInfo_Click(object sender, RoutedEventArgs e)
        {
            Editform.Visibility = Visibility.Visible;
            EditInfo.Visibility = Visibility.Collapsed;
            UserInfoBox.Visibility = Visibility.Collapsed;

        }
        private async void SubmitStuUpdate_Click(object sender, RoutedEventArgs e)
        {
            //we can check to see if its an empty string
            //CONVERT TO A FRIENDLY STRING
            _updateaboutme = EditUserInfoInput.Text;
            if (_updateaboutme != "")
            {
                _updateaboutme = ConverttoFriendly(_updateaboutme);
                //int result=-1;
                byte[] result = default(byte[]);
                try
                {
                    using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == System.Data.ConnectionState.Open)
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            SqlCommand command = new SqlCommand($"UPDATE Users SET AboutInfo = '{_updateaboutme}' WHERE HUID ={_userHuid};", sqlConn);
                            adapter.UpdateCommand = command;
                            adapter.UpdateCommand.ExecuteNonQuery();

                            if (reallyAnnoyingByteArray != null)
                            {
                                using (SqlCommand cmd = sqlConn.CreateCommand())
                                {
                                    cmd.CommandText = $"SELECT ProfilePic FROM ProfilePics WHERE HUID ={_userHuid};";
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            result = (byte[])reader.GetValue(0);
                                        }
                                    }
                                }
                                if (result == null)
                                {//this is an insert*/
                                    command = new SqlCommand($"INSERT INTO ProfilePics(HUID,ProfilePic) VALUES (@id,@image);", sqlConn);
                                    command.Parameters.AddWithValue("id", _userHuid);
                                    command.Parameters.AddWithValue("image", reallyAnnoyingByteArray);
                                    adapter.InsertCommand = command;
                                    adapter.InsertCommand.ExecuteNonQuery();
                                }
                                else
                                {//update

                                    command = new SqlCommand($"UPDATE ProfilePics SET ProfilePic = @image WHERE HUID ={_userHuid};", sqlConn);
                                    command.Parameters.AddWithValue("image", reallyAnnoyingByteArray);
                                    adapter.UpdateCommand = command;
                                    adapter.UpdateCommand.ExecuteNonQuery();
                                }
                            }
                            sqlConn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = new MessageDialog(ex.Message);
                    await errorMessage.ShowAsync();
                }
                Frame.Navigate(typeof(CounselorAccount), _userHuid);
            }
            else
            {
                var NotValidMessage = new MessageDialog("About Info is Empty, please try again");
                await NotValidMessage.ShowAsync();
            }
        }

        private string ConverttoFriendly(string badstring)
        {
            var goodstring = new StringBuilder();
            foreach (var char1 in badstring)
            {
                goodstring.Append(char1);
                if (char1 == '\'')
                {
                    goodstring.Append('\'');
                }
            }
            return goodstring.ToString();
        }

        private void CancelUpdate_Click(object sender, RoutedEventArgs e)
        {
            Editform.Visibility = Visibility.Collapsed;
            EditInfo.Visibility = Visibility.Visible;
            UserInfoBox.Visibility = Visibility.Visible;
        }

        private async void Form2BrowsePictures_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap.
                    bitmapImage.SetSource(fileStream);
                }
                RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromFile(file);
                IRandomAccessStreamWithContentType streamWithContent = await streamRef.OpenReadAsync();
                byte[] buffer = new byte[streamWithContent.Size];
                await streamWithContent.ReadAsync(buffer.AsBuffer(), (uint)streamWithContent.Size, InputStreamOptions.None);
                reallyAnnoyingByteArray = buffer;
                CounselorProfilePic.Source = bitmapImage;

            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }
        }
    }
}
