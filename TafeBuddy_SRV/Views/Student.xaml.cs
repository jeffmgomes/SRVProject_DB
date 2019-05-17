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
using MySql.Data.MySqlClient;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TafeBuddy_SRV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Student : Page
    {
        public Student()
        {
            this.InitializeComponent();
            ConnectDB();
        }

        public void ConnectDB() {

            // Creates the connection
            MySqlConnection conn = new MySqlConnection("server = localhost; user id = root; password = mysql; database = srvdb");
            // Creates the SQL command
            MySqlCommand command = new MySqlCommand("SELECT * FROM AreasOfStudy", conn);

            MySqlDataReader dr; // Creates a reader to read the data

            conn.Open(); // Open the connection

            dr = command.ExecuteReader(); // Execute the command and attach to the reader

            // While there are rows in the read
            while (dr.Read()) {
                // Add an item in the Combobox
                areaOfStudcomboBox.Items.Add(dr.GetString("Name"));
            }

            // Close the connection
            conn.Close();
            

        }
    }
}
