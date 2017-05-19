namespace SyncMe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedtodatetime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "startTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Events", "endTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "endTime", c => c.String());
            AlterColumn("dbo.Events", "startTime", c => c.String());
        }
    }
}
