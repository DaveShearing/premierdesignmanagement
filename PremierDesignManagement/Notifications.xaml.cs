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
        public Notifications()
        {
            InitializeComponent();

            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)10;

            InitNotificationRows();


        }

        public void InitNotificationRows()
        {

            int noOfNotifications = DataStructures.notificationRows.Count();

            foreach (DataStructures.NotificationStruct notification in DataStructures.notificationRows)
            {
                string notificationHeaderString = notification.notificationSender + " at " + notification.notificationTime.ToString("HH:mm dd/MM/yyyy") + ": ";
                string notificationContentString = notification.notificationText;

                RowDefinition notificationRow = new RowDefinition();
                notificationRow.Height = new GridLength(100, GridUnitType.Auto);
                NotificationsGrid.RowDefinitions.Add(notificationRow);

                Border notificationBorder = new Border();
                notificationBorder.Width = 380;
                


                Grid notificationTextGrid = new Grid();
                
                
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

                if (notification.readByRecipients.Contains(Application.Current.Properties["username"]) != true)
                {
                    notificationBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF4F5A");
                    notificationHeaderTextBlock.FontWeight = FontWeights.Bold;
                    notificationContentTextBlock.FontWeight = FontWeights.Bold;
                    notificationBorder.BorderThickness = new Thickness(2);
                } else
                {
                    notificationBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF51545D");
                    notificationHeaderTextBlock.FontWeight = FontWeights.Normal;
                    notificationContentTextBlock.FontWeight = FontWeights.Normal;
                    notificationBorder.BorderThickness = new Thickness(1);
                }

                Grid.SetRow(notificationHeaderTextBlock, 0);
                Grid.SetRow(notificationContentTextBlock, 1);
                notificationBorder.Child = notificationTextGrid;
                notificationTextGrid.Children.Add(notificationHeaderTextBlock);
                notificationTextGrid.Children.Add(notificationContentTextBlock);

                Grid.SetRow(notificationBorder, noOfNotifications - 1);
                NotificationsGrid.Children.Add(notificationBorder);

                noOfNotifications--;
            }

            
        }

        public void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Resources["BlurEffectRadius"] = (double)0;

            Close();
        }

    }
}
