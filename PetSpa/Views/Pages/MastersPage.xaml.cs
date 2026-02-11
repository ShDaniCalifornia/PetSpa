using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    public partial class MastersPage : Page
    {
        private Model.PetSpaEntities _context = App.context;
        public ObservableCollection<MasterViewModel> MasterViewModels { get; private set; }

        public MastersPage()
        {
            InitializeComponent();
            MasterViewModels = new ObservableCollection<MasterViewModel>();

            // Если используешь ItemsControl вместо ListBox
            MastersItemsControl.ItemsSource = MasterViewModels;

            LoadFromDatabase();
        }

        private void LoadFromDatabase()
        {
            try
            {
                var masters = _context.Masters
                    .Include("Specialization")
                    .ToList();

                MasterViewModels.Clear();

                foreach (var master in masters)
                {
                    // Разбираем experience как в макете
                    // Формат: "опыт X лет" и навыки каждый с новой строки
                    var experienceText = master.experience ?? "";

                    // Ищем количество лет опыта
                    string experienceYears = "0";
                    foreach (var word in experienceText.Split(' ', '\n', '\r'))
                    {
                        if (int.TryParse(word, out int years))
                        {
                            experienceYears = years.ToString();
                            break;
                        }
                    }

                    // Навыки - все строки после заголовков
                    var lines = experienceText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    var skills = lines.Where(line =>
                        !line.Contains("опыт") &&
                        !line.Contains("О мастере") &&
                        !line.Contains("О мастере:") &&
                        !string.IsNullOrWhiteSpace(line.Trim()))
                        .Select(line => line.Trim())
                        .ToArray();

                    var viewModel = new MasterViewModel
                    {
                        MasterId = master.id_master,
                        FullName = master.full_name?.Trim() ?? "",
                        Experience = experienceYears,
                        PhotoUrl = master.photo?.Trim() ?? "/Resources/Images/DefaultMaster.png",
                        SpecializationName = master.Specialization?.name_specialization?.Trim() ?? "животными",
                        Skills = skills
                    };

                    MasterViewModels.Add(viewModel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        // Класс ViewModel прямо в этом файле
        public class MasterViewModel
        {
            public int MasterId { get; set; }
            public string FullName { get; set; }
            public string Experience { get; set; }
            public string PhotoUrl { get; set; }
            public string SpecializationName { get; set; }
            public string[] Skills { get; set; }
        }

        // Остальные методы
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Поиск")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Поиск";
                SearchTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Реализуй поиск при необходимости
        }

    }
}