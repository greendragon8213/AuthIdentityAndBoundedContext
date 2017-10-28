using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace DL.Identity
{
    public class CustomUserManager : UserManager<CustomIdentityUser, int>
    {
        public CustomUserManager(IUserStore<CustomIdentityUser, int> store)
            : base(store)
        {
            var provider = new DpapiDataProtectionProvider("AuthIdentityTest");
            this.UserTokenProvider = new DataProtectorTokenProvider<CustomIdentityUser, int>(provider.Create("EmailConfirmation"));
        }

        public static CustomUserManager New(IdentityFactoryOptions<CustomUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<IdentityContext>();
            var appUserManager = new CustomUserManager(new CustomUserStore(appDbContext));

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider = new DataProtectorTokenProvider<CustomIdentityUser, int>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Code for email confirmation and reset password life time
                    TokenLifespan = TimeSpan.FromHours(3)
                };
            }

            return appUserManager;
        }

        //public static ApplicationUserManager Create(
        //    IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        //{
        //    var manager = new ApplicationUserManager(
        //        new CustomUserStore(context.Get<StoreDBModels>()));
        //    // Configure validation logic for usernames 
        //    manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
        //    {
        //        AllowOnlyAlphanumericUserNames = false,
        //        RequireUniqueEmail = true
        //    };
        //    // Configure validation logic for passwords 
        //    manager.PasswordValidator = new PasswordValidator
        //    {
        //        RequiredLength = 6,
        //        //RequireNonLetterOrDigit = true,
        //        //RequireDigit = true,
        //        //RequireLowercase = true,
        //        //RequireUppercase = true,
        //    };



        #region two Factor authentication
        // Register two factor authentication providers. This application uses Phone 
        // and Emails as a step of receiving a code for verifying the user 
        // You can write your own provider and plug in here. 


        //manager.RegisterTwoFactorProvider("PhoneCode",
        //    new PhoneNumberTokenProvider<ApplicationUser, int>
        //    {
        //        MessageFormat = "Your security code is: {0}"
        //    });
        //manager.RegisterTwoFactorProvider("EmailCode",
        //    new EmailTokenProvider<ApplicationUser, int>
        //    {
        //        Subject = "Security Code",
        //        BodyFormat = "Your security code is: {0}"
        //    });
        //manager.EmailService = new EmailService();
        //manager.SmsService = new SmsService();
        //var dataProtectionProvider = options.DataProtectionProvider;
        //if (dataProtectionProvider != null)
        //{
        //    manager.UserTokenProvider =
        //        new DataProtectorTokenProvider<ApplicationUser, int>(
        //            dataProtectionProvider.Create("ASP.NET Identity"));
        //}
        #endregion
        //    return manager;
        //}
    }
}
