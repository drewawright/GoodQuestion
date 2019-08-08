namespace GoodQuestion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedUserAudioFeatureCalculations : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ApplicationUser", "Danceability");
            DropColumn("dbo.ApplicationUser", "Energy");
            DropColumn("dbo.ApplicationUser", "Key");
            DropColumn("dbo.ApplicationUser", "Loudness");
            DropColumn("dbo.ApplicationUser", "Mode");
            DropColumn("dbo.ApplicationUser", "Speechiness");
            DropColumn("dbo.ApplicationUser", "Acousticness");
            DropColumn("dbo.ApplicationUser", "Instrumentalness");
            DropColumn("dbo.ApplicationUser", "Liveness");
            DropColumn("dbo.ApplicationUser", "Valence");
            DropColumn("dbo.ApplicationUser", "Tempo");
            DropColumn("dbo.ApplicationUser", "Duration_ms");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApplicationUser", "Duration_ms", c => c.Int(nullable: false));
            AddColumn("dbo.ApplicationUser", "Tempo", c => c.Single(nullable: false));
            AddColumn("dbo.ApplicationUser", "Valence", c => c.Single(nullable: false));
            AddColumn("dbo.ApplicationUser", "Liveness", c => c.Single(nullable: false));
            AddColumn("dbo.ApplicationUser", "Instrumentalness", c => c.Single(nullable: false));
            AddColumn("dbo.ApplicationUser", "Acousticness", c => c.Single(nullable: false));
            AddColumn("dbo.ApplicationUser", "Speechiness", c => c.Single(nullable: false));
            AddColumn("dbo.ApplicationUser", "Mode", c => c.Int(nullable: false));
            AddColumn("dbo.ApplicationUser", "Loudness", c => c.Single(nullable: false));
            AddColumn("dbo.ApplicationUser", "Key", c => c.Int(nullable: false));
            AddColumn("dbo.ApplicationUser", "Energy", c => c.Single(nullable: false));
            AddColumn("dbo.ApplicationUser", "Danceability", c => c.Single(nullable: false));
        }
    }
}
