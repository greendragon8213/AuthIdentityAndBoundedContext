using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using API.Models;
using AutoMapper;
using BLL.Interfaces;
using BLL.Models;

namespace API.Controllers
{
    public class AccountController : ApiController
    {
        private readonly IMapper _mapper = Mapper.Mapper.GetMapperInstance;
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Route("Register")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(UserForRegisterVM user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _accountService.RegisterAsync(_mapper.Map<UserForRegisterVM, UserForRegisterDTO>(user));
            await _accountService.SaveChangesAsync();
            
            return Ok();
        }
        
        [Route("LogOut")]
        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> LogOut([FromBody]string refreshToken)
        {
            await _accountService.LogOutAsync(refreshToken);
            await _accountService.SaveChangesAsync();

            return Ok();
        }

        #region Password

        [Route("ForgotPassword")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ForgotPassword(string userName)
        {
            //we need to send requests like that because there is problem with external and internal IP
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + Request.GetRequestContext().VirtualPathRoot;
            
            await _accountService.SendEmailToRenewPasswordAsync(userName, baseUrl);

            return Ok();
        }

        [Route("ChangePassword")]
        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword([FromBody]ChangePasswordVM changePassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _accountService.ChangePasswordAsync(HttpContext.Current.User?.Identity.Name, 
                changePassword.OldPassword, changePassword.NewPassword);
            await _accountService.SaveChangesAsync();

            return Ok();
        }

        [Route("ResetPassword")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ResetPasswordByToken(ResetPasswordVM resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _accountService.ResetPasswordAsync(resetPassword.UserId, resetPassword.Code, resetPassword.NewPassword);
            await _accountService.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}
