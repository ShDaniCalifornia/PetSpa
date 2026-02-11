using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PetSpa.Model
{
    public class EditClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Данные клиента
        private int _clientId;
        private string _fullName;
        private string _phone;
        private DateTime _birthDate;

        public int ClientId
        {
            get => _clientId;
            set { _clientId = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set { _birthDate = value; OnPropertyChanged(); }
        }

        // Данные питомца
        private int? _petId;
        private string _petName;
        private int _petWeight;
        private DateTime _petBirthday;
        private string _breed;
        private string _photo;

        public int? PetId
        {
            get => _petId;
            set { _petId = value; OnPropertyChanged(); }
        }

        public string PetName
        {
            get => _petName;
            set { _petName = value; OnPropertyChanged(); }
        }

        public int PetWeight
        {
            get => _petWeight;
            set { _petWeight = value; OnPropertyChanged(); }
        }

        public DateTime PetBirthday
        {
            get => _petBirthday;
            set { _petBirthday = value; OnPropertyChanged(); }
        }

        public string Breed
        {
            get => _breed;
            set { _breed = value; OnPropertyChanged(); }
        }

        public string Photo
        {
            get => _photo;
            set { _photo = value; OnPropertyChanged(); }
        }

        // Для ComboBox
        public List<Pets> PetsList { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}