namespace PetSpa.Model
{
    public class MasterViewModel
    {
        public int MasterId { get; set; }
        public string FullName { get; set; }
        public string Experience { get; set; }
        public string PhotoUrl { get; set; }
        public string SpecializationName { get; set; }
        public string[] Skills { get; set; } // Массив навыков/достижений

        // Для поиска
        public string SearchText =>
            $"{FullName} {Experience} {SpecializationName} {string.Join(" ", Skills)}";
    }
}
