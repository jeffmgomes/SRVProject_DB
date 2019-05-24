using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

namespace TafeBuddy_SRV.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Competences : Page
    {
        private ObservableCollection<Competence> Core = new ObservableCollection<Competence>();
        private ObservableCollection<Competence> Elective = new ObservableCollection<Competence>();
        private string studentID;
        private string qualID;

        public Competences()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                string[] parameter = (string[])e.Parameter;
                studentID = parameter[0];
                qualID = parameter[1];

                int areaOfStudyIndex = int.Parse(parameter[2]);
                int qualificationIndex = int.Parse(parameter[3]);

                PopulateAreasOfStudy(studentID);
                PopulateQualification(studentID);

                areaOfStudcomboBox.SelectedIndex = areaOfStudyIndex;
                comboBox.SelectedIndex = qualificationIndex;
                
            }
        }

        public void AddCompetences(string studentID, string qualID)
        {
            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.StudentID, s.Login, p.FirstName, p.LastName, q.nationaltitle, c.nationalcode, c.`description`, IF(qrc.iscore = 1, 'Core', 'Elective') AS 'Type', c.tafecode AS 'Subject', IF(ISNULL(sce.nationalcode), 'Not Enrolled', IF(ISNULL(sce.Result),'No Result',sce.Result)) AS 'Situation' ");
            sb.Append("FROM qualification_release_comp qrc ");
            sb.Append("INNER JOIN qualification q ON qrc.qualificationid = q.qualificationid ");
            sb.Append("INNER JOIN `release` r ON qrc.releaseid = r.releaseid ");
            sb.Append("INNER JOIN competence c ON qrc.nationalcode = c.nationalcode ");
            sb.Append("LEFT JOIN  student_competences_enroll sce ON qrc.nationalcode = sce.nationalcode ");
            sb.Append("AND sce.studentid = '").Append(studentID).Append("' ");
            sb.Append("LEFT JOIN  student s ON sce.studentid = s.studentid ");
            sb.Append("LEFT JOIN  person p ON s.personid = p.personid ");
            sb.Append("WHERE q.qualificationid = '").Append(qualID).Append("' ");
            sb.Append("AND r.releaseid = (SELECT MAX(releaseid) FROM qualification_release_comp ");
            sb.Append(" WHERE qualificationid = '").Append(qualID).Append("') ");
            sb.Append("ORDER BY q.qualificationid , `type`; ");


            // Creates the SQL command
            MySqlCommand command = new MySqlCommand(sb.ToString(), conn);

            MySqlDataReader dr; // Creates a reader to read the data

            conn.Open(); // Open the connection

            dr = command.ExecuteReader(); // Execute the command and attach to the reader

            if (Core.Count > 0) {
                Core.Clear();
            }

            if (Elective.Count > 0) {
                Elective.Clear();
            }

            // While there are rows in the read
            while (dr.Read())
            {
                Competence i = new Competence(dr.GetString("Situation"), dr.GetString("NationalCode"), dr.GetString("Description"));
                if (dr.GetString("Type") == "Core")
                {
                    Core.Add(i);
                }
                else
                {
                    Elective.Add(i);
                }

                if (!dr.IsDBNull(dr.GetOrdinal("Login")))
                {
                    studentIDtxtblk2.Text = dr.GetString("Login");
                    studNametxtblk2.Text = dr.GetString("FirstName") + " " + dr.GetString("LastName");
                }
            }


            // Close the connection
            conn.Close();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Login));
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the Type of the Last page to know what to return
            Type previousPageType = Frame.BackStack.Last().SourcePageType;
            if (previousPageType == typeof(Student)) {
                Frame.Navigate(typeof(Student), new string[] { studentIDtxtblk2.Text, areaOfStudcomboBox.SelectedIndex.ToString(), comboBox.SelectedIndex.ToString() });
            }
            if (previousPageType == typeof(Admin)) {
                Frame.Navigate(typeof(Admin), new string[] { studentID, studentIDtxtblk2.Text, areaOfStudcomboBox.SelectedIndex.ToString(), comboBox.SelectedIndex.ToString()});
            }
            
        }

        public void PopulateAreasOfStudy(string studentID)
        {

            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);

            StringBuilder sb = new StringBuilder();
            sb.Append("select distinct a.AreasOfStudyID, a.Name from Person p ");
            sb.Append("inner join student s on p.personid = s.PersonID ");
            sb.Append("inner join student_qualification sq on s.StudentID = sq.StudentID ");
            sb.Append("inner join qualification q on sq.QualificationID = q.QualificationID ");
            sb.Append("inner join areasofstudy a on q.AreasOfStudyID = a.AreasOfStudyID ");
            sb.Append("WHERE s.studentID = '").Append(studentID).Append("' ");
            
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
                areaOfStudcomboBox.Items.Add(new Item(dr.GetString("AreasOfStudyID"), dr.GetString("Name")));
            }

            // Close the connection
            conn.Close();
        }

        public void PopulateQualification(string studentID, string area = "")
        {
            // Creates the connection
            MySqlConnection conn = new MySqlConnection(App.connectionString);

            StringBuilder sb = new StringBuilder();
            sb.Append("select distinct q.QualificationID, q.NationalTitle from Person p ");
            sb.Append("inner join student s on p.personid = s.PersonID ");
            sb.Append("inner join student_qualification sq on s.StudentID = sq.StudentID ");
            sb.Append("inner join qualification q on sq.QualificationID = q.QualificationID ");
            sb.Append("inner join areasofstudy a on q.AreasOfStudyID = a.AreasOfStudyID ");
            sb.Append("WHERE (s.studentID = '").Append(studentID).Append("' ");
            sb.Append(") ");

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

        private void AreaOfStudcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateQualification(studentID, ((Item)areaOfStudcomboBox.SelectedItem).Id);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddCompetences(studentID, ((Item)comboBox.SelectedItem).Id);
        }
    }





    // Class to handle the list of competences
    class Competence
    {
        public bool Marked;
        public string Code;
        public string Description;
        public string Result;

        public Competence(string situation, string code, string desc)
        {
            this.Marked = situation == "Pass"; // If the compentence is marked as Pass the checkbox will be marked
            this.Code = code;
            this.Description = desc;
            this.Result = situation;
        }
    }
}
