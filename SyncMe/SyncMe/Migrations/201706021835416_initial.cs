namespace SyncMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
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
                "dbo.ContactRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateSent = c.DateTime(nullable: false),
                        Status = c.String(nullable: false),
                        Receiver_Id = c.Int(),
                        Sender_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Receiver_Id)
                .ForeignKey("dbo.Profiles", t => t.Sender_Id)
                .Index(t => t.Receiver_Id)
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
                        UserId_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Calendars", t => t.Calendar_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId_Id)
                .Index(t => t.Calendar_Id)
                .Index(t => t.UserId_Id);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContactId = c.Int(),
                        FriendId = c.Int(),
                        ProfilePictureId = c.Binary(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Age = c.Int(),
                        State = c.String(),
                        CompanyName = c.String(),
                        SchoolName = c.String(),
                        Phone = c.String(maxLength: 10),
                        Email = c.String(),
                        Member_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.EventInvitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Status = c.String(nullable: false),
                        Event_Id = c.Int(),
                        Receiver_Id = c.Int(),
                        Sender_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .ForeignKey("dbo.Members", t => t.Receiver_Id)
                .ForeignKey("dbo.Profiles", t => t.Sender_Id)
                .Index(t => t.Event_Id)
                .Index(t => t.Receiver_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        streetAddress = c.String(),
                        city = c.String(),
                        state = c.String(),
                        zipCode = c.String(maxLength: 5),
                        start = c.DateTime(nullable: false),
                        end = c.DateTime(nullable: false),
                        startTime = c.DateTime(nullable: false),
                        endTime = c.DateTime(nullable: false),
                        details = c.String(),
                        isPrivate = c.Boolean(nullable: false),
                        Member_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfilePictureId = c.Binary(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Age = c.Int(),
                        State = c.String(),
                        CompanyName = c.String(),
                        SchoolName = c.String(),
                        Phone = c.String(maxLength: 10),
                        Email = c.String(),
                        Member_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.GroupCalendars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        RequestId = c.Int(nullable: false),
                        Invited = c.Int(nullable: false),
                        Creator_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.Creator_Id)
                .Index(t => t.Creator_Id);
            
            CreateTable(
                "dbo.GroupSyncRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Status = c.String(nullable: false),
                        Invited = c.Int(nullable: false),
                        Sender_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.Sender_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.SyncRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Status = c.String(nullable: false),
                        Receiver_Id = c.Int(),
                        Sender_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Receiver_Id)
                .ForeignKey("dbo.Profiles", t => t.Sender_Id)
                .Index(t => t.Receiver_Id)
                .Index(t => t.Sender_Id);
            
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
            
            CreateTable(
                "dbo.GroupCalendarMembers",
                c => new
                    {
                        GroupCalendar_Id = c.Int(nullable: false),
                        Member_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupCalendar_Id, t.Member_Id })
                .ForeignKey("dbo.GroupCalendars", t => t.GroupCalendar_Id, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.Member_Id, cascadeDelete: true)
                .Index(t => t.GroupCalendar_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.GroupSyncRequestMembers",
                c => new
                    {
                        GroupSyncRequest_Id = c.Int(nullable: false),
                        Member_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupSyncRequest_Id, t.Member_Id })
                .ForeignKey("dbo.GroupSyncRequests", t => t.GroupSyncRequest_Id, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.Member_Id, cascadeDelete: true)
                .Index(t => t.GroupSyncRequest_Id)
                .Index(t => t.Member_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ContactRequests", "Sender_Id", "dbo.Profiles");
            DropForeignKey("dbo.Members", "UserId_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SyncRequests", "Sender_Id", "dbo.Profiles");
            DropForeignKey("dbo.SyncRequests", "Receiver_Id", "dbo.Members");
            DropForeignKey("dbo.GroupSyncRequests", "Sender_Id", "dbo.Profiles");
            DropForeignKey("dbo.GroupSyncRequestMembers", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.GroupSyncRequestMembers", "GroupSyncRequest_Id", "dbo.GroupSyncRequests");
            DropForeignKey("dbo.GroupCalendarMembers", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.GroupCalendarMembers", "GroupCalendar_Id", "dbo.GroupCalendars");
            DropForeignKey("dbo.GroupCalendars", "Creator_Id", "dbo.Profiles");
            DropForeignKey("dbo.Events", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.EventInvitations", "Sender_Id", "dbo.Profiles");
            DropForeignKey("dbo.Profiles", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.EventInvitations", "Receiver_Id", "dbo.Members");
            DropForeignKey("dbo.EventInvitations", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.Contacts", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.ContactRequests", "Receiver_Id", "dbo.Members");
            DropForeignKey("dbo.Members", "Calendar_Id", "dbo.Calendars");
            DropIndex("dbo.GroupSyncRequestMembers", new[] { "Member_Id" });
            DropIndex("dbo.GroupSyncRequestMembers", new[] { "GroupSyncRequest_Id" });
            DropIndex("dbo.GroupCalendarMembers", new[] { "Member_Id" });
            DropIndex("dbo.GroupCalendarMembers", new[] { "GroupCalendar_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.SyncRequests", new[] { "Sender_Id" });
            DropIndex("dbo.SyncRequests", new[] { "Receiver_Id" });
            DropIndex("dbo.GroupSyncRequests", new[] { "Sender_Id" });
            DropIndex("dbo.GroupCalendars", new[] { "Creator_Id" });
            DropIndex("dbo.Profiles", new[] { "Member_Id" });
            DropIndex("dbo.Events", new[] { "Member_Id" });
            DropIndex("dbo.EventInvitations", new[] { "Sender_Id" });
            DropIndex("dbo.EventInvitations", new[] { "Receiver_Id" });
            DropIndex("dbo.EventInvitations", new[] { "Event_Id" });
            DropIndex("dbo.Contacts", new[] { "Member_Id" });
            DropIndex("dbo.Members", new[] { "UserId_Id" });
            DropIndex("dbo.Members", new[] { "Calendar_Id" });
            DropIndex("dbo.ContactRequests", new[] { "Sender_Id" });
            DropIndex("dbo.ContactRequests", new[] { "Receiver_Id" });
            DropTable("dbo.GroupSyncRequestMembers");
            DropTable("dbo.GroupCalendarMembers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SyncRequests");
            DropTable("dbo.GroupSyncRequests");
            DropTable("dbo.GroupCalendars");
            DropTable("dbo.Profiles");
            DropTable("dbo.Events");
            DropTable("dbo.EventInvitations");
            DropTable("dbo.Contacts");
            DropTable("dbo.Members");
            DropTable("dbo.ContactRequests");
            DropTable("dbo.Calendars");
        }
    }
}
