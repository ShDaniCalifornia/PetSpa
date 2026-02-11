using System.Windows;

namespace PetSpa.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для InstructionWindow.xaml
    /// </summary>
    public partial class InstructionWindow : Window
    {
        public InstructionWindow()
        {
            InitializeComponent();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
