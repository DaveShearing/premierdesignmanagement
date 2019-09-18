using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows;
using System.Threading;

namespace PremierDesignManagement
{
    class DataHandling
    {
        //Loads users from database into app
        public static void GetUsers()
        {
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                SqlCommand getUsers = new SqlCommand("SELECT Username, Forename, Surname FROM dbo.Users;", sqlConn);
                sqlConn.Open();

                SqlDataReader reader = getUsers.ExecuteReader();

                Properties.Settings.Default.UsernamesStringCollection.Clear();
                Properties.Settings.Default.UsersStringCollection.Clear();


                while (reader.Read())
                {
                    string username = reader.GetString(0);
                    string name = reader.GetString(1) + " " + reader.GetString(2);
                    Properties.Settings.Default.UsernamesStringCollection.Add(username);
                    Properties.Settings.Default.UsersStringCollection.Add(name);
                }

                reader.Close();
                sqlConn.Close();
            }
        }

        //Loads tasks from database into app (only some details)
        public static void GetTasks()
        {
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                SqlCommand getTasks = new SqlCommand("SELECT TaskName, StartDate, Deadline, AssignedTo, TaskStatus, LastEdited FROM dbo.Tasks", sqlConn);

                sqlConn.Open();

                

                SqlDataReader reader = getTasks.ExecuteReader();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).TaskList2.ItemsSource = null;
                    }
                }

                DataStructures.taskRows.Clear();

                while (reader.Read())
                {
                    DataStructures.TaskRowStruct taskRow = new DataStructures.TaskRowStruct();
                    taskRow.taskName = reader.GetString(0);
                    taskRow.startDate = reader.GetDateTime(1);
                    taskRow.deadline = reader.GetDateTime(2);
                    taskRow.assignedTo = (string)reader.GetString(3);
                    taskRow.taskStatus = (string)reader.GetString(4);
                    taskRow.lastEdited = reader.GetDateTime(5);

                    DataStructures.taskRows.Add(taskRow);
                }

                reader.Close();
                sqlConn.Close();
            }

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).TaskList2.ItemsSource = DataStructures.taskRows;

                }
            }
        }

        //Loads tasks from database into app (all details)
        public static void GetTasksFull()
        {
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                SqlCommand getTasks = new SqlCommand("SELECT TaskName, StartDate, Deadline, Details, TaskListFileTableDir, AssignedBy, AssignedTo, TaskStatus, " +
                    "LastEdited, LastEditedBy FROM dbo.Tasks", sqlConn);

                sqlConn.Open();

                SqlDataReader reader = getTasks.ExecuteReader();
                
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).TaskList2.ItemsSource = null;
                    }
                }
                
                DataStructures.taskRows.Clear();

                while (reader.Read())
                {
                    DataStructures.TaskRowStruct taskRow = new DataStructures.TaskRowStruct();
                    taskRow.taskName = reader.GetString(0);
                    taskRow.startDate = reader.GetDateTime(1);
                    taskRow.deadline = reader.GetDateTime(2);
                    taskRow.details = reader.GetString(3);
                    taskRow.taskListFileTableDir = reader.GetString(4);
                    taskRow.assignedBy = reader.GetString(5);
                    taskRow.assignedTo = (string)reader.GetString(6);
                    taskRow.taskStatus = (string)reader.GetString(7);
                    taskRow.lastEdited = reader.GetDateTime(8);
                    taskRow.lastEditedBy = reader.GetString(9);

                    DataStructures.taskRows.Add(taskRow);
                }

                reader.Close();
                sqlConn.Close();
            }

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).TaskList2.ItemsSource = DataStructures.taskRows;
                    
                }
            }
        }

        //Loads tasks from database into app (all details)
        public static void GetTasksFullThread(MainWindow mainWindow)
        {
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                SqlCommand getTasks = new SqlCommand("SELECT TaskName, StartDate, Deadline, Details, TaskListFileTableDir, AssignedBy, AssignedTo, TaskStatus FROM dbo.Tasks", sqlConn);

                sqlConn.Open();

                SqlDataReader reader = getTasks.ExecuteReader();

                lock (mainWindow.TaskList2)
                mainWindow.TaskList2.ItemsSource = null;

                /*
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).TaskList2.ItemsSource = null;
                    }
                }
                */

                DataStructures.taskRows.Clear();

                while (reader.Read())
                {
                    DataStructures.TaskRowStruct taskRow = new DataStructures.TaskRowStruct();
                    taskRow.taskName = reader.GetString(0);
                    taskRow.startDate = reader.GetDateTime(1);
                    taskRow.deadline = reader.GetDateTime(2);
                    taskRow.details = reader.GetString(3);
                    taskRow.taskListFileTableDir = reader.GetString(4);
                    taskRow.assignedBy = reader.GetString(5);
                    taskRow.assignedTo = (string)reader.GetString(6);
                    taskRow.taskStatus = (string)reader.GetString(7);

                    DataStructures.taskRows.Add(taskRow);
                }

                reader.Close();
                sqlConn.Close();
            }

            mainWindow.TaskList2.ItemsSource = DataStructures.taskRows;
            /*
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).TaskList2.ItemsSource = DataStructures.taskRows;

                }
            }
            */
        }

        public static int GetTaskID(string taskName, string taskDetails)
        {
            int taskID;
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                SqlCommand getTaskIDComm = new SqlCommand("GetTaskIDSP", sqlConn);
                getTaskIDComm.CommandType = CommandType.StoredProcedure;
                getTaskIDComm.Parameters.AddWithValue("@taskName", taskName);
                getTaskIDComm.Parameters.AddWithValue("@details", taskDetails);
                SqlParameter taskIDSQL = new SqlParameter("@taskID", SqlDbType.Int);
                taskIDSQL.Direction = ParameterDirection.Output;
                getTaskIDComm.Parameters.Add(taskIDSQL);

                sqlConn.Open();
                int i = getTaskIDComm.ExecuteNonQuery();
                taskID = (int)taskIDSQL.Value;
            }

                return taskID;
        }

        public static void UpdateTask(int taskID, string taskName, DateTime startDate, DateTime deadline, string details, string taskListFileTableDir, string assignedBy,
            string assignedTo, string taskStatus)
        {
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                SqlCommand updateTaskComm = new SqlCommand("UpdateTaskSP", sqlConn);
                updateTaskComm.CommandType = CommandType.StoredProcedure;
                updateTaskComm.Parameters.AddWithValue("@taskID", taskID);
                updateTaskComm.Parameters.AddWithValue("@taskName", taskName);
                updateTaskComm.Parameters.AddWithValue("@startdate", startDate);
                updateTaskComm.Parameters.AddWithValue("@deadline", deadline);
                updateTaskComm.Parameters.AddWithValue("@details", details);
                updateTaskComm.Parameters.AddWithValue("@tasklistfiletabledir", taskListFileTableDir);
                updateTaskComm.Parameters.AddWithValue("@assignedby", assignedBy);
                updateTaskComm.Parameters.AddWithValue("@assignedto", assignedTo);
                updateTaskComm.Parameters.AddWithValue("@taskstatus", taskStatus);
                updateTaskComm.Parameters.AddWithValue("@lastedited", DateTime.Now);
                updateTaskComm.Parameters.AddWithValue("@lasteditedby", System.Windows.Application.Current.Properties["username"]);

                sqlConn.Open();
                int i = updateTaskComm.ExecuteNonQuery();
            }
        }

        public static void RefreshDataLoop(object data)
        {
            MainWindow mainWindow = (MainWindow)data;

            while (mainWindow.Dispatcher.Thread.IsAlive == true)
            {
                DataHandling.GetTasksFullThread(mainWindow);
                Console.WriteLine("Tasks Updated");
                Thread.Sleep(1000);
                
            }

            
        }
    }
}
