using PetSpa.AppData;
using System.Windows;

namespace PetSpa.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegAuthWindow.xaml
    /// </summary>
    public partial class RegAuthWindow : Window
    {
        public RegAuthWindow()
        {
            InitializeComponent();

            ClassFrame.FramePanel = FramePanel;
            FramePanel.Navigate(new Views.Pages.InfoPanelPage());

            ClassFrame.FrameBody = FrameBody;
            FrameBody.Navigate(new Views.Pages.RegistrationPage());
        }
    }
}