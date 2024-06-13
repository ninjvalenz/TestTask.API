using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestTask.API.Model;
using TestTask.API.Model.Entities;
using TestTask.API.Services;

namespace TestTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest model)
        {
            var response = _userService.Authenticate(model, ipAddress());
            if(response != null)
            {
                setTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            else return Unauthorized();
        }

        [Authorize]
        [HttpGet("ListAll")]
        public IActionResult ListAll()
        {
            var currentLoggedUser = HttpContext.Items["User"] as GetUserByIdResponseModel;
            var users = _userService.GetAllByCompany(currentLoggedUser.CompanyId, currentLoggedUser.IsAdmin);

            return Ok(users);
        }

        [Authorize]
        [HttpPost("CreateUser")]
        public IActionResult CreateUser(InsertUserRequestModel user)
        {
            var currentLoggedUser = HttpContext.Items["User"] as GetUserByIdResponseModel;
            if (currentLoggedUser.IsAdmin)
            {
                _userService.InsertUser(user);
                return Ok();
            }
            else return Unauthorized();
        }

        [Authorize]
        [HttpPost("UpdateUser")]
        public IActionResult UpdateUser(UpdateUserRequestModel user)
        {
            var currentLoggedUser = HttpContext.Items["User"] as GetUserByIdResponseModel;
            if (currentLoggedUser.IsAdmin)
            {
                _userService.UpdateUser(user);
                return Ok();
            }
            else return Unauthorized();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete(int userId)
        {
            var currentLoggedUser = HttpContext.Items["User"] as GetUserByIdResponseModel;
            if (currentLoggedUser.IsAdmin)
            {
                _userService.DeleteUser(userId);
                return Ok();
            }
            else return Unauthorized();
        }

        #region Private Methods
        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
        #endregion
    }
}
