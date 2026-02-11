using PetSpa.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PetSpa.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private Users _currentUser;

        public string FullName => _currentUser?.full_name ?? "Не указано";

        public string RoleName => _currentUser?.Role?.role1 ?? "Роль не определена";

        public string Phone => _currentUser?.phone ?? "Не указан";

        public string Email => _currentUser?.email ?? "Не указан";

        public ProfileViewModel(Users user)
        {
            _currentUser = user;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}