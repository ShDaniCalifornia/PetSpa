using PetSpa.Model;
using PetSpa.Views.Windows;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            // Очищаем телефон от всех нецифровых символов
            string cleanPhone = Regex.Replace(PhoneTb.Text, @"[^\d]", "");

            // Проверяем длину (должно быть 11 цифр)
            if (cleanPhone.Length != 11)
            {
                MessageBox.Show("Введите корректный номер телефона (11 цифр, например: 89001234567)",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверяем уникальность телефона
            if (App.context.Users.Any(u => u.phone == cleanPhone))
            {
                MessageBox.Show("Пользователь с таким номером телефона уже существует", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверяем уникальность email
            if (App.context.Users.Any(u => u.email == EmailTb.Text.Trim()))
            {
                MessageBox.Show("Пользователь с таким email уже существует", "Ошибка",
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

        private string CleanPhone(string phone)
        {
            // Оставляем только цифры
            return Regex.Replace(phone, @"[^\d]", "");
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                string generatedLogin = GenerateLogin(EmailTb.Text.Trim());
                string generatedPassword = GeneratePassword();
                string cleanPhone = CleanPhone(PhoneTb.Text);

                var newUser = new Users
                {
                    full_name = FCsTb.Text.Trim(),
                    email = EmailTb.Text.Trim(),
                    phone = cleanPhone, // Сохраняем только цифры (11 символов)
                    id_role = 2,
                    login = generatedLogin,
                    password = generatedPassword,
                };

                App.context.Users.Add(newUser);
                App.context.SaveChanges();

                var newClient = new Clients
                {
                    full_name = FCsTb.Text.Trim(),
                    phone = cleanPhone, // Сохраняем только цифры (11 символов)
                    date_of_birth = DateTime.Now.AddYears(-25),
                    id_user = newUser.id_user
                };

                App.context.Clients.Add(newClient);
                App.context.SaveChanges();

                string message = $"Регистрация успешна!\n\n" +
                               $"Ваши данные для входа:\n" +
                               $"Код сотрудника: {newUser.id_user}\n" +
                               $"Пароль: {generatedPassword}\n\n" +
                               $"Сохраните эти данные!";

                MessageBox.Show(message, "Успешная регистрация",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                var authWindow = new RegAuthWindow();
                authWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                authWindow.Show();

                Window.GetWindow(this)?.Close();
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessage = "Ошибки валидации:\n";
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessage += $"• {validationError.PropertyName}: {validationError.ErrorMessage}\n";
                    }
                }

                MessageBox.Show($"Ошибка при регистрации:\n{errorMessage}",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (email.Contains("@"))
                return email.Split('@')[0];
            return "user_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private string GeneratePassword()
        {
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

        // Обработчики плейсхолдеров
        private void FCsTb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "ФИО")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.White;
            }
        }

        private void FCsTb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "ФИО";
                textBox.Foreground = new SolidColorBrush(Color.FromArgb(0x77, 0xFF, 0xFF, 0xFF));
            }
        }

        private void EmailTb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Почта")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.White;
            }
        }

        private void EmailTb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Почта";
                textBox.Foreground = new SolidColorBrush(Color.FromArgb(0x77, 0xFF, 0xFF, 0xFF));
            }
        }

        private void PhoneTb_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Телефон")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.White;
            }
        }

        private void PhoneTb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Телефон";
                textBox.Foreground = new SolidColorBrush(Color.FromArgb(0x77, 0xFF, 0xFF, 0xFF));
            }
            else if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text) && textBox.Text != "Телефон")
            {
                // Форматируем телефон для удобного отображения
                string cleanPhone = CleanPhone(textBox.Text);
                if (cleanPhone.Length == 11)
                {
                    textBox.Text = $"+7 ({cleanPhone.Substring(1, 3)}) {cleanPhone.Substring(4, 3)}-{cleanPhone.Substring(7, 2)}-{cleanPhone.Substring(9, 2)}";
                    textBox.Foreground = Brushes.White;
                }
            }
        }
    }
}