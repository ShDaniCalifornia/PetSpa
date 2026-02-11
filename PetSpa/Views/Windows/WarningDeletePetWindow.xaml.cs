using System.Windows;

namespace PetSpa.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WarningDeletePetWindow.xaml
    /// </summary>
    public partial class WarningDeletePetWindow : Window
    {
        public WarningDeletePetWindow()
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
