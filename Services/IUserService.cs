using TestTask.API.Model;
using TestTask.API.Model.Entities;

namespace TestTask.API.Services
{
    public interface IUserService
    {
        LoginResponse Authenticate(LoginRequest model, string ipAddress);
        bool DeleteUser(int userId);
        List<GetAllCompanyResponseModel> GetAllByCompany(int companyId, bool isAdmin);
        void InsertUser(InsertUserRequestModel user);
        bool IsExistingUser(string firstName, string lastName, string userName);
        bool IsTokenUnique(string token);
        bool UpdateUser(UpdateUserRequestModel user);
        GetUserByIdResponseModel? GetUserById(int userId);
    }
}
