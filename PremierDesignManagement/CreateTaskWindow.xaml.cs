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
    public partial class CreateTaskWindow : Window
    {
        public CreateTaskWindow()
        {
            InitializeComponent();
            AssignToComboBox.ItemsSource = Properties.Settings.Default.UsersStringCollection;
            StatusComboBox.ItemsSource = Properties.Settings.Default.MainWorkflow;
        }

        private void CreateTaskButtonClick (object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                int assignedToUserIndex = Properties.Settings.Default.UsersStringCollection.IndexOf(AssignToComboBox.Text);
                string assignedUsername = Properties.Settings.Default.UsernamesStringCollection[assignedToUserIndex];

                SqlCommand createTask = new SqlCommand("CreateTaskSP", sqlConn);
                createTask.CommandType = CommandType.StoredProcedure;
                createTask.Parameters.AddWithValue("@taskname", TaskNameTextBox.Text);
                createTask.Parameters.AddWithValue("@startdate", StartDatePicker.SelectedDate);
                createTask.Parameters.AddWithValue("@deadline", DeadlinePicker.SelectedDate);
                createTask.Parameters.AddWithValue("@details", TaskDetailsTextBox.Text);
                createTask.Parameters.AddWithValue("@tasklistfiletabledir", "");
                createTask.Parameters.AddWithValue("@assignedby", System.Windows.Application.Current.Properties["username"]);
                createTask.Parameters.AddWithValue("@assignedto", assignedUsername);
                createTask.Parameters.AddWithValue("@taskstatus", ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString());

                sqlConn.Open();
                int i = createTask.ExecuteNonQuery();
            }

            

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
