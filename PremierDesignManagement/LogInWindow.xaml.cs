using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;




namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        //Cancel Log In
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Log in as User
        private void LogInButtonClick(object sender, RoutedEventArgs e)
        {
            Security security = new Security();
            SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.DatabaseConnHome);
            SqlCommand sqlCommand = new SqlCommand("GetSaltAndHashSP", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@username", (string)UsernameTextBox.Text);
            //sqlCommand.Parameters.Add("@passwordSalt", SqlDbType.NChar);
            //sqlCommand.Parameters.Add("@passwordHash", SqlDbType.NChar);

            SqlParameter returnedSalt = new SqlParameter("@passwordSalt", SqlDbType.NVarChar);
            SqlParameter returnedHash = new SqlParameter("@passwordHash", SqlDbType.NVarChar);
            SqlParameter returnedUserFound = new SqlParameter("@userFound", SqlDbType.Int);

            returnedSalt.Direction = ParameterDirection.Output;
            returnedHash.Direction = ParameterDirection.Output;
            returnedUserFound.Direction = ParameterDirection.Output;
            returnedSalt.Size = 256;
            returnedHash.Size = 256;

            sqlCommand.Parameters.Add(returnedSalt);
            sqlCommand.Parameters.Add(returnedHash);
            sqlCommand.Parameters.Add(returnedUserFound);

            sqlConnection.Open();
            int i = sqlCommand.ExecuteNonQuery();

            
            string databaseSalt = sqlCommand.Parameters["@passwordSalt"].Value.ToString();
            string databaseHash = sqlCommand.Parameters["@passwordHash"].Value.ToString();
            int userFound = (int)sqlCommand.Parameters["@userFound"].Value;
            

            sqlConnection.Close();
                        
            string passwordhash = security.HashPassword(PasswordTextBox.Text, databaseSalt);

            Console.WriteLine(databaseSalt);
            Console.WriteLine(databaseHash);
            Console.WriteLine(passwordhash);

            if (databaseHash.Equals(passwordhash))
            {
                Console.WriteLine("Log In Success");

                MainWindow.Username = UsernameTextBox.Text;

                //Sets welcome string to User
                Application.Current.Properties["username"] = UsernameTextBox.Text;
                Application.Current.Resources["WelcomeTextString"] = "Welcome, " + Application.Current.Properties["username"];

                Close();

            } else
            {
                if (userFound == 0)
                {
                    Console.WriteLine("User not found.");
                } else
                {
                    Console.WriteLine("Incorrect password.");
                }


                Console.WriteLine("Log In Failed");
            }

            
            
            
        }

        //Create New User
        private void CreateUserTextBlockClick(object sender, RoutedEventArgs e)
        {
            Window createUser = new CreateUserWindow();
            createUser.Show();
        }

    }
}
