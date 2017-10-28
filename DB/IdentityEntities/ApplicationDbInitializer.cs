using System.Collections.Generic;
using System.Data.Entity;
using Common;

namespace DB.IdentityEntities
{
    internal class ApplicationDbInitializer : CreateDatabaseIfNotExists<FullDbContext>
    {
        protected override void Seed(FullDbContext context)
        {
            #region Clients

            IList<Client> defaultClients = new List<Client>();

            defaultClients.Add(new Client
            {
                Active = true,
                AllowedOrigin = "*",
                ApplicationType = ApplicationTypes.WebClient,
                Id = "WebClient",
                Name = "Web Client",
                RefreshTokenLifeTime = 43200,
                Secret = AuthHelper.GetHash("secretForWebClientHere"),

            });
            defaultClients.Add(new Client
            {
                Active = true,
                AllowedOrigin = "*",
                ApplicationType = ApplicationTypes.MobileClient,
                Id = "MobileApp",
                Name = "Mobile Application",
                RefreshTokenLifeTime = 43200,
                Secret = AuthHelper.GetHash("MobileApp"),

            });

            foreach (Client app in defaultClients)
                context.Clients.Add(app);

            #endregion

            #region Roles

            context.Roles.Add(new CustomRole("Робити то"));
            context.Roles.Add(new CustomRole("Робити сьо"));

            #endregion
            
            base.Seed(context);
            context.SaveChanges();
        }
    }
}