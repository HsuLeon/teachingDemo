namespace WebAPI_db.Models
{
    public class UserInfo
    {
        public string? Account { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public int Gender { get; set; } //0: female, 1: male
    }
}
