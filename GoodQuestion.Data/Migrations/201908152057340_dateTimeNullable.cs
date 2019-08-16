namespace GoodQuestion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateTimeNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ApplicationUser", "TokenExpiration", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ApplicationUser", "TokenExpiration", c => c.DateTime(nullable: false));
        }
    }
}
