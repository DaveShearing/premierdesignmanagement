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
    /// Interaction logic for CreateTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        public DataStructures.TaskRowStruct selectedTask;

        public EditTaskWindow(DataStructures.TaskRowStruct task)
        {
            InitializeComponent();
            AssignToComboBox.ItemsSource = Properties.Settings.Default.UsersStringCollection;
            StatusComboBox.ItemsSource = Properties.Settings.Default.MainWorkflow;

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
            int assignedToUserIndex = Properties.Settings.Default.UsersStringCollection.IndexOf(AssignToComboBox.Text);
            string assignedUsername = Properties.Settings.Default.UsernamesStringCollection[assignedToUserIndex];

            DataHandling.UpdateTask(taskID, TaskNameTextBox.Text, StartDatePicker.SelectedDate.Value, DeadlinePicker.SelectedDate.Value, TaskDetailsTextBox.Text,
                selectedTask.taskListFileTableDir, selectedTask.assignedBy, assignedUsername, StatusComboBox.Text);
            

            DataHandling.GetTasksFull();

            Close();
        }

        private void CancelButtonClick (object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TaskDetailsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
