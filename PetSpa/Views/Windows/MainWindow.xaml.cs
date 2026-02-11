using PetSpa.AppData;
using PetSpa.Views.Pages;
using System.Windows;

namespace PetSpa.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ClassFrame.FramePanel = FramePanelMain;
            FramePanelMain.Navigate(new Views.Pages.PanelPage());

            ClassFrame.FrameBody = FrameBodyMain;
            FrameBodyMain.Navigate(new Views.Pages.ClientPage());
        }

        public void RefreshPricePage()
        {
            // Если на данный момент открыта страница прайсов, обновляем ее
            if (FrameBodyMain.Content is PricePage pricePage)
            {
                pricePage.RefreshData();
            }
        }
    }
}
