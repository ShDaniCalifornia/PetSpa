using PetSpa.AppData;
using PetSpa.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для PanelPage.xaml
    /// </summary>
    public partial class PanelPage : Page
    {
        // Свойство для активной кнопки
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

            // Сбрасываем все кнопки к обычному размеру
            page.ResetAllButtons();

            // Устанавливаем увеличенный размер для активной кнопки
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

            ActiveButton = ClientBtn;
        }

        private void ProfileBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
        }

        private void ClientBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActiveButton = ClientBtn;

            ClassFrame.FrameBody.Navigate(new Views.Pages.ClientPage());
        }

        private void RecordsBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActiveButton = RecordsBtn;

            ClassFrame.FrameBody.Navigate(new Views.Pages.AppointmentPage());
        }

        private void MastersBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActiveButton = MastersBtn;

            ClassFrame.FrameBody.Navigate(new Views.Pages.MastersPage());
        }

        private void PriceBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActiveButton = PriceBtn;

            ClassFrame.FrameBody.Navigate(new Views.Pages.PricePage());
        }

        private void HelpBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActiveButton = HelpBtn;

            ClassFrame.FrameBody.Navigate(new Views.Pages.HelpPage());
        }
    }
}
