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
using System.ComponentModel;
using System.Windows.Controls.Primitives;


namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

        

    public partial class MainWindow : Window
    {
        public CollectionView taskListView;
        public DataGridColumnHeader taskListSortColumn;

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
            else if ((sender as Button).Name.Equals(ViewTaskButton_ToYou.Name))
            {
                selectedTask = (DataStructures.TaskRowStruct)AssignedToYouList.SelectedItems[0];
            } else
            {
                MessageBox.Show("Please select a task");
            }

            try
            {
                if (selectedTask != null)
                {
                    ViewTaskWindow viewTaskWindow = new ViewTaskWindow(selectedTask);
                    viewTaskWindow.Show();
                }
                
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

            selectedTask = ((FrameworkElement) e.OriginalSource).DataContext as DataStructures.TaskRowStruct;

            if (selectedTask == null)
            {
                //MessageBox.Show("Please select a task");
            }

            try
            {
                if (selectedTask != null)
                {
                    ViewTaskWindow viewTaskWindow = new ViewTaskWindow(selectedTask);
                    viewTaskWindow.Show();
                }
                
            }
            catch (ArgumentOutOfRangeException)
            {
                //MessageBox.Show("Please select a task");
            }
        }

        private void TaskListMouseClick (object sender, RoutedEventArgs e)
        {

        }

        private void HeaderClick (object sender, RoutedEventArgs e)
        {
            
            DataGridColumnHeader columnHeader = (sender as DataGridColumnHeader);
            string column = columnHeader.Content.ToString();
            string sortBy = "lastEdited";

            switch (column)
            {
                case "Task Name":
                    sortBy = "taskName";
                    break;
                case "Start Date":
                    sortBy = "startDate";
                    break;
                case "Deadline":
                    sortBy = "deadline";
                    break;
                case "Assigned To":
                    sortBy = "assignedTo";
                    break;
                case "Task Status":
                    sortBy = "taskStatus";
                    break;
                case "Last Edited":
                    sortBy = "lastEdited";
                    break;
            }

            SortDescription previousSort = taskListView.SortDescriptions[0];

            
            taskListView.SortDescriptions.Clear();
            

            ListSortDirection newDirection = ListSortDirection.Descending;
            

            if (taskListSortColumn == columnHeader && previousSort.Direction == ListSortDirection.Descending)
            {
                newDirection = ListSortDirection.Ascending;
            }

            taskListSortColumn = columnHeader;
            taskListView.SortDescriptions.Add(new SortDescription(sortBy, newDirection));
        }

        private void HeaderRightClick (object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader columnHeader = (sender as DataGridColumnHeader);
            columnHeader.ContextMenu.IsOpen = true;

            //TODO: Set context menu items to app resource.
            //TODO: Also do the same for the status combo box!!
        }

    }
}
