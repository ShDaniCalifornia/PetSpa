using PetSpa.Model;
using PetSpa.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PetSpa.Views.Pages
{
    public partial class PricePage : Page, INotifyPropertyChanged
    {
        private readonly PetSpaEntities _context = App.context;

        private List<ServiceViewModel> _allServices = new List<ServiceViewModel>();
        private int _currentPage = 0;
        private const int ServicesPerPage = 3;

        public ObservableCollection<ServiceViewModel> CurrentPageServices { get; set; }
        public ObservableCollection<PriceTableRow> PriceTableRows { get; set; }

        private string _currentServiceName1 = "";
        private string _currentServiceName2 = "";
        private string _currentServiceName3 = "";

        private bool _isTableVisible = true;

        public bool IsTableVisible
        {
            get => _isTableVisible;
            set { _isTableVisible = value; OnPropertyChanged(); }
        }

        public string CurrentServiceName1
        {
            get => _currentServiceName1;
            set { _currentServiceName1 = value; OnPropertyChanged(); }
        }

        public string CurrentServiceName2
        {
            get => _currentServiceName2;
            set { _currentServiceName2 = value; OnPropertyChanged(); }
        }

        public string CurrentServiceName3
        {
            get => _currentServiceName3;
            set { _currentServiceName3 = value; OnPropertyChanged(); }
        }

        public string PageIndicator => $"Страница {_currentPage + 1} из {TotalPages}";
        public bool HasPreviousPage => _currentPage > 0;
        public bool HasNextPage => _currentPage < TotalPages - 1;
        private int TotalPages => (int)Math.Ceiling((double)(_allServices.Count) / ServicesPerPage);

        public PricePage()
        {
            InitializeComponent();
            InitializeDataContext();
            LoadServicesFromDatabase();
            LoadCurrentPage();
        }

        private void InitializeDataContext()
        {
            CurrentPageServices = new ObservableCollection<ServiceViewModel>();
            PriceTableRows = new ObservableCollection<PriceTableRow>();
            DataContext = this;
        }

        private void LoadServicesFromDatabase()
        {
            try
            {
                _allServices.Clear();

                // Загружаем услуги с весовыми категориями
                var services = _context.Services
                    .Include(s => s.ServicePrices)
                    .Include("ServicePrices.WeightCategories")
                    .ToList();

                Debug.WriteLine($"Загружено {services.Count} услуг из базы");

                foreach (var service in services)
                {
                    Debug.WriteLine($"Услуга: {service.name_service}, Цена: {service.price}, Описание: {service.description}, Фото: {service.photo}");

                    var serviceVM = new ServiceViewModel(service);
                    _allServices.Add(serviceVM);
                }

                // Добавляем карточку "Добавить прайс"
                _allServices.Add(new ServiceViewModel()
                {
                    ServiceId = -1,
                    Name = "Добавить прайс",
                    IsAddCard = true,
                    PhotoUrl = null,
                    Description = "",
                    Price = 0m
                });

                Debug.WriteLine($"Всего карточек: {_allServices.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Debug.WriteLine($"Ошибка загрузки услуг: {ex.Message}");
            }
        }

        private void LoadCurrentPage()
        {
            CurrentPageServices.Clear();

            int startIndex = _currentPage * ServicesPerPage;
            int count = Math.Min(ServicesPerPage, _allServices.Count - startIndex);

            for (int i = 0; i < count; i++)
            {
                CurrentPageServices.Add(_allServices[startIndex + i]);
            }

            UpdatePriceTable();

            OnPropertyChanged(nameof(PageIndicator));
            OnPropertyChanged(nameof(HasPreviousPage));
            OnPropertyChanged(nameof(HasNextPage));
        }

        private void UpdatePriceTable()
        {
            PriceTableRows.Clear();

            bool hasAddCard = CurrentPageServices.Any(s => s.IsAddCard);
            IsTableVisible = !hasAddCard;

            if (!IsTableVisible)
            {
                OnPropertyChanged(nameof(PriceTableRows));
                return;
            }

            CurrentServiceName1 = "";
            CurrentServiceName2 = "";
            CurrentServiceName3 = "";

            for (int i = 0; i < CurrentPageServices.Count; i++)
            {
                if (i == 0 && !CurrentPageServices[i].IsAddCard)
                    CurrentServiceName1 = CurrentPageServices[i].Name;
                if (i == 1 && !CurrentPageServices[i].IsAddCard)
                    CurrentServiceName2 = CurrentPageServices[i].Name;
                if (i == 2 && !CurrentPageServices[i].IsAddCard)
                    CurrentServiceName3 = CurrentPageServices[i].Name;
            }

            if (CurrentPageServices.Count >= 3 && !CurrentPageServices[0].IsAddCard)
            {
                LoadPricesFromDatabase();
            }

            OnPropertyChanged(nameof(PriceTableRows));
        }

        private void LoadPricesFromDatabase()
        {
            try
            {
                var serviceIds = CurrentPageServices
                    .Where(s => !s.IsAddCard)
                    .Take(3)
                    .Select(s => s.ServiceId)
                    .ToList();

                // Загружаем услуги с ценами по весовым категориям
                var services = _context.Services
                    .Include(s => s.ServicePrices.Select(sp => sp.WeightCategories))
                    .Where(s => serviceIds.Contains(s.id_service))
                    .ToList();

                if (services.Count >= 3)
                {
                    var priceList1 = GetWeightCategoryPrices(services[0]);
                    var priceList2 = GetWeightCategoryPrices(services[1]);
                    var priceList3 = GetWeightCategoryPrices(services[2]);

                    // Используем стандартные весовые категории для отображения
                    var weightCategories = new[]
                    {
                        "<2,5 кг",
                        "2,6-5,5 кг",
                        "5,6-10,5 кг",
                        ">10,6 кг"
                    };

                    // Отображаем первые 4 весовые категории
                    int categoriesToShow = Math.Min(weightCategories.Length, 4);

                    for (int i = 0; i < categoriesToShow; i++)
                    {
                        PriceTableRows.Add(new PriceTableRow
                        {
                            Weight = weightCategories[i],
                            Price1 = i < priceList1.Count ? $"{priceList1[i]:N0} ₽" : "-",
                            Price2 = i < priceList2.Count ? $"{priceList2[i]:N0} ₽" : "-",
                            Price3 = i < priceList3.Count ? $"{priceList3[i]:N0} ₽" : "-"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки цен: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Не показываем тестовые данные - таблица останется пустой
                Debug.WriteLine($"Ошибка загрузки цен: {ex.Message}");
            }
        }

        private List<decimal> GetWeightCategoryPrices(Services service)
        {
            try
            {
                var prices = new List<decimal>();

                // Проверяем, есть ли связанные цены в ServicePrices
                if (service.ServicePrices != null && service.ServicePrices.Any())
                {
                    Debug.WriteLine($"Для услуги '{service.name_service}' найдено {service.ServicePrices.Count} цен");

                    // Сортируем по весовым категориям (по возрастанию минимального веса)
                    var sortedPrices = service.ServicePrices
                        .Where(sp => sp.price.HasValue && sp.WeightCategories != null)
                        .OrderBy(sp => sp.WeightCategories.min_weight)
                        .Select(sp => Math.Round(sp.price.Value, 0)) // Округляем до целых
                        .ToList();

                    if (sortedPrices.Any())
                    {
                        Debug.WriteLine($"Отсортированные цены: {string.Join(", ", sortedPrices)}");

                        // Если есть меньше 4 цен, дополняем
                        while (sortedPrices.Count < 4)
                        {
                            decimal lastPrice = sortedPrices.LastOrDefault();
                            sortedPrices.Add(Math.Round(lastPrice * 1.2m, 0));
                        }

                        return sortedPrices.Take(4).ToList();
                    }
                }

                // Если не удалось получить цены из ServicePrices, создаем на основе базовой цены
                decimal basePrice = Math.Round(service.price, 0); // Округляем базовую цену
                var defaultPrices = new List<decimal>
                {
                    Math.Round(basePrice * 0.9m, 0),    // <2,5 кг
                    basePrice,                          // 2,6-5,5 кг
                    Math.Round(basePrice * 1.2m, 0),    // 5,6-10,5 кг
                    Math.Round(basePrice * 1.5m, 0)     // >10,6 кг
                };

                Debug.WriteLine($"Используются цены по умолчанию для услуги '{service.name_service}': {string.Join(", ", defaultPrices)}");
                return defaultPrices;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в GetWeightCategoryPrices: {ex.Message}");
                decimal basePrice = Math.Round(service.price, 0);
                return new List<decimal> {
                    basePrice,
                    Math.Round(basePrice * 1.2m, 0),
                    Math.Round(basePrice * 1.5m, 0),
                    Math.Round(basePrice * 2m, 0)
                };
            }
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                LoadCurrentPage();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < TotalPages - 1)
            {
                _currentPage++;
                LoadCurrentPage();
            }
        }

        private void AddPriceCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                NavigationService?.Navigate(new AddPricePage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перехода: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            LoadServicesFromDatabase();
            _currentPage = 0;
            LoadCurrentPage();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}