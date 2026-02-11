using System.Windows;

namespace PetSpa.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WarningForgetPasswordWindow.xaml
    /// </summary>
    public partial class WarningForgetPasswordWindow : Window
    {
        public WarningForgetPasswordWindow()
        {
            InitializeComponent();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
