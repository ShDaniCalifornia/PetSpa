using PetSpa.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    public partial class EditAppointmentPage : Page, System.ComponentModel.INotifyPropertyChanged
    {
        private readonly PetSpaEntities _context = App.context;
        private DateTime _currentMonth;
        private DateTime? _selectedCalendarDate;
        private readonly int _appointmentId;
        private Appointments _appointment;

        public class CalendarDay
        {
            public int Day { get; set; }
            public DateTime Date { get; set; }
            public bool IsCurrentMonth { get; set; }
            public bool IsToday { get; set; }
            public bool IsSelected { get; set; }

            public Brush TextColor
            {
                get
                {
                    if (IsToday || IsSelected)
                        return Brushes.White;

                    if (Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday)
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));

                    return Brushes.Black;
                }
            }
        }

        public ObservableCollection<CalendarDay> CalendarDays { get; set; }

        public string DisplayYear => _currentMonth.ToString("yyyy");
        public string DisplayMonth => GetMonthName(_currentMonth.Month);

        public EditAppointmentPage(int appointmentId)
        {
            InitializeComponent();
            this.DataContext = this;

            _appointmentId = appointmentId;
            CalendarDays = new ObservableCollection<CalendarDay>();
            CalendarDaysControl.ItemsSource = CalendarDays;

            _currentMonth = DateTime.Today;

            InitializeCalendar();
            LoadServices();
            LoadAvailableTimes();
            LoadAppointmentData();

            CalendarDaysControl.MouseLeftButtonUp += CalendarDaysControl_MouseLeftButtonUp;
        }

        public EditAppointmentPage() : this(0) { }

        private string GetMonthName(int month)
        {
            switch (month)
            {
                case 1: return "январь";
                case 2: return "февраль";
                case 3: return "март";
                case 4: return "апрель";
                case 5: return "май";
                case 6: return "июнь";
                case 7: return "июль";
                case 8: return "август";
                case 9: return "сентябрь";
                case 10: return "октябрь";
                case 11: return "ноябрь";
                case 12: return "декабрь";
                default: return "месяц";
            }
        }

        private void InitializeCalendar()
        {
            CalendarDays.Clear();

            // Обновление привязок
            OnPropertyChanged(nameof(DisplayYear));
            OnPropertyChanged(nameof(DisplayMonth));

            var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int firstDayWeekDay = (int)firstDayOfMonth.DayOfWeek;
            firstDayWeekDay = firstDayWeekDay == 0 ? 6 : firstDayWeekDay - 1;

            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Дни предыдущего месяца
            var previousMonth = firstDayOfMonth.AddDays(-firstDayWeekDay);
            for (int i = 0; i < firstDayWeekDay; i++)
            {
                var dayDate = previousMonth.AddDays(i);
                CalendarDays.Add(new CalendarDay
                {
                    Day = dayDate.Day,
                    Date = dayDate,
                    IsCurrentMonth = false,
                    IsToday = dayDate.Date == DateTime.Today.Date,
                    IsSelected = _selectedCalendarDate.HasValue && dayDate.Date == _selectedCalendarDate.Value.Date
                });
            }

            // Дни текущего месяца
            for (int day = 1; day <= lastDayOfMonth.Day; day++)
            {
                var dayDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
                CalendarDays.Add(new CalendarDay
                {
                    Day = day,
                    Date = dayDate,
                    IsCurrentMonth = true,
                    IsToday = dayDate.Date == DateTime.Today.Date,
                    IsSelected = _selectedCalendarDate.HasValue && dayDate.Date == _selectedCalendarDate.Value.Date
                });
            }

            // Дни следующего месяца
            int totalCells = 42;
            int currentCount = CalendarDays.Count;

            if (currentCount < totalCells)
            {
                var nextMonth = lastDayOfMonth.AddDays(1);
                for (int i = 0; i < totalCells - currentCount; i++)
                {
                    var dayDate = nextMonth.AddDays(i);
                    CalendarDays.Add(new CalendarDay
                    {
                        Day = dayDate.Day,
                        Date = dayDate,
                        IsCurrentMonth = false,
                        IsToday = dayDate.Date == DateTime.Today.Date,
                        IsSelected = _selectedCalendarDate.HasValue && dayDate.Date == _selectedCalendarDate.Value.Date
                    });
                }
            }

            // Обновляем выделение сегодняшней даты
            UpdateTodaySelection();
        }

        private void UpdateTodaySelection()
        {
            foreach (var day in CalendarDays)
            {
                if (day.Date.Date == DateTime.Today.Date)
                {
                    day.IsToday = true;
                }
            }

            var tempDays = new ObservableCollection<CalendarDay>(CalendarDays);
            CalendarDays.Clear();
            foreach (var day in tempDays)
            {
                CalendarDays.Add(day);
            }
        }

        private void LoadServices()
        {
            var services = _context.Services
                .OrderBy(s => s.name_service)
                .ToList();

            ServiceComboBox.ItemsSource = services;
            ServiceComboBox.DisplayMemberPath = "name_service";
            ServiceComboBox.SelectedValuePath = "id_service";
        }

        private void LoadAvailableTimes()
        {

            TimeComboBox.Items.Clear();

            // Создаем элементы времени
            for (int hour = 9; hour <= 18; hour++)
            {
                var timeString = $"{hour:00}:00";

                var comboItem = new ComboBoxItem
                {
                    Content = timeString,
                    Tag = timeString,
                    FontSize = 14
                };

                TimeComboBox.Items.Add(comboItem);
            }
        }

        private void LoadAppointmentData()
        {
            try
            {
                if (_appointmentId <= 0)
                {
                    MessageBox.Show("ID записи не указан", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService?.GoBack();
                    return;
                }

                _appointment = _context.Appointments
                    .Include(a => a.Clients)
                    .Include(a => a.Pets)
                    .Include(a => a.Services)
                    .FirstOrDefault(a => a.id_appointments == _appointmentId);

                if (_appointment == null)
                {
                    MessageBox.Show("Запись не найдена!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService?.GoBack();
                    return;
                }

                // Заполняем данные
                if (_appointment.Clients != null)
                {
                    OwnerNameTb.Text = _appointment.Clients.full_name?.Trim() ?? "";
                }

                if (_appointment.Pets != null)
                {
                    PetNameTb.Text = _appointment.Pets.name_pet?.Trim() ?? "";
                    PetBreedTb.Text = _appointment.Pets.Breed?.Trim() ?? "";
                    PetWeightTb.Text = _appointment.Pets.weight.ToString();
                }

                if (_appointment.Services != null)
                {
                    ServiceComboBox.SelectedValue = _appointment.id_service;
                }

                // Устанавливаем дату
                if (_appointment.data_appointment != null)
                {
                    AppointmentDatePicker.SelectedDate = _appointment.data_appointment;
                    _selectedCalendarDate = _appointment.data_appointment;
                    UpdateCalendarSelection();
                }

                // Устанавливаем время
                if (_appointment.time_appointment != null)
                {
                    var timeString = _appointment.time_appointment.ToString(@"hh\:mm");
                    foreach (ComboBoxItem item in TimeComboBox.Items)
                    {
                        if (item.Tag != null && item.Tag.ToString() == timeString)
                        {
                            TimeComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                // Убираем плейсхолдеры
                RemovePlaceholderStyles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemovePlaceholderStyles()
        {
            if (!string.IsNullOrWhiteSpace(OwnerNameTb.Text) && OwnerNameTb.Text != "ФИО владельца")
            {
                OwnerNameTb.Foreground = Brushes.Black;
            }

            if (!string.IsNullOrWhiteSpace(PetNameTb.Text) && PetNameTb.Text != "Кличка питомца")
            {
                PetNameTb.Foreground = Brushes.Black;
            }

            if (!string.IsNullOrWhiteSpace(PetBreedTb.Text) && PetBreedTb.Text != "Порода питомца")
            {
                PetBreedTb.Foreground = Brushes.Black;
            }

            if (!string.IsNullOrWhiteSpace(PetWeightTb.Text) && PetWeightTb.Text != "Вес питомца (кг)")
            {
                PetWeightTb.Foreground = Brushes.Black;
            }
        }

        private void UpdateCalendarSelection()
        {
            foreach (var day in CalendarDays)
            {
                day.IsSelected = _selectedCalendarDate.HasValue &&
                                day.Date.Date == _selectedCalendarDate.Value.Date;
            }

            var tempDays = new ObservableCollection<CalendarDay>(CalendarDays);
            CalendarDays.Clear();
            foreach (var day in tempDays)
            {
                CalendarDays.Add(day);
            }
        }

        private void CalendarDaysControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is CalendarDay calendarDay)
            {
                _selectedCalendarDate = calendarDay.Date;
                AppointmentDatePicker.SelectedDate = calendarDay.Date;
                UpdateCalendarSelection();
            }
        }

        private void PrevMonthBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            InitializeCalendar();
        }

        private void NextMonthBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            InitializeCalendar();
        }

        // Обработчики плейсхолдеров
        private void OwnerNameTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "ФИО владельца")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void OwnerNameTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "ФИО владельца";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PetNameTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Кличка питомца")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PetNameTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Кличка питомца";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PetBreedTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Порода питомца")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PetBreedTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Порода питомца";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PetWeightTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Вес питомца (кг)")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PetWeightTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Вес питомца (кг)";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (ServiceComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите услугу", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    ServiceComboBox.Focus();
                    return;
                }

                if (!AppointmentDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    AppointmentDatePicker.Focus();
                    return;
                }

                if (TimeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите время", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    TimeComboBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(OwnerNameTb.Text) || OwnerNameTb.Text == "ФИО владельца")
                {
                    MessageBox.Show("Введите ФИО владельца", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    OwnerNameTb.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PetNameTb.Text) || PetNameTb.Text == "Кличка питомца")
                {
                    MessageBox.Show("Введите кличку питомца", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    PetNameTb.Focus();
                    return;
                }

                // Проверка веса
                if (string.IsNullOrWhiteSpace(PetWeightTb.Text) || PetWeightTb.Text == "Вес питомца (кг)")
                {
                    MessageBox.Show("Введите вес питомца", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    PetWeightTb.Focus();
                    return;
                }

                if (!decimal.TryParse(PetWeightTb.Text, out decimal weight) || weight <= 0)
                {
                    MessageBox.Show("Введите корректный вес питомца (число больше 0)",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    PetWeightTb.Focus();
                    return;
                }

                // ПОЛУЧАЕМ ДАННЫЕ
                var selectedService = ServiceComboBox.SelectedItem as Services;
                if (selectedService == null)
                {
                    MessageBox.Show("Выберите услугу", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedDate = AppointmentDatePicker.SelectedDate.Value;
                var timeStr = (TimeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

                if (string.IsNullOrEmpty(timeStr) || !timeStr.Contains(":"))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var timeParts = timeStr.Split(':');
                if (!int.TryParse(timeParts[0], out int hours) || !int.TryParse(timeParts[1], out int minutes))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var appointmentDateTime = new DateTime(
                    selectedDate.Year,
                    selectedDate.Month,
                    selectedDate.Day,
                    hours,
                    minutes,
                    0);

                // Проверки
                if (appointmentDateTime < DateTime.Now)
                {
                    MessageBox.Show("Нельзя записаться на прошедшую дату или время",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (hours < 9 || hours > 18)
                {
                    MessageBox.Show("Время должно быть в диапазоне с 9:00 до 18:00",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool timeIsAvailable = CheckTimeAvailability(selectedDate, TimeSpan.FromHours(hours).Add(TimeSpan.FromMinutes(minutes)), _appointmentId);
                if (!timeIsAvailable)
                {
                    MessageBox.Show("На это время уже есть запись. Выберите другое время.",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Обновляем клиента
                Clients client = _context.Clients
                    .FirstOrDefault(c => c.full_name == OwnerNameTb.Text.Trim());

                if (client == null)
                {
                    client = new Clients
                    {
                        full_name = OwnerNameTb.Text.Trim(),
                        date_of_birth = DateTime.Now.AddYears(-25),
                        phone = "не указан"
                    };
                    _context.Clients.Add(client);
                    _context.SaveChanges();
                }

                // Обновляем питомца
                Pets pet = _appointment.Pets;
                if (pet == null)
                {
                    pet = new Pets
                    {
                        name_pet = PetNameTb.Text.Trim(),
                        Breed = string.IsNullOrWhiteSpace(PetBreedTb.Text) || PetBreedTb.Text == "Порода питомца"
                                ? "Не указано" : PetBreedTb.Text.Trim(),
                        weight = (int)Math.Round(weight),
                        birthday = DateTime.Now.AddYears(-1),
                        id_client = client.id_client,
                        photo = null
                    };
                    _context.Pets.Add(pet);
                }
                else
                {
                    pet.name_pet = PetNameTb.Text.Trim();
                    pet.Breed = string.IsNullOrWhiteSpace(PetBreedTb.Text) || PetBreedTb.Text == "Порода питомца"
                            ? "Не указано" : PetBreedTb.Text.Trim();
                    pet.weight = (int)Math.Round(weight);
                    pet.id_client = client.id_client;
                }
                _context.SaveChanges();

                // Вычисление цены с учетом веса
                decimal finalPrice = CalculatePriceByWeight(selectedService, weight);

                // Обновление записи
                _appointment.id_service = selectedService.id_service;
                _appointment.data_appointment = selectedDate;
                _appointment.time_appointment = TimeSpan.FromHours(hours).Add(TimeSpan.FromMinutes(minutes));
                _appointment.price = finalPrice;
                _appointment.id_client = client.id_client;
                _appointment.id_pet = pet.id_pet;

                _context.SaveChanges();

                MessageBox.Show($"Запись успешно обновлена!\nСтоимость: {finalPrice:N0} ₽", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private decimal CalculatePriceByWeight(Services service, decimal weight)
        {
            try
            {
                var servicePrice = _context.ServicePrices
                    .Include(sp => sp.WeightCategories)
                    .FirstOrDefault(sp => sp.id_service == service.id_service &&
                                         sp.WeightCategories != null &&
                                         weight >= sp.WeightCategories.min_weight &&
                                         (sp.WeightCategories.max_weight == null ||
                                          weight <= sp.WeightCategories.max_weight));

                return servicePrice?.price ?? service.price;
            }
            catch
            {
                return service.price;
            }
        }

        private bool CheckTimeAvailability(DateTime date, TimeSpan time, int excludeAppointmentId)
        {
            try
            {
                // Проверка, есть ли другие записи на это время
                return !_context.Appointments
                    .Any(a => a.data_appointment.Date == date.Date &&
                             a.time_appointment == time &&
                             a.id_appointments != excludeAppointmentId);
            }
            catch
            {
                return true;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}