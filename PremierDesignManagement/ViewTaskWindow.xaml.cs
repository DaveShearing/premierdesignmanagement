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
using System.IO;
using System.Diagnostics;
using System.Security;
using Microsoft.Win32;

namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ViewTaskWindow : Window
    {
        public DataStructures.TaskRowStruct selectedTask;
        public int selectedTaskID;
        public int noOfUpdates;
        public RowDefinition addUpdateRow;
        public Rectangle divider;
        public TextBlock addUpdateTextBlock;
        public TextBox addUpdateTextBox;
        public Button addUpdateButton;
        string updateText;

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


            InitUpdateRows();
            InitAddUpdateRow();

        }

        private void InitAddUpdateRow()
        {
            addUpdateRow = new RowDefinition();
            addUpdateRow.Height = new GridLength(100, GridUnitType.Auto);
            TaskDetailsGrid.RowDefinitions.Add(addUpdateRow);

            divider = new Rectangle();
            addUpdateTextBlock = new TextBlock();
            addUpdateTextBox = new TextBox();
            addUpdateButton = new Button();

            divider.Width = 470;
            divider.Height = 1;
            divider.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            divider.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            divider.Margin = new Thickness(20, 0, 0, 0);
            divider.VerticalAlignment = VerticalAlignment.Top;
            divider.HorizontalAlignment = HorizontalAlignment.Left;

            addUpdateTextBlock.Text = "Add new update...";
            addUpdateTextBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            addUpdateTextBlock.Margin = new Thickness(0, 10, 0, 0);

            addUpdateTextBox.Height = 50;
            addUpdateTextBox.Width = 510;
            addUpdateTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            addUpdateTextBox.VerticalAlignment = VerticalAlignment.Top;
            addUpdateTextBox.Margin = new Thickness(0, 30, 0, 0);
            addUpdateTextBox.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            addUpdateTextBox.Name = "addUpdateTextBox";
            addUpdateTextBox.TextChanged += AddUpdateTextChanged;
            addUpdateTextBox.TextWrapping = TextWrapping.Wrap;
            KeyBinding keyBinding = new KeyBinding(EditingCommands.EnterLineBreak, Key.Enter, ModifierKeys.Shift);
            addUpdateTextBox.KeyDown += AddUpdateKeyPressed;
            addUpdateTextBox.InputBindings.Add(keyBinding);
            

            addUpdateButton.Name = "addUpdateButton";
            addUpdateButton.Click += AddUpdateButtonClick;
            addUpdateButton.HorizontalAlignment = HorizontalAlignment.Left;
            addUpdateButton.VerticalAlignment = VerticalAlignment.Top;
            addUpdateButton.Margin = new Thickness(0, 90, 0, 0);
            addUpdateButton.Height = 30;
            addUpdateButton.Width = 100;
            addUpdateButton.Content = "Add Update";
            addUpdateButton.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
            addUpdateButton.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
            addUpdateButton.FontSize = 16;
            
            Grid.SetRow(divider, noOfUpdates+1);
            Grid.SetRow(addUpdateTextBlock, noOfUpdates + 1);
            Grid.SetRow(addUpdateTextBox, noOfUpdates + 1);
            Grid.SetRow(addUpdateButton, noOfUpdates + 1);
            TaskDetailsGrid.Children.Add(divider);
            TaskDetailsGrid.Children.Add(addUpdateTextBlock);
            TaskDetailsGrid.Children.Add(addUpdateTextBox);
            TaskDetailsGrid.Children.Add(addUpdateButton);

            addUpdateTextBox.Focus();
            TaskDetailsScrollViewer.ScrollToBottom();
        }

        private void InitUpdateRows()
        {
            selectedTaskID = DataHandling.GetTaskID(selectedTask.taskName, selectedTask.details);
            noOfUpdates = DataHandling.GetTaskUpdates(selectedTaskID);

            for (int i = 0; i < noOfUpdates; i++)
            {
                RowDefinition updateRow = new RowDefinition();
                updateRow.Height = new GridLength(100, GridUnitType.Auto);
                TaskDetailsGrid.RowDefinitions.Add(updateRow);

                string updateHeaderString = DataStructures.updateRows[i].updatedBy + " at " + DataStructures.updateRows[i].updateTimeDate.ToString("HH:mm dd/MM/yyyy")
                    + ": ";
                string updateContentString = DataStructures.updateRows[i].updateDetails;

                TextBlock updateHeaderTextBlock = new TextBlock();
                updateHeaderTextBlock.Text = updateHeaderString;
                updateHeaderTextBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
                updateHeaderTextBlock.Margin = new Thickness(0, 10, 0, 10);
                updateHeaderTextBlock.FontSize = 12;

                TextBlock updateContentTextBlock = new TextBlock();
                updateContentTextBlock.Text = updateContentString;
                updateContentTextBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
                updateContentTextBlock.Margin = new Thickness(20, 30, 20, 10);
                updateContentTextBlock.FontSize = 14;
                updateContentTextBlock.TextWrapping = TextWrapping.Wrap;

                Rectangle divider = new Rectangle();
                divider.Width = 470;
                divider.Height = 1;
                divider.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
                divider.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
                divider.Margin = new Thickness(20, 0, 0, 0);
                divider.VerticalAlignment = VerticalAlignment.Top;
                divider.HorizontalAlignment = HorizontalAlignment.Left;

                Grid.SetRow(divider, i + 1);
                Grid.SetRow(updateHeaderTextBlock, i + 1);
                Grid.SetRow(updateContentTextBlock, i + 1);
                TaskDetailsGrid.Children.Add(divider);
                TaskDetailsGrid.Children.Add(updateHeaderTextBlock);
                TaskDetailsGrid.Children.Add(updateContentTextBlock);
            }
        }

        public void EditTaskButtonClick(object sender, RoutedEventArgs e)
        {
            EditTaskWindow editTaskWindow = new EditTaskWindow(selectedTask);
            editTaskWindow.Show();
            Close();
        }

        public void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DataHandling.GetTasksFull();
            Close();
        }

        private void TaskDetailsGrid_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void AddUpdateTextChanged(object sender, TextChangedEventArgs e)
        {
            updateText = addUpdateTextBox.Text;
        }

        private void AddUpdateKeyPressed (object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    int ci = addUpdateTextBox.CaretIndex;
                    addUpdateTextBox.Text += Environment.NewLine;
                    addUpdateTextBox.CaretIndex = ci + 1;
                } else
                {
                    if (updateText != null)
                    {
                        DataHandling.UpdateTask(selectedTask);
                        DataHandling.AddTaskUpdate(selectedTaskID, addUpdateTextBox.Text);

                        DataStructures.NotificationStruct notificationStruct = new DataStructures.NotificationStruct();
                        notificationStruct.notificationSender = Application.Current.Properties["username"].ToString();
                        notificationStruct.notificationText = "[" + selectedTask.taskName + "]: " + addUpdateTextBox.Text;
                        notificationStruct.taskID = selectedTaskID;
                        notificationStruct.notificationTime = DateTime.Now;
                        notificationStruct.notificationRecipients = selectedTask.notifyUsers;

                        DataHandling.AddNotification(notificationStruct);

                        DataHandling.GetTasksFull();

                        int taskIndex = 0;

                        foreach (DataStructures.TaskRowStruct task in DataStructures.taskRows)
                        {
                            if (task.taskName.Equals(selectedTask.taskName))
                            {
                                taskIndex = DataStructures.taskRows.IndexOf(task);
                            }
                        }

                        ViewTaskWindow updatedTask = new ViewTaskWindow(DataStructures.taskRows[taskIndex]);
                        updatedTask.Show();
                        Close();
                    }
                }
            }
        }

        private void AddUpdateButtonClick (object sender, RoutedEventArgs e)
        {
            if (updateText != null)
            {
                DataHandling.UpdateTask(selectedTask);
                DataHandling.AddTaskUpdate(selectedTaskID, addUpdateTextBox.Text);

                DataStructures.NotificationStruct notificationStruct = new DataStructures.NotificationStruct();
                notificationStruct.notificationSender = Application.Current.Properties["username"].ToString();
                notificationStruct.notificationText = "[" + selectedTask.taskName + "]: " + addUpdateTextBox.Text;
                notificationStruct.taskID = selectedTaskID;
                notificationStruct.notificationTime = DateTime.Now;
                notificationStruct.notificationRecipients = selectedTask.notifyUsers;

                DataHandling.AddNotification(notificationStruct);

                DataHandling.GetTasksFull();

                int taskIndex = 0;

                foreach (DataStructures.TaskRowStruct task in DataStructures.taskRows)
                {
                    if (task.taskName.Equals(selectedTask.taskName))
                    {
                        taskIndex = DataStructures.taskRows.IndexOf(task);
                    }
                }

                ViewTaskWindow updatedTask = new ViewTaskWindow(DataStructures.taskRows[taskIndex]);
                updatedTask.Show();
                Close();
            }
        }

        private void ViewFilesButtonClick (object sender, RoutedEventArgs e)
        {
            ProcessStartInfo openTaskDirectory = new ProcessStartInfo("explorer.exe", Properties.Settings.Default.FileDirectory + selectedTask.taskListFileTableDir + "\\");
            Process.Start(openTaskDirectory);
        }

        private void AddFilesButtonClick (object sender, RoutedEventArgs e)
        {
            string taskFilesUpdateString = DataHandling.AddTaskFiles(selectedTask);

            if (selectedTask.taskFiles == null)
            {
                selectedTask.taskFiles = new List<string>();
            }

            string[] taskFilesArray = selectedTask.taskFiles.ToArray();
            string taskFiles = String.Join(",", taskFilesArray);

            if (taskFilesUpdateString.Equals("Added File(s): ") == false)
            {
                DataHandling.AddTaskUpdate(selectedTaskID, taskFilesUpdateString);

                DataHandling.GetTasksFull();

                ViewTaskWindow updatedTask = new ViewTaskWindow(selectedTask);
                updatedTask.Show();
                Close();
            }
        }
    }
}
