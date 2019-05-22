using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
        private List<Competence> Core = new List<Competence>();
        private List<Competence> Elective = new List<Competence>();

        public Competences()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                string[] parameter = (string[])e.Parameter;
                string studentID = parameter[0];
                string qualID = parameter[1];
                AddCompetences(studentID, qualID);
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
            }

            // Close the connection
            conn.Close();
        }
    }

    class Competence
    {
        public bool Marked;
        public string Code;
        public string Description;
        public string Result;

        public Competence(string situation, string code, string desc)
        {
            this.Marked = situation == "Pass";
            this.Code = code;
            this.Description = desc;
            this.Result = situation;
        }
    }
}
