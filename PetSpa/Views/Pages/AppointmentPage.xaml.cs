using PetSpa.Model;
using PetSpa.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    public partial class AppointmentPage : Page
    {
        private Model.PetSpaEntities _context = App.context;
        public ObservableCollection<AppointmentViewModel> AppointmentViewModels { get; private set; }
        private ICollectionView _appointmentsView;
        private string _searchText = "";

        public AppointmentPage()
        {
            InitializeComponent();
            AppointmentViewModels = new ObservableCollection<AppointmentViewModel>();
            LoadFromDatabase();

            // Создаем CollectionView для группировки и фильтрации
            _appointmentsView = CollectionViewSource.GetDefaultView(AppointmentViewModels);
            _appointmentsView.GroupDescriptions.Add(new PropertyGroupDescription("FormattedDate"));

            // Сортируем по дате и времени
            _appointmentsView.SortDescriptions.Add(new SortDescription("AppointmentDate", ListSortDirection.Descending));
            _appointmentsView.SortDescriptions.Add(new SortDescription("AppointmentTime", ListSortDirection.Ascending));

            AppointmentsItemsControl.ItemsSource = _appointmentsView;
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

                // Загружаем записи с связанными данными
                var appointments = _context.Appointments
                    .Include(a => a.Clients)
                    .Include(a => a.Pets)
                    .Include(a => a.Services)
                    .Include(a => a.Masters)
                    .OrderByDescending(a => a.data_appointment)
                    .ThenBy(a => a.time_appointment)
                    .ToList();

                AppointmentViewModels.Clear();

                foreach (var appointment in appointments)
                {
                    // Получаем информацию о клиенте и питомце
                    string petName = appointment.Pets?.name_pet?.Trim() ?? "";
                    string petBreed = appointment.Pets?.Breed?.Trim() ?? "";
                    string petInfo = !string.IsNullOrEmpty(petBreed) ? $"{petName} ({petBreed})" : petName;

                    // Получаем название услуги
                    string serviceName = appointment.Services?.name_service?.Trim() ??
                                         "Услуга";

                    var viewModel = new AppointmentViewModel
                    {
                        AppointmentId = appointment.id_appointments,
                        AppointmentDate = appointment.data_appointment,
                        AppointmentTime = appointment.time_appointment,
                        ClientFullName = appointment.Clients?.full_name?.Trim() ?? "",
                        PetInfo = petInfo,
                        ServiceName = serviceName,
                        ServicePrice = appointment.price,
                        MasterFullName = appointment.Masters?.full_name?.Trim() ?? ""
                    };

                    AppointmentViewModels.Add(viewModel);
                }

                Debug.WriteLine($"Загружено записей: {AppointmentViewModels.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки записей: {ex.Message}\n\n{ex.InnerException?.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_appointmentsView == null) return;

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                string searchLower = _searchText.ToLower();
                _appointmentsView.Filter = item =>
                {
                    var appointment = item as AppointmentViewModel;
                    if (appointment == null) return false;

                    return (appointment.ClientFullName != null && appointment.ClientFullName.ToLower().Contains(searchLower)) ||
                           (appointment.PetInfo != null && appointment.PetInfo.ToLower().Contains(searchLower)) ||
                           (appointment.ServiceName != null && appointment.ServiceName.ToLower().Contains(searchLower)) ||
                           (appointment.MasterFullName != null && appointment.MasterFullName.ToLower().Contains(searchLower));
                };
            }
            else
            {
                _appointmentsView.Filter = null;
            }
        }

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

        private void AddAppointmenttBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
            {
                NavigationService.Navigate(new AddAppointmentPage());
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is AppointmentViewModel appointment)
            {
                // Переход на страницу редактирования записи с передачей ID записи
                if (NavigationService != null)
                {
                    // Создаем параметр для передачи ID записи
                    var editPage = new EditAppointmentPage(appointment.AppointmentId);
                    NavigationService.Navigate(editPage);
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is AppointmentViewModel appointment)
            {
                // Создаем окно подтверждения удаления записи
                var warningWindow = new WarningDeleteAppointmentWindow();

                // Создаем объект с информацией о записи
                var appointmentInfo = new
                {
                    Time = appointment.FormattedTime,
                    Date = appointment.AppointmentDate.ToString("dd.MM.yyyy"),
                    Client = appointment.ClientFullName,
                    Pet = appointment.PetInfo,
                    Service = appointment.ServiceName
                };

                warningWindow.DataContext = appointmentInfo;

                // Устанавливаем владельца 
                warningWindow.Owner = Application.Current.MainWindow;

                bool? dialogResult = warningWindow.ShowDialog();

                // Если пользователь подтвердил удаление
                if (dialogResult == true)
                {
                    // Находим запись в базе данных
                    var appointmentToDelete = _context.Appointments.Find(appointment.AppointmentId);
                    if (appointmentToDelete != null)
                    {
                        // Удаляем запись
                        _context.Appointments.Remove(appointmentToDelete);
                        _context.SaveChanges();

                        // Удаляем из коллекции
                        AppointmentViewModels.Remove(appointment);

                        // Обновляем представление
                        _appointmentsView.Refresh();

                        // Показываем сообщение об успешном удалении
                        MessageBox.Show($"Запись от {appointment.AppointmentDate:dd.MM.yyyy} {appointment.FormattedTime} удалена",
                            "Успешно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
            }
        }
    }
}