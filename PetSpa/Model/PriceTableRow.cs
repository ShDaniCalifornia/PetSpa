using System.ComponentModel;

namespace PetSpa.Model
{
    public class PriceTableRow : INotifyPropertyChanged
    {
        private string _weight;
        private string _price1;
        private string _price2;
        private string _price3;

        public string Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        public string Price1
        {
            get => _price1;
            set
            {
                _price1 = value;
                OnPropertyChanged(nameof(Price1));
            }
        }

        public string Price2
        {
            get => _price2;
            set
            {
                _price2 = value;
                OnPropertyChanged(nameof(Price2));
            }
        }

        public string Price3
        {
            get => _price3;
            set
            {
                _price3 = value;
                OnPropertyChanged(nameof(Price3));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}