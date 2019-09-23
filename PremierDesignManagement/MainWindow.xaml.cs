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
using System.Threading;


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
            Application.Current.Resources.Add("taskrows", DataStructures.taskRows);

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

            DataHandling.GetUsers();

            //Opens new Log In Window
            LogInWindow logIn = new LogInWindow();
            logIn.Show();
            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)10;

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

            DataHandling.GetTasksFull();
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

            DataHandling.GetTasksFull();
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

        private void EditTaskButtonClick(object sender, RoutedEventArgs e)
        {
            DataStructures.TaskRowStruct selectedTask = new DataStructures.TaskRowStruct();
            
            if ((sender as Button).Name.Equals(EditTaskButton.Name))
            {
                selectedTask = (DataStructures.TaskRowStruct)TaskList2.SelectedItems[0];
            } else if ((sender as Button).Name.Equals(EditTaskButton_ByYou.Name))
            {
                selectedTask = (DataStructures.TaskRowStruct)AssignedByYouList.SelectedItems[0];
            } else
            {
                selectedTask = (DataStructures.TaskRowStruct)AssignedToYouList.SelectedItems[0];
            }

            try
            {
                Window editTask = new EditTaskWindow(selectedTask);
                editTask.Show();
            } catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Please select a task");
            }
        }

        private void ViewTaskButtonClick(object sender, RoutedEventArgs e)
        {
            DataStructures.TaskRowStruct selectedTask = new DataStructures.TaskRowStruct();

            if ((sender as Button).Name.Equals(ViewTaskButton.Name))
            {
                selectedTask = (DataStructures.TaskRowStruct)TaskList2.SelectedItems[0];
            }
            else if ((sender as Button).Name.Equals(ViewTaskButton_ByYou.Name))
            {
                selectedTask = (DataStructures.TaskRowStruct)AssignedByYouList.SelectedItems[0];
            }
            else
            {
                selectedTask = (DataStructures.TaskRowStruct)AssignedToYouList.SelectedItems[0];
            }

            try
            {
                ViewTaskWindow viewTaskWindow = new ViewTaskWindow(selectedTask);
                viewTaskWindow.Show();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Please select a task");
            }

        }

        private void UpdateTasksButtonClick(object sender, RoutedEventArgs e)
        {
            DataHandling.GetTasksFull();
        }

        private void MouseDoubleClickItem (object sender, RoutedEventArgs e)
        {
            DataStructures.TaskRowStruct selectedTask = new DataStructures.TaskRowStruct();

            if ((sender as ListViewItem).IsDescendantOf(TaskList2Grid))
            {
                selectedTask = (sender as DataStructures.TaskRowStruct);
            }
            else if ((sender as Button).Name.Equals(ViewTaskButton_ByYou.Name))
            {
                selectedTask = (DataStructures.TaskRowStruct)AssignedByYouList.SelectedItems[0];
            }
            else
            {
                selectedTask = (DataStructures.TaskRowStruct)AssignedToYouList.SelectedItems[0];
            }

            try
            {
                ViewTaskWindow viewTaskWindow = new ViewTaskWindow(selectedTask);
                viewTaskWindow.Show();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Please select a task");
            }
        }

    }
}
