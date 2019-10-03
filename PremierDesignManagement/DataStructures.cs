using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;


namespace PremierDesignManagement
{
    
    public class DataStructures
    {
        
        public class TaskRowStruct
        {
            public string taskName{ get; set; }
            public DateTime startDate { get; set; }
            public DateTime deadline { get; set; }
            public string details { get; set; }
            public string taskListFileTableDir { get; set; }
            public string assignedBy { get; set; }
            public string assignedTo { get; set; }
            public string taskStatus { get; set; }
            public DateTime lastEdited { get; set; }
            public string lastEditedBy { get; set; }
            public List<string> taskFiles { get; set; }

            public List<string> notifyUsers { get; set; }
            
        }

        
        public static List<TaskRowStruct> taskRows = new List<TaskRowStruct>();
        public static List<TaskRowStruct> assignedToTaskRows = new List<TaskRowStruct>();
        public static List<TaskRowStruct> assignedByTaskRows = new List<TaskRowStruct>();
        public static List<TaskRowStruct> filteredTaskRows = new List<TaskRowStruct>();

        public static List<string> taskNames = new List<string>();

        public class UpdateRowStruct
        {
            public int updateID { get; set; }
            public string updatedBy { get; set; }
            public DateTime updateTimeDate { get; set; }
            public string updateDetails { get; set; }

        }

        public static List<UpdateRowStruct> updateRows = new List<UpdateRowStruct>();

        public class NotificationStruct
        {
            public int notificationID { get; set; }

            public string notificationText { get; set; }

            public DateTime notificationTime { get; set; }

            public string notificationSender { get; set; }

            public List<string> notificationRecipients { get; set; }

            public int taskID { get; set; }

            public List<string> readByRecipients { get; set; }

            public List<string> hiddenByRecipients { get; set; }
        }

        public static List<NotificationStruct> notificationRows = new List<NotificationStruct>();

    }
}
