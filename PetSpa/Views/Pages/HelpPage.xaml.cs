using PetSpa.View.Windows;
using PetSpa.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetSpa.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для HelpPage.xaml
    /// </summary>
    public partial class HelpPage : Page
    {
        public HelpPage()
        {
            InitializeComponent();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            // Получаем главное окно
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {

                var authWindow = new RegAuthWindow();
                authWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                authWindow.Show();

                mainWindow.Close();
            }
        }
    }
}
