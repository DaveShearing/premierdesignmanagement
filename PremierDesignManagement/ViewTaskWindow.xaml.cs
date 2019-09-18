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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ViewTaskWindow : Window
    {
        public DataStructures.TaskRowStruct selectedTask;

        public ViewTaskWindow(DataStructures.TaskRowStruct task)
        {
            InitializeComponent();

            selectedTask = task;
            TaskNameLabel.Content = selectedTask.taskName;
            StartDateLabel.Content = StartDateLabel.Content.ToString() + " " + selectedTask.startDate.ToString("dd/MM/yyyy");
            DeadlineLabel.Content = DeadlineLabel.Content.ToString() + " " + selectedTask.deadline.ToString("dd/MM/yyyy");
            AssignToLabel.Content = AssignToLabel.Content.ToString() + " " + selectedTask.assignedTo;
            StatusLabel.Content = StatusLabel.Content.ToString() + " " + selectedTask.taskStatus;
            LastEditedLabel.Content = LastEditedLabel.Content.ToString() + " " + selectedTask.lastEdited.ToString("HH:mm dd/MM/yyyy");
            DetailsTextBlock.Text = selectedTask.details;
            LastEditedByLabel.Content = LastEditedByLabel.Content.ToString() + " " + selectedTask.lastEditedBy;


            //TODO: Add code to generate rows for updates
        }

        public void EditTaskButtonClick(object sender, RoutedEventArgs e)
        {
            EditTaskWindow editTaskWindow = new EditTaskWindow(selectedTask);
            editTaskWindow.Show();
            Close();
        }

        public void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
