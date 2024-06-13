using TestTask.API.Helper;
using TestTask.API.Model;
using TestTask.API.Model.Entities;

namespace TestTask.API.Services
{
    public class UserService : IUserService
    {
        private DataContext _context;
        private IJwtHelper _jwtHelper;

        public UserService(DataContext dataContext, IJwtHelper jwtHelper) 
        {
            _context = dataContext;
            _jwtHelper = jwtHelper;
        }

        public LoginResponse? Authenticate(LoginRequest model, string ipAddress)
        {
            var user = _context.AuthenticateUser(model.Username, model.Password);
            if (user != null)
            {
                var jwtToken = _jwtHelper.GenerateJwtToken(user.Id);
                var refreshToken = _jwtHelper.GenerateRefreshToken(ipAddress, user.Id);
                if (jwtToken != null)
                {
                    _context.InsertRefreshToken(new Model.Entities.InsertRefreshTokenRequestModel()
                    {
                        UserId = refreshToken.Id,
                        Token = refreshToken.Token,
                        ExpiryDate = refreshToken.ExpiryDate, 
                        CreatedByIp = refreshToken.CreatedByIp,
                        CreatedDate = refreshToken.CreatedDate
                    });

                    return new LoginResponse(user, jwtToken, refreshToken.Token);
                }
            }

            return null;
        }

        public bool DeleteUser(int userId)
        {
            return _context.DeleteUser(userId);
        }

        public List<GetAllCompanyResponseModel> GetAllByCompany(int companyId, bool isAdmin)
        {
            return _context.GetAllByCompany(companyId, isAdmin);
        }

        public GetUserByIdResponseModel? GetUserById(int userId)
        {
            return _context.GetUserById(userId);
        }

        public void InsertUser(InsertUserRequestModel user)
        {
            _context.InsertUser(user);
        }

        public bool IsExistingUser(string firstName, string lastName, string userName)
        {
            return _context.IsExistingUser(firstName, lastName, userName);
        }

        public bool IsTokenUnique(string token)
        {
            return _context.IsTokenUnique(token);
        }

        public bool UpdateUser(UpdateUserRequestModel user)
        {
            return _context.UpdateUser(user);
        }
    }
}
