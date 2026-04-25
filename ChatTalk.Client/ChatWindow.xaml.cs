using ChatTalk.Client.Commads;
using ChatTalk.Client.Command;
using ChatTalk.Client.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatTalk.Client
{
    public partial class ChatWindow : Window
    {
        private readonly Client _client;
        private readonly MainWindow _mainWindow;
        private string[] _connectedUsers = Array.Empty<string>();
        private bool _isClosing = false;

        public ChatWindow(Client client, MainWindow mainWindow)
        {
            InitializeComponent();
            _client = client;
            _mainWindow = mainWindow;
            _client.MessageReceived += OnMessageReceived;
            _client.UserListReceived += OnUserListReceived;

            _ = _client.ReceiveMsgAsync();
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
            this.Close();
        }

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isClosing) return;
            e.Cancel = true;
            _isClosing = true;
            await _client.Disconnect();

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
        private void AddMyMessage(ChatMessage chatMessage)
        {
            StackPanel messageContainer = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 12),
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxWidth = 260
            };

            TextBlock? senderText = null;

            Border bubble = new Border
            {
                Background = (Brush)FindResource("PrimaryBrush"),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(12, 10, 12, 10)
            };

            TextBlock textBlock = new TextBlock
            {
                Text = chatMessage.Content,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            /* 26.04.23 - 귓속말 UI 추가 START */
            if (chatMessage.IsWhisper)
            {
                senderText = new TextBlock
                {
                    Text = "[귓속말]",
                    FontSize = 11,
                    Margin = new Thickness(8, 0, 0, 4),
                    Foreground = (Brush)FindResource("SubTextBrush")
                };
                textBlock.Text = chatMessage.Content;
                bubble.Background = (Brush)FindResource("MyWhisperMessageBrush");
                bubble.BorderBrush = (Brush)FindResource("WhisperBorderBrush");
                bubble.BorderThickness = new Thickness(1);
                messageContainer.Children.Add(senderText);
            }
            /* 26.04.23 - 귓속말 UI 추가 END  */
            
            bubble.Child = textBlock;
            messageContainer.Children.Add(bubble);
            ChatPanel.Children.Add(messageContainer);

            ChatScrollViewer.ScrollToEnd();
        }

        private void AddOtherMessage(ChatMessage chatMessage)
        {
            
            StackPanel messageContainer = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 12),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 260
            };

            TextBlock senderText = new TextBlock
            {
                Text = chatMessage.SenderName,
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
                Text = chatMessage.Content,
                Foreground = (Brush)FindResource("TextBrush"),
                TextWrapping = TextWrapping.Wrap
            };

            /* 26.04.23 - 귓속말 UI 추가 START */
            if (chatMessage.IsWhisper)
            {
                senderText.Text = $"[귓속말] {chatMessage.SenderName}";
                senderText.Foreground = (Brush)FindResource("WhisperLabelBrush");

                bubble.Background = (Brush)FindResource("WhisperMessageBrush");
                bubble.BorderBrush = (Brush)FindResource("WhisperBorderBrush");
                bubble.BorderThickness = new Thickness(1);

                messageText.Foreground = (Brush)FindResource("WhisperTextBrush");
            }
            /* 26.04.23 - 귓속말 UI 추가 END   */

            bubble.Child = messageText;

            messageContainer.Children.Add(senderText);
            messageContainer.Children.Add(bubble);

            ChatPanel.Children.Add(messageContainer);

            ChatScrollViewer.ScrollToEnd();
        }

        private async Task SendMessageAsync()
        {
            string message = MessageTextBox.Text.Trim();
            ChatMessage chatMessage;
            ChatCommand chatCommand;

            if (string.IsNullOrEmpty(message)) return;

            try
            {
                chatCommand = ChatCommandParser.Parse(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if(chatCommand.Type == MessageType.Whisper)
            {
                if(string.IsNullOrEmpty(chatCommand.ToUserName))
                {
                    MessageBox.Show("귓속말 대상 사용자가 없습니다.");
                    return;
                }

                if (!_connectedUsers.Contains(chatCommand.ToUserName))
                {
                    MessageBox.Show("존재하지 않는 사용자입니다.");
                    return;
                }

                chatMessage = chatCommand.ToChatMessage(_client.UserName);

                await _client.SendWhisperMsgAsync(chatMessage.SenderName, chatMessage.Content);
            }
            else
            {
                chatMessage = chatCommand.ToChatMessage(_client.UserName);
                await _client.SendChatMsgAsync(chatMessage.Content);
            }

            this.AddMyMessage(chatMessage);
            MessageTextBox.Clear();
            MessageTextBox.Focus();
        }

        private void OnMessageReceived(ChatMessage chatMessage)
        {
            Dispatcher.Invoke(() =>
            {
                AddOtherMessage(chatMessage);
            });
        }

        private void OnUserListReceived(string[] users, string count)
        {
            ConnectedUserCountTextBlock.Text = count;
            _connectedUsers = users;
        }
    }
}
