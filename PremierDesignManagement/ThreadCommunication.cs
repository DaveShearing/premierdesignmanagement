using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows;

namespace PremierDesignManagement
{
    public class ThreadCommunication
    {
        private static PassTaskRows _update;
        private static Thread _thread;
        private static ISynchronizeInvoke _synch;
        private static object _object;
        public static List<DataStructures.TaskRowStruct> taskRowsTemp = new List<DataStructures.TaskRowStruct>();
        

        public delegate void PassTaskRows(List<DataStructures.TaskRowStruct> taskRowsTemp, int status);

        public ThreadCommunication()
        {
            _synch = null;
            _update = null;
        }

        public ThreadCommunication(ISynchronizeInvoke syn, PassTaskRows taskRowsTemp)
        {
            _synch = syn;
            _update = taskRowsTemp;
        }

        public void StartProcess()
        {
            _thread = new Thread(RefreshTaskListData);
            _thread.IsBackground = true;
            _thread.Name = "RefreshDataThread";
            _thread.Start();
        }

        private static void RefreshTaskListData()
        {

        }
    }
}
