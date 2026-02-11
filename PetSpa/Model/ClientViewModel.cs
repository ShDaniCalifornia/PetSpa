using System;

namespace PetSpa.Model
{

    public class ClientViewModel
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string FormattedPhone { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthDateText { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public string LastVisitText { get; set; }
        public string PetInfo { get; set; }
        public string PetWeight { get; set; }
        public DateTime? PetBirthDate { get; set; }
        public string PetAgeText { get; set; }
        public int ClientId { get; set; }
        public string PetPhotoUrl { get; set; }

        public string SearchText =>
            $"{FullName} {Phone} {PetInfo} {PetWeight} {PetAgeText} {FormattedPhone}";
    }
}