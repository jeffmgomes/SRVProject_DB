using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

namespace TafeBuddy_SRV.Views
{
    enum UserType {
        Student = 0,
        Staff = 1,
        Unknown = -1
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        private UserType LoginUser(string user, string pass) {
            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);
            // Creates the SQL commands
            MySqlCommand getStudent = new MySqlCommand("SELECT * FROM Person p INNER JOIN Student s ON p.personid = s.personid WHERE p.Email = '"+user+"' OR s.Login = '"+user+"'", conn);
            MySqlCommand getStaff = new MySqlCommand("SELECT * FROM Person p INNER JOIN Staff s ON p.personid = s.personid WHERE p.Email = '" + user + "' OR s.Login = '" + user + "'", conn);

            MySqlDataReader drStudent; // Creates a reader to read the data
            MySqlDataReader drStaff; // Creates a reader to read the data

            string passCompare = ""; // Variable to store the password from the DataBase
            UserType result = UserType.Unknown; // Variable to store the result of this method

            conn.Open(); // Open the connection to execute the Student Select
            drStudent = getStudent.ExecuteReader(); // Execute the command and attach to the reader 

            // Check if there are rows
            if (drStudent.HasRows)
            {
                // If so, get the password in the database
                while (drStudent.Read())
                {
                    // Get the password from DataBase
                    passCompare = drStudent.GetString("Password");
                }
                if (passCompare == pass) { result = UserType.Student; }
            }

            conn.Close();// Close the connection for the Student select

            conn.Open(); // Open the connection for the Staff select
            drStaff = getStaff.ExecuteReader(); // Execute the command and attach to the reader

            // Check if there are rows
            if (drStaff.HasRows) {

                // If so, get the password in the database
                while (drStaff.Read())
                {
                    // Get the password from DataBase
                    passCompare = drStaff.GetString("Password");
                }
                if (passCompare == pass) { result = UserType.Staff; }
            }

            conn.Close();// Close the connection for the Staff

            return result; // Return the result
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msg;
            if (String.IsNullOrEmpty(usernameTextBox.Text)) {
                msg = new MessageDialog("Provide your Student ID or Email!");
                await msg.ShowAsync();
                return;
            }

            if (String.IsNullOrEmpty(passwordTextBox.Password)) {
                msg = new MessageDialog("Provide your password!");
                await msg.ShowAsync();
                return;
            }

            string user = usernameTextBox.Text;
            string pass = passwordTextBox.Password;

            UserType result = LoginUser(user, pass);

            if (result == UserType.Student)
            {
                Frame.Navigate(typeof(Student),user);                
            }
            else if (result == UserType.Staff) {
                Frame.Navigate(typeof(Admin));
            }
            else {
                msg = new MessageDialog("User name or password incorrect. Try again!");
                await msg.ShowAsync();
                return;
            }
        }
    }
}
