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
using System.Data.SqlClient;
using System.Data;

namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        public CreateUserWindow()
        {
            InitializeComponent();
        }

        //Creates new User in Database.Users Table
        private void CreateUserButtonClick(object sender, RoutedEventArgs e)
        {
            Security security = new Security();
            SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString);

            string userSalt = security.GenerateSalt();
            string hashedPassword = security.HashPassword(PasswordTextBox.Password, userSalt);

            //Initialise CheckUsernameSP
            SqlCommand checkUserComm = new SqlCommand("CheckUsernameSP", sqlConnection);
            checkUserComm.CommandType = CommandType.StoredProcedure;
            checkUserComm.Parameters.AddWithValue("@username", UsernameTextBox.Text);
            SqlParameter userFound = new SqlParameter("@userFound", SqlDbType.Int);
            userFound.Direction = ParameterDirection.Output;
            userFound.Size = 30;
            checkUserComm.Parameters.Add(userFound);

            //Run CheckUsernameSP
            sqlConnection.Open();
            int x = checkUserComm.ExecuteNonQuery();
            int userFoundInDB = (int)checkUserComm.Parameters["@userFound"].Value;

            //Add user to DB if not already
            if (userFoundInDB == 0)
            {
                SqlCommand sqlCommand = new SqlCommand("CreateUserSP", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@username", UsernameTextBox.Text);
                sqlCommand.Parameters.AddWithValue("@forename", ForenameTextBox.Text);
                sqlCommand.Parameters.AddWithValue("@surname", SurnameTextBox.Text);
                sqlCommand.Parameters.AddWithValue("@emailaddress", EmailTextBox.Text);
                sqlCommand.Parameters.AddWithValue("@passwordSalt", userSalt);
                sqlCommand.Parameters.AddWithValue("@passwordHash", hashedPassword);

                //sqlConnection.Open();

                int i = sqlCommand.ExecuteNonQuery();

                if (i == 0)
                {
                    Console.WriteLine("User Created");
                }

                sqlConnection.Close();

                Close();

            } else
            {
                Console.WriteLine("User already exists. Please Log In.");
            }

            

            

            sqlConnection.Close();

            //Close();
        }

        //Cancel User creation
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
