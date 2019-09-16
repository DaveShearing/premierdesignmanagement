using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Windows.Data;

namespace PremierDesignManagement
{
    class DataHandling
    {
        //Loads users from database into app
        public static void getUsers()
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
        public static void getTasks()
        {
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                SqlCommand getTasks = new SqlCommand("SELECT TaskName, StartDate, Deadline, AssignedTo, TaskStatus FROM dbo.Tasks", sqlConn);

                sqlConn.Open();

                SqlDataReader reader = getTasks.ExecuteReader();

                DataStructures.taskRows.Clear();
                //Properties.Settings.Default.taskRows.Clear();

                while (reader.Read())
                {
                    DataStructures.TaskRowStruct taskRow = new DataStructures.TaskRowStruct();
                    taskRow.taskName = reader.GetString(0);
                    taskRow.startDate = reader.GetDateTime(1);
                    taskRow.deadline = reader.GetDateTime(2);
                    taskRow.assignedTo = (string)reader.GetString(3);
                    taskRow.taskStatus = (string)reader.GetString(4);

                    DataStructures.taskRows.Add(taskRow);
                    //Properties.Settings.Default.taskRows.Add(taskRow);
                }

                reader.Close();
                sqlConn.Close();
            }
        }

        //Loads tasks from database into app (all details)
        public static void getTasksFull()
        {
            using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.PDMDatabaseConnectionString))
            {
                SqlCommand getTasks = new SqlCommand("SELECT TaskName, StartDate, Deadline, Details, TaskListFileTableDir, AssignedBy, AssignedTo, TaskStatus FROM dbo.Tasks", sqlConn);

                sqlConn.Open();

                SqlDataReader reader = getTasks.ExecuteReader();

                DataStructures.taskRows.Clear();
                //Properties.Settings.Default.taskRows.Clear();

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
                    //Properties.Settings.Default.taskRows.Add(taskRow);
                }

                reader.Close();
                sqlConn.Close();
            }
        }

    }
}
