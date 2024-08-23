using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPP.Model;
using Task = WebAPP.Model.Task;

namespace WebAPP.Services
{
  
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(string username, string password, string email)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new Exception("Username already exists");

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Email = email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Constants.Issuer,
                audience: Constants.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = userId
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
          
            var tokens = _context.RefreshTokens
               .Where(r => r.Token == token && r.Expires > DateTime.UtcNow && r.IsExpired == false)
               .AsEnumerable()  
               .Where(r => r.IsActive)
               .ToList();
            return await _context.RefreshTokens.Include(rt => rt.User)
           .SingleOrDefaultAsync(rt => rt.Token == token && rt.IsActive);
        }

  

        private string HashPassword(string password)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("y123"));
            
            var Hash= BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();
            Console.Write(Hash);
            return Hash;
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("y123"));
            var computedHash = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();
            Console.Write(computedHash);
            return computedHash == storedHash;
        }
    }
}
