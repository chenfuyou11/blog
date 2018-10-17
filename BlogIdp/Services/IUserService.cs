using BlogIdp.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogIdp.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password);
        Task<bool> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword, Action<string, string> reportError);
        Task<ApplicationUser> GetAuthenticatedUserAsync(ClaimsPrincipal principal);
        Task<ApplicationUser> GetUserAsync(string userName);
        Task<ApplicationUser> GetForgotPasswordUserAsync(string userIdentifier);
        Task<bool> ResetPasswordAsync(string userIdentifier, string resetToken, string newPassword, Action<string, string> reportError);
    }
}
