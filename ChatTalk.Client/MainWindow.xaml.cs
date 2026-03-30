using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            ChatWindow chatWindow = new ChatWindow(_client);
            chatWindow.Owner = this;
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
                await _client.ConenectAsync(ip, port);
                await _client.SendAsync($"^||^ID^||^{UserNameTextBox.Text}\n");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 연결 실패 : {ex.Message}");
                return false;
            }
        }
    }
}