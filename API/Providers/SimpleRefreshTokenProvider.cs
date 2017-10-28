using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using BLL.Interfaces;
using Microsoft.Owin.Security.Infrastructure;

namespace API.Providers
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            //var refreshTokenService = (IRefreshTokenService)
            //        GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof (IRefreshTokenService));

            var autofacLifetimeScope = context.OwinContext.GetAutofacLifetimeScope();
            var refreshTokenService = autofacLifetimeScope.Resolve<IRefreshTokenService>();

            await refreshTokenService.CreateRefreshTokenAsync(context);
            await refreshTokenService.SaveChangesAsync();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            //var refreshTokenService = (IRefreshTokenService)
            //        GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IRefreshTokenService));
            var autofacLifetimeScope = context.OwinContext.GetAutofacLifetimeScope();
            var refreshTokenService = autofacLifetimeScope.Resolve<IRefreshTokenService>();

            await refreshTokenService.ReceiveRefreshTokenAsync(context);
            await refreshTokenService.SaveChangesAsync();
        }
    }
}