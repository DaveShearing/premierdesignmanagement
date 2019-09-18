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
            
        }

        
        public static List<TaskRowStruct> taskRows = new List<TaskRowStruct>();
        
    }
}
