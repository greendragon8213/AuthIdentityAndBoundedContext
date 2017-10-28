using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Http;
using API.Providers;
using BLL.Exceptions;
using DI;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Owin;

//Чтобы приложение знало, где у нас находится класс OWIN Startup
//Теперь оно знает, что таковым классом является API.Startup.
[assembly: OwinStartup(typeof(API.Startup))]
namespace API
{
    public class Startup
    {
        //Этот класс содержит один метод Configuration, который в качестве параметра принимает интерфейс IAppBuilder. 
        //Этот интерфейс и является тем промежуточным слоем middleware, который участвует в обработке запросов.
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);

            GlobalConfiguration.Configuration.DependencyResolver = config.DependencyResolver;

            var container = DependencyContainer.CreateContainer(Assembly.GetExecutingAssembly());
            app.UseAutofacMiddleware(container);

            ConfigureOAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            
            GlobalConfiguration.Configure(WebApiConfig.Register);

        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //Authorization exception handling
            app.Use(async (c, n) =>
            {
                //check if the request was for the token endpoint
                if (c.Request.Path == new PathString("/token"))
                {
                    var buffer = new MemoryStream();
                    var body = c.Response.Body;
                    c.Response.Body = buffer; // we'll buffer the response, so we may change it if needed

                    await n.Invoke(); //invoke next middleware (auth)
                    
                    //smth
                    if (c.Get<bool>(nameof(UserDoesntHavePermissionException)))
                    {
                        var json = JsonConvert.SerializeObject(
                            new
                            {
                                error = "UserDoesntHavePermission",
                                error_description = "InactiveUserCannotLogIn",
                            });

                        var bytes = Encoding.UTF8.GetBytes(json);

                        buffer.SetLength(0); //change the buffer
                        buffer.Write(bytes, 0, bytes.Length);

                        //override the response headers
                        c.Response.StatusCode = 403;
                        c.Response.ContentType = "application/json";
                        c.Response.ContentLength = bytes.Length;
                    }

                    buffer.Position = 0; //reset position
                    await buffer.CopyToAsync(body); //copy to real response stream
                    c.Response.Body = body; //set again real stream to response body
                }
                else
                {
                    await n.Invoke(); //normal behavior
                }
            });

            OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),// /token
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                
                //We’ve specified the implementation on how to validate the credentials for users 
                //asking for tokens in custom class
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };
            
            // Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}