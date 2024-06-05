using Identity.Domain.Authentication;
using Identity.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services.Authentication
{
    public interface IAuthenticationService
    {
        LoginViewModel Login(LoginUser loginUser);

        User GetUser();

        void Logout();

        ICollection<UserViewModel> Get();

        bool DeleteUser(string userId);

        ICollection<RoleViewModel> GetRoles();

        UserViewModel UpdateUser(UserViewModel userViewModel);

        UserViewModel CreateUser(UserViewModel userViewModel);

        bool ChangePassword(ChangePasswordViewModel changePasswordViewModel);

        bool RecoverPassword(RecoverPasswordViewModel recoverPasswordViewModel);

        bool ResetPassword(ResetTokenViewModel resetTokenViewModel);

        UserViewModel GetUserWithRole();

        bool IsAdmin();
    }
}
