namespace GoodQuestion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedJoiningTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPlaylist",
                c => new
                    {
                        AppUserId = c.String(nullable: false, maxLength: 128),
                        PlaylistId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.AppUserId, t.PlaylistId })
                .ForeignKey("dbo.ApplicationUser", t => t.AppUserId, cascadeDelete: true)
                .ForeignKey("dbo.Playlist", t => t.PlaylistId, cascadeDelete: true)
                .Index(t => t.AppUserId)
                .Index(t => t.PlaylistId);
            
            DropColumn("dbo.Playlist", "AppUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Playlist", "AppUserId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.UserPlaylist", "PlaylistId", "dbo.Playlist");
            DropForeignKey("dbo.UserPlaylist", "AppUserId", "dbo.ApplicationUser");
            DropIndex("dbo.UserPlaylist", new[] { "PlaylistId" });
            DropIndex("dbo.UserPlaylist", new[] { "AppUserId" });
            DropTable("dbo.UserPlaylist");
        }
    }
}
