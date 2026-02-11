using PetSpa.Model;
using System.Windows;

namespace PetSpa
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static PetSpaEntities context = new PetSpaEntities();
    }
}
