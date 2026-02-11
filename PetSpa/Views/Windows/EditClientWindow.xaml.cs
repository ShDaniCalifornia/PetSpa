using PetSpa.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetSpa.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditClientWindow.xaml
    /// </summary>
    public partial class EditClientWindow : Window
    {
        private readonly PetSpaEntities _context = App.context;
        private readonly int _clientId;
        private EditClientViewModel _viewModel;

        public EditClientWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadData();
        }

        private void LoadData()
        {
            // Загружаем клиента
            var client = _context.Clients
                .Include("Pets")
                .FirstOrDefault(c => c.id_client == _clientId);

            if (client == null)
            {
                MessageBox.Show("Клиент не найден!");
                this.Close();
                return;
            }

            // Создаем ViewModel
            _viewModel = new EditClientViewModel
            {
                ClientId = client.id_client,
                FullName = client.full_name ?? "",
                Phone = client.phone ?? "",
                BirthDate = client.date_of_birth,
                PetsList = client.Pets?.ToList() ?? new System.Collections.Generic.List<Pets>()
            };

            // Заполняем ComboBox питомцами
            PetsComboBox.Items.Clear();
            foreach (var pet in _viewModel.PetsList)
            {
                PetsComboBox.Items.Add(pet.name_pet ?? "Без имени");
            }

            // Если есть питомцы, загружаем первого
            if (_viewModel.PetsList.Any())
            {
                PetsComboBox.SelectedIndex = 0;
                LoadSelectedPet(0);
            }

            // Устанавливаем DataContext для привязки
            this.DataContext = _viewModel;
        }

        private void LoadSelectedPet(int index)
        {
            if (index >= 0 && index < _viewModel.PetsList.Count)
            {
                var pet = _viewModel.PetsList[index];
                _viewModel.PetId = pet.id_pet;
                _viewModel.PetName = pet.name_pet ?? "";
                _viewModel.PetWeight = pet.weight;
                _viewModel.PetBirthday = pet.birthday;
                _viewModel.Breed = pet.Breed ?? "";
                _viewModel.Photo = pet.photo ?? "";
            }
        }

        private void PetsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PetsComboBox.SelectedIndex >= 0)
            {
                LoadSelectedPet(PetsComboBox.SelectedIndex);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Находим клиента в БД
            var client = _context.Clients.Find(_clientId);
            if (client == null) return;

            // Обновляем данные клиента
            client.full_name = _viewModel.FullName;
            client.phone = _viewModel.Phone;
            client.date_of_birth = _viewModel.BirthDate;

            // Обновляем данные текущего питомца
            if (_viewModel.PetId.HasValue)
            {
                var pet = _context.Pets.Find(_viewModel.PetId.Value);
                if (pet != null)
                {
                    pet.name_pet = _viewModel.PetName;
                    pet.weight = _viewModel.PetWeight;
                    pet.birthday = _viewModel.PetBirthday;
                    pet.Breed = _viewModel.Breed;
                    pet.photo = _viewModel.Photo;
                }
            }

            // Сохраняем изменения
            _context.SaveChanges();

            MessageBox.Show("Данные сохранены!");
            this.DialogResult = true;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, есть ли выбранный питомец для удаления
            if (_viewModel == null || !_viewModel.PetId.HasValue || PetsComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Нет питомца для удаления!", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Получаем данные о питомце для сообщения
            var petName = _viewModel.PetName;
            if (string.IsNullOrWhiteSpace(petName))
                petName = "питомец без имени";

            // Открываем окно подтверждения удаления
            var warningWindow = new WarningDeletePetWindow();

            // Можно передать имя питомца через DataContext, если хотите его отображать
            // Создаем простой объект с данными
            var petInfo = new { Name = petName };
            warningWindow.DataContext = petInfo;

            // Устанавливаем владельца для центрирования
            warningWindow.Owner = this;

            // Показываем окно модально
            bool? result = warningWindow.ShowDialog();

            // Если пользователь подтвердил удаление
            if (result == true)
            {
                // Находим питомца в БД
                var petToDelete = _context.Pets.Find(_viewModel.PetId.Value);
                if (petToDelete != null)
                {
                    // Удаляем питомца
                    _context.Pets.Remove(petToDelete);
                    _context.SaveChanges();

                    // Удаляем из списка в ViewModel
                    _viewModel.PetsList.RemoveAt(PetsComboBox.SelectedIndex);

                    // Обновляем ComboBox
                    PetsComboBox.Items.RemoveAt(PetsComboBox.SelectedIndex);

                    // Если остались питомцы, выбираем первого
                    if (_viewModel.PetsList.Any())
                    {
                        PetsComboBox.SelectedIndex = 0;
                        LoadSelectedPet(0);
                    }
                    else
                    {
                        // Если питомцев не осталось, очищаем поля
                        ClearPetFields();
                    }

                    MessageBox.Show($"Питомец {petName} удален",
                        "Успешно",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                }
            }
        }

        private void ClearPetFields()
        {
            _viewModel.PetId = null;
            _viewModel.PetName = "";
            _viewModel.PetWeight = 0;
            _viewModel.PetBirthday = DateTime.Now;
            _viewModel.Breed = "";
            _viewModel.Photo = "";
        }

        private void AddPetBtn_Click(object sender, RoutedEventArgs e)
        {
            var addPetWindow = new AddPetWindow(_clientId);
            addPetWindow.Owner = this;
            addPetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            bool? result = addPetWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }
    }
}