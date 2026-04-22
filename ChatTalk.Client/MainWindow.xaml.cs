using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatTalk.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Client _client = new Client();

        public MainWindow()
        {
            InitializeComponent();
        }


        /* ===================================================================== *
            1. Event
         * ===================================================================== */
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string serverIP = ServerIpTextBox.Text;
            string portText = PortTextBox.Text;
            string userName = UserNameTextBox.Text;
            int? port = null;

            if (!ValidateInputText()) return;
            port = ValidatePort();
            if (port == null) return;

            StatusTextBlock.Text = $"[접속 시도 중] {serverIP} : {portText}";
            StatusTextBlock.Foreground = Brushes.Orange;
            ConnectButton.IsEnabled = false;

            if(!await Connect(serverIP, (int)port))
            {
                StatusTextBlock.Text = $"[연결 실패] {serverIP} : {portText}";
                StatusTextBlock.Foreground = Brushes.Orange;
                ConnectButton.IsEnabled = true;
                return;
            }

            ChatWindow chatWindow = new ChatWindow(_client, this);
            /* 
             * [Feature007] 26.04.05
             * Owner 사용하지 않고, ChatWindow에서 직접 받도록 변경
             * chatWindow.Owner = this; 
             */
            chatWindow.Closed += ChatWindow_Closed;
            chatWindow.Show();
            
            this.Hide();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /* ===================================================================== *
            2. 사용자정의 함수
         * ===================================================================== */
        private bool ValidateInputText()
        {
            string ip       = ServerIpTextBox.Text;
            string portText = PortTextBox.Text;
            string userId   = UserNameTextBox.Text;

            if(string.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("IP를 입력해주세요.");
                ServerIpTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(portText))
            {
                MessageBox.Show("Port를 입력해주세요.");
                PortTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                MessageBox.Show("User Name를 입력해주세요.");
                UserNameTextBox.Focus();
                return false;
            }

            if(userId.Contains("^||^"))
            {
                MessageBox.Show("UserName에는 \"^||^\" 포함된 문자는 사용할 수 없습니다.");
                UserNameTextBox.Focus();
                return false;
            }

            return true;
        }

        private int? ValidatePort()
        {
            if (!int.TryParse(PortTextBox.Text, out int port))
            {
                MessageBox.Show("Port는 숫자로 입력해주세요.");
                PortTextBox.Focus();
                return null;
            }

            return port;
        }

        private async Task<bool> Connect(string ip, int port)
        {
            try
            {
                await _client.ConnectAsync(ip, port);

                string userName = UserNameTextBox.Text;
                await _client.SendJoinMsgAsync(userName);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 연결 실패 : {ex.Message}");
                return false;
            }
        }

        private void ChatWindow_Closed(Object? sender, EventArgs e)
        {
            StatusTextBlock.Text = "[연결대기]";
            StatusTextBlock.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#F87171")
            );
            ConnectButton.IsEnabled = true;

            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }
    }
}