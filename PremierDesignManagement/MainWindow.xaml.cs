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
using System.Globalization;
using System.Windows.Threading;


namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

        

    public partial class MainWindow : Window
    {
        public CollectionView taskListView, assignedToListView, assignedByListView;
        public DataGridColumnHeader taskListSortColumn, assignedToListSortColumn, assignedByListSortColumn;
        public CultureInfo cultureInfo = new CultureInfo("en-GB", false);
        public StringCollection mainWorkflowPlusAll = new StringCollection();
        public DispatcherTimer refreshTimer = new DispatcherTimer();
        public static int noOfNotifications = 0;

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

            
            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)10;

            //Opens new Log In Window
            LogInWindow logIn = new LogInWindow();
            logIn.ShowDialog();

            foreach (String username in Properties.Settings.Default.UsernamesStringCollection)
            {
                Console.WriteLine(username);
            }

            foreach (String name in Properties.Settings.Default.UsersStringCollection)
            {
                Console.WriteLine(name);
            }

            mainWorkflowPlusAll = Properties.Settings.Default.MainWorkflow;
            mainWorkflowPlusAll.Add("All");

            refreshTimer.Interval = new TimeSpan(0,0,30);
            refreshTimer.Tick += new EventHandler(RefreshTimerTick);
            refreshTimer.Start();
        }
        
        public void RefreshTimerTick (object sender, EventArgs e)
        {
            DataHandling.GetTasksFull();
            DataHandling.GetNotifications();

            noOfNotifications = 0;

            foreach (DataStructures.NotificationStruct notification in DataStructures.notificationRows)
            {
                if (notification.readByRecipients.Contains(Application.Current.Properties["username"]) != true)
                {
                    noOfNotifications++;
                }
            }

            if (noOfNotifications != 0)
            {
                NotificationsButton.Content = "Notifications (" + noOfNotifications + ")";
            } else
            {
                NotificationsButton.Content = "Notifications";
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

        //Opens Create Task Window
        private void CreateTaskButtonClick(object sender, RoutedEventArgs e)
        {
            Window createTask = new CreateTaskWindow();
            createTask.Show();
        }

        //Opens Edit Task Window for Selected Task
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

        //Opens View Task Window for Selected Task
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

        //Refresh Task List
        private void UpdateTasksButtonClick(object sender, RoutedEventArgs e)
        {
            DataHandling.GetTasksFull();
        }

        //Opens View Task Window for Selected Task
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

        //
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

            if (columnHeader.Tag.Equals("TaskStatusColumnHeader"))
            {
                columnHeader.ContextMenu.ItemsSource = mainWorkflowPlusAll;
            }

            if (columnHeader.Tag.Equals("TaskNameColumnHeader"))
            {
                
            }
        }

        private void TaskNameContextMenuSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                string enteredText = (sender as TextBox).Text.ToString();
                DataStructures.filteredTaskRows.Clear();
                TaskList2.ItemsSource = null;

                if (enteredText != "")
                {
                    foreach (DataStructures.TaskRowStruct taskRow in DataStructures.taskRows)
                    {
                        if (cultureInfo.CompareInfo.IndexOf(taskRow.taskName, enteredText, CompareOptions.IgnoreCase) >= 0) {
                            DataStructures.filteredTaskRows.Add(taskRow);
                        }
                    }

                    TaskList2.ItemsSource = DataStructures.filteredTaskRows;

                } else
                {
                    TaskList2.ItemsSource = DataStructures.taskRows;
                }

                



            }
        }

        private void TaskStatusMenuClick(object sender, RoutedEventArgs e)
        {
            string selectedStatus = (e.OriginalSource as MenuItem).Header.ToString();

            Console.WriteLine(selectedStatus);

            DataStructures.filteredTaskRows.Clear();
            TaskList2.ItemsSource = null;

            if (selectedStatus != "All")
            {
                foreach (DataStructures.TaskRowStruct taskRow in DataStructures.taskRows)
                {
                    if (taskRow.taskStatus.Equals(selectedStatus))
                    {
                        DataStructures.filteredTaskRows.Add(taskRow);
                    }
                }

                TaskList2.ItemsSource = DataStructures.filteredTaskRows;

            }
            else
            {
                TaskList2.ItemsSource = DataStructures.taskRows;
            }

        }

        private void HeaderRightClickAssignedTo(object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader columnHeader = (sender as DataGridColumnHeader);
            columnHeader.ContextMenu.IsOpen = true;

            if (columnHeader.Tag.Equals("TaskStatusColumnHeader"))
            {
                columnHeader.ContextMenu.ItemsSource = mainWorkflowPlusAll;
            }

            if (columnHeader.Tag.Equals("TaskNameColumnHeader"))
            {

            }
        }

        private void HeaderClickAssignedTo(object sender, RoutedEventArgs e)
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

            SortDescription previousSort = assignedToListView.SortDescriptions[0];


            assignedToListView.SortDescriptions.Clear();


            ListSortDirection newDirection = ListSortDirection.Descending;


            if (assignedToListSortColumn == columnHeader && previousSort.Direction == ListSortDirection.Descending)
            {
                newDirection = ListSortDirection.Ascending;
            }

            assignedToListSortColumn = columnHeader;
            assignedToListView.SortDescriptions.Add(new SortDescription(sortBy, newDirection));
        }

        private void TaskNameContextMenuSearchBox_KeyDownAssignedTo(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                string enteredText = (sender as TextBox).Text.ToString();
                DataStructures.filteredTaskRows.Clear();
                AssignedToYouList.ItemsSource = null;

                if (enteredText != "")
                {
                    foreach (DataStructures.TaskRowStruct taskRow in DataStructures.assignedToTaskRows)
                    {
                        if (cultureInfo.CompareInfo.IndexOf(taskRow.taskName, enteredText, CompareOptions.IgnoreCase) >= 0)
                        {
                            DataStructures.filteredTaskRows.Add(taskRow);
                        }
                    }

                    AssignedToYouList.ItemsSource = DataStructures.filteredTaskRows;

                }
                else
                {
                    AssignedToYouList.ItemsSource = DataStructures.assignedToTaskRows;
                }





            }
        }

        private void TaskStatusMenuClickAssignedTo(object sender, RoutedEventArgs e)
        {
            string selectedStatus = (e.OriginalSource as MenuItem).Header.ToString();

            Console.WriteLine(selectedStatus);

            DataStructures.filteredTaskRows.Clear();
            AssignedToYouList.ItemsSource = null;

            if (selectedStatus != "All")
            {
                foreach (DataStructures.TaskRowStruct taskRow in DataStructures.assignedToTaskRows)
                {
                    if (taskRow.taskStatus.Equals(selectedStatus))
                    {
                        DataStructures.filteredTaskRows.Add(taskRow);
                    }
                }

                AssignedToYouList.ItemsSource = DataStructures.filteredTaskRows;

            }
            else
            {
                AssignedToYouList.ItemsSource = DataStructures.assignedToTaskRows;
            }

        }

        private void HeaderRightClickAssignedBy(object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader columnHeader = (sender as DataGridColumnHeader);
            columnHeader.ContextMenu.IsOpen = true;

            if (columnHeader.Tag.Equals("TaskStatusColumnHeader"))
            {
                columnHeader.ContextMenu.ItemsSource = mainWorkflowPlusAll;
            }

            if (columnHeader.Tag.Equals("TaskNameColumnHeader"))
            {

            }
        }

        private void HeaderClickAssignedBy(object sender, RoutedEventArgs e)
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

            SortDescription previousSort = assignedByListView.SortDescriptions[0];


            assignedByListView.SortDescriptions.Clear();


            ListSortDirection newDirection = ListSortDirection.Descending;


            if (assignedByListSortColumn == columnHeader && previousSort.Direction == ListSortDirection.Descending)
            {
                newDirection = ListSortDirection.Ascending;
            }

            assignedByListSortColumn = columnHeader;
            assignedByListView.SortDescriptions.Add(new SortDescription(sortBy, newDirection));
        }

        private void TaskNameContextMenuSearchBox_KeyDownAssignedBy(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                string enteredText = (sender as TextBox).Text.ToString();
                DataStructures.filteredTaskRows.Clear();
                AssignedByYouList.ItemsSource = null;

                if (enteredText != "")
                {
                    foreach (DataStructures.TaskRowStruct taskRow in DataStructures.assignedByTaskRows)
                    {
                        if (cultureInfo.CompareInfo.IndexOf(taskRow.taskName, enteredText, CompareOptions.IgnoreCase) >= 0)
                        {
                            DataStructures.filteredTaskRows.Add(taskRow);
                        }
                    }

                    AssignedByYouList.ItemsSource = DataStructures.filteredTaskRows;

                }
                else
                {
                    AssignedByYouList.ItemsSource = DataStructures.assignedByTaskRows;
                }





            }
        }

        private void TaskStatusMenuClickAssignedBy(object sender, RoutedEventArgs e)
        {
            string selectedStatus = (e.OriginalSource as MenuItem).Header.ToString();

            Console.WriteLine(selectedStatus);

            DataStructures.filteredTaskRows.Clear();
            AssignedByYouList.ItemsSource = null;

            if (selectedStatus != "All")
            {
                foreach (DataStructures.TaskRowStruct taskRow in DataStructures.assignedByTaskRows)
                {
                    if (taskRow.taskStatus.Equals(selectedStatus))
                    {
                        DataStructures.filteredTaskRows.Add(taskRow);
                    }
                }

                AssignedByYouList.ItemsSource = DataStructures.filteredTaskRows;

            }
            else
            {
                AssignedByYouList.ItemsSource = DataStructures.assignedByTaskRows;
            }

        }

        private void NotificationsButtonClick(object sender, RoutedEventArgs e)
        {
            Notifications notifications = new Notifications();

            notifications.Top = this.Top + 220;
            notifications.Left = this.Left + 20;
            notifications.ShowDialog();
        }

    }


}
