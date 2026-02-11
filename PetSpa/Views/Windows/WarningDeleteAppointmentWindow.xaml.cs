using System.Windows;

namespace PetSpa.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WarningDeleteAppointmentWindow.xaml
    /// </summary>
    public partial class WarningDeleteAppointmentWindow : Window
    {
        public WarningDeleteAppointmentWindow()
        {
            InitializeComponent();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
