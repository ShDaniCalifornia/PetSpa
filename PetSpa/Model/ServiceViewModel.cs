using PetSpa.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PetSpa.ViewModel
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        private Services _serviceEntity;
        private int _serviceId;
        private string _name;
        private string _description;
        private string _photoUrl;
        private decimal _price;
        private bool _isAddCard;
        private List<string> _descriptionPoints;

        // Конструктор для обычной услуги из БД
        public ServiceViewModel(Services serviceEntity)
        {
            _serviceEntity = serviceEntity;
            if (serviceEntity != null)
            {
                _serviceId = serviceEntity.id_service;
                _name = serviceEntity.name_service;
                _description = serviceEntity.description ?? "";
                _price = Math.Round(serviceEntity.price, 0);
                _photoUrl = ConvertPhotoPathToUri(serviceEntity.photo);

                // Инициализируем список пунктов описания
                UpdateDescriptionPoints();

                Debug.WriteLine($"ServiceViewModel создан: Name={_name}");
                Debug.WriteLine($"Description: {_description}");
                Debug.WriteLine($"DescriptionPoints count: {_descriptionPoints.Count}");
                foreach (var point in _descriptionPoints)
                {
                    Debug.WriteLine($"  - {point}");
                }
            }
            _isAddCard = false;
        }

        // Конструктор для карточки "Добавить прайс"
        public ServiceViewModel()
        {
            _isAddCard = true;
            _name = "Добавить прайс";
            _description = "";
            _descriptionPoints = new List<string>();
            _price = 0m;
            _serviceId = -1;
        }

        // Обновляет список пунктов описания
        private void UpdateDescriptionPoints()
        {
            if (string.IsNullOrEmpty(_description))
            {
                _descriptionPoints = new List<string>();
                return;
            }

            // Разделяем описание на строки и убираем маркеры •
            _descriptionPoints = _description
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s.StartsWith("•") ? s.Substring(1).Trim() : s.Trim())
                .ToList();
        }

        // Преобразует локальный путь к файлу в URI для WPF Image
        private string ConvertPhotoPathToUri(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
                return null;

            if (photoPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                photoPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return photoPath;
            }

            if (File.Exists(photoPath))
            {
                return new Uri(photoPath).AbsoluteUri;
            }

            return null;
        }

        // Свойства
        public int ServiceId
        {
            get => _serviceId;
            set { _serviceId = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                UpdateDescriptionPoints(); // Обновляем список при изменении описания
                OnPropertyChanged();
                OnPropertyChanged(nameof(DescriptionPoints)); // Уведомляем об изменении DescriptionPoints
            }
        }

        // Свойство для привязки в XAML (ТОЛЬКО ДЛЯ ЧТЕНИЯ)
        public List<string> DescriptionPoints => _descriptionPoints;

        public string PhotoUrl
        {
            get => _photoUrl;
            set { _photoUrl = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = Math.Round(value, 0);
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedPrice));
                OnPropertyChanged(nameof(PriceFrom));
            }
        }

        public bool IsAddCard
        {
            get => _isAddCard;
            set
            {
                _isAddCard = value;
                OnPropertyChanged();
            }
        }

        // Форматированная цена БЕЗ КОПЕЕК
        public string FormattedPrice => $"от {Price:N0} ₽";

        // Для привязки в XAML
        public string PriceFrom => $"{Price:N0}";

        // Старый метод (можно удалить или оставить для обратной совместимости)
        public List<string> GetDescriptionPoints()
        {
            return DescriptionPoints;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}