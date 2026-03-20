using PetSpa.ViewModels;
using PetSpa.Views.Pages;
using System.Windows;

namespace PetSpa.Views.Windows
{
    public partial class ProfileWindow : Window
    {
        public bool IsAdmin => AuthorizationPage.currentUser?.id_role == 1;

        public ProfileWindow()
        {
            InitializeComponent();

            // Устанавливаем DataContext для отображения информации о пользователе
            if (AuthorizationPage.currentUser != null)
            {
                var viewModel = new ProfileViewModel(AuthorizationPage.currentUser);
                this.DataContext = viewModel;
            }

            this.Deactivated += ProfileWindow_Deactivated;
        }

        private void ProfileWindow_Deactivated(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}