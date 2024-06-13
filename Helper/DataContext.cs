using Dapper;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using System.Data;
using TestTask.API.Model;
using TestTask.API.Model.Entities;

namespace TestTask.API.Helper
{
    public class DataContext
    {
        private readonly IDbConnection _dbConnection;

        public DataContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IDbConnection Connection => _dbConnection;

        public bool DeleteUser(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@isDeleted", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            _dbConnection.Execute("DeleteUser", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@isDeleted");
        }

        public List<GetAllCompanyResponseModel> GetAllByCompany(int companyId, bool isAdmin)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@companyId", companyId);
            parameters.Add("@isAdmin", isAdmin);

            var users = _dbConnection.Query<GetAllCompanyResponseModel>("GetAllByCompany", commandType: CommandType.StoredProcedure);
            return users.Any() ? users.ToList() : new List<GetAllCompanyResponseModel>();
        }

        public void InsertUser(InsertUserRequestModel user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@firstName", user.FirstName);
            parameters.Add("@lastName", user.LastName);
            parameters.Add("@userName", user.UserName);
            parameters.Add("@passwordHash", user.PasswordHash);
            parameters.Add("@isAdmin", user.IsAdmin);
            parameters.Add("@companyId", user.CompanyId);

            _dbConnection.Execute("InsertUser", parameters, commandType: CommandType.StoredProcedure);
        }

        public bool IsExistingUser(string firstName, string lastName, string userName)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@firstName", firstName);
            parameters.Add("@lastName", lastName);
            parameters.Add("@userName", userName);

            return _dbConnection.QuerySingleOrDefault<bool>("IsExistingUser", commandType: CommandType.StoredProcedure);
        }

        public bool IsTokenUnique(string token)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@token", token);

            return _dbConnection.QuerySingleOrDefault<bool>("IsTokenUnique", commandType: CommandType.StoredProcedure);
        }
        
        public bool UpdateUser(UpdateUserRequestModel user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", user.UserId);
            parameters.Add("@firstName", user.FirstName);
            parameters.Add("@lastName", user.LastName);
            parameters.Add("@isAdmin", user.IsAdmin);
            parameters.Add("@passwordHash", user.PasswordHash);
            parameters.Add("@isUpdateSuccess", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            _dbConnection.Execute("UpdateUser", parameters, commandType: CommandType.StoredProcedure);
         
            return parameters.Get<bool>("@isUpdateSuccess");
        }

        public GetUserByIdResponseModel? GetUserById(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);

            var user = _dbConnection.QueryFirstOrDefault<GetUserByIdResponseModel>("GetUserById", parameters, commandType: CommandType.StoredProcedure);
            return user != null ? user : null;
        }

        public AuthenticateUserResponseModel? AuthenticateUser(string userName, string password)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userName", userName);
            parameters.Add("@password", BCrypt.Net.BCrypt.HashPassword(password));

            var user = _dbConnection.QueryFirstOrDefault<AuthenticateUserResponseModel>("AuthenticateUser", commandType: CommandType.StoredProcedure);
            return user != null ? user : null;
        }

        public void InsertRefreshToken(InsertRefreshTokenRequestModel refreshToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@token", refreshToken.Token);
            parameters.Add("@userId", refreshToken.UserId);
            parameters.Add("@expiryDate", refreshToken.ExpiryDate);
            parameters.Add("@createdDate", refreshToken.CreatedDate);
            parameters.Add("@createdByIp", refreshToken.CreatedByIp);

            _dbConnection.Execute("InsertRefreshToken", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
