using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace PremierDesignManagement
{
    class DataStructures
    {
        public class TaskRowStruct
        {
            public string taskName;
            public DateTime startDate;
            public DateTime deadline;
            public string details;
            public string taskListFileTableDir;
            public string assignedBy;
            public string assignedTo;
            public string taskStatus;
        }

    }
}
