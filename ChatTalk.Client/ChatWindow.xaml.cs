using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatTalk.Client
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private readonly Client _client;
        private readonly MainWindow _mainWindow;
        private string[] _connectedUsers = Array.Empty<string>();

        public ChatWindow(Client client, MainWindow mainWindow)
        {
            InitializeComponent();
            _client = client;
            _mainWindow = mainWindow;
            _client.MessageReceived += OnMessageReceived;
            _client.UserListReceived += OnUserListReceived;

            _ = _client.ReceiveMsgAsync();
            //this.ReceiveMessageAsync();
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
            _client?.Disconnect();
            this.Close();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await this.SendMessageAsync();
        }

        private async void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await this.SendMessageAsync();
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowState == WindowState.Normal)
                    this.WindowState = WindowState.Maximized;
                else
                    this.WindowState = WindowState.Normal;
            }

            if(e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void ConnectedUserCountTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserListWindow userListWindow = new UserListWindow(_connectedUsers);
            userListWindow.Owner = this;
            userListWindow.ShowDialog();
        }

        /* ===================================================================== *
            2. 사용자 정의 함수
         * ===================================================================== */
        private void AddMyMessage(string message)
        {
            StackPanel messageContainer = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 12),
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxWidth = 260
            };

            Border bubble = new Border
            {
                Background = (Brush)FindResource("PrimaryBrush"),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(12, 10, 12, 10)
            };

            TextBlock textBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = textBlock;
            messageContainer.Children.Add(bubble);
            ChatPanel.Children.Add(messageContainer);

            ChatScrollViewer.ScrollToEnd();
        }

        private void AddOtherMessage(string sender, string message)
        {
            StackPanel messageContainer = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 12),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 260
            };

            TextBlock senderText = new TextBlock
            {
                Text = sender,
                FontSize = 11,
                Margin = new Thickness(8, 0, 0, 4),
                Foreground= (Brush)FindResource("SubTextBrush")
            };

            Border bubble = new Border
            {                
                Background = (Brush)FindResource("OtherMessageBrush"),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(12, 10, 12, 10)
            };

            TextBlock messageText = new TextBlock
            {
                Text = message,
                Foreground = (Brush)FindResource("TextBrush"),
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = messageText;

            messageContainer.Children.Add(senderText);
            messageContainer.Children.Add(bubble);

            ChatPanel.Children.Add(messageContainer);

            ChatScrollViewer.ScrollToEnd();
        }

        private async Task SendMessageAsync()
        {
            string message = MessageTextBox.Text.Trim();

            if (string.IsNullOrEmpty(message)) return;

            await _client.SendChatMsgAsync(message);

            this.AddMyMessage(message);
            MessageTextBox.Clear();
            MessageTextBox.Focus();
        }

        private void OnMessageReceived(string userName, string message)
        {
            Dispatcher.Invoke(() =>
            {
                AddOtherMessage(userName, message);
            });
        }

        private void OnUserListReceived(string[] users, string count)
        {
            ConnectedUserCountTextBlock.Text = count;
            _connectedUsers = users;
            Debug.WriteLine($"################################{string.Join(", ", _connectedUsers)}");
        }
    }
}
