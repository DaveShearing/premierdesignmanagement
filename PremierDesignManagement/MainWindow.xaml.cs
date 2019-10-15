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
using Windows.UI;
using Windows.Data;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.IO;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json;
using ToastNotifications;
using AdaptiveCards;
using Colors = System.Windows.Media.Colors;

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
        public static DateTime calendarMonth = DateTime.Today;

        public static string username;
        public static string forename;
        public static string surname;

        
        private const string APP_ID = "2daa3a0e-1f2c-4cab-b33b-3c2d2890f215";

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

            refreshTimer.Interval = new TimeSpan(0,0,20);
            refreshTimer.Tick += new EventHandler(RefreshTimerTick);
            refreshTimer.Start();

            InitialiseCalendar();
            LoadTasksIntoCalendar();
        }
        
        public void RefreshTimerTick (object sender, EventArgs e)
        {
            DataHandling.GetTasksFull();
            DataHandling.GetNotifications();
            int oldNoOfNotifications = noOfNotifications;
            noOfNotifications = 0;
            bool seenByUser = false;

            foreach (DataStructures.NotificationStruct notification in DataStructures.notificationRows)
            {
                bool notificationForUser = false;
                seenByUser = false;

                foreach (string username in notification.notificationRecipients)
                {
                    if (username.Equals(Application.Current.Properties["username"]) == true)
                    {
                        notificationForUser = true;
                    }
                }

                foreach (string username in notification.readByRecipients)
                {
                    if ((username.Equals(Application.Current.Properties["username"]) == true) && (notificationForUser == true))
                    {
                        seenByUser = true;
                    }
                }

                if ((notificationForUser == true) && (seenByUser == false))
                {
                    noOfNotifications++;
                }

            }

            if (noOfNotifications != 0)
            {
                NotificationsButton.Content = "Notifications (" + noOfNotifications + ")";

                if (noOfNotifications > oldNoOfNotifications)
                {
                    /*
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

                    XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                    
                    stringElements[0].AppendChild(toastXml.CreateTextNode("New Notification(s)"));
                    //stringElements[1].AppendChild(toastXml.CreateTextNode("Open Premier Task Management to see what's changed"));

                    string imagePath = "/assets/Premier_P_Logo_Red_RGB_Small_Trans.png";
                    XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                    imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

                    ToastNotification toast = new ToastNotification(toastXml);

                    ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
                    */
                }

            } else
            {
                NotificationsButton.Content = "Notifications";
            }

            for (int i = CalendarGrid.Children.Count-1; i > 0; i--)
            {
                if (CalendarGrid.Children[i].GetType() == typeof(Grid))
                {
                    Grid grid = (Grid)CalendarGrid.Children[i];
                    CalendarGrid.Children.Remove(grid);
                }
            }

            LoadTasksIntoCalendar();
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

            if (this.WindowState == WindowState.Normal)
            {
                notifications.Top = this.Top + 150;
                notifications.Left = this.Left + 20;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                notifications.Top = this.Height - 400;
                notifications.Left = 10;
            }

            
            notifications.VerticalAlignment = VerticalAlignment.Bottom;

            while ((bool)notifications.ShowDialog())
            {

            }
            EventArgs e2 = new EventArgs();
            RefreshTimerTick(sender, e2);

            //notifications.ShowDialog();
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void InitialiseCalendar()
        {
            DateTime today = DateTime.Today;
            DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
            DayOfWeek firstDayOfMonth = firstOfMonth.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            calendarMonth = today;

            /*
            TextBlock todaymarker = new TextBlock();
            todaymarker.Text = "Today!";
            todaymarker.HorizontalAlignment = HorizontalAlignment.Center;
            todaymarker.FontSize = 20;

            TextBlock firstDayMarker = new TextBlock();
            firstDayMarker.Text = "First!";
            firstDayMarker.FontSize = 20;
            firstDayMarker.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(firstDayMarker, 2);

            if ((int)firstDayOfMonth > 0)
            {
                Grid.SetColumn(firstDayMarker, (int)firstDayOfMonth);
            } else if ((int)firstDayOfMonth == 0)
            {
                Grid.SetColumn(firstDayMarker, (int)firstDayOfMonth);
            }

            CalendarGrid.Children.Add(firstDayMarker);
            

            if ((double)(today.Day+firstDayOfMonth-1)/7 <= 1)
            {
                Grid.SetRow(todaymarker, 2);
            } else if ((double)(today.Day + firstDayOfMonth - 1) / 7 > 1 && (double)(today.Day + firstDayOfMonth - 1) / 7 <= 2)
            {
                Grid.SetRow(todaymarker, 3);
            }
            else if ((double)(today.Day + firstDayOfMonth - 1) / 7 > 2 && (double)(today.Day + firstDayOfMonth - 1) / 7 <= 3)
            {
                Grid.SetRow(todaymarker, 4);
            }
            else if ((double)(today.Day + firstDayOfMonth - 1) / 7 > 3 && (double)(today.Day + firstDayOfMonth - 1) / 7 <= 4)
            {
                Grid.SetRow(todaymarker, 5);
            }
            else if ((double)(today.Day + firstDayOfMonth - 1) / 7 > 4 && (double)(today.Day + firstDayOfMonth - 1) / 7 <= 5)
            {
                Grid.SetRow(todaymarker, 6);
            }

            Grid.SetColumn(todaymarker, (int)today.DayOfWeek);
            CalendarGrid.Children.Add(todaymarker);

            //Grid.SetColumn(todaymarker, (int)today.DayOfWeek+1);
            */

            MonthLabel.Content = DateTimeFormatInfo.InvariantInfo.GetMonthName(calendarMonth.Month);
            YearLabel.Content = calendarMonth.Year;

            for (int i = 1; i <= daysInMonth; i++)
            {
                TextBlock dayMarker = new TextBlock();
                dayMarker.Text = i.ToString();
                dayMarker.HorizontalAlignment = HorizontalAlignment.Center;
                dayMarker.VerticalAlignment = VerticalAlignment.Center;
                dayMarker.FontSize = 50;
                dayMarker.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFCACBCF");

                Rectangle dayBorder = new Rectangle();
                dayBorder.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFCACBCF");
                dayBorder.StrokeDashArray = new DoubleCollection() { 2 };

                if ((int)firstDayOfMonth < 5)
                {
                    if ((double)(i + firstDayOfMonth - 1) / 7 <= 1 && (double)(i + firstDayOfMonth - 1) / 7 > 0)
                    {
                        Grid.SetRow(dayMarker, 3);
                        Grid.SetRow(dayBorder, 3);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 1 && (double)(i + firstDayOfMonth - 1) / 7 <= 2)
                    {
                        Grid.SetRow(dayMarker, 4);
                        Grid.SetRow(dayBorder, 4);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 2 && (double)(i + firstDayOfMonth - 1) / 7 <= 3)
                    {
                        Grid.SetRow(dayMarker, 5);
                        Grid.SetRow(dayBorder, 5);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 3 && (double)(i + firstDayOfMonth - 1) / 7 <= 4)
                    {
                        Grid.SetRow(dayMarker, 6);
                        Grid.SetRow(dayBorder, 6);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 4 && (double)(i + firstDayOfMonth - 1) / 7 <= 5)
                    {
                        Grid.SetRow(dayMarker, 7);
                        Grid.SetRow(dayBorder, 7);
                    }
                    else
                    {
                        Grid.SetRow(dayMarker, 2);
                        Grid.SetRow(dayBorder, 2);
                    }
                } else
                {
                    if ((double)(i + firstDayOfMonth - 1) / 7 <= 1 && (double)(i + firstDayOfMonth - 1) / 7 > 0)
                    {
                        Grid.SetRow(dayMarker, 2);
                        Grid.SetRow(dayBorder, 2);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 1 && (double)(i + firstDayOfMonth - 1) / 7 <= 2)
                    {
                        Grid.SetRow(dayMarker, 3);
                        Grid.SetRow(dayBorder, 3);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 2 && (double)(i + firstDayOfMonth - 1) / 7 <= 3)
                    {
                        Grid.SetRow(dayMarker, 4);
                        Grid.SetRow(dayBorder, 4);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 3 && (double)(i + firstDayOfMonth - 1) / 7 <= 4)
                    {
                        Grid.SetRow(dayMarker, 5);
                        Grid.SetRow(dayBorder, 5);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 4 && (double)(i + firstDayOfMonth - 1) / 7 <= 5)
                    {
                        Grid.SetRow(dayMarker, 6);
                        Grid.SetRow(dayBorder, 6);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 5)
                    {
                        Grid.SetRow(dayMarker, 8);
                        Grid.SetRow(dayBorder, 8);
                    }
                    else
                    {
                        Grid.SetRow(dayMarker, 7);
                        Grid.SetRow(dayBorder, 7);
                    }
                }
                

                DateTime day = new DateTime(today.Year, today.Month, i);

                if ((int)day.DayOfWeek == 0)
                {
                    Grid.SetColumn(dayMarker, 7);
                    Grid.SetColumn(dayBorder, 7);
                } else
                {
                    Grid.SetColumn(dayMarker, (int)day.DayOfWeek);
                    Grid.SetColumn(dayBorder, (int)day.DayOfWeek);
                }

                
                CalendarGrid.Children.Add(dayMarker);
                CalendarGrid.Children.Add(dayBorder);
            }

        }

        private void LoadTasksIntoCalendar()
        {
            DateTime today = DateTime.Today;
            DateTime firstOfMonth = new DateTime(calendarMonth.Year, calendarMonth.Month, 1);
            DayOfWeek firstDayOfMonth = firstOfMonth.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(calendarMonth.Year, calendarMonth.Month);

            for (int i = 1; i <= daysInMonth; i++)
            {
                DateTime day = new DateTime(calendarMonth.Year, calendarMonth.Month, i);
                Grid dayGrid = new Grid();
                int dayRows = 0;
                
                foreach (DataStructures.TaskRowStruct task in DataStructures.taskRows)
                {
                    if (day >= task.startDate && day <= task.deadline)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        dayGrid.RowDefinitions.Add(rowDefinition);
                        Rectangle taskRectangle = new Rectangle();
                        taskRectangle.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF505B");
                        if (day == today)
                        {
                            taskRectangle.Opacity = 0.85;
                        } else
                        {
                            taskRectangle.Opacity = 0.7;
                        }
                        taskRectangle.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFCACBCF");
                        taskRectangle.MouseUp += CalendarTaskClick;

                        Label taskLabel = new Label();
                        taskLabel.Content = task.taskName;
                        taskLabel.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
                        taskLabel.HorizontalAlignment = HorizontalAlignment.Center;
                        taskLabel.VerticalAlignment = VerticalAlignment.Center;
                        taskLabel.FontWeight = FontWeights.Bold;
                        taskLabel.Name = "task";
                        taskLabel.MouseUp += CalendarTaskLabelClick;

                        Grid.SetRow(taskRectangle, dayRows);
                        Grid.SetRow(taskLabel, dayRows);
                        dayGrid.Children.Add(taskRectangle);
                        dayGrid.Children.Add(taskLabel);
                        dayRows++;
                    }
                }

                if ((double)(i + firstDayOfMonth - 1) / 7 <= 1)
                {
                    Grid.SetRow(dayGrid, 2);
                }
                else if ((double)(i + firstDayOfMonth - 1) / 7 > 1 && (double)(i + firstDayOfMonth - 1) / 7 <= 2)
                {
                    Grid.SetRow(dayGrid, 3);
                }
                else if ((double)(i + firstDayOfMonth - 1) / 7 > 2 && (double)(i + firstDayOfMonth - 1) / 7 <= 3)
                {
                    Grid.SetRow(dayGrid, 4);
                }
                else if ((double)(i + firstDayOfMonth - 1) / 7 > 3 && (double)(i + firstDayOfMonth - 1) / 7 <= 4)
                {
                    Grid.SetRow(dayGrid, 5);
                }
                else if ((double)(i + firstDayOfMonth - 1) / 7 > 4 && (double)(i + firstDayOfMonth - 1) / 7 <= 5)
                {
                    Grid.SetRow(dayGrid, 6);
                }

                if ((int)day.DayOfWeek == 0)
                {
                    Grid.SetColumn(dayGrid, 7);
                }
                else
                {
                    Grid.SetColumn(dayGrid, (int)day.DayOfWeek);
                }

                CalendarGrid.Children.Add(dayGrid);
            }

        }

        private void CalendarTaskClick (object sender, MouseButtonEventArgs e)
        {
            int children = ((e.Source as Rectangle).Parent as Grid).Children.Count;

            for (int i = children-1; i >= 0; i--)
            {
                if (((e.Source as Rectangle).Parent as Grid).Children[i].GetType() == typeof(Label)) 
                {
                    if (Grid.GetRow(((e.Source as Rectangle).Parent as Grid).Children[i]) == Grid.GetRow((e.Source as Rectangle)))
                    {
                        string taskname = (((e.Source as Rectangle).Parent as Grid).Children[i] as Label).Content.ToString();

                        foreach (DataStructures.TaskRowStruct task in DataStructures.taskRows)
                        {
                            if (task.taskName.Equals(taskname))
                            {
                                ViewTaskWindow viewTask = new ViewTaskWindow(task);
                                viewTask.Show();
                            }
                        }
                    }
                }
            }
        }

        private void CalendarTaskLabelClick(object sender, MouseButtonEventArgs e)
        {
            int children = ((e.Source as Label).Parent as Grid).Children.Count;

            for (int i = children - 1; i >= 0; i--)
            {
                if (((e.Source as Label).Parent as Grid).Children[i].GetType() == typeof(Label))
                {
                    if (Grid.GetRow(((e.Source as Label).Parent as Grid).Children[i]) == Grid.GetRow((e.Source as Label)))
                    {
                        string taskname = (((e.Source as Label).Parent as Grid).Children[i] as Label).Content.ToString();

                        foreach (DataStructures.TaskRowStruct task in DataStructures.taskRows)
                        {
                            if (task.taskName.Equals(taskname))
                            {
                                ViewTaskWindow viewTask = new ViewTaskWindow(task);
                                viewTask.Show();
                            }
                        }
                    }
                }
            }
        }

        private void PreviousMonthMouseUp (object sender, MouseButtonEventArgs e)
        {
            for (int i = CalendarGrid.Children.Count - 1; i > 0; i--)
            {
                if (CalendarGrid.Children[i].GetType() == typeof(Grid))
                {
                    Grid grid = (Grid)CalendarGrid.Children[i];
                    CalendarGrid.Children.Remove(grid);
                }
            }

            DateTime newMonth = calendarMonth.AddMonths(-1);
            calendarMonth = newMonth;

            MonthLabel.Content = DateTimeFormatInfo.InvariantInfo.GetMonthName(calendarMonth.Month);
            YearLabel.Content = calendarMonth.Year;

            ChangeCalendarMonth();
            LoadTasksIntoCalendar();
        }

        private void NextMonthMouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int i = CalendarGrid.Children.Count - 1; i > 0; i--)
            {
                if (CalendarGrid.Children[i].GetType() == typeof(Grid))
                {
                    Grid grid = (Grid)CalendarGrid.Children[i];
                    CalendarGrid.Children.Remove(grid);
                }
            }

            DateTime newMonth = calendarMonth.AddMonths(1);
            calendarMonth = newMonth;

            MonthLabel.Content = DateTimeFormatInfo.InvariantInfo.GetMonthName(calendarMonth.Month);
            YearLabel.Content = calendarMonth.Year;

            ChangeCalendarMonth();
            LoadTasksIntoCalendar();
        }

        private void ChangeCalendarMonth()
        {
            DateTime firstOfMonth = new DateTime(calendarMonth.Year, calendarMonth.Month, 1);
            DayOfWeek firstDayOfMonth = firstOfMonth.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(calendarMonth.Year, calendarMonth.Month);

            
            for (int i = CalendarGrid.Children.Count-1; i > 0; i--)
            {
                if (Grid.GetRow(CalendarGrid.Children[i]) > 1)
                {
                    CalendarGrid.Children.RemoveAt(i);
                }
            }

            for (int i = 1; i <= daysInMonth; i++)
            {
                TextBlock dayMarker = new TextBlock();
                dayMarker.Text = i.ToString();
                dayMarker.HorizontalAlignment = HorizontalAlignment.Center;
                dayMarker.VerticalAlignment = VerticalAlignment.Center;
                dayMarker.FontSize = 50;
                dayMarker.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFCACBCF");

                Rectangle dayBorder = new Rectangle();
                dayBorder.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFCACBCF");
                dayBorder.StrokeDashArray = new DoubleCollection() { 2 };


                if ((int)firstDayOfMonth != 7)
                {
                    if ((double)(i + firstDayOfMonth - 1) / 7 <= 1 && (double)(i + firstDayOfMonth - 1) / 7 > 0)
                    {
                        Grid.SetRow(dayMarker, 2);
                        Grid.SetRow(dayBorder, 2);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 1 && (double)(i + firstDayOfMonth - 1) / 7 <= 2)
                    {
                        Grid.SetRow(dayMarker, 3);
                        Grid.SetRow(dayBorder, 3);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 2 && (double)(i + firstDayOfMonth - 1) / 7 <= 3)
                    {
                        Grid.SetRow(dayMarker, 4);
                        Grid.SetRow(dayBorder, 4);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 3 && (double)(i + firstDayOfMonth - 1) / 7 <= 4)
                    {
                        Grid.SetRow(dayMarker, 5);
                        Grid.SetRow(dayBorder, 5);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 4 && (double)(i + firstDayOfMonth - 1) / 7 <= 5)
                    {
                        Grid.SetRow(dayMarker, 6);
                        Grid.SetRow(dayBorder, 6);
                    }
                    else
                    {
                        Grid.SetRow(dayMarker, 2);
                        Grid.SetRow(dayBorder, 2);
                    }

                } else
                {
                    if ((double)(i + firstDayOfMonth - 1) / 7 <= 1 && (double)(i + firstDayOfMonth - 1) / 7 > 0)
                    {
                        Grid.SetRow(dayMarker, 2);
                        Grid.SetRow(dayBorder, 2);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 1 && (double)(i + firstDayOfMonth - 1) / 7 <= 2)
                    {
                        Grid.SetRow(dayMarker, 3);
                        Grid.SetRow(dayBorder, 3);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 2 && (double)(i + firstDayOfMonth - 1) / 7 <= 3)
                    {
                        Grid.SetRow(dayMarker, 4);
                        Grid.SetRow(dayBorder, 4);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 3 && (double)(i + firstDayOfMonth - 1) / 7 <= 4)
                    {
                        Grid.SetRow(dayMarker, 5);
                        Grid.SetRow(dayBorder, 5);
                    }
                    else if ((double)(i + firstDayOfMonth - 1) / 7 > 4 && (double)(i + firstDayOfMonth - 1) / 7 <= 5)
                    {
                        Grid.SetRow(dayMarker, 6);
                        Grid.SetRow(dayBorder, 6);
                    }
                    else
                    {
                        Grid.SetRow(dayMarker, 7);
                        Grid.SetRow(dayBorder, 7);
                    }
                }
                    

                DateTime day = new DateTime(calendarMonth.Year, calendarMonth.Month, i);

                if ((int)day.DayOfWeek == 0)
                {
                    Grid.SetColumn(dayMarker, 7);
                    Grid.SetColumn(dayBorder, 7);
                }
                else
                {
                    Grid.SetColumn(dayMarker, (int)day.DayOfWeek);
                    Grid.SetColumn(dayBorder, (int)day.DayOfWeek);
                }


                CalendarGrid.Children.Add(dayMarker);
                CalendarGrid.Children.Add(dayBorder);
            }
        }



    }


}
