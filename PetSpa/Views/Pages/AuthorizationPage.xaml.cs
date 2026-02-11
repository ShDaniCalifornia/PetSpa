using PetSpa.AppData;
using PetSpa.Model;
using PetSpa.View.Windows;
using PetSpa.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    public partial class AuthorizationPage : Page
    {
        public static Users currentUser;
        private bool isPasswordPlaceholder = true;
        private bool isPasswordVisible = false; // Добавляем флаг видимости пароля

        public AuthorizationPage()
        {
            InitializeComponent();
            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            if (RememberPassword.IsRememberEnabled())
            {
                CodeTb.Text = RememberPassword.GetSavedLogin();

                // Для сохраненного пароля
                string savedPassword = RememberPassword.GetSavedPassword();
                if (!string.IsNullOrEmpty(savedPassword))
                {
                    PasswordTb.Password = savedPassword;
                    PasswordVisibleTb.Text = savedPassword;
                    PasswordTb.Foreground = Brushes.White;
                    PasswordVisibleTb.Foreground = Brushes.White;
                    isPasswordPlaceholder = false;
                    RememberMeCb.IsChecked = true;

                    // Устанавливаем правильную иконку
                    UpdatePasswordEyeIcon();
                }
            }
        }

        private void UpdatePasswordEyeIcon()
        {
            if (isPasswordVisible)
            {
                PasswordEyeIcon.Source = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri("/Resources/Icons/EyeOpen.png", System.UriKind.Relative));
            }
            else
            {
                PasswordEyeIcon.Source = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri("/Resources/Icons/EyeClosed.png", System.UriKind.Relative));
            }
        }

        // Кнопка переключения видимости пароля
        private void TogglePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Показываем текст
                PasswordVisibleTb.Text = PasswordTb.Password;
                PasswordVisibleTb.Visibility = Visibility.Visible;
                PasswordTb.Visibility = Visibility.Collapsed;

                // Передаем фокус на видимое поле
                PasswordVisibleTb.Focus();
                PasswordVisibleTb.CaretIndex = PasswordVisibleTb.Text.Length;
            }
            else
            {
                // Показываем звездочки
                PasswordTb.Password = PasswordVisibleTb.Text;
                PasswordTb.Visibility = Visibility.Visible;
                PasswordVisibleTb.Visibility = Visibility.Collapsed;

                // Передаем фокус на поле пароля
                PasswordTb.Focus();
            }

            UpdatePasswordEyeIcon();
        }

        // Синхронизация пароля из видимого поля в скрытое
        private void PasswordVisibleTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordTb.Password = PasswordVisibleTb.Text;
        }

        // Синхронизация пароля из скрытого поля в видимое
        private void PasswordTb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                PasswordVisibleTb.Text = PasswordTb.Password;
            }

            if (isPasswordPlaceholder && !string.IsNullOrEmpty(PasswordTb.Password) && PasswordTb.Password != "Пароль")
            {
                isPasswordPlaceholder = false;
                PasswordTb.Foreground = Brushes.White;
                PasswordVisibleTb.Foreground = Brushes.White;
            }
        }

        // Обработчики для видимого поля пароля
        private void PasswordVisibleTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isPasswordPlaceholder)
            {
                PasswordVisibleTb.Text = "";
                PasswordVisibleTb.Foreground = Brushes.White;
                isPasswordPlaceholder = false;
            }
        }

        private void PasswordVisibleTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordVisibleTb.Text))
            {
                PasswordVisibleTb.Text = "Пароль";
                PasswordVisibleTb.Foreground = Brushes.Gray;
                isPasswordPlaceholder = true;
            }
        }

        // Обработчики для скрытого поля пароля (оставляем как есть)
        private void PasswordTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isPasswordPlaceholder)
            {
                PasswordTb.Password = "";
                PasswordTb.Foreground = Brushes.White;
                isPasswordPlaceholder = false;
            }
        }

        private void PasswordTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordTb.Password))
            {
                PasswordTb.Password = "Пароль";
                PasswordTb.Foreground = Brushes.Gray;
                isPasswordPlaceholder = true;
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrEmpty(CodeTb.Text) || CodeTb.Text == "Код сотрудника")
            {
                MessageBox.Show("Введите код сотрудника!");
                return;
            }

            // Получаем актуальный пароль из активного поля
            string actualPassword = isPasswordVisible ? PasswordVisibleTb.Text : PasswordTb.Password;

            if (string.IsNullOrEmpty(actualPassword) || isPasswordPlaceholder)
            {
                MessageBox.Show("Введите пароль!");
                return;
            }

            if (!int.TryParse(CodeTb.Text, out int userId))
            {
                MessageBox.Show("Код сотрудника должен быть числом!");
                return;
            }

            // Поиск в базе
            currentUser = App.context.Users
                .FirstOrDefault(user => user.id_user == userId && user.password == actualPassword);

            if (currentUser != null)
            {
                // Сохраняем или очищаем данные
                if (RememberMeCb.IsChecked == true)
                {
                    RememberPassword.SaveLogin(CodeTb.Text, actualPassword);
                }
                else
                {
                    RememberPassword.Forget();
                }

                Window authWindow = Window.GetWindow(this);

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                if (authWindow != null) authWindow.Close();
            }
            else
            {
                MessageBox.Show("Неверный код сотрудника или пароль!");
            }
        }

        private void ForgetBtn_Click(object sender, RoutedEventArgs e)
        {
            WarningForgetPasswordWindow warningForgetPasswordWindow = new WarningForgetPasswordWindow();
            warningForgetPasswordWindow.Show();
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegistrationPage());
        }

        private void CodeTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CodeTb.Text == "Код сотрудника")
            {
                CodeTb.Text = "";
                CodeTb.Foreground = Brushes.White;
            }
        }

        private void CodeTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CodeTb.Text))
            {
                CodeTb.Text = "Код сотрудника";
                CodeTb.Foreground = Brushes.Gray;
            }
        }
    }
}