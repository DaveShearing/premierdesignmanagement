﻿using System;
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
        }

        public void CancelButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
