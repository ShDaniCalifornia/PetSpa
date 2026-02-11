using PetSpa.ViewModels;
using PetSpa.Views.Pages;
using System.Windows;

namespace PetSpa.Views.Windows
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            LoadCurrentUserData();
            this.Deactivated += ProfileWindow_Deactivated;
        }

        private void LoadCurrentUserData()
        {
            try
            {
                if (AuthorizationPage.currentUser != null)
                {
                    // Создаем ViewModel с текущим пользователем
                    var viewModel = new ProfileViewModel(AuthorizationPage.currentUser);
                    this.DataContext = viewModel;
                }
                else
                {
                    // Тестовые данные, если пользователь не авторизован
                    var testUser = new Model.Users
                    {
                        full_name = "Схведиани Дарья Евгеньевна",
                        phone = "8 (904) 361 65 75",
                        email = "darya.skhvediani@yandex.ru",
                        Role = new Model.Role { role1 = "Администратор" }
                    };

                    var viewModel = new ProfileViewModel(testUser);
                    this.DataContext = viewModel;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке профиля: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProfileWindow_Deactivated(object sender, System.EventArgs e)
        {
            // Закрываем окно при потере фокуса
            this.Close();
        }
    }
}