using PetSpa.AppData;
using PetSpa.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    public partial class PanelPage : Page
    {
        public static readonly DependencyProperty ActiveButtonProperty =
            DependencyProperty.Register("ActiveButton", typeof(Button), typeof(PanelPage),
                new PropertyMetadata(null, OnActiveButtonChanged));

        public Button ActiveButton
        {
            get => (Button)GetValue(ActiveButtonProperty);
            set => SetValue(ActiveButtonProperty, value);
        }

        private static void OnActiveButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as PanelPage;
            if (page == null) return;

            page.ResetAllButtons();

            if (e.NewValue is Button activeButton)
            {
                activeButton.FontSize = 33;
                activeButton.Foreground = new SolidColorBrush(Colors.White);
                activeButton.FontWeight = FontWeights.Bold;
            }
        }

        private void ResetAllButtons()
        {
            ClientBtn.FontSize = 30;
            ClientBtn.Foreground = Brushes.White;
            ClientBtn.FontWeight = FontWeights.Normal;

            RecordsBtn.FontSize = 30;
            RecordsBtn.Foreground = Brushes.White;
            RecordsBtn.FontWeight = FontWeights.Normal;

            MyAppointmentsBtn.FontSize = 30;
            MyAppointmentsBtn.Foreground = Brushes.White;
            MyAppointmentsBtn.FontWeight = FontWeights.Normal;

            MastersBtn.FontSize = 30;
            MastersBtn.Foreground = Brushes.White;
            MastersBtn.FontWeight = FontWeights.Normal;

            PriceBtn.FontSize = 30;
            PriceBtn.Foreground = Brushes.White;
            PriceBtn.FontWeight = FontWeights.Normal;

            HelpBtn.FontSize = 30;
            HelpBtn.Foreground = Brushes.White;
            HelpBtn.FontWeight = FontWeights.Normal;
        }

        public PanelPage()
        {
            InitializeComponent();
            SetupRoleBasedVisibility();

            // Загружаем начальную страницу после того, как страница полностью загрузится
            this.Loaded += PanelPage_Loaded;
        }

        private void PanelPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInitialPage();
        }

        private void SetupRoleBasedVisibility()
        {
            bool isAdmin = IsAdmin();

            if (isAdmin)
            {
                // Администратор: видит Клиенты, Записи, Мастера, Прайс, Помощь
                ClientBtn.Visibility = Visibility.Visible;
                RecordsBtn.Visibility = Visibility.Visible;
                MyAppointmentsBtn.Visibility = Visibility.Collapsed;
                MastersBtn.Visibility = Visibility.Visible;
                PriceBtn.Visibility = Visibility.Visible;
                HelpBtn.Visibility = Visibility.Visible;

                // Расположение для админа
                Grid.SetColumn(ClientBtn, 4);
                Grid.SetColumn(RecordsBtn, 6);
                Grid.SetColumn(MastersBtn, 8);
                Grid.SetColumn(PriceBtn, 10);
                Grid.SetColumn(HelpBtn, 12);
            }
            else
            {
                // Обычный пользователь: не видит Клиенты, вместо Записей — Мои записи
                ClientBtn.Visibility = Visibility.Collapsed;
                RecordsBtn.Visibility = Visibility.Collapsed;
                MyAppointmentsBtn.Visibility = Visibility.Visible;
                MastersBtn.Visibility = Visibility.Visible;
                PriceBtn.Visibility = Visibility.Visible;
                HelpBtn.Visibility = Visibility.Visible;

                // Расположение для обычного пользователя
                Grid.SetColumn(MyAppointmentsBtn, 4);
                Grid.SetColumn(MastersBtn, 6);
                Grid.SetColumn(PriceBtn, 8);
                Grid.SetColumn(HelpBtn, 10);
            }
        }

        private void LoadInitialPage()
        {
            bool isAdmin = IsAdmin();

            if (isAdmin)
            {
                // Администратор: открываем страницу "Клиенты"
                if (ClassFrame.FrameBody != null)
                {
                    ActiveButton = ClientBtn;
                    ClassFrame.FrameBody.Navigate(new ClientPage());
                }
            }
            else
            {
                // Обычный пользователь: открываем страницу "Мои записи"
                if (ClassFrame.FrameBody != null)
                {
                    ActiveButton = MyAppointmentsBtn;
                    ClassFrame.FrameBody.Navigate(new MyAppointmentsPage());
                }
            }
        }

        private bool IsAdmin()
        {
            var currentUser = AuthorizationPage.currentUser;
            return currentUser != null && currentUser.id_role == 1;
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
        }

        private void ClientBtn_Click(object sender, RoutedEventArgs e)
        {
            ActiveButton = ClientBtn;
            if (ClassFrame.FrameBody != null)
            {
                ClassFrame.FrameBody.Navigate(new ClientPage());
            }
        }

        private void RecordsBtn_Click(object sender, RoutedEventArgs e)
        {
            ActiveButton = RecordsBtn;
            if (ClassFrame.FrameBody != null)
            {
                ClassFrame.FrameBody.Navigate(new AppointmentPage());
            }
        }

        private void MyAppointmentsBtn_Click(object sender, RoutedEventArgs e)
        {
            ActiveButton = MyAppointmentsBtn;
            if (ClassFrame.FrameBody != null)
            {
                ClassFrame.FrameBody.Navigate(new MyAppointmentsPage());
            }
        }

        private void MastersBtn_Click(object sender, RoutedEventArgs e)
        {
            ActiveButton = MastersBtn;
            if (ClassFrame.FrameBody != null)
            {
                ClassFrame.FrameBody.Navigate(new MastersPage());
            }
        }

        private void PriceBtn_Click(object sender, RoutedEventArgs e)
        {
            ActiveButton = PriceBtn;
            if (ClassFrame.FrameBody != null)
            {
                ClassFrame.FrameBody.Navigate(new PricePage());
            }
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            ActiveButton = HelpBtn;
            if (ClassFrame.FrameBody != null)
            {
                ClassFrame.FrameBody.Navigate(new HelpPage());
            }
        }
    }
}