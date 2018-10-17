using BlogDemo.Core.Entities;
using BlogDemo.Core.Interfaces;
using BlogIdp.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogIdp.Services
{
    public class UserService:IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IStringLocalizer<UserService> _T;
        private readonly IOptions<IdentityOptions> _identityOptions;

        public UserService(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> identityOptions)
        {
            _userManager = userManager;
           // _T = T;
            _identityOptions = identityOptions;
        }

        public Task<bool> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password)
        {
            if (!(user is ApplicationUser newUser))
            {
                throw new ArgumentException("Expected a User instance.", nameof(user));
            }

            // Accounts can be created with no password
            var identityResult = String.IsNullOrWhiteSpace(password)
                ? await _userManager.CreateAsync(user)
                : await _userManager.CreateAsync(user, password);

            //identityResult = _userManager.AddClaimsAsync(user, new Claim[]{
            //            new Claim(JwtClaimTypes.Name, "wang fuyou"),
            //            new Claim(JwtClaimTypes.GivenName, "wang"),
            //            new Claim(JwtClaimTypes.FamilyName, "fuyou"),
            //            new Claim(JwtClaimTypes.Email, "wangfuyou@email.com"),
            //            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            //            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
            //            new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
            //        }).Result;
            if (!identityResult.Succeeded)
            {
                throw new Exception(identityResult.Errors.First().Description);
               // return null;
            }

 

            return user;
        }

        public Task<ApplicationUser> GetAuthenticatedUserAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetForgotPasswordUserAsync(string userIdentifier)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> GetUserAsync(string userName)
        {
             return await  _userManager.FindByNameAsync(userName);
        }

        public async Task<bool> ResetPasswordAsync(string userIdentifier, string resetToken, string newPassword, Action<string, string> reportError)
        {
            var result = true;
            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                reportError("UserName", "A user name or email is required.");
                result = false;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                reportError("Password", "A password is required.");
                result = false;
            }

            if (string.IsNullOrWhiteSpace(resetToken))
            {
                reportError("Token", "A token is required.");
                result = false;
            }

            if (!result)
            {
                return result;
            }

            var user = await FindByUsernameOrEmailAsync(userIdentifier) as ApplicationUser;

            if (user == null)
            {
                return false;
            }

            var identityResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!identityResult.Succeeded)
            {
                ProcessValidationErrors(identityResult.Errors, user, reportError);
            }

            return identityResult.Succeeded;
        }

        private async Task<ApplicationUser> FindByUsernameOrEmailAsync(string userIdentifier)
        {
            userIdentifier = userIdentifier.Normalize();

            var user = await _userManager.FindByNameAsync(userIdentifier);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userIdentifier);
            }

            return user;
        }


        public void ProcessValidationErrors(IEnumerable<IdentityError> errors, ApplicationUser user, Action<string, string> reportError)
        {
            foreach (var error in errors)
            {
                switch (error.Code)
                {
                    // Password
                    case "PasswordRequiresDigit":
                        reportError("Password", "Passwords must have at least one digit ('0'-'9').");
                        break;
                    case "PasswordRequiresLower":
                        reportError("Password", "Passwords must have at least one lowercase ('a'-'z').");
                        break;
                    case "PasswordRequiresUpper":
                        reportError("Password", "Passwords must have at least one uppercase('A'-'Z').");
                        break;
                    case "PasswordRequiresNonAlphanumeric":
                        reportError("Password", "Passwords must have at least one non letter or digit character.");
                        break;


                    // CurrentPassword
                    case "PasswordMismatch":
                        reportError("CurrentPassword", "Incorrect password.");
                        break;


                }
            }
        }
    }
}
