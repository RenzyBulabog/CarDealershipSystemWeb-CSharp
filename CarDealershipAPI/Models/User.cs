namespace CarDealershipAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string? Role { get; set; }        // 👈 add ?
        public string? Fullname { get; set; }    // 👈 add ?
        public string? Email { get; set; }       // 👈 add ?
    }
}