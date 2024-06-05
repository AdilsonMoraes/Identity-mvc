using Identity.API.Attributes;
using Identity.Domain.Authentication;
using Identity.Domain.ViewModel;
using Identity.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers.Authentication
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginUser loginUser)
        {
            return Ok(_authenticationService.Login(loginUser));
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _authenticationService.Logout();
            return Ok();
        }

        [ExtendedAuthorize("Administrador")]
        [HttpGet]
        public ICollection<UserViewModel> Users()
        {
            return _authenticationService.Get();
        }

        [ExtendedAuthorize("Administrador")]
        [HttpDelete("{userId}")]
        public bool DeleteUser(string userId)
        {
            return _authenticationService.DeleteUser(userId);
        }

        [ExtendedAuthorize("Administrador")]
        [HttpGet]
        public ICollection<RoleViewModel> Roles()
        {
            return _authenticationService.GetRoles();
        }

        [ExtendedAuthorize("Administrador")]
        [HttpPut]
        public UserViewModel UpdateUser([FromBody] UserViewModel userViewModel)
        {
            return _authenticationService.UpdateUser(userViewModel);
        }

        [ExtendedAuthorize("Administrador")]
        [HttpPost]
        public UserViewModel CreateUser([FromBody] UserViewModel userViewModel)
        {
            return _authenticationService.CreateUser(userViewModel);
        }

        [HttpPost]
        public bool ChangePassword([FromBody] ChangePasswordViewModel changePasswordViewModel)
        {
            return _authenticationService.ChangePassword(changePasswordViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public bool RecoverPassword([FromBody] RecoverPasswordViewModel recoverPasswordViewModel)
        {
            return _authenticationService.RecoverPassword(recoverPasswordViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public bool ResetPassword([FromBody] ResetTokenViewModel resetTokenViewModel)
        {
            return _authenticationService.ResetPassword(resetTokenViewModel);
        }
    }
}
