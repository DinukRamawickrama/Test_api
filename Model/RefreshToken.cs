namespace WebAPP.Model
{
    public class RefreshToken
    {
        public int ID { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;

        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
