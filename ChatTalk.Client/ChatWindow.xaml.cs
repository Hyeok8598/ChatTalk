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
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private readonly Client _client;

        public ChatWindow(Client client)
        {
            InitializeComponent();
            _client = client;
        }


        /* ===================================================================== *
            1. Event
         * ===================================================================== */
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(Object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner is MainWindow mainWindow)
            {
                _client.Disconnect();
                mainWindow.StatusTextBlock.Text = "[연결대기]";
                mainWindow.StatusTextBlock.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#F87171")
                );
                mainWindow.ConnectButton.IsEnabled = true;
                mainWindow.Show();
            }

            this.Close();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MessageTextBox.Text)) return;
        }
    }
}
