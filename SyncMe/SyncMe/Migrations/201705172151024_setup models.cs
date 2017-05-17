namespace SyncMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setupmodels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Calendars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.String(),
                        ZipCode = c.String(maxLength: 5),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        StartTime = c.String(),
                        EndTime = c.String(),
                        Details = c.String(),
                        Private = c.Boolean(nullable: false),
                        Calendar_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Calendars", t => t.Calendar_Id)
                .Index(t => t.Calendar_Id);
            
            CreateTable(
                "dbo.ContactRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateSent = c.DateTime(nullable: false),
                        Status = c.String(nullable: false),
                        Member_Id = c.Int(),
                        Reciever_Id = c.Int(),
                        Sender_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .ForeignKey("dbo.Members", t => t.Reciever_Id)
                .ForeignKey("dbo.Members", t => t.Sender_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Reciever_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Age = c.Int(),
                        StreetAddress = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        ZipCode = c.String(nullable: false, maxLength: 5),
                        Phone = c.String(maxLength: 10),
                        Email = c.String(),
                        Calendar_Id = c.Int(),
                        Profile_Id = c.Int(),
                        UserId_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Calendars", t => t.Calendar_Id)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId_Id)
                .Index(t => t.Calendar_Id)
                .Index(t => t.Profile_Id)
                .Index(t => t.UserId_Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfilePictureId = c.String(),
                        CompanyName = c.String(),
                        SchoolName = c.String(),
                        Phone = c.String(maxLength: 10),
                        Email = c.String(),
                        Member_Id = c.Int(),
                        Member_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .ForeignKey("dbo.Members", t => t.Member_Id1)
                .Index(t => t.Member_Id)
                .Index(t => t.Member_Id1);
            
            CreateTable(
                "dbo.EventInvitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Status = c.String(nullable: false),
                        Event_Id = c.Int(),
                        Sender_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .ForeignKey("dbo.Members", t => t.Sender_Id)
                .Index(t => t.Event_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.SyncRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateSent = c.DateTime(nullable: false),
                        Status = c.String(nullable: false),
                        Reciever_Id = c.Int(),
                        Sender_Id = c.Int(),
                        Member_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Reciever_Id)
                .ForeignKey("dbo.Members", t => t.Sender_Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .Index(t => t.Reciever_Id)
                .Index(t => t.Sender_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ContactRequests", "Sender_Id", "dbo.Members");
            DropForeignKey("dbo.ContactRequests", "Reciever_Id", "dbo.Members");
            DropForeignKey("dbo.Members", "UserId_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SyncRequests", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.SyncRequests", "Sender_Id", "dbo.Members");
            DropForeignKey("dbo.SyncRequests", "Reciever_Id", "dbo.Members");
            DropForeignKey("dbo.Members", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.EventInvitations", "Sender_Id", "dbo.Members");
            DropForeignKey("dbo.EventInvitations", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.Profiles", "Member_Id1", "dbo.Members");
            DropForeignKey("dbo.Profiles", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.ContactRequests", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.Members", "Calendar_Id", "dbo.Calendars");
            DropForeignKey("dbo.Events", "Calendar_Id", "dbo.Calendars");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.SyncRequests", new[] { "Member_Id" });
            DropIndex("dbo.SyncRequests", new[] { "Sender_Id" });
            DropIndex("dbo.SyncRequests", new[] { "Reciever_Id" });
            DropIndex("dbo.EventInvitations", new[] { "Sender_Id" });
            DropIndex("dbo.EventInvitations", new[] { "Event_Id" });
            DropIndex("dbo.Profiles", new[] { "Member_Id1" });
            DropIndex("dbo.Profiles", new[] { "Member_Id" });
            DropIndex("dbo.Members", new[] { "UserId_Id" });
            DropIndex("dbo.Members", new[] { "Profile_Id" });
            DropIndex("dbo.Members", new[] { "Calendar_Id" });
            DropIndex("dbo.ContactRequests", new[] { "Sender_Id" });
            DropIndex("dbo.ContactRequests", new[] { "Reciever_Id" });
            DropIndex("dbo.ContactRequests", new[] { "Member_Id" });
            DropIndex("dbo.Events", new[] { "Calendar_Id" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SyncRequests");
            DropTable("dbo.EventInvitations");
            DropTable("dbo.Profiles");
            DropTable("dbo.Members");
            DropTable("dbo.ContactRequests");
            DropTable("dbo.Events");
            DropTable("dbo.Calendars");
        }
    }
}
