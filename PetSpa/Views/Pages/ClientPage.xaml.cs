using PetSpa.AppData;
using PetSpa.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private Model.PetSpaEntities _context = App.context;
        public ObservableCollection<Model.ClientViewModel> ClientViewModels { get; private set; }
        private System.ComponentModel.ICollectionView _clientsView;

        // Для поиска
        private string _searchText = "";

        public ClientPage()
        {
            InitializeComponent();
            ClientViewModels = new ObservableCollection<Model.ClientViewModel>();
            DataContext = this;

            _clientsView = System.Windows.Data.CollectionViewSource.GetDefaultView(ClientViewModels);

            LoadFromDatabase();
        }

        private void LoadFromDatabase()
        {
            try
            {
                if (!_context.Database.Exists())
                {
                    MessageBox.Show("База данных не найдена!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var clients = _context.Clients
                    .Include(c => c.Pets)
                    .Include(c => c.Appointments)
                    .ToList();

                ClientViewModels.Clear();

                foreach (var client in clients)
                {
                    var viewModel = new Model.ClientViewModel
                    {
                        FullName = client.full_name?.Trim() ?? "",
                        Phone = client.phone?.Trim() ?? "",
                        FormattedPhone = FormatPhone(client.phone),
                        BirthDate = client.date_of_birth,
                        BirthDateText = client.date_of_birth.ToString("dd.MM.yyyy"),
                        ClientId = client.id_client
                    };

                    // Получаем последний визит
                    if (client.Appointments != null && client.Appointments.Any())
                    {
                        var lastAppointment = client.Appointments
                            .OrderByDescending(a => a.data_appointment)
                            .ThenByDescending(a => a.time_appointment)
                            .FirstOrDefault();

                        if (lastAppointment != null)
                        {
                            var time = lastAppointment.time_appointment.ToString(@"hh\:mm");
                            viewModel.LastVisitText = $"{lastAppointment.data_appointment:dd.MM.yyyy} {time}";
                            viewModel.LastVisitDate = lastAppointment.data_appointment;
                        }
                        else
                        {
                            viewModel.LastVisitText = "не было визитов";
                            viewModel.LastVisitDate = null;
                        }
                    }
                    else
                    {
                        viewModel.LastVisitText = "не было визитов";
                        viewModel.LastVisitDate = null;
                    }

                    // Получаем информацию о питомце
                    if (client.Pets != null && client.Pets.Any())
                    {
                        var firstPet = client.Pets.First();
                        viewModel.PetInfo = $"{firstPet.Breed?.Trim()} {firstPet.name_pet?.Trim()}";
                        viewModel.PetWeight = firstPet.weight.ToString();
                        viewModel.PetPhotoUrl = firstPet.photo?.Trim();

                        // Дата рождения питомца
                        DateTime petBirthday = firstPet.birthday;
                        DateTime today = DateTime.Today;

                        // Сохраняем дату рождения питомца
                        viewModel.PetBirthDate = petBirthday;

                        // Вычисляем возраст
                        int age = today.Year - petBirthday.Year;

                        if (petBirthday.Date > today.AddYears(-age))
                        {
                            age--;
                        }

                        // Формируем текст возраста
                        string formattedAge;
                        if (age == 0)
                        {
                            int months = (today.Year - petBirthday.Year) * 12 + today.Month - petBirthday.Month;
                            if (months <= 0) months = 1;

                            if (months == 1)
                                formattedAge = "1 месяц";
                            else if (months >= 2 && months <= 4)
                                formattedAge = $"{months} месяца";
                            else if (months >= 5 && months <= 12)
                                formattedAge = $"{months} месяцев";
                            else
                                formattedAge = $"{months} месяцев";
                        }
                        else if (age == 1)
                        {
                            formattedAge = "1 год";
                        }
                        else if (age >= 2 && age <= 4)
                        {
                            formattedAge = $"{age} года";
                        }
                        else
                        {
                            formattedAge = $"{age} лет";
                        }

                        // Формируем полный текст
                        string ageText = $"{petBirthday:dd.MM.yyyy} ({formattedAge})";
                        ageText = ageText.Replace(">", "").Trim();
                        viewModel.PetAgeText = ageText;
                    }
                    else
                    {
                        viewModel.PetInfo = "нет питомца";
                        viewModel.PetWeight = "0";
                        viewModel.PetAgeText = "возраст неизвестен";
                        viewModel.PetPhotoUrl = null;
                        viewModel.PetBirthDate = null;
                    }

                    ClientViewModels.Add(viewModel);
                }

                if (ClientsLb.ItemsSource == null)
                {
                    ClientsLb.ItemsSource = _clientsView;
                }

                // Начальные фильтры
                ApplyFilters();

                Debug.WriteLine($"Загружено клиентов: {ClientViewModels.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод применения всех фильтров
        private void ApplyFilters()
        {
            if (_clientsView == null) return; // Проверяем на null

            // Фильтрация по поиску
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                string searchLower = _searchText.ToLower();
                _clientsView.Filter = item =>
                {
                    var client = item as Model.ClientViewModel;
                    if (client == null) return false;

                    return client.FullName?.ToLower().Contains(searchLower) == true ||
                           client.Phone?.ToLower().Contains(searchLower) == true ||
                           client.FormattedPhone?.ToLower().Contains(searchLower) == true ||
                           client.PetInfo?.ToLower().Contains(searchLower) == true ||
                           client.PetWeight?.ToLower().Contains(searchLower) == true ||
                           client.PetAgeText?.ToLower().Contains(searchLower) == true;
                };
            }
            else
            {
                _clientsView.Filter = null; // Сбрасываем фильтр если поиск пустой
            }

            // Сортировка
            _clientsView.SortDescriptions.Clear();

            switch (FilterComboBox.SelectedIndex)
            {
                case 1: // По дате рождения клиента
                    _clientsView.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("BirthDate",
                        System.ComponentModel.ListSortDirection.Ascending));
                    break;

                case 2: // По дате рождения питомца
                    _clientsView.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("PetBirthDate",
                        System.ComponentModel.ListSortDirection.Ascending));
                    break;

                case 3: // По дате визита
                    _clientsView.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("LastVisitDate",
                        System.ComponentModel.ListSortDirection.Descending));
                    break;
            }

            // Обновляем представление
            _clientsView.Refresh();
        }

        // Обработчик поиска
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text != "Поиск")
            {
                _searchText = SearchTextBox.Text.Trim();
                ApplyFilters();
            }
            else
            {
                _searchText = "";
                ApplyFilters();
            }
        }

        // Обработчик ComboBox
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";

            string digits = new string(phone.Where(char.IsDigit).ToArray());

            if (digits.Length == 11)
            {
                return $"8 ({digits.Substring(1, 3)}) {digits.Substring(4, 3)} {digits.Substring(7, 2)} {digits.Substring(9, 2)}";
            }
            else if (digits.Length == 10)
            {
                return $"8 ({digits.Substring(0, 3)}) {digits.Substring(3, 3)} {digits.Substring(6, 2)} {digits.Substring(8, 2)}";
            }

            return phone;
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Поиск")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = Brushes.White;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Поиск";
                SearchTextBox.Foreground = Brushes.Gray;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is Model.ClientViewModel client)
            {
                // Создаем и показываем окно редактирования
                EditClientWindow editWindow = new EditClientWindow(client.ClientId);

                // Устанавливаем владельца
                editWindow.Owner = Application.Current.MainWindow;

                bool? result = editWindow.ShowDialog();

                // Если данные были сохранены, обновляем список
                if (result == true)
                {
                    LoadFromDatabase(); // Перезагружаем данные клиентов
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is Model.ClientViewModel client)
            {
                // Открываем окно подтверждения удаления
                var warningWindow = new WarningDeletePetWindow();
                warningWindow.DataContext = client;
                warningWindow.Owner = Application.Current.MainWindow;

                bool? dialogResult = warningWindow.ShowDialog();

                if (dialogResult == true)
                {
                    // Клиент и данные
                    var clientToDelete = _context.Clients
                        .Include(c => c.Pets) // Загружаем питомцев
                        .Include(c => c.Appointments) // Загружаем записи
                        .FirstOrDefault(c => c.id_client == client.ClientId);

                    if (clientToDelete != null)
                    {
                        // Удаляем все записи на прием этого клиента
                        if (clientToDelete.Appointments != null && clientToDelete.Appointments.Any())
                        {
                            _context.Appointments.RemoveRange(clientToDelete.Appointments);
                        }

                        // Удаляем всех питомцев клиента
                        if (clientToDelete.Pets != null && clientToDelete.Pets.Any())
                        {
                            _context.Pets.RemoveRange(clientToDelete.Pets);
                        }

                        // Удаляем самого клиента
                        _context.Clients.Remove(clientToDelete);

                        // Сохраняем все изменения
                        _context.SaveChanges();

                        // Перезагружаем данные
                        LoadFromDatabase();

                        MessageBox.Show($"Клиент {client.FullName} удален",
                            "Успешно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
            }
        }

        private void AddClientBtn_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.FramePanel.Navigate(new Views.Pages.PanelAddClientPage());
            ClassFrame.FrameBody.Navigate(new Views.Pages.AddClientPetPage());
        }
    }
}