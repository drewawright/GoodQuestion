namespace GoodQuestion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Playlist",
                c => new
                    {
                        PlaylistId = c.String(nullable: false, maxLength: 128),
                        OwnerId = c.String(),
                        AppUserId = c.Guid(nullable: false),
                        PlaylistName = c.String(),
                        TracksUrl = c.String(),
                        ImageUrl = c.String(),
                        HasSongs = c.Boolean(nullable: false),
                        LastRefreshed = c.DateTime(nullable: false),
                        LastSyncedWithSpotify = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PlaylistId);
            
            CreateTable(
                "dbo.Song",
                c => new
                    {
                        SongId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Artists = c.String(),
                        ImageUrl = c.String(),
                        PlayerUrl = c.String(),
                        DurationMs = c.Int(nullable: false),
                        HasAudioFeatures = c.Boolean(nullable: false),
                        LastRefreshed = c.DateTime(nullable: false),
                        Danceability = c.Single(nullable: false),
                        Energy = c.Single(nullable: false),
                        Key = c.Int(nullable: false),
                        Loudness = c.Single(nullable: false),
                        Mode = c.Int(nullable: false),
                        Speechiness = c.Single(nullable: false),
                        Acousticness = c.Single(nullable: false),
                        Instrumentalness = c.Single(nullable: false),
                        Liveness = c.Single(nullable: false),
                        Valence = c.Single(nullable: false),
                        Tempo = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.SongId);
            
            CreateTable(
                "dbo.IdentityRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(),
                        IdentityRole_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.IdentityRole", t => t.IdentityRole_Id)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.IdentityRole_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.ApplicationUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        SpotifyUserId = c.String(),
                        SpotifyAuthToken = c.String(),
                        SpotifyRefreshToken = c.String(),
                        HasPlaylists = c.Boolean(nullable: false),
                        Danceability = c.Single(nullable: false),
                        Energy = c.Single(nullable: false),
                        Key = c.Int(nullable: false),
                        Loudness = c.Single(nullable: false),
                        Mode = c.Int(nullable: false),
                        Speechiness = c.Single(nullable: false),
                        Acousticness = c.Single(nullable: false),
                        Instrumentalness = c.Single(nullable: false),
                        Liveness = c.Single(nullable: false),
                        Valence = c.Single(nullable: false),
                        Tempo = c.Single(nullable: false),
                        Duration_ms = c.Int(nullable: false),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.IdentityUserLogin",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.PlaylistSong",
                c => new
                    {
                        PlaylistId = c.String(nullable: false, maxLength: 128),
                        SongId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.PlaylistId, t.SongId })
                .ForeignKey("dbo.Playlist", t => t.PlaylistId, cascadeDelete: true)
                .ForeignKey("dbo.Song", t => t.SongId, cascadeDelete: true)
                .Index(t => t.PlaylistId)
                .Index(t => t.SongId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IdentityUserRole", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserLogin", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserClaim", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserRole", "IdentityRole_Id", "dbo.IdentityRole");
            DropForeignKey("dbo.PlaylistSong", "SongId", "dbo.Song");
            DropForeignKey("dbo.PlaylistSong", "PlaylistId", "dbo.Playlist");
            DropIndex("dbo.PlaylistSong", new[] { "SongId" });
            DropIndex("dbo.PlaylistSong", new[] { "PlaylistId" });
            DropIndex("dbo.IdentityUserLogin", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserClaim", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "IdentityRole_Id" });
            DropTable("dbo.PlaylistSong");
            DropTable("dbo.IdentityUserLogin");
            DropTable("dbo.IdentityUserClaim");
            DropTable("dbo.ApplicationUser");
            DropTable("dbo.IdentityUserRole");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.Song");
            DropTable("dbo.Playlist");
        }
    }
}
