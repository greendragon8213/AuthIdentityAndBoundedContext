using System;
using System.Data.Entity;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using Common;
using DAL.Interfaces;
using DL.Identity;
using Microsoft.Owin.Security.Infrastructure;

namespace BLL.Implementation
{
    public class RefreshTokenService : BaseService, IRefreshTokenService
    {
        private readonly IRepository<RefreshToken> _refreshTokenRepository;

        public RefreshTokenService(IUnitOfWork unitOfWork, IMapper mapper,
            IRepository<RefreshToken> refreshTokenRepository)
            :base(unitOfWork, mapper)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task CreateRefreshTokenAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");


            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken()
            {
                Id = AuthHelper.GetHash(refreshTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            await AddRefreshTokenAsync(token);
            
            context.SetToken(refreshTokenId);
        }

        public async Task ReceiveRefreshTokenAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var hashedTokenId = AuthHelper.GetHash(context.Token);

            var refreshToken = await _refreshTokenRepository.GetByIdAsync(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                await RemoveRefreshTokenAsync(hashedTokenId);
            }
        }

        #region Private methods

        private async Task AddRefreshTokenAsync(RefreshToken token)
        {
            var existingToken = await _refreshTokenRepository.GetAll()
                .SingleOrDefaultAsync(r => r.Subject == token.Subject && r.ClientId == token.ClientId);

            if (existingToken != null)
            {
                _refreshTokenRepository.Remove(existingToken);
            }

            _refreshTokenRepository.Create(token);
        }

        private async Task RemoveRefreshTokenAsync(string refreshTokenId)
        {
            var refreshToken = await _refreshTokenRepository.GetByIdAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _refreshTokenRepository.Remove(refreshToken);
            }
        }

        #endregion

    }
}
