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
        public MainWindow()
        {
            InitializeComponent();
        }


        /* ===================================================================== *
            1. Event
         * ===================================================================== */
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string serverIP = ServerIpTextBox.Text;
            string port = PortTextBox.Text;
            string userName = UserNameTextBox.Text;

            if(!validateInputText()) return;

            StatusTextBlock.Text = $"[접속 시도 중] {serverIP} : {port}";
            StatusTextBlock.Foreground = Brushes.Orange;

            ChatWindow chatWindow = new ChatWindow();
            chatWindow.Owner = this;
            chatWindow.Show();
            
            this.Hide();
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
        private bool validateInputText()
        {
            if (string.IsNullOrWhiteSpace(ServerIpTextBox.Text) ||
                string.IsNullOrWhiteSpace(PortTextBox.Text)     ||
                string.IsNullOrWhiteSpace(UserNameTextBox.Text))
            {
                StatusTextBlock.Text = "입력값을 확인하세요.";
                StatusTextBlock.Foreground = Brushes.Red;
                return false;
            }

            return true;
        }
    }
}