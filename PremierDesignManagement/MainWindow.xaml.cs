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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

        

    public partial class MainWindow : Window
    {
        

        public static string username;
        public static string forename;
        public static string surname;

        public static String Username
        {
            get { return username; }
            set { username = value; }
        }

        public MainWindow()
        {
            InitializeComponent();

            username = null;
            forename = null;
            surname = null;

            LogInWindow logIn = new LogInWindow();
            logIn.Show();
            Application.Current.Resources["BlurEffectRadius"] = (double)10;

        }
        
        //Opens Log In Window
        private void LogInButtonClick(object sender, RoutedEventArgs e)
        {
            Window logIn = new LogInWindow();
            logIn.Show();
        }

        //Shows Home Tab
        private void HomeButtonClick(object sender, RoutedEventArgs e)
        {
            HomeBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
            CalendarBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            TaskListBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            HomeGrid.Visibility = Visibility.Visible;
            CalendarGrid.Visibility = Visibility.Hidden;
            TaskListGrid.Visibility = Visibility.Hidden;
        }

        //Shows Task List Tab
        private void TaskListButtonClick(object sender, RoutedEventArgs e)
        {
            TaskListBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
            CalendarBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            HomeBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            CalendarGrid.Visibility = Visibility.Hidden;
            TaskListGrid.Visibility = Visibility.Visible;
            HomeGrid.Visibility = Visibility.Hidden;
        }

        //Shows Calendar Tab
        private void CalendarButtonClick(object sender, RoutedEventArgs e)
        {
            CalendarBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
            TaskListBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            HomeBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            TaskListGrid.Visibility = Visibility.Hidden;
            CalendarGrid.Visibility = Visibility.Visible;
            HomeGrid.Visibility = Visibility.Hidden;
        }

        /*
        //Hashes password with salt using SHA256
        private string HashPassword(string password, string salt)
        {
            
            string hashedPassword;
            byte[] hashByte;
            byte[] passwordByte = Encoding.ASCII.GetBytes(password);
            byte[] saltByte = Encoding.ASCII.GetBytes(salt);
            byte[] saltedPassword = new byte[passwordByte.Length + saltByte.Length];

            System.Buffer.BlockCopy(passwordByte, 0, saltedPassword, 0, passwordByte.Length);
            System.Buffer.BlockCopy(saltByte, 0, saltedPassword, passwordByte.Length, salt.Length);
            

            hashByte = sha256SP.ComputeHash(saltedPassword);
            hashedPassword = hashByte.ToString();

            return hashedPassword;

        }

        //Generates salt for password hashing
        private string GenerateSalt()
        {
            string salt;
            byte[] saltByte = new byte[24];

            rngSP.GetBytes(saltByte);
            salt = saltByte.ToString();

            return salt;
        }

        //Verifies entered password against database hash
        private bool VerifyPassword(string username, string password)
        {
            bool verified = false;

            


            return verified;
        }
        */
    }
}
