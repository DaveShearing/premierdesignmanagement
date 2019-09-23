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
            
        }

        
        public static List<TaskRowStruct> taskRows = new List<TaskRowStruct>();
        public static List<TaskRowStruct> assignedToTaskRows = new List<TaskRowStruct>();
        public static List<TaskRowStruct> assignedByTaskRows = new List<TaskRowStruct>();

        public class UpdateRowStruct
        {
            public int updateID { get; set; }
            public string updatedBy { get; set; }
            public DateTime updateTimeDate { get; set; }
            public string updateDetails { get; set; }

        }

        public static List<UpdateRowStruct> updateRows = new List<UpdateRowStruct>();

    }
}
