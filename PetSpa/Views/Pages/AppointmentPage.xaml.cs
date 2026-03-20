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

        public bool IsAdmin => AuthorizationPage.currentUser?.id_role == 1;

        public AppointmentPage()
        {
            InitializeComponent();
            AppointmentViewModels = new ObservableCollection<AppointmentViewModel>();

            _appointmentsView = CollectionViewSource.GetDefaultView(AppointmentViewModels);
            _appointmentsView.GroupDescriptions.Add(new PropertyGroupDescription("FormattedDate"));
            _appointmentsView.SortDescriptions.Add(new SortDescription("AppointmentDate", ListSortDirection.Descending));
            _appointmentsView.SortDescriptions.Add(new SortDescription("AppointmentTime", ListSortDirection.Ascending));

            AppointmentsItemsControl.ItemsSource = _appointmentsView;
            DataContext = this;

            // Подписываемся на событие загрузки страницы
            this.Loaded += AppointmentPage_Loaded;
        }

        private void AppointmentPage_Loaded(object sender, RoutedEventArgs e)
        {
            // При каждой загрузке страницы перезагружаем данные
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
                    string petName = appointment.Pets?.name_pet?.Trim() ?? "";
                    string petBreed = appointment.Pets?.Breed?.Trim() ?? "";
                    string petInfo = !string.IsNullOrEmpty(petBreed) ? $"{petName} ({petBreed})" : petName;

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

                // Обновляем фильтр после загрузки
                ApplyFilters();

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

            _appointmentsView.Refresh();
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
                if (NavigationService != null)
                {
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
                var warningWindow = new WarningDeleteAppointmentWindow();

                var appointmentInfo = new
                {
                    Time = appointment.FormattedTime,
                    Date = appointment.AppointmentDate.ToString("dd.MM.yyyy"),
                    Client = appointment.ClientFullName,
                    Pet = appointment.PetInfo,
                    Service = appointment.ServiceName
                };

                warningWindow.DataContext = appointmentInfo;
                warningWindow.Owner = Application.Current.MainWindow;

                bool? dialogResult = warningWindow.ShowDialog();

                if (dialogResult == true)
                {
                    var appointmentToDelete = _context.Appointments.Find(appointment.AppointmentId);
                    if (appointmentToDelete != null)
                    {
                        _context.Appointments.Remove(appointmentToDelete);
                        _context.SaveChanges();
                        AppointmentViewModels.Remove(appointment);
                        _appointmentsView.Refresh();

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