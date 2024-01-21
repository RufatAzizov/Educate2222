using EdukateMvcb.Models.Common;

namespace EdukateMvcb.Models
{
    public class Slider :BaseEntity
    {
        public string? ImageUrl { get; set; }
        public string Instructor { get; set; }
        public string Profession { get; set; }
    }
}
