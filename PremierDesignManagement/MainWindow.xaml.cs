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
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;

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

            //Sets initial GUI state
            HomeBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
            CalendarBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            TaskListBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            HomeGrid.Visibility = Visibility.Visible;
            CalendarGrid.Visibility = Visibility.Hidden;
            TaskListGrid.Visibility = Visibility.Hidden;

            username = null;
            forename = null;
            surname = null;

            //Opens new Log In Window
            LogInWindow logIn = new LogInWindow();
            logIn.Show();
            Application.Current.Resources["BlurEffectRadius"] = (double)10;

            //Loads users from DB into app
            SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString);
            SqlCommand getUsers = new SqlCommand("SELECT Username, Forename, Surname FROM dbo.Users;", sqlConn);
            sqlConn.Open();

            SqlDataReader reader = getUsers.ExecuteReader();

            Properties.Settings.Default.UsernamesStringCollection.Clear();
            Properties.Settings.Default.UsersStringCollection.Clear();
            
            
            while (reader.Read())
            {
                string username = reader.GetString(0);
                string name = reader.GetString(1) + " " + reader.GetString(2);
                Properties.Settings.Default.UsernamesStringCollection.Add(username);
                Properties.Settings.Default.UsersStringCollection.Add(name);
            }

            reader.Close();

            //Loads tasks from DB into app
            SqlCommand getTasks = new SqlCommand("SELECT TaskName, StartDate, Deadline, AssignedTo, TaskStatus FROM dbo.Tasks", sqlConn);

            DataStructures.TaskRowStruct taskRow = new DataStructures.TaskRowStruct();
            
            
            int RowLength;

            
            reader = getTasks.ExecuteReader();

            while (reader.Read())
            {
                taskRow.taskName = reader.GetString(0);
                taskRow.startDate = reader.GetDateTime(1);
                taskRow.deadline = reader.GetDateTime(2);
                taskRow.assignedTo = reader.GetString(3);
                taskRow.taskStatus = reader.GetString(4);

                //TODO
            }

            reader.Close();
            sqlConn.Close();

            foreach (String username in Properties.Settings.Default.UsernamesStringCollection)
            {
                Console.WriteLine(username);
            }

            foreach (String name in Properties.Settings.Default.UsersStringCollection)
            {
                Console.WriteLine(name);
            }

            

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

        private void CreateTaskButtonClick(object sender, RoutedEventArgs e)
        {
            Window createTask = new CreateTaskWindow();
            createTask.Show();
        }


    }
}
