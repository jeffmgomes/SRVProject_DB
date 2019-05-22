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
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TafeBuddy_SRV.Views
{
    // Class created to handle the object in the ComboBox
    class Item
    {
        public string Id;
        public string Value;
        public Item(string id, string value)
        {
            Id = id;
            Value = value;
        }
        public override string ToString()
        {
            return Value; // The ComboBox actually uses the ToString method to display the text in the control
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Student : Page
    {
        private string User;
        private static string StudentID;
        public Student()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //base.OnNavigatedTo(e);
            User = e.Parameter.ToString();

            PopulateAreasOfStudy();
            PopulateQualification();
            PopulateUser();

            areaOfStudcomboBox.SelectedIndex = 0; // Select the first item the list
            comboBox.SelectedIndex = 0; // Select the first item in the list

            StudentResult();
        }

        public void PopulateAreasOfStudy()
        {

            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);

            StringBuilder sb = new StringBuilder();
            sb.Append("select distinct a.AreasOfStudyID, a.Name from Person p ");
            sb.Append("inner join student s on p.personid = s.PersonID ");
            sb.Append("inner join student_qualification sq on s.StudentID = sq.StudentID ");
            sb.Append("inner join qualification q on sq.QualificationID = q.QualificationID ");
            sb.Append("inner join areasofstudy a on q.AreasOfStudyID = a.AreasOfStudyID ");
            sb.Append("WHERE p.Email = '").Append(User).Append("' ");
            sb.Append(" OR s.Login = '").Append(User).Append("' ");
            // Creates the SQL command
            MySqlCommand command = new MySqlCommand(sb.ToString(), conn);

            MySqlDataReader dr; // Creates a reader to read the data

            conn.Open(); // Open the connection

            dr = command.ExecuteReader(); // Execute the command and attach to the reader

            areaOfStudcomboBox.Items.Clear(); // Clear all the items

            // While there are rows in the read
            while (dr.Read())
            {
                // Add an item in the Combobox
                //Combo c = new Combo() { id = dr.GetString("AreasOfStudyID"), value = dr.GetString("Name") };
                //areaOfStudcomboBox.Items.Insert(dr.GetInt32("AreasOfStudyID"), dr.GetString("Name"));

                areaOfStudcomboBox.Items.Add(new Item(dr.GetString("AreasOfStudyID"), dr.GetString("Name")));

            }

            // Close the connection
            conn.Close();
        }

        public void PopulateQualification(string area = "")
        {
            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);

            StringBuilder sb = new StringBuilder();
            sb.Append("select distinct q.QualificationID, q.NationalTitle from Person p ");
            sb.Append("inner join student s on p.personid = s.PersonID ");
            sb.Append("inner join student_qualification sq on s.StudentID = sq.StudentID ");
            sb.Append("inner join qualification q on sq.QualificationID = q.QualificationID ");
            sb.Append("inner join areasofstudy a on q.AreasOfStudyID = a.AreasOfStudyID ");
            sb.Append("WHERE (p.Email = '").Append(User).Append("' ");
            sb.Append(" OR s.Login = '").Append(User).Append("') ");

            if (!String.IsNullOrEmpty(area))
            {
                sb.Append(" AND a.AreasOfStudyID = '").Append(area).Append("' ");
            }

            // Creates the SQL command
            MySqlCommand command = new MySqlCommand(sb.ToString(), conn);

            MySqlDataReader dr; // Creates a reader to read the data

            conn.Open(); // Open the connection

            dr = command.ExecuteReader(); // Execute the command and attach to the reader

            comboBox.Items.Clear(); // Clear all the items
            // While there are rows in the read
            while (dr.Read())
            {
                // Add an item in the Combobox
                comboBox.Items.Add(new Item(dr.GetString("QualificationID"), dr.GetString("NationalTitle")));
            }

            // Close the connection
            conn.Close();
        }

        public void PopulateUser()
        {
            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);
            // Creates the SQL command
            MySqlCommand command = new MySqlCommand("SELECT * FROM Person p INNER JOIN Student s ON p.personid = s.personid WHERE p.Email = '" + User + "' OR s.Login = '" + User + "'", conn);

            MySqlDataReader dr; // Creates a reader to read the data

            conn.Open(); // Open the connection

            dr = command.ExecuteReader(); // Execute the command and attach to the reader

            // While there are rows in the read
            while (dr.Read())
            {
                StudentID = dr.GetString("StudentID"); // Stores the StudentID to use in the Competences page
                studentIDtxtblk2.Text = dr.GetString("Login");
                studNametxtblk2.Text = dr.GetString("FirstName") + " " + dr.GetString("LastName");
            }

            // Close the connection
            conn.Close();
        }

        public void PopulateQualInfo(string qualID)
        {
            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);

            StringBuilder sb = new StringBuilder();
            sb.Append("select * from qualification p ");
            sb.Append("WHERE qualificationID = '").Append(qualID).Append("' ");
            sb.Append("; ");
            sb.Append("select * from `release` ");
            sb.Append(" where releaseid = (SELECT MAX(releaseid) FROM qualification_release_comp WHERE qualificationid = '" + qualID + "'); ");

            // Creates the SQL command
            MySqlCommand command = new MySqlCommand(sb.ToString(), conn);

            MySqlDataReader dr; // Creates a reader to read the data

            conn.Open(); // Open the connection

            dr = command.ExecuteReader(); // Execute the command and attach to the reader

            // While there are rows in the read
            while (dr.Read())
            {
                //Combo c = new Combo { id = dr.GetString("QualificationID"), value = dr.GetString("NationalTitle") };
                // Add an item in the Combobox
                tafeCodetxtblk2.Text = dr.GetString("TafeCode");
                nationalCodetxtblk2.Text = dr.GetString("NationalCode");
            }

            if (dr.NextResult()) {
                while (dr.Read())
                {
                    unitstxtblk2.Text = dr.GetString("NumTotalUnits") + " Units - " + dr.GetString("NumCoreUnits") + " core, " + dr.GetString("NumElectiveUnits") + " electives";
                }
            }
            
            // Close the connection
            conn.Close();
        }

        public void StudentResult() {
            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.StudentID, s.Login, p.FirstName, p.LastName, q.nationaltitle, c.nationalcode, c.`description`, IF(qrc.iscore = 1, 'Core', 'Elective') AS 'Type', c.tafecode AS 'Subject', IF(ISNULL(sce.nationalcode), 'Not Enrolled', IF(ISNULL(sce.Result),'No Result',sce.Result)) AS 'Situation' ");
            sb.Append("FROM qualification_release_comp qrc ");
            sb.Append("INNER JOIN qualification q ON qrc.qualificationid = q.qualificationid ");
            sb.Append("INNER JOIN `release` r ON qrc.releaseid = r.releaseid ");
            sb.Append("INNER JOIN competence c ON qrc.nationalcode = c.nationalcode ");
            sb.Append("LEFT JOIN  student_competences_enroll sce ON qrc.nationalcode = sce.nationalcode ");
            sb.Append("AND sce.studentid = '").Append(StudentID).Append("' ");
            sb.Append("LEFT JOIN  student s ON sce.studentid = s.studentid ");
            sb.Append("LEFT JOIN  person p ON s.personid = p.personid ");
            sb.Append("WHERE q.qualificationid = '").Append(((Item)comboBox.SelectedItem).Id).Append("' ");
            sb.Append("AND r.releaseid = (SELECT MAX(releaseid) FROM qualification_release_comp ");
            sb.Append(" WHERE qualificationid = '").Append(((Item)comboBox.SelectedItem).Id).Append("') ");
            sb.Append("ORDER BY q.qualificationid , `type`; ");


            // Creates the SQL command
            MySqlCommand command = new MySqlCommand(sb.ToString(), conn);

            MySqlDataReader dr; // Creates a reader to read the data

            conn.Open(); // Open the connection

            dr = command.ExecuteReader(); // Execute the command and attach to the reader

            int rowsCount = 0;
            int marked = 0;
            // While there are rows in the read
            while (dr.Read())
            {
                rowsCount += 1;
                string situation = dr.GetString("Situation");
                if (situation == "Pass") {
                    marked += 1;
                }                
            }

            // Close the connection
            conn.Close();

            // Calculates the Percentage
            double percent = 0;
            percent = ((double)marked / (double)rowsCount) * 100;
            myProgressBar.Value = percent; // Updates the progressbar value
        }

        private void AreaOfStudcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateQualification(((Item)areaOfStudcomboBox.SelectedItem).Id);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateQualInfo(((Item)comboBox.SelectedItem).Id);
            StudentResult();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Login));
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Competences),new string[] {StudentID, ((Item)comboBox.SelectedItem).Id });
        }
    }
}
