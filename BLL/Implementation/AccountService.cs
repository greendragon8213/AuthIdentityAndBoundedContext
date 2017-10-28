using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Models;
using Common;
using DAL.Interfaces;
using DL.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.OAuth;

namespace BLL.Implementation
{
    public class AccountService : BaseService, IAccountService
    {
        //private readonly IAuthRepository _authRepository;
        private readonly CustomUserManager _userManager;
        //private readonly IRepository<ApplicationUser> _applicationUserRepository;
        private readonly IRepository<CustomIdentityUser> _customIdentityUseRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly IRepository<CustomRole> _customRoleRepository;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper,
            CustomUserManager userManager,
            IRepository<CustomIdentityUser> customIdentityUseRepository,
            IRepository<Client> clientRepository,
            IRepository<RefreshToken> refreshTokenRepository,
            IRepository<CustomRole> customRoleRepository) 
            : base(unitOfWork, mapper)
        {
            _userManager = userManager;
            _customIdentityUseRepository = customIdentityUseRepository;
            _clientRepository = clientRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _customRoleRepository = customRoleRepository;
        }
        
        public async Task RegisterAsync(UserForRegisterDTO user)
        {
            ValidateModel(user);

            var customIdentityUser = Mapper.Map<UserForRegisterDTO, CustomIdentityUser>(user);
            //var applicationUserEntity = Mapper.Map<UserForRegisterDTO, ApplicationUser>(user);

            //userEntity.PasswordHash = Guid.NewGuid().ToString().Substring(0, 13);

            IdentityResult result = await _userManager.CreateAsync(customIdentityUser, customIdentityUser.Password);
            
            var errors = GetErrorResult(result);
            if (errors?.Count > 0)
                throw new ValidationFailedException(errors.Aggregate("", (current, v) => current + v + " "));

            //_applicationUserRepository.Create(applicationUserEntity);

            //foreach (var roleId in user.Claims)
            //{
            //    _customUserRoleRepository.Create(new CustomUserRole()
            //    {
            //        RoleId = roleId,
            //        UserId = userEntity.Id
            //    });
            //}

            //await _passwordManagementService.SendEmailToRenewPasswordAsync(userEntity.UserName, baseUrl);
        }

        public async Task ValidateClientAuthenticationAsync(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return;
            }

            var client = await _clientRepository.GetByIdAsync(context.ClientId);

            if (client == null)
            {
                context.SetError("invalid_clientId",
                    $"Client '{context.ClientId}' is not registered in the system.");
                return;
            }

            if (client.ApplicationType == ApplicationTypes.MobileClient)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return;
                }
                if (client.Secret != AuthHelper.GetHash(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret is invalid.");
                    return;
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return;
            }

            context.OwinContext.Set("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
        }
        
        public async Task LogOutAsync(string refreshToken)
        {
            var refreshTokenHash = AuthHelper.GetHash(refreshToken);
            var refreshTokenEntity = await _refreshTokenRepository.GetAll().FirstOrDefaultAsync(x => x.Id == refreshTokenHash);
            if (refreshTokenEntity == null)
            {
                throw new ObjectNotFoundException("Cannot find refresh token");
            }
            _refreshTokenRepository.Remove(refreshTokenEntity);
        }

        [Obsolete]
        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _customRoleRepository.GetAll().ToListAsync();
            return Mapper.Map<List<CustomRole>, List<RoleDTO>>(roles);
        }

        public async Task<UserPermissionDTO> GetUserPermissionsAsync(string userName, string password)
        {
            return Mapper.Map<CustomIdentityUser, UserPermissionDTO>(await _userManager.FindAsync(userName, password));
        }

        public async Task<UserPermissionDTO> GetUserPermissionsAsync(string userName)
        {
            return Mapper.Map<CustomIdentityUser, UserPermissionDTO>(await _userManager.FindByNameAsync(userName));
        }

        #region Password

        public async Task<string> GeneratePasswordResetTokenAsync(int userId)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(userId);
        }

        public async Task ResetPasswordAsync(int userId, string token, string newPassword)
        {
            //var user = await _userManager.FindByNameAsync(userName);
            var user = await _customIdentityUseRepository.GetByIdAsync(userId);

            //var r = _userManager.ResetPassword(userId, code, newPassword);//doesn't work
            if (user == null)
            {
                throw new NullReferenceException("Usre with such Id not found.");
            }

            // Make sure the token is valid and the stamp matches.
            if (!await _userManager.UserTokenProvider.ValidateAsync("ResetPassword", token, _userManager, user).ConfigureAwait(false))
            {
                throw new ValidationException("Invalid token.");
            }

            // Make sure the new password is valid.
            var result = await _userManager.PasswordValidator.ValidateAsync(newPassword)
                .ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new ValidationException("New password is invalid.");
            }

            // Update the password hash and invalidate the current security stamp.
            user.Password = /*user.ConfirmPassword =*/ newPassword;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(newPassword);
            user.SecurityStamp = Guid.NewGuid().ToString();

            _customIdentityUseRepository.Update(user);

            // Save the user and return the outcome.
            //await _userManager.UpdateAsync(user);//doesn't work
        }

        public async Task ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            //var user = await _customIdentityUseRepository.GetByIdAsync(userId);

            //await _userManager.ChangePasswordAsync(userId, oldPassword, newPassword);//doesn't work
            if (user == null)
            {
                throw new NullReferenceException("User not found.");
            }

            //if (_userManager.PasswordHasher.HashPassword(oldPassword) != user.PasswordHash)//doesn't work!
            if (_userManager.PasswordHasher.VerifyHashedPassword(user.Password, oldPassword) != PasswordVerificationResult.Success)
            {
                throw new ValidationException("Wrong old password.");
            }

            // Make sure the new password is valid.
            var result = await _userManager.PasswordValidator.ValidateAsync(newPassword).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new ValidationException("New password is invalid.");
            }

            // Update the password hash and invalidate the current security stamp.
            //user.Password = user.ConfirmPassword = newPassword;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(newPassword);
            user.SecurityStamp = Guid.NewGuid().ToString();

            _customIdentityUseRepository.Update(user);

            // Save the user and return the outcome.
            //await _userManager.UpdateAsync(user).ConfigureAwait(false);//doesn't work
        }

        public Task SendEmailToRenewPasswordAsync(string userName, string baseUrl)
        {
            throw new NotImplementedException();
        }

        #endregion

        private List<string> GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                throw new Exception();
            }

            return result.Succeeded ? null : result.Errors?.ToList();
        }
    }
}
