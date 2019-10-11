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
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Globalization;

namespace PremierDesignManagement
{
    /// <summary>
    /// Interaction logic for Notifications.xaml
    /// </summary>
    public partial class Notifications : Window
    {
        int[,] taskIDArray;


        public Notifications()
        {
            InitializeComponent();

            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)10;

            InitNotificationRows();


        }

        public void InitNotificationRows()
        {

            int noOfNotifications = DataStructures.notificationRows.Count();
            taskIDArray = new int[noOfNotifications, 2];

            NotificationsGrid.Children.Clear();

            foreach (DataStructures.NotificationStruct notification in DataStructures.notificationRows)
            {
                

                string notificationHeaderString = notification.notificationSender + " at " + notification.notificationTime.ToString("HH:mm dd/MM/yyyy") + ": ";
                string notificationContentString = notification.notificationText;

                int[,] notifIDtaskID = new int[noOfNotifications,notification.taskID];
                taskIDArray[noOfNotifications-1, 0] = notification.taskID;
                taskIDArray[noOfNotifications - 1, 1] = notification.notificationID;

                RowDefinition notificationRow = new RowDefinition();
                notificationRow.Height = new GridLength(100, GridUnitType.Auto);
                
                NotificationsGrid.RowDefinitions.Add(notificationRow);

                Rectangle divider = new Rectangle();
                divider.Width = 320;
                divider.Height = 1;
                divider.HorizontalAlignment = HorizontalAlignment.Center;
                divider.VerticalAlignment = VerticalAlignment.Bottom;
                divider.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
                divider.Margin = new Thickness(0, 0, 0, 0);

                Border notificationBorder = new Border();
                notificationBorder.Width = 360;
                notificationBorder.Margin = new Thickness(2);
                
                Grid notificationTextGrid = new Grid();
                notificationTextGrid.MouseDown += NotificationTextGrid_MouseDown;
                
                
                TextBlock notificationHeaderTextBlock = new TextBlock();
                notificationHeaderTextBlock.Text = notificationHeaderString;
                notificationHeaderTextBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
                notificationHeaderTextBlock.Margin = new Thickness(5, 10, 0, 10);
                notificationHeaderTextBlock.FontSize = 12;

                TextBlock notificationContentTextBlock = new TextBlock();
                notificationContentTextBlock.Text = notificationContentString;
                notificationContentTextBlock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
                notificationContentTextBlock.Margin = new Thickness(20, 25, 10, 10);
                notificationContentTextBlock.FontSize = 14;
                notificationContentTextBlock.TextWrapping = TextWrapping.Wrap;

                bool readByUser = false;

                foreach (string username in notification.readByRecipients)
                {
                    if (username.Equals(Application.Current.Properties["username"].ToString()) == true)
                    {
                        readByUser = true;
                    }
                }

                if (readByUser != true)
                {
                    notificationBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
                    notificationHeaderTextBlock.FontWeight = FontWeights.Bold;
                    notificationContentTextBlock.FontWeight = FontWeights.Bold;
                    notificationBorder.BorderThickness = new Thickness(10,0,0,0);
                } else
                {
                    notificationBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
                    notificationHeaderTextBlock.FontWeight = FontWeights.Normal;
                    notificationContentTextBlock.FontWeight = FontWeights.Normal;
                    notificationBorder.BorderThickness = new Thickness(10,0,0,0);
                }

                
                Grid.SetRow(notificationHeaderTextBlock, 0);
                Grid.SetRow(notificationContentTextBlock, 1);
                Grid.SetRow(divider, 2);
                notificationBorder.Child = notificationTextGrid;
                
                notificationTextGrid.Children.Add(notificationHeaderTextBlock);
                notificationTextGrid.Children.Add(notificationContentTextBlock);
                notificationTextGrid.Children.Add(divider);

                Grid.SetRow(notificationBorder, noOfNotifications - 1);

                bool showForUser = false;

                foreach (string username in notification.notificationRecipients)
                {
                    if (username.Equals(Application.Current.Properties["username"].ToString()) == true)
                    {
                        showForUser = true;
                    } 
                }

                if (showForUser != false)
                {
                    NotificationsGrid.Children.Add(notificationBorder);
                }
                else
                {
                    Border border = new Border();
                    Grid.SetRow(border, noOfNotifications - 1);
                    NotificationsGrid.Children.Add(border);
                }


                noOfNotifications--;
            }

            
        }

        private void NotificationTextGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int notificationRow = -1;

            foreach (Border border in NotificationsGrid.Children)
            {
                if (((UIElement)e.Source).IsDescendantOf(border))
                {
                    notificationRow = Grid.GetRow(border);
                }
            }

            
            Console.WriteLine(notificationRow);

            for (int i = DataStructures.notificationRows.Count(); i >= 0; i--)
            {
                if (i == notificationRow)
                {
                    foreach (DataStructures.TaskRowStruct task in DataStructures.taskRows)
                    {
                        if (task.taskID == taskIDArray[i,0])
                        {
                            ViewTaskWindow viewTask = new ViewTaskWindow(task);
                            
                            DataHandling.UpdateNotificationReadBy(taskIDArray[i, 1]);
                            //taskIDArray = null;
                            NotificationsGrid.Children.Clear();

                            Close();
                            
                            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)0;

                            viewTask.Show();
                        }
                    }
                }
            }
        }

        public void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            taskIDArray = null;
            NotificationsGrid.Children.Clear();
            Close();
            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)0;
        }

        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            


        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
            {
                taskIDArray = null;
                NotificationsGrid.Children.Clear();
                System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)0;
                Close();

            }
        }
    }
}
