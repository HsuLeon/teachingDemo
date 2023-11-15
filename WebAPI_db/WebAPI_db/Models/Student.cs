namespace WebAPI_db.Models
{
    public class Student
    {
        public string? Account { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public string? Note { get; set; }
        public bool Deleted { get; set; }
    }
}
