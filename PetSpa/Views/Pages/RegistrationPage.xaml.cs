using PetSpa.Model;
using PetSpa.Views.Windows;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PetSpa.Views.Pages
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private bool ValidateFields()
        {
            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(FCsTb.Text) || FCsTb.Text == "ФИО")
            {
                MessageBox.Show("Введите ФИО", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(EmailTb.Text) || EmailTb.Text == "Почта")
            {
                MessageBox.Show("Введите email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!IsValidEmail(EmailTb.Text))
            {
                MessageBox.Show("Введите корректный email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка телефона
            if (string.IsNullOrWhiteSpace(PhoneTb.Text) || PhoneTb.Text == "Телефон")
            {
                MessageBox.Show("Введите номер телефона", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка уникальности в базе данных
            if (App.context.Users.Any(u => u.email == EmailTb.Text.Trim()))
            {
                MessageBox.Show("Пользователь с таким email уже существует", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (App.context.Users.Any(u => u.phone == PhoneTb.Text.Trim()))
            {
                MessageBox.Show("Пользователь с таким номером телефона уже существует", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Оставляем только цифры
            string cleanPhone = Regex.Replace(phone, @"\D", "");
            return cleanPhone.Length == 11;
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                // ОЗДАНИЕ ПОЛЬЗОВАТЕЛЯ В БАЗЕ
                var newUser = new Users
                {
                    full_name = FCsTb.Text.Trim(),
                    email = EmailTb.Text.Trim(),
                    phone = PhoneTb.Text.Trim(),
                    id_role = 2, // Роль "Клиент" (админ = 1)
                    login = GenerateLogin(EmailTb.Text.Trim()),
                    password = GeneratePassword() // Генерируем случайный пароль
                };

                App.context.Users.Add(newUser);
                App.context.SaveChanges(); // Сохраняем чтобы получить id_user

                // СОЗДАНИЕ КЛИЕНТА В БАЗЕ 
                var newClient = new Clients
                {
                    full_name = FCsTb.Text.Trim(),
                    phone = PhoneTb.Text.Trim(),
                    id_user = newUser.id_user, // Связь с таблицей Users
                };

                App.context.Clients.Add(newClient);
                App.context.SaveChanges();

                string message = $"Регистрация успешна!\n\n" +
                               $"Ваши данные для входа:\n" +
                               $"Код сотрудника: {newUser.id_user}\n" +
                               $"Пароль: {newUser.password}\n\n" +
                               $"Сохраните эти данные!";

                MessageBox.Show(message, "Успешная регистрация",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService?.Navigate(new AuthorizationPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}\n\n" +
                              $"Проверьте подключение к базе данных.",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateLogin(string email)
        {
            // Генерируем логин из email (часть до @)
            if (email.Contains("@"))
                return email.Split('@')[0];

            // Или создаем уникальный логин
            return "user_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private string GeneratePassword()
        {
            // Генерируем 6-значный пароль
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AuthorizationPage());
        }

        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            InstructionWindow instructionWindow = new InstructionWindow();
            instructionWindow.Show();
        }

        // Обработчики placeholder для TextBox
        private void FCsTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FCsTb.Text == "ФИО")
            {
                FCsTb.Text = "";
                FCsTb.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void FCsTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FCsTb.Text))
            {
                FCsTb.Text = "ФИО";
                FCsTb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void EmailTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailTb.Text == "Почта")
            {
                EmailTb.Text = "";
                EmailTb.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void EmailTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTb.Text))
            {
                EmailTb.Text = "Почта";
                EmailTb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void PhoneTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PhoneTb.Text == "Телефон")
            {
                PhoneTb.Text = "";
                PhoneTb.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void PhoneTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PhoneTb.Text))
            {
                PhoneTb.Text = "Телефон";
                PhoneTb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void FCsTb_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EmailTb_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PhoneTb_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}