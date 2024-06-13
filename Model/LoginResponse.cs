using System.Text.Json.Serialization;
using TestTask.API.Model.Entities;

namespace TestTask.API.Model
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] 
        public string RefreshToken { get; set; }


        public LoginResponse(AuthenticateUserResponseModel user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.UserName;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
