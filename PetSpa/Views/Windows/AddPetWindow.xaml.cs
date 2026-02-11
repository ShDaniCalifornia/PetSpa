using PetSpa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetSpa.Views.Windows
{
    public partial class AddPetWindow : Window
    {
        private readonly PetSpaEntities _context = App.context;
        private readonly int _clientId;
        private List<PetData> _pets = new List<PetData>();
        private PetData _selectedPet;

        public AddPetWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadExistingPets();
            AddNewPet();

            // Привязываем обработчики событий
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Привязываем общие обработчики ко всем полям
            AttachEventHandlers();
        }

        private void LoadExistingPets()
        {
            // Загружаем существующих питомцев клиента из базы
            var existingPets = _context.Pets
                .Where(p => p.id_client == _clientId)
                .ToList();

            foreach (var pet in existingPets)
            {
                var petData = new PetData
                {
                    Id = pet.id_pet,
                    Name = pet.name_pet ?? "",
                    Weight = pet.weight,
                    Birthday = pet.birthday,
                    Breed = pet.Breed ?? ""
                };
                _pets.Add(petData);
            }
        }

        private void AttachEventHandlers()
        {
            // Поля питомца
            AttachPlaceholderHandler(PetName3Tb, "Имя питомца");
            AttachPlaceholderHandler(Weight2Tb, "Вес");
            AttachPlaceholderHandler(PetBirthday2Tb, "Дата рождения");
            AttachPlaceholderHandler(PetBreed2Tb, "Порода");
        }

        private void AttachPlaceholderHandler(TextBox textBox, string placeholder)
        {
            // Сохраняем плейсхолдер в Tag
            textBox.Tag = placeholder;

            // Устанавливаем оранжевый цвет для плейсхолдера
            var orangeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            textBox.Foreground = orangeColor;

            // Обработчики событий
            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.Foreground = orangeColor;
                }
                else
                {
                    // Обновляем данные питомца
                    UpdatePetDataFromTextBox(textBox);
                }
            };
        }

        private void UpdatePetDataFromTextBox(TextBox textBox)
        {
            if (_selectedPet == null) return;

            switch (textBox.Name)
            {
                case "PetName3Tb":
                    _selectedPet.Name = textBox.Text.Trim();
                    UpdatePetsComboBox();
                    break;

                case "Weight2Tb":
                    if (int.TryParse(textBox.Text, out int weight))
                        _selectedPet.Weight = weight;
                    break;

                case "PetBirthday2Tb":
                    if (DateTime.TryParse(textBox.Text, out DateTime birthday))
                        _selectedPet.Birthday = birthday;
                    break;

                case "PetBreed2Tb":
                    _selectedPet.Breed = textBox.Text.Trim();
                    break;
            }
        }

        private void AddNewPet()
        {
            var newPet = new PetData
            {
                Id = null, // Новый питомец, еще нет ID
                Name = "",
                Weight = 0,
                Birthday = DateTime.Now,
                Breed = ""
            };

            _pets.Add(newPet);
            _selectedPet = newPet;
            UpdatePetsComboBox();
        }

        private void UpdatePetsComboBox()
        {
            PetsComboBox.Items.Clear();

            foreach (var pet in _pets)
            {
                PetsComboBox.Items.Add(pet.DisplayName);
            }

            PetsComboBox.Items.Add("+ Добавить питомца");

            if (_selectedPet != null)
            {
                int index = _pets.IndexOf(_selectedPet);
                if (index >= 0)
                {
                    PetsComboBox.SelectedIndex = index;
                }
            }
        }

        private void UpdatePetFields()
        {
            if (_selectedPet != null)
            {
                var orangeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));

                // Имя питомца
                if (string.IsNullOrWhiteSpace(_selectedPet.Name))
                {
                    PetName3Tb.Text = "Имя питомца";
                    PetName3Tb.Foreground = orangeColor;
                }
                else
                {
                    PetName3Tb.Text = _selectedPet.Name;
                    PetName3Tb.Foreground = Brushes.Black;
                }

                // Вес
                if (_selectedPet.Weight <= 0)
                {
                    Weight2Tb.Text = "Вес";
                    Weight2Tb.Foreground = orangeColor;
                }
                else
                {
                    Weight2Tb.Text = _selectedPet.Weight.ToString();
                    Weight2Tb.Foreground = Brushes.Black;
                }

                // Дата рождения
                if (_selectedPet.Birthday == default)
                {
                    PetBirthday2Tb.Text = "Дата рождения";
                    PetBirthday2Tb.Foreground = orangeColor;
                }
                else
                {
                    PetBirthday2Tb.Text = _selectedPet.Birthday.ToString("dd.MM.yyyy");
                    PetBirthday2Tb.Foreground = Brushes.Black;
                }

                // Порода
                if (string.IsNullOrWhiteSpace(_selectedPet.Breed))
                {
                    PetBreed2Tb.Text = "Порода";
                    PetBreed2Tb.Foreground = orangeColor;
                }
                else
                {
                    PetBreed2Tb.Text = _selectedPet.Breed;
                    PetBreed2Tb.Foreground = Brushes.Black;
                }
            }
        }

        private void PetsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PetsComboBox.SelectedItem == null) return;

            string selectedItem = PetsComboBox.SelectedItem.ToString();

            if (selectedItem == "+ Добавить питомца")
            {
                AddNewPet();
                PetName3Tb.Focus();
            }
            else
            {
                int index = PetsComboBox.SelectedIndex;
                if (index >= 0 && index < _pets.Count)
                {
                    _selectedPet = _pets[index];
                    UpdatePetFields();
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            bool hasChanges = false;

            using (var context = new PetSpaEntities())
            {
                foreach (var petData in _pets)
                {
                    if (petData.Id.HasValue)
                    {
                        var existingPet = context.Pets.Find(petData.Id.Value);
                        if (existingPet != null)
                        {
                            existingPet.name_pet = string.IsNullOrWhiteSpace(petData.Name)
                                ? "Без имени"
                                : petData.Name.Trim();

                            existingPet.weight = petData.Weight > 0 ? petData.Weight : 5;

                            if (petData.Birthday == default)
                                existingPet.birthday = DateTime.Now.AddYears(-1);
                            else
                                existingPet.birthday = petData.Birthday;

                            existingPet.Breed = string.IsNullOrWhiteSpace(petData.Breed)
                                ? null
                                : petData.Breed.Trim();

                            hasChanges = true;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(petData.Name) && petData.Name != "Имя питомца")
                    {
                        // Добавляем нового питомца
                        var newPet = new Pets
                        {
                            name_pet = petData.Name.Trim(),
                            weight = petData.Weight > 0 ? petData.Weight : 5,
                            birthday = petData.Birthday == default
                                ? DateTime.Now.AddYears(-1)
                                : petData.Birthday,
                            Breed = string.IsNullOrWhiteSpace(petData.Breed) ? null : petData.Breed.Trim(),
                            photo = null,
                            id_client = _clientId
                        };

                        context.Pets.Add(newPet);
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    context.SaveChanges();
                    MessageBox.Show("Данные сохранены!");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Нет изменений");
                }

            }
        }


        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPet == null || _pets.Count <= 1)
            {
                MessageBox.Show("Нельзя удалить последнего питомца", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string petName = string.IsNullOrEmpty(_selectedPet.Name)
                ? "питомца без имени"
                : $"питомца '{_selectedPet.Name}'";

            var result = MessageBox.Show($"Удалить {petName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Если питомец уже сохранен в БД, удаляем его
                if (_selectedPet.Id.HasValue)
                {
                    var petToDelete = _context.Pets.Find(_selectedPet.Id.Value);
                    if (petToDelete != null)
                    {
                        _context.Pets.Remove(petToDelete);
                    }
                }

                _pets.Remove(_selectedPet);
                _selectedPet = _pets.FirstOrDefault();
                UpdatePetsComboBox();
                UpdatePetFields();
            }
        }

        private void AddPetBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewPet();
            PetName3Tb.Focus();
        }
    }

    public class PetData
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public DateTime Birthday { get; set; }
        public string Breed { get; set; }

        public string DisplayName => string.IsNullOrEmpty(Name) ? "Без имени" : Name;
    }
}