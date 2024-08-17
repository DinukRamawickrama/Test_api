namespace WebAPP.Model
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Store hashed password
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Default role
    }
}
