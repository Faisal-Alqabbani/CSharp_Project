namespace C_Exam.Models
{
    public class Association
    {
        public int AssociationId { get; set; }
        public int UserId { get; set; }
        public int HobbyId { get; set; }
        public string Proficiency { get; set; }
        public User User { get; set; }
        public Hobby Hobby { get; set; }
    }
}