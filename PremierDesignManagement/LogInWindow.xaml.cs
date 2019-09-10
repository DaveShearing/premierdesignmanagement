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

            string salt = security.GenerateSalt();

            string passwordhash = security.HashPassword(PasswordTextBox.Text, salt);

            MainWindow.Username = UsernameTextBox.Text;

            //Sets welcome string to User
            Application.Current.Properties["username"] = UsernameTextBox.Text;
            Application.Current.Resources["WelcomeTextString"] = "Welcome, " + Application.Current.Properties["username"];
            
            Close();
        }

        //Create New User
        private void CreateUserTextBlockClick(object sender, RoutedEventArgs e)
        {
            Window createUser = new CreateUserWindow();
            createUser.Show();
        }

    }
}
