using Microsoft.Win32;
using PetSpa.Model;
using PetSpa.View.Windows;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    public partial class AddPricePage : Page
    {
        private readonly PetSpaEntities _context = App.context;
        private string _photoUrl;
        private const int MaxDescriptionPoints = 5;

        public class DescriptionPoint
        {
            public int Number { get; set; }
            public string Text { get; set; }
            public bool IsFirst => Number == 1;
            public string Placeholder => $"Пункт {Number}";
        }

        public ObservableCollection<DescriptionPoint> DescriptionPoints { get; set; }

        public AddPricePage()
        {
            InitializeComponent();
            InitializeDescriptionPoints();
            DataContext = this;
            SetPlaceholderColors();
        }

        private void InitializeDescriptionPoints()
        {
            DescriptionPoints = new ObservableCollection<DescriptionPoint>
            {
                new DescriptionPoint { Number = 1, Text = "" }
            };
            UpdateAddButtonVisibility();
        }

        private void SetPlaceholderColors()
        {
            TitleTb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            MinPriceTb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            PriceValue1Tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            PriceValue2Tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            PriceValue3Tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фото услуги"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Получаем путь к файлу
                    string filePath = openFileDialog.FileName;

                    // Для простоты будем сохранять путь к локальному файлу
                    // В реальном приложении нужно загрузить фото на сервер и получить URL
                    _photoUrl = filePath;

                    MessageBox.Show("Фото выбрано. В реальном приложении фото будет загружено на сервер.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки фото: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddPointButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DescriptionPoints.Count >= MaxDescriptionPoints)
                {
                    MessageBox.Show($"Можно добавить не более {MaxDescriptionPoints} пунктов описания",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int nextNumber = DescriptionPoints.Any()
                    ? DescriptionPoints.Max(p => p.Number) + 1
                    : 1;

                DescriptionPoints.Add(new DescriptionPoint
                {
                    Number = nextNumber,
                    Text = ""
                });

                UpdateAddButtonVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления пункта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateAddButtonVisibility()
        {
            AddPointButton.Visibility = DescriptionPoints.Count >= MaxDescriptionPoints
                ? Visibility.Collapsed
                : Visibility.Visible;

            MaxPointsMessage.Visibility = DescriptionPoints.Count >= MaxDescriptionPoints
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void RemovePointBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is DescriptionPoint point)
                {
                    if (DescriptionPoints.Count > 1)
                    {
                        DescriptionPoints.Remove(point);

                        int number = 1;
                        foreach (var p in DescriptionPoints.OrderBy(p => p.Number))
                        {
                            p.Number = number++;
                        }

                        UpdateAddButtonVisibility();
                    }
                    else
                    {
                        MessageBox.Show("Должен остаться хотя бы один пункт описания",
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления пункта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчики плейсхолдеров
        private void TitleTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Название услуги")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TitleTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Название услуги";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void MinPriceTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Минимальная цена")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void MinPriceTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Минимальная цена";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PriceValue1Tb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "2,6-5,5 кг")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PriceValue1Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "2,6-5,5 кг";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PriceValue2Tb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "5,6-10,5 кг")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PriceValue2Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "5,6-10,5 кг";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PriceValue3Tb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == ">10,6 кг")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PriceValue3Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = ">10,6 кг";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(TitleTb.Text) || TitleTb.Text == "Название услуги")
                {
                    MessageBox.Show("Введите название услуги", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    TitleTb.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(MinPriceTb.Text) || MinPriceTb.Text == "Минимальная цена" ||
                    !decimal.TryParse(MinPriceTb.Text, out decimal minPrice) || minPrice <= 0)
                {
                    MessageBox.Show("Введите корректную минимальную цену (число больше 0)",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    MinPriceTb.Focus();
                    return;
                }

                // Валидация весовых категорий
                if (string.IsNullOrWhiteSpace(PriceValue1Tb.Text) || PriceValue1Tb.Text == "2,6-5,5 кг" ||
                    !decimal.TryParse(PriceValue1Tb.Text, out decimal price1) || price1 <= 0)
                {
                    MessageBox.Show("Введите корректную цену для категории 2,6-5,5 кг",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    PriceValue1Tb.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PriceValue2Tb.Text) || PriceValue2Tb.Text == "5,6-10,5 кг" ||
                    !decimal.TryParse(PriceValue2Tb.Text, out decimal price2) || price2 <= 0)
                {
                    MessageBox.Show("Введите корректную цену для категории 5,6-10,5 кг",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    PriceValue2Tb.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PriceValue3Tb.Text) || PriceValue3Tb.Text == ">10,6 кг" ||
                    !decimal.TryParse(PriceValue3Tb.Text, out decimal price3) || price3 <= 0)
                {
                    MessageBox.Show("Введите корректную цену для категории >10,6 кг",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    PriceValue3Tb.Focus();
                    return;
                }

                var emptyPoints = DescriptionPoints
                    .Where(p => string.IsNullOrWhiteSpace(p.Text))
                    .ToList();
                if (emptyPoints.Any())
                {
                    MessageBox.Show($"Заполните пункт описания №{emptyPoints.First().Number}",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаем описание
                string description = string.Join("\n",
                    DescriptionPoints
                        .Where(p => !string.IsNullOrWhiteSpace(p.Text))
                        .Select(p => "• " + p.Text.Trim()));

                Debug.WriteLine($"Создаваемое описание: {description}");
                Debug.WriteLine($"Фото URL: {_photoUrl}");

                // Создаем новую услугу
                var newService = new Services
                {
                    name_service = TitleTb.Text.Trim(),
                    price = Math.Round(minPrice, 0), // Округляем до целых
                    description = description,
                    photo = _photoUrl // Сохраняем путь к файлу
                };

                Debug.WriteLine($"Новая услуга: Name={newService.name_service}, Price={newService.price}");

                // Сохраняем услугу
                _context.Services.Add(newService);
                _context.SaveChanges();

                // Сохраняем весовые категории
                SaveWeightCategories(newService.id_service,
                    Math.Round(price1, 0),
                    Math.Round(price2, 0),
                    Math.Round(price3, 0));

                MessageBox.Show("Услуга успешно добавлена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем страницу прайсов
                RefreshPricePage();
                NavigationService?.GoBack();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveWeightCategories(int serviceId, decimal price1, decimal price2, decimal price3)
        {
            try
            {
                // Округляем цены
                price1 = Math.Round(price1, 0);
                price2 = Math.Round(price2, 0);
                price3 = Math.Round(price3, 0);

                Debug.WriteLine($"Сохраняем весовые категории с ценами: {price1}, {price2}, {price3}");

                // 1. Категория 2,6-5,5 кг
                var category1 = _context.WeightCategories
                    .FirstOrDefault(w => w.min_weight == 2.6m && w.max_weight == 5.5m);

                if (category1 == null)
                {
                    category1 = new WeightCategories
                    {
                        min_weight = 2.6m,
                        max_weight = 5.5m,
                        description = "2,6-5,5 кг"
                    };
                    _context.WeightCategories.Add(category1);
                    _context.SaveChanges();
                    Debug.WriteLine($"Создана новая весовая категория: 2,6-5,5 кг");
                }

                // 2. Категория 5,6-10,5 кг
                var category2 = _context.WeightCategories
                    .FirstOrDefault(w => w.min_weight == 5.6m && w.max_weight == 10.5m);

                if (category2 == null)
                {
                    category2 = new WeightCategories
                    {
                        min_weight = 5.6m,
                        max_weight = 10.5m,
                        description = "5,6-10,5 кг"
                    };
                    _context.WeightCategories.Add(category2);
                    _context.SaveChanges();
                    Debug.WriteLine($"Создана новая весовая категория: 5,6-10,5 кг");
                }

                // 3. Категория >10,6 кг
                var category3 = _context.WeightCategories
                    .FirstOrDefault(w => w.min_weight == 10.6m && w.max_weight == null);

                if (category3 == null)
                {
                    category3 = new WeightCategories
                    {
                        min_weight = 10.6m,
                        max_weight = null,
                        description = ">10,6 кг"
                    };
                    _context.WeightCategories.Add(category3);
                    _context.SaveChanges();
                    Debug.WriteLine($"Создана новая весовая категория: >10,6 кг");
                }

                // Создаем связи между услугой и весовыми категориями
                var servicePrice1 = new ServicePrices
                {
                    id_service = serviceId,
                    id_weight_category = category1.id_weight_category,
                    price = price1
                };

                var servicePrice2 = new ServicePrices
                {
                    id_service = serviceId,
                    id_weight_category = category2.id_weight_category,
                    price = price2
                };

                var servicePrice3 = new ServicePrices
                {
                    id_service = serviceId,
                    id_weight_category = category3.id_weight_category,
                    price = price3
                };

                _context.ServicePrices.Add(servicePrice1);
                _context.ServicePrices.Add(servicePrice2);
                _context.ServicePrices.Add(servicePrice3);

                _context.SaveChanges();

                Debug.WriteLine($"Сохранено 3 весовые категории для услуги {serviceId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Не удалось сохранить весовые категории: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                // Добавляем информацию о ценах в описание услуги
                try
                {
                    var service = _context.Services.Find(serviceId);
                    if (service != null)
                    {
                        service.description += $"\n\nЦены по весовым категориям:\n" +
                                              $"• 2,6-5,5 кг: {price1}₽\n" +
                                              $"• 5,6-10,5 кг: {price2}₽\n" +
                                              $"• >10,6 кг: {price3}₽";

                        _context.SaveChanges();
                    }
                }
                catch (Exception innerEx)
                {
                    Debug.WriteLine($"Не удалось добавить цены в описание: {innerEx.Message}");
                }
            }
        }

        private void RefreshPricePage()
        {
            try
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    var frame = mainWindow.FindName("MainFrame") as Frame;
                    if (frame != null && frame.Content is PricePage pricePage)
                    {
                        pricePage.RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления страницы: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}