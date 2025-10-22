using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class TeacherSubject
    {
        [Key]
        public int TsId { get; set; }

        public int TId { get; set; }
        public Teacher? Teacher { get; set; }

        public int SuId { get; set; }
        public Subject? Subject { get; set; }
    }
}
