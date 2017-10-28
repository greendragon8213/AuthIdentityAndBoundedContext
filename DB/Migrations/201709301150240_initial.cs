namespace DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        FirstName = c.String(maxLength: 20),
                        LastName = c.String(maxLength: 20),
                        UserImagePath = c.String(maxLength: 1024),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationUserToSomethings",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        SomethingId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.SomethingId })
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Something", t => t.SomethingId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.SomethingId);
            
            CreateTable(
                "dbo.Something",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PhoneNumber = c.String(maxLength: 100),
                        Email = c.String(maxLength: 255),
                        UserName = c.String(nullable: false, maxLength: 20),
                        IsDeleted = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        CustomIdentityUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUsers", t => t.CustomIdentityUser_Id)
                .Index(t => t.CustomIdentityUser_Id);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                        CustomIdentityUser_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.IdentityUsers", t => t.CustomIdentityUser_Id)
                .Index(t => t.CustomIdentityUser_Id);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        CustomIdentityUser_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.IdentityUsers", t => t.CustomIdentityUser_Id)
                .ForeignKey("dbo.CustomRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.CustomIdentityUser_Id);
            
            CreateTable(
                "dbo.CustomRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUsers", "Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.CustomRoles");
            DropForeignKey("dbo.UserRoles", "CustomIdentityUser_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.UserLogins", "CustomIdentityUser_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.CustomUserClaims", "CustomIdentityUser_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.ApplicationUserToSomethings", "SomethingId", "dbo.Something");
            DropForeignKey("dbo.ApplicationUserToSomethings", "UserId", "dbo.ApplicationUsers");
            DropIndex("dbo.UserRoles", new[] { "CustomIdentityUser_Id" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserLogins", new[] { "CustomIdentityUser_Id" });
            DropIndex("dbo.CustomUserClaims", new[] { "CustomIdentityUser_Id" });
            DropIndex("dbo.ApplicationUserToSomethings", new[] { "SomethingId" });
            DropIndex("dbo.ApplicationUserToSomethings", new[] { "UserId" });
            DropIndex("dbo.ApplicationUsers", new[] { "Id" });
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Clients");
            DropTable("dbo.CustomRoles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.UserLogins");
            DropTable("dbo.CustomUserClaims");
            DropTable("dbo.IdentityUsers");
            DropTable("dbo.Something");
            DropTable("dbo.ApplicationUserToSomethings");
            DropTable("dbo.ApplicationUsers");
        }
    }
}
