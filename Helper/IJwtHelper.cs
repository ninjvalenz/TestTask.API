using TestTask.API.Model;

namespace TestTask.API.Helper
{
    public interface IJwtHelper
    {
        public string GenerateJwtToken(int userId);
        public int? ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress, int userId);
    }
}