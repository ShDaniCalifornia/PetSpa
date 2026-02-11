using PetSpa.AppData;
using PetSpa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetSpa.Views.Pages
{
    public partial class AddClientPetPage : Page
    {
        private readonly PetSpaEntities _context = App.context;
        private List<SimplePet> _pets = new List<SimplePet>();
        private SimplePet _currentPet;

        public AddClientPetPage()
        {
            InitializeComponent();
            AddNewPet();
        }

        private void AddNewPet()
        {
            var newPet = new SimplePet();
            _pets.Add(newPet);
            _currentPet = newPet;
            UpdatePetsComboBox();
            ClearPetFields();
        }

        private void UpdatePetsComboBox()
        {
            PetsComboBox.Items.Clear();

            foreach (var pet in _pets)
            {
                string displayName = string.IsNullOrWhiteSpace(pet.Name) ? "Без имени" : pet.Name;
                PetsComboBox.Items.Add(displayName);
            }

            PetsComboBox.Items.Add("+ Добавить питомца");
            PetsComboBox.SelectedIndex = _pets.Count - 1;
        }

        private void ClearPetFields()
        {
            PetNameTb.Text = "Имя питомца";
            PetNameTb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));

            WeightTb.Text = "Вес";
            WeightTb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));

            PetBirthdayTb.Text = "Дата рождения";
            PetBirthdayTb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));

            PetBreedTb.Text = "Порода";
            PetBreedTb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
        }

        private void ShowCurrentPet()
        {
            if (_currentPet == null) return;

            // Имя
            PetNameTb.Text = string.IsNullOrWhiteSpace(_currentPet.Name) ?
                "Имя питомца" : _currentPet.Name;
            PetNameTb.Foreground = string.IsNullOrWhiteSpace(_currentPet.Name) ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D")) : Brushes.Black;

            // Вес
            WeightTb.Text = _currentPet.Weight <= 0 ?
                "Вес" : _currentPet.Weight.ToString();
            WeightTb.Foreground = _currentPet.Weight <= 0 ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D")) : Brushes.Black;

            // Дата рождения
            PetBirthdayTb.Text = _currentPet.Birthday == DateTime.MinValue ?
                "Дата рождения" : _currentPet.Birthday.ToString("dd.MM.yyyy");
            PetBirthdayTb.Foreground = _currentPet.Birthday == DateTime.MinValue ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D")) : Brushes.Black;

            // Порода
            PetBreedTb.Text = string.IsNullOrWhiteSpace(_currentPet.Breed) ?
                "Порода" : _currentPet.Breed;
            PetBreedTb.Foreground = string.IsNullOrWhiteSpace(_currentPet.Breed) ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D")) : Brushes.Black;
        }

        // Сохраняем данные из полей в текущего питомца
        private void SaveCurrentPetFromFields()
        {
            if (_currentPet == null) return;

            // Имя
            if (PetNameTb.Text != "Имя питомца")
                _currentPet.Name = PetNameTb.Text.Trim();

            // Вес
            if (WeightTb.Text != "Вес" && int.TryParse(WeightTb.Text, out int weight))
                _currentPet.Weight = weight > 0 ? weight : 5; // Не допускаем 0

            // Дата рождения
            if (PetBirthdayTb.Text != "Дата рождения" && DateTime.TryParse(PetBirthdayTb.Text, out DateTime birthday))
                _currentPet.Birthday = birthday;

            // Порода
            if (PetBreedTb.Text != "Порода")
                _currentPet.Breed = PetBreedTb.Text.Trim();
        }

        // Обработчики плейсхолдеров для клиента
        private void FCsTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "ФИО")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void FCsTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "ФИО";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PhoneTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Номер телефона")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PhoneTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Номер телефона";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void ClientBirthdayTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Дата рождения")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void ClientBirthdayTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Дата рождения";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        // Обработчики плейсхолдеров для питомца
        private void PetNameTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Имя питомца")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PetNameTb_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            SaveCurrentPetFromFields();
            UpdatePetsComboBox();

            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Имя питомца";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void WeightTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Вес")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void WeightTb_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveCurrentPetFromFields();

            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Вес";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PetBirthdayTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Дата рождения")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PetBirthdayTb_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveCurrentPetFromFields();

            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Дата рождения";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PetBreedTb_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Порода")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PetBreedTb_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveCurrentPetFromFields();

            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Порода";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DF6C2D"));
            }
        }

        private void PetsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PetsComboBox.SelectedItem == null) return;

            string selected = PetsComboBox.SelectedItem.ToString();

            if (selected == "+ Добавить питомца")
            {
                SaveCurrentPetFromFields();
                AddNewPet();
                PetNameTb.Focus();
            }
            else
            {
                SaveCurrentPetFromFields();

                int index = PetsComboBox.SelectedIndex;
                if (index >= 0 && index < _pets.Count)
                {
                    _currentPet = _pets[index];
                    ShowCurrentPet();
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            if (FCsTb.Text == "ФИО" || string.IsNullOrWhiteSpace(FCsTb.Text))
            {
                MessageBox.Show("Введите ФИО клиента");
                FCsTb.Focus();
                return;
            }

            if (PhoneTb.Text == "Номер телефона" || string.IsNullOrWhiteSpace(PhoneTb.Text))
            {
                MessageBox.Show("Введите номер телефона");
                PhoneTb.Focus();
                return;
            }

            SaveCurrentPetFromFields();


            var validPets = _pets.Where(p => !string.IsNullOrWhiteSpace(p.Name)).ToList();
            if (validPets.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одного питомца с именем");
                PetNameTb.Focus();
                return;
            }

            DateTime clientBirthday;
            if (ClientBirthdayTb.Text == "Дата рождения" ||
                !DateTime.TryParse(ClientBirthdayTb.Text, out clientBirthday))
            {
                clientBirthday = DateTime.Now.AddYears(-25);
            }

            string phone = PhoneTb.Text.Trim();
            if (phone == "Номер телефона" || string.IsNullOrWhiteSpace(phone))
            {
                phone = "не указан";
            }

            var newClient = new Clients
            {
                full_name = FCsTb.Text.Trim(),
                phone = phone,
                date_of_birth = clientBirthday,
                id_user = null
            };

            _context.Clients.Add(newClient);
            _context.SaveChanges();

            foreach (var pet in validPets)
            {
                int petWeight = pet.Weight > 0 ? pet.Weight : 5;

                DateTime petBirthday = pet.Birthday == DateTime.MinValue ?
                    DateTime.Now.AddYears(-1) : pet.Birthday;

                // Имя не может быть пустым
                string petName = string.IsNullOrWhiteSpace(pet.Name) ? "Без имени" : pet.Name.Trim();

                var newPet = new Pets
                {
                    name_pet = petName,
                    weight = petWeight,
                    birthday = petBirthday,
                    Breed = string.IsNullOrWhiteSpace(pet.Breed) ? null : pet.Breed.Trim(),
                    photo = null,
                    id_client = newClient.id_client
                };

                _context.Pets.Add(newPet);
            }

            _context.SaveChanges();

            MessageBox.Show("Клиент и питомцы сохранены!");

            ClassFrame.FrameBody.Navigate(new Views.Pages.ClientPage());
            ClassFrame.FramePanel.Navigate(new Views.Pages.PanelPage());
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.FrameBody.Navigate(new Views.Pages.ClientPage());
            ClassFrame.FramePanel.Navigate(new Views.Pages.PanelPage());
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPet == null || _pets.Count <= 1) return;

            if (MessageBox.Show("Удалить этого питомца?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int index = _pets.IndexOf(_currentPet);
                _pets.Remove(_currentPet);

                if (_pets.Count > 0)
                {
                    _currentPet = _pets[Math.Min(index, _pets.Count - 1)];
                }
                else
                {
                    _currentPet = null;
                }

                UpdatePetsComboBox();
                if (_currentPet != null)
                    ShowCurrentPet();
            }
        }

        private void AddPetBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentPetFromFields();
            AddNewPet();
            PetNameTb.Focus();
        }
    }

    public class SimplePet
    {
        private string _name = "";
        private int _weight = 5; // по умолчанию 5, не 0!
        private DateTime _birthday = DateTime.Now.AddYears(-1);
        private string _breed = "";

        public string Name
        {
            get => _name;
            set => _name = value?.Trim() ?? "";
        }

        public int Weight
        {
            get => _weight;
            set => _weight = value > 0 ? value : 5; // Гарантия, что вес > 0
        }

        public DateTime Birthday
        {
            get => _birthday;
            set => _birthday = value == DateTime.MinValue ? DateTime.Now.AddYears(-1) : value;
        }

        public string Breed
        {
            get => _breed;
            set => _breed = value?.Trim() ?? "";
        }
    }
}