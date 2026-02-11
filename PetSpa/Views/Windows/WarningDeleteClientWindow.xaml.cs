using System.Windows;

namespace PetSpa.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WarningDeleteClientWindow.xaml
    /// </summary>
    public partial class WarningDeleteClientWindow : Window
    {
        public WarningDeleteClientWindow()
        {
            InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}