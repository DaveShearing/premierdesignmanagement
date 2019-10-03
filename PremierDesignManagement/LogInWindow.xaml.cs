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
            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)10;
        }

        //Cancel Log In
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)0;
            Close();
            Application.Current.Shutdown();
        }

        //Log in as User
        private void LogInButtonClick(object sender, RoutedEventArgs e)
        {
            Security security = new Security();
            SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString);
            SqlCommand sqlCommand = new SqlCommand("GetSaltAndHashSP", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@username", (string)UsernameTextBox.Text);
            
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

            try
            {

                sqlConnection.Open();
                int i = sqlCommand.ExecuteNonQuery();


                string databaseSalt = sqlCommand.Parameters["@passwordSalt"].Value.ToString();
                string databaseHash = sqlCommand.Parameters["@passwordHash"].Value.ToString();
                int userFound = (int)sqlCommand.Parameters["@userFound"].Value;

                UsernameExistsLabel.Content = "";
                PasswordIncorrectLabel.Content = "";


                string passwordhash = security.HashPassword(PasswordTextBox.Password, databaseSalt);

                Console.WriteLine(databaseSalt);
                Console.WriteLine(databaseHash);
                Console.WriteLine(passwordhash);

                if (databaseHash.Equals(passwordhash))
                {
                    Console.WriteLine("Log In Success");

                    MainWindow.Username = UsernameTextBox.Text;

                    //Sets welcome string to User
                    SqlCommand getUser = new SqlCommand("GetNamesSP", sqlConnection);
                    getUser.CommandType = CommandType.StoredProcedure;
                    getUser.Parameters.AddWithValue("@username", UsernameTextBox.Text);
                    SqlParameter forename = new SqlParameter("@forename", SqlDbType.NVarChar);
                    SqlParameter surname = new SqlParameter("@surname", SqlDbType.NVarChar);
                    forename.Direction = ParameterDirection.Output;
                    surname.Direction = ParameterDirection.Output;
                    forename.Size = 30;
                    surname.Size = 40;
                    getUser.Parameters.Add(forename);
                    getUser.Parameters.Add(surname);

                    int x = getUser.ExecuteNonQuery();
                    string forenameDB = getUser.Parameters["@forename"].Value.ToString();
                    string surnameDB = getUser.Parameters["@surname"].Value.ToString();

                    System.Windows.Application.Current.Properties["username"] = getUser.Parameters["@username"].Value.ToString();
                    System.Windows.Application.Current.Properties["forename"] = getUser.Parameters["@forename"].Value.ToString();
                    System.Windows.Application.Current.Properties["surname"] = getUser.Parameters["@surname"].Value.ToString();
                    System.Windows.Application.Current.Resources["WelcomeTextString"] = "Welcome, " + System.Windows.Application.Current.Properties["forename"];
                    System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)0;
                    System.Windows.Application.Current.Resources["LogInButtonVisibility"] = Visibility.Hidden;

                    sqlConnection.Close();

                    DataHandling.GetTasksFull();
                    DataHandling.GetNotifications();

                    foreach (DataStructures.NotificationStruct notification in DataStructures.notificationRows)
                    {
                        if (notification.readByRecipients.Contains(Application.Current.Properties["username"]) != true)
                        {
                            MainWindow.noOfNotifications++;
                        }
                    }

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(MainWindow))
                        {
                            if (MainWindow.noOfNotifications != 0)
                            {
                                (window as MainWindow).NotificationsButton.Content = "Notifications (" + MainWindow.noOfNotifications + ")";
                            }
                            else
                            {
                                (window as MainWindow).NotificationsButton.Content = "Notifications";
                            }
                        }
                    }


                    Close();

                }
                else
                {
                    if (userFound == 0)
                    {

                        UsernameExistsLabel.Content = "User not found.";
                        Console.WriteLine("User not found.");
                    }
                    else
                    {
                        PasswordIncorrectLabel.Content = "Incorrect Password";
                        Console.WriteLine("Incorrect password.");
                    }


                    Console.WriteLine("Log In Failed");
                }


                sqlConnection.Close();

            } catch (SqlException sqle)
            {
                MessageBox.Show("Server unavailable.");
            }

        }

        //Create New User
        private void CreateUserTextBlockClick(object sender, RoutedEventArgs e)
        {
            Window createUser = new CreateUserWindow();
            createUser.Show();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LogInWindowClosed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)0;
        }

    }
}
