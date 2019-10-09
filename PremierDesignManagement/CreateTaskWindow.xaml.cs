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
using System.IO;
using Microsoft.Win32;
using System.Collections.Specialized;

namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        DataStructures.TaskRowStruct newTask = new DataStructures.TaskRowStruct();
        OpenFileDialog selectFiles = new OpenFileDialog();
        

        public CreateTaskWindow()
        {
            InitializeComponent();
            AssignToComboBox.ItemsSource = Properties.Settings.Default.UsersStringCollection;
            StatusComboBox.ItemsSource = (StringCollection)Application.Current.Resources["MainWorkflow"];
        }

        private void CreateTaskButtonClick (object sender, RoutedEventArgs e)
        {
            int assignedToUserIndex = Properties.Settings.Default.UsersStringCollection.IndexOf(AssignToComboBox.Text);
            string assignedUsername = Properties.Settings.Default.UsernamesStringCollection[assignedToUserIndex];

            newTask.taskName = TaskNameTextBox.Text;
            newTask.startDate = StartDatePicker.SelectedDate.Value;
            newTask.deadline = DeadlinePicker.SelectedDate.Value;
            newTask.details = TaskDetailsTextBox.Text;
            newTask.taskListFileTableDir = TaskNameTextBox.Text;
            newTask.assignedBy = System.Windows.Application.Current.Properties["username"].ToString();
            newTask.assignedTo = assignedUsername;
            newTask.taskStatus = StatusComboBox.SelectedValue.ToString();
            newTask.lastEdited = DateTime.Now;
            newTask.taskFiles = new List<string>();
            newTask.notifyUsers = new List<string>();
            string[] taskFilesArray = newTask.taskFiles.ToArray();
            string taskFilesString = string.Join(",", taskFilesArray);

            string[] notifyUsersArray = new string[0];
            notifyUsersArray.Append(newTask.assignedBy);
            notifyUsersArray.Append(newTask.assignedTo);
            string notifyUsersString = string.Join(",", notifyUsersArray);

            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                
                SqlCommand createTask = new SqlCommand("CreateTaskSP", sqlConn);
                createTask.CommandType = CommandType.StoredProcedure;
                createTask.Parameters.AddWithValue("@taskname", newTask.taskName);
                createTask.Parameters.AddWithValue("@startdate", newTask.startDate);
                createTask.Parameters.AddWithValue("@deadline", newTask.deadline);
                createTask.Parameters.AddWithValue("@details", newTask.details);
                createTask.Parameters.AddWithValue("@tasklistfiletabledir", newTask.taskListFileTableDir);
                createTask.Parameters.AddWithValue("@assignedby", newTask.assignedBy);
                createTask.Parameters.AddWithValue("@assignedto", newTask.assignedTo);
                createTask.Parameters.AddWithValue("@taskstatus", newTask.taskStatus);
                createTask.Parameters.AddWithValue("@lastedited", newTask.lastEdited);
                createTask.Parameters.AddWithValue("@taskFiles", taskFilesString);
                createTask.Parameters.AddWithValue("@notifyUsers", notifyUsersString);

                sqlConn.Open();
                int i = createTask.ExecuteNonQuery();

                string sqlQuery = "INSERT INTO dbo.TaskListFiles (name,is_directory,is_archive) VALUES ('" + TaskNameTextBox.Text + "', 1, 0);";
                SqlCommand createFileTableDirComm = new SqlCommand(sqlQuery,sqlConn);
                createFileTableDirComm.CommandType = CommandType.Text;
                createFileTableDirComm.ExecuteNonQuery();

                Directory.SetCurrentDirectory(Properties.Settings.Default.FileDirectory);
                Directory.CreateDirectory(TaskNameTextBox.Text);

                
            }

            DataHandling.GetTasksFull();

            int newTaskID = DataHandling.GetTaskID(newTask.taskName, newTask.details);

            DataStructures.NotificationStruct notificationStruct = new DataStructures.NotificationStruct();
            notificationStruct.notificationSender = Application.Current.Properties["username"].ToString();
            notificationStruct.notificationText = "Created Task: " + newTask.taskName;
            notificationStruct.taskID = newTaskID;
            notificationStruct.notificationTime = DateTime.Now;
            notificationStruct.notificationRecipients = new List<string>();
            notificationStruct.notificationRecipients.Add(newTask.assignedTo);
            notificationStruct.notificationRecipients.Add(newTask.assignedBy);

            DataHandling.AddNotification(notificationStruct);

            string taskFilesUpdateString = DataHandling.AddTaskFiles(newTask, selectFiles);

            if (taskFilesUpdateString.Equals("Added File(s): ") == false)
            {
                DataHandling.AddTaskUpdate(newTaskID, taskFilesUpdateString);

                DataHandling.GetTasksFull();
            }


            Close();
        }

        private void CancelButtonClick (object sender, RoutedEventArgs e)
        {
            DataHandling.GetTasksFull();
            Close();
        }

        private void TaskDetailsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AddFilesButtonClick (object sender, RoutedEventArgs e)
        {
            selectFiles.Multiselect = true;
            selectFiles.ShowDialog();
        }

        
    }
}
