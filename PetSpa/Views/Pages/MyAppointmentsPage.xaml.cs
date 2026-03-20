using PetSpa.Model;
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
using System.Windows.Threading;

namespace PetSpa.Views.Pages
{
    public partial class MyAppointmentsPage : Page
    {
        private PetSpaEntities _context = App.context;
        public ObservableCollection<AppointmentViewModel> AppointmentViewModels { get; private set; }
        private ICollectionView _appointmentsView;
        private string _searchText = "";
        private int _currentUserId;

        public MyAppointmentsPage()
        {
            InitializeComponent();

            if (AuthorizationPage.currentUser != null)
            {
                _currentUserId = AuthorizationPage.currentUser.id_user;
            }
            else
            {
                MessageBox.Show("Ошибка: пользователь не авторизован", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService?.GoBack();
                return;
            }

            AppointmentViewModels = new ObservableCollection<AppointmentViewModel>();

            _appointmentsView = CollectionViewSource.GetDefaultView(AppointmentViewModels);
            _appointmentsView.GroupDescriptions.Add(new PropertyGroupDescription("FormattedDate"));
            _appointmentsView.SortDescriptions.Add(new SortDescription("AppointmentDate", ListSortDirection.Descending));
            _appointmentsView.SortDescriptions.Add(new SortDescription("AppointmentTime", ListSortDirection.Ascending));

            AppointmentsItemsControl.ItemsSource = _appointmentsView;

            // Загружаем данные сразу
            LoadFromDatabase();
        }

        private void LoadFromDatabase()
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var client = _context.Clients.FirstOrDefault(c => c.id_user == _currentUserId);

                    if (client == null)
                    {
                        AppointmentViewModels.Clear();
                        return;
                    }

                    var appointments = _context.Appointments
                        .Include(a => a.Clients)
                        .Include(a => a.Pets)
                        .Include(a => a.Services)
                        .Include(a => a.Masters)
                        .Where(a => a.id_client == client.id_client)
                        .OrderByDescending(a => a.data_appointment)
                        .ThenBy(a => a.time_appointment)
                        .ToList();

                    AppointmentViewModels.Clear();

                    foreach (var appointment in appointments)
                    {
                        string petName = appointment.Pets?.name_pet?.Trim() ?? "";
                        string petBreed = appointment.Pets?.Breed?.Trim() ?? "";
                        string petInfo = !string.IsNullOrEmpty(petBreed) ? $"{petName} ({petBreed})" : petName;

                        string serviceName = appointment.Services?.name_service?.Trim() ?? "Услуга";

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

                    ApplyFilters();

                    Debug.WriteLine($"Загружено записей для пользователя {_currentUserId}: {AppointmentViewModels.Count}");

                    // Принудительно обновляем ItemsControl
                    AppointmentsItemsControl.Items.Refresh();
                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки записей: {ex.Message}", "Ошибка",
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
    }
}