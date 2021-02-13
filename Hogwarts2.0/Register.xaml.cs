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
using Windows.UI.Popups;
using Windows.Media.Playback;
using Windows.Media.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Hogwarts2._0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        private string facFirstName = "";
        private string facLastName = "";
        private string facUserName = "";
        private string facPassword = "";
        private string facCPassword = "";
        
        private string stuFirstName = "";
        private string stuLastName = "";
        private string stuUserName = "";
        private string stuPassword = "";
        private string stuCPassword = "";

        private string mymessage = "";
        private string house = "";
        private string pet = "";
        private int Rpoints, Gpoints, Spoints, Hpoints;

        private string frole = "";
        private Random _random = new Random();
        public Register()
        {
            this.InitializeComponent();
            //Frame.Navigate(typeof(Register));
        }
                        
        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            //MainPage Login = new MainPage();
            //Window.Current.Content=
            Frame.Navigate(typeof(MainPage));
            //this.Frame.Navigate(typeof(MainPage));
            //this.Content = Login;
            
        }

        private void Student_Click(object sender, RoutedEventArgs e)
        {
            StudentRegisterForm.Visibility = Visibility.Visible;
            FacultyRegisterForm.Visibility = Visibility.Collapsed;
            StudentRegButton.Visibility = Visibility.Collapsed;
            FacultyRegButton.Visibility = Visibility.Collapsed;
        }
        private void StudentRegisterCancel_Click(object sender, RoutedEventArgs e)
        {
            sFirstNameInput.Text = "";
            sLastNameInput.Text = "";
            sUsernameInput.Text = "";
            sPasswordInput.Text = "";
            sConfirmPasswordInput.Text = "";
            sFirstNameWarning.Visibility = Visibility.Collapsed;
            sLastNameWarning.Visibility = Visibility.Collapsed;
            sUsernameWarning.Visibility = Visibility.Collapsed;
            sPasswordWarning.Visibility = Visibility.Collapsed;
            sPasswordWarning2.Visibility = Visibility.Collapsed;
            sCPasswordWarning.Visibility = Visibility.Collapsed;
            StudentRegisterForm.Visibility = Visibility.Collapsed;
            StudentRegButton.Visibility = Visibility.Visible;
            FacultyRegButton.Visibility = Visibility.Visible;
        }

        private async void StudentRegisterContinue_Click(object sender, RoutedEventArgs e)
        {
            //This goes to sorting hat part
            stuFirstName = sFirstNameInput.Text;
            stuLastName = sLastNameInput.Text;
            stuUserName = sUsernameInput.Text;
            stuPassword = sPasswordInput.Text;
            stuCPassword = sConfirmPasswordInput.Text;
            int stuValid = 0;
            //CHeck for blanks first
            if(stuFirstName == "")
            {
                sFirstNameWarning.Visibility = Visibility.Visible;
                stuValid--;
            }
            else
            {
                sFirstNameWarning.Visibility = Visibility.Collapsed;
                stuValid++;
            }
            if (stuLastName == "")
            {
                sLastNameWarning.Visibility = Visibility.Visible;
                stuValid--;
            }
            else
            {
                sLastNameWarning.Visibility = Visibility.Collapsed;
                stuValid++;
            }
            if(stuUserName == "")
            {
                sUsernameWarning.Visibility = Visibility.Visible;
                stuValid--;
            }
            else
            {
                sUsernameWarning.Visibility = Visibility.Collapsed;
                stuValid++;
            }
            if(stuPassword == "")
            {
                sPasswordWarning2.Visibility = Visibility.Visible;
                sPasswordWarning.Visibility = Visibility.Collapsed;
                stuValid--;
            }
            else
            {
                sPasswordWarning2.Visibility = Visibility.Collapsed;
                stuValid++;
            }
            if (stuCPassword == "")
            {
                sCPasswordWarning.Visibility = Visibility.Visible;
                stuValid--;
            }
            else
            {
                sCPasswordWarning.Visibility = Visibility.Collapsed;
                stuValid++;
            }
            //passwords check to be equal
            if (stuCPassword != "" && stuPassword != "")
            {
                sCPasswordWarning.Visibility = Visibility.Collapsed;
                sPasswordWarning2.Visibility = Visibility.Collapsed;
                if (stuPassword == stuCPassword)
                {
                    sPasswordWarning.Visibility = Visibility.Collapsed;
                    stuValid++;
                }
                else
                {
                    sPasswordWarning.Visibility = Visibility.Visible;
                    stuValid--;
                }
            }
            //This checks username
            if (stuValid == 6){
                var validStuMessage1 = new MessageDialog($"first name is {stuFirstName} \nlast name is {stuLastName} \nusername is {stuUserName} \npassword is {stuPassword}");
                await validStuMessage1.ShowAsync();
                StudentRegisterForm.Visibility = Visibility.Collapsed;
                StudentRegisterForm2.Visibility = Visibility.Visible;
            }
        }
        private void Faculty_Click(object sender, RoutedEventArgs e)
        {
            FacultyRegisterForm.Visibility = Visibility.Visible;
            StudentRegisterForm.Visibility = Visibility.Collapsed;
            StudentRegButton.Visibility = Visibility.Collapsed;
            FacultyRegButton.Visibility = Visibility.Collapsed;   
        }
        private void FacultyRegisterCancel_Click(object sender, RoutedEventArgs e)
        {
            fFirstNameInput.Text = "";
            fLastNameInput.Text = "";
            fUsernameInput.Text = "";
            fPasswordInput.Text = "";
            fConfirmPasswordInput.Text = "";
            fFirstNameWarning.Visibility = Visibility.Collapsed;
            fLastNameWarning.Visibility = Visibility.Collapsed;
            fUsernameWarning.Visibility = Visibility.Collapsed;
            fPasswordWarning.Visibility = Visibility.Collapsed;
            fPasswordWarning2.Visibility = Visibility.Collapsed;
            fCPasswordWarning.Visibility = Visibility.Collapsed;
            FacultyRegisterForm.Visibility = Visibility.Collapsed;
            StudentRegButton.Visibility = Visibility.Visible;
            FacultyRegButton.Visibility = Visibility.Visible;
        }
        private async void FacultyRegister_Click(object sender, RoutedEventArgs e)
        {
            facFirstName=fFirstNameInput.Text;
            facLastName=fLastNameInput.Text;
            facUserName=fUsernameInput.Text;
            facPassword=fPasswordInput.Text;
            facCPassword=fConfirmPasswordInput.Text;
            if (fRoleInput.SelectedValue == null)
            {
                var fRoleError = new MessageDialog("You need to select a role");
                await fRoleError.ShowAsync();
                //frole = "None";
            }
            else
            {
                frole = fRoleInput.SelectedValue.ToString();
            }
            int facValid= 0;
            //CHECKS FOR BLANK VALUES IN FORM FIRST
            if (facFirstName == "")
            {
                fFirstNameWarning.Visibility = Visibility.Visible;
                facValid--;
            }
            else
            {
                fFirstNameWarning.Visibility = Visibility.Collapsed;
                facValid++;
            }
            
            if (facLastName == "")
            {
                fLastNameWarning.Visibility = Visibility.Visible;
                facValid--;
            }
            else
            {
                fLastNameWarning.Visibility = Visibility.Collapsed;
                facValid++;
            }
            if(facUserName == "")
            {
                fUsernameWarning.Visibility = Visibility.Visible;
                facValid--;
            }
            else
            {
                fUsernameWarning.Visibility = Visibility.Collapsed;
                facValid++;
                //do a check to see if the username is valid here
            }
            if (facPassword == "")
            {
                fPasswordWarning2.Visibility = Visibility.Visible;
                fPasswordWarning.Visibility = Visibility.Collapsed;
                facValid--;
            }
            else
            {
                fPasswordWarning2.Visibility = Visibility.Collapsed;
                facValid++;
            }
            if (facCPassword == "")
            {
                fCPasswordWarning.Visibility = Visibility.Visible;
                facValid--;
            }
            else
            { 
                fCPasswordWarning.Visibility = Visibility.Collapsed;
                facValid++;
            }
            //THIS CHECKS FOR PASSWORD TO BE EQUAL TO EACH OTHER
            if(facCPassword !="" && facPassword != "")
            {
                fCPasswordWarning.Visibility = Visibility.Collapsed;
                fPasswordWarning2.Visibility = Visibility.Collapsed;
                if(facPassword == facCPassword)
                {
                    fPasswordWarning.Visibility = Visibility.Collapsed;
                    facValid++;
                }
                else
                {
                    fPasswordWarning.Visibility = Visibility.Visible;
                    facValid--;
                }
            }
            //Checks for username to be valid needs to be updated
            if(facValid==6)
            {
                var ValidFacMessage = new MessageDialog($"first name is {facFirstName} \nlast name is {facLastName} \nusername is {facUserName}\nrole is {frole}");
                await ValidFacMessage.ShowAsync();
                //MainPage Login = new MainPage();
                //this.Content = Login;
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void StudentBack_Click(object sender, RoutedEventArgs e)
        {
            StudentRegisterForm2.Visibility = Visibility.Collapsed;
            StudentRegisterForm.Visibility = Visibility.Visible;
        }

        private void StudentRegisterCancel2_Click(object sender, RoutedEventArgs e)
        {
            sFirstNameInput.Text = "";
            sLastNameInput.Text = "";
            sUsernameInput.Text = "";
            sPasswordInput.Text = "";
            sConfirmPasswordInput.Text = "";
            sFirstNameWarning.Visibility = Visibility.Collapsed;
            sLastNameWarning.Visibility = Visibility.Collapsed;
            sUsernameWarning.Visibility = Visibility.Collapsed;
            sPasswordWarning.Visibility = Visibility.Collapsed;
            sPasswordWarning2.Visibility = Visibility.Collapsed;
            sCPasswordWarning.Visibility = Visibility.Collapsed;
            StudentRegisterForm.Visibility = Visibility.Collapsed;
            StudentRegisterForm2.Visibility = Visibility.Collapsed;
            StudentRegButton.Visibility = Visibility.Visible;
            FacultyRegButton.Visibility = Visibility.Visible;
        }

        private void StudentRegister_Click(object sender, RoutedEventArgs e)
        {
            mymessage = "";
            Gpoints = 0;
            Spoints = 0;
            Hpoints = 0;
            Rpoints = 0;
            if (PetInput.SelectedValue == null)
            {
                pet = "None";
            }
            else {
                pet = PetInput.SelectedValue.ToString();
            }
            //Weghted question process
            if (A1.IsChecked == true)
            {
                Spoints += 5;           
            } 
            else if (B1.IsChecked == true)
            {
                Rpoints += 5;
            } 
            else if (C1.IsChecked == true)
            {
                Gpoints += 5;
            }
            else if(D1.IsChecked == true)
            {
                Hpoints += 5;
            }
            else
            {
                mymessage += "You forgot to answer question 1";
            
            }
            
            if (A2.IsChecked == true)
            {
                Gpoints += 5;
            }
            else if (B2.IsChecked == true)
            {
                Rpoints += 5;
            }
            else if (C2.IsChecked == true)
            {
                Hpoints += 5;
            }
            else if (D2.IsChecked == true)
            {
                Spoints += 5;
            }
            else
            {
                if (mymessage!="")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 2";
            }
            
            if(A3.IsChecked == true)
            {
                Rpoints += 5;
            }
            else if(B3.IsChecked==true)
            {
                Hpoints += 5;
            }
            else if (C3.IsChecked == true)
            {
                Gpoints += 5;
            }
            else if (D3.IsChecked == true)
            {
                Spoints += 5;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 3";
            }
            
            if(A4.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (B4.IsChecked == true)
            {
                Spoints += 10;
            }
            else if(C4.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (D4.IsChecked == true)
            {
                Gpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 4";
            }

            if(A5.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (B5.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (C5.IsChecked == true)
            {
                Spoints += 10;
            }
            else if (D5.IsChecked == true)
            {
                Gpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 5";
            }

            if (A6.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (B6.IsChecked == true)
            {
                Gpoints += 10;
            }
            else if (C6.IsChecked == true)
            {
                Spoints += 10;
            }
            else if (D6.IsChecked == true)
            {
                Rpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 6";
            }

            if (A7.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (B7.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (C7.IsChecked == true)
            {
                Spoints += 10;
            }
            else if (D7.IsChecked == true)
            {
                Gpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 7";
            }

            if (A8.IsChecked == true)
            {
                Spoints += 10;
            }
            else if (B8.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (C8.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (D8.IsChecked == true)
            {
                Gpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 8";
            }

            if (A9.IsChecked == true)
            {
                Gpoints += 10;
            }
            else if (B9.IsChecked == true)
            {
                Spoints += 10;
            }
            else if (C9.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (D9.IsChecked == true)
            {
                Hpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 9";
            }

            if (A10.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (B10.IsChecked == true)
            {
                Gpoints += 10;
            }
            else if (C10.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (D10.IsChecked == true)
            {
                Spoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 10";
            }

            if (A11.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (B11.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (C11.IsChecked == true)
            {
                Gpoints += 10;
            }
            else if (D11.IsChecked == true)
            {
                Spoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 11";
            }

            if (A12.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (B12.IsChecked == true)
            {
                Spoints += 10;
            }
            else if (C12.IsChecked == true)
            {
                Gpoints += 10;
            }
            else if (D12.IsChecked == true)
            {
                Hpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 12";
            }

            if (A13.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (B13.IsChecked == true)
            {
                Gpoints += 10;
            }
            else if (C13.IsChecked == true)
            {
                Spoints += 10;
            }
            else if (D13.IsChecked == true)
            {
                Rpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 13";
            }

            if (A14.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (B14.IsChecked == true)
            {
                Spoints += 10;
            }
            else if (C14.IsChecked == true)
            {
                Gpoints += 10;
            }
            else if (D14.IsChecked == true)
            {
                Rpoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 14";
            }

            if (A15.IsChecked == true)
            {
                Rpoints += 10;
            }
            else if (B15.IsChecked == true)
            {
                Gpoints += 10;
            }
            else if (C15.IsChecked == true)
            {
                Hpoints += 10;
            }
            else if (D15.IsChecked == true)
            {
                Spoints += 10;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 15";
            }

            if (A16.IsChecked == true)
            {
                Gpoints += 50;
            }
            else if (B16.IsChecked == true)
            {
                Rpoints += 50;
            }
            else if (C16.IsChecked == true)
            {
                Hpoints += 50;
            }
            else if (D16.IsChecked == true)
            {
                Spoints += 50;
            }
            else
            {
                if (mymessage != "")
                {
                    mymessage += "\n";
                }
                mymessage += "You forgot to answer question 16";
            }

            if (mymessage != "")
            {
                var NotValidStuMessage = new MessageDialog((mymessage));
                NotValidStuMessage.ShowAsync();
            }
            else
            {
                calculateHouse(Gpoints, Rpoints, Spoints, Hpoints);
                mymessage += $"\npet is {pet} Slytherin is {Spoints}, Ravenclaw is {Rpoints}, Gryffindor is {Gpoints} and Hufflepuff is {Hpoints}\nYOU ARE IN {house}";
                var ValidStuMessage = new MessageDialog(mymessage);
                ValidStuMessage.ShowAsync();
            }
        }

        private async void calculateHouse(int gpoints, int rpoints, int spoints, int hpoints)
        {
            int[] numbers = new int[] { gpoints, rpoints, spoints,hpoints };
            int maximumNumber = numbers.Max();
            //CHECKS FOR 4 HOUSE TIE BREAKER
            if (gpoints == rpoints && rpoints == spoints && spoints == hpoints)
            {
                int mytiebreaker = RandomHouse(1, 5);
                if (mytiebreaker == 1)
                {
                    house = "Gryffindor";
                }
                else if (mytiebreaker == 2)
                {
                    house = "Ravenclaw";
                }
                else if (mytiebreaker == 3)
                {
                    house = "Slytherin";
                }
                else if (mytiebreaker == 4)
                {
                    house = "Hufflepuff";
                }
                else
                {
                    var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                    await Fixme.ShowAsync();
                }
            }
            else
            {
                //check for 3 house tie breaker
                if ((gpoints == rpoints && rpoints == spoints) || (gpoints == rpoints && rpoints == hpoints) || (gpoints == spoints && spoints == hpoints) || (rpoints == spoints && spoints == hpoints))
                {
                    int mytiebreaker = RandomHouse(1, 4);
                    //tie with gryffindor, ravenclaw and slytherin
                    if (gpoints == rpoints && rpoints == spoints)
                    {
                       if( mytiebreaker == 1)
                        {
                            house = "Gryffindor";
                        }
                        else if(mytiebreaker == 2)
                        {
                            house = "Ravenclaw";
                        }
                        else if(mytiebreaker == 3)
                        {
                            house = "Slytherin";
                        }
                        else
                        {
                            var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                            await Fixme.ShowAsync();
                        }

                    }
                    //tie with gryffindor, ravenclaw and hufflepuff
                    else if(gpoints == rpoints && rpoints == hpoints)
                    {
                        if (mytiebreaker == 1)
                        {
                            house = "Gryffindor";
                        }
                        else if (mytiebreaker == 2)
                        {
                            house = "Ravenclaw";
                        }
                        else if (mytiebreaker == 3)
                        {
                            house = "Hufflepuff";
                        }
                        else
                        {
                            var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                            await Fixme.ShowAsync();
                        }
                    }
                    //tie with grffindor, slytherin and hufflepuff
                    else if(gpoints==spoints && spoints== hpoints)
                    {
                        if (mytiebreaker == 1)
                        {
                            house = "Gryffindor";
                        }
                        else if (mytiebreaker == 2)
                        {
                            house = "Slytherin";
                        }
                        else if (mytiebreaker == 3)
                        {
                            house = "Hufflepuff";
                        }
                        else
                        {
                            var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                            await Fixme.ShowAsync();
                        }
                    }
                    //tie with ravenclaw, slytherin and hufflepuff
                    else if(rpoints==spoints && spoints == hpoints)
                    {
                        if (mytiebreaker == 1)
                        {
                            house = "Ravenclaw";
                        }
                        else if (mytiebreaker == 2)
                        {
                            house = "Slytherin";
                        }
                        else if (mytiebreaker == 3)
                        {
                            house = "Hufflepuff";
                        }
                        else
                        {
                            var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                            await Fixme.ShowAsync();
                        }
                    }
                    else
                    {
                        var threetieFixme = new MessageDialog("The threeway tie is broken fix me");
                        await threetieFixme.ShowAsync();
                    }
                }
                else
                {
                    //check for 2 house tiebreaker
                    if (gpoints == rpoints || gpoints == spoints || gpoints == hpoints || rpoints == spoints || rpoints == hpoints || spoints == hpoints)
                    {
                        int mytiebreaker = RandomHouse(1, 3);
                        //if Gryffindor and ravenclaw tie
                        if (gpoints == rpoints)
                        {
                            if (mytiebreaker == 1)
                            {
                                house = "Gryffindor";
                            }
                            else if(mytiebreaker ==2) 
                            {
                                house = "Ravenclaw";
                            }
                            else
                            {
                                var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                                await Fixme.ShowAsync();
                            }
                        }
                        //if Gryffindor and Slytherin tie
                        else if(gpoints == spoints)
                        {
                            if (mytiebreaker == 1)
                            {
                                house = "Gryffindor";
                            }
                            else if (mytiebreaker == 2)
                            {
                                house = "Slytherin";
                            }
                            else
                            {
                                var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                                await Fixme.ShowAsync();
                            }
                        }
                        //if  Gryffindor and Hufflepuff tie
                        else if(gpoints == hpoints)
                        {
                            if (mytiebreaker == 1)
                            {
                                house = "Gryffindor";
                            }
                            else if (mytiebreaker == 2)
                            {
                                house = "Hufflepuff";
                            }
                            else
                            {
                                var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                                await Fixme.ShowAsync();
                            }
                        }
                        // if Ravenclaw and Slytherin tie 
                        else if (rpoints == spoints)
                        {
                            if (mytiebreaker == 1)
                            {
                                house = "Ravenclaw";
                            }
                            else if (mytiebreaker == 2)
                            {
                                house = "Slytherin";
                            }
                            else
                            {
                                var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                                await Fixme.ShowAsync();
                            }
                        }
                        // if Ravenclaw and Hufflepuff tie 
                        else if(rpoints == hpoints)
                        {
                            if (mytiebreaker == 1)
                            {
                                house = "Ravenclaw";
                            }
                            else if (mytiebreaker == 2)
                            {
                                house = "Hufflepuff";
                            }
                            else
                            {
                                var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                                await Fixme.ShowAsync();
                            }
                        }
                        //if  Slytherin and Hufflepuff tie
                        else if(spoints == hpoints)
                        {
                            if (mytiebreaker == 1)
                            {
                                house = "Slytherin";
                            }
                            else if (mytiebreaker == 2)
                            {
                                house = "Hufflepuff";
                            }
                            else
                            {
                                var Fixme = new MessageDialog("THE RANDOM GEN IS BROKEN");
                                await Fixme.ShowAsync();
                            }
                        }
                        else
                        {
                            var twotieFixme = new MessageDialog("The two tiebreaker is broken fix me please");
                            await twotieFixme.ShowAsync();
                        }
                    }
                    else
                    {
                        //assign a house normally
                        if (maximumNumber == gpoints)
                        {
                            house = "Gryffindor";
                        }
                        else if (maximumNumber == rpoints)
                        {
                            house = "Ravenclaw";
                        }
                        else if (maximumNumber == spoints)
                        {
                            house = "Slytherin";
                        }
                        else if (maximumNumber == hpoints)
                        {
                            house = "Hufflepuff";
                        }
                    }
                }
            }
        }
        //fixes tie breaker scenerio
        private int RandomHouse(int min, int max)
        {
            int tiebreaker=_random.Next(min, max);
            return tiebreaker;
        }
    }
}
