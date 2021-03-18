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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CounselorEnrollStudents : Page
    {
        private string _userHuid;
        public CounselorEnrollStudents()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _userHuid = e.Parameter.ToString();
        }
        private void EnrollGryffindorCancel_Click(object sender, RoutedEventArgs e)
        {
            GryffindorOptions.Visibility = Visibility.Collapsed;
        }

        private void EnableGryffindor(object sender, RoutedEventArgs e)
        {
            GryffindorOptions.Visibility = Visibility.Visible;
        }

        private void EnrollGryffindor_Click(object sender, RoutedEventArgs e)
        {
            GryffindorEnroll.Visibility = Visibility.Visible;
            GryffindorOptions.Visibility = Visibility.Collapsed;
        }
        private void DisenrollGryffindor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GryffindorEnrollCancel_Click(object sender, RoutedEventArgs e)
        {
            GryffindorEnroll.Visibility = Visibility.Collapsed;
            GryffindorOptions.Visibility = Visibility.Visible;
        }

        private void GryffindorEnrollSubmit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
