using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PetSpa.Model
{
    public class AppointmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _appointmentId;
        private DateTime _appointmentDate;
        private TimeSpan _appointmentTime;
        private string _clientFullName;
        private string _petInfo;
        private string _serviceName;
        private decimal _servicePrice;
        private string _masterFullName;
        private string _formattedDate;

        public int AppointmentId
        {
            get => _appointmentId;
            set { _appointmentId = value; OnPropertyChanged(); }
        }

        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set
            {
                _appointmentDate = value;
                OnPropertyChanged();
                UpdateFormattedDate();
            }
        }

        public TimeSpan AppointmentTime
        {
            get => _appointmentTime;
            set { _appointmentTime = value; OnPropertyChanged(); }
        }

        public string ClientFullName
        {
            get => _clientFullName;
            set { _clientFullName = value; OnPropertyChanged(); }
        }

        public string PetInfo
        {
            get => _petInfo;
            set { _petInfo = value; OnPropertyChanged(); }
        }

        public string ServiceName
        {
            get => _serviceName;
            set { _serviceName = value; OnPropertyChanged(); }
        }

        public decimal ServicePrice
        {
            get => _servicePrice;
            set { _servicePrice = value; OnPropertyChanged(); }
        }

        public string MasterFullName
        {
            get => _masterFullName;
            set { _masterFullName = value; OnPropertyChanged(); }
        }

        public string FormattedDate
        {
            get => _formattedDate;
            private set { _formattedDate = value; OnPropertyChanged(); }
        }

        // Полное ФИО клиента и питомец
        public string ClientPetInfo => $"{ClientFullName}, {PetInfo}";

        // Форматированное время
        public string FormattedTime => $"{AppointmentTime:hh\\:mm}";

        // Форматированная цена
        public string FormattedPrice => $"{ServicePrice:N0}";

        private void UpdateFormattedDate()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            if (_appointmentDate.Date == today)
            {
                FormattedDate = "Сегодня";
            }
            else if (_appointmentDate.Date == yesterday)
            {
                FormattedDate = "Вчера";
            }
            else
            {
                FormattedDate = _appointmentDate.ToString("dd.MM.yyyy");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}