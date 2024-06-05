using AutoMapper;
using Identity.Domain.Authentication;
using Identity.Domain.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Identity.Data;
using Identity.Services.Log;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Services.Authentication
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityContext _context;

        public AuthenticationService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager,
            SignInManager<User> signInManager, IConfiguration configuration, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IdentityContext context, ILogService logService) : base(mapper, context, logService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _roleManager = roleManager;
        }

        public bool ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var result = false;
                var currentUser = GetUser();
                if (currentUser != null)
                {
                    result = _userManager.ChangePasswordAsync(currentUser, changePasswordViewModel.CurrentPassword, changePasswordViewModel.NewPassword)?.Result == IdentityResult.Success;
                }
                return result;
            });
        }

        public UserViewModel CreateUser(UserViewModel userViewModel)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var result = userViewModel;
                var userEntity = _mapper.Map<User>(userViewModel);
                userEntity.Id = Guid.NewGuid().ToString();
                var resultAdd = _userManager.CreateAsync(userEntity, userViewModel.Password)?.Result;
                if (resultAdd?.Succeeded ?? false)
                {
                    result = _mapper.Map<UserViewModel>(userEntity);
                    AddRolesToUser(userViewModel, userEntity);
                }
                return result;
            });
        }

        private void AddRolesToUser(UserViewModel userViewModel, User userEntity)
        {
            if (userViewModel?.Roles?.Any() ?? false)
            {
                userViewModel?.Roles?.ToList().ForEach(r =>
                {
                    var roleAddResult = _userManager.AddToRoleAsync(userEntity, r).Result;
                });
            }
        }


        public bool DeleteUser(string userId)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var result = false;
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    DeleteRolesFromUser(user);
                    result = _userManager.DeleteAsync(user)?.Result?.Succeeded ?? false;
                }
                return result;
            });
        }

        private void DeleteRolesFromUser(User userEntity)
        {
            var initialRoles = _userManager.GetRolesAsync(userEntity)?.Result;
            if (initialRoles?.Any() ?? false)
            {
                initialRoles.ToList().ForEach(ir =>
                {
                    var roleRemoveResult = _userManager.RemoveFromRoleAsync(userEntity, ir).Result;
                });
            }
        }

        public ICollection<UserViewModel> Get()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var userEntities = _userManager.Users.ToList();
                var result = _mapper.Map<ICollection<UserViewModel>>(userEntities);
                result.ToList().ForEach(u =>
                {
                    var userEntity = userEntities.FirstOrDefault(ue => ue.Id == u.Id);
                    if (userEntity != null)
                    {
                        u.Roles = _userManager.GetRolesAsync(userEntity)?.Result;
                    }
                });
                return result;
            });
        }

        public ICollection<RoleViewModel> GetRoles()
        {
            return ExecuteFaultHandledOperation(() => _mapper.Map<ICollection<RoleViewModel>>(_roleManager.Roles.ToList()));
        }

        public User GetUser()
        {
            return ExecuteFaultHandledOperation(() => _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User)?.Result);
        }

        public UserViewModel GetUserWithRole()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var userEntities = GetUser();
                var result = _mapper.Map<UserViewModel>(userEntities);
                result.Roles = _userManager.GetRolesAsync(userEntities)?.Result;
                return result;
            });
        }

        public bool IsAdmin()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var result = false;
                var user = GetUser();
                if (user != null)
                {
                    var roles = _userManager.GetRolesAsync(user)?.Result;
                    if (!roles.IsNullOrEmpty() && roles.Contains("Administrador"))
                    {
                        result = true;
                    }
                }
                return result;
            });
        }

        public LoginViewModel Login(LoginUser loginUser)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var result = _signInManager.PasswordSignInAsync(loginUser.UserName, loginUser.Password, false, false).Result;
                if (result.Succeeded)
                {
                    var appUser = _userManager.Users.FirstOrDefault(u => u.UserName == loginUser.UserName && u.Active);
                    if (appUser != null)
                    {
                        var token = GenerateJwtToken(appUser);
                        var loginViewModel = _mapper.Map<LoginViewModel>(appUser);
                        loginViewModel.Token = token;
                        loginViewModel.Roles = _userManager.GetRolesAsync(appUser)?.Result;
                        return loginViewModel;

                    }
                }
                return null;
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(Sectoken);
        }

        public void Logout()
        {
            ExecuteFaultHandledOperation(() =>
            {
                _signInManager.SignOutAsync();
            });
        }

        public bool RecoverPassword(RecoverPasswordViewModel recoverPasswordViewModel)
        {
            throw new NotImplementedException();
        }

        public bool ResetPassword(ResetTokenViewModel resetTokenViewModel)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var result = false;
                var user = _userManager.FindByEmailAsync(resetTokenViewModel.Email)?.Result;
                if (user != null)
                {
                    result = _userManager.ResetPasswordAsync(user, resetTokenViewModel.Token, resetTokenViewModel.Password)?.Result == IdentityResult.Success;
                }
                return result;
            });
        }

        public UserViewModel UpdateUser(UserViewModel userViewModel)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var result = userViewModel;
                var userEntity = _context.Users.FirstOrDefault(u => u.Id == userViewModel.Id);
                if (userEntity != null)
                {
                    userEntity = _mapper.Map(userViewModel, userEntity);
                    if (_userManager.UpdateAsync(userEntity)?.Result?.Succeeded ?? false)
                    {
                        DeleteRolesFromUser(userEntity);
                        AddRolesToUser(userViewModel, userEntity);
                        result = _mapper.Map<UserViewModel>(userEntity);
                    }
                }
                return result;
            });
        }
    }
}
