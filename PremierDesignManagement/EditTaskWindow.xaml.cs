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
using System.Collections.Specialized;

namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for CreateTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        public DataStructures.TaskRowStruct selectedTask;
        string assignedUsername;

        public EditTaskWindow(DataStructures.TaskRowStruct task)
        {
            InitializeComponent();
            AssignToComboBox.ItemsSource = Properties.Settings.Default.UsersStringCollection;
            StatusComboBox.ItemsSource = (StringCollection)Application.Current.Resources["MainWorkflow"];

            selectedTask = task;

            TaskNameTextBox.Text = selectedTask.taskName;
            StartDatePicker.SelectedDate = selectedTask.startDate;
            DeadlinePicker.SelectedDate = selectedTask.deadline;
            AssignToComboBox.SelectedItem = selectedTask.assignedTo;
            StatusComboBox.SelectedItem = selectedTask.taskStatus;
            TaskDetailsTextBox.Text = selectedTask.details;
        }

        private void EditTaskButtonClick (object sender, RoutedEventArgs e)
        {
            int taskID = DataHandling.GetTaskID(selectedTask.taskName, selectedTask.details);
            int assignedToUserIndex = Properties.Settings.Default.UsersStringCollection.IndexOf(AssignToComboBox.SelectedItem.ToString());
            assignedUsername = Properties.Settings.Default.UsernamesStringCollection[assignedToUserIndex];

            if (selectedTask.taskFiles == null)
            {
                selectedTask.taskFiles = new List<string>();
            }

            string[] taskFilesArray = selectedTask.taskFiles.ToArray();
            string taskFilesString = string.Join(",", taskFilesArray);

            DataHandling.UpdateTask(taskID, TaskNameTextBox.Text, StartDatePicker.SelectedDate.Value, DeadlinePicker.SelectedDate.Value, TaskDetailsTextBox.Text,
                selectedTask.taskListFileTableDir, selectedTask.assignedBy, assignedUsername, StatusComboBox.Text, taskFilesString);

            string updateString = GetUpdatedFields();
            DataHandling.AddTaskUpdate(taskID, updateString);

            DataHandling.GetTasksFull();

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

        //TODO: Add Task Update entries for all changed fields. (base string in class with += for each changed field)

        private string GetUpdatedFields ()
        {
            string updateString = "Changed: ";

            if (TaskNameTextBox.Text.Equals(selectedTask.taskName) == false)
            {
                updateString += "Task Name to " + TaskNameTextBox.Text + ", ";
            }

            if (StartDatePicker.SelectedDate.Value.Equals(selectedTask.startDate) == false)
            {
                updateString += "Start Date to " + StartDatePicker.SelectedDate.Value.ToString("dd/MM/yyyy") + ", ";
            }

            if (DeadlinePicker.SelectedDate.Value.Equals(selectedTask.deadline) == false)
            {
                updateString += "Deadline to " + DeadlinePicker.SelectedDate.Value.ToString("dd/MM/yyyy") + ", ";
            }

            if (TaskDetailsTextBox.Text.Equals(selectedTask.details) == false)
            {
                updateString += "Task Details, ";
            }

            if (assignedUsername.Equals(selectedTask.assignedTo) ==  false)
            {
                updateString += "Assignee to " + AssignToComboBox.SelectedValue.ToString() + ", ";
            }

            if (StatusComboBox.SelectedValue.ToString().Equals(selectedTask.taskStatus) == false)
            {
                updateString += "Status to " + StatusComboBox.SelectedValue.ToString() + ", ";
            }

            return updateString;
        }

    }
}
