using WebAPP.Model;
using Task = WebAPP.Model.Task;

namespace WebAPP.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string username, string password, string email);
        Task<User?> AuthenticateAsync(string username, string password);
        Task<RefreshToken> GenerateRefreshTokenAsync(int userId);
        string GenerateJwtToken(User user);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        
       
    }
}
