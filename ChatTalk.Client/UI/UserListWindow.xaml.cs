using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatTalk.Client
{
    public partial class UserListWindow : Window
    {
        public UserListWindow(string[] users)
        {
            InitializeComponent();
            UserListBox.ItemsSource = users;
            UserCountTextBlock.Text = $"{users.Length}명";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
