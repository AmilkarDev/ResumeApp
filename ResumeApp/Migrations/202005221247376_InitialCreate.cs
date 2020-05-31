namespace ResumeApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.KnowledgeBases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CvFileId = c.Int(nullable: false),
                        FullName = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                        Nationality = c.String(),
                        Validated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CvFiles", t => t.CvFileId, cascadeDelete: true)
                .Index(t => t.CvFileId);
            
            CreateTable(
                "dbo.CvFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fileName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Entities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Type = c.String(),
                        Subtype = c.String(),
                        Offset = c.Int(nullable: false),
                        Length = c.Int(nullable: false),
                        Score = c.Double(nullable: false),
                        Profile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.KeyPhrases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Profile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Titles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tools",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        PostalCode = c.String(),
                        PhotoLink = c.String(),
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
                "dbo.KnowledgeBaseCountries",
                c => new
                    {
                        KnowledgeBase_Id = c.Int(nullable: false),
                        Country_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.KnowledgeBase_Id, t.Country_Id })
                .ForeignKey("dbo.KnowledgeBases", t => t.KnowledgeBase_Id, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.Country_Id, cascadeDelete: true)
                .Index(t => t.KnowledgeBase_Id)
                .Index(t => t.Country_Id);
            
            CreateTable(
                "dbo.LanguageKnowledgeBases",
                c => new
                    {
                        Language_Id = c.Int(nullable: false),
                        KnowledgeBase_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Language_Id, t.KnowledgeBase_Id })
                .ForeignKey("dbo.Languages", t => t.Language_Id, cascadeDelete: true)
                .ForeignKey("dbo.KnowledgeBases", t => t.KnowledgeBase_Id, cascadeDelete: true)
                .Index(t => t.Language_Id)
                .Index(t => t.KnowledgeBase_Id);
            
            CreateTable(
                "dbo.ProfileCountries",
                c => new
                    {
                        Profile_Id = c.Int(nullable: false),
                        Country_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Profile_Id, t.Country_Id })
                .ForeignKey("dbo.Profiles", t => t.Profile_Id, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.Country_Id, cascadeDelete: true)
                .Index(t => t.Profile_Id)
                .Index(t => t.Country_Id);
            
            CreateTable(
                "dbo.ProfileLanguages",
                c => new
                    {
                        Profile_Id = c.Int(nullable: false),
                        Language_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Profile_Id, t.Language_Id })
                .ForeignKey("dbo.Profiles", t => t.Profile_Id, cascadeDelete: true)
                .ForeignKey("dbo.Languages", t => t.Language_Id, cascadeDelete: true)
                .Index(t => t.Profile_Id)
                .Index(t => t.Language_Id);
            
            CreateTable(
                "dbo.SkillKnowledgeBases",
                c => new
                    {
                        Skill_Id = c.Int(nullable: false),
                        KnowledgeBase_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Skill_Id, t.KnowledgeBase_Id })
                .ForeignKey("dbo.Skills", t => t.Skill_Id, cascadeDelete: true)
                .ForeignKey("dbo.KnowledgeBases", t => t.KnowledgeBase_Id, cascadeDelete: true)
                .Index(t => t.Skill_Id)
                .Index(t => t.KnowledgeBase_Id);
            
            CreateTable(
                "dbo.SkillProfiles",
                c => new
                    {
                        Skill_Id = c.Int(nullable: false),
                        Profile_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Skill_Id, t.Profile_Id })
                .ForeignKey("dbo.Skills", t => t.Skill_Id, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id, cascadeDelete: true)
                .Index(t => t.Skill_Id)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.TitleKnowledgeBases",
                c => new
                    {
                        Title_Id = c.Int(nullable: false),
                        KnowledgeBase_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Title_Id, t.KnowledgeBase_Id })
                .ForeignKey("dbo.Titles", t => t.Title_Id, cascadeDelete: true)
                .ForeignKey("dbo.KnowledgeBases", t => t.KnowledgeBase_Id, cascadeDelete: true)
                .Index(t => t.Title_Id)
                .Index(t => t.KnowledgeBase_Id);
            
            CreateTable(
                "dbo.TitleProfiles",
                c => new
                    {
                        Title_Id = c.Int(nullable: false),
                        Profile_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Title_Id, t.Profile_Id })
                .ForeignKey("dbo.Titles", t => t.Title_Id, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id, cascadeDelete: true)
                .Index(t => t.Title_Id)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.ToolKnowledgeBases",
                c => new
                    {
                        Tool_Id = c.Int(nullable: false),
                        KnowledgeBase_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tool_Id, t.KnowledgeBase_Id })
                .ForeignKey("dbo.Tools", t => t.Tool_Id, cascadeDelete: true)
                .ForeignKey("dbo.KnowledgeBases", t => t.KnowledgeBase_Id, cascadeDelete: true)
                .Index(t => t.Tool_Id)
                .Index(t => t.KnowledgeBase_Id);
            
            CreateTable(
                "dbo.ToolProfiles",
                c => new
                    {
                        Tool_Id = c.Int(nullable: false),
                        Profile_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tool_Id, t.Profile_Id })
                .ForeignKey("dbo.Tools", t => t.Tool_Id, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id, cascadeDelete: true)
                .Index(t => t.Tool_Id)
                .Index(t => t.Profile_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ToolProfiles", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.ToolProfiles", "Tool_Id", "dbo.Tools");
            DropForeignKey("dbo.ToolKnowledgeBases", "KnowledgeBase_Id", "dbo.KnowledgeBases");
            DropForeignKey("dbo.ToolKnowledgeBases", "Tool_Id", "dbo.Tools");
            DropForeignKey("dbo.TitleProfiles", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.TitleProfiles", "Title_Id", "dbo.Titles");
            DropForeignKey("dbo.TitleKnowledgeBases", "KnowledgeBase_Id", "dbo.KnowledgeBases");
            DropForeignKey("dbo.TitleKnowledgeBases", "Title_Id", "dbo.Titles");
            DropForeignKey("dbo.SkillProfiles", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.SkillProfiles", "Skill_Id", "dbo.Skills");
            DropForeignKey("dbo.SkillKnowledgeBases", "KnowledgeBase_Id", "dbo.KnowledgeBases");
            DropForeignKey("dbo.SkillKnowledgeBases", "Skill_Id", "dbo.Skills");
            DropForeignKey("dbo.ProfileLanguages", "Language_Id", "dbo.Languages");
            DropForeignKey("dbo.ProfileLanguages", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.KeyPhrases", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.Entities", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.Profiles", "CvFileId", "dbo.CvFiles");
            DropForeignKey("dbo.ProfileCountries", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.ProfileCountries", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.LanguageKnowledgeBases", "KnowledgeBase_Id", "dbo.KnowledgeBases");
            DropForeignKey("dbo.LanguageKnowledgeBases", "Language_Id", "dbo.Languages");
            DropForeignKey("dbo.KnowledgeBaseCountries", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.KnowledgeBaseCountries", "KnowledgeBase_Id", "dbo.KnowledgeBases");
            DropIndex("dbo.ToolProfiles", new[] { "Profile_Id" });
            DropIndex("dbo.ToolProfiles", new[] { "Tool_Id" });
            DropIndex("dbo.ToolKnowledgeBases", new[] { "KnowledgeBase_Id" });
            DropIndex("dbo.ToolKnowledgeBases", new[] { "Tool_Id" });
            DropIndex("dbo.TitleProfiles", new[] { "Profile_Id" });
            DropIndex("dbo.TitleProfiles", new[] { "Title_Id" });
            DropIndex("dbo.TitleKnowledgeBases", new[] { "KnowledgeBase_Id" });
            DropIndex("dbo.TitleKnowledgeBases", new[] { "Title_Id" });
            DropIndex("dbo.SkillProfiles", new[] { "Profile_Id" });
            DropIndex("dbo.SkillProfiles", new[] { "Skill_Id" });
            DropIndex("dbo.SkillKnowledgeBases", new[] { "KnowledgeBase_Id" });
            DropIndex("dbo.SkillKnowledgeBases", new[] { "Skill_Id" });
            DropIndex("dbo.ProfileLanguages", new[] { "Language_Id" });
            DropIndex("dbo.ProfileLanguages", new[] { "Profile_Id" });
            DropIndex("dbo.ProfileCountries", new[] { "Country_Id" });
            DropIndex("dbo.ProfileCountries", new[] { "Profile_Id" });
            DropIndex("dbo.LanguageKnowledgeBases", new[] { "KnowledgeBase_Id" });
            DropIndex("dbo.LanguageKnowledgeBases", new[] { "Language_Id" });
            DropIndex("dbo.KnowledgeBaseCountries", new[] { "Country_Id" });
            DropIndex("dbo.KnowledgeBaseCountries", new[] { "KnowledgeBase_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.KeyPhrases", new[] { "Profile_Id" });
            DropIndex("dbo.Entities", new[] { "Profile_Id" });
            DropIndex("dbo.Profiles", new[] { "CvFileId" });
            DropTable("dbo.ToolProfiles");
            DropTable("dbo.ToolKnowledgeBases");
            DropTable("dbo.TitleProfiles");
            DropTable("dbo.TitleKnowledgeBases");
            DropTable("dbo.SkillProfiles");
            DropTable("dbo.SkillKnowledgeBases");
            DropTable("dbo.ProfileLanguages");
            DropTable("dbo.ProfileCountries");
            DropTable("dbo.LanguageKnowledgeBases");
            DropTable("dbo.KnowledgeBaseCountries");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Tools");
            DropTable("dbo.Titles");
            DropTable("dbo.Skills");
            DropTable("dbo.KeyPhrases");
            DropTable("dbo.Entities");
            DropTable("dbo.CvFiles");
            DropTable("dbo.Profiles");
            DropTable("dbo.Languages");
            DropTable("dbo.KnowledgeBases");
            DropTable("dbo.Countries");
        }
    }
}
