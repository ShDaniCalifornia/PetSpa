using PetSpa.AppData;
using PetSpa.Views.Pages;
using System.Windows;

namespace PetSpa.View.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ClassFrame.FramePanel = FramePanelMain;
            FramePanelMain.Navigate(new PanelPage());

            ClassFrame.FrameBody = FrameBodyMain;

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var panelPage = FramePanelMain.Content as PanelPage;
            if (panelPage != null) { }
            else
            {
                var currentUser = AuthorizationPage.currentUser;
                if (currentUser != null && currentUser.id_role == 1)
                {
                    FrameBodyMain.Navigate(new ClientPage());
                }
                else
                {
                    FrameBodyMain.Navigate(new MyAppointmentsPage());
                }
            }
        }

        public void RefreshPricePage()
        {
            if (FrameBodyMain.Content is PricePage pricePage)
            {
                pricePage.RefreshData();
            }
        }
    }
}