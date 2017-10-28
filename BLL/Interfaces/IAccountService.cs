using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Models;
using Microsoft.Owin.Security.OAuth;

namespace BLL.Interfaces
{
    public interface IAccountService : IBaseService
    {
        Task RegisterAsync(UserForRegisterDTO user);
        Task ValidateClientAuthenticationAsync(OAuthValidateClientAuthenticationContext context);
        Task LogOutAsync(string refreshToken);
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<UserPermissionDTO> GetUserPermissionsAsync(string userName, string password);
        Task<UserPermissionDTO> GetUserPermissionsAsync(string userName);

        Task<string> GeneratePasswordResetTokenAsync(int userId);
        Task ResetPasswordAsync(int userId, string token, string newPassword);
        Task ChangePasswordAsync(string userName, string oldPassword, string newPassword);
        Task SendEmailToRenewPasswordAsync(string userName, string baseUrl);
    }
}
