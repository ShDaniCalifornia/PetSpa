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

        public bool IsAdmin => AuthorizationPage.currentUser?.id_role == 1;

        public MastersPage()
        {
            InitializeComponent();
            DataContext = this;
            MasterViewModels = new ObservableCollection<MasterViewModel>();
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
                    var experienceText = master.experience ?? "";
                    string experienceYears = "0";
                    foreach (var word in experienceText.Split(' ', '\n', '\r'))
                    {
                        if (int.TryParse(word, out int years))
                        {
                            experienceYears = years.ToString();
                            break;
                        }
                    }

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
                        Skills = skills,
                        IsAdmin = IsAdmin
                    };

                    MasterViewModels.Add(viewModel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

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

        private void DeleteMasterBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Реализовать удаление мастера
            MessageBox.Show("Функция удаления мастера в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class MasterViewModel
    {
        public int MasterId { get; set; }
        public string FullName { get; set; }
        public string Experience { get; set; }
        public string PhotoUrl { get; set; }
        public string SpecializationName { get; set; }
        public string[] Skills { get; set; }
        public bool IsAdmin { get; set; }

        public string SearchText =>
            $"{FullName} {Experience} {SpecializationName} {string.Join(" ", Skills)}";
    }
}