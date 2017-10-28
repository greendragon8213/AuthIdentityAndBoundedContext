using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Security.OAuth;
using BLL.Interfaces;
using BLL.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Autofac.Integration.Owin;

namespace API.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //using (var scope = GlobalConfiguration.Configuration.DependencyResolver.BeginScope())
            //{
            //    var accountService = scope.GetService(typeof (IAccountService)) as IAccountService;

            var autofacLifetimeScope = context.OwinContext.GetAutofacLifetimeScope();
            var accountService = autofacLifetimeScope.Resolve<IAccountService>();

            //var accountService = (IAccountService)
            //        GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAccountService));

            await accountService.ValidateClientAuthenticationAsync(context);
            
            //}
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            UserPermissionDTO user;
            try
            {
                var autofacLifetimeScope = context.OwinContext.GetAutofacLifetimeScope();
                var accountService = autofacLifetimeScope.Resolve<IAccountService>();
                //var accountService = (IAccountService)
                //    GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAccountService));
                user = await accountService.GetUserPermissionsAsync(context.UserName, context.Password);
            }
            catch (NullReferenceException)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (user.IsDeleted)
            {
                context.OwinContext.Set("UserDoesntHavePermissionException", true);
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("sub", context.UserName));

            foreach (var role in user.Roles)
            {
                identity.AddClaim(new Claim("role", role.Name));
            }

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {
                    "as:client_id", context.ClientId ?? string.Empty
                },
                {
                    "userName", context.UserName
                    //"userId", user.Id.ToString()
                }
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
            
            ClaimsIdentity cookiesIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override async Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            var autofacLifetimeScope = context.OwinContext.GetAutofacLifetimeScope();
            var accountService = autofacLifetimeScope.Resolve<IAccountService>();
            //IAccountService userPermissionsService = (IAccountService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAccountService));
            var userPermissions = await accountService.GetUserPermissionsAsync(context.Identity.Name);
            if (userPermissions.IsDeleted)
            {
                context.OwinContext.Set("UserDoesntHavePermissionException", true);
                return;
            }

            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            //Todo update sync date
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }
    }
}